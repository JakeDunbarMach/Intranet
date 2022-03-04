var animateEffect = "fade";

(function ($) {
    if ($.validator) {
        //get the reference to the original function into a local variable
        var _getLength = $.validator.prototype.getLength;

        //overwrite existing getLength of validator
        $.validator.prototype.getLength = function (value, element) {

            //double count line breaks for textareas only
            if (element.nodeName.toLowerCase() === 'textarea') {

                //Counts all the newline characters (\r = return for macs, \r\n for Windows, \n for Linux/unix)
                var newLineCharacterRegexMatch = /\r?\n|\r/g;

                //use [element.value] rather than [value] since I found that the value passed in does cut off leading and trailing line breaks.
                if (element.value) {

                    //count newline characters
                    var regexResult = element.value.match(newLineCharacterRegexMatch);
                    var newLineCount = regexResult ? regexResult.length : 0;

                    //replace newline characters with nothing
                    var replacedValue = element.value.replace(newLineCharacterRegexMatch, "");

                    //return the length of text without newline characters + doubled newline character count
                    return replacedValue.length + (newLineCount * 2);
                } else {
                    return 0;
                }
            }
            //call the original function reference with apply
            return _getLength.apply(this, arguments);
        };
    }
})(jQuery);

function Ping() {

    // This function sets up an interval which calls the PingServer function every X seconds.
    // See Constants.js for PingSeconds
    this.PingSetup = function () {
        setInterval(this.PingServer, oConstants.PingSeconds);
    }

    // This function call's the server's 'ping' method. The only purpose of this is to keep the client's session alive on the server.
    this.PingServer = function () {

        // This closure can be used to manipulate the ping's result if required, not that it will be very interesting.
        function OnPingComplete(pingResult) {

        }

        $.ajax({
            type: 'get',
            dataType: 'json',
            url: VIRTUAL_PATH + '/System/Ping',
            success: OnPingComplete
        });
    }
}

/*
    Ping not required in Azure 
    Just set the Application Property "Always On" to Yes
    Only enable the 2 lines below if you are deploying locally
*/

//var oPing = new Ping();
//oPing.PingSetup();


function SearchInitialize() {
    $("#AdvancedLink").on("click", function () {
        //If div is shown then hiding div so set to false before toggling
        if ($('#AdvancedOptions').css('display') != 'none') {
            $('#ShowAdvanced').val('False');
            $('#AdvancedLink').html("More Filters <i class= 'far fa-arrow-alt-circle-down' >");
        }
        else {
            $('#ShowAdvanced').val('True');
            $('#AdvancedLink').html("Less Filters <i class= 'far fa-arrow-alt-circle-up' >");
        }
        $('#AdvancedOptions').slideToggle();
        return false;
    });

    if ($('#ShowAdvanced').val() == 'True') {
        $('#AdvancedOptions').show();
        $('#AdvancedLink').html("Less Filters <i class= 'far fa-arrow-alt-circle-up' >");
    }
    else {
        $('#AdvancedOptions').hide();
        $('#AdvancedLink').html("More Filters <i class= 'far fa-arrow-alt-circle-down' >");
    }

    //Set class clearable val to '' when click ClearButton
    $('#ClearButton').click(
        function () {
            $('.clearable').val('');
            $('.clearable').attr('checked', false);
            $('#SearchButton').click();
        }
    );

    //On click of SearchButton set model bool ExportResults to false
    $('#SearchButton').click(function () {
        $('#ExportResults').val('False');
    });
}

function populateDropdown(id) {
    var oAutoPopulate = new AutoPopulate();
    oAutoPopulate.Populate(id);
}

function AutoPopulate() {
    this.Populate = function (element) {

        if (element != undefined) {
            //If there's only two options in the drop down 
            //set it to the one that isnt 'Please Select....'
            if ($(element).find('option').length == 2) {
                //For each option for the element
                $(element).find('option').each(function () {
                    var $this = $(this);
                    if ($this.text().indexOf('Please Select') < 0) {
                        $this.attr('selected', 'selected');
                        //Trigger on change event of the dropdown so filters/sets to Please Select
                        $(element).trigger('change');
                        return false;
                    }
                });
            }
        }
    }
}

