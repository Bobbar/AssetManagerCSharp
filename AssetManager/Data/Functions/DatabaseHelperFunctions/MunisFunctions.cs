using AssetManager.Data.Classes;
using AssetManager.Data.Communications;
using AssetManager.Helpers;
using AssetManager.UserInterface.CustomControls;
using AssetManager.UserInterface.Forms;
using AssetManager.UserInterface.Forms.AssetManagement;
using AdvancedDialog;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssetManager.Data.Functions
{
    public static class MunisFunctions //Be warned. This whole class is a horrible bastard...
    {
        private const int intMaxResults = 100;
        private static MunisComms munisComms = new MunisComms();

        public static string GetReqNumberFromPO(string po)
        {
            if (!ReferenceEquals(po, null))
            {
                if (!string.IsNullOrEmpty(po))
                {
                    return munisComms.ReturnSqlValue("Requisitions", "PurchaseOrderNumber", po, "RequisitionNumber").ToString();
                }
            }
            return null;
        }

        private static async Task<string> GetReqNumberFromPOAsync(string po)
        {
            if (!string.IsNullOrEmpty(po))
            {
                return await munisComms.ReturnSqlValueAsync("Requisitions", "PurchaseOrderNumber", po, "RequisitionNumber");
            }
            return string.Empty;
        }

        public static async Task<string> GetPOFromReqNumberAsync(string reqNum, string fy)
        {
            if (!string.IsNullOrEmpty(reqNum))
            {
                return await munisComms.ReturnSqlValueAsync("rqdetail", "rqdt_req_no", reqNum, "rqdt_pur_no", "rqdt_fsc_yr", fy);
            }
            return string.Empty;
        }

        public static async Task<string> GetPOFromDevice(Device device)
        {
            string POFromAsset = "";
            string POFromSerial = "";
            string POFromAssetFromPurchaseHist = "";

            if (!string.IsNullOrEmpty(device.AssetTag))
            {
                POFromAsset = await munisComms.ReturnSqlValueAsync("famaster", "fama_tag", device.AssetTag, "fama_purch_memo");
                POFromAssetFromPurchaseHist = await munisComms.ReturnSqlValueAsync("fapurchh", "faph_asset", device.AssetTag, "faph_po_num");
            }

            if (!string.IsNullOrEmpty(device.Serial))
            {
                POFromSerial = await munisComms.ReturnSqlValueAsync("famaster", "fama_serial", device.Serial, "fama_purch_memo");
            }

            POFromAsset = POFromAsset.Trim();
            POFromSerial = POFromSerial.Trim();

            if (!string.IsNullOrEmpty(POFromAsset))
            {
                return POFromAsset;
            }
            else if (!string.IsNullOrEmpty(POFromSerial))
            {
                return POFromSerial;
            }
            else if (!string.IsNullOrEmpty(POFromAssetFromPurchaseHist))
            {
                return POFromAssetFromPurchaseHist;
            }

            return string.Empty;
        }

        public static async Task<string> GetSerialFromAsset(string assetTag)
        {
            var value = await munisComms.ReturnSqlValueAsync("famaster", "fama_tag", assetTag, "fama_serial");
            if (value != null)
            {
                return value.ToString().Trim();
            }
            return string.Empty;
        }

        public static async Task<string> GetAssetFromSerial(string serial)
        {
            var value = await munisComms.ReturnSqlValueAsync("famaster", "fama_serial", serial, "fama_tag");
            if (value != null)
            {
                return value.ToString().Trim();
            }
            return string.Empty;
        }

        public static string GetFYFromAsset(string assetTag)
        {
            return munisComms.ReturnSqlValue("famaster", "fama_tag", assetTag, "fama_fisc_yr").ToString().Trim();
        }

        public static DateTime GetPODate(string po)
        {
            try
            {
                return DateTime.Parse(munisComms.ReturnSqlValue("RequisitionItems", "PurchaseOrderNumber", po, "PurchaseOrderDate").ToString().Trim());
            }
            catch (Exception)
            {
                return DateTime.Now;
            }
        }

        public static string GetVendorNameFromPO(string po)
        {
            var vendorNumber = munisComms.ReturnSqlValue("rqdetail", "rqdt_req_no", GetReqNumberFromPO(po), "rqdt_sug_vn", "rqdt_fsc_yr", GetFYFromPO(po));
            return munisComms.ReturnSqlValue("ap_vendor", "a_vendor_number", vendorNumber, "a_vendor_name").ToString();
        }

        public static async Task<string> GetVendorNumberFromReqNumber(string reqNum, string fy)
        {
            var vendorNumber = await munisComms.ReturnSqlValueAsync("rqdetail", "rqdt_req_no", reqNum, "rqdt_sug_vn", "rqdt_fsc_yr", fy);
            if (vendorNumber != null)
            {
                return vendorNumber.ToString();
            }
            return string.Empty;
        }

        public static string GetFYFromPO(string po)
        {
            string twoDigitYear = po.Substring(0, 2);
            return "20" + twoDigitYear;
        }

        public static async Task<string> GetPOStatusFromPO(int po)
        {
            string statusString = "";
            string statusCode = await munisComms.ReturnSqlValueAsync("poheader", "pohd_pur_no", po, "pohd_sta_cd");
            if (!string.IsNullOrEmpty(statusCode))
            {
                int parseCode = -1;
                if (!int.TryParse(statusCode, out parseCode))
                {
                    return string.Empty;
                }
                statusString = statusCode.ToString() + " - " + POStatusTextFromCode(parseCode);
                return statusString;
            }
            return string.Empty;
        }

        public static async Task<string> GetReqStatusFromReqNum(string reqNum, int fy)
        {
            string statusString = "";
            string statusCode = await munisComms.ReturnSqlValueAsync("rqheader", "rqhd_req_no", reqNum, "rqhd_sta_cd", "rqhd_fsc_yr", fy);
            if (!string.IsNullOrEmpty(statusCode))
            {
                int parseCode = -1;
                if (!int.TryParse(statusCode, out parseCode))
                {
                    return string.Empty;
                }
                statusString = statusCode.ToString() + " - " + ReqStatusTextFromCode(parseCode);
                return statusString;
            }
            return string.Empty;
        }

        private static string POStatusTextFromCode(int code)
        {
            switch (code)
            {
                case 0:
                    return "Closed";

                case 1:
                    return "Rejected";

                case 2:
                    return "Created";

                case 4:
                    return "Allocated";

                case 5:
                    return "Released";

                case 6:
                    return "Posted";

                case 8:
                    return "Printed";

                case 9:
                    return "Carry Forward";

                case 10:
                    return "Canceled";

                case 11:
                    return "Closed";
            }
            return string.Empty;
        }

        private static string ReqStatusTextFromCode(int code)
        {
            switch (code)
            {
                case 0:
                    return "Converted";

                case 1:
                    return "Rejected";

                case 2:
                    return "Created";

                case 4:
                    return "Allocated";

                case 6:
                    return "Released";

                default:
                    return "NA";
            }
        }

        public static void AssetSearch(ExtendedForm parentForm)
        {
            try
            {
                Device device = new Device();
                using (var newDialog = new Dialog(parentForm))
                {
                    newDialog.Text = "Asset Search";
                    newDialog.AddTextBox("AssetNumber", "Asset:");
                    newDialog.AddTextBox("SerialNumber", "Serial:");
                    newDialog.ShowDialog();
                    if (newDialog.DialogResult == DialogResult.OK)
                    {
                        device.AssetTag = newDialog.GetControlValue("AssetNumber").ToString().Trim();
                        device.Serial = newDialog.GetControlValue("SerialNumber").ToString().Trim();
                        LoadMunisInfoByDevice(device, parentForm);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        public static void NameSearch(ExtendedForm parentForm)
        {
            try
            {
                using (var newDialog = new Dialog(parentForm))
                {
                    newDialog.Text = "Employee Search";
                    newDialog.AddTextBox("Name", "First or Last Name:");
                    newDialog.ShowDialog();
                    if (newDialog.DialogResult == DialogResult.OK)
                    {
                        var strName = newDialog.GetControlValue("Name").ToString();
                        if (strName.Trim() != "")
                        {
                            NewMunisEmployeeSearch(strName.Trim(), parentForm);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        public static void POSearch(ExtendedForm parentForm)
        {
            try
            {
                string po = "";
                using (var newDialog = new Dialog(parentForm))
                {
                    newDialog.Text = "PO Search";
                    newDialog.AddTextBox("PO", "PO #:");
                    newDialog.ShowDialog();
                    if (newDialog.DialogResult == DialogResult.OK)
                    {
                        po = newDialog.GetControlValue("PO").ToString();
                        NewMunisPOSearch(po, parentForm);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        public static async void ReqSearch(ExtendedForm parentForm)
        {
            try
            {
                string reqNumber = "";
                string fy = "";
                using (var newDialog = new Dialog(parentForm))
                {
                    newDialog.Text = "Req Search";
                    newDialog.AddTextBox("ReqNum", "Requisition #:");
                    newDialog.AddTextBox("FY", "FY:");
                    newDialog.ShowDialog();
                    if (newDialog.DialogResult == DialogResult.OK)
                    {
                        reqNumber = newDialog.GetControlValue("ReqNum").ToString();
                        fy = newDialog.GetControlValue("FY").ToString();
                        if (DataConsistency.IsValidYear(fy))
                        {
                            parentForm.Waiting();

                            var blah = await NewMunisReqSearch(reqNumber, fy, parentForm);
                        }
                        else
                        {
                            OtherFunctions.Message("Invalid year.", MessageBoxButtons.OK, MessageBoxIcon.Information, "Invalid", parentForm);
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
                parentForm.DoneWaiting();
            }
        }

        public static void OrgObjSearch(ExtendedForm parentForm)
        {
            try
            {
                using (var newDialog = new Dialog(parentForm))
                {
                    string org = "";
                    string obj = "";
                    string fy = "";
                    newDialog.Text = "Org/Object Code Search";
                    newDialog.AddTextBox("Org", "Org Code:");
                    newDialog.AddTextBox("Obj", "Object Code:");
                    newDialog.AddTextBox("FY", "Fiscal Year:");
                    newDialog.SetControlValue("FY", DateTime.Now.Year);
                    newDialog.ShowDialog();
                    if (newDialog.DialogResult == DialogResult.OK)
                    {
                        org = newDialog.GetControlValue("Org").ToString();
                        obj = newDialog.GetControlValue("Obj").ToString();
                        fy = newDialog.GetControlValue("FY").ToString();
                        if (org.Trim() != "" && DataConsistency.IsValidYear(fy))
                        {
                            NewOrgObjView(org, obj, fy, parentForm);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        public static DataTable ListOfEmpsBySup(string supEmpNum)
        {
            string query = "SELECT TOP 100 a_employee_number FROM pr_employee_master WHERE e_supervisor='" + supEmpNum + "'";
            return munisComms.ReturnSqlTable(query);
        }

        public static async void NewOrgObjView(string org, string obj, string fy, ExtendedForm parentForm)
        {
            try
            {
                parentForm.Waiting();
                GridForm newGridForm = new GridForm(parentForm, "Org/Obj Info");
                string glColumns = " glma_org, glma_obj, glma_desc, glma_seg5, glma_bud_yr, glma_orig_bud_cy, glma_rev_bud_cy, glma_encumb_cy, glma_memo_bal_cy, glma_rev_bud_cy-glma_encumb_cy-glma_memo_bal_cy AS 'Funds Available' ";
                string glMasterQry = "Select TOP " + intMaxResults + " " + glColumns + "FROM glmaster";

                QueryParamCollection glParams = new QueryParamCollection();
                glParams.Add("glma_org", org, true);

                if (!string.IsNullOrEmpty(obj)) //Show Rollup info for Object
                {
                    glParams.Add("glma_obj", obj, true);

                    string rollUpCode = await munisComms.ReturnSqlValueAsync("gl_budget_rollup", "a_org", org, "a_rollup_code");
                    string rollUpByCodeQry = "SELECT TOP " + intMaxResults + " * FROM gl_budget_rollup WHERE a_rollup_code = '" + rollUpCode + "'";
                    string budgetQry = "SELECT TOP " + intMaxResults + " a_projection_no,a_org,a_object,db_line,db_bud_desc_line1,db_bud_reason_desc,db_bud_req_qty5,db_bud_unit_cost,db_bud_req_amt5,a_account_id FROM gl_budget_detail_2"; // WHERE a_projection_no='" & FY & "' AND a_org='" & Org & "' AND a_object='" & Obj & "'"

                    QueryParamCollection budgetParams = new QueryParamCollection();
                    budgetParams.Add("a_projection_no", fy, true);
                    budgetParams.Add("a_org", org, true);
                    budgetParams.Add("a_object", obj, true);

                    newGridForm.AddGrid("OrgGrid", "GL Info:", await munisComms.ReturnSqlTableFromCmdAsync(munisComms.GetSqlCommandFromParams(glMasterQry, glParams.Parameters)));
                    newGridForm.AddGrid("RollupGrid", "Rollup Info:", await munisComms.ReturnSqlTableAsync(rollUpByCodeQry));
                    newGridForm.AddGrid("BudgetGrid", "Budget Info:", await munisComms.ReturnSqlTableFromCmdAsync(munisComms.GetSqlCommandFromParams(budgetQry, budgetParams.Parameters)));
                }
                else // Show Rollup info for all Objects in Org
                {
                    string rollUpAllQry = "SELECT TOP " + intMaxResults + " * FROM gl_budget_rollup";

                    QueryParamCollection rollUpParams = new QueryParamCollection();
                    rollUpParams.Add("a_org", org, true);

                    newGridForm.AddGrid("OrgGrid", "GL Info:", await munisComms.ReturnSqlTableFromCmdAsync(munisComms.GetSqlCommandFromParams(glMasterQry, glParams.Parameters))); //MunisComms.Return_MSSQLTableAsync(Qry))
                    newGridForm.AddGrid("RollupGrid", "Rollup Info:", await munisComms.ReturnSqlTableFromCmdAsync(munisComms.GetSqlCommandFromParams(rollUpAllQry, rollUpParams.Parameters))); //MunisComms.Return_MSSQLTableAsync("SELECT TOP " & intMaxResults & " * FROM gl_budget_rollup WHERE a_org = '" & Org & "'"))
                }
                newGridForm.Show();
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
            finally
            {
                parentForm.DoneWaiting();
            }
        }

        private static async void NewMunisEmployeeSearch(string name, ExtendedForm parentForm)
        {
            try
            {
                parentForm.Waiting();

                string columns = "e.a_employee_number,e.a_name_last,e.a_name_first,e.a_org_primary,e.a_object_primary,e.a_location_primary,e.a_location_p_desc,e.a_location_p_short,e.e_work_location,m.a_employee_number as sup_employee_number,m.a_name_first as sup_name_first,m.a_name_last as sup_name_last";
                string query = "SELECT TOP " + intMaxResults + " " + columns + @"
FROM pr_employee_master e
INNER JOIN pr_employee_master m on e.e_supervisor = m.a_employee_number";

                QueryParamCollection searchParams = new QueryParamCollection();
                searchParams.Add("e.a_name_last", name.ToUpper(), "OR");
                searchParams.Add("e.a_name_first", name.ToUpper(), "OR");

                using (var cmd = munisComms.GetSqlCommandFromParams(query, searchParams.Parameters))
                using (var results = await munisComms.ReturnSqlTableFromCmdAsync(cmd))
                {
                    if (HasResults(results, parentForm))
                    {
                        parentForm.DoneWaiting();

                        GridForm newGridForm = new GridForm(parentForm, "MUNIS Employee Info");
                        newGridForm.AddGrid("EmpGrid", "MUNIS Info:", results);
                        newGridForm.Show();
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
            finally
            {
                parentForm.DoneWaiting();
            }
        }

        public static async void NewMunisPOSearch(string po, ExtendedForm parentForm)
        {
            try
            {
                parentForm.Waiting();
              
                if (po == "")
                {
                    return;
                }
                string query = "SELECT TOP " + intMaxResults + @" pohd_pur_no, pohd_fsc_yr, pohd_req_no, pohd_gen_cm, pohd_buy_id, pohd_pre_dt, pohd_exp_dt, pohd_sta_cd, pohd_vnd_cd, pohd_dep_cd, pohd_shp_cd, pohd_tot_amt, pohd_serial
FROM poheader";

                QueryParamCollection searchParams = new QueryParamCollection();
                searchParams.Add("pohd_pur_no", po, true);

                GridForm newGridForm = new GridForm(parentForm, "MUNIS PO Info");
                using (var cmd = munisComms.GetSqlCommandFromParams(query, searchParams.Parameters))
                {
                    using (var results = await munisComms.ReturnSqlTableFromCmdAsync(cmd))
                    {
                        if (HasResults(results, parentForm))
                        {
                            newGridForm.AddGrid("POGrid", "PO Info:", results);
                            newGridForm.Show();
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
                parentForm.DoneWaiting();
            }
        }

        public static async Task<string> NewMunisReqSearch(string reqNumber, string fy, ExtendedForm parentForm, bool selectMode = false)
        {
            if (reqNumber == "" || fy == "")
            {
                return string.Empty;
            }
            GridForm newGridForm = new GridForm(parentForm, "MUNIS Requisition Info");
            using (var ReqLineItemsTable = await GetReqLineItemsFromReqNum(reqNumber, fy))
            {
                if (HasResults(ReqLineItemsTable, parentForm))
                {
                    if (!selectMode)
                    {
                        using (var ReqHeaderTable = await GetReqHeaderFromReqNum(reqNumber, fy))
                        {
                            newGridForm.AddGrid("ReqHeaderGrid", "Requisition Header:", ReqHeaderTable);
                        }

                        newGridForm.AddGrid("ReqLineGrid", "Requisition Line Items:", ReqLineItemsTable);
                        newGridForm.Show();
                        return string.Empty;
                    }
                    else
                    {
                        newGridForm.AddGrid("ReqLineGrid", "Requisition Line Items:", DoubleClickAction.SelectValue, ReqLineItemsTable);
                        newGridForm.ShowDialog(parentForm);
                        if (newGridForm.DialogResult == DialogResult.OK)
                        {
                            return newGridForm.SelectedRow.Cells["rqdt_uni_pr"].Value.ToString().Trim();
                        }
                    }
                }
            }

            return string.Empty;
        }

        private static bool HasResults(DataTable results, Form parentForm)
        {
            if (results != null && results.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                OtherFunctions.Message("No results found.", MessageBoxButtons.OK, MessageBoxIcon.Information, "No results", parentForm);
                return false;
            }
        }

        private static async Task<DataTable> GetReqHeaderFromReqNum(string reqNumber, string fiscalYr)
        {
            if (reqNumber == "" || fiscalYr == "")
            {
                return null;
            }
            string query = "SELECT TOP " + intMaxResults + " * FROM rqheader";

            QueryParamCollection searchParams = new QueryParamCollection();
            searchParams.Add("rqhd_req_no", reqNumber, true);
            searchParams.Add("rqhd_fsc_yr", fiscalYr, true);

            DataTable results = await munisComms.ReturnSqlTableFromCmdAsync(munisComms.GetSqlCommandFromParams(query, searchParams.Parameters));

            if (results.Rows.Count > 0)
            {
                return results;
            }
            return null;
        }

        private static async Task<DataTable> GetReqLineItemsFromReqNum(string reqNumber, string fiscalYr)
        {
            if (reqNumber == "" || fiscalYr == "")
            {
                return null;
            }
            var vendorNum = await GetVendorNumberFromReqNumber(reqNumber, fiscalYr);
            if ((string)vendorNum == "")
            {
                return null;
            }
            string nendorName = await munisComms.ReturnSqlValueAsync("ap_vendor", "a_vendor_number", vendorNum, "a_vendor_name");
            string query = "SELECT TOP " + intMaxResults + @" dbo.rq_gl_info.rg_fiscal_year, dbo.rq_gl_info.a_requisition_no, dbo.rq_gl_info.rg_org, dbo.rq_gl_info.rg_object, dbo.rq_gl_info.a_org_description, dbo.rq_gl_info.a_object_desc,
'" + nendorName + "' AS a_vendor_name, '" + vendorNum + @"' AS a_vendor_number, dbo.rqdetail.rqdt_pur_no, dbo.rqdetail.rqdt_pur_dt, dbo.rqdetail.rqdt_lin_no, dbo.rqdetail.rqdt_uni_pr, dbo.rqdetail.rqdt_net_pr, dbo.rqdetail.rqdt_qty_no, dbo.rqdetail.rqdt_des_ln, dbo.rqdetail.rqdt_vdr_part_no
FROM dbo.rq_gl_info INNER JOIN
dbo.rqdetail ON dbo.rq_gl_info.rg_line_number = dbo.rqdetail.rqdt_lin_no AND dbo.rq_gl_info.a_requisition_no = dbo.rqdetail.rqdt_req_no AND dbo.rq_gl_info.rg_fiscal_year = dbo.rqdetail.rqdt_fsc_yr";

            QueryParamCollection searchParams = new QueryParamCollection();
            searchParams.Add("dbo.rq_gl_info.a_requisition_no", reqNumber, true);
            searchParams.Add("dbo.rq_gl_info.rg_fiscal_year", fiscalYr, true);

            var reqTable = await munisComms.ReturnSqlTableFromCmdAsync(munisComms.GetSqlCommandFromParams(query, searchParams.Parameters));
            if (reqTable.Rows.Count > 0)
            {
                return reqTable;
            }
            else
            {
                return null;
            }
        }

        public static async void LoadMunisInfoByDevice(Device device, ExtendedForm parentForm)
        {
            try
            {
                parentForm.Waiting();

                DataTable reqLinesTable = new DataTable();
                DataTable reqHeaderTable = new DataTable();
                DataTable inventoryTable = new DataTable();

                if (device.PO == "" || device.PO == null)
                {
                    device.PO = await GetPOFromDevice(device);
                }

                if (device.PO != string.Empty)
                {
                    inventoryTable = await LoadMunisInventoryGrid(device);
                    reqLinesTable = await GetReqLineItemsFromReqNum(await GetReqNumberFromPOAsync(device.PO), GetFYFromPO(device.PO));
                    reqHeaderTable = await GetReqHeaderFromReqNum(await GetReqNumberFromPOAsync(device.PO), GetFYFromPO(device.PO));
                }
                else
                {
                    inventoryTable = await LoadMunisInventoryGrid(device);
                    reqLinesTable = null;
                    reqHeaderTable = null;
                }
                if (inventoryTable != null || reqLinesTable != null)
                {
                    GridForm newGridForm = new GridForm(parentForm, "MUNIS Info");
                    if (ReferenceEquals(inventoryTable, null))
                    {
                        OtherFunctions.Message("Munis Fixed Asset info. not found.", MessageBoxButtons.OK, MessageBoxIcon.Information, "No FA Record");
                    }
                    else
                    {
                        newGridForm.AddGrid("InvGrid", "FA Info:", inventoryTable);
                    }
                    if (ReferenceEquals(reqLinesTable, null))
                    {
                        OtherFunctions.Message("Could not resolve PO from Asset Tag or Serial. Please add a valid PO if possible.", MessageBoxButtons.OK, MessageBoxIcon.Information, "No Req. Record");
                    }
                    else
                    {
                        newGridForm.AddGrid("ReqHeadGrid", "Requisition Header:", reqHeaderTable);
                        newGridForm.AddGrid("ReqLineGrid", "Requisition Line Items:", reqLinesTable);
                    }

                    if (newGridForm.GridCount > 0)
                    {
                        newGridForm.Show();
                    }
                    else
                    {
                        newGridForm.Dispose();
                    }
                }
                else if (ReferenceEquals(inventoryTable, null) && ReferenceEquals(reqLinesTable, null))
                {
                    OtherFunctions.Message("Could not resolve any purchase or Fixed Asset info.", MessageBoxButtons.OK, MessageBoxIcon.Information, "Nothing Found");
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
            finally
            {
                parentForm.DoneWaiting();
            }
        }

        private static async Task<DataTable> LoadMunisInventoryGrid(Device device)
        {
            string columns = "fama_asset,fama_status,fama_class,fama_subcl,fama_tag,fama_serial,fama_desc,fama_dept,fama_loc,FixedAssetLocations.LongDescription,fama_acq_dt,fama_fisc_yr,fama_pur_cost,fama_manuf,fama_model,fama_est_life,fama_repl_dt,fama_purch_memo";
            string query = "SELECT TOP 1 " + columns + " FROM famaster INNER JOIN FixedAssetLocations ON FixedAssetLocations.Code = famaster.fama_loc WHERE fama_tag='" + device.AssetTag + "' AND fama_tag <> '' OR fama_serial='" + device.Serial + "' AND fama_serial <> ''";
            DataTable results = await munisComms.ReturnSqlTableAsync(query);

            if (results.Rows.Count > 0)
            {
                return results;
            }
            return null;
        }

        public static MunisEmployee MunisUserSearch(ExtendedForm parentForm)
        {
            using (MunisUserForm newMunisSearch = new MunisUserForm(parentForm))
            {
                if (newMunisSearch.DialogResult == DialogResult.OK)
                {
                    return newMunisSearch.EmployeeInfo;
                }
                else
                {
                    return new MunisEmployee();
                }
            }
        }
    }
}