﻿using AssetManager.UserInterface.CustomControls;
using AssetManager.Data;
using AssetManager.Data.Classes;
using AssetManager.Data.Communications;
using AssetManager.Data.Functions;
using AssetManager.Helpers;
using AssetManager.Security;
using AssetManager.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssetManager.UserInterface.Forms.AdminTools
{
    public partial class AdvancedSearchForm : ExtendedForm
    {
        private AdvancedSearch AdvSearch = new AdvancedSearch();


        public AdvancedSearchForm(ExtendedForm parentForm)
        {
            InitializeComponent();
            this.ParentForm = parentForm;
            PopulateTableTree();
            this.Show();
        }

        private void PopulateTableTree()
        {
            try
            {
                foreach (var table in GetTables())
                {
                    TreeNode parentNode = new TreeNode(table);
                    parentNode.Tag = false;
                    TreeNode childAllNode = new TreeNode("*All");
                    childAllNode.Tag = true;
                    childAllNode.Checked = true;
                    parentNode.Nodes.Add(childAllNode);
                    foreach (var col in AdvSearch.GetColumns(table))
                    {
                        TreeNode childNode = new TreeNode(col);
                        childNode.Tag = false;
                        childNode.Checked = true;
                        parentNode.Nodes.Add(childNode);
                    }
                    TableTree.Nodes.Add(parentNode);
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
            }
        }

        private List<string> GetTables()
        {
            List<string> Tables = new List<string>();
            var Qry = "SHOW TABLES IN " + ServerInfo.CurrentDataBase.ToString();
            using (DataTable Results = DBFactory.GetDatabase().DataTableFromQueryString(Qry))
            {
                foreach (DataRow row in Results.Rows)
                {
                    Tables.Add(row[Results.Columns[0].ColumnName].ToString());
                }
            }

            return Tables;
        }


        private async void StartSearch()
        {
            try
            {
                OtherFunctions.SetWaitCursor(true, ParentForm);
                AdvSearch = new AdvancedSearch(SearchStringTextBox.Text.Trim(), GetSelectedTables()); // GetSelectedTables.ToArray, GetSelectedColumns.ToArray)
                GridForm DisplayGrid = new GridForm(ParentForm, "Advanced Search Results");

                List<DataTable> Tables = await Task.Run(() =>
                {
                    return AdvSearch.GetResults();
                });

                foreach (var table in Tables)
                {
                    DisplayGrid.AddGrid(table.TableName, table.TableName, table);
                }
                DisplayGrid.Show();
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
            }
            finally
            {
                OtherFunctions.SetWaitCursor(false, ParentForm);
            }
        }

        private List<TableInfo> GetSelectedTables()
        {
            List<TableInfo> tables = new List<TableInfo>();
            foreach (TreeNode node in TableTree.Nodes)
            {
                if (node.Checked)
                {
                    tables.Add(new TableInfo(node.Text, GetSelectedColumns(node)));
                }
            }
            return tables;
        }

        private List<string> GetSelectedColumns(TreeNode TableNode)
        {
            List<string> columns = new List<string>();
            foreach (TreeNode childNode in TableNode.Nodes)
            {
                if (childNode.Index > 0)
                {
                    if (childNode.Checked)
                    {
                        columns.Add(childNode.Text);
                    }
                }
            }
            return columns;
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            StartSearch();
        }

        private void SearchStringTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                StartSearch();
            }
        }

        private void TableTree_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action != TreeViewAction.Unknown)
            {
                if (e.Node.Level > 0)
                {
                    if (e.Node.Index == 0)
                    {
                        foreach (TreeNode n in e.Node.Parent.Nodes)
                        {
                            if (n.Index > 0)
                            {
                                n.Checked = e.Node.Checked;
                            }
                        }
                    }
                    else
                    {
                        if (e.Node.Checked == false && e.Node.Parent.Nodes[0].Checked)
                        {
                            e.Node.Parent.Nodes[0].Checked = false;
                        }
                    }
                }
            }
        }
    }
}