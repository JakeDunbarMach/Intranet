$(document).ready(function () {

    /*Must be called for model validation when using javascript*/
    $.validator.unobtrusive.parse(document);

    /*On click of the button load a dialog*/
    $('#btnAddDocument').click(function () {
        if (($('#txtPermissionID').val() == 2) || ($('#txtPermissionID').val() == 3)) {
            var url = VIRTUAL_PATH + '/EntityDocument/AddDocument/?EntityID=' + $('#EntityID').val();
            openDialog(url, 'New Document', 780);
        }
    });

    /*On click of the button load a confirm dialog*/
    $(".lnkDocumentDelete").on("click", function (event) {
        event.preventDefault();
        var url = VIRTUAL_PATH + '/EntityDocument/DeleteDocument?id=' + getURLParameterValue(this.href, 'id');
        var msg = '<div class="bold">' + $(this).closest('td').siblings('td.doc').text().trim() + '</div><br /><div>Are you sure you want to delete this document?</div>';

        /* No Model so need the following 2 lines otherwise the  $form.data("validator") in the ajaxPostSerialize function will return undefined. */
        var $frm = $("#frmDeleteDocument");
        var test = $frm.validate({ showErrors: function () { } });
        confirmDialogWithPost("#frmDeleteDocument", "Delete Document", msg, url, deleteDocumentCallback);
    });

});

function deleteDocumentCallback() {
    var nocache = "&nocache=" + String((new Date).getTime()).replace(/\D/gi, "");
    $("#lstDocuments").load(VIRTUAL_PATH + '/Entity/ListDocuments?ID=' + $("#EntityID").val() + nocache)
    closeDialog();
}
