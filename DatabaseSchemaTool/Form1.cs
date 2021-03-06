﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
namespace DatabaseSchemaTool
{
    public partial class Form1 : Form
    {
        private DataSet tableNameDataSet = new System.Data.DataSet();
        DataSet exportSchemaTableDataSet = new System.Data.DataSet();
        SqlDataAdapter da = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string strServer = textBoxServer.Text;
            string strDb = textBoxDatabase.Text;
            string strUserName = textBoxUserName.Text;
            string strPassword = textBoxPassword.Text;
            string connStr = string.Format("Data Source={0};Initial Catalog={1};User ID={2};Password={3}"
                ,strServer,strDb,strUserName,strPassword);
            //if(connStr != Properties.Settings.Default.ConnectString)
            //{
            //    Properties.Settings.Default.ConnectString = connStr;
            //}
            SqlConnection conn = new SqlConnection(connStr);
            //dataGridView1.DataSource = null;
            string sql = "select 0 as 'select', table_name from information_schema.tables";
            try
            {
                tableNameDataSet.Tables.Clear();
                da = new SqlDataAdapter(sql, conn);
                da.Fill(tableNameDataSet);
            }
            catch(SqlException ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            string tableName = tableNameDataSet.Tables[0].TableName;
            
            dataGridView1.DataSource = tableNameDataSet.Tables[0];
            //dataGridView1.DataMember = tableName;
            //dataGridView1.Refresh();
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            DataTable dt = tableNameDataSet.Tables[0];
            foreach(DataRow dr in dt.Rows)
            {
                bool isSelect = (int)dr["select"] == 0 ? false : true;
                string strName = dr["table_name"] as string;
                if (!isSelect)
                    continue;
                string sql = string.Format("select * from [{0}] where 1=2", strName);
                da.SelectCommand.CommandText = sql;
                da.Fill(exportSchemaTableDataSet,strName);
            }
            if(exportSchemaTableDataSet.Tables.Count < 1)
            {
                MessageBox.Show("请选择导出的表!");
                return;
            }
            SaveFileDialog sfDlg = new SaveFileDialog();
            sfDlg.DefaultExt = "xml";
            sfDlg.Filter = "xml file (*.xml)|*.xml|All files (*.*)|*.*";
            if(sfDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                exportSchemaTableDataSet.WriteXmlSchema(sfDlg.FileName);
                MessageBox.Show("导出成功!");
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonSelectAll_Click(object sender, EventArgs e)
        {
            DataTable dt = tableNameDataSet.Tables[0];
            foreach(DataRow row in dt.Rows)
            {
                row["select"] = 1;
            }
        }

        private void buttonClearSelect_Click(object sender, EventArgs e)
        {
            DataTable dt = tableNameDataSet.Tables[0];
            foreach (DataRow row in dt.Rows)
            {
                row["select"] = 0;
            }
        }

        private void buttonExpData_Click(object sender, EventArgs e)
        {
            DataTable dt = tableNameDataSet.Tables[0];
            exportSchemaTableDataSet.Namespace = "";
            exportSchemaTableDataSet.Prefix = "";

            foreach (DataRow dr in dt.Rows)
            {
                bool isSelect = (int)dr["select"] == 0 ? false : true;
                string strName = dr["table_name"] as string;
                if (!isSelect)
                    continue;
                string sql = string.Format("select * from [{0}]", strName);
                da.SelectCommand.CommandText = sql;
                da.Fill(exportSchemaTableDataSet, strName);
            }
            if (exportSchemaTableDataSet.Tables.Count < 1)
            {
                MessageBox.Show("请选择导出的表!");
                return;
            }
            SaveFileDialog sfDlg = new SaveFileDialog();
            sfDlg.DefaultExt = "xml";
            sfDlg.Filter = "xml file (*.xml)|*.xml|All files (*.*)|*.*";
            if (sfDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {                
                exportSchemaTableDataSet.WriteXml(sfDlg.FileName,XmlWriteMode.IgnoreSchema);
                MessageBox.Show("导出成功!");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string strConn = Properties.Settings.Default.ConnectString;
            int index1 = strConn.IndexOf("Data Source=");
            int index2 = strConn.IndexOf(";", index1);
            string server = strConn.Substring(index1, index2 - index1).Replace("Data Source=", "");

            index1 = strConn.IndexOf("Initial Catalog=");
            index2 = strConn.IndexOf(";", index1);
            string database = strConn.Substring(index1, index2 - index1).Replace("Initial Catalog=", "");

            index1 = strConn.IndexOf("User ID=");
            index2 = strConn.IndexOf(";", index1);
            string userName = strConn.Substring(index1, index2 - index1).Replace("User ID=", "");

            index1 = strConn.IndexOf("Password=");            
            string password = strConn.Substring(index1).Replace("Password=", "");

            textBoxServer.Text = server;
            textBoxDatabase.Text = database;
            textBoxUserName.Text = userName;
            textBoxPassword.Text = password;
        }
    }
}
