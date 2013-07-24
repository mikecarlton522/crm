using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Web.UI.HtmlControls;

namespace TrimFuel.Web.Admin.Controls
{
    public abstract class ModelDataControl : System.Web.UI.UserControl
    {
        protected abstract void GetData();
        protected abstract void SetData();

        public event EventHandler Updated;
        public event EventHandler RequireData;

        protected virtual void OnUpdated()
        {
            if (Updated != null)
            {
                Updated(this, EventArgs.Empty);
            }
        }

        protected virtual void OnRequireData()
        {
            if (RequireData != null)
            {
                RequireData(this, EventArgs.Empty);
            }
        }

        public void LoadData()
        {
            LoadDataRecurcive(this);
                if (ViewMode == ViewModeEnum.Edit)
            {
                GetData();
            }
        }


        public ViewModeEnum ViewMode
        {
            get 
            {
                if (ViewState["ViewMode"] == null)
                {
                    ViewState["ViewMode"] = ViewModeEnum.View;
                }
                return (ViewModeEnum)ViewState["ViewMode"]; 
            }
            set { ViewState["ViewMode"] = value; }
        }



        //private HtmlInputHidden viewModeContainer = null;

        //protected override void CreateChildControls()
        //{
        //    base.CreateChildControls();

        //    viewModeContainer = new HtmlInputHidden(){
        //        ID = "viewModeContainer"
        //    };

        //    Controls.Add(viewModeContainer);
        //}

        //public ViewModeEnum ViewMode { get; set; }

        //private ViewModeEnum ViewModeContainer
        //{
        //    get 
        //    {
        //        if (string.IsNullOrEmpty(viewModeContainer.Value))
        //        {
        //            viewModeContainer.Value = ViewModeEnum.View.ToString();
        //        }
        //        return (ViewModeEnum)Enum.Parse(typeof(ViewModeEnum), viewModeContainer.Value); 
        //    }
        //    set { viewModeContainer.Value = value.ToString(); }
        //}

        //protected override object SaveControlState()
        //{
        //    ViewModeContainer = ViewMode;
        //    return base.SaveControlState();            
        //}

        //protected override void LoadControlState(object savedState)
        //{
        //    base.LoadControlState(savedState);
        //    ViewMode = ViewModeContainer;
        //}

        private void EventRecurcive(System.Web.UI.Control c)
        {
            foreach (System.Web.UI.Control child in c.Controls)
            {
                if (child is ModelDataControl)
                {
                    (child as ModelDataControl).Updated += new EventHandler(ModelDataControl_Updated);
                }
                else
                {
                    EventRecurcive(child);
                }
            }
        }

        private void ModelDataControl_Updated(object sender, EventArgs e)
        {
            OnUpdated();
        }

        private void LoadDataRecurcive(System.Web.UI.Control c)
        {
            foreach (System.Web.UI.Control child in c.Controls)
            {
                if (child is ModelDataControl)
                {
                    (child as ModelDataControl).LoadData();
                }
                else
                {
                    LoadDataRecurcive(child);
                }
            }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            OnRequireData();
            SetData();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            //viewModeContainer = new HtmlInputHidden()
            //{
            //    ID = "viewModeContainer"
            //};

            //Controls.Add(viewModeContainer);

            Load += new EventHandler(ModelDataControl_Load);
        }

        void ModelDataControl_Load(object sender, EventArgs e)
        {
            EventRecurcive(this);
            if (Page.IsPostBack)
            {
                OnRequireData();
                SetData();
            }
        }
    }
}
