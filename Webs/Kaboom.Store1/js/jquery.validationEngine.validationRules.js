(function($) {
    $.fn.validationEngineLanguage = function() { };
    $.validationEngineLanguage = {
        newLang: function() {
            $.validationEngineLanguage.allRules = {
                "required": {
                    "regex": "none",
                    "alertText": "This field is required"
                },
                "Quantity": {
                    "regex": "/^\\d{1,2}$/",
                    "alertText": "Please, enter a valid Quantity"
                },
                "Coupon": {
                    "regex": "/^.+$/",
                    "alertText": "Please, enter a valid Coupon Code"
                },
                "RefererCode": {
                    "regex": "/^[a-zA-Z0-9]+$/",
                    "alertText": "Please, enter a valid Referer Code"
                },
                "FirstName": {
                    "regex": "/^[a-zA-Z_\\-\\.\\,\\(\\)\\'\\\"\\s]{2,}$/",
                    "alertText": "Please, eneter a valid First Name"
                },
                "LastName": {
                    "regex": "/^[a-zA-Z_\\-\\.\\,\\(\\)\\'\\\"\\s]{2,}$/",
                    "alertText": "Please, enter a valid Last Name"
                },
                "Address": {
                    "regex": "/^[a-zA-Z_0-9\\#\\-\\.\\,\\(\\)\\'\\\"\\s]{2,}$/",
                    "alertText": "Please, enter a valid Address"
                },
                "City": {
                    "regex": "/^[a-zA-Z_0-9\\-\\.\\,\\(\\)\\'\\\"\\s]{2,}$/",
                    "alertText": "Please, enter a valid City"
                },
                "Zip": {
                    "regex": "/^\\d{5}$/",
                    "alertText": "Please, enter a valid Zip"
                },
                "ValidatePhone": {
                    "nname": "ValidatePhone",
                    "alertText": "Please, enter a valid Phone"
                },
                "Email": {
                    "regex": "/^[a-zA-Z_0-9\\.\\-]+@([a-zA-Z_0-9\\-]+\\.)+[a-zA-Z0-9]{2,4}$/",
                    "alertText": "Please, enter a valid Email"
                },
                "ValidateCreditCard": {
                    "nname": "ValidateCreditCard",
                    "alertText": "Please, enter a valid Credit Card"
                },
                "CVV": {
                    "regex": "/^[\\d-]+$/",
                    "alertText": "Please, enter a valid Credit Card Security Code"
                },
                "Login": {
                    "regex": "/^[a-zA-Z0-9_\\.\\-]+$/",
                    "alertText": "Please, enter a valid Login"
                },
                "Password": {
                    "regex": "/^[a-zA-Z0-9]+$/",
                    "alertText": "Please, enter a valid Password"
                },
                "ValidatePasswordConfirm": {
                    "nname": "ValidatePasswordConfirm",
                    "alertText": "Please, confirm your Password"
                },
                "ValidateUsernameConfirm": {
                    "nname": "ValidateUsernameConfirm",
                    "alertText": "Please, confirm your Email"
                }
            }
        } 
    }
}
)(jQuery);

$(document).ready(function() {	
	$.validationEngineLanguage.newLang()
});