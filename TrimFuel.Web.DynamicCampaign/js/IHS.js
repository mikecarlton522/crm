var IHS = IHS || {};
var internalLink = false;
IHS.isAsync = false;
self.canSubmit = true;

$(document).ready(function () {
    
});

IHS.submit = function (action) {
    if (!self.canSubmit) {
        return;
    }
    self.canSubmit = false;
    $('#__action').val(action);
    internalLink = true;    

    if (!self.validator || self.validator.isValid()) {
        if (IHS.submitHandler) {
            IHS.submitHandler($('#_form')[0]);
        }
        else {
            $('#_form').submit();
        }
    }
    else {
        self.canSubmit = true;
    }
};

IHS.submitCoupon = function () {    
    $('#__action').val('Coupon');
    internalLink = true;
    $('#_form').submit();      
};

function changeCountry(el) {
    var className = el.className.replace(/country|\s/g, '').replace('USOnly', '').replace('UKOnly', '').replace('CanadaOnly', '').replace('AustraliaOnly', '').replace('ArgentinaOnly', '').replace('BrasilOnly', '');
    if (el.value == "US") {
        $(".USOnly." + className).hide();
        $(".postal." + className).removeClass("postal").addClass("zip").attr("maxlength", "5");
        $(".phone." + className).removeClass("optional");
    }
    else {
        $(".USOnly." + className).hide();
        $(".zip." + className).removeClass("zip").addClass("postal").attr("maxlength", "20");
        $(".phone." + className).addClass("optional");

    }
    changeSBCountry("Shipping");
    changeSBCountry("Billing");
}

function changeSBCountry(prefix) {
    var countryElem = document.getElementById(prefix + "_Country");
    if (!!!countryElem) {
        setActiveStates(prefix, "US");
        return;
    }
    if ($(countryElem).hasClass("USOnly")) {
        setActiveStates(prefix, "US");
        countryElem.value = "US";
        return;
    }
    if ($(countryElem).hasClass("UKOnly")) {
        setActiveStates(prefix, "United Kingdom");
        countryElem.value = "United Kingdom";
        return;
    }
    if ($(countryElem).hasClass("CanadaOnly")) {
        setActiveStates(prefix, "Canada");
        countryElem.value = "Canada";
        return;
    }
    if ($(countryElem).hasClass("AustraliaOnly")) {
        setActiveStates(prefix, "Australia");
        countryElem.value = "Australia";
        return;
    }
    if ($(countryElem).hasClass("ArgentinaOnly")) {
        setActiveStates(prefix, "Argentina");
        countryElem.value = "Argentina";
        return;
    }
    if ($(countryElem).hasClass("BrasilOnly")) {
        setActiveStates(prefix, "Brasil");
        countryElem.value = "Brasil";
        return;
    }

    if (countryElem.value == "United Kingdom" ||
        countryElem.value == "US" ||
        countryElem.value == "Canada" ||
        countryElem.value == "Australia" ||
        countryElem.value == "Argentina" ||
        countryElem.value == "Brasil"
        ) {
        setActiveStates(prefix, countryElem.value);
    }
    else
        setActiveStates(prefix, "Other");

}
function setActiveStates(prefix, country) {
    if (country == "United Kingdom")
        country = "UK";

    $("#" + prefix + "_State_Other").hide();
    $("#" + prefix + "_State_UK").hide();
    $("#" + prefix + "_State_US").hide();
    $("#" + prefix + "_State_Canada").hide();
    $("#" + prefix + "_State_Australia").hide();
    $("#" + prefix + "_State_Argentina").hide();
    $("#" + prefix + "_State_Brasil").hide();
    $("#" + prefix + "_State_" + country).show();
}