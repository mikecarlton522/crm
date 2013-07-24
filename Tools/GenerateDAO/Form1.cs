using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace GenerateDAO
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            IEnumerable<string> types = from t in typeof(TrimFuel.Model.Entity).Assembly.GetTypes()
                                       where t.BaseType == typeof(TrimFuel.Model.Entity)
                                       orderby t.Name
                                       select t.Name;

            this.listBox1.Items.AddRange(types.ToArray());
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void listBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            ShowProps(listBox1.SelectedItem.ToString());
            ShowCode(listBox1.SelectedItem.ToString());
            ShowSQL(listBox1.SelectedItem.ToString());
        }

        private void ShowProps(string className)
        {
            checkedListBox1.Items.Clear();
            PropertyInfo[] props = Type.GetType("TrimFuel.Model." + className + ",TrimFuel.Model").GetProperties();
            foreach (var item in props)
            {
                checkedListBox1.Items.Add(item.Name.ToString());
            }
        }

        private void ShowSQL(string className)
        {
            textBox2.Clear();
            PropertyInfo[] props = Type.GetType("TrimFuel.Model." + className + ",TrimFuel.Model").GetProperties();

            textBox2.AppendText(@"CREATE TABLE `" + className + @"` (
 ");

            textBox2.AppendText(@"`" + FixProp(props[0].Name) + @"` " + GetMySQLType2(props[0]) + @" AUTO_INCREMENT,
");
            for (int i = 1; i < props.Length; i++)
            {
                textBox2.AppendText(@" `" + FixProp(props[i].Name) + @"` " + GetMySQLType2(props[i]) + @",
");
            }

            textBox2.AppendText(@" PRIMARY KEY (`" + FixProp(props[0].Name) + @"`)");
            foreach (var item in checkedListBox1.CheckedItems)
	        {
                textBox2.AppendText(@",
 CONSTRAINT `FK_" + className + @"_" + item.ToString() + @"` FOREIGN KEY (`" + item.ToString() + @"` ) REFERENCES `" + item.ToString().Replace("ID", "") + @"` (`" + item.ToString() + @"` )");
	        }
            textBox2.AppendText(@"
) ENGINE=InnoDB DEFAULT CHARSET=utf8;");
        }

        private void ShowCode(string className)
        {
            textBox1.Clear();
            PropertyInfo[] props = Type.GetType("TrimFuel.Model." + className + ",TrimFuel.Model").GetProperties();

            textBox1.AppendText(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class " + className + @"DataProvider : EntityDataProvider<" + className + @">
    {
        private const string INSERT_COMMAND = ""INSERT INTO " + className + "("
            + string.Join(", ", props.Select(i => FixProp(i.Name)).Skip(1).ToArray()) +
            @") VALUES(@"
            + string.Join(", @", props.Select(i => FixProp(i.Name)).Skip(1).ToArray()) +
            @"); SELECT @@IDENTITY;"";
        private const string UPDATE_COMMAND = ""UPDATE " + className + " SET "
            + string.Join(", ", props.Select(i => FixProp(i.Name) + "=@" + FixProp(i.Name)).Skip(1).ToArray()) +
            @" WHERE " + FixProp(props[0].Name) + @"=@" + FixProp(props[0].Name) + @";"";
        private const string SELECT_COMMAND = ""SELECT * FROM " + className + " WHERE " + FixProp(props[0].Name) + @"=@" + FixProp(props[0].Name) + @";"";" + 
            @"

        public override void Save(" + className + @" entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity." + FixProp(props[0].Name) + @" == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add(""@" + FixProp(props[0].Name) + @""", " + GetMySQLType(props[0]) + ").Value = entity." + FixProp(props[0].Name) + @";
            }

");
            for (int i = 1; i < props.Length; i++)
            {
                textBox1.AppendText(@"            cmd.Parameters.Add(""@" + FixProp(props[i].Name) + @""", " + GetMySQLType(props[i]) + @").Value = entity." + FixProp(props[i].Name) + @";
");
            }

            textBox1.AppendText(@"

            if (entity." + FixProp(props[0].Name) + @" == null)
            {
                entity." + FixProp(props[0].Name) + @" = Convert.To" + GetCSharpType(props[0]) + @"(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format(""" + className + @"({0}) was not found in database."", entity." + FixProp(props[0].Name) + @"));
                }
            }
        }

        public override " + className + @" Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add(""@" + FixProp(props[0].Name) + @""", " + GetMySQLType(props[0]) + @").Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override " + className + @" Load(DataRow row)
        {
            " + className + @" res = new " + className + @"();

");

            for (int i = 0; i < props.Length; i++)
            {
                textBox1.AppendText(@"            if (!(row[""" + FixProp(props[i].Name) + @"""] is DBNull))
                res." + FixProp(props[i].Name) + @" = Convert.To" + GetCSharpType(props[i]) + @"(row[""" + FixProp(props[i].Name) + @"""]);
");
            }

            textBox1.AppendText(@"
            return res;
        }
    }
}
");
        }

        private string FixProp(string propName)
        {
            return propName.TrimEnd('_');
        }

        private string GetMySQLType(PropertyInfo prop)
        {
            string res = prop.PropertyType.Name;
            if (typeof(string) == prop.PropertyType)
                res = "VarChar";
            else if (typeof(int?) == prop.PropertyType)
                res = "Int32";
            else if (typeof(long?) == prop.PropertyType)
                res = "Int64";
            else if (typeof(DateTime?) == prop.PropertyType)
                res = "Timestamp";
            else if (typeof(bool?) == prop.PropertyType)
                res = "Bit";
            else if (typeof(decimal?) == prop.PropertyType)
                res = "Decimal";
            return "MySqlDbType." + res;
        }

        private string GetMySQLType2(PropertyInfo prop)
        {
            string res = prop.PropertyType.Name;
            if (typeof(string) == prop.PropertyType)
                res = "VARCHAR(50) NOT NULL";
            else if (typeof(int?) == prop.PropertyType)
                res = "INT(10) NOT NULL";
            else if (typeof(long?) == prop.PropertyType)
                res = "BIGINT(19) NOT NULL";
            else if (typeof(DateTime?) == prop.PropertyType)
                res = "TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP";
            else if (typeof(bool?) == prop.PropertyType)
                res = "BIT(1) NOT NULL DEFAULT b'0'";
            else if (typeof(decimal?) == prop.PropertyType)
                res = "DECIMAL(19,2) NOT NULL";
            return res;
        }

        private string GetCSharpType(PropertyInfo prop)
        {
            string res = prop.PropertyType.Name;
            if (typeof(string) == prop.PropertyType)
                res = "String";
            else if (typeof(int?) == prop.PropertyType)
                res = "Int32";
            else if (typeof(long?) == prop.PropertyType)
                res = "Int64";
            else if (typeof(DateTime?) == prop.PropertyType)
                res = "DateTime";
            else if (typeof(bool?) == prop.PropertyType)
                res = "Boolean";
            else if (typeof(decimal?) == prop.PropertyType)
                res = "Decimal";
            return res;
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowSQL(listBox1.SelectedItem.ToString());
        }
    }
}
