using AssetManager.Data;
using AssetManager.Data.Communications;
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
    public partial class Hierarchy : ExtendedForm
    {
        private int employeesFound = 0;
        private List<Employee> employeeList;
        private Employee currentSupervisor;
        private EmployeeTree currentTree;

        public Hierarchy(ExtendedForm parentForm) : base(parentForm)
        {
            InitializeComponent();
            HierarchyTree.DoubleBuffered(true);
            StartSearch();
        }

        public async void StartSearch()
        {
            try
            {
                // Prompt user for supervisor.
                var supervisorMunis = MunisFunctions.MunisUserSearch(this);

                // Make sure we have an employee number to work with.
                if (string.IsNullOrEmpty(supervisorMunis.Number)) return;

                var supervisorThis = new Employee(supervisorMunis.Name, supervisorMunis.Number);
                currentSupervisor = supervisorThis;

                // Setup form and display working spinner.
                this.Text += " for " + supervisorThis.Name;
                this.Show();
                workingSpinner.Visible = true;

                // Populate the employee list.
                employeeList = await GetEmployeeList();

                // Make sure the form hasn't been disposed.
                if (!this.IsDisposed)
                {
                    // Build employee hierarchy tree.
                    var empTree = GetSubordinates(supervisorThis);
                    currentTree = empTree;
                    // Build node tree from the employee tree.
                    var nodeTree = new TreeNode(empTree.Employee.Name);
                    nodeTree.Tag = empTree.Employee.Number;
                    BuildTree(empTree, nodeTree);
                    // Set the tree view control to the node tree.
                    HierarchyTree.Nodes.Add(nodeTree);
                    HierarchyTree.Nodes[0].Expand();
                    // Report completion.
                    statusLabel.Text = employeesFound + " employees found.";
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
            finally
            {
                // Stop the spinner and remove the big employee list from scope.
                workingSpinner.Visible = false;
                employeeList?.Clear();
                employeeList = null;
            }
        }

        /// <summary>
        /// Collect and display a table containing all the devices associated with the current employee tree.
        /// </summary>
        private async void ShowAllDevices()
        {
            Waiting();

            var deviceTable = await Task.Run(() => { return GetDevicesTable(currentTree); });

            DoneWaiting();

            if (deviceTable.Rows.Count > 0)
            {
                var newGridView = new GridForm(ParentForm);
                newGridView.Text = "Supervisor Devices";
                newGridView.AddGrid("devices", "Supervisor Devices - " + currentSupervisor.Name, DoubleClickAction.ViewDevice, deviceTable);
                newGridView.Show();
            }
        }

        /// <summary>
        /// Display a table containing the devices associated with the specified employee.
        /// </summary>
        /// <param name="employee"></param>
        private void ShowDevicesByEmployee(Employee employee)
        {
            Waiting();

            var results = DBFactory.GetMySqlDatabase().DataTableFromQueryString(Queries.SelectDevicesByEmpNum(employee.Number));

            DoneWaiting();

            if (results.Rows.Count > 0)
            {
                var newGridView = new GridForm(ParentForm);
                newGridView.Text = "Devices For " + employee.Name;
                newGridView.AddGrid("devices", employee.Name + "'s Devices", DoubleClickAction.ViewDevice, results);
                newGridView.Show();
            }
        }

        /// <summary>
        /// Recursively collects a table of all devices associated with the employees in the specified tree.
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="table"></param>
        private DataTable GetDevicesTable(EmployeeTree tree, DataTable table = null)
        {
            if (table == null)
            {
                table = new DataTable("devices");
            }

            string query = Queries.SelectDevicesByEmpNum(tree.Employee.Number);

            using (var results = DBFactory.GetDatabase().DataTableFromQueryString(query))
            {
                if (results.Rows.Count > 0)
                {
                    table.Merge(results);
                }
            }

            if (tree.Subordinates.Count > 0)
            {
                foreach (var sub in tree.Subordinates)
                {
                    table.Merge(GetDevicesTable(sub, table));
                }
            }

            return table;
        }

        /// <summary>
        /// Pull a complete list of employees from the database.
        /// </summary>
        /// <returns></returns>
        private async Task<List<Employee>> GetEmployeeList()
        {
            MunisComms comms = new MunisComms();
            string query = "SELECT a_employee_number,a_name_last,a_name_first,e_supervisor FROM pr_employee_master";

            var tmpList = new List<Employee>();

            using (var results = await comms.ReturnSqlTableAsync(query))
            {
                foreach (DataRow row in results.Rows)
                {
                    var firstName = row["a_name_first"].ToString().Trim();
                    var lastName = row["a_name_last"].ToString().Trim();
                    var empNumber = row["a_employee_number"].ToString().Trim();
                    var supId = row["e_supervisor"].ToString().Trim();

                    tmpList.Add(new Employee(firstName + " " + lastName, empNumber, supId));
                }
            }
            return tmpList;
        }

        /// <summary>
        /// Recursively builds a node tree from the employee tree.
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="parentNode"></param>
        private void BuildTree(EmployeeTree tree, TreeNode parentNode = null)
        {
            if (parentNode == null)
            {
                parentNode = new TreeNode(tree.Employee.Name);
                parentNode.Tag = tree.Employee.Number;
            }

            foreach (var sub in tree.Subordinates)
            {
                // Add child nodes to parent node.
                var childNode = new TreeNode(sub.Employee.Name);
                childNode.Tag = sub.Employee.Number;
                parentNode.Nodes.Add(childNode);

                // If sub tree has subordinates, resurse with them.
                if (sub.Subordinates.Count > 0)
                {
                    BuildTree(sub, childNode);
                }
            }
        }

        /// <summary>
        /// Recursively searches data for specified employee/supervisor and builds a hierarchy tree.
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="tree"></param>
        /// <returns></returns>
        private EmployeeTree GetSubordinates(Employee employee, EmployeeTree tree = null)
        {
            if (this.IsDisposed)
            {
                return null;
            }

            // Start a new tree on first recurse.
            if (tree == null)
            {
                tree = new EmployeeTree(employee);
            }

            // Search the list of employees for matching supervisors.
            var results = employeeList.FindAll(e => e.SupervisorId == employee.Number);

            if (results.Count > 0)
            {
                employeesFound += results.Count;

                foreach (var emp in results)
                {
                    // Make sure employee is not set as their own supervisor to prevent endless loops.
                    if (emp.Number != employee.Number)
                    {
                        tree.Subordinates.Add(new EmployeeTree(new Employee(emp.Name, emp.Number)));
                        statusLabel.Text = "Searching... Found " + employeesFound;
                    }
                }
            }

            // Recurse with subordinates.
            if (tree.Subordinates.Count > 0)
            {
                foreach (var sub in tree.Subordinates)
                {
                    GetSubordinates(sub.Employee, sub);
                }
            }

            return tree;
        }

        private void ShowDevicesDropDown_Click(object sender, EventArgs e)
        {
            ShowAllDevices();
        }

        /// <summary>
        /// Employee tree object.
        /// </summary>
        private class EmployeeTree
        {
            public Employee Employee;
            public List<EmployeeTree> Subordinates;

            public EmployeeTree()
            {
                Subordinates = new List<EmployeeTree>();
            }

            public EmployeeTree(Employee employee)
            {
                Employee = employee;
                Subordinates = new List<EmployeeTree>();
            }
        }

        /// <summary>
        /// Object for storing employee info.
        /// </summary>
        private class Employee
        {
            public string Name;
            public string Number;
            public string SupervisorId;

            public Employee()
            {
            }

            public Employee(string name, string number)
            {
                Name = name;
                Number = number;
                SupervisorId = string.Empty;
            }

            public Employee(string name, string number, string supervisorId)
            {
                Name = name;
                Number = number;
                SupervisorId = supervisorId;
            }
        }

        private void ViewDevicesMenuItem_Click(object sender, EventArgs e)
        {
            ShowDevicesByEmployee(new Employee(HierarchyTree.SelectedNode.Text, HierarchyTree.SelectedNode.Tag.ToString()));
        }

        private void HierarchyTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            // Console.WriteLine(e.Node.Text + " - " + e.Node.Tag.ToString());
            HierarchyTree.SelectedNode = e.Node;
        }
    }
}