function HighlightNavBar(id) {
    $(id).addClass("cur_lnk");
    return false;
}

/*
Ajax Form Post (serialise's the form data) 
Parameters: 
    frm : Form ID
    url: Controller/Action/[Parameters]
    callbackFunc: CallBack Function (code to run if success) 
    showSuccessMessage: if true shows the success status Message e.g. returned from controller/action
    validationErrorDiv: specific div to write error to if supplied

    Note: use when a model or ViewModel is required to be passed to the controller
*/
function ajaxPostSerialize(frm, url, callbackFunc, showSuccessMessage, validationErrorDiv) {
    //First check is form model is valid and return so model validation errors are shown
    var $form = $(frm);
    var validator = $form.data("validator");
    if (!validator || !$form.valid()) {
        return;
    }
    //serialize the Form Data into formData
    var formData = $(frm).serialize();

    $.ajax(
        {
            type: 'post',
            dataType: 'json',
            url: url,
            data: formData,

            success: function (JsonResponse) {
                if (JsonResponse == JSON_ERROR) {
                    alert(oConstants.Json_Error_Message);
                }
                else if (JsonResponse.IsValid == false || JsonResponse.Saved == false) {

                    if ($("#AjaxFail").length == 0) { $('body').append("<div id='AjaxFail'/>"); }

                    $('#AjaxFail').dialog(
                        {
                            height: 240,
                            resizable: false,
                            modal: true,
                            title: 'Error',
                            buttons:
                            {
                                OK: function () {
                                    $('#AjaxFail').dialog("close");
                                }
                            }
                        });
                    $('#AjaxFail').html('<p>' + JsonResponse.Message + '</p>');
                }
                else if (JsonResponse.Saved == true) {

                    if (showSuccessMessage) {

                        if ($("#AjaxSuccess").length == 0) { $('body').append("<div id='AjaxSuccess'/>"); }

                        $('#AjaxSuccess').html('<p>' + JsonResponse.Message + '</p>');
                        $('#AjaxSuccess').dialog(
                            {
                                height: 200,
                                resizable: false,
                                modal: true,
                                title: JsonResponse.Title,
                                buttons:
                                {
                                    OK: function () {
                                        $('#AjaxSuccess').dialog("close");
                                    }
                                }
                            });
                    }
                    callbackFunc() //Passed Function to execute
                }
            },
            error: function (JsonResponse) {

                var data = JSON.parse(JsonResponse.responseText);
                var errorMessage = '';
                for (var i in data) {
                    errorMessage = errorMessage + '<p>' + data[i].errors + '</p>';
                }

                //If passed in ID of div to write error to use that otherwise #ValidationError
                if ((validationErrorDiv != undefined) && $(validationErrorDiv).length > 0) {
                    $(validationErrorDiv).html(errorMessage);
                    $(validationErrorDiv).show();
                }
                else {
                    if ($("#ValidationError").length == 0) { $('body').append("<div id='ValidationError'/>"); }
                    $('#ValidationError').html(errorMessage);
                    $('#ValidationError').show();
                }
            }
        });
}

/*
Ajax Form Post
Parameters: 
    url: Controller/Action/[Parameters]
    callbackFunc: CallBack Function (code to run if success) 
    validationErrorDiv: specific div to write error to if supplied
*/
function ajaxPost(url, callbackFunc, validationErrorDiv) {

    $.ajax(
        {
            type: 'post',
            dataType: 'json',
            url: url,

            success: function (JsonResponse) {
                if (JsonResponse == JSON_ERROR) {
                    alert(oConstants.Json_Error_Message);
                }
                else if (JsonResponse.IsValid == false || JsonResponse.Saved == false) {

                    if ($("#AjaxFail").length == 0) { $('body').append("<div id='AjaxFail'/>"); }

                    $('#AjaxFail').dialog(
                        {
                            height: 240,
                            resizable: false,
                            modal: true,
                            title: 'Error',
                            buttons:
                            {
                                OK: function () {
                                    $('#AjaxFail').dialog("close");
                                }
                            }
                        });
                    $('#AjaxFail').html('<p>' + JsonResponse.Message + '</p>');
                }
                else if (JsonResponse.Saved == true) {

                    callbackFunc() //Passed Function to execute 
                }
            },
            error: function (JsonResponse) {

                try {

                    var data = JSON.parse(JsonResponse.responseText);
                    var errorMessage = '';
                    for (var i in data) {
                        errorMessage = errorMessage + '<p>' + data[i].errors + '</p>';
                    }

                    //If passed in ID of div to write error to use that otherwise #ValidationError
                    if ((validationErrorDiv != undefined) && $(validationErrorDiv).length > 0) {
                        $(validationErrorDiv).html(errorMessage);
                        $(validationErrorDiv).show();
                    }
                    else {
                        if ($("#ValidationError").length == 0) { $('body').append("<div id='ValidationError'/>"); }
                        $('#ValidationError').html(errorMessage);
                        $('#ValidationError').show();
                    }
                }
                catch (e) {
                    alert("JsonResponse Error:\n\n" + e.name + "\n" + e.number + "\n" + e.message);
                }
            }
        });
}

