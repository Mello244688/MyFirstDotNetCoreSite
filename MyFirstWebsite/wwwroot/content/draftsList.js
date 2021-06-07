$(function () {

    setupEvents();

    function setupEvents() {
        $(document).on('click', '#listOfDraftsTable button', function () {
            var draftLink = $(this).closest('tr').find('a').attr('href')
            var id = draftLink.substring(draftLink.lastIndexOf('/')+1)
            deleteDraft(id);
            $(this).closest('tr').remove();
        });
    }

    function deleteDraft(id) {

        $.ajax({
            url: '/api/FantasyApi/DeleteDraft/' + id,
            type: "DELETE",
            processData: true,
            cache: false,
            beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: function (result) {

            },
            error: function (result) {
                console.log("Can't delete draft: " + result);
            }
        });
    }
});
