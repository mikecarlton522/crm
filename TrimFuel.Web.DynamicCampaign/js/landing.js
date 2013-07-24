var IHS = IHS || {};

IHS.onLoad = function() {

    self.validator = new IHS.validator();
    changeSBCountry("Shipping");
    var el = document.getElementById("Shipping_Country");
    if (!!el)
        changeCountry(el);
    /*
    $.validator.addClassRules(
    {
    phone:
    {
    phone: 1
    },
    zip:
    {
    zip: true
    },
    postal:
    {
    required: true,
    alphanumeric: true
    }
    });
        
   
   
    var validator = $('#_form').validate({
    onkeyup: false,
    onfocusout: false,               
    invalidHandler: function(f,v) {
    var res="";
    for (var i = 0; i<v.errorList.length; i++)
    {
    //res+=v.errorList[i].message+"<br>";
    res+=v.errorList[i].message+"\n";
    }
    //$.prompt(res, {callback: function() { v.focusInvalid() }});
    alert(res);
    v.focusInvalid();
    window.canSubmit = true;
    },
    ignore: ".noValidate input",
    showErrors: function() {}
    }
    );
   
    $.validator.addMethod("phone", function(value, elem,param) {
    var list = $(elem).parent().find("input");
    var re = /^\d+$/;        
    if (elem.className.indexOf("optional")>-1)
    param = 0;
    for (var i = 0; i<list.length; i++)
    {
    var el = list[i];
    if ((el.value.length == 0 && param == 0 && (i ==0 || list[i-1].value.length == 0)) || (re.test(el.value) && el.value.length == el.maxLength))
    {
    continue;
    }
    else
    {
    if (!validator.lastActive || validator.lastActive.name == el.name)
    validator.lastActive = el;
    return false;
    }
    }
    return true;
    }, "a");
   
    $.validator.addMethod("zip", function(value, elem) {
    var list = $(elem).parent().find("input");
    var re = /^\d+$/;
    for (var i = 0; i<list.length; i++)
    {
    var el = list[i];
    if ((el.value.length == 0 && i ==1) || (re.test(el.value) && el.value.length == el.maxLength))
    {
    continue;
    }
    else
    {
    if (!validator.lastActive || validator.lastActive.name == el.name)
    validator.lastActive = el;
    return false;
    }
    }
    return true;
    }, "a");  
   
    */

};