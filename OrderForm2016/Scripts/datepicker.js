
// http://subpopular.github.io/d-calendar/components/d-calendar/demo.html


var isMobile = {
	Android: function () { return navigator.userAgent.match(/Android/i) != null; },
	BlackBerry: function () { return navigator.userAgent.match(/BlackBerry/i) != null; },
	iOS: function () { return navigator.userAgent.match(/iPhone|iPad|iPod/i) != null; },
	Opera: function () { return navigator.userAgent.match(/Opera Mini/i) != null; },
	Windows: function () { return navigator.userAgent.match(/IEMobile/i) != null; },

	Webkit: function () { return (isMobile.Android() || isMobile.iOS()); },
	any: function () { return (isMobile.Android() || isMobile.BlackBerry() || isMobile.iOS() || isMobile.Opera() || isMobile.Windows()); }
};


function isNationwide() {
	var productId = parseInt($('#product_id').val());
	return (productId == 65 || productId == 66 || productId == 67);
}



var DEBUG = false;


var dateFormat = 'MM/DD/YYYY';


function setDatepickerDefaults() {
	// Datepicker defaults
	$.fn.datepicker.defaults.disableTouchKeyboard = true;
	$.fn.datepicker.defaults.enableOnReadonly = false;
	$.fn.datepicker.defaults.assumeNearbyYear = true;
	$.fn.datepicker.defaults.autoclose = true;
	$.fn.datepicker.defaults.format = 'mm/dd/yyyy';
	$.fn.datepicker.defaults.zIndexOffset = 1000;
	$.fn.datepicker.defaults.templates = {
		leftArrow: '<i class="fa fa-caret-left"></i>',
		rightArrow: '<i class="fa fa-caret-right"></i>'
	};
}


function defaultPickerActions(picker) {
	picker.on('hide', function (e) {
		picker.trigger('blur');
	})
}




// Bind all Events related to Datepicker
function bindDatepickerEvents() {

	var eff = $('[data-daterange="start"]'),
			term = $('[data-daterange="end"]');
	if (eff.length) addRangePickers(eff, term);

	$('.date-purchase').each(addPurchaseDateAttr);
	$('.date-expire').each(addCCExpDateAttr);
	$('.date-dob').each(addBirthDatePicker);


	$(document).on('click', '.input-group-addon', function (e) {
		$(this).siblings('.form-control').focus();
		//input.toggleClass('focused', input.is(':focus'));
	})
}






// NEW METHODS


function fixDateYear(picker) {
	var input = picker.is(':input') ? picker : picker.find('input');
	picker.on('changeDate', function (e) {
		if (input.val().length > 10) {
			// format year to only 4 characters
			var year = e.date.getFullYear().toString();
			e.date.setFullYear(year.substr(0, 4));
			picker.datepicker('update', e.date);
		}
	})
}

function getTimeSpan(start, end, callback) {
	var min = start.datepicker('getDate');
	var max = end.datepicker('getDate');
	//console.log('getTimeSpan: ', start.datepicker(), end.datepicker());
	if (min && max) {
		var days = moment(max).diff(moment(min), 'd') + 1;
		if (callback) callback(days);
		else return days;
	} else {
		return false;
	}
}






function addPurchaseDateAttr(e) {
	var input = $(this);
	if (isMobile.Webkit() || input.is('.mobile')) {
		input.attr({
			'type': 'date',
			'max': moment().subtract(1, 'd').format('YYYY-MM-DD'),
			'placeholder': 'mm/dd/yyyy'
		})
		return;
	}
	else {
		input.attr({
			'data-provide': 'datepicker',
			'data-date-end-date': '-1d',
			'placeholder': 'mm/dd/yyyy'
		})
		defaultPickerActions(input);
	}
}






function addCCExpDateAttr(e) {
	var input = $(this);
	if (isMobile.Webkit() || input.is('.mobile')) {
		var m = moment().startOf('month');
		input.attr({
			'type': 'month',
			'min': m.format('YYYY-MM'),
			'max': m.add(20, 'y').format('YYYY-MM')
		})
		return;
	}
	else {
		input.attr({
			'data-provide': 'datepicker',
			'data-date-format': 'mm/yy',
			'data-date-start-date': '0d',
			'data-date-end-date': '+20y',
			'data-date-start-view': 'year',
			'data-date-min-view-mode': 'months',
			'placeholder': 'mm/yy'
		})
		defaultPickerActions(input);
	}
}






