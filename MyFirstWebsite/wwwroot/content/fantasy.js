$(function () {

    var url = window.location.pathname;
    var idStr = url.substring(url.lastIndexOf('/') + 1);
    var players = [];
    var draftedPlayers = [];
    var playersEdited = [];
    var numPlayers;
    var draftPosition;
    var pickCounter = 1;
    var round = 1;

    setUpEventListeners();
    hideDraftBoard();
    setUpRoundAndPickLabels(round, pickCounter);
    updateRoundAndPickText();
    getNumberOfDraftTeams(idStr);
    getDraftPosition(idStr);
    getAvailablePlayers(idStr);
    getDraftedPlayers(idStr);
    getUserTeam(idStr);

    function setUpEventListeners() {

        $(document).on('keyup', '#searchInput', function () {
            //searchBar();
            filterPlayerAndPosition($('#filterPositions option:selected').text());
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
                }
            });
        });

        $(document).on('click', '#draftButton', function () {
            hideDraftBoard();
           
        });

        $(document).on('click', '#editDraftButton', function () {
            editDraft();
        });

        $(document).on('click', '#hideShowTeamButton', function () {
            hideShowTeam();
        })

        $(document).on('change', '#filterPositions', function () {
            filterPlayerAndPosition($(this).children('option:selected').text());
        })
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

    function filterPlayerAndPosition(position) {
        var filter, table, tr, td, i, input;
        
        if ($('#filterPositions option:selected').val() == 0) {
            position = "";
        }

        input = document.getElementById("searchInput");
        filter = input.value.toUpperCase();
        table = document.getElementById("draftTable");
        tr = table.getElementsByTagName("tr");

        for (i = 0; i < tr.length; i++) {
            nameTd = tr[i].getElementsByTagName("td")[0];
            positionTd = tr[i].getElementsByTagName("td")[1];
            if (nameTd || positionTd) {
                if (positionTd.innerHTML.toUpperCase().includes(position) && nameTd.innerHTML.toUpperCase().indexOf(filter) > -1) {
                    console.log(filter + " " + position);
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
                        if (flexPosition.children().length == 1 && (position == 'WR' || position == 'RB' || position == 'TE')) {
                            flexPosition.first().append('<td>' + GetPlayerLink(v.name) + '</td>');
                        } else {
                            $('#teamTable tbody').append('<tr><td>BN</td><td>' + GetPlayerLink(v.name) + '</td></tr>');
                        }

                    } else if (player.length == 0) {
                        tableRow.first().append('<td>' + GetPlayerLink(v.name) + '</td>');
                    }
                });
                console.log('user team :');
                console.log(result);
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

    function getDraftedPlayers(draftId) {
        $.ajax({
            url: '/api/FantasyApi/GetDraftedPlayers/' + draftId,
            type: "GET",
            data: {},
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (result) {
                pickCounter = result.length + 1;
                updateRoundAndPickText();

                $.each(result, function (i, v) {
                    draftedPlayers.push({
                        id: v.id,
                        rank: v.rank,
                        name: v.name,
                        position: v.position,
                        positionDrafted: v.positionDrafted
                    });
                });
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
                playersDrafted = [];
                getDraftedPlayers(idStr);
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
        if (r % 2 == 0) {

            return (r * numPlayers - draftPick + 1) == numPicks;
        }
        return ((r - 1) * numPlayers + draftPick) == numPicks;

    }

    function hideDraftBoard() {
        $('#draftBoard').hide();
        $('#draftButton').parent().parent().hide();
        $('#draftContent').show();
        $('#draftBoardButton').parent().parent().show();
        $('#draftbutton').parent().parent().removeClass('col-md-3');
        $('#draftBoardButton').parent().addClass('col-md-3');
    }

    function hideDraft() {
        $('#draftContent').hide();
        $('#draftButton').parent().parent().show();
        $('#draftBoardButton').parent().parent().hide();
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
            editButton.text('Edit');
            $('.card').removeClass('edit-effect');
            removeCardClickEvent();
            swapDraftedPlayers(playersEdited, idStr);
        } else {
            editButton.removeClass('btn-primary').addClass('btn-danger active');
            draftButton.removeClass('btn-primary').addClass('disabled btn-secondary').prop('disabled', true);
            editButton.text('Save');
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

            if ($('.card-selected').length > 1) {
                swapCards();
            }
            
        });
    }

    function removeCardClickEvent() {
        $(document).off('click', '.card');
        $('.card').removeClass('card-selected');
    }

    function swapCards() {

        if ($('.card-selected').length > 1) {
            var card1 = $('.card-selected').eq(0);
            var card2 = $('.card-selected').eq(1);

            var card1NameElement = card1.children().children().get(0);
            var card1PositionElement = card1.children().children().eq(1).children().get(0);
            var card2NameElement = card2.children().children().get(0);
            var card2PositionElement = card2.children().children().eq(1).children().get(0);

            var name1 = card1NameElement.innerText.trim();
            var name2 = card2NameElement.innerText.trim();
            var position1 = card1PositionElement.innerText.trim();
            var position2 = card2PositionElement.innerText.trim();

            card1NameElement.innerText = name2;
            card1PositionElement.innerText = position2;
            card2NameElement.innerText = name1;
            card2PositionElement.innerText = position1;

            setupDraftBoardColors();
            $('.card').removeClass('card-selected');

            var indexesToRemove = [];

            $.each(playersEdited, function (i, v) {
     
                if (v.name.includes(name1) || v.name.includes(name2)) {
                    indexesToRemove.push(i);
                }
            });

            for (var i = indexesToRemove.length - 1; i >= 0; i--) {
                playersEdited.splice(indexesToRemove[i], 1);
            }

            var player1;
            var player2;

            $.each(draftedPlayers, function (i, v) {
                if (v.name.includes(name1)) {
                    console.log(v);
                    player1 = v;
                }
                else if (v.name.includes(name2)) {
                    console.log(v);
                    player2 = v;
                }
            });
            console.log(draftedPlayers);
            var temp = player1.positionDrafted
            player1.positionDrafted = player2.positionDrafted;
            player2.positionDrafted = temp;

            playersEdited.push(player1);
            playersEdited.push(player2);
        }
    }

    function swapDraftedPlayers(players, draftId) {
        
        $.ajax({
            url: '/api/FantasyApi/SwapDraftedPlayers/' + draftId,
            type: 'PUT',
            data: JSON.stringify(players),
            contentType: "application/json; charset=utf-8",
            processData: true,
            cache: false,
            success: function (result) {
                getUserTeam(idStr);
                $('#teamTable tbody tr').children('td:nth-child(2)').remove();
                $('#teamTable tbody tr').children('td:nth-child(2)')
                playersEdited = [];
            }
        });
    }

    function hideShowTeam() {
        if ($('#teamTable').is(':visible')) {
            $('#teamTable').hide();
            $('#draftTable').parent().removeClass('col-md-8').addClass('col-md-12');
            $('#hideShowTeamButton').text('Show Team');
        }
        else {
            console.log('hit');
            $('#draftTable').parent().removeClass('col-md-12').addClass('col-md-8');
            $('#teamTable').show();
            $('#hideShowTeamButton').text('Hide Team');
        }
    }


});