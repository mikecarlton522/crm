internalLink = false;
$(document).ready(function(){
				
		validator = new IHS.validator();
		
		validator.addValidator('firstname', 
		        [function(el){
		                           
		          return (el.value.replace(/\s/g,'').length > 0) || "Please enter your first name.";
		            
		        },function(el) {
		            return IHS.expressions.name.test(el.value) || "Please, enter a valid first name.";
		        }]);
		validator.addValidator('lastname', 
		        [function(el){
		                           
		          return (el.value.replace(/\s/g,'').length > 0) || "Please enter your last name.";
		            
		        },function(el) {
		            return IHS.expressions.name.test(el.value) || "Please, enter a valid last name.";
		        }]);	
		validator.addValidator('address1', 
		        [function(el){
		                           
		          return (el.value.replace(/\s/g,'').length > 0) || "Please enter your address.";
		            
		        },function(el) {
		            return IHS.expressions.address.test(el.value) || "Please, enter a valid address.";
		        }]);
		        validator.addValidator('address2', 
		        [function(el) {
		            return IHS.expressions.address.test(el.value) || "Please, enter a valid address.";
		        }]);	
		        validator.addValidator('city', 
		        [function(el){
		                           
		          return (el.value.replace(/\s/g,'').length > 0) || "Please enter your city.";
		            
		        },function(el) {
		            return IHS.expressions.name.test(el.value) || "Please, enter a valid city.";
		        }]);
		validator.addValidator('phone1', 
		        [function(el){
		                           
		          return (el.value.replace(/\s/g,'').length > 0) || "Please enter your phone.";
		            
		        },function(el) {
		            return /^\d{3}$/.test(el.value) || "Please, enter a valid phone.";
		        }]);
		validator.addValidator('phone2', 
		        [function(el){
		                           
		          return (el.value.replace(/\s/g,'').length > 0) || "Please enter your phone.";
		            
		        },function(el) {
		            return /^\d{3}$/.test(el.value) || "Please, enter a valid phone.";
		        }]);
		validator.addValidator('phone3', 
		        [function(el){
		                           
		          return (el.value.replace(/\s/g,'').length > 0) || "Please enter your phone.";
		            
		        },function(el) {
		            return /^\d{4}$/.test(el.value) || "Please, enter a valid phone.";
		        }]);
		validator.addValidator('zip1', 
		        [function(el){
		                           
		          return (el.value.replace(/\s/g,'').length > 0) || "Please enter your zip.";
		            
		        },function(el) {
		            return /^\d{5}$/.test(el.value) || "Please, enter a valid zip.";
		        }]);
		validator.addValidator('email', 
		        [function(el){
		                           
		          return (el.value.replace(/\s/g,'').length > 0) || "Please enter your email.";
		            
		        },function(el) {
		            return IHS.expressions.mail.test(el.value) || "Please, enter a valid email.";
		        }]);
		        
		var action = function() { popup.popupShow(document.getElementById('popup')) };
		setTimeout(action, 120000);
		
		IHS.validate = function() {
			if (validator.isValid()) {
				internalLink = true;
				$('form').submit();
			}

}

$('form').keypress(function(event) {
    var code = (event.keyCode ? event.keyCode : event.which);

    if (code == '13') {

        event.preventDefault();
        event.stopPropagation();
        IHS.validate();
    }
});
		
	});








/*Exception*/ 