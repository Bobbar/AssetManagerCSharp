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
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssetManager.UserInterface.Forms.Sibi
{
    public partial class SibiMainForm : ExtendedForm
    {
        #region Fields

        private bool gridFilling = false;
        private DbCommand lastCmd;
        private bool rebuildingCombo = false;
        private Dictionary<string, Color> statusColumnColors;
        private WindowList windowList;

        #endregion Fields

        #region Constructors

        public SibiMainForm(ExtendedForm parentForm) : base(parentForm, false)
        {
            windowList = new WindowList(this);
            // This call is required by the designer.
            InitializeComponent();

            InitForm();
        }

        #endregion Constructors

        #region Methods

        public override void RefreshData()
        {
            ExecuteCmd(ref lastCmd);
        }

        private QueryParamCollection BuildSearchListNew()
        {
            var searchParams = new QueryParamCollection();
            searchParams.Add(SibiRequestCols.RTNumber, RTNumberTextBox.Text.Trim(), false);
            searchParams.Add(SibiRequestCols.Description, DescriptionTextBox.Text.Trim(), false);
            searchParams.Add(SibiRequestCols.PO, POTextBox.Text, false);
            searchParams.Add(SibiRequestCols.RequisitionNumber, ReqNumberTextBox.Text, false);

            //Filter out unpopulated fields.
            QueryParamCollection popSearchParams = new QueryParamCollection();
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
                    ComboBox cmb = (ComboBox)ctl;
                    cmb.SelectedIndex = 0;
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
                        string Value = "%" + fld.Value.ToString() + "%";
                        cmd.AddParameterWithValue("@" + fld.FieldName, Value);
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
                    ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
                }
            }
        }

        /// <summary>
        /// Gets the color associated with the specified attribute code. Alpha blended with gray to make it more pastel.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private Color GetRowColor(string code)
        {
            // Gray color.
            Color blendColor = Color.FromArgb(222, 222, 222);

            // Get a list from the attrib array.
           // var attribList = Attributes.SibiAttributes.StatusType.OfType<DBCode>().ToList();

            // Use List.Find to locate the matching attribute.
            var attribColor = Attributes.SibiAttributes.StatusType[code].Color;
            //var attribColor = attribList.Find((a) => { return a.Code == code; }).Color;

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
                this.Show();
                this.Activate();
                Application.DoEvents();
                ShowAll("All");
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
        private void OpenRequest(string strGuid)
        {
            try
            {
                OtherFunctions.SetWaitCursor(true, this);
                if (!Helpers.ChildFormControl.FormIsOpenByGuid(typeof(SibiManageRequestForm), strGuid))
                {
                    new SibiManageRequestForm(this, strGuid);
                }
            }
            finally
            {
                OtherFunctions.SetWaitCursor(false, this);
            }
        }

        private void ResetView()
        {
            try
            {
                OtherFunctions.SetWaitCursor(true, this);
                searchSlider.Clear();
                ClearAll(this);
                SetDisplayYears();
                ShowAll();
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
            finally
            {
                OtherFunctions.SetWaitCursor(false, this);
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
                    gridFilling = false;
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
                    var yr = DateTime.Parse(row[SibiRequestCols.DateStamp].ToString()).Year.ToString();
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

        private void ShowAll(string Year = "")
        {
            if (string.IsNullOrEmpty(Year))
                Year = DisplayYearComboBox.Text;
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
            columnList.Add(new GridColumnAttrib(SibiRequestCols.RequisitionNumber, "Req. Number"));
            columnList.Add(new GridColumnAttrib(SibiRequestCols.RTNumber, "RT Number"));
            columnList.Add(new GridColumnAttrib(SibiRequestCols.DateStamp, "Create Date"));
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
                OtherFunctions.SetWaitCursor(true, this);
                new SibiManageRequestForm(this);
            }
            finally
            {
                OtherFunctions.SetWaitCursor(false, this);
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

        private void ResultGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (SibiResultGrid.CurrentRow.Index > -1) OpenRequest(SibiResultGrid.CurrentRowStringValue(SibiRequestCols.Guid));
        }

        private void ResultGrid_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (!gridFilling)
            {
                StyleFunctions.HighlightRow(SibiResultGrid, GridTheme, e.RowIndex);
            }
        }

        private void ResultGrid_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            StyleFunctions.LeaveRow(SibiResultGrid, e.RowIndex);
            SetStatusCellColors();
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