/*
Ajax Form Post
Parameters: 
    url: Controller/Action/[Parameters]
    callbackFunc: CallBack Function (code to run if success) 
    validationErrorDiv: specific div to write error to if supplied
*/
function ajaxPostReturnJsonID(url, callbackFunc, validationErrorDiv) {
    $.ajax(
        {
            type: 'post',
            dataType: 'json',
            url: url,

            success: function (JsonResponse) {
                if (JsonResponse == JSON_ERROR) {
                    alert(oConstants.Json_Error_Message);
                }
                else if (JsonResponse.IsValid == false || JsonResponse.Saved == false) {

                    var titleStr = "Error";
                    try {
                        if (JsonResponse.Title != "") {
                            titleStr = JsonResponse.Title;
                        }
                    }
                    catch (e) { }

                    if ($("#AjaxFail").length == 0) { $('body').append("<div id='AjaxFail'/>"); }

                    $('#AjaxFail').dialog(
                        {
                            height: 240,
                            resizable: false,
                            modal: true,
                            title: titleStr,
                            buttons:
                            {
                                OK: function () {
                                    $('#AjaxFail').dialog("close");
                                }
                            }
                        });

                    $('#AjaxFail').html('<p style="white-space: pre-line">' + JsonResponse.Message + '</p>');
                }
                else if (JsonResponse.Saved == true) {

                    try {
                        callbackFunc(JsonResponse.ID) //Passed Function with parameter to execute
                    }
                    catch (e) {
                        callbackFunc //Passed Function to execute
                    }
                }
            },
            error: function (JsonResponse) {

                try {

                    var data = JSON.parse(JsonResponse.responseText);
                    var errorMessage = '';
                    for (var i in data) {
                        errorMessage = errorMessage + '<p>' + data[i].errors + '</p>';
                    }

                    //If passed in ID of div to write error to use that otherwise #ValidationError
                    if ((validationErrorDiv != undefined) && $(validationErrorDiv).length > 0) {
                        $(validationErrorDiv).html(errorMessage);
                        $(validationErrorDiv).show();
                    }
                    else {
                        if ($("#ValidationError").length == 0) { $('body').append("<div id='ValidationError'/>"); }
                        $('#ValidationError').html(errorMessage);
                        $('#ValidationError').show();
                    }
                }
                catch (e) {
                    alert("JsonResponse Error:\n\n" + e.name + "\n" + e.number + "\n" + e.message);
                }
            }
        });
}

