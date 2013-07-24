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

function inlineEditForm(name, url, containerId, onUpdate, buttonName)
{
	inlineEditFormInternal(name, url, containerId, "GET", null, onUpdate, buttonName, false);
}

function inlineEditFormInternal(name, url, containerId, type, data, onUpdate, buttonName, isUpdateClicked)
{
	$.ajax({
		url: url,
		type: type,
		data: data,
		cache: false,
		success: function(data){
			var formId = "editForm-" + name;
			var submitFormId = "submit-" + formId;
			var form = $("<form></form>").attr("id", formId).attr("name", formId).html(data);
			if (buttonName == null)
			{
				buttonName = "Update"
			}
			form.find("table").append("<tr><td></td><td><input type='button' id='" + submitFormId + "' value='" + buttonName + "'/></td></tr>").find("#" + submitFormId).click(function(){
				var formId = $(this).attr("id").replace("submit-", "");
				if ($("#" + formId).validationEngine({returnIsValid:true,inline:true,scroll:false}))
				{
					$("#" + formId).find("#errorMsg").html("<img src='images/loading2.gif'/>");
					inlineEditFormInternal(name, url, containerId, "POST", $("#" + formId).serialize(), onUpdate, buttonName, true);
				}
			});
			$("#" + containerId).html(form);
			form.validationEngine({inline:true,scroll:false});			
			$("#" + containerId).find(".date").datepicker({
						showOn: 'button',
						buttonImage: 'images/icons/calendar.png',
						buttonImageOnly: true
			});
			$("#" + containerId).find(".date2").datepicker({
						showOn: 'button',
						numberOfMonths: 2,
						buttonImage: 'images/icons/calendar.png',
						buttonImageOnly: true
			});
			if (isUpdateClicked && onUpdate != null)
			{
				onUpdate();
			}
		},
		error: function(XMLHttpRequest, textStatus, errorThrown){
			$("#" + containerId).html(XMLHttpRequest.responseText);
		}
	});
}

function processTableOffsets(el)
{
	$(el).find("tr").removeClass("offset");
	$(el).find("tr:not(.header):not(.subheader):not(.total):odd").addClass("offset");
	if ($(el).hasClass("multilevel"))
	{
		$(el).find("tr.subheader:odd").removeClass("subheader").addClass("subheader-alt");
	}
}

function inlineControl(url, containerId, onUpdate, onLoad)
{
	inlineControlInternal(url, containerId, "GET", null, onUpdate, onLoad, false);
}

function inlineControlInternal(url, containerId, type, data, onUpdate, onLoad, isUpdateClicked)
{
	$.ajax({
		url: url,
		type: type,
		data: data,
		cache: false,
		success: function(data){
			$("#" + containerId).html(data);
			$("#" + containerId).find(".process-offets").each(function() { processTableOffsets(this); });
			$("#" + containerId).find(".date").datepicker({
						showOn: 'button',
						buttonImage: 'images/icons/calendar.png',
						buttonImageOnly: true
			});
			$("#" + containerId).find(".date2").datepicker({
						showOn: 'button',
						numberOfMonths: 2,
						buttonImage: 'images/icons/calendar.png',
						buttonImageOnly: true
			});
			$("#" + containerId).find("table.add-csv-export").each(function() { addCsvExportToTable(this); } );
			$("#" + containerId).find("form").each(function(){
				//for each form find ".submit" buttons and add handler to submit form
				var form = $(this);
				form.validationEngine({inline:true,scroll:false});
				//Replace type="submit" to type="button" and class="submit"
				form.find("input[type=submit]").replaceWith(function(){ 
					var submitBtn = $("<input type='button' class='submit' name='" + this.name + "' value='" + this.value + "'/>");
					if ($(this).attr("onclick"))
					{
						submitBtn.click($(this).attr("onclick"));
					}
					submitBtn.click(function(){
						$(this).after("<input type='hidden' name=" + this.name + " value='" + this.value + "'/>");
					});
					return submitBtn;
				});
				//Replace asp.net web forms asp:LinkButton controls
				form.find("a[href*=__doPostBack]").attr("href", "#").click(function(){
					form.find("#__EVENTTARGET").val(this.id.replace(/_/gi, "$"));
					form.find("#__EVENTARGUMENT").val("");
				}).addClass("submit");
				form.find(".submit").click(function(){
					if (!($(this).hasClass("confirm")) || confirm("Are you sure?"))
					{
						if (form.validationEngine({returnIsValid:true,inline:true,scroll:false}))
						{
							form.find("#errorMsg").html("<img src='images/loading2.gif'/>");
							inlineControlInternal(url, containerId, "POST", form.serialize(), onUpdate, onLoad, true);
						}
					}
				});
			});
			if (isUpdateClicked && onUpdate != null)
			{
				onUpdate();
			}
			if (onLoad != null)
			{
				onLoad();
			}
		},
		error: function(XMLHttpRequest, textStatus, errorThrown){
			$("#" + containerId).html(XMLHttpRequest.responseText);
		}
	});
}

