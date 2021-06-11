$(function () {

    var url = window.location.pathname;
    var idStr = url.substring(url.lastIndexOf('/') + 1);
    var players = [];
    var draftedPlayers = [];
    var numTeams;
    var draftPosition; //users draft position
    var pickCounter = parseInt($('#pickVal').text());
    var round = parseInt($('#roundVal').text());

    getDraftPosition(idStr);
    setUpEventListeners();
    hideDraftBoard();

    getDraftPick(idStr);
    getNumberOfDraftTeams(idStr);

    getAvailablePlayers(idStr);
    
    getUserTeam(idStr);

    function setUpEventListeners() {

        $(document).on('keyup', '#searchInput', function () {
            //searchBar();
            filterPlayerAndPosition($('#filterPositions option:selected').text());
        });

        $(document).on('click', '#draftButton', function () {

            var name = ($(this).closest('tr').children('td:first').text());
            var tr = $(this).closest('tr');
            var player;
            tr.remove();

            $.each(players, function (i, v) {
                if (v.name == name) {
                    player = v;
                }
            });

            if (player != null) {
                AddPlayerToTeam(idStr, getTeamNum(round, pickCounter, numTeams), player);
            }

            updateRound();
            updateRoundAndPickText();

            $('#searchInput').val("");
            filterPlayerAndPosition();
        });

        $(document).on('click', '#draftBoardButton', function () {
            hideDraft();
            showDraftBoard();
            getDraftBoard(idStr);
        });

        $(document).on('click', '#editDraftButton', function () {
            setDraftedPlayers(idStr);
            editDraft();
        });

        $(document).on('click', '#hideShowTeamButton', function () {
            hideShowTeam();
        });

        $(document).on('click', '#showDraftButton', function () {
            hideDraftBoard();
            showDraft();
            getUserTeam(idStr);
        });

        $(document).on('click', '#saveDraftChanges', function () {
            cancelSaveDraftBoardCleanup();
            updateDraftTeams(draftedPlayers, idStr);
            
        });

        $(document).on('click', '#cancelDraftChanges', function () {
            cancelSaveDraftBoardCleanup()
            getDraftBoard(idStr);
        });

        $(document).on('change', '#filterPositions', function () {
            filterPlayerAndPosition($(this).children('option:selected').text());
        });

        $(document).on('click', '#addPlayer', function () {
            addPlayerForm();
        });

        $(document).on('submit', '#addPlayerForm', function (e) {
            e.preventDefault();
            addPlayer(idStr);
        });

        $(document).on('click', '#closeFormButton', function () {
            closeForm();
        });
    }

    function cancelSaveDraftBoardCleanup() {
        $('.card').removeClass('edit-effect');
        removeCardClickEvent();
        $('#saveDraftBoardButtonGroup').hide();
        $('#draftBoardButtonGroup').css('display', 'flex');
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
                        position: v.position,
                        playerUrl: v.playerUrl
                    });
                    $('#draftTable tbody').append('<tr><th scope="row">' + v.rank + '</th>' +
                        '<td>' + getPlayerLink(v) + '</td>' + '<td>' + v.position + '</td>' +
                        '<td><button id="draftButton" class="btn btn-success">Draft</button></td>' + '</tr>');
                });
                $('#draftTable tbody').append('<tr><td><button class="btn btn-link" id="addPlayer">Add Player</button><td/td></tr>');
            },
            error: function (result) {

            }
        });
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
                    tr[i].style.display = "";
                } else {
                    if (!tr[i].innerHTML.includes("Add Player")) {
                        tr[i].style.display = "none";
                    }
                    
                }
            }
        }
    }

    function getDraftPick(draftId) {
        return $.ajax({
            url: '/api/FantasyApi/GetDraftPickNumber/' + draftId,
            type: 'GET',
            contentType: "application/json; charset=utf-8",
            processData: true,
            success: function (result) {
                pickCounter = result;
            }
        }).responseJSON;
    }

    function getUserTeam(draftId) {

        $.ajax({
            url: "/api/FantasyApi/GetUserTeam/" + draftId,
            type: "GET",
            data: {},
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (result) {
                $('#teamTable > tbody').find('a').closest('td').remove();
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
                            flexPosition.first().append('<td>' + getPlayerLink(v) + '</td>');
                        } else {
                            $('#teamTable tbody').append('<tr><td>BN</td><td>' + getPlayerLink(v) + '</td></tr>');
                        }

                    } else if (player.length == 0) {
                        tableRow.first().append('<td>' + getPlayerLink(v) + '</td>');
                    }
                });
            },
            error: function (result) {
                console.log(result);
            }
        });
    }

    function getNumberOfDraftTeams(draftId) {
        return $.ajax({
            url: '/api/FantasyApi/GetNumberOfTeams/' + draftId,
            type: 'GET',
            contentType: "application/json; charset=utf-8",
            processData: true,
            success: function (result) {
                numTeams = result;
            }
        }).responseJSON;
    }

    function getDraftPosition(draftId) {
        $.ajax({
            url: '/api/FantasyApi/GetDraftPosition/' + draftId,
            type: 'GET',
            contentType: "application/json; charset=utf-8",
            processData: true,
            success: function (result) {
                draftPosition = result;
            }
        }).responseJSON;
    }

    function AddPlayerToTeam(draftId, teamNum, player) {

        player.positionDrafted = pickCounter;
        var isPlayerTurn = isTurn(round, numTeams, pickCounter);

        $.ajax({
            url: '/api/FantasyApi/AddToTeam/' + draftId + '/' + teamNum,
            type: 'PUT',
            data: JSON.stringify(player),
            contentType: "application/json; charset=utf-8",
            processData: true,
            cache: false,
            success: function (result) {
                if (isPlayerTurn) {
                    getUserTeam(idStr);
                }

                pickCounter++;
                updateRound(pickCounter, numTeams);
                updateRoundAndPickText(round, pickCounter);
            }
        });
    }

    function updateRoundAndPickText(rnd, pick) {
        $('#roundVal').text(rnd);
        $('#pickVal').text(pick);
    }

    function updateRound(pickCounter, numTeams) {

        round = Math.ceil(pickCounter / numTeams);
    }

    function isTurn(r, numTeams, numPicks) {
        draftPick = draftPosition;
        if (r % 2 == 0) {

            return (r * numTeams - draftPick + 1) == numPicks;
        }
        return ((r - 1) * numTeams + draftPick) == numPicks;

    }

    function hideDraft() {
        $('#draftButtonGroup').hide();
        $('#draftContent').hide();
    }

    function hideDraftBoard() {
        $('#draftBoard').hide();
        $('#draftBoardButtonGroup').hide();
    }

    function showDraft() {
        $('#draftButtonGroup').css('display', 'flex');
        $('#draftContent').show();
    }

    function showDraftBoard() {
        $('#draftBoard').show();
        $('#draftBoardButtonGroup').css('display', 'flex');
    }

    function setDraftedPlayers(draftId) {
        draftedPlayers = [];

        $.ajax({
            url: '/api/FantasyApi/GetDraftedPlayers/' + draftId,
            type: 'GET',
            contentType: "application/json; charset=utf-8",
            cache: false,
            success: function (result) {
                $.each(result, function (i, v) {
                    draftedPlayers.push({
                        name: v.name,
                        position: v.position,
                        rank: v.rank,
                        positionDrafted: v.positionDrafted
                    });
                });
            }
        });
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

        $('#draftBoardButtonGroup').hide();
        $('#saveDraftBoardButtonGroup').css('display', 'flex');
        $('.card').addClass('edit-effect');

        setupCardClickEvent();
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
                shiftCards();
            }
            
        });
    }

    function removeCardClickEvent() {
        $(document).off('click', '.card');
        $('.card').removeClass('card-selected');
    }

    function shiftCards() {
        if ($('.card-selected').length !== 2) {
            return;
        }

        //get the 2 selected card
        var card1 = $('.card-selected').eq(0);
        var card2 = $('.card-selected').eq(1);

        //get the elements for player name and position
        var card1NameElement = card1.children().children().get(0);
        var card1PositionElement = card1.children().children().eq(1).children().get(0);
        var card2NameElement = card2.children().children().get(0);
        var card2PositionElement = card2.children().children().eq(1).children().get(0);

        //get the text for name and position
        var name1 = card1NameElement.innerText.trim();
        var name2 = card2NameElement.innerText.trim();
        var position1 = card1PositionElement.innerText.trim();
        var position2 = card2PositionElement.innerText.trim();

        var shiftedPlayerIndices = [];

        /*make sure players are sorted by position drafted*/
        sortPlayersByDrafted(draftedPlayers);
 
        $.each(draftedPlayers, function (i, v) {
            if (v.name.includes(name1) && v.position.includes(position1) || v.name.includes(name2) && v.position.includes(position2)) {
                shiftedPlayerIndices.push(i);
            }
        });

        var largerIndex = shiftedPlayerIndices.pop();
        var smallerIndex = shiftedPlayerIndices.pop();

        for (var i = 0; i < draftedPlayers.length; i++) {
            if (i === smallerIndex) {
                
                var tempPosDrafted = draftedPlayers[i].positionDrafted;
                draftedPlayers[largerIndex].positionDrafted = tempPosDrafted;
                draftedPlayers[i].positionDrafted = tempPosDrafted + 1;
            }
            else if (i > smallerIndex && i < largerIndex) {

                var tempPos = draftedPlayers[i].positionDrafted;
                draftedPlayers[i].positionDrafted = tempPos + 1;
            }
        }
        console.log(draftedPlayers);
        sortPlayersByDrafted(draftedPlayers);

        //updateDraftBoardUi(draftedPlayers, idStr);
        updateDraftBoardCards(draftedPlayers);
    }

    function sortPlayersByDrafted(players) {
        players.sort((pos1, pos2) => (pos1.positionDrafted > pos2.positionDrafted) ? 1 : -1)
    }

    function hideShowTeam() {
        if ($('#teamTable').is(':visible')) {
            $('#teamTable').hide();
            $('#draftTable').parent().removeClass('col-md-8').addClass('col-md-12');
            $('#hideShowTeamButton').text('Show Team');
        }
        else {
            $('#draftTable').parent().removeClass('col-md-12').addClass('col-md-8');
            $('#teamTable').show();
            $('#hideShowTeamButton').text('Hide Team');
        }
    }

    function addPlayerForm() {
        $('body').append('<div class="popup-form row col-md-6 justify-content-center align-items-center" id="addPlayerFormContainer"></div>')
        $.ajax({
            url: '/api/FantasyApi/GetAddPlayerForm/',
            type: 'GET',
            success: function (result) {
                $('#addPlayerFormContainer').html(result).css('border', 'groove');
            }
        });

        $("#addPlayerForm").validate({
            rules: {
                name: {
                    required: true
                }
            }
        });
    }

    function closeForm() {
        $('#addPlayerFormContainer').remove();
    }

    function addPlayer(draftId) {

        var name = $('#addPlayerNameInput').val();
        var position = $('#addPlayerPosition option:selected').text();

        var player = {
            name: name,
            position: position,
            positionDrafted: pickCounter
        };

        closeForm();

        $.ajax({
            url: '/api/FantasyApi/AddPlayerToDraft/' + draftId,
            type: 'PUT',
            data: JSON.stringify(player),
            contentType: "application/json; charset=utf-8",
            processData: true,
            cache: false,
            success: function (result) {
                players.push({
                    id: result.id,
                    name: result.name,
                    rank: result.rank,
                    position: result.position
                });
                $('#draftTable tbody tr:last').before('<tr><th scope="row">' + result.rank + '</th>' +
                    '<td>' + GetPlayerLink(result.name) + '</td>' + '<td>' + result.position + '</td>' +
                    '<td><button id="draftButton" class="btn btn-success">Draft</button></td>' + '</tr>');
                filterPlayerAndPosition($('#filterPositions option:selected').text());
            }
        });
    }

    function getDraftBoard(draftId) {
        $.ajax({
            url: '/api/FantasyApi/GetDraftBoard/' + draftId,
            type: "GET",
            //contentType: "application/json",
            //dataType: "html",
            success: function (result) {
                $('#draftBoard').show();
                $('#draftBoard').html(result);
                var percent = (1 / numTeams) * 100;
                $(".flex-item").css("flex", "0 0 " + percent + "%");
                setupDraftBoardColors();
            },
            error: function (result) {
            }
        });
    }

    function getTeamNum(roundNum, pickNum, numTeams) {

        var mnum = 0;

        //odd round
        if (roundNum % 2 === 1) {
            if (pickNum % numTeams === 0)
                num = numTeams;
            else
                num = pickNum % numTeams;
        }
        //even round
        else {
            if (pickNum % numTeams === 0)
                num = 1;
            else
                num = (numTeams - pickNum % numTeams) + 1;
        }
        return num;
    }

    function getPlayerLink(player) {
        return '<a href="' + player.playerLink + '" target="_blank">' + player.name + '</a>'
    }

    function updateDraftBoardUi(draftedPlayers, draftId) {
        $.ajax({
            url: '/api/FantasyApi/GetDraftBoardUi/' + draftId,
            type: "PUT",
            data: JSON.stringify(draftedPlayers),
            contentType: "application/json; charset=utf-8",
            processData: true,
            cache: false,
            success: function (result) {
                removeCardClickEvent()
                $('#draftBoard').html(result);
                setupCardClickEvent();
                setupDraftBoardColors();
                $('.card').addClass('edit-effect');
            },
            error: function (result) {
            }
        });
    }

    function updateDraftBoardCards(draftedPlayers) {

        $("#draftBoard .card-body").each(function (i) {

            $(this).children(".card-title").get(0).innerText = draftedPlayers[i].name;
            $(this).find("footer small").get(0).innerText = draftedPlayers[i].position;
        });
        $('.card').removeClass('card-selected');
        setupDraftBoardColors();
    }

    function updateDraftTeams(draftedPlayers, draftId) {

        $.ajax({
            url: '/api/FantasyApi/UpdateDraftTeams/' + draftId,
            type: "PUT",
            data: JSON.stringify(draftedPlayers),
            contentType: "application/json; charset=utf-8",
            processData: true,
            cache: false,
            success: function (result) {

            },
            error: function (result) {
            }
        });
    }
});