/*
Ajax Form Post (serialise's the form data) and returns the ID of an inserted or updated record (JsonResponse.ID)
Parameters: 
    frm : Form ID
    url: Controller/Action/[Parameters]
    callbackFunc: CallBack Function (code to run if success) 
    showSuccessMessage: if true shows the success status Message e.g. returned from controller/action   
    validationErrorDiv: specific div to write error to if supplied

    Note: use when a model or ViewModel is required to be passed to the controller
    Add  $.validator.unobtrusive.parse(document); in script
*/
function ajaxPostSerializeReturnJsonID(frm, url, callbackFunc, showSuccessMessage, validationErrorDiv) {
    //First check is form model is valid and return so model validation errors are shown
    var $form = $(frm);
    var validator = $form.data("validator");

    if (!validator || !$form.valid()) {
        return;
    }
    //serialize the Form Data into formData
    var formData = $(frm).serialize();

    $.ajax(
        {
            type: 'post',
            dataType: 'json',
            url: url,
            data: formData,

            success: function (JsonResponse) {
                if (JsonResponse == JSON_ERROR) {
                    alert(oConstants.Json_Error_Message);
                }
                else if (JsonResponse.IsValid == false || JsonResponse.Saved == false) {

                    var titleStr = "Error";
                    try {
                        if (JsonResponse.Title != "") {
                            titleStr = JsonResponse.Title;
                        }
                    }
                    catch (e) { }

                    if ($("#AjaxFail").length == 0) { $('body').append("<div id='AjaxFail'/>"); }

                    $('#AjaxFail').dialog(
                        {
                            height: 240,
                            resizable: false,
                            modal: true,
                            title: titleStr,
                            buttons:
                            {
                                OK: function () {
                                    $('#AjaxFail').dialog("close");
                                }
                            }
                        });
                    $('#AjaxFail').html('<p style="white-space: pre-line">' + JsonResponse.Message + '</p>');
                }
                else if (JsonResponse.Saved == true) {

                    if (showSuccessMessage) {
                        if ($("#AjaxSuccess").length == 0) { $('body').append("<div id='AjaxSuccess'/>"); }

                        $('#AjaxSuccess').html('<p>' + JsonResponse.Message + '</p>');
                        $('#AjaxSuccess').dialog(
                            {
                                resizable: false,
                                modal: true,
                                closeOnEscape: false,
                                title: JsonResponse.Title,
                                buttons:
                                {
                                    OK: function () {
                                        $('#AjaxSuccess').dialog("close");

                                        try {
                                            callbackFunc(JsonResponse.ID) //Passed Function with parameter to execute
                                        }
                                        catch (e) {
                                            callbackFunc //Passed Function to execute
                                        }

                                    }
                                }
                            });
                    }
                    else {
                        try {
                            callbackFunc(JsonResponse.ID) //Passed Function with parameter to execute
                        }
                        catch (e) {
                            callbackFunc //Passed Function to execute
                        }
                    }

                }
            },
            error: function (JsonResponse) {

                var data = JSON.parse(JsonResponse.responseText);
                var errorMessage = '';
                for (var i in data) {
                    errorMessage = errorMessage + '<p>' + data[i].errors + '</p>';
                }

                //If passed in ID of div to write error to use that otherwise #ValidationError
                if ((validationErrorDiv != undefined) && $(validationErrorDiv).length > 0) {
                    $(validationErrorDiv).html(errorMessage);
                    $(validationErrorDiv).show();
                }
                else {
                    if ($("#ValidationError").length == 0) { $('body').append("<div id='ValidationError'/>"); }
                    $('#ValidationError').html(errorMessage);
                    $('#ValidationError').show();
                }
            }
        });
}


/*
Opens a Dialog window; note close and remove when finished
Parameters: 
    url: Controller/Action/[Parameters]
    title: Heading Text e.g. Edit User/Add User 
    width: Dialog width
*/
function openDialog(url, title, width) {
    //Add new div for dialogs if required
    if ($("#OpenDialog").length == 0) { $('body').append("<div id=\"OpenDialog\"/>"); }

    $('#OpenDialog').dialog(
        {
            autoOpen: false,
            width: width,
            resizable: false,
            modal: true,
            title: title,
            close: closeDialog,
            responsive: true,
            clickOut: false
        });

    $('#OpenDialog').load(
        url,
        function () {
            $(this).dialog('open');
        }
    );
    return false;
}

/*
Closes an open Dialog window
*/
function closeDialog() {
    if ($("#OpenDialog").length > 0) {
        $('#OpenDialog').dialog('close');
        $('#OpenDialog').remove();
    }
}

/*
Returns the value of a named parameter in the url
Parameters: 
    url: Controller/Action?a=1&b=2&c3
    name: parameter name e.g. "b" would return 2

*/
function getURLParameterValue(url, name) {
    return (RegExp(name + '=' + '(.+?)(&|$)').exec(url) || [, null])[1];
}

