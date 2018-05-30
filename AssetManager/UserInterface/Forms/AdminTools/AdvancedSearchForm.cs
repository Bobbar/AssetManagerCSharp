using AssetManager.Data;
using AssetManager.Data.Functions;
using AssetManager.Helpers;
using AssetManager.UserInterface.CustomControls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssetManager.UserInterface.Forms.AdminTools
{
    public partial class AdvancedSearchForm : ExtendedForm
    {
        private AdvancedSearch advancedSearch = new AdvancedSearch();

        public AdvancedSearchForm(ExtendedForm parentForm) : base(parentForm)
        {
            InitializeComponent();
            PopulateTableTree();
            this.Show();
        }

        private void PopulateTableTree()
        {
            try
            {
                foreach (var tableName in GetTableNames())
                {
                    var parentNode = new TreeNode(tableName);
                    var childAllNode = new TreeNode("*All");

                    parentNode.Tag = false;
                    childAllNode.Tag = true;
                    childAllNode.Checked = true;

                    parentNode.Nodes.Add(childAllNode);

                    foreach (var col in advancedSearch.GetColumns(tableName))
                    {
                        var childNode = new TreeNode(col);
                        childNode.Tag = false;
                        childNode.Checked = true;
                        parentNode.Nodes.Add(childNode);
                    }

                    TableTree.Nodes.Add(parentNode);
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        private List<string> GetTableNames()
        {
            var tables = new List<string>();
            var query = "SHOW TABLES IN " + ServerInfo.CurrentDataBase.ToString();

            using (var results = DBFactory.GetDatabase().DataTableFromQueryString(query))
            {
                foreach (DataRow row in results.Rows)
                {
                    tables.Add(row[results.Columns[0].ColumnName].ToString());
                }
            }

            return tables;
        }

        private async void StartSearch()
        {
            try
            {
                ParentForm.Waiting();

                var searchString = SearchStringTextBox.Text.Trim();
                var selectedTables = GetSelectedTables();

                if (string.IsNullOrEmpty(searchString) || selectedTables.Count < 1) return;

                advancedSearch = new AdvancedSearch(searchString, selectedTables);

                List<DataTable> tables = await Task.Run(() =>
                        {
                            return advancedSearch.GetResults();
                        });

                var displayGrid = new GridForm(ParentForm, "Advanced Search Results");
                foreach (var table in tables)
                {
                    displayGrid.AddGrid(table.TableName, table.TableName, table);
                }
                displayGrid.Show();
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
            finally
            {
                ParentForm.DoneWaiting();
            }
        }

        private List<TableInfo> GetSelectedTables()
        {
            var tables = new List<TableInfo>();

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
            var columns = new List<string>();

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
            // Set all other nodes checked values to match the top "All" node.

            if ((e.Action != TreeViewAction.Unknown) && (e.Node.Level > 0))
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