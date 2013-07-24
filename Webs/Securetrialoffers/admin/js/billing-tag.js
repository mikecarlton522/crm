//TODO
var ajaxTagServiceUrl = "http://localhost/securetrialoffers/admin/AjaxService/AjaxTagService.asmx/";
//var ajaxTagServiceUrl = "https://www.securetrialoffers.com/admin/newApp/admin/AjaxService/AjaxTagService.asmx/";

function trim(string) {
    return string.replace(/(^\s+)|(\s+$)/g, "");
}

function saveBillingTags(billingID, callback) {
    var params = {};
    params.billingID = billingID;
    params.tagIDList = "";

    $("#tag-list li input[name=tag-selected]:checked").each(function() {
        params.tagIDList += $(this).parent().attr("tag-id") + ",";
    });

    var paramsStr = "{billingID:'" + params.billingID + "',tagIDList:'" + params.tagIDList + "'}";

    $.ajax({
        url: ajaxTagServiceUrl + "SaveBillingTags",
        contentType: "application/json; charset=utf-8",
        type: "POST",
        dataType: "json",
        data: paramsStr,
        success: function(msg) {
            callback();
        },
        error: function(xhr, textStatus, errorThrown) {
            //callbackError("Can't connect to the server.");
            alert(xhr.responseText);
        }
    });
    
}

function loadBillingTags(destination, billingID) {
    var params = {};
    params.billingID = billingID;

    var paramsStr = "{billingID:'" + params.billingID + "'}";

    $.ajax({
        url: ajaxTagServiceUrl + "LoadBillingTags",
        contentType: "application/json; charset=utf-8",
        type: "POST",
        dataType: "json",
        data: paramsStr,
        success: function(msg) {
            if (msg.d != "") {
                $(destination).html(msg.d);
                obtainTagList();
            }
        },
        error: function(xhr, textStatus, errorThrown) {
            //callbackError("Can't connect to the server.");
            alert(xhr.responseText);
        }
    });
}

function loadBillingTagLinks(destination, billingID) {
    var params = {};
    params.billingID = billingID;
    
    var paramsStr = "{billingID:'" + params.billingID + "'}";

    $.ajax({
        url: ajaxTagServiceUrl + "LoadBillingTagLinks",
        contentType: "application/json; charset=utf-8",
        type: "POST",
        dataType: "json",
        data: paramsStr,
        success: function(msg) {
            if (msg.d != "") {
                $(destination).html(msg.d);
                obtainTagList();
            }
        },
        error: function(xhr, textStatus, errorThrown) {
            //callbackError("Can't connect to the server.");
            alert(xhr.responseText);
        }
    });
}

function editTag(el) {
    $(el).find("div[name=tools]").hide();
    $(el).find("div[name=edit-tools]").show();
    showEditBox($(el).get());
}

function cancelTag(el) {
    if ($(el).attr("tag-id") == "-1") {
        $(el).remove();
    }
    else {
        $(el).find("div[name=edit-tools]").hide();
        $(el).find("div[name=tools]").show();
        hideEditBox($(el).get());
    }
}

function saveTag(el) {
    var params = {};
    params.tagID = $(el).attr("tag-id");
    params.tagValue = trim($(el).find("span[name=edit-box] input").val());

    if (params.tagValue == "") {
        return false;
    }

    var paramsStr = "{tagID:'" + params.tagID + "',tagValue:'" + params.tagValue + "'}";
    $.ajax({
        url: ajaxTagServiceUrl + "SaveTag",
        contentType: "application/json; charset=utf-8",
        type: "POST",
        dataType: "json",
        data: paramsStr,
        success: function(msg) {
            if (msg.d != -1) {
                $(el).find("span[name=value]").text(params.tagValue);
                $(el).attr("tag-id", msg.d);
                cancelTag(el);
            }
        },
        error: function(xhr, textStatus, errorThrown) {
            //callbackError("Can't connect to the server.");
            alert(xhr.responseText);
        }
    });

    return true;
}

function removeTag(el) {
    var params = {};
    params.tagID = $(el).attr("tag-id");
    params.tagValue = trim($(el).find("span[name=value]").text());

    if (!confirm("Are you sure you want to remove tag '" + params.tagValue + "'?")) {
        return false;
    }

    var paramsStr = "{tagID:'" + params.tagID + "'}";
    $.ajax({
        url: ajaxTagServiceUrl + "RemoveTag",
        contentType: "application/json; charset=utf-8",
        type: "POST",
        dataType: "json",
        data: paramsStr,
        success: function(msg) {
            if (msg.d) {
                $(el).attr("tag-id", "-1");
                cancelTag(el);
            }
        },
        error: function(xhr, textStatus, errorThrown) {
            //callbackError("Can't connect to the server.");
            alert(xhr.responseText);
        }
    });

    return true;
}

function addNewTag() {
    editTag(obtainElement($("#new-item").clone().attr("id", "").attr("tag-id", "-1").show().appendTo($("#tag-list")).get()));
}

function saveAllTag() {
    $("#tag-list span[name=edit-box]").each(function() { saveTag($(this).parent().get()); });
}

function cancelAllTag() {
    $("#tag-list span[name=edit-box]").each(function() { cancelTag($(this).parent().get()); });
}

function showEditBox(el) {
    $("#edit-box input").val(trim($(el).find("span[name=value]").text()));
    $(el).find("span[name=value]").hide();
    $("#edit-box").clone().attr("id", "").show().insertAfter($(el).find("span[name=value]")).find("input").focus();
}

function hideEditBox(el) {
    $(el).find("span[name=value]").show();
    $(el).find("span[name=edit-box]").remove();
}

function obtainElement(el) {
    $(el).find("a[name=edit]").click(function() { editTag($(this).parent().parent().get()); return false; });
    $(el).find("a[name=cancel]").click(function() { cancelTag($(this).parent().parent().get()); return false; });
    $(el).find("a[name=save]").click(function() { saveTag($(this).parent().parent().get()); return false; });
    $(el).find("a[name=remove]").click(function() { removeTag($(this).parent().parent().get()); return false; });
    return $(el);
}

function obtainTagList() {
    $("#tag-list li").each(function() { obtainElement(this); });
    $("#add-new-tag").click(function() { addNewTag(); return false; });
    $("#save-all-tag").click(function() { saveAllTag(); return false; });
    $("#cancel-all-tag").click(function() { cancelAllTag(); return false; });
}
