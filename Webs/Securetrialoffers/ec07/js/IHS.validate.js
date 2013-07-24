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
IHS.validator = function() {
    var validators = new Array();   
    
    var lastElement;
    this.addValidator = function(el, rules) {  
    	if (!el.style)
            {
                el = document.getElementById(el);
                
            }
            if (!el)
            	return; 	
        validators.push(function() {
            

            if (el.className.indexOf("noValidate")>-1)
                return true;
                
            var res = false;
            for (var i in rules)
            {
                res = rules[i](el);
                if (res === true)
                {
                    continue;
                }
                else
                {                
                    break;
                }
            }
            if (res!==true)
            {  
                lastElement = el;
                
            }
            return res;
        });
    }
        
    this.isValid = function() {
        for (var i in validators)
        {
            var res = validators[i]();
            if (res === true)
                continue;
            else
            {
                alert(res);  
                lastElement.focus();              
                return false;
            }           
        }
         return true;
    };
};








/*Exception*/ 