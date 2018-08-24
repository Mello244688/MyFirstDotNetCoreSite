$(function () {

    var url = window.location.pathname;
    var idStr = url.substring(url.lastIndexOf('/') + 1);
    var players = [];

    //$('#searchInput').keyup(searchBar());
    

    $.ajax({
        url: '/api/FantasyApi/GetAvailablePlayers/' + idStr,
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
                $('#draftTable tbody').append('<tr><th scope="row">' + v.rank + '</th>' + '<td><a href="https://www.fantasypros.com/nfl/players/' + formatName(v.name) + '.php" target="_blank"' + '>' + v.name + '</a></td>' + '<td>' + v.position + '</td>' + '</tr>');
            });
        },
        error: function (result) {
            console.log(result);
        }
    });

    $(document).on('keyup', '#searchInput', function () {
        searchBar();
    });

    function formatName(pname) {
        var first = pname.indexOf(' ');
        var second = pname.indexOf(' ', first + 1);
        var newName = pname.substring(0, first) + "-" + pname.substring(first + 1, second);

        return newName.trim().toLowerCase().replace(/'/, '');
    }

    function searchBar() {
        console.log('hit');select box
        var input, filter, table, tr, td, i;

        input = document.getElementById("searchInput");
        console.log(input)
        filter = input.value.toUpperCase();
        table = document.getElementById("draftTable");
        tr = table.getElementsByTagName("tr");

        console.log(input);
        console.log(filter);
        console.log(table);
        console.log(tr);

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
});