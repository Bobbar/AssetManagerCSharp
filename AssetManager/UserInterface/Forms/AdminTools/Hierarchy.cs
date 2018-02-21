using AssetManager.Data;
using AssetManager.Data.Classes;
using AssetManager.Data.Communications;
using AssetManager.Data.Functions;
using AssetManager.UserInterface.CustomControls;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssetManager.UserInterface.Forms.AdminTools
{

    public partial class Hierarchy : ExtendedForm
    {
        private MunisComms MunisComms = new MunisComms();
        private int subsFound = 0;

        public Hierarchy(ExtendedForm parentForm) : base(parentForm)
        {
            InitializeComponent();
            this.Show();
            StartSearch();
        }

        public async void StartSearch()
        {
            var supervisor = MunisFunctions.MunisUserSearch(this);
            workingSpinner.Visible = true;
            var empTree = await GetSubordinates(supervisor);
            if (!this.IsDisposed)
            {
                var nodeTree = new TreeNode(empTree.Employee.Name);
                BuildTree(empTree, nodeTree);
                HierarchyTree.Nodes.Add(nodeTree);
                statusLabel.Text = "Done. " + subsFound + " employees found.";
                workingSpinner.Visible = false;
            }
        }

        private void BuildTree(EmpTree tree, TreeNode node = null)
        {
            if (node == null)
            {
                node = new TreeNode(tree.Employee.Name);
            }

            foreach (var sub in tree.Subordinates)
            {
                TreeNode childNode = new TreeNode(sub.Employee.Name);
                node.Nodes.Add(childNode);

                if (sub.Subordinates.Count > 0)
                {
                    BuildTree(sub, childNode);
                }
            }
        }


        /// <summary>
        /// Recursively searches database for specified employee/supervisor and builds a hierarchy tree.
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="tree"></param>
        /// <returns></returns>
        private async Task<EmpTree> GetSubordinates(MunisEmployee employee, EmpTree tree = null)
        {
            int maxResults = 300;

            if (this.IsDisposed)
            {
                return null;
            }

            // Start a new tree on first recurse.
            if (tree == null)
            {
                tree = new EmpTree(employee);
            }


            // Query the database for employees whose supervisor matches the specified employee.
            string strColumns = "a_employee_number,a_name_last,a_name_first,e_supervisor";
            string strQRY = "SELECT TOP " + maxResults + " " + strColumns + @" FROM pr_employee_master";

            QueryParamCollection searchParams = new QueryParamCollection();
            searchParams.Add("e_supervisor", employee.Number, true);
            using (var cmd = MunisComms.GetSqlCommandFromParams(strQRY, searchParams.Parameters))
            using (var results = await MunisComms.ReturnSqlTableFromCmdAsync(cmd))
            {
                if (results.Rows.Count > 0)
                {
                    subsFound += results.Rows.Count;
                    foreach (DataRow row in results.Rows)
                    {
                        string firstName = row["a_name_first"].ToString().Trim();
                        string lastName = row["a_name_last"].ToString().Trim();
                        string empNumber = row["a_employee_number"].ToString().Trim();

                        // Make sure employee is not set as their own supervisor to prevent endless loops.
                        if (empNumber != employee.Number)
                        {
                            tree.Subordinates.Add(new EmpTree(new MunisEmployee(firstName + " " + lastName, empNumber)));
                            statusLabel.Text = "Searching... Found " + subsFound;
                        }
                    }
                }

                // Recurse with subordinates.

                if (tree.Subordinates.Count > 0)
                {
                    foreach (var sub in tree.Subordinates)
                    {
                        await GetSubordinates(sub.Employee, sub);
                    }
                }
            }
            return tree;
        }

        private class EmpTree
        {
            public MunisEmployee Employee;
            public List<EmpTree> Subordinates;

            public EmpTree()
            {
                Subordinates = new List<EmpTree>();
            }

            public EmpTree(MunisEmployee employee)
            {
                Employee = employee;
                Subordinates = new List<EmpTree>();
            }
        }
    }
}