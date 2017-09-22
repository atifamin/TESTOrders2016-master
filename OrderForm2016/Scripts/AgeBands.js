
var AgeBands = {
	
	element: $('#ageband'), 
	input: null, 
	debug: false, 
	
	Event: {
		OVER_99: 0, 
		LOADING: 1, 
		OUTSIDE: 2, 
		UPDATE: 3, 
		ERROR: 4 
	},
	
	
	isDate: function (dateStr) {
		var date = new Date(dateStr);
		return (date instanceof Date && !isNaN(date.valueOf()));
	},	
	
	
	/*setMessage: function (code, data) {
		if (AgeBands.debug) console.log('AgeBands.setMessage('+code+', '+data+')');
		
		var title = '', message = '';
		switch (code) {
			case AgeBands.Event.OVER_99:
				title = 'We are very sorry';
				message = '<h4 class="widget-title-">The age you submitted was <span class="text-danger">' + AgeBands.selectedAge;
				message += '</span> and there are currently NO plans available for travelers over 99.</h4>';
				break;
			case AgeBands.Event.LOADING:
				title = 'Updating Quote';
				message = '<div id="loading" class="loading-screen"></div>';
				break;
			case AgeBands.Event.OUTSIDE:
				title = 'There appears to be a problem';
				message = '<h4 class="text-info">The quote was based on the age you originally entered <span class="text-muted">(' + AgeBands.input.dataset.memberAge + ')</span>.</h4>';
				message += '<h5>Click "Update Quote" for a quote based on the birthdate you entered <span class="text-muted">(' + AgeBands.input.value + ')</span>.</h5>';
				break;
			case AgeBands.Event.UPDATE:
				title = 'New Quote Amount';
				message = '<h4 class="text-success">Your new quote is $' + data.amount.toFixed(2) + '.</h4>';
				break;
			case AgeBands.Event.ERROR:
				title = 'Error';
				message = '<h4 class="text-danger">Error: ' + data.error + '</h4>';
				break;
		}
		AgeBands.element.find('.modal-title').text(title);
		AgeBands.element.find('.modal-body').html('<div class="text-left">' + message + '</div>');
	},
	
	
	setButtons: function (code) {
		if (AgeBands.debug) console.log('AgeBands.setButtons('+code+')');
		
		var updateBtn = AgeBands.element.find('#ab-update');
		var cancelBtn = AgeBands.element.find('#ab-cancel');
		var closeBtn = AgeBands.element.find('[data-dismiss]');
		
		closeBtn.text(code == AgeBands.Event.UPDATE ? 'Accept' : 'Close');
		
		switch (code) {
			case AgeBands.Event.OVER_99:
				closeBtn.one('click', AgeBands.resetAge);
				break;
			case AgeBands.Event.OUTSIDE:
				cancelBtn.one('click', AgeBands.cancel);
				updateBtn.one('click', AgeBands.updateResults);
				break;
		}
		
		switch (code) {// show and hide
			case AgeBands.Event.OUTSIDE:
				updateBtn.removeClass('hidden');
				cancelBtn.removeClass('hidden');
				closeBtn.addClass('hidden');
				break;
			case AgeBands.Event.OVER_99:
			case AgeBands.Event.UPDATE:
			case AgeBands.Event.ERROR:
				updateBtn.addClass('hidden');
				cancelBtn.addClass('hidden');
				closeBtn.removeClass('hidden');
				break;
		}
	},*/
	
	
	resetAge: function (event) {
		if (AgeBands.debug) console.log('AgeBands.resetAge', event);

		var input = AgeBands.input;
		var preValue = input.dataset.defaultValue || input.defaultValue;

		input = $(input);
		if (input.data('datepicker')) {
			input.datepicker('update', AgeBands.isDate(preValue) ? preValue : '');
		} else { input.val(AgeBands.isDate(preValue) ? preValue : ''); }
		input.closest('.form-group').removeClass('has-success has-error');
	},
	
	
	updateAge: function (event) {
		if (AgeBands.debug) console.log('AgeBands.updateAge', event);

		var input = AgeBands.input, age = AgeBands.selectedAge;
		input.dataset.defaultValue = input.value;
		input.dataset.memberAge = age;
		var displayAge = age > 0 ? age : 'Less than a year';
		$(input).closest('fieldset').find('.widget-subtitle>.age').text(displayAge);
	},
	
	
	cancel: function (event) {
		if (AgeBands.debug) console.log('AgeBands.cancel', event);

		AgeBands.resetAge();
		AgeBands.element.modal('hide');
	},


	notBorn: function (input) {
		if (AgeBands.debug) console.log('AgeBands.notBorn', input);

		AgeBands.input = input;
		swal({
			title: "Did you enter the correct date of birth?",
			text: "The date you just entered would make you not yet born at the time of travel. You're going to have to try again.",
			type: 'error',
			showCancelButton: false,
			confirmButtonColor: '#DD6B55',
			confirmButtonText: 'Try Again'
		},
		function (isConfirm) {
			AgeBands.resetAge();
		});
	},

	
	checkAge: function (input, selectedAge) {
		if (AgeBands.debug) {
			console.clear();
			console.log('AgeBands.checkAge', input, selectedAge);
		}

		AgeBands.input = input;
		AgeBands.selectedAge = selectedAge;
		
		// show error for any age over 99
		if (selectedAge > 99) {// product 66 maxAge = 80
			swal({
				title: 'We are very sorry',
				text: 'The age you submitted was ' + AgeBands.selectedAge + ' and there are currently NO plans available for travelers over 99.',
				confirmButtonColor: '#DD6B55',
				confirmButtonText: 'OK'
			},
			function () {

			});
			return;
		}
		
		if (!input.dataset.defaultValue) {
			input.dataset.defaultValue = 'mm/dd/yyyy';
		}
		
		var isMatrix = Boolean(input.dataset.matrix);
		var mid = parseInt(input.dataset.memberId);
		var params = { 'ageFromDOB': selectedAge, 'memID': mid };
		
		$.post('/Home/CheckMemberAgeBand', params)		
		.done(function (data) {
			if (AgeBands.debug) console.log('AgeBands.checkAge => /Home/CheckMemberAgeBand/', data);

			if (!data.isInAgeBand) {
				// Wrong age band, rerun quote...
				var ageMsg = AgeBands.selectedAge > 0 ? AgeBands.selectedAge : 'less than a year old';
				var message = '<strong>This quote is based on the age you previously entered <span class="text-muted">(' + input.dataset.memberAge + ')</span>.</strong><br><br>';
				message += 'The date you just entered would make you ' + ageMsg + ' at the time of travel. This might alter the cost some, is that OK?';

				// inside return from check age ajax
				swal({
					title: 'Did you enter the correct date of birth?',
					text: message,
					type: 'warning',
					showCancelButton: true,
					confirmButtonColor: '#DD6B55',
					confirmButtonText: isMatrix ? 'Yes, start over' : 'Yes, update my quote',
					cancelButtonText: 'No, cancel',
					closeOnConfirm: false,
					//closeOnCancel: false, 
					showLoaderOnConfirm: true,
					html: true
				},
				function (isConfirm) {
					if (isConfirm) {
						if (isMatrix) {
							// go back to baseform
							sessionStorage.clear();
							window.location.replace('/?base_form_id=' + $('#bFormID').val());
						}
						else {
							AgeBands.updateResults();
						}
					} else {
						// reset input back to previous date
						//swal('Cancelled', 'Sorry about that.', 'error');
						AgeBands.resetAge();
					}
				});
			}
			else {
				AgeBands.updateAge();
			}
		})
		.fail(function (data) {
			console.log('Error: ' + data);
		})
	},
	
	
	updateResults: function () {
		if (AgeBands.debug) console.log('AgeBands.updateResults');

		var input = AgeBands.input;
		var mid = parseInt(input.dataset.memberId);
		var age = AgeBands.selectedAge;
		var params = { 'dob': input.value, 'ageFromDOB': age, 'memID': mid };
		
		$.post('/Home/UpdateMemberAgeBand', params)		
		.done(function (data) {
			if (data.error) {
				//swal('Error', data.error, 'error');
				//AgeBands.resetAge();
				swal({
					title: 'Error',
					text: data.error,
					type: 'error',
					showCancelButton: true,
					confirmButtonColor: '#DD6B55',
					confirmButtonText: 'Start Over',
					cancelButtonText: 'Wrong DOB',
					//closeOnConfirm: false,
					//closeOnCancel: false, 
					showLoaderOnConfirm: true,
					html: true
				},
				function (isConfirm) {
					if (isConfirm) {
						// go back to baseform
						sessionStorage.clear();
						window.location.replace('/?base_form_id=' + $('#bFormID').val());
					} else {
						AgeBands.resetAge();
					}
				});
			}
			else {
				swal('Your quote has been updated!', 'The new price is: $' + data.amount.toFixed(2), 'success');
				AgeBands.updateAge();
			}
		})
		.fail(function (data) {
			console.log('Error: ' + data);
		})
	}

};



