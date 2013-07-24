using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace SelectDB
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();
                using(MySqlConnection sto = new MySqlConnection(textBox2.Text))
	            {
                    try
                    {
                        MySqlDataAdapter ad1 = new MySqlDataAdapter(textBox1.Text, sto);
                        DataTable dt1 = new DataTable();
                        ad1.Fill(dt1);

                        ShowHeader(dt1);
                        ShowRows(dt1, "Triangle");
                    }
                    catch (Exception ex)
                    {
                        int rowNum = dataGridView1.Rows.Add(new object[] { ex.ToString() });
                        dataGridView1.Rows[rowNum].HeaderCell.Value = "Triangle";
                    }

                    MySqlDataAdapter ad2 = new MySqlDataAdapter("select ConnectionStringDotNET, Name from TPClient", sto);                    
                    DataTable dt2 = new DataTable();
                    ad2.Fill(dt2);
                    foreach (DataRow item in dt2.Rows)
                    {
                        using (MySqlConnection client = new MySqlConnection(item.Field<string>("ConnectionStringDotNET")))
                        {
                            try
                            {
                                MySqlDataAdapter ad3 = new MySqlDataAdapter(textBox1.Text, client);
                                DataTable dt3 = new DataTable();
                                ad3.Fill(dt3);

                                ShowRows(dt3, item.Field<string>("Name"));
                            }
                            catch (Exception ex)
                            {
                                int rowNum = dataGridView1.Rows.Add(new object[] { ex.ToString() });
                                dataGridView1.Rows[rowNum].HeaderCell.Value = item.Field<string>("Name");
                            }
                        }
                    }
	            }                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ShowHeader(DataTable dt)
        {
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                dataGridView1.Columns.Add(i.ToString(), dt.Columns[i].ColumnName);              
            }
        }

        private void ShowRows(DataTable dt, string clientName)
        {
            foreach (DataRow item in dt.Rows)
            {
                int rowNum = dataGridView1.Rows.Add(item.ItemArray);
                dataGridView1.Rows[rowNum].HeaderCell.Value = clientName;
            }
        }
    }
}
