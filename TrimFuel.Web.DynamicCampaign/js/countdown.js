function countDown(el, timeOutMin, timeOutSec) {
	
	var timeOutMin = timeOutMin || 7;
	var timeOutSec = timeOutSec || 0;
	
	expires = window.setTimeout(counter, 1000);
	
	var counter = function() {
		timeOutSec--;
		if (timeOutSec == -01) {
		timeOutSec = 59;
		timeOutMin = timeOutMin - 1;
		}
		else {
			timeOutMin = timeOutMin;
		}
		
		if (timeOutSec <= 9) {
			timeOutSec = "0" + timeOutSec;
		}
		
		timeExpiration = (timeOutMin <= 9 ? + timeOutMin : timeOutMin) + ":" + timeOutSec + "";
		document.getElementById("time").innerHTML = timeExpiration;
		
		if (timeOutMin == '00' && timeOutSec == '00') {
		timeOutSec = "00";
		window.clearTimeout(expires);
		alert('Time is up! Please order your free bottle today.');
	}
		
		
};
	
	
	
	
}