var numberOfDocs = 0;
$(document).ready(function () {

    /*Must be called for model validation when using javascript*/
    $.validator.unobtrusive.parse(document);

    $('#fileupload').fileupload({
        type: 'post',
        dataType: 'json',
        url: VIRTUAL_PATH + '/EntityDocument/UploadFile',
        autoUpload: true,
        done: function (e, data) {
            var trClass = "failed";

            if (data.result.status == "True") {
                trClass = "uploaded"
            }

            $('#tblDocument tr:last').after(
                '<tr class="' + trClass + '">' +
                '<td>' + data.result.name + '</td>' +
                '<td>' + data.result.size + '</td>' +
                '<td>' + data.result.message + '</td>' +
                '</tr>'
            );
        }
    }).on('fileuploadprogressall', function (e, data) {
        var progress = parseInt(data.loaded / data.total * 100, 10);
        $('.progress .progress-bar').css('width', progress + '%');
        $('#progreestext').html(progress + '% Complete');

        setTimeout(function () {
            $('#btnAddFiles').focus();
        }, 100);
    });

    setTimeout(function () {
        $('#btnAddFiles').focus();
    }, 100);

    if ($('#tblDocument tbody tr.uploaded').length > 0) {
        numberOfDocs = $('#tblDocument tbody tr.uploaded').length;
    }

    $('.ui-dialog').on('dialogclose', function (event) {
        var nocache = "&nocache=" + String((new Date).getTime()).replace(/\D/gi, "");
        $("#lstDocuments").load(VIRTUAL_PATH + '/Entity/ListDocuments?ID=' + $("#EntityID").val() + nocache)
    });
});
