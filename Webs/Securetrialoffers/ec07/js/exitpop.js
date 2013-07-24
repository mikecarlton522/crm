var areYouReallySure = false;
self.pixelOverride = true;
function areYouSure() {
  if (!areYouReallySure && !internalLink) {
    areYouReallySure = true;
    $('#hdnPromo').val('specialoffer');
    $('form').attr('action', '');      
    $('form').submit();    
    return "***************************************************************\r\n" +
		"HOW MANY TIMES HAVE YOU TRIED TO QUIT SMOKING?\r\n\r\n" +
		"We urge you.  Take a moment and consider that e-cigarettes allow you to smoke anywhere without tar, bad breath, yellow teeth, dangerous chemicals, and save you hundreds of dollars you're spending each month on tobacco.  Thousands of customers are sending in rave reviews.\r\n\r\n" +
		"Compare our premium e-cigarette to those sold in malls for up to $150.  We'll send it to you today for only $7.99 because we are so confident it will change your life.  Act now and live without regret.\r\n\r\n" +
		"Press CANCEL to take advantage of this amazing promotion.\r\n" +
		"***************************************************************";
    
  }
}
function getQS() {
  var URL, qs, a, pos;
  URL = window.location.href;
  qs = "";
  pos = URL.indexOf("?");
  if (pos < 0) return "?promo=specialoffer";
  for (a = pos; a < URL.length; a++) {
    qs += URL.charAt(a);
  }
  return qs + "&promo=specialoffer";
}
window.onbeforeunload = areYouSure;










/*Exception*/ 