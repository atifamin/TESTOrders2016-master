var alertHtml = '<div class="alert alert-danger alert-dismissable fade in">' +
	'<a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>' +
	'<strong>Error!</strong> Duplicate travelers found.' +
	'</div>';


function getValidTravelers() {
	// Valid as far as these 3 fields filled out.
	return $('[data-traveler-id]').map(function () {
		var trav = $(this);
		var fname = trav.find('input[name$="firstName"]').val();
		var lname = trav.find('input[name$="lastName"]').val();
		var dob = trav.find('input[name$="DOB"]').val();
		if (fname && lname && dob) {
			return {
				'id': this.dataset.travelerId,
				'inputs': [fname, lname, dob].join(':')
			}
		}
	}).get();
}


function hasDuplicateInObject(propertyName, inputArray) {
	// find duplicate property in Array of Objects
	var duplicateFound = false, temp = {};
	inputArray.map(function (item) {
		var prop = item[propertyName];
		if (prop in temp) {
			temp[prop].duplicate = true;
			item.duplicate = true;
			duplicateFound = true;
		}
		else {
			temp[prop] = item;
			delete item.duplicate;
		}
	});
	return duplicateFound;
}


function markDuplicates(travelers) {
	$('[data-traveler-id]').removeClass('border-danger has-error');
	$('#errorMsg').html('');
	travelers.map(function (item) {
		if (item.duplicate) {
			$('[data-traveler-id="' + item['id'] + '"]').addClass('border-danger has-error');
		}
	});
}



$('[type=submit]').on('click', function (e) {
	//e.preventDefault();
	var travelers = getValidTravelers();
	//console.log('travelers: ', travelers);
	var dupesFound = hasDuplicateInObject('inputs', travelers);
	//console.log('duplicates: ', dupesFound);
	markDuplicates(travelers);
	if (dupesFound) {
		$('#errorMsg').html(alertHtml);
		e.preventDefault();
	}
});
