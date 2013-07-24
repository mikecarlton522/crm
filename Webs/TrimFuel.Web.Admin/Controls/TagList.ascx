<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TagList.ascx.cs" Inherits="TrimFuel.Web.Admin.Controls.TagList" %>
<%@ Import Namespace="TrimFuel.Model" %>
<div id="tag-content" style="margin-left: auto; margin-right: auto; text-align: left;">
    <div id="elements-prototypes" style="display: none;">
        <span id="edit-box" name="edit-box" class="editBox">
            <input type="text" />
        </span>
        <ul>
            <li id="new-item" name="new-item">
                <asp:PlaceHolder runat="server" ID="phCheckbox" Visible='<%# IsGrouping %>'>
                    <input type="checkbox" name="tag-selected" style="display: block; float: left;" checked="checked" />
                </asp:PlaceHolder>
                <span name="value">                
                </span>
                <div name="tools">
                    <a href="#" name="edit" class="editIcon">Edit</a>&nbsp;
                    <a href="#" name="remove" class="removeIcon">Remove</a>&nbsp;
                    <!--<a href="#" name="group">Group</a>-->
                </div>
                <div name="edit-tools" style="display: none;">
                    <a href="#" name="save" class="saveIcon">Save</a>&nbsp;
                    <a href="#" name="cancel" class="cancelIcon">Cancel</a>&nbsp;
                </div>
            </li>
        </ul>
    </div>
    <div class="toolBox">
        <a href="#" id="add-new-tag" class="addNewIcon">Add New Tag</a>&nbsp;&nbsp;&nbsp;&nbsp;
        <a href="#" id="save-all-tag" class="diskIcon">Save All</a>&nbsp;&nbsp;&nbsp;&nbsp;
        <a href="#" id="cancel-all-tag" class="cancelIcon">Cancel All</a>
    </div>
    <ul id="tag-list">
    <asp:Repeater runat="server" ID="rTag">
        <ItemTemplate>
            <li tag-id="<%# DataBinder.Eval(Container.DataItem, "TagID") %>">
                <asp:PlaceHolder runat="server" ID="phCheckbox" Visible='<%# IsGrouping %>'>
                    <input type="checkbox" name="tag-selected" class='checkBox' <%# (IsInGroup((Tag)Container.DataItem)) ? " checked='checked'" : "" %> />
                </asp:PlaceHolder>
                <span name="value">
                    <%# DataBinder.Eval(Container.DataItem, "TagValue") %>
                </span>
                <div name="tools">
                    <a href="#" name="edit" class="editIcon">Edit</a>&nbsp;&nbsp;
                    <a href="#" name="remove" class="removeIcon">Remove</a>&nbsp;
                    <!--<a href="#" name="group">Group</a>-->
                </div>
                <div name="edit-tools" style="display: none;">
                    <a href="#" name="save" class="saveIcon">Save</a>&nbsp;&nbsp;
                    <a href="#" name="cancel" class="cancelIcon">Cancel</a>&nbsp;
                </div>
            </li>
        </ItemTemplate>
    </asp:Repeater>
    </ul>
    <br clear="both"/>
</div>
