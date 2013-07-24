internalLink = false;
$(document).ready(function() {

    validator = new IHS.validator();

    validator.addValidator('First_Name',
		        [function(el) {

		            return (el.value.replace(/\s/g, '').length > 0) || "Please enter your first name.";

		        }, function(el) {
		            return IHS.expressions.name.test(el.value) || "Please, enter a valid first name.";
		        } ]);
    validator.addValidator('Last_Name',
		        [function(el) {

		            return (el.value.replace(/\s/g, '').length > 0) || "Please enter your last name.";

		        }, function(el) {
		            return IHS.expressions.name.test(el.value) || "Please, enter a valid last name.";
		        } ]);
    validator.addValidator('Billing_Address_1',
		        [function(el) {

		            return (el.value.replace(/\s/g, '').length > 0) || "Please enter your address.";

		        }, function(el) {
		            return IHS.expressions.address.test(el.value) || "Please, enter a valid address.";
		        } ]);
    validator.addValidator('Billing_Address_2',
		        [function(el) {
		            return IHS.expressions.address.test(el.value) || "Please, enter a valid address.";
		        } ]);
    validator.addValidator('City',
		        [function(el) {

		            return (el.value.replace(/\s/g, '').length > 0) || "Please enter your city.";

		        }, function(el) {
		            return IHS.expressions.name.test(el.value) || "Please, enter a valid city.";
		        } ]);
    validator.addValidator('Home_Tel1',
		        [function(el) {

		            return (el.value.replace(/\s/g, '').length > 0) || "Please enter your phone.";

		        }, function(el) {
		            return /^\d{3}$/.test(el.value) || "Please, enter a valid phone.";
		        } ]);
    validator.addValidator('Home_Tel2',
		        [function(el) {

		            return (el.value.replace(/\s/g, '').length > 0) || "Please enter your phone.";

		        }, function(el) {
		            return /^\d{3}$/.test(el.value) || "Please, enter a valid phone.";
		        } ]);
    validator.addValidator('Home_Tel3',
		        [function(el) {

		            return (el.value.replace(/\s/g, '').length > 0) || "Please enter your phone.";

		        }, function(el) {
		            return /^\d{4}$/.test(el.value) || "Please, enter a valid phone.";
		        } ]);
    validator.addValidator('Zip1',
		        [function(el) {

		            return (el.value.replace(/\s/g, '').length > 0) || "Please enter your zip.";

		        }, function(el) {
		            return /^\d{5}$/.test(el.value) || "Please, enter a valid zip.";
		        } ]);
    validator.addValidator('Email',
		        [function(el) {

		            return (el.value.replace(/\s/g, '').length > 0) || "Please enter your email.";

		        }, function(el) {
		            return IHS.expressions.mail.test(el.value) || "Please, enter a valid email.";
		        } ]);

    validator.addValidator('CC_Number',
		        [function(el) {

		            return (el.value.replace(/\s/g, '').length > 0) || "Please enter your credit card number.";

		        }, function(el) {
		            return Mod10(el.value) || "Please, enter a valid credit card.";
		        } ]);

    validator.addValidator('CVV',
		        [function(el) {

		            return (el.value.replace(/\s/g, '').length > 0) || "Please enter your card security code.";

		        }, function(el) {
		            return /^[\d-]+$/.test(el.value) || "Please, enter a valid card security code.";
		        } ]);

    with ($('#Agree_With_Terms')) {
        if (length > 0) {
            validator.addValidator(get(0),
	                [function(el) {

	                    return (el.checked) || "Please check the agreement checkbox if you wish to continue.";

	                } ]);
        }
    }

    IHS.validate = function() {
        if (validator.isValid()) {
            internalLink = true;
            var ht = $('#Home_Tel');
            if (ht.length > 0) {
                ht.val($('#Home_Tel1').val() + $('#Home_Tel2').val() + $('#Home_Tel3').val());
            }
            $('#_action').val('order');
            $('form').submit();

        }

    }

    var setPrice = function(el) {
        var bPrice = $(el).parent().parent().find('.bPrice').html();
        bPrice = parseFloat(bPrice);
        bPrice = (bPrice + 3.95).toFixed(2);
        if ($('#discount').val().length > 0) {
            bPrice = (bPrice - bPrice * $('#discount').val()).toFixed(2);
        }
        $('#total').html('$' + bPrice);
    };
    $('#tblOptions input.subs').click(function() {
        setPrice(this);
    });

    $('#tblOptions input.subs').each(function() {
        if (this.checked) {
            setPrice(this);
        };
    });


});

var submitCoupon = function(el) {
    $('#_action').val('coupon');
    $('form').attr('action', '');
    $('form').submit();
};