//function addBirthDateAttr(e) {
//	var input = $(this), viewDate = '';
//	var effDate = input.data('effDate');
//	var memberAge = input.data('memberAge');
//	if (effDate && memberAge != undefined) {
//		viewDate = moment(effDate, dateFormat)
//		.subtract(memberAge, 'y')
//		.format(dateFormat);
//	}
//	input.attr({
//		'data-provide': 'datepicker',
//		'data-date-default-view-date': viewDate,
//		'data-date-start-view': 'decade',
//		'placeholder': 'mm/dd/yyyy',
//		'pattern': '\d{1,2}/\d{1,2}/\d{4}'// might not be necessary, but definitely use for mobiles
//	})
//	defaultPickerActions(input);
//}

function addBirthDatePicker(e) {
	var input = $(this), viewDate = '';
	var effDate = input.data('effDate');
	var memberAge = input.data('memberAge');
	if (effDate && memberAge != undefined) {
		effDate = moment(effDate, dateFormat);
		viewDate = moment(effDate).subtract(memberAge, 'y').format(dateFormat);
	}
	if (isMobile.Webkit() || input.is('.mobile')) {
		addBirthDatePickerMobile(input, effDate, memberAge)
		return;
	}
	else {
		input.datepicker({
			defaultViewDate: viewDate,
			startView: 'decade',
		})
		.on('hide', function (e) {
			if (DEBUG) console.log('dob => onHide: ', e);
			if (e.date && effDate && memberAge >= 0) {
				// Age at time of travel.
				var isBorn = effDate.diff(moment(e.date), 'days', false) > 0;
				if (!isBorn) { AgeBands.notBorn(input[0]); }
				else {
					var selectedAge = effDate.diff(moment(e.date), 'years', false);
					$('#output').append('<li><strong>Selected Age: </strong> ' + selectedAge + '</li>');
					AgeBands.checkAge(input[0], selectedAge);
					//if (input.data('update')) $(input.data('update')).text(selectedAge);
				}
			}
			input.trigger('blur');
		})
	}
}

function addBirthDatePickerMobile(input, effDate, memberAge) {
	var dateFormat = 'YYYY-MM-DD';
	input.attr({
		'type': 'date',
		'placeholder': 'mm/dd/yyyy'
	})
	.on('input', function (e) {
		if (effDate && memberAge >= 0) {
			// Age at time of travel.
			var isBorn = effDate.diff(moment(this.value), 'days', false) > 0;
			if (!isBorn) { AgeBands.notBorn(this); }
			else {
				var selectedAge = effDate.diff(moment(this.value), 'years', false);
				$('#output').append('<li><strong>Selected Age: </strong> ' + selectedAge + '</li>');
				AgeBands.checkAge(this, selectedAge);
				//if (input.data('update')) $(input.data('update')).text(selectedAge);
			}
		}
		//input.trigger('blur');
	})
}






function addRangePickers(start, end) {
	if (isMobile.Webkit()) {
		addRangePickersMobile(start, end);
		return;
	}
	else {
		start.datepicker({ startDate: '+1d' })
		.on('hide', function (e) {
			if (DEBUG) console.log('start => onHide: ', e);
			if (e.date) {
				var minDate = moment(e.date).add(1, 'd').format(dateFormat);
				end.datepicker('setStartDate', minDate);
				if (isNationwide()) {
					var maxDate = moment(e.date).add(89, 'd').format(dateFormat);
					end.datepicker('setEndDate', maxDate);
				}
			}
			start.trigger('blur');
		})
		end.datepicker({ startDate: '+2d' })
		.on('hide', function (e) {
			if (DEBUG) console.log('end => onHide: ', e);
			if (e.date) {
				var maxDate = moment(e.date).subtract(1, 'd').format(dateFormat);
				start.datepicker('setEndDate', maxDate);
			}
			end.trigger('blur');
		})
	}
}

