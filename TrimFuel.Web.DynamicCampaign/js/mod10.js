internalLink = false;
	
	
	function Mod10(ccNumb) {  // v2.0
		var valid = "0123456789"  // Valid digits in a credit card number
		var len = ccNumb.length;  // The length of the submitted cc number
		var iCCN = parseInt(ccNumb);  // integer of ccNumb
		var sCCN = ccNumb.toString();  // string of ccNumb
		sCCN = sCCN.replace (/^\s+|\s+$/g,'');  // strip spaces
		var iTotal = 0;  // integer total set at zero
		var bNum = true;  // by default assume it is a number
		var bResult = false;  // by default assume it is NOT a valid cc
		var temp;  // temp variable for parsing string
		var calc;  // used for calculation of each digit
		
		// Determine if the ccNumb is in fact all numbers
		for (var j=0; j<len; j++) {
			temp = "" + sCCN.substring(j, j+1);
			if (valid.indexOf(temp) == "-1"){bNum = false;}
		}
		if(!bNum){
			bResult = false;
		}
		if((len == 0)&&(bResult)){  
			bResult = false;
		} else {  
			if(len >= 15){ 
				for(var i=len;i>0;i--){ 
					calc = parseInt(iCCN) % 10;  
					calc = parseInt(calc); 
					iTotal += calc;  
					i--;  
					iCCN = iCCN / 10; 
					calc = parseInt(iCCN) % 10 ; 
					calc = calc *2;              
					switch(calc){
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
				if ((iTotal%10)==0){  
					bResult = true;  
					} else {
						bResult = false; 
				}
			}
		}
		return bResult; // Return the results
	}








/*Exception*/ 