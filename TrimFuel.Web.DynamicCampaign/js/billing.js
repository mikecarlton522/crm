var IHS = IHS || {};
function displayShipping(cb) {
    if (cb != null && cb != 'undefined') {
        if (cb.checked) {
            $('#shippingTable').slideDown(300).find('input').removeClass("noValidate");
        }
        else {
            $('#shippingTable').slideUp(300).find('input').addClass("noValidate");
        }
    }
}

IHS.submitHandler = function(form) {
    if (IHS.isAsync) {
        $('#__action').val('BillingAsync');
        $("select").hide();
        $("body").append("<div class='progress_overlay'></div>").css("filter", "alpha(opacity=80)");
        $('.progress_load').fadeIn(500);
        $(form).ajaxSubmit({ dataType: "json", success: IHS.callback });
        self.canSubmit = true;
    }
    else
        form.submit();
}

IHS.onLoad = function() {

    //$('#Ship_To_Different_Address').click(function() { displayBilling(this) });
    changeSBCountry("Billing");
    changeSBCountry("Shipping");
    var el = document.getElementById("Shipping_Country");
    if (!!el)
        changeCountry(el);
    el = document.getElementById("Billing_Country");
    if (!!el)
        changeCountry(el);

    displayShipping($('#Ship_To_Different_Address')[0]);
    $('.country').each(function() { changeCountry(this) });
    var script = document.createElement("script");

    script.type = "text/javascript";
    script.src = "js/jquery.form.js";

    document.getElementsByTagName("head")[0].appendChild(script);

    var validator = new IHS.validator();

    validator.addValidator('Billing_First_Name',
        [function(el) {

            return (el.value.replace(/\s/g, '').length > 0) || "Please enter your billing first name.";

        }, function(el) {
            return IHS.expressions.name.test(el.value) || "Please enter valid billing first name.";
        } ]);

    validator.addValidator('Billing_Last_Name',
        [function(el) {
            return (el.value.replace(/\s/g, '').length > 0) || "Please enter your billing last name.";

        }, function(el) {
            return IHS.expressions.name.test(el.value) || "Please enter valid billing last name.";
        } ]);

    validator.addValidator('Billing_Address_1',
        [function(el) {
            if (el.value.replace(/\s/g, '').length > 0) {
                return true;
            }
            else {
                return "Please enter your billing address.";
            }
        }, function(el) {
            return (IHS.expressions.address.test(el.value) && !/^\d+$/.test(el.value)) || "Please enter valid billing address.";
        } ]);

    validator.addValidator('Billing_Address_2',
        [function(el) {
            return (IHS.expressions.address.test(el.value) && !/^\d+$/.test(el.value)) || "Please enter valid billing address2.";
        } ]);

    validator.addValidator('Billing_City',
        [function(el) {

            return (el.value.replace(/\s/g, '').length > 0) || "Please enter your billing city.";

        }, function(el) {
            return IHS.expressions.name.test(el.value) || "Please enter valid billing city.";
        } ]);

    var szips = $('.zip.billing, .billing .zip, .postal.billing');
    if (szips.length > 0) {
        validator.addValidator(szips[0],
        [function(el) {
            return (el.value.replace(/\s/g, '').length > 0) || "Please enter your billing zip.";

        }, function(el) {
            if ($(szips[0]).attr("maxlength") == 5)
                return IHS.expressions.zip1.test(el.value) || "Please, enter valid billing zip.";
            else
                return szips[0].value.replace(/\s/g, '').length > 0 || "Please, enter valid billing zip.";
        } ]);
        if (szips.length > 1) {
            validator.addValidator(szips[1]
            , [function(el) {
                return el.style.display == "none" || el.value.replace(/\s/g, '').length == 0 || IHS.expressions.zip2.test(el.value) || "Please enter valid billing zip.";
            } ]);
        }
    }

    var sphones = $('.phone.billing');
    if (sphones.length > 0) {
        validator.addValidator(sphones[0],
        [function(el) {
            return (el.value.replace(/\s/g, '').length > 0) || "Please enter your billing phone.";

        }, function(el) {
            return (IHS.expressions.phone.test(el.value) && el.value.replace(/\s/g, '').length == $(el).attr("maxlength")) || "Please enter valid billing phone.";
        } ]);

        validator.addValidator(sphones[1],
        [function(el) {
            return sphones[0].value.replace(/\s/g, '').length == 0 || (IHS.expressions.phone.test(el.value) && el.value.replace(/\s/g, '').length == $(el).attr("maxlength")) || "Please enter valid billing phone.";
        } ]);

        validator.addValidator(sphones[2],
        [function(el) {
            return sphones[1].value.replace(/\s/g, '').length == 0 || (IHS.expressions.phone.test(el.value) && el.value.replace(/\s/g, '').length == $(el).attr("maxlength")) || "Please enter valid billing phone.";
        } ]);
    }

    validator.addValidator('Billing_Email',
        [function(el) {

            return (el.value.replace(/\s/g, '').length > 0) || "Please enter your billing email.";

        }, function(el) {
            return IHS.expressions.mail.test(el.value) || "Please enter valid billing email.";
        } ]);

    validator.addValidator('CC_Number',
        [function(el) {

            return (el.value.replace(/\s/g, '').length > 0) || "Please enter your credit card number.";

        }, function(el) {
            return IHS.IsCC(el.value) || "The credit card number you entered does not appear to be valid. Please check and re-submit.";
        } ]);

    validator.addValidator('CC_CVV',
        [function(el) {

            return (el.value.replace(/\s/g, '').length > 0) || "Please enter your CVV.";

        }, function(el) {
            return /^\d{3,}$/.test(el.value) || "The CVV does not appear to be valid. Please check and re-submit.";
        } ]);



    self.validator = validator;

    IHS.callback = function(r, s) {
        if (r.success) {
            location.href = r.url;
        }
        else {
            $('#Error_Message').html(r.message);
            self.scrollTo(0, 0);
            if (r.bid) {
                var exEl = $('#ExistingBillingID');
                if (exEl.length == 0) {
                    $('<input type="hidden" name="ExistingBillingID" id="ExistingBillingID" value="' + r.bid + '" />').appendTo($('#_form'));
                }
                else {
                    exEl.val(r.bid);
                }
            }
        }

        $("select").show();
        $('.progress_overlay').remove();
        $('.progress_load').fadeOut(500);
    }

    IHS.IsCC = function Mod10(ccNumb) {  // v2.0
        var valid = "0123456789"  // Valid digits in a credit card number
        var len = ccNumb.length;  // The length of the submitted cc number
        var iCCN = parseInt(ccNumb);  // integer of ccNumb
        var sCCN = ccNumb.toString();  // string of ccNumb
        sCCN = sCCN.replace(/^\s+|\s+$/g, '');  // strip spaces
        var iTotal = 0;  // integer total set at zero
        var bNum = true;  // by default assume it is a number
        var bResult = false;  // by default assume it is NOT a valid cc
        var temp;  // temp variable for parsing string
        var calc;  // used for calculation of each digit

        // Determine if the ccNumb is in fact all numbers
        for (var j = 0; j < len; j++) {
            temp = "" + sCCN.substring(j, j + 1);
            if (valid.indexOf(temp) == "-1") { bNum = false; }
        }
        if (!bNum) {
            bResult = false;
        }
        if ((len == 0) && (bResult)) {
            bResult = false;
        } else {
            if (len >= 15) {
                for (var i = len; i > 0; i--) {
                    calc = parseInt(iCCN) % 10;
                    calc = parseInt(calc);
                    iTotal += calc;
                    i--;
                    iCCN = iCCN / 10;
                    calc = parseInt(iCCN) % 10;
                    calc = calc * 2;
                    switch (calc) {
                        case 10: calc = 1; break;       //5*2=10 & 1+0 = 1
                        case 12: calc = 3; break;       //6*2=12 & 1+2 = 3
                        case 14: calc = 5; break;       //7*2=14 & 1+4 = 5
                        case 16: calc = 7; break;       //8*2=16 & 1+6 = 7
                        case 18: calc = 9; break;       //9*2=18 & 1+8 = 9
                        default: calc = calc;           //4*2= 8 &   8 = 8  -same for all lower numbers
                    }
                    iCCN = iCCN / 10;  // subtracts right most digit from ccNum
                    iTotal += calc;  // running total of the card number as we loop
                }
                if ((iTotal % 10) == 0) {
                    bResult = true;
                } else {
                    bResult = false;
                }
            }
        }
        return bResult; // Return the results
    }

};