/*
Populates a Dropdown from passed ID and Url Parameters: 
    id: Dropdown List ID
    url: Controller/Action?id=1
    blankOptionText: e.g. "Please Select...."
*/
function jsonPopulateList(id, url, blankOptionText, blankOptionValue) {
    $(id).empty();
    $(id).prepend($('<option></option>').html('Loading...'));
    $.getJSON(url, null, function (data) {
        data = $.map(data, function (item, a) {
            if (item.Selected) {
                return "<option value=" + item.Value + " selected=selected" + ">" + item.Text + "</option>";
            }
            else {
                return "<option value=" + item.Value + ">" + item.Text + "</option>";
            }
        });
        blankOptionText = (typeof blankOptionText === 'undefined') ? '' : blankOptionText;
        if (blankOptionText != '') {
            blankOptionValue = (typeof blankOptionValue === 'undefined') ? "''" : blankOptionValue;
            data.unshift("<option value=" + blankOptionValue + " selected=selected" + ">" + blankOptionText + "</option>");
        }
        $(id).html(data.join(""));
    });
}


/*  
    Initialise all Date Picker Controls for use with Dialog where the class is set to "date-picker". 
    Resolves issues with Date Picker staying open after the date is clicked within a dialog popup
       
    Parameters: 
        focusControlID: #id of a control where focus can be passed once date is selected 
*/
function initialiseDatePickerControlsForDialog(focusControlID) {

    /*Shows jQuery UI date picker for all fields with the class datepicker*/
    $(".date-picker").datepicker(
        {
            dateFormat: 'dd/mm/yy',
            onSelect: function (dateText, inst) {
                $("#" + this.id).datepicker("destroy");
            }
        }
    );

    /*prevents datepicker staying open when date is clicked*/
    $(".date-picker").click(function () {
        $(focusControlID).focus();
        $("#" + this.id).focus();
        $(".date-picker").datepicker(
            {
                dateFormat: 'dd/mm/yy',
                onSelect: function (dateText, inst) {
                    $("#" + this.id).datepicker("destroy");
                }
            });
    });
    $(".date-picker").prop("autocomplete", "off");
}


/*
  Initialise all Date Picker Controls on the document where the class is set to "date-picker"
*/
function initialiseDatePickerControls() {

    // This will make every element with the class "date-picker" into a DatePicker element
    $(".date-picker").datepicker({
        dateFormat: 'dd/mm/yy', gotoCurrent: true
    });

    //Would throw an error if no datepicker is in the form hence a try
    try {
        $(".date-picker").rules("add", { customDateValidator: true });
    } catch (e) { }

    //Customised Date function
    jQuery.validator.addMethod("customDateValidator",
        function (value, element) {
            try {
                $.datepicker.parseDate("dd/mm/yy", value); return true;
            }
            catch (e) {
                return false;
            }
        },
        "Invalid date format!"
    );
    $(".date-picker").prop("autocomplete", "off");
}

//Chrome datepicker fix for invalid date model error
jQuery.validator.methods["date"] = function (value, element) { return true; };

/*
Get the value of a named parameter from a url
    Parameters: 
    frm: form name requesting post
    title: Dialog Title e.g.  "Delete ?"
    msg: action message e.g. "Are you sure you want to delete #?"
    url: Controller/Action?a=1&b=2&c3
        callbackFunc: CallBack Function (code to run if success) 
*/
function confirmDialogWithPost(frm, title, msg, url, callbackFunc) {
    //Add confirm for dialogs if required
    if ($("#Confirm").length == 0) { $('body').append("<div id='Confirm'/>"); }

    $('#Confirm').html(msg);

    //Load dialog confirming delete
    $('#Confirm').dialog({
        autoOpen: false,
        resizeable: false,
        width: 640,
        modal: true,
        hide: animateEffect,
        show: animateEffect,
        title: title,
        buttons:
            [
                {
                    text: "No",
                    class: 'form-control btn btn-primary',
                    style: 'width:80px;margin-left:12px',
                    click: function () {
                        $('#Confirm').dialog('close');
                        $('#Confirm').remove();
                    }
                },
                {
                    text: "Yes",
                    class: 'form-control btn btn-primary',
                    style: 'width:80px',
                    click: function () {
                        ajaxPostSerializeReturnJsonID(frm, url, callbackFunc, false);
                        $('#Confirm').dialog('close');
                        $('#Confirm').remove();
                    }
                }
            ]
    });

    $('#Confirm').dialog('open');
}

