function ajaxLogin(username, password, success, error) {
    $.ajax({
        url: "service/login.asmx/Login",
        type: "POST",
        data: "{username:\"" + username + "\",password:\"" + password + "\"}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function(res) {
            if (res != null && res.d != null && res.d != "") {
                if (res.d.State == 1 && success != null) {
                    ajaxUpdateLoginToken();
                    success(res.d.ReturnValue);
                }
                else if (error != null) {
                    ajaxLoginDefaultError(error);
                }
            }
            else if (error != null) {
                ajaxLoginDefaultError(error);
            }
        },
        error: function(xmlHttpRequest, textStatus, errorThrown) {
            if (error != null) {
                ajaxLoginDefaultError(error);
            }
        }
    });
}

function ajaxUpdateLoginToken() {
    $.ajax({
        url: "service/login.asmx/GetLoginToken",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function(res) {
            if (res != null && res.d != null && res.d != "") {
                $("#login-token").replaceWith(res.d.replace("../",""));
            }
            else {
                alert("error");
                //TODO: error
            }
        },
        error: function(xmlHttpRequest, textStatus, errorThrown) {
            alert(xmlHttpRequest.responseText);
            //TODO: error
        }
    });
}

function ajaxLoginDefaultError(errorFnc) {
    if (errorFnc != null) {
        errorFnc("Invalid login or password.");
    }
}