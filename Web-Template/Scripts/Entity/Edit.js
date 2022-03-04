$(document).ready(function () {

    //Call function to highlight the current section on the nav bar
    HighlightNavBar('#lnkEntities');

    $.validator.unobtrusive.parse(document);

    initialiseDatePickerControls();

    //If update or admin the enable otherwise then disable everything
    if (($('#txtPermissionID').val() == 2) || ($('#txtPermissionID').val() == 3)) {
        $('#frmEntity :input').attr("disabled", false);
        $('#btnEntitySave').attr("disabled", false);
    }
    else {
        $('#frmEntity :input').attr("disabled", true);
        $('#btnEntitySave').attr("disabled", true);
        $('#btnEntityClose').attr("disabled", false);
    }

    /*On click of button close dialog */
    $('#btnEntityClose').click(function () {
        window.location.href = VIRTUAL_PATH + '/EntitySearch/Search/';
        return false;
    });

    /* On click of button call Save controller action */
    $('#btnEntitySave').click(function () {
        if (($('#txtPermissionID').val() == 2) || ($('#txtPermissionID').val() == 3)) {
            ajaxPostSerializeReturnJsonID('#frmEntity', VIRTUAL_PATH + '/Entity/Save', saveEntityCallback, true);
        }
    });

    /* On click of the tabs load the divs with the content */
    $('a[data-toggle="tab"]').on('show.bs.tab', function (e) {

        var tabHREF = $(this).attr('href');
        switch (tabHREF) {
            case '#tabDetails':
                $('#lstPointsRaised').load(VIRTUAL_PATH + '/Entity/ListPointsRaised/?EntityID=' + $('#EntityID').val());
                break;
            case '#tabDocuments':
                $('#lstDocuments').load(VIRTUAL_PATH + '/Entity/ListDocuments/?ID=' + $('#EntityID').val());
                break;
        }
    });
});

/* Method called above after saving form */
function saveEntityCallback(id) {
    if (id > 0 && $("#EntityID").val() == 0) {
        $("#EntityID").val(id);
    }
    window.location.href = VIRTUAL_PATH + '/Entity/Load/' + $('#EntityID').val();
}