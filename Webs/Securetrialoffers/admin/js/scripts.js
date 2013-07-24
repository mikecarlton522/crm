function editForm(url, w, refreshUrl)
{
    $("<div id='editFormDiag' title='Loading...' style='display:none'></div>").appendTo("body");
    $("#editFormDiag").dialog({
        modal: true,
        width: w,
        resizable: false,
		draggable: false,
        position: "center",
		minHeight: 300,
        close: function() {
			$.validationEngine.closePrompt(".formError",true);
            $(this).remove();
        },
        open: function(){
            $("<form id='editForm' name='editForm'></form>").load(url, function(){
				$("#editFormDiag").parent().attr("id", "editFormDiagContainer");
                var title = $(this).find("h2").html();        
                $(this).find("h2").remove();
                $("#editFormDiag").parent().find(".ui-dialog-title").html(title);
                $("#editFormDiag").append(this);
				$("#editForm").validationEngine({inline:true,scroll:false})
            });
        },
        buttons: {
            "Cancel": function(){
                $(this).dialog("close");
            },
            "Save": function(){
				if ($("#editForm").validationEngine({returnIsValid:true,inline:true,scroll:false}))
				{
					var params = $(this).find("#editForm").serialize();
					$.ajax({
						type: 'POST',
						url: url,
						data: params,
						success: function(data, textStatus, XMLHttpRequest) {
							$("#editFormDiag").dialog("close");
							window.location = refreshUrl;
						},
						error: function(XMLHttpRequest, textStatus, errorThrown) {
							alert("Error occured while saving object.")
						}
					});
				}
            }
        }
    });
}

function processTableOffsets(el)
{
	$(el).find("tr").removeClass("offset");
	$(el).find("tr:not(.header):not(.subheader):odd").addClass("offset");
}

$(document).ready(function(){
	$(".process-offets").each(function() { processTableOffsets(this); });
})