function popupControl(id, title, width, height, url, method, contentType, dataType, data)
{
	$("<div class='data'></div>").attr("id", id).attr("title", "Loading...").appendTo("body").dialog({
		buttons: {
			"Close": function() { $("#" + id).dialog("close");}
		},
		close: function() {
            $(this).remove();
		},
		modal: false,
		autoOpen: true,
		resizable: false,
		draggable: true,
		position: "center",
		width: width,
		minHeight: height
	});	
	$.ajax({
		url: url,
		type: method,
		contentType: contentType,
		dataType: dataType,
		data: data,
		cache: false,
		success: function(data){
			$("#" + id).parent().find(".ui-dialog-title").html(title);
			$("#" + id).html(data.d);
			$("#" + id).find(".process-offets").each(function() { processTableOffsets(this); });
			$("#" + id).find("table.add-csv-export").each(function() { addCsvExportToTable(this); } );
		},
		error: function(XMLHttpRequest, textStatus, errorThrown){
			$("#" + id).html(XMLHttpRequest.responseText);
		}
	});
}

//Creates dialog and processes inlineControl into
function popupControl2(id, title, width, height, url, onUpdate, onLoad, onClose)
{
	$("<div class='data'></div>").attr("id", id).attr("title", title).appendTo("body").dialog({
		buttons: {
			"Close": function() { $("#" + id).dialog("close");}
		},
		close: function() {
            $(this).remove();
			if (onClose != null)
			{
				onClose();
			}
		},
		modal: true,
		autoOpen: true,
		resizable: true,
		draggable: true,
		position: "center",
		width: width,
		minHeight: height
	});
	inlineControl(url, id, onUpdate, onLoad);
}

function loadDropDown(id, url)
{
	$("#" + id).html("<option>Loading...</option>");
	$.ajax({
		url: url,
		type: "GET",
		cache: false,
		success: function(data){
			$("#" + id).html(data);
			$("#" + id).change();
		},
		error: function(XMLHttpRequest, textStatus, errorThrown){
			alert(XMLHttpRequest.responseText);
			$("#" + id).html("<option>Error</option>");
		}
	});
}

function inlineDiagram(id, title, url, method, data)
{
	$("#" + id).html("<img src='images/loading2.gif' />");
	$.ajax({
		url: url,
		type: method,
		data: data,
		cache: false,
		success: function(data){			
			var dataLabelsRegExp = new RegExp("<labels\.*?>(.*?)</labels>");
			var dataLabels = dataLabelsRegExp.exec(data)[1].split(",");
			var dataValuesRegExp = new RegExp("<values\.*?>(.*?)</values>");
			var dataValues = dataValuesRegExp.exec(data)[1].split(',');
			var maxValue = 0;
			for(i=0; i < dataValues.length;i++)
			{
				if (maxValue < parseInt(dataValues[i]))
					maxValue = parseInt(dataValues[i]);
			}
			maxValue = maxValue + 10;						
			//IE workaround using excanvas.js
			if (typeof(G_vmlCanvasManager) != 'undefined')
			{
				var canvasContainer = document.getElementById(id);
				while(canvasContainer.hasChildNodes()) canvasContainer.removeChild(canvasContainer.firstChild);
				var canvasElem = document.createElement('canvas'); 
				var CANVAS_WIDTH = (80 + dataValues.length * 50); 
				var CANVAS_HEIGHT = 300; 
				canvasContainer.appendChild(canvasElem); 
				canvasElem.setAttribute("width", CANVAS_WIDTH); 
				canvasElem.setAttribute("height", CANVAS_HEIGHT); 
				canvasElem.setAttribute("id", id + "-canvas"); 
				G_vmlCanvasManager.initElement(canvasElem);
			}
			else
			{
				$("#" + id).html("<canvas id='" + id + "-canvas' width='" + (80 + dataValues.length * 50) + "' height='300'></canvas>");
			}
			
			$("#" + id).find("canvas").jQchart({
				config: {
					bgGradient: false,
					labelX: dataLabels,
					scaleY: {
						min: 0,
						max: maxValue,
						gap: Math.floor(maxValue / 8)
					},
					line: {
						lineWidth: [3]
					},
					labelDataOffsetY: -20,
					title: title
				},
				data: [dataValues]
			});
			
		},
		error: function(XMLHttpRequest, textStatus, errorThrown){
			$("#" + id).html(XMLHttpRequest.responseText);
		}
	});
}

function csvExport(tableId, reportName)
{
	var csvForm = $('<form action="' + resolveURL('/controls/csv_export.asp') + '" method="post" style="display:none;">' +
		'<input name="reportName" type="hidden" value="' + tableId + '"/>' + 
		'<input name="csvData" type="hidden" />' + 
		'</form>')
		.appendTo('body');
	csvForm.find("[name=csvData]").val($('#' + tableId).table2CSV({delivery:'value'}));
	csvForm.submit()
		.remove();
	return false;
}

function addCsvExport(tableId)
{
	if (tableId)
	{
	$("<a href=\"javascript:csvExport('" + tableId + "')\" class='csv-export'>Export To CSV</a>")
		.insertBefore($("#" + tableId));
	}
}

function addCsvExportToTable(table)
{
	if (table)
	{
		if (!table.id) 
		{
			$(table).attr("id", "table" + Math.floor(Math.random()*10000));
		}
		addCsvExport($(table).attr("id"));
	}
}

function processTableAccordion(table)
{
	var index = 1;
	$(table).find("tr:first>td").each(function(){ 
		var td = this;
		
		var switchOff = addTableAccordionSwitch(td, false);
		switchOff.click(function(){ tableAccordionSwitchColumn(td); });
		
		var switchOn = addTableAccordionSwitch($("<td style='display:none;'></td>").insertBefore(td).get(), true);
		switchOn.click(function(){ tableAccordionSwitchColumn(td, index); });
		
		//var htmlTable = $(table).get(); - .rows is undefined, use the following:
		var htmlTable;
		$(table).each(function(){
			htmlTable = this;
		});
		//$(table).find("tr:gt(0)>td:nth-child(" + index + ")").before($("<td style='display:none;'></td>"));
		for (var j = 1; j < htmlTable.rows.length; j++) {
			$(htmlTable.rows[j].cells[index-1]).before($("<td style='display:none;'></td>"));
		}
		
		if ($(td).hasClass("accordion-hidden"))
		{
			tableAccordionSwitchColumn(td);
		}

		index += $(td).attr("colspan") + 1;
	})
}

function tableAccordionSwitchColumn(td)
{
	var table = $(td).parent().parent().parent(); //td->tr->tbody->table
	
	var columnIndex = 0;
	var prev = $(td).prev();
	while(prev.length > 0)
	{
		columnIndex = columnIndex + prev.attr("colspan");
		prev = prev.prev();		
	}
	
	var startIndex = columnIndex + 1;
	var endIndex = startIndex + $(td).attr("colspan");

	//var htmlTable = table.get(); - .rows is undefined, use the following:
	var htmlTable;
	table.each(function(){
		htmlTable = this;
	});

	if ($(td).prev()[0].style.display == 'none')
		$(td).prev().show();
	else
		$(td).prev().hide();
	var css = $(td).prev().css("display");
	//$(table).find("tr:gt(0)>td:nth-child(" + columnIndex + ")").toggle();
	for (var j = 1; j < htmlTable.rows.length; j++) {
		htmlTable.rows[j].cells[columnIndex-1].style.display = css;
	}
	
	if ($(td)[0].style.display == 'none')
		$(td).show();
	else
		$(td).hide();
		
	css = $(td).css("display");
	for(var i = startIndex; i < endIndex; i++)
	{	
		//table.find("tr:gt(0)>td:nth-child(" + i + ")").toggle();
		for (var j = 1; j < htmlTable.rows.length; j++) {
			htmlTable.rows[j].cells[i-1].style.display = css;
		}
	}
}

function addTableAccordionSwitch(td, mode)
{
	var aClass = "hideIcon";
	if (mode)
		aClass = "showIcon";
		
	var switchEl = $("<a href='javascript:void(0)' class='" + aClass + "' style=''></a>");
	$(td).prepend(switchEl);
	return switchEl;
}

//Workaround for localhost
function resolveURL(url)
{
	if (url[0] == "/" && document.location.href.indexOf("localhost/") > 0)
	{
		return "/adminNew" + url;
	}
	return url;
}

$(document).ready(function(){
	$(".process-offets").each(function() { processTableOffsets(this); });
	$(".date").datepicker({
				showOn: 'button',
				buttonImage: 'images/icons/calendar.png',
				buttonImageOnly: true
	});
	$(".date2").datepicker({
				showOn: 'button',
				numberOfMonths: 2,
				buttonImage: 'images/icons/calendar.png',
				buttonImageOnly: true
	});
	$(".data-loading").hide();
	$(".data").show();
	$("table.add-csv-export").each(function() { addCsvExportToTable(this); } );
	$("table.accordion").each(function() { processTableAccordion(this); });
})