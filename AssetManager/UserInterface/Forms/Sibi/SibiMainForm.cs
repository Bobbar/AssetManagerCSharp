using AssetManager.Data;
using AssetManager.Data.Classes;
using AssetManager.Data.Communications;
using AssetManager.Data.Functions;
using AssetManager.Helpers;
using AssetManager.Tools;
using AssetManager.UserInterface.CustomControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssetManager.UserInterface.Forms.Sibi
{
    public partial class SibiMainForm : ExtendedForm
    {
        private bool bolGridFilling = false;
        private WindowList MyWindowList;
        private DbCommand LastCmd;
        private bool bolRebuildingCombo = false;

        private List<StatusColumnColor> StatusColors;

        public SibiMainForm(ExtendedForm parentForm)
        {
            MyWindowList = new WindowList(this);
            Disposed += SibiMainForm_Disposed;
            Closing += frmSibiMain_Closing;
            this.InheritTheme = false;
            this.ParentForm = parentForm;
            // This call is required by the designer.
            InitializeComponent();

            InitForm();
        }

        private void InitForm()
        {
            try
            {
                this.Icon = Properties.Resources.sibi_icon;
                SibiResultGrid.DoubleBufferedDataGrid(true);
                this.GridTheme = new GridTheme(Colors.HighlightBlue, Colors.SibiSelectColor, Colors.SibiSelectAltColor, SibiResultGrid.DefaultCellStyle.BackColor);
                StyleFunctions.SetGridStyle(SibiResultGrid, this.GridTheme);
                ToolStrip1.BackColor = Colors.SibiToolBarColor;
                ImageCaching.CacheControlImages(this);
                MyWindowList.InsertWindowList(ToolStrip1);
                SetDisplayYears();
                this.Show();
                this.Activate();
                Application.DoEvents();
                ShowAll("All");
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
                this.Dispose();
            }
        }

        public override void RefreshData()
        {
            ExecuteCmd(ref LastCmd);
        }

        private void ClearAll(System.Windows.Forms.Control.ControlCollection TopControl)
        {
            foreach (Control ctl in TopControl)
            {
                if (ctl is TextBox)
                {
                    TextBox txt = (TextBox)ctl;
                    txt.Clear();
                }
                else if (ctl is ComboBox)
                {
                    ComboBox cmb = (ComboBox)ctl;
                    cmb.SelectedIndex = 0;
                }
                else if (ctl.Controls.Count > 0)
                {
                    ClearAll(ctl.Controls);
                }
            }
        }

        private void RefreshResetButton_Click(object sender, EventArgs e)
        {
            try
            {
                OtherFunctions.SetWaitCursor(true, this);
                searchSlider.Clear();
                ClearAll(this.Controls);
                SetDisplayYears();
                ShowAll();
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
            }
            finally
            {
                OtherFunctions.SetWaitCursor(false, this);
            }
        }

        private List<DBQueryParameter> BuildSearchListNew()
        {
            List<DBQueryParameter> tmpList = new List<DBQueryParameter>();
            tmpList.Add(new DBQueryParameter(SibiRequestCols.RTNumber, txtRTNum.Text.Trim(), false));
            tmpList.Add(new DBQueryParameter(SibiRequestCols.Description, txtDescription.Text.Trim(), false));
            tmpList.Add(new DBQueryParameter(SibiRequestCols.PO, txtPO.Text, false));
            tmpList.Add(new DBQueryParameter(SibiRequestCols.RequisitionNumber, txtReq.Text, false));

            //Filter out unpopulated fields.
            var popList = new List<DBQueryParameter>();
            foreach (DBQueryParameter param in tmpList)
            {
                if (param.Value.ToString() != "")
                {
                    popList.Add(param);
                }
            }
            return popList;
        }

        //dynamically creates sql query using any combination of search filters the users wants
        private void DynamicSearch()
        {
            var cmd = DBFactory.GetDatabase().GetCommand();
            string strStartQry = null;
            strStartQry = "SELECT * FROM " + SibiRequestCols.TableName + " WHERE";
            string strDynaQry = "";
            List<DBQueryParameter> SearchValCol = BuildSearchListNew();
            foreach (DBQueryParameter fld in SearchValCol)
            {
                if ((fld.Value != null))
                {
                    if (!string.IsNullOrEmpty(fld.Value.ToString()))
                    {
                        strDynaQry += " " + fld.FieldName + " LIKE @" + fld.FieldName;
                        string Value = "%" + fld.Value.ToString() + "%";
                        cmd.AddParameterWithValue("@" + fld.FieldName, Value);
                        if (SearchValCol.IndexOf(fld) != SearchValCol.Count - 1)
                        {
                            strDynaQry += " AND";
                        }
                    }
                }
            }
            if (string.IsNullOrEmpty(strDynaQry))
            {
                return;
            }
            var strQry = strStartQry + strDynaQry;
            strQry += " ORDER BY " + SibiRequestCols.RequestNumber + " DESC";
            cmd.CommandText = strQry;
            ExecuteCmd(ref cmd);
        }

        /// <summary>
        /// Searches all request item columns for item that match the specified search string.
        /// </summary>
        private async void ItemSearch(string searchString)
        {
            // If search string is empty, clear slider label and return.
            if (searchString.Trim() == "")
            {
                searchSlider.Clear();
                return;
            }

            // Get a new AdvancedSearch instance.
            AdvancedSearch AdvSearch = new AdvancedSearch();

            // Perform search on Request Items table
            using (DataTable results = AdvSearch.GetSingleTableResults(searchString.Trim(), SibiRequestItemsCols.TableName))
            {

                // Make sure we have results.
                if (results.Rows.Count > 0)
                {

                    //Clear slider label of any previous errors.
                    searchSlider.Clear();

                    // Iterate through results and use Request Items Request UID column to query for the full request data.
                    // Task.Run lambda to keep UI alive.
                    DataTable resultsTable = await Task.Run(() =>
                    {
                        DataTable rtables = new DataTable();
                        foreach (DataRow row in results.Rows)
                        {
                            DataTable requestTable = DBFactory.GetDatabase().DataTableFromQueryString(Queries.SelectSibiRequestsByGUID(row[SibiRequestItemsCols.RequestUID].ToString()));
                            // Merge results into one table.
                            rtables.Merge(requestTable);
                        }
                        return rtables;
                    });
                    // Display the table containing a collection of corresponding Requests.
                    SendToGrid(resultsTable);
                }
                else
                {
                    // Notify user of no results.
                    searchSlider.NewSlideMessage("No matches found!", SlideDirection.Up, SlideDirection.Down, 0);
                }
            }
        }


        private void ExecuteCmd(ref DbCommand cmd)
        {
            try
            {
                LastCmd = cmd;
                SendToGrid(DBFactory.GetDatabase().DataTableFromCommand(cmd));
            }
            catch (Exception ex)
            {
                //InvalidCastException is expected when the last LastCmd was populated while in cached DB mode and now cached mode is currently false.
                //ShowAll will start a new connection and populate LastCmd with a correctly matching DBCommand. See DBFactory.GetCommand()
                if (ex is InvalidCastException)
                {
                    ShowAll();
                }
                else
                {
                    ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
                }
            }
        }

        public void SendToGrid(DataTable results)
        {
            try
            {
                using (results)
                {
                    bolGridFilling = true;
                    SibiResultGrid.SuspendLayout();
                    SibiResultGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
                    SibiResultGrid.ColumnHeadersHeight = 38;
                    StatusColors = GetStatusColors(results);
                    SibiResultGrid.Populate(results, SibiTableColumns());
                    SibiResultGrid.FastAutoSizeColumns();
                    SibiResultGrid.ClearSelection();
                    SibiResultGrid.ResumeLayout();
                    bolGridFilling = false;
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
            }
        }

        private List<StatusColumnColor> GetStatusColors(DataTable Results)
        {
            List<StatusColumnColor> StatusList = new List<StatusColumnColor>();
            foreach (DataRow row in Results.Rows)
            {
                StatusList.Add(new StatusColumnColor(row[SibiRequestCols.RequestNumber].ToString(), GetRowColor(row[SibiRequestCols.Status].ToString())));
            }
            return StatusList;
        }

        private List<GridColumnAttrib> SibiTableColumns()
        {
            List<GridColumnAttrib> ColList = new List<GridColumnAttrib>();
            ColList.Add(new GridColumnAttrib(SibiRequestCols.RequestNumber, "Request #", typeof(int)));
            ColList.Add(new GridColumnAttrib(SibiRequestCols.Status, "Status", GlobalInstances.SibiAttribute.StatusType, ColumnFormatTypes.AttributeDisplayMemberOnly));
            ColList.Add(new GridColumnAttrib(SibiRequestCols.Description, "Description", typeof(string)));
            ColList.Add(new GridColumnAttrib(SibiRequestCols.RequestUser, "Request User", typeof(string)));
            ColList.Add(new GridColumnAttrib(SibiRequestCols.Type, "Request Type", GlobalInstances.SibiAttribute.RequestType, ColumnFormatTypes.AttributeDisplayMemberOnly));
            ColList.Add(new GridColumnAttrib(SibiRequestCols.NeedBy, "Need By", typeof(System.DateTime)));
            ColList.Add(new GridColumnAttrib(SibiRequestCols.PO, "PO Number", typeof(string)));
            ColList.Add(new GridColumnAttrib(SibiRequestCols.RequisitionNumber, "Req. Number", typeof(string)));
            ColList.Add(new GridColumnAttrib(SibiRequestCols.RTNumber, "RT Number", typeof(string)));
            ColList.Add(new GridColumnAttrib(SibiRequestCols.DateStamp, "Create Date", typeof(System.DateTime)));
            ColList.Add(new GridColumnAttrib(SibiRequestCols.UID, "UID", typeof(string)));
            return ColList;
        }

        private void SetGridHeaders()
        {
            foreach (DataGridViewColumn col in SibiResultGrid.Columns)
            {
                col.HeaderText = ((DataTable)SibiResultGrid.DataSource).Columns[col.HeaderText].Caption;
            }
        }

        private void SetDisplayYears()
        {
            bolRebuildingCombo = true;
            using (DataTable results = DBFactory.GetDatabase().DataTableFromQueryString(Queries.SelectSibiDisplayYears))
            {
                List<string> Years = new List<string>();
                Years.Add("All");
                foreach (DataRow r in results.Rows)
                {
                    var yr = DataConsistency.YearFromDate(DateTime.Parse(r[SibiRequestCols.DateStamp].ToString()));
                    if (!Years.Contains(yr))
                    {
                        Years.Add(yr);
                    }
                }
                cmbDisplayYear.DataSource = Years;
                cmbDisplayYear.SelectedIndex = 0;
                bolRebuildingCombo = false;
            }
        }

        private void ShowAll(string Year = "")
        {
            if (string.IsNullOrEmpty(Year))
                Year = cmbDisplayYear.Text;
            if (Year == "All")
            {
                DbCommand newCommand;
                newCommand = DBFactory.GetDatabase().GetCommand(Queries.SelectSibiRequestsTable);
                ExecuteCmd(ref newCommand);
            }
            else
            {
                DbCommand newCommand;
                newCommand = DBFactory.GetDatabase().GetCommand(Queries.SelectSibiRequestsByYear(Year));
                ExecuteCmd(ref newCommand);
            }
        }

        private void cmdManage_Click(object sender, EventArgs e)
        {
            try
            {
                OtherFunctions.SetWaitCursor(true, this);
                SibiManageRequestForm NewRequest = new SibiManageRequestForm(this);
            }
            finally
            {
                OtherFunctions.SetWaitCursor(false, this);
            }
        }

        private void ResultGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (SibiResultGrid.CurrentRow.Index > -1)
                OpenRequest(SibiResultGrid.CurrentRowStringValue(SibiRequestCols.UID));
        }

        private void OpenRequest(string strUID)
        {
            try
            {
                OtherFunctions.SetWaitCursor(true, this);
                if (!Helpers.ChildFormControl.FormIsOpenByUID(typeof(SibiManageRequestForm), strUID))
                {
                    SibiManageRequestForm NewRequest = new SibiManageRequestForm(this, strUID);
                }
            }
            finally
            {
                OtherFunctions.SetWaitCursor(false, this);
            }
        }

        private void ResultGrid_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                DataGridViewCell dvgCell = SibiResultGrid.Rows[e.RowIndex].Cells[SibiRequestCols.Status];
                DataGridViewRow dvgRow = SibiResultGrid.Rows[e.RowIndex];
                Color BackCol = default(Color);
                Color ForeCol = default(Color);
                BackCol = GetRowColorFromID(dvgRow.Cells[SibiRequestCols.RequestNumber].Value.ToString());
                ForeCol = Color.Black;
                dvgCell.Style.BackColor = BackCol;
                dvgCell.Style.ForeColor = ForeCol;
                dvgCell.Style.SelectionBackColor = Colors.ColorAlphaBlend(BackCol, Color.FromArgb(87, 87, 87));
            }
        }

        private Color GetRowColorFromID(string ReqID)
        {
            foreach (StatusColumnColor status in StatusColors)
            {
                if (status.StatusID == ReqID)
                    return status.StatusColor;
            }
            return Color.Red;
        }

        /// <summary>
        /// Gets the color associated with the specified attribute code. Alpha blended with gray to make it more pastel.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private Color GetRowColor(string code)
        {
            //gray color
            Color DarkColor = Color.FromArgb(222, 222, 222);
            // Get a list from the attrib array.
            var attribList = GlobalInstances.SibiAttribute.StatusType.OfType<AttributeDataStruct>().ToList();
            // Use List.Find to locate the matching attribute.
            var attribColor = attribList.Find((i) => { return i.Code == code; }).Color;
            // Return the a blended color.
            return Colors.ColorAlphaBlend(attribColor, DarkColor);
        }

        private void HighlightCurrentRow(int Row)
        {
            try
            {
                if (!bolGridFilling)
                {
                    StyleFunctions.HighlightRow(SibiResultGrid, GridTheme, Row);
                }
            }
            catch
            {
            }
        }

        private void ResultGrid_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            HighlightCurrentRow(e.RowIndex);
        }

        private void ResultGrid_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            StyleFunctions.LeaveRow(SibiResultGrid, e.RowIndex);
        }

        private void cmbDisplayYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbDisplayYear.Text != null & !bolRebuildingCombo)
            {
                ShowAll(cmbDisplayYear.Text);
            }
        }

        public override bool OKToClose()
        {
            bool CanClose = true;
            if (!Helpers.ChildFormControl.OKToCloseChildren(this))
                CanClose = false;
            return CanClose;
        }

        private void frmSibiMain_Closing(object sender, CancelEventArgs e)
        {
            if (!OKToClose())
            {
                e.Cancel = true;
            }
        }

        private void txtPO_TextChanged(object sender, EventArgs e)
        {
            DynamicSearch();
        }

        private void txtReq_TextChanged(object sender, EventArgs e)
        {
            DynamicSearch();
        }

        private void txtDescription_TextChanged(object sender, EventArgs e)
        {
            DynamicSearch();
        }

        private void txtRTNum_TextChanged(object sender, EventArgs e)
        {
            DynamicSearch();
        }

        private void SibiMainForm_Disposed(object sender, EventArgs e)
        {
            if (LastCmd != null)
                LastCmd.Dispose();
            MyWindowList.Dispose();
            Helpers.ChildFormControl.CloseChildren(this);
        }

        private void ItemSearchButton_Click(object sender, EventArgs e)
        {
            ItemSearch(ItemSearchTextBox.Text);
        }

        private void ItemSearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ItemSearch(ItemSearchTextBox.Text);
            }
        }
    }
}