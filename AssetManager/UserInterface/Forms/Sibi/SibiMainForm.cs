using AssetManager.Data;
using AssetManager.Data.Classes;
using AssetManager.Data.Communications;
using AssetManager.Data.Functions;
using AssetManager.Helpers;
using AssetManager.UserInterface.CustomControls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssetManager.UserInterface.Forms.Sibi
{
    public partial class SibiMainForm : ExtendedForm
    {
        #region Fields

        private Dictionary<string, Color> statusColumnColors;
        private Dictionary<int, string> poStatuses = new Dictionary<int, string>();
        private Dictionary<string, string> reqStatuses = new Dictionary<string, string>();
        private bool rebuildingCombo = false;
        private bool munisStatusRefreshing = false;
        private bool gridFilling = false;
        private DbCommand lastCmd;
        private WindowList windowList;

        #endregion Fields

        #region Constructors

        public SibiMainForm(ExtendedForm parentForm) : base(parentForm)
        {
            InitializeComponent();
            windowList = new WindowList(this);
            InitForm();
        }

        #endregion Constructors

        #region Methods

        public override void RefreshData()
        {
            ExecuteCmd(ref lastCmd);
        }

        /// <summary>
        /// Populates the PO and Reqisition status dictionaries which are used in the <see cref="SetMunisStatuses"/> method.
        /// </summary>
        /// <remarks>
        /// This is done to avoid querying the database too frequently. We only update the values from the database at more critical moments.
        /// </remarks>
        private async void GetMunisStatuses()
        {
            try
            {
                // Only allow one instance to run at a time.
                if (munisStatusRefreshing)
                    return;

                munisStatusRefreshing = true;

                poStatuses.Clear();
                reqStatuses.Clear();

                var dataSource = (DataTable)SibiResultGrid.DataSource;

                if (dataSource == null)
                    return;

                foreach (DataRow row in dataSource.Rows)
                {
                    if (row[SibiRequestCols.PO] != null)
                    {
                        var poVal = row[SibiRequestCols.PO].ToString();
                        int poInt = 0;

                        if (int.TryParse(poVal, out poInt))
                        {
                            var poStatus = await MunisFunctions.GetPOStatusFromPO(poInt);
                            if (!poStatuses.ContainsKey(poInt))
                                poStatuses.Add(poInt, poStatus);
                        }
                    }

                    if (row[SibiRequestCols.RequisitionNumber] != null)
                    {
                        var reqVal = row[SibiRequestCols.RequisitionNumber].ToString();
                        var fiscalYear = ((DateTime)row[SibiRequestCols.NeedBy]).Year;
                        int reqInt = 0;

                        if (int.TryParse(reqVal, out reqInt))
                        {
                            var reqStatus = await MunisFunctions.GetReqStatusFromReqNum(reqVal, fiscalYear);
                            var reqKey = reqInt.ToString() + fiscalYear.ToString();
                            if (!reqStatuses.ContainsKey(reqKey))
                                reqStatuses.Add(reqKey, reqStatus);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
            finally
            {
                munisStatusRefreshing = false;
            }
        }

        /// <summary>
        /// Populates the data grid PO and Requisition status from the dictionary caches.
        /// </summary>
        private async void SetMunisStatuses()
        {
            // Delay to wait for data binding to finish.
            // Normally we would use the DataBindingComplete event,
            // but that event is firing multiple times and the
            // grid still never seems to be ready afterwards.
            // Waiting works better.
            await Task.Delay(200);

            // If the refresh method is still running, wait until it finishes or this form is closed.
            if (munisStatusRefreshing)
            {
                while (munisStatusRefreshing && !this.IsDisposed)
                {
                    await Task.Delay(200);
                }

                if (this.IsDisposed) return;
            }

            try
            {
                foreach (DataGridViewRow row in SibiResultGrid.Rows)
                {
                    if (row.Cells[SibiRequestCols.PO].Value != null)
                    {
                        var poVal = row.Cells[SibiRequestCols.PO].Value.ToString();
                        int poInt = 0;
                        if (int.TryParse(poVal, out poInt))
                        {
                            if (poStatuses.ContainsKey(poInt))
                                row.Cells["postatus"].Value = poStatuses[poInt];
                        }
                    }

                    if (row.Cells[SibiRequestCols.RequisitionNumber].Value != null)
                    {
                        var reqVal = row.Cells[SibiRequestCols.RequisitionNumber].Value.ToString();
                        var fiscalYear = ((DateTime)row.Cells[SibiRequestCols.NeedBy].Value).Year;
                        int reqInt = 0;

                        if (int.TryParse(reqVal, out reqInt))
                        {
                            var reqKey = reqInt.ToString() + fiscalYear.ToString();
                            if (reqStatuses.ContainsKey(reqKey))
                                row.Cells["reqstatus"].Value = reqStatuses[reqKey];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Since this is a quiet background process, we want to fail silently.
                // But log any errors for posterity.
                Logging.Logger("ERROR: SetMunisStatus: " + ex.ToString());
            }
        }

        private QueryParamCollection BuildSearchListNew()
        {
            var searchParams = new QueryParamCollection();
            searchParams.Add(SibiRequestCols.RTNumber, RTNumberTextBox.Text.Trim(), false);
            searchParams.Add(SibiRequestCols.Description, DescriptionTextBox.Text.Trim(), false);
            searchParams.Add(SibiRequestCols.PO, POTextBox.Text, false);
            searchParams.Add(SibiRequestCols.RequisitionNumber, ReqNumberTextBox.Text, false);

            //Filter out unpopulated fields.
            var popSearchParams = new QueryParamCollection();
            foreach (var param in searchParams.Parameters)
            {
                if (param.Value.ToString() != "")
                {
                    popSearchParams.Add(param);
                }
            }
            return popSearchParams;
        }

        private void ClearAll(Control parentControl)
        {
            foreach (Control ctl in parentControl.Controls)
            {
                if (ctl is TextBox)
                {
                    TextBox txt = (TextBox)ctl;
                    txt.Clear();
                }
                else if (ctl is ComboBox)
                {
                    //ComboBox cmb = (ComboBox)ctl;
                    //cmb.SelectedIndex = 0;
                }
                else if (ctl.Controls.Count > 0)
                {
                    ClearAll(ctl);
                }
            }
        }

        // Dynamically creates query using any combination of search filters.
        private void DynamicSearch()
        {
            var cmd = DBFactory.GetDatabase().GetCommand();
            var partialQuery = "SELECT * FROM " + SibiRequestCols.TableName + " WHERE";
            var dynaQuery = "";
            QueryParamCollection searchValCols = BuildSearchListNew();

            foreach (var fld in searchValCols.Parameters)
            {
                if ((fld.Value != null))
                {
                    if (!string.IsNullOrEmpty(fld.Value.ToString()))
                    {
                        dynaQuery += " " + fld.FieldName + " LIKE @" + fld.FieldName;

                        string value = "%" + fld.Value.ToString() + "%";
                        cmd.AddParameterWithValue("@" + fld.FieldName, value);

                        if (searchValCols.Parameters.IndexOf(fld) != searchValCols.Parameters.Count - 1)
                        {
                            dynaQuery += " AND";
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(dynaQuery))
            {
                return;
            }

            var completeQuery = partialQuery + dynaQuery;
            completeQuery += " ORDER BY " + SibiRequestCols.RequestNumber + " DESC";
            cmd.CommandText = completeQuery;

            ExecuteCmd(ref cmd);
        }

        private void ExecuteCmd(ref DbCommand cmd)
        {
            try
            {
                lastCmd = cmd;
                SendToGrid(DBFactory.GetDatabase().DataTableFromCommand(cmd));
                cmd.Dispose();

                //  SetMunisStatuses();
            }
            catch (Exception ex)
            {
                // InvalidCastException is expected when the last comand was populated while in cached DB mode and now cached mode is currently false.
                // Make a fresh call to create a new command instance.
                if (ex is InvalidCastException)
                {
                    //GetMunisStatuses();
                    ShowAll();
                }
                else
                {
                    ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
                }
            }
        }

        /// <summary>
        /// Gets the color associated with the specified attribute code. Alpha blended with gray to make it more pastel.
        /// </summary>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        private Color GetRowColor(string statusCode)
        {
            // Gray color.
            var blendColor = Color.FromArgb(222, 222, 222);
            var attribColor = Attributes.SibiAttributes.StatusType[statusCode].Color;

            // Return the a blended color.
            return StyleFunctions.ColorAlphaBlend(attribColor, blendColor);
        }

        // Since the data grid is not setup with a value/display member we need another way to
        // record the color values for the status row. This method populates a dictionary with
        // the unique request ID as a key and the corresponding status color as the value.
        // This is then referenced later to set the status cells to the correct color.
        private void GetStatusColors(DataTable results)
        {
            statusColumnColors = new Dictionary<string, Color>();

            foreach (DataRow row in results.Rows)
            {
                var requestNum = row[SibiRequestCols.RequestNumber].ToString();
                var status = row[SibiRequestCols.Status].ToString();

                if (!statusColumnColors.ContainsKey(requestNum))
                {
                    statusColumnColors.Add(requestNum, GetRowColor(status));
                }
            }
        }

        private void InitForm()
        {
            try
            {
                this.Icon = Properties.Resources.sibi_icon;
                SibiResultGrid.DoubleBuffered(true);
                this.GridTheme = new GridTheme(Colors.HighlightBlue, Colors.SibiSelectColor, Colors.SibiSelectAltColor, SibiResultGrid.DefaultCellStyle.BackColor);
                StyleFunctions.SetGridStyle(SibiResultGrid, this.GridTheme);
                ToolStrip1.BackColor = Colors.SibiToolBarColor;
                windowList.InsertWindowList(ToolStrip1);
                SetDisplayYears();
                ShowAll("All");
                GetMunisStatuses();
                this.Show();
                this.Activate();
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
                this.Dispose();
            }
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
            var AdvSearch = new AdvancedSearch();

            // Perform search on Request Items table
            using (var results = AdvSearch.GetSingleTableResults(searchString.Trim(), SibiRequestItemsCols.TableName))
            {
                // Make sure we have results.
                if (results.Rows.Count > 0)
                {
                    //Clear slider label of any previous errors.
                    searchSlider.Clear();

                    // Iterate through results and use Request Items Request Guid column to query for the full request data.
                    // Task.Run lambda to keep UI alive.
                    DataTable resultsTable = await Task.Run(() =>
                    {
                        DataTable rtables = new DataTable();
                        foreach (DataRow row in results.Rows)
                        {
                            DataTable requestTable = DBFactory.GetDatabase().DataTableFromQueryString(Queries.SelectSibiRequestsByGuid(row[SibiRequestItemsCols.RequestGuid].ToString()));
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
                    searchSlider.QueueMessage("No matches found!", Color.Red, SlideDirection.Up, SlideDirection.Down, 0);
                }
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1806")]
        private void OpenRequest(string requestGuid)
        {
            try
            {
                Waiting();
                if (!Helpers.ChildFormControl.FormIsOpenByGuid(typeof(SibiManageRequestForm), requestGuid))
                {
                    new SibiManageRequestForm(this, requestGuid);
                }
            }
            finally
            {
                DoneWaiting();
            }
        }

        private void OpenSelectedRequest()
        {
            if (SibiResultGrid.CurrentRow.Index > -1) OpenRequest(SibiResultGrid.CurrentRowStringValue(SibiRequestCols.Guid));
        }

        private void ResetView()
        {
            try
            {
                Waiting();
                searchSlider.Clear();
                ClearAll(this);
                SetDisplayYears();
                ShowAll();
                GetMunisStatuses();
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
            finally
            {
                DoneWaiting();
            }
        }

        private void SendToGrid(DataTable results)
        {
            try
            {
                using (results)
                {
                    gridFilling = true;
                    SibiResultGrid.SuspendLayout();
                    SibiResultGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
                    SibiResultGrid.ColumnHeadersHeight = 38;
                    GetStatusColors(results);
                    SibiResultGrid.Populate(results, SibiTableColumns());
                    SetStatusCellColors();
                    SibiResultGrid.FastAutoSizeColumns();
                    SibiResultGrid.ClearSelection();
                    SibiResultGrid.ResumeLayout();
                    if (this.Visible) gridFilling = false;
                    SetMunisStatuses();
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        private void SetDisplayYears()
        {
            rebuildingCombo = true;
            using (var results = DBFactory.GetDatabase().DataTableFromQueryString(Queries.SelectSibiDisplayYears))
            {
                var years = new List<string>();

                years.Add("All");

                foreach (DataRow row in results.Rows)
                {
                    var yr = DateTime.Parse(row[SibiRequestCols.CreateDate].ToString()).Year.ToString();
                    if (!years.Contains(yr))
                    {
                        years.Add(yr);
                    }
                }

                DisplayYearComboBox.DataSource = years;
                DisplayYearComboBox.SelectedIndex = 0;
                rebuildingCombo = false;
            }
        }

        private void SetStatusCellColors()
        {
            foreach (DataGridViewRow row in SibiResultGrid.Rows)
            {
                var cell = row.Cells[SibiRequestCols.Status];
                var reqID = row.Cells[SibiRequestCols.RequestNumber].Value.ToString();
                var backColor = new Color();
                var foreColor = Color.Black;

                if (statusColumnColors.ContainsKey(reqID))
                {
                    backColor = statusColumnColors[reqID];
                }
                else
                {
                    backColor = row.InheritedStyle.BackColor;
                }

                cell.Style.BackColor = backColor;
                cell.Style.ForeColor = foreColor;
            }
        }

        private void ShowAll(string year = "")
        {
            if (string.IsNullOrEmpty(year)) year = DisplayYearComboBox.Text;

            string query = "";

            if (year == "All")
            {
                query = Queries.SelectSibiRequestsTable;
            }
            else
            {
                query = Queries.SelectSibiRequestsByYear(year);
            }

            var newCommand = DBFactory.GetDatabase().GetCommand(query);
            ExecuteCmd(ref newCommand);
        }

        private List<GridColumnAttrib> SibiTableColumns()
        {
            var columnList = new List<GridColumnAttrib>();
            columnList.Add(new GridColumnAttrib(SibiRequestCols.RequestNumber, "Request #"));
            columnList.Add(new GridColumnAttrib(SibiRequestCols.Status, "Status", Attributes.SibiAttributes.StatusType, ColumnFormatType.AttributeDisplayMemberOnly));
            columnList.Add(new GridColumnAttrib(SibiRequestCols.Description, "Description"));
            columnList.Add(new GridColumnAttrib(SibiRequestCols.RequestUser, "Request User"));
            columnList.Add(new GridColumnAttrib(SibiRequestCols.Type, "Request Type", Attributes.SibiAttributes.RequestType, ColumnFormatType.AttributeDisplayMemberOnly));
            columnList.Add(new GridColumnAttrib(SibiRequestCols.NeedBy, "Need By"));
            columnList.Add(new GridColumnAttrib(SibiRequestCols.PO, "PO Number"));
            columnList.Add(new GridColumnAttrib("postatus", "PO Status")); // Unbound status column.
            columnList.Add(new GridColumnAttrib(SibiRequestCols.RequisitionNumber, "Req. Number"));
            columnList.Add(new GridColumnAttrib("reqstatus", "Req. Status")); // Unbound status column.
            columnList.Add(new GridColumnAttrib(SibiRequestCols.RTNumber, "RT Number"));
            columnList.Add(new GridColumnAttrib(SibiRequestCols.CreateDate, "Create Date"));
            columnList.Add(new GridColumnAttrib(SibiRequestCols.ModifyDate, "Modified"));
            columnList.Add(new GridColumnAttrib(SibiRequestCols.ModifyUser, "Modified By"));
            columnList.Add(new GridColumnAttrib(SibiRequestCols.Guid, "Guid"));
            return columnList;
        }

        #endregion Methods

        #region Control Events

        private void DisplayYearComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DisplayYearComboBox.Text != null & !rebuildingCombo)
            {
                ShowAll(DisplayYearComboBox.Text);
            }
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

        [SuppressMessage("Microsoft.Design", "CA1806")]
        private void NewRequestButton_Click(object sender, EventArgs e)
        {
            try
            {
                Waiting();
                new SibiManageRequestForm(this);
            }
            finally
            {
                DoneWaiting();
            }
        }

        private void DescriptionTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            DynamicSearch();
        }

        private void POTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            DynamicSearch();
        }

        private void ReqNumberTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            DynamicSearch();
        }

        private void RTNumberTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            DynamicSearch();
        }

        private void RefreshResetButton_Click(object sender, EventArgs e)
        {
            ResetView();
        }

        private void SibiResultGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            OpenSelectedRequest();
        }

        private void SibiResultGrid_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (!gridFilling && this.Visible)
            {
                StyleFunctions.HighlightRow(SibiResultGrid, GridTheme, e.RowIndex);
            }
        }

        private void SibiResultGrid_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            StyleFunctions.LeaveRow(SibiResultGrid, e.RowIndex);
            SetStatusCellColors();
        }

        private void SibiResultGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex >= 0 & e.RowIndex >= 0)
            {
                if (e.Button == MouseButtons.Right & !SibiResultGrid[e.ColumnIndex, e.RowIndex].Selected)
                {
                    SibiResultGrid.Rows[e.RowIndex].Selected = true;
                    SibiResultGrid.CurrentCell = SibiResultGrid[e.ColumnIndex, e.RowIndex];
                }
            }
        }

        private void SibiResultGrid_Sorted(object sender, EventArgs e)
        {
            SetStatusCellColors();
        }

        private void SibiMainForm_Shown(object sender, EventArgs e)
        {
            gridFilling = false;
            SetStatusCellColors();
        }

        private void ViewRequestMenuItem_Click(object sender, EventArgs e)
        {
            OpenSelectedRequest();
        }

        #endregion Control Events

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (components != null)
                    {
                        components.Dispose();
                    }

                    lastCmd?.Dispose();
                    windowList?.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}