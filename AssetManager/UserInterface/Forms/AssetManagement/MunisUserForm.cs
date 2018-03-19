using AssetManager.Data;
using AssetManager.Data.Classes;
using AssetManager.Data.Communications;
using AssetManager.Data.Functions;
using AssetManager.Helpers;
using AssetManager.UserInterface.CustomControls;
using System;
using System.Data;
using System.Windows.Forms;

namespace AssetManager.UserInterface.Forms.AssetManagement
{
    public partial class MunisUserForm : ExtendedForm
    {
        public MunisEmployee EmployeeInfo
        {
            get
            {
                return SelectedEmpInfo;
            }
        }

        private MunisEmployee SelectedEmpInfo = new MunisEmployee();

        private const int intMaxResults = 50;

        public MunisUserForm(ExtendedForm parentForm) : base(parentForm)
        {
            Shown += MunisUserForm_Shown;
            Load += MunisUser_Load;
            InitializeComponent();
            ShowDialog(parentForm);
        }

        private async void EmpNameSearch(string name)
        {
            try
            {
                string columns = "a_employee_number,a_name_last,a_name_first,a_org_primary,a_object_primary,a_location_primary,a_location_p_desc,a_location_p_short";
                string query = "SELECT TOP " + intMaxResults + " " + columns + " FROM pr_employee_master";

                var searchParams = new QueryParamCollection();
                searchParams.Add("a_name_last", name.ToUpper(), false, "OR");
                searchParams.Add("a_name_first", name.ToUpper(), false, "OR");

                SetWorking(true);

                MunisComms comms = new MunisComms();
                using (var cmd = comms.GetSqlCommandFromParams(query, searchParams.Parameters))
                using (DataTable results = await comms.ReturnSqlTableFromCmdAsync(cmd))
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

        private void SetWorking(bool Working)
        {
            pbWorking.Visible = Working;
        }

        private void MunisUser_Load(object sender, EventArgs e)
        {
            MunisResults.DefaultCellStyle = StyleFunctions.DefaultGridStyles;
        }

        private void MunisResults_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            SelectedEmpInfo.Name = MunisResults.CurrentRowStringValue("a_name_first") + " " + MunisResults.CurrentRowStringValue("a_name_last");
            SelectedEmpInfo.Number = MunisResults.CurrentRowStringValue("a_employee_number");
            lblSelectedEmp.Text = "Selected Emp: " + SelectedEmpInfo.Name + " - " + SelectedEmpInfo.Number;
        }

        private void cmdSearch_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            EmpNameSearch(txtSearchName.Text.Trim());
            this.Cursor = Cursors.Default;
        }

        private void SelectEmp()
        {
            if (!string.IsNullOrEmpty(SelectedEmpInfo.Name) && !string.IsNullOrEmpty(SelectedEmpInfo.Number))
            {
                AssetManagerFunctions.AddNewEmp(SelectedEmpInfo);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                this.DialogResult = DialogResult.Abort;
                this.Close();
            }
        }

        private void cmdAccept_Click(object sender, EventArgs e)
        {
            SelectEmp();
        }

        private void txtSearchName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.Cursor = Cursors.WaitCursor;
                EmpNameSearch(txtSearchName.Text.Trim());
                this.Cursor = Cursors.Default;
            }
        }

        private void MunisResults_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            SelectEmp();
        }

        private void MunisUserForm_Shown(object sender, EventArgs e)
        {
            txtSearchName.Focus();
        }
    }
}