using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.Services;

using TrimFuel.Business;
using TrimFuel.Model;
using TrimFuel.Web.Admin.Logic;

namespace TrimFuel.Web.Admin
{
    public partial class management_errors : PageX
    {
        private const GroupMode DEFAULT_GROUP_MODE = GroupMode.Application;

        private ErrorsLogService _errorsLogService = new ErrorsLogService();

        private GroupMode _groupMode = DEFAULT_GROUP_MODE;

        protected void DoErrorGroupAction(object sender, CommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "application":
                    _groupMode = GroupMode.Application;
                    break;
                case "applicationID":
                    _groupMode = GroupMode.ApplicationID;
                    break;
                case "briefError":
                    _groupMode = GroupMode.BriefErrorText;
                    break;
                case "className":
                    _groupMode = GroupMode.ClassName;
                    break;
            }

            DataBind();
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            IList<ErrorsLog> errors = _errorsLogService.GetAllErrors();

            if (errors.Count > 0)
            {
                switch (_groupMode)
                {
                    case GroupMode.Application:
                        errors = errors.OrderBy(x => x.Application).ToList();                        
                        break;
                    case GroupMode.ApplicationID:
                        errors = errors.OrderBy(x => x.ApplicationID).ToList();
                        break;
                    case GroupMode.BriefErrorText:
                        errors = errors.OrderBy(x => x.BriefErrorText).ToList();
                        break;
                    case GroupMode.ClassName:
                        errors = errors.OrderBy(x => x.ClassName).ToList();
                        break;
                }

                StringBuilder html = new StringBuilder();

                int i = 0;
                string groupRow = "<tr class=\"subheader master level0\" master-id=\"err-{0}\"><td colspan=\"7\">{1}</td><td><a href=\"javascript:void(0);\" onclick=\"groupDelete('{1}','{2}');\">Resolve</a></td></tr>";
                string row = @"
                        <tr class=""detail level1"" detail-id=""err-{0}"">
                        <td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td>
                        <td><a href=""javascript:void(0);"" onclick=""viewErrorDetails({1})"">Details</a></td>
                        <td><a href=""javascript:void(0);"" onclick=""singleDelete({1})"">Resolve</a></td></tr>";
                string currentIdentifier = string.Empty;
                string identifier = string.Empty;
                string groupMode = string.Empty;

                foreach (ErrorsLog error in errors)
                {
                    switch (_groupMode)
                    {
                        case GroupMode.Application:
                            identifier = error.Application;
                            groupMode = "application";
                            break;
                        case GroupMode.ApplicationID:
                            identifier = error.ApplicationID;
                            groupMode = "applicationID";
                            break;
                        case GroupMode.BriefErrorText:
                            identifier = error.BriefErrorText;
                            groupMode = "briefErrorText";
                            break;
                        case GroupMode.ClassName:
                            identifier = error.ClassName;
                            groupMode = "className";
                            break;
                    }

                    if (!currentIdentifier.Equals(identifier))
                    {
                        i++;
                        html.AppendFormat(groupRow, i, identifier, groupMode);
                        html.AppendLine();

                        currentIdentifier = identifier;
                    }

                    html.AppendFormat(row, i, error.ErrorsLogID, error.ErrorDate.ToString("yyyy-MM-dd HH:mm"), error.Application, error.ApplicationID, error.ClassName, error.BriefErrorText);
                    html.AppendLine();
                }

                ltOutput.Text = html.ToString();
            }
        }

        public override string HeaderString
        {
            get { return "Error Management"; }
        }

        [WebMethod]
        public static void GroupDelete(string identifier, string type)
        {
            ErrorsLogService els = new ErrorsLogService();

            switch (type)
            {
                case "application":
                    els.DeleteErrorsByApplication(identifier);
                    break;
                case "applicationID":
                    els.DeleteErrorsByApplicationID(identifier);
                    break;
                case "briefErrorText":
                    els.DeleteErrorsByBriefErrorText(identifier);
                    break;
                case "className":
                    els.DeleteErrorsByClassName(identifier);
                    break;
            }
        }

        [WebMethod]
        public static void SingleDelete(int id)
        {
            ErrorsLogService els = new ErrorsLogService();

            els.DeleteErrorsById(id);            
        }

        private enum GroupMode
        {
            Application,
            ApplicationID,
            ClassName,
            BriefErrorText,
        }
    }
}