$(function () {

    var url = window.location.pathname;
    var idStr = url.substring(url.lastIndexOf('/') + 1);
    var players = [];
    var numPlayers;
    var draftPosition;
    var pickCounter = 1;
    var round = 1;

    hideDraftBoard();
    setUpRoundAndPickLabels(round, pickCounter);
    updateRoundAndPickText();
    getNumberOfDraftTeams(idStr);
    getDraftPosition(idStr);
    getAvailablePlayers(idStr);
    getUserTeam(idStr);
    getDraftedPlayers(idStr);
    updateRoundAndPickText();
    setUpEventListeners();

    function setUpEventListeners() {

        $(document).on('keyup', '#searchInput', function () {
            searchBar();
        });

        $(document).on('click', '#draftTable button', function () {
            var name = ($(this).closest('tr').children('td:first').text());
            var tr = $(this).closest('tr');
            var player;
            tr.remove();

            $.each(players, function (i, v) {
                if (v.name == name) {
                    player = v;
                }
            });

            removePlayer(idStr, player);

            updateRound();
            updateRoundAndPickText();

            $('#searchInput').val("");
            searchBar();
        });

        $(document).on('click', '#draftBoardButton', function () {

            $.ajax({
                url: '/api/FantasyApi/GetDraftBoard/' + idStr,
                type: "GET",
                //contentType: "application/json",
                //dataType: "html",
                success: function (result) {
                    hideDraft();
                    $('#draftBoard').html(result);
                    setupDraftBoardColors();
                },
                error: function (result) {
                    console.log(result);
                }
            });
        });

        $(document).on('click', '#draftButton', function () {
            hideDraftBoard();
           
        });

        $(document).on('click', '#editDraftButton', function () {
            editDraft();
        });
    }

    function setUpRoundAndPickLabels(round, pickCounter) {
        $('#roundPickGroup').append('<div class="col-md-6"><h4 id="round">Round: ' + round + '</h4></div><div class="col-md-6"><h4 id="pick">Pick: ' + pickCounter + '</h4></div>');
    }

    function getAvailablePlayers(draftId) {
        $.ajax({
            url: '/api/FantasyApi/GetAvailablePlayers/' + draftId,
            type: "GET",
            data: {},
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (result) {
                $.each(result, function (i, v) {
                    players.push({
                        id: v.id,
                        rank: v.rank,
                        name: v.name,
                        position: v.position
                    });
                    $('#draftTable tbody').append('<tr><th scope="row">' + v.rank + '</th>' +
                        '<td>' + GetPlayerLink(v.name) + '</td>' + '<td>' + v.position + '</td>' +
                        '<td><button class="btn btn-success">Draft</button></td>' + '</tr>');
                });
            },
            error: function (result) {

            }
        });
    }

    function formatName(pname) {
        var first = pname.indexOf(' ');
        var second = pname.indexOf(' ', first + 1);
        var newName = pname.substring(0, first) + "-" + pname.substring(first + 1, second);

        return newName.trim().toLowerCase().replace(/'/, '');
    }

    function searchBar() {

        var input, filter, table, tr, td, i;

        input = document.getElementById("searchInput");
        filter = input.value.toUpperCase();
        table = document.getElementById("draftTable");
        tr = table.getElementsByTagName("tr");

        for (i = 0; i < tr.length; i++) {
            td = tr[i].getElementsByTagName("td")[0];
            if (td) {
                if (td.innerHTML.toUpperCase().indexOf(filter) > -1) {
                    tr[i].style.display = "";
                } else {
                    tr[i].style.display = "none";
                }
            }
        }
    }

    function getUserTeam(draftId) {

        var tableRow = $('#teamTable tr:contains(position)');

        $.ajax({
            url: '/api/FantasyApi/GetUserTeam/' + draftId,
            type: 'GET',
            contentType: "application/json; charset=utf-8",
            processData: true,
            cache: false,
            success: function (result) {
                $.each(result, function (i, v) {

                    var position = v.position.replace(/[^a-z]/gi, '').toUpperCase();

                    var tableRow = $("td").filter(function () {
                        return $(this).text() == position && $(this).parent().children().length == 1;
                    }).closest("tr");

                    var flexPosition = $('td').filter(function () {
                        return $(this).text() == "flex".toUpperCase();
                    }).closest('tr');

                    var player = $('td').filter(function () {
                        return $(this).text() == v.name;
                    });

                    if (tableRow.length == 0 && player.length == 0) {
                        if (flexPosition.children().length == 1 && (position == 'QB' || position == 'RB' || position == 'TE')) {
                            flexPosition.first().append('<td>' + GetPlayerLink(v.name) + '</td>');
                        } else {
                            $('#teamTable tbody').append('<tr><td>BN</td><td>' + GetPlayerLink(v.name) + '</td></tr>');
                        }

                    } else if (player.length == 0) {
                        tableRow.first().append('<td>' + GetPlayerLink(v.name) + '</td>');
                    }
                });
            }
        });
    }

    function GetPlayerLink(name) {
        return '<a href="https://www.fantasypros.com/nfl/players/' + formatName(name) +
            '.php" target="_blank"' + '>' + name + '</a>';
    }

    function getNumberOfDraftTeams(draftId) {
        return $.ajax({
            url: '/api/FantasyApi/GetNumberOfTeams/' + draftId,
            type: 'GET',
            contentType: "application/json; charset=utf-8",
            processData: true,
            success: function (result) {
                numPlayers = result;
                updateRoundAndPickText();
            }
        }).responseJSON;
    }

    function getDraftPosition(draftId) {
        return $.ajax({
            url: '/api/FantasyApi/GetDraftPosition/' + draftId,
            type: 'GET',
            contentType: "application/json; charset=utf-8",
            processData: true,
            success: function (result) {
                draftPosition = result;
            }
        }).responseJSON;
    }

    function addPlayerToTeam(draftId, player) {
        $.ajax({
            url: '/api/FantasyApi/AddToTeam/' + draftId,
            type: 'PUT',
            data: JSON.stringify(player),
            contentType: "application/json; charset=utf-8",
            processData: true,
            cache: false,
            success: function (result) {
                getUserTeam(draftId);
            }
        });
    }

    function getDraftedPlayers(draftId) {
        $.ajax({
            url: '/api/FantasyApi/GetDraftedPlayers/' + draftId,
            type: "GET",
            data: {},
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (result) {
                pickCounter = result.length + 1;
                console.log('test: ' + pickCounter);
                updateRoundAndPickText();
            },
            error: function (result) {

            }
        });
    }

    function addPlayerToDrafted(draftId, player) {

        player.positionDrafted = pickCounter;

        $.ajax({
            url: '/api/FantasyApi/AddToDrafted/' + draftId,
            type: 'PUT',
            data: JSON.stringify(player),
            contentType: "application/json; charset=utf-8",
            processData: true,
            cache: false,
            success: function (result) {
                if (isTurn(round, numPlayers, draftPosition, pickCounter)) {
                    addPlayerToTeam(idStr, player);
                }
                pickCounter++;
                updateRoundAndPickText();
            }
        });
    }

    function removePlayer(draftId, player) {
        $.ajax({
            url: '/api/FantasyApi/RemovePlayer/' + draftId,
            type: 'DELETE',
            data: JSON.stringify(player),
            contentType: "application/json; charset=utf-8",
            processData: true,
            cache: false,
            success: function (result) {
                addPlayerToDrafted(idStr, player);
            }
        });
    }

    function updateRoundAndPickText() {
        $('#round').text("Round: " + round);
        $('#pick').text("Pick: " + pickCounter);
    }

    function updateRound() {

        round = Math.ceil(pickCounter / numPlayers);
        if (pickCounter == 0)
            round = 1;
    }

    function isTurn(r, numPlayers, draftPick, numPicks) {
        console.log("r: " + r + "numPicks: " + numPlayers + "draftPick: " + draftPick)
        if (r % 2 == 0) {
            console.log('even');
            console.log(r * numPlayers - draftPick + 1)
            return (r * numPlayers - draftPick + 1) == numPicks;
        }
        console.log('odd');
        console.log((r - 1) * numPlayers + draftPick);
        return ((r - 1) * numPlayers + draftPick) == numPicks;

    }

    function hideDraftBoard() {
        $('#draftBoard').hide();
        $('#draftButton').parent().parent().hide();
        $('#draftContent').show();
        $('#draftBoardButton').show();
        $('#draftbutton').parent().parent().removeClass('col-md-3');
        $('#draftBoardButton').parent().addClass('col-md-3');
    }

    function hideDraft() {
        $('#draftContent').hide();
        $('#draftButton').parent().parent().show();
        $('#draftBoardButton').hide();
        $('#draftBoard').show();
        $('#draftBoardButton').parent().removeClass('col-md-3');
        $('#draftbutton').parent().parent().addClass('col-md-3');  
    }

    function setupDraftBoardColors() {
        $.each($('.card-body footer'), function () {
            if ($(this).text().trim().startsWith('QB')) {
                $(this).parent().parent().css('background-color', '#ff3333');
            }
            else if ($(this).text().trim().startsWith('WR')) {
                $(this).parent().parent().css('background-color', '#8080ff');
            }
            else if ($(this).text().trim().startsWith('RB')) {
                $(this).parent().parent().css('background-color', '#33ff33');
            }
            else if ($(this).text().trim().startsWith('D')) {
                $(this).parent().parent().css('background-color', '#ffd700');
            }
            else if ($(this).text().trim().startsWith('TE')) {
                $(this).parent().parent().css('background-color', '#b84dff');
            }
            else if ($(this).text().trim().startsWith('K')) {
                $(this).parent().parent().css('background-color', '#00FFFF');
            }

        });
    }

    function editDraft() {
        var editButton = $('#editDraftButton');
        var draftButton = $('#draftButton');

        if (draftButton.prop('disabled')) {
            editButton.removeClass('btn-danger active').addClass('btn-primary');
            draftButton.removeClass('btn-secondary disabled').addClass('btn-primary').prop('disabled', false);
            $('.card').removeClass('edit-effect');
            removeCardClickEvent();
        } else {
            editButton.removeClass('btn-primary').addClass('btn-danger active');
            draftButton.removeClass('btn-primary').addClass('disabled btn-secondary').prop('disabled', true);
            $('.card').addClass('edit-effect');
            setupCardClickEvent();
        }
    }

    function setupCardClickEvent() {
        $(document).on('click', '.card', function () {
            if ($(this).hasClass('card-selected')) {
                $(this).removeClass('card-selected');
            }
            else
            {
                $(this).addClass('card-selected');
            }
            
        });
    }

    function removeCardClickEvent() {
        $(document).off('click', '.card');
        $('.card').removeClass('card-selected');
    }


});