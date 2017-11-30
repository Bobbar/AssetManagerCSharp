using AssetManager.UserInterface.CustomControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace AssetManager.UserInterface.Forms.Sibi
{
    public partial class SibiMainForm : ExtendedForm
    {
        private bool bolGridFilling = false;
        private WindowList MyWindowList;
        private DbCommand LastCmd;
        private bool bolRebuildingCombo = false;

        private List<StatusColumnColorStruct> StatusColors;

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
            DatabaseHelperFunctions.AdvancedSearch AdvSearch = new DatabaseHelperFunctions.AdvancedSearch();

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
                    StatusColors = GetStatusColors(results);
                    GridFunctions.PopulateGrid(SibiResultGrid, results, SibiTableColumns());
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

        private List<StatusColumnColorStruct> GetStatusColors(DataTable Results)
        {
            List<StatusColumnColorStruct> StatusList = new List<StatusColumnColorStruct>();
            foreach (DataRow row in Results.Rows)
            {
                StatusList.Add(new StatusColumnColorStruct(row[SibiRequestCols.RequestNumber].ToString(), GetRowColor(row[SibiRequestCols.Status].ToString())));
            }
            return StatusList;
        }

        private List<DataGridColumn> SibiTableColumns()
        {
            List<DataGridColumn> ColList = new List<DataGridColumn>();
            ColList.Add(new DataGridColumn(SibiRequestCols.RequestNumber, "Request #", typeof(int)));
            ColList.Add(new DataGridColumn(SibiRequestCols.Status, "Status", GlobalInstances.SibiAttribute.StatusType, ColumnFormatTypes.AttributeDisplayMemberOnly));
            ColList.Add(new DataGridColumn(SibiRequestCols.Description, "Description", typeof(string)));
            ColList.Add(new DataGridColumn(SibiRequestCols.RequestUser, "Request User", typeof(string)));
            ColList.Add(new DataGridColumn(SibiRequestCols.Type, "Request Type", GlobalInstances.SibiAttribute.RequestType, ColumnFormatTypes.AttributeDisplayMemberOnly));
            ColList.Add(new DataGridColumn(SibiRequestCols.NeedBy, "Need By", typeof(System.DateTime)));
            ColList.Add(new DataGridColumn(SibiRequestCols.PO, "PO Number", typeof(string)));
            ColList.Add(new DataGridColumn(SibiRequestCols.RequisitionNumber, "Req. Number", typeof(string)));
            ColList.Add(new DataGridColumn(SibiRequestCols.RTNumber, "RT Number", typeof(string)));
            ColList.Add(new DataGridColumn(SibiRequestCols.DateStamp, "Create Date", typeof(System.DateTime)));
            ColList.Add(new DataGridColumn(SibiRequestCols.UID, "UID", typeof(string)));
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
                OpenRequest(GridFunctions.GetCurrentCellValue(SibiResultGrid, SibiRequestCols.UID));
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
                ForeCol = GetFontColor(BackCol);
                dvgCell.Style.BackColor = BackCol;
                dvgCell.Style.ForeColor = ForeCol;
                dvgCell.Style.SelectionBackColor = Colors.ColorAlphaBlend(BackCol, Color.FromArgb(87, 87, 87));
            }
        }

        private Color GetRowColorFromID(string ReqID)
        {
            foreach (StatusColumnColorStruct status in StatusColors)
            {
                if (status.StatusID == ReqID)
                    return status.StatusColor;
            }
            return Color.Red;
        }

        private Color GetRowColor(string Value)
        {
            Color DarkColor = Color.FromArgb(222, 222, 222);
            //gray color
            switch (Value)
            {
                case "NEW":
                    return Colors.ColorAlphaBlend(Color.FromArgb(0, 255, 30), DarkColor);

                case "QTN":
                    return Colors.ColorAlphaBlend(Color.FromArgb(242, 255, 0), DarkColor);

                case "QTR":
                    return Colors.ColorAlphaBlend(Color.FromArgb(255, 208, 0), DarkColor);

                case "QRC":
                    return Colors.ColorAlphaBlend(Color.FromArgb(255, 162, 0), DarkColor);

                case "RQN":
                    return Colors.ColorAlphaBlend(Color.FromArgb(0, 255, 251), DarkColor);

                case "RQR":
                    return Colors.ColorAlphaBlend(Color.FromArgb(0, 140, 255), DarkColor);

                case "POS":
                    return Colors.ColorAlphaBlend(Color.FromArgb(197, 105, 255), DarkColor);

                case "SHD":
                    return Colors.ColorAlphaBlend(Color.FromArgb(255, 79, 243), DarkColor);

                case "ORC":
                    return Colors.ColorAlphaBlend(Color.FromArgb(79, 144, 255), DarkColor);

                case "NPAY":
                    return Colors.ColorAlphaBlend(Color.FromArgb(255, 36, 36), DarkColor);

                case "RCOMP":
                    return Colors.ColorAlphaBlend(Color.FromArgb(158, 158, 158), DarkColor);

                case "ONH":
                    return Colors.ColorAlphaBlend(Color.FromArgb(255, 255, 255), DarkColor);
            }
            return DarkColor;
        }

        private Color GetFontColor(Color color)
        {
            //get contrasting font color
            int d = 0;
            double a = 0;
            a = 1 - (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;
            if (a < 0.5)
            {
                d = 0;
            }
            else
            {
                d = 255;
            }
            return Color.FromArgb(d, d, d);
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