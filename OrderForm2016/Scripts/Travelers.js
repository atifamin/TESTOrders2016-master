
var Travelers = function(element, callback) {
	this.element = $(element);
	this.onUpdate = callback;
	this.PER_ROW = 1;//4
	
	if (!this.element.length) {
		return false;
	}
	return this.init();
};


Travelers.prototype = {
	
	init: function () {
		//this.productId = parseInt($('#product_id').val()) || null;

		this.template = this.element.find('input').first().clone();
		this.template.removeClass('valid').val(0);
		
		this.tempName = this.template[0].name.replace(/\[(\d+)\]/g, '[{x}]');
		this.tempId = this.template[0].id.replace(/\_(\d+)\_/g, '_{x}_');
		
		var target = $(this.element.data('counter'));
		
		if (target.length && target.is('input')) {
			this.attachCounter(target);
		}
		else if (!target.length) {
			if (this.PER_ROW == 1) {
				this.button = this.createButton(this.element.selector);
			}
		}
		
		this.bindEvents('body');
		//this.bindEvents(document);

		return this;		
	},
	
	bindEvents: function(container) {
		var self = this;
		var inputs = this.travelerSelector();
		$(container)
		// onclick needed for IE and Edge
		.on('click focus', inputs, function (e) {
			this.select();
			// Needed to work on mobile Safari
			//this.setSelectionRange(0, this.value.length);
		})
		.on('blur', inputs, function (e) {
			var input = $(this);
			//console.log(input);
			if (input.attr('max')) {// Maybe unneccessary!!!
				var max = parseInt(input.attr('max'));
				var value = parseInt(input.val());
				// Correct the value to the max
				//input.val(Math.min(value, max));
				// Or alert the user
				/*if (value > max) {
					input.closest('.traveler').addClass('has-error');
				} else {
					input.closest('.traveler').removeClass('has-error');
					self.sortAll(inputs);
				}*/
			}
			self.sortAll(inputs);
		})
		.on('keydown', inputs + ':last-child', function (e) {
			var code = e.keyCode || e.which;
			if (self.button && code === 9) {
				e.preventDefault();
				self.button.trigger('click');
			}
		});
	},
	
	travelerSelector: function () {
		return (this.element.selector + ' input');
	},
	
	travelerCount: function () {
		return this.element.find('input').length;
	},
	
	getMaxId: function (inputs) {
		var max = -1;
		$(inputs || this.element.find('input')).each(function () {
			var number = this.id.match(/\_(\d+)\_/)[1];
			max = Math.max(parseInt(number), max);
		});
		return parseInt(max);
	},	
	
	// Push all empty/zero fields to the end.
	sortAll: function (inputs) {
		var lastEmpty = null;
		$(inputs).each(function () {
			// Keep the value in the positive(+).
			this.value = parseInt(this.value) || 0;
			var val = Math.max(this.value, 0);
			if (!val && !lastEmpty) {
				lastEmpty = this;
			}
			if (val && lastEmpty) {
				lastEmpty.value = val;
				this.value = 0;
				lastEmpty = this;
			}
		});
	},
	
	// Adds the (+) button after the last field
	createButton: function (target) {
		var self = this;
		var btn = this.element.find('#addTraveler');
		if (!btn || !btn.length) {
			var wrapper = $('<div class="button col-xs-3"></div>');
			btn = $('<button type="button" id="addTraveler" />')
			.html('<i class="fa fa-plus-circle"></i>')
			.addClass('btn btn-default')
			//.data('target', target);
			.attr('data-target', target);
			this.element.append(wrapper.append(btn));
		}
		btn.on('click', function (e) {
			var last = self.element.find('input:last');
			var value = parseInt(last.val());
			//console.log(last, value > 0, value <= last.attr('max'));
			if (value > 0 && value <= parseInt(last.attr('max'))) {
				// re-insert the field before this button.
				var added = self.addTravelers.apply(self);
				btn.parent().before(added.parent());
				//added[0].offsetWidth;// force reflow
				added.focus();
				return added;
			} else {
				// add a tooltip or something here
				last.focus();
			}
		});
		
		return btn;
	},
	
	addTravelers: function (e) {
		var added;
		var last = this.element.find('input:last');
		var maxAge = this.getMaxId();
		//if (($.trim(last.val()) && last.valid()) || maxAge < 0) {
		if ($.trim(last.val()) || maxAge < 0) {
			if (this.PER_ROW == 1) {
				// Add ONLY ONE Age input at a time.
				added = this.addSingle(this.element, maxAge + 1);
				if (e !== false) { added.focus(); }
			}
			else {
				// Add TWO(2+) or more Age inputs at a time.
				added = this.addMultiple(this.element, maxAge, this.PER_ROW);
			}
			// If callback function passed in.
			if (this.onUpdate) this.onUpdate(added);
		}
		return added;
	},
	
	addSingle: function (target, id) {
		var html = $('<div class="traveler col-xs-3"></div>');
		// Clone and change: value, id, name
		var input = this.template.clone().attr({
			id: this.tempId.replace('{x}', id),
			name: this.tempName.replace('{x}', id)
		});
		$(target).append(html.append(input));
		return input;
	},
	
	addMultiple: function (target, id, count) {
		var inputs = [], row = $('<div class="group"></div>');
		for (var i = id; i < id + count;) {
			inputs.push(this.addSingle(row, ++i).focus());
		}
		$(target).append(row);
		return inputs;
	},
	
	removeLast: function () {
		var inputs = this.element.find('input');
		var last = inputs.last().closest('.traveler');
		if (last.length) last.remove();
	},
	
	attachCounter: function (input) {
		var self = this;
		input.on('keyup input', function (e) {
			var total = Math.max(input.val(), 0);
			if (input.attr('max')) {
				total = Math.min(total, input.attr('max'));
				input.val(total);
			}
			while (total != self.travelerCount()) {
				if (total > self.travelerCount()) {
					self.addTravelers(false);
				} else {
					self.removeLast();
				}
			}
		})
	}
	
};

//var travelers = new Travelers('#travelers');



