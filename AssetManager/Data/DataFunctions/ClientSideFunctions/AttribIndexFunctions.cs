using AssetManager.Data.Classes;
using AssetManager.Data.Communications;
using AssetManager.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;

namespace AssetManager.Data.Functions
{
    internal static class AttributeFunctions
    {
        private static Dictionary<string, string> departmentCodes;

        public static void PopulateAttributeIndexes()
        {
            var BuildIdxs = Task.Run(() =>
            {
                Attributes.DeviceAttributes.Locations = BuildIndex(DevicesBaseCols.AttribTable, DeviceAttribType.Location);
                Attributes.DeviceAttributes.ChangeType = BuildIndex(DevicesBaseCols.AttribTable, DeviceAttribType.ChangeType);
                Attributes.DeviceAttributes.EquipType = BuildIndex(DevicesBaseCols.AttribTable, DeviceAttribType.EquipType);
                Attributes.DeviceAttributes.OSType = BuildIndex(DevicesBaseCols.AttribTable, DeviceAttribType.OSType);
                Attributes.DeviceAttributes.StatusType = BuildIndex(DevicesBaseCols.AttribTable, DeviceAttribType.StatusType);
                Attributes.SibiAttributes.StatusType = BuildIndex(SibiRequestCols.AttribTable, SibiAttribType.SibiStatusType);
                Attributes.SibiAttributes.ItemStatusType = BuildIndex(SibiRequestCols.AttribTable, SibiAttribType.SibiItemStatusType);
                Attributes.SibiAttributes.RequestType = BuildIndex(SibiRequestCols.AttribTable, SibiAttribType.SibiRequestType);
                PopulateDepartments();
            });
            BuildIdxs.Wait();
        }

        private static void PopulateDepartments()
        {
            departmentCodes = new Dictionary<string, string>();
            var selectDpmtQuery = "SELECT * FROM munis_departments";
            using (DataTable results = DBFactory.GetDatabase().DataTableFromQueryString(selectDpmtQuery))
            {
                foreach (DataRow row in results.Rows)
                {
                    departmentCodes.Add(row["asset_location_code"].ToString(), row["munis_department_code"].ToString());
                }
            }
        }

        public static string DepartmentOf(string location)
        {
            if (departmentCodes.ContainsKey(location))
            {
                return departmentCodes[location];
            }
            return string.Empty;
        }

        private static DbAttributes BuildIndex(string codeType, string typeName)
        {
            try
            {
                using (DataTable results = DBFactory.GetDatabase().DataTableFromQueryString(Queries.SelectAttributeCodes(codeType, typeName)))
                {
                    var tmpAttrib = new DbAttributes();
                    foreach (DataRow r in results.Rows)
                    {
                        string DisplayValue = "";
                        if (r.Table.Columns.Contains("munis_code"))
                        {
                            if (r["munis_code"] != DBNull.Value)
                            {
                                DisplayValue = r[ComboCodesBaseCols.DisplayValue].ToString() + " - " + r["munis_code"].ToString();
                            }
                            else
                            {
                                DisplayValue = r[ComboCodesBaseCols.DisplayValue].ToString();
                            }
                        }
                        else
                        {
                            DisplayValue = r[ComboCodesBaseCols.DisplayValue].ToString();
                        }

                        Color attribColor = Color.Empty;
                        if (!string.IsNullOrEmpty(r[ComboCodesBaseCols.Color].ToString()))
                        {
                            attribColor = ColorTranslator.FromHtml(r[ComboCodesBaseCols.Color].ToString());
                        }

                        tmpAttrib.Add(DisplayValue, r[ComboCodesBaseCols.CodeValue].ToString(), Convert.ToInt32(r[ComboCodesBaseCols.Id]), attribColor);
                    }
                    return tmpAttrib;
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
                return null;
            }
        }
    }
}