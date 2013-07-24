using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using TrimFuel.Business;

namespace TrimFuel.Web.Admin.Logic
{
    public class Error : Literal
    {
        protected override void OnInit(EventArgs e)
        {
            //Type = TypeEnum.Error;
            base.OnInit(e);

            Hide();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Hide();
        }

        public Unit? Width { get; set; }

        public void Show()
        {
            Visible = true;
        }

        public void Show(string text)
        {
            Text = text;
            Visible = true;
        }

        public void Show(string text, TypeEnum type)
        {
            Text = text;
            Type = type;
            Visible = true;
        }

        public void Show(string text, BusinessErrorState resultState)
        {
            Text = text;
            Type = (resultState == BusinessErrorState.Success ? TypeEnum.Notification : TypeEnum.Error);
            Visible = true;
        }

        public void Hide()
        {
            Visible = false;
        }

        public enum TypeEnum
        {
            Error = 1,
            Notification = 2
        }

        public TypeEnum Type { get; set; }

        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            string spanClass = (Type == TypeEnum.Error ? " class='small-alert'" : "");
            string widthString = (Width != null ? " style='width:" + Width.Value.ToString() + ";'" : "");
            writer.Write("<div id='errorMsg'" + widthString + ">"); 
            if (!string.IsNullOrEmpty(Text)) writer.Write("<span" + spanClass + ">");
            base.Render(writer);
            if (!string.IsNullOrEmpty(Text)) writer.Write("</span>"); 
            writer.Write("<div class='space'></div></div>");
        }
    }
}