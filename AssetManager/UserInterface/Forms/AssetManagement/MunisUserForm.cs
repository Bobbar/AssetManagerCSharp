using AssetManager.Data;
using AssetManager.Data.Classes;
using AssetManager.Data.Communications;
using AssetManager.Data.Functions;
using AssetManager.Helpers;
using AssetManager.UserInterface.CustomControls;
using System;
using System.Windows.Forms;

namespace AssetManager.UserInterface.Forms.AssetManagement
{
    public partial class MunisUserForm : ExtendedForm
    {
        private const int maxResults = 50;

        private MunisEmployee selectedEmpInfo = new MunisEmployee();

        public MunisUserForm(ExtendedForm parentForm) : base(parentForm)
        {
            Shown += MunisUserForm_Shown;
            Load += MunisUser_Load;
            InitializeComponent();
            ShowDialog(parentForm);
        }

        public MunisEmployee EmployeeInfo
        {
            get
            {
                return selectedEmpInfo;
            }
        }

        private async void EmpNameSearch(string name)
        {
            try
            {
                string columns = "a_employee_number,a_name_last,a_name_first,a_org_primary,a_object_primary,a_location_primary,a_location_p_desc,a_location_p_short";
                string query = "SELECT TOP " + maxResults + " " + columns + " FROM pr_employee_master";

                var searchParams = new QueryParamCollection();
                searchParams.Add("a_name_last", name.ToUpper(), false, "OR");
                searchParams.Add("a_name_first", name.ToUpper(), false, "OR");

                SetWorking(true);

                using (var cmd = MunisComms.GetSqlCommandFromParams(query, searchParams.Parameters))
                using (var results = await MunisComms.ReturnSqlTableFromCmdAsync(cmd))
                {
                    if (results.Rows.Count > 0)
                    {
                        MunisResults.DataSource = null;
                        MunisResults.DataSource = results;
                        MunisResults.ClearSelection();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
            finally
            {
                SetWorking(false);
            }
        }

        private void MunisResults_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedEmpInfo.Name = MunisResults.CurrentRowStringValue("a_name_first") + " " + MunisResults.CurrentRowStringValue("a_name_last");
            selectedEmpInfo.Number = MunisResults.CurrentRowStringValue("a_employee_number");
            SelectedEmpLabel.Text = "Selected Emp: " + selectedEmpInfo.Name + " - " + selectedEmpInfo.Number;
        }

        private void MunisResults_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            SelectEmp();
        }

        private void MunisUser_Load(object sender, EventArgs e)
        {
            MunisResults.DefaultCellStyle = StyleFunctions.DefaultGridStyles;
        }

        private void MunisUserForm_Shown(object sender, EventArgs e)
        {
            SearchNameTextBox.Focus();
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            StartSearch();
        }

        private void SearchNameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                StartSearch();
            }
        }

        private void SelectButton_Click(object sender, EventArgs e)
        {
            SelectEmp();
        }

        private void SelectEmp()
        {
            if (!string.IsNullOrEmpty(selectedEmpInfo.Name) && !string.IsNullOrEmpty(selectedEmpInfo.Number))
            {
                AssetManagerFunctions.AddNewEmp(selectedEmpInfo);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                this.DialogResult = DialogResult.Abort;
                this.Close();
            }
        }

        private void SetWorking(bool value)
        {
            if (value)
            {
                this.Cursor = Cursors.WaitCursor;
            }
            else
            {
                this.Cursor = Cursors.Default;
            }

            WorkSpinner.Visible = value;
        }

        private void StartSearch()
        {
            var searchText = SearchNameTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(searchText))
            {
                EmpNameSearch(searchText);
            }
        }
    }
}