function addRangePickersMobile(start, end) {
	var dateFormat = 'YYYY-MM-D';
	var pattern = "[0-9]{4}-[0-9]{2}-[0-9]{2}";

	start.attr({
		'type': 'date',
		'min': moment().add(1, 'd').format(dateFormat),
		'placeholder': 'mm/dd/yyyy'
	})
	.on('input', function (e) {
		var minDate = moment(this.value).add(1, 'd');
		end.attr('min', minDate.format(dateFormat));
		if (isNationwide()) {
			var maxDate = moment(this.value).add(89, 'd');
			end.attr('max', maxDate.format(dateFormat));
		}
	})
	end.attr({
		'type': 'date',
		'min': moment().add(2, 'd').format(dateFormat),
		'placeholder': 'mm/dd/yyyy'
	})
	.on('input', function (e) {
		var maxDate = moment(this.value).subtract(1, 'd');
		start.attr('max', maxDate.format(dateFormat));
	})
}






/* <input type="date" id="cal"> */
//function openPicker(inputDateElem) {
//	var ev = document.createEvent('KeyboardEvent');
//	ev.initKeyboardEvent('keydown', true, true, document.defaultView, 'F4', 0);
//	inputDateElem.dispatchEvent(ev);
//}

//var cal = document.querySelector('#cal');
//cal.focus();
//openPicker(cal);














// OLD METHODS

function getTravelDate() {
	var date = $('#effDate').datepicker('getDate');
	return date ? moment(date, dateFormat) : moment();
}


function updateAllAgeBadges() {
	$('[name$="travelerDOB"]').each(function () {
		var input = $(this), dob = input.val();
		if (dob) {
			var age = getTravelDate().diff(moment(dob, dateFormat), 'years');
			input.closest('.row').find('[name$="travelerAge"]').val(age);
			input.closest('.row').find('label>.badge').text(age);
		}
	});
}








// Bind all Events related to Travelers
function bindTravelerEvents() {
	var numberOfTravelers = $('#numberOfTravelers');
	var addTravelers = $('#addTravelers');

	numberOfTravelers
	.on('focus click', function (e) {
		this.select();
	})
	.on('input', function (e) {
		var add = numberOfTravelers[0].value;
		// Make sure at least 1 remains
		add = Math.max(add, 1);
		numberOfTravelers.val(add);
		var total = $('.traveler').length;
		if (add && add != total) {
			//var icon = add > total ? 'plus' : 'minus';
			//addTravelers.prop('disabled', false)
			//.html('Travelers <i class="fa fa-' + icon + '-circle"></i>');
			var icon = add > total ? 'Add' : 'Remove';
			addTravelers.prop('disabled', false)
			.html(icon + ' Travelers').removeClass('hidden');
		} else {
			//addTravelers.prop('disabled', true)
			//.html('Travelers <i class="fa fa-check-circle"></i>');
			addTravelers.prop('disabled', true)
			.html('Travelers').addClass('hidden');
		}
	});

	addTravelers.on('click', function (e) {
		var add = numberOfTravelers.val();
		var total = $('.traveler').length;
		updateBHTravelerList(total, add);
		//$(this).prop('disabled', true)
		//.html('Travelers <i class="fa fa-check-circle"></i>');
		$(this).prop('disabled', true)
		.html('Travelers').addClass('hidden');

		addTravelers.trigger('postclick');
	});


	var destination = $('#destination');
	var stateSelect = $('#stateSelect');

	destination.on('change', function (e) {
		if (this.value === 'US') {
			stateSelect.collapse('show');
		}
		else {
			stateSelect.collapse('hide');
			stateSelect.find('.form-group')
			.removeClass('has-success has-error')
			.find('.form-control').val('');
		}
	});

	//$('#bh-travelers .dob').each(createPriorPicker);

	if (typeof Travelers === 'function') {
		var kidsallowed = [14, 17, 38, 39];
		var pId = parseInt($('#product_id').val());
		if (kidsallowed.indexOf(pId) >= 0) {
			var children = new Travelers('#children');
		}
		else {
			var travelers = new Travelers('#travelers');
		}
	}
}


// Traveler info inputs
function updateBHTravelerList(total, add) {
	//var template = $('#primary').clone().removeAttr('id').html();
	//template = template.replace(/\_(\d+)\__/g, '_{x}__').replace(/\[(\d+)\]./g, '[{x}].');
	var template = $('#primary').clone();
	template.find(':input').removeAttr('aria-invalid');
	template = template.html().replace(/\_(\d+)\__/g, '_{x}__').replace(/\[(\d+)\]./g, '[{x}].');
	//console.log(template);
	if (add > total) {
		var fragment = $(document.createDocumentFragment());
		for (var i = total; i < add; i++) {
			var row = $('<div/>').attr('data-traveler', i).addClass('traveler');
			row.append(template.replace(/{x}/g, i));
			fragment.append(row);
		}
		fragment.find('.has-success').removeClass('has-success');
		fragment.find('.has-error').removeClass('has-error');
		//fragment.find(':input').val('');
		fragment.find(':input').each(function (e) {
			var input = $(this);
			input.val(input[0].defaultValue);
			if (input.is('[type=number]')) {
				input.attr('value', 0);
				input.val(0);
			}
		});
		fragment.find('.badge').text('');
		fragment.find('.dob').each(createPriorPicker);
		fragment.appendTo($('#bh-travelers'));
	}
	if (add < total) {
		var rows = $('.traveler')
		rows.each(function (e) {
			var row = $(this);
			if (row.data('traveler') >= add) {
				row.remove();
			}
		});
	}
}





function bindEvents() {
	//alert('bindEvents');
	setDatepickerDefaults();
	bindDatepickerEvents();
	bindTravelerEvents();
}




$(function () {
	

	bindEvents();


	$('body').on('change', 'select.form-control', function (e) {
		$(this).trigger('blur');
	});
	
	
	//$('body').on('show hide', '.input-group.date', function (e) {
	//	$(this).find('input').toggleClass('focus', e.type == 'show');
	//});
	
	
	var baseForm = $('#baseform');
	//var formAny = $('#baseform, #optionsform, #memberform, #ccform');
	//var state = new StateManager(baseForm, bindEvents);
	
	if (baseForm.length) {
		
		// Add validation to form elements
		var formValidator = baseForm.validate({
			
			debug: false,
			errorClass: 'has-error',
			validClass: 'has-success',
			
			highlight: function (element, errorClass, validClass) {
				$(element).closest('.form-group')
				.addClass(errorClass).removeClass(validClass);
			},
			unhighlight: function (element, errorClass, validClass) {
				$(element).closest('.form-group')
				.addClass(validClass).removeClass(errorClass);
			},
			showErrors: function (errorMap, errorList) {
				// Clean up any tooltips for valid elements
				$.each(this.validElements(), function (index, element) {
					//var $element = $(element).not('[readonly]');
					var $element = $(element);
					$element.tooltip('destroy')
					.data('title', '')
					.closest('.form-group')
					.addClass('has-success')
					.removeClass('has-error');
				});
				// Create new tooltips for invalid elements
				$.each(errorList, function (index, error) {
					var $element = $(error.element);
					$element.tooltip('destroy')
					.data('title', error.message)
					.tooltip({ placement: 'top' })
					.closest('.form-group')
					.addClass('has-error')
					.removeClass('has-success');
				});
			},
			rules: {
				'eff_date': {
					required: true, dateLessThan: "#term_date"
				},
				'term_date': {
					required: true, dateGreaterThan: "#eff_date"
				},
				'CCPartial.school_name': {
					required: true
				},
				'CCPartial.spouseAge': {
					required: {
						depends: function () {
							return ($('#CCPartial_includeSpouse').is(':checked'));
						}
					},
					min: 1
				},
				'TravelerAges[0].travelerAge': {
					required: true,
					min: 1
				},
				'country': {
					required: true,
					notEqualTo: '#destination'
				},
				'destination': {
					required: true,
					notEqualTo: '#country'
				}
			},
			submitHandler: function (form) {
				if (formValidator.form()) {
					//state.saveFormState();
					normalizeDates();
					form.submit();
				}
			}
		});
		
		//if (formValidator.form()) {}
		
		$('#submit').on('click', function (e) {
			$('#output').empty();
			output('Trip Begins', $('#eff_date').val());
			output('Trip Ends', $('#term_date').val());
			output('Purchase Date', $('#purchase_date').val());
			output('Date of Birth', $('#birth_date').val());
			output('Expire Date', $('#expirationDate').val());
			//baseForm.submit();
		});
	}
	
});



function output(key, value) {
	var output = $('#output');
	var keyStr = key ? '<strong>' + key + ': </strong>' : '<br>';
	//output.append('<li>' + keyStr + ' ' + value + '</li>');
	var item = $('<li><strong>' + keyStr + '</strong> <span></span></li>');
	output.append(item);
	item.find('span').text(value);
}


//$.fn.equalizeHeights = function () {
//	var maxHeight = this.map(function (i, e) {
//		return $(e).height();
//	}).get();
//	return this.height(Math.max.apply(this, maxHeight));
//};