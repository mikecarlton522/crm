<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TagList.ascx.cs" Inherits="Securetrialoffers.admin.Controls.TagList" %>
<%@ Import Namespace="TrimFuel.Model" %>
<div id="tag-content" style="margin-left: auto; margin-right: auto; text-align: left;">
    <div id="elements-prototypes" style="display: none;">
        <span id="edit-box" name="edit-box">
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
                    <a href="#" name="edit" class="edit">Edit</a>&nbsp;
                    <a href="#" name="remove" class="remove">Remove</a>&nbsp;
                    <!--<a href="#" name="group">Group</a>-->
                </div>
                <div name="edit-tools" style="display: none;">
                    <a href="#" name="save" class="save">Save</a>&nbsp;
                    <a href="#" name="cancel" class="cancel">Cancel</a>&nbsp;
                </div>
            </li>
        </ul>
    </div>
    <hr />
    <a href="#" id="add-new-tag" class="addNew">Add New Tag</a>&nbsp;&nbsp;&nbsp;&nbsp;
    <a href="#" id="save-all-tag" class="saveAll">Save All</a>&nbsp;&nbsp;&nbsp;&nbsp;
    <a href="#" id="cancel-all-tag" class="cancelAll">Cancel All</a>
    <hr />
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
                    <a href="#" name="edit" class="edit">Edit</a>&nbsp;&nbsp;
                    <a href="#" name="remove" class="remove">Remove</a>&nbsp;
                    <!--<a href="#" name="group">Group</a>-->
                </div>
                <div name="edit-tools" style="display: none;">
                    <a href="#" name="save" class="save">Save</a>&nbsp;&nbsp;
                    <a href="#" name="cancel" class="cancel">Cancel</a>&nbsp;
                </div>
            </li>
        </ItemTemplate>
    </asp:Repeater>
    </ul>
    <br clear="both"/>
</div>