function confirmDialogNoPost(title, msg, callbackFunc) {

    //Add confirm for dialogs if required
    if ($("#Confirm").length == 0) { $('body').append("<div id='Confirm'/>"); }

    $('#Confirm').html(msg);

    //Load dialog confirming delete
    $('#Confirm').dialog({
        autoOpen: false,
        resizeable: false,
        width: 540,
        modal: true,
        title: title,
        buttons:
            [
                {
                    text: "Yes",
                    class: 'btn btn-primary',
                    click: function () {
                        callbackFunc();
                        $('#Confirm').dialog('close');
                        $('#Confirm').remove();
                    }
                },
                {
                    text: "No",
                    class: 'btn btn-secondary btnLeft',
                    autofocus: true,
                    click: function () {
                        $('#Confirm').dialog('close');
                        $('#Confirm').remove();
                    }
                }

            ],
        close: function () {
            $('#Confirm').dialog('close');
            $('#Confirm').remove();
        }
    });

    $('#Confirm').dialog('open');
}

function confirmDialogYesNo(title, msg, callbackNoFunc, callbackYesFunc) {

    //Add confirm for dialogs if required
    if ($("#Confirm").length == 0) { $('body').append("<div id='Confirm'/>"); }

    $('#Confirm').html(msg);

    //Load dialog confirming delete
    $('#Confirm').dialog({
        autoOpen: false,
        resizeable: false,
        width: 540,
        modal: true,
        title: title,
        buttons:
            [
                {
                    text: "Yes",
                    class: 'btn btn-primary',
                    click: function () {
                        callbackYesFunc();
                        $('#Confirm').dialog('close');
                        $('#Confirm').remove();
                    }
                },
                {
                    text: "No",
                    class: 'btn btn-secondary btnLeft',
                    autofocus: true,
                    click: function () {
                        callbackNoFunc();
                        $('#Confirm').dialog('close');
                        $('#Confirm').remove();
                    }
                }

            ],
        close: function () {
            $('#Confirm').dialog('close');
            $('#Confirm').remove();
        }
    });

    $('#Confirm').dialog('open');
}

function confirmDialogContinue(title, msg, callbackContinueFunc) {

    //Add confirm for dialogs if required
    if ($("#Confirm").length == 0) { $('body').append("<div id='Confirm'/>"); }

    $('#Confirm').html(msg);

    //Load dialog confirming delete
    $('#Confirm').dialog({
        autoOpen: false,
        resizeable: false,
        width: 540,
        modal: true,
        title: title,
        buttons:
            [
                {
                    text: "Continue",
                    class: 'btn btn-primary',
                    autofocus: true,
                    click: function () {
                        callbackContinueFunc();
                        $('#Confirm').dialog('close');
                        $('#Confirm').remove();
                    }
                }
            ],
        close: function () {
            $('#Confirm').dialog('close');
            $('#Confirm').remove();
        }
    });

    $('#Confirm').dialog('open');
}

function msgDialog(title, msg, callbackOkFunc) {

    //Add confirm for dialogs if required
    if ($("#Confirm").length == 0) { $('body').append("<div id='Confirm'/>"); }

    $('#Confirm').html(msg);

    //Load dialog confirming delete
    $('#Confirm').dialog({
        autoOpen: false,
        resizeable: false,
        width: 540,
        modal: true,
        title: title,
        buttons:
            [
                {
                    text: " OK ",
                    class: 'btn btn-primary',
                    autofocus: true,
                    click: function () {
                        callbackOkFunc();
                        $('#Confirm').dialog('close');
                        $('#Confirm').remove();
                    }
                }
            ],
        close: function () {
            $('#Confirm').dialog('close');
            $('#Confirm').remove();
        }
    });

    $('#Confirm').dialog('open');
}

/* GET THE TABLE COLUMN TEXT*/
function getSortColumn(obj) {
    var s = $(obj).text();
    s = s.replace(/\n/g, ''); //removes line-breaks
    s = s.replace(/\t/g, ''); //removes tabs
    s = s.replace(/\./g, ''); //removes dots e.g. Just. column
    s = s.replace(/\s+/g, ''); //removes spaces
    return s;
}

