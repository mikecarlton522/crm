using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using System.Data.Linq;
using TrimFuel.Business.Utils;

namespace UpdateDBApplication
{
    public partial class fmMain : Form
    {
        DBRepository _repository = new DBRepository();

        public fmMain()
        {
            InitializeComponent();
        }

        private void fmMain_Load(object sender, EventArgs e)
        {
            chListBDs.Items.Clear();
            var clients = _repository.GetClientList();
            foreach (var client in clients)
                chListBDs.Items.Add(client.Name + "_" + client.TPClientID, true);
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            foreach (var checkItem in chListBDs.CheckedItems)
            {
                string[] values = checkItem.ToString().Split('_');
                if (values != null && values.Length == 2)
                {
                    int? clientID = Utility.TryGetInt(values[1]);
                    _repository.RunScript(txtQuery.Text.Replace('\n', ' ').Replace('\r', ' ').Trim(), 
                                          _repository.GetConnectionStringForClient(clientID));
                }
            }
            this.Enabled = true;
        }
    }
}
