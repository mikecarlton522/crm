<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ClientList.aspx.cs" Inherits="EditForms_ClientList" %>

<input type="hidden" name="action" value="process" />
<div class="module" style="width: 95%">
    <h2>
        Edit Client List</h2>
    <a href="javascript:addClient()" class="addNewIcon">Add Client</a>
    <div class="toolbox" id="clients">
        <asp:Literal ID="ltClients" runat="server" />
        <div style="display: none;" id="client-prototype">
            <select name="clientId">
                <option>-- Select --</option>
                <asp:repeater runat="server" id="rClients">
                     <ItemTemplate>
                        <option value="<%# Eval("TPClientID")  %>"><%# Eval("Name") %></option>
                     </ItemTemplate>
                </asp:repeater>
            </select>
            <a href="#" onclick="return removeClient(this);" class="removeIcon" style="float: right;">Remove</a>
        </div>
    </div>
</div>
<script type="text/javascript">
    function addClient() {
        $("#client-prototype").clone().attr("id", "").appendTo($("#clients")).show();
    }

    function removeClient(el) {
        $(el).parent().remove();
        return false;
    }

</script>
