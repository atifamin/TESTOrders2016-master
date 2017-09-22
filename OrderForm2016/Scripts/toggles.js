
$(document).ready(function () {


	/*
	<input type="radio" id="acctCheck" name="acctCheck" value="checkToAddr" checked>
	<input type="radio" id="acctCheck" name="acctCheck" value="checkToOther" data-require-field="#acctCheckToAddr">
	<input type="radio" id="acctCheck" name="acctCheck" value="ACH" data-require-field="#acctBankName,#acctName,#acctNum,#acctRouting,#acctSWIFT,#acctBankAddr">
	
	<div class="collapse" data-radio-toggle="acctCheck" data-radio-value="checkToOther">
		<textarea class="form-control" id="acctCheckToAddr" name="acctCheckToAddr" rows="3" required></textarea>
	</div>
	<div class="collapse" data-radio-toggle="acctCheck" data-radio-value="ACH">
		<input class="form-control" id="acctBankName" name="acctBankName" type="text">
		<input class="form-control" id="acctName" name="acctName" type="text">
		<input class="form-control" id="acctNum" name="acctNum" type="text">
		<input class="form-control" id="acctRouting" name="acctRouting" type="text">
		<input class="form-control" id="acctSWIFT" name="acctSWIFT" type="text">
		<input class="form-control" id="acctBankAddr" name="acctBankAddr" type="text">
	</div>
	*/
	//$('[data-radio-toggle]').each(function (e) {
	//	var wrapper = $(this).addClass('collapse');
	//	var name = this.dataset.radioToggle;
	//	var value = this.dataset.radioValue;
	//	$('[name="' + name + '"]').on('click', function (e) {
	//		wrapper.toggleClass('collapse', this.value != value);
	//	});
	//});


	/*
	<input type="checkbox" id="OtherCarrier" name="OtherCarrier" value="true" data-require-field="#CarrierInfo">
	
	<div class="collapse" data-checkbox-toggle="OtherCarrier">
		<textarea class="form-control" id="CarrierInfo" name="CarrierInfo" rows="3" required></textarea>
	</div>
	*/
	//$('[data-checkbox-toggle]').each(function (e) {
	//	var wrapper = $(this).addClass('collapse');
	//	var name = this.dataset.checkboxToggle;
	//	var value = this.dataset.checkboxValue || 'true';
	//	$('[name="' + name + '"]').on('click', function (e) {
	//		var isChecked = value == 'false' ? this.checked : !this.checked;
	//		wrapper.toggleClass('collapse', isChecked);
	//	});
	//});





	/*
	<input type="checkbox" id="OtherCarrier" name="OtherCarrier" value="true" data-require-field="#CarrierInfo">
	
	<div class="collapse" data-check-toggle="[name=OtherCarrier]" data-check-value="true">
		<textarea class="form-control" id="CarrierInfo" name="CarrierInfo" rows="3" required></textarea>
	</div>
	*/
	$('[data-check-toggle]').each(function (e) {
		var wrapper = $(this);
		var input = $(this.dataset.checkToggle);
		var value = this.dataset.checkValue || 'true';
		function toggle(e) {
			var isChecked = value == 'false' ? !this.checked : this.checked;
			if (input.is(':radio')) isChecked = this.value == value;
			wrapper.toggleClass('collapse', !isChecked);
		}
		input.on('click', toggle);
		if (input.length > 1) {
			$(input.selector + ':checked').trigger('click');
		} else toggle.call(input);
	});





	/*
	<input type="checkbox" id="GBGContacted" name="GBGContacted" value="true" data-require-field="#GBGFileNumber">
	<input type="text" class="form-control" id="GBGFileNumber" name="GBGFileNumber" required>
	*/
	$('[data-require-field]').each(function (e) {
		var _this = $(this), isRadio = false, reqValue;
		var name = this.dataset.requireField;

		if (_this.is(':radio')) {
			isRadio = true;
			reqValue = this.value;
			_this = $('[name="' + this.name + '"]');
		}
		else if (this.dataset.requireValue) {
			if (this.dataset.requireValue == 'false') {
				$(name).prop('required', true);
			}
		}

		_this.on('click', function (e) {
			var isChecked = reqValue ? this.value == reqValue : this.checked;
			if (!isRadio && this.dataset.requireValue == 'false') isChecked = !this.checked;
			var field = $(name).prop('required', isChecked);
			if (isChecked) field[0].focus();
		});
	});


	$('[data-reset-radio]').each(function (e) {
		var _this = $(this);
		var name = this.dataset.resetRadio;
		var field = $(name);
		var _default = field.val();

		if (field.length > 1) {
			var target = $($.grep(field, function (x) {
				return x.value === _default
			}));
			field = $(target);
		}
		field.data('default', _default);
		//console.log(field.data());

		_this.on('click', function (e) {
			console.log(field);
			field.val(field.data('default')).trigger('click');
			//field.prop('required', false);
		});
	});



})