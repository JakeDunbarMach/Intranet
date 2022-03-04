$(document).ready(function () {

	//Call function to highlight the current section on the nav bar
	HighlightNavBar('#lnkEntities');

	$.validator.unobtrusive.parse(document);

	initialiseDatePickerControls();

	SearchInitialize();

	//On click of any a within body-content pagedList then reload the results div
	//with results from controller Search method
	//Must be written like this as pagedList is redrawn everytime
	$(".body-content").on("click", ".pagedList a", function () {

		var $a = $(this);
		if ($a.attr("href")) {
			//Set value to false so does correct controller code to return pagedList
			$('#ExportResults').val('False');

			var serializedString = $("form").serialize();
			serializedString = serializedString.replace(/EntitySearch.DateFrom=([^&]*)/, "EntitySearch.DateFrom=" + fixDateFormatInSearch("#EntitySearch_DateFro") + "&");
			serializedString = serializedString.replace(/EntitySearch.DateTo=([^&]*)/, "EntitySearch.DateTo=" + fixDateFormatInSearch("#EntitySearch_DateTo") + "&");

			var options = {
				url: $a.attr("href"),
				data: serializedString,
				type: "get"
			};

			$.ajax(options).done(function (data) {
				var target = $('#entityResults');
				$(target).replaceWith(data);
			});
		}

		return false;

	});

	/* Example multiselect code*/
	//setupMultiSelect("#Search_EntityTypeID", "Please Select....", false, 400);

	//$("#Search_EntityTypeID").multiselect();




});