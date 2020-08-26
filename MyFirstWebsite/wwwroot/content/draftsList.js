$(function () {
    var teams = [];

    getAllDrafts();
    setupEvents();

    function setupEvents() {
        $(document).on('click', '#listOfDraftsTable button', function () {
            var teamName = $(this).closest('tr').data('teamName');
            var leageName = $(this).closest('tr').data('leagueName');
            deleteTeam(teamName, leageName);
            $(this).closest('tr').remove();
        });
    }

    function deleteTeam(teamName, leagueName) {
        var team;

        $.each(teams, function (i, v) {
            if (v.leagueName === leagueName && v.teamName === teamName) {
                team = v;
            }
        });
        $.ajax({
            url: '/api/FantasyApi/DeleteDraft/',
            type: "DELETE",
            data: JSON.stringify(team),
            contentType: "application/json; charset=utf-8",
            processData: true,
            cache: false,
            success: function (result) {

            },
            error: function (result) {
                console.log("Can't delete draft: " + result);
            }
        });
    }

    function getAllDrafts() {
        $.ajax({
            url: '/api/FantasyApi/GetAllUserDrafts/',
            type: "GET",
            data: {},
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (result) {
                $.each(result, function (i, v) {
                    teams.push({
                        id: v.id,
                        leagueName: v.leagueName,
                        teamName: v.teamName,
                        draftPosition: v.draftPosition
                    });
                });
            },
            error: function (result) {
                console.log("Can't get team: " + result);
            }
        });
    }
});
