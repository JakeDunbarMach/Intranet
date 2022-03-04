$(document).ready(function () {

    //$('#tblResults').DataTable({
    //       searching: false, 
    //       lengthChange: false,
    //       order: [1, "desc"],
    //       pageLength: 12,
    //       "columnDefs": [
    //           { targets: [0], orderable: false }
    //           ]
    //});


    $("tbody tr").hoverIntent(function () {
        $(this).find(".row-icon i").fadeIn("slow").css("display", "inline-block");
    }, function () {
        $(this).find(".row-icon i").fadeOut();
    });

    /*On click of the link load form*/
    $('.btnAddEntity').click(function () {
        if (($('#txtPermissionID').val() == 2) || ($('#txtPermissionID').val() == 3)) {
            var EditUrl = VIRTUAL_PATH + '/Entity/Load/';
            window.location.href = EditUrl;
        }
        return false;
    });

    /*On click of the link Edit form*/
    $('.lnkEditEntity').click(function () {
        alert('test');
        if (($('#txtPermissionID').val() == 2) || ($('#txtPermissionID').val() == 3)) {
            var url = VIRTUAL_PATH + '/Entity/Load?ID=' + getURLParameterValue(this.href, "ID");
            window.location.href = EditUrl;
        }
        return false;
    });

    //On click of ExportButton set model bool ExportResults to True so can access in controller and export search results
    $('.ExportButton').click(function () {
        $('#ExportResults').val('True');
    });

    /*On click of the link load a dialog of Confirm*/
    $('.lnkEntityDelete').click(function () {
        if ($('#txtPermissionID').val() == 3) {
            var url = VIRTUAL_PATH + '/Entity/Delete?ID=' + getURLParameterValue(this.href, "ID");
            confirmDialogWithPost("#frmEntitySearch", "Delete Entity", "Are you sure you want to delete this entity?", url, deleteEntityCallback);
        }
        return false; // Must do this so href is ignored
    });

});

function deleteEntityCallback() {
    $("#SearchButton").click();
}