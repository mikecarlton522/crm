var IHS = IHS || {};

IHS.expressions =
    {
        name: /^[a-z-.,()'\"\s]+$/i,
        address: /^[a-z0-9-.,()'\"\s]*$/i,
        zip1: /^\d{5}$/,
        zip2: /^\d{4}$/,
        phone: /^\d+$/,
        mail: /^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$/i
    };
    IHS.validator = function () {
        var validators = new Array();

        var lastElement;
        this.addValidator = function (el, rules) {
            if (el != null) {
                validators.push(function () {
                    if (el != null) {
                        if (!el.className) {
                            el = document.getElementById(el);
                        }
                        if (el != null) {
                            if (el.className.indexOf("noValidate") > -1)
                                return true;
                            var res = false;
                            for (var i in rules) {
                                res = rules[i](el);
                                if (res === true) {
                                    continue;
                                }
                                else {
                                    break;
                                }
                            }
                            if (res !== true) {
                                lastElement = el;

                            }
                        } else {
                            return true;
                        }
                    } else {
                        return true;
                    }
                    return res;
                });
            }
        }

        this.addValidator('Shipping_First_Name',
        [function (el) {

            return (el.value.replace(/\s/g, '').length > 0) || "Please enter shipping first name.";

        }, function (el) {
            return IHS.expressions.name.test(el.value) || "Please, enter valid shipping first name.";
        } ]);

        this.addValidator('Shipping_Last_Name',
        [function (el) {
            return (el.value.replace(/\s/g, '').length > 0) || "Please enter shipping last name.";

        }, function (el) {
            return IHS.expressions.name.test(el.value) || "Please, enter valid shipping last name.";
        } ]);

        this.addValidator('Shipping_Address_1',
        [function (el) {
            if (el.value.replace(/\s/g, '').length > 0) {
                return true;
            }
            else {
                return "Please enter shipping address.";
            }
        }, function (el) {
            return (IHS.expressions.address.test(el.value) && !/^\d+$/.test(el.value)) || "Please, enter valid shipping address.";
        } ]);

        this.addValidator('Shipping_Address_2',
        [function (el) {
            return (IHS.expressions.address.test(el.value) && !/^\d+$/.test(el.value)) || "Please, enter valid shipping address2.";
        } ]);

        this.addValidator('Shipping_State_Other',
        [function (el) {
            if (el.style.display == "none")
                return true;
            return (el.value.replace(/\s/g, '').length > 0) || "Please enter shipping state.";

        }, function (el) {
            if (el.style.display == "none")
                return true;
            return IHS.expressions.name.test(el.value) || "Please, enter valid shipping state.";
        } ]);

        this.addValidator('Billing_State_Other',
        [function (el) {
            if (el.style.display == "none")
                return true;
            return (el.value.replace(/\s/g, '').length > 0) || "Please enter billing state.";

        }, function (el) {
            if (el.style.display == "none")
                return true;
            return IHS.expressions.name.test(el.value) || "Please, enter valid billing state.";
        } ]);

        this.addValidator('Agree_With_Terms',
        [function (el) {
            if (!!el) {
                if (el.checked)
                    return true;
                else
                    return "Please, agree with terms.";
            }
            else
                return true;
        } ]);

        this.addValidator('Shipping_City',
        [function (el) {

            return (el.value.replace(/\s/g, '').length > 0) || "Please enter shipping city.";

        }, function (el) {
            return IHS.expressions.name.test(el.value) || "Please, enter valid shipping city.";
        } ]);

        var szips = $('.zip.shipping, .shipping .zip, .postal.shipping');
        if (szips.length > 0) {
            this.addValidator(szips[0],
        [function (el) {
            return (el.value.replace(/\s/g, '').length > 0) || "Please enter shipping zip.";

        }, function (el) {
            if ($(el).attr("maxlength") == 5) {
                return IHS.expressions.zip1.test(el.value) || "Please, enter valid shipping zip.";
            }
            else
                return szips[0].value.replace(/\s/g, '').length >= 5 || "Please, enter valid shipping zip.";
        } ]);
            if (szips.length > 1) {
                this.addValidator(szips[1]
            , [function (el) {
                return el.style.display == "none" || el.value.replace(/\s/g, '').length == 0 || IHS.expressions.zip2.test(el.value) || "Please, enter valid shipping zip.";
            } ]);
            }
        }

        var sphones = $('.phone.shipping');
        if (sphones.length > 0) {
            this.addValidator(sphones[0],
        [function (el) {
            return (el.value.replace(/\s/g, '').length > 0) || "Please enter shipping phone.";

        }, function (el) {
            return (IHS.expressions.phone.test(el.value) && el.value.replace(/\s/g, '').length == $(el).attr("maxlength")) || "Please, enter valid shipping phone.";
        } ]);

            this.addValidator(sphones[1],
        [function (el) {
            return sphones[0].value.replace(/\s/g, '').length == 0 || (IHS.expressions.phone.test(el.value) && el.value.replace(/\s/g, '').length == $(el).attr("maxlength")) || "Please, enter valid shipping phone.";
        } ]);

            this.addValidator(sphones[2],
        [function (el) {
            return sphones[1].value.replace(/\s/g, '').length == 0 || (IHS.expressions.phone.test(el.value) && el.value.replace(/\s/g, '').length == $(el).attr("maxlength")) || "Please, enter valid shipping phone.";
        } ]);
        }

        this.addValidator('Shipping_Email',
        [function (el) {

            return (el.value.replace(/\s/g, '').length > 0) || "Please enter shipping email.";

        }, function (el) {
            return IHS.expressions.mail.test(el.value) || "Please, enter valid shipping email.";
        } ]);

        this.isValid = function () {
            for (var i in validators) {
                var res = validators[i]();
                if (res === true)
                    continue;
                else {
                    alert(res);
                    lastElement.focus();
                    return false;
                }
            }
            return true;
        };
    };