//Taken from ComplaintsSearchResult.js but moved into here as multiple searches
//SORT TABLE BY COLUMN 
function ordering(orderCol, orderBy) {
    if (orderBy == '') {
        $('#OrderBy').val(orderCol);
    }
    else {
        var orderByArarry = orderBy.split("_");
        if (orderByArarry.length == 2) {
            if (orderByArarry[0] == orderCol) {
                $('#OrderBy').val(orderByArarry[0]);
            }
            else {
                $('#OrderBy').val(orderCol);
            }
        }
        else {
            if (orderByArarry[0] == orderCol) {
                $('#OrderBy').val(orderCol + '_desc');
            }
            else {
                $('#OrderBy').val(orderCol);
            }
        }
    }
}


/* RESET ALL COLUMNS AND SET ACTIVE COLUMN*/
function orderSetClass(thisOrderBy) {
    $('.orderby').each(function () {

        $(this).removeClass("orderdesc").removeClass("orderasc").removeClass("ordernone");  //remove all sort classes

        var thisCol = getSortColumn($(this)); //get the current sort column

        if (thisCol == thisOrderBy) {
            $(this).addClass("orderasc");
        }
        else if (thisCol + "_desc" == thisOrderBy) {
            $(this).addClass("orderdesc");
        }
        else {
            $(this).addClass("ordernone");
        }
    });
}

//Format dates into mm/dd/yyyy for comparison, etc
function formatDate(dateToFormat) {
    var dateSplit = dateToFormat.split("/");
    var dateDay = dateSplit[0];
    var dateMonth = dateSplit[1];
    var dateYear = dateSplit[2];
    var formattedDate = dateMonth + '-' + dateDay + '-' + dateYear;
    formattedDate = new Date(formattedDate);
    return formattedDate;
}

function validation(id, show, msg) {
    if (show) {
        $("[data-valmsg-for='" + id + "']").removeClass('field-validation-valid').addClass('field-validation-error').text(msg);
        isOkay = false;
    } else {
        $("[data-valmsg-for='" + id + "']").removeClass('field-validation-error').addClass('field-validation-valid').text("");
    }
}

/* This should only be required when using search with ToPagedList */
function fixDateFormatInSearch(id) {
    //Split out the date fields and concatenate below by mm-dd-yyyy as converted 
    //back to english when at model (otherwise get american format!)
    var dateFrom = "";
    try {
        var dateFromSplit = $(id).val().split("/");
        var dateFromDay = dateFromSplit[0];
        var dateFromMonth = dateFromSplit[1];
        var dateFromYear = dateFromSplit[2];
        dateFrom = dateFromMonth + '/' + dateFromDay + '/' + dateFromYear;
    }
    catch (e) { }

    return dateFrom;
}

function setupMultiSelect(listID, noSelectText, includeSelectAllOption, maxHeight, numberDisplayed, enableFiltering) {
    $(listID).multiselect({
        buttonContainer: '<div class="form-control multiselectSearchRow dropdown"></div>',
        buttonClass: '',
        includeSelectAllOption: includeSelectAllOption,
        numberDisplayed: numberDisplayed || 1,
        enableFiltering: enableFiltering || false,
        enableCaseInsensitiveFiltering: enableFiltering || false,
        nonSelectedText: noSelectText,
        maxHeight: maxHeight,
        templates: {
            button: '<div class="multiselect dropdown-toggle" data-toggle="dropdown" data-display="static" data-flip="false"><span class="multiselect-selected-text"></span></div>',
            li: '<li><a class="dropdown-item" tabindex="0"><label style="display: block;"></label></a></li>',
            ul: '<ul class="multiselect-container dropdown-menu"></ul>',
            filter: '<li class="multiselect-item filter"><div class="input-group m-0"><input class="form-control multiselect-search" type="text"></div></li>',
            filterClearBtn: '<span class="input-group-btn"><button class="btn btn-secondary multiselect-clear-filter" type="button"><i class="fas fa-minus-circle"></i></button></span>'
        }
    });
}

function setUpSelect2() {
    $('.searchField').each(function (index, element) {
        $(this).select2({
            dropdownAutoWidth: true,
            width: '100%',
            containerCssClass: 'form-control'
        });
    });
}