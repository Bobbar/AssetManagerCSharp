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
        public static Dictionary<string, string> DepartmentCodes;

        public static string GetDisplayValueFromCode(DBCode[] codeIndex, string code)
        {
            foreach (DBCode item in codeIndex)
            {
                if (item.Code == code)
                    return item.DisplayValue;
            }
            return string.Empty;
        }

        public static int GetComboIndexFromCode(DBCode[] codeIndex, string code)
        {
            for (int i = 0; i <= codeIndex.Length - 1; i++)
            {
                if (codeIndex[i].Code == code)
                    return i;
            }
            return -1;
        }

        public static void PopulateAttributeIndexes()
        {
            var BuildIdxs = Task.Run(() =>
            {
                Attributes.DeviceAttribute.Locations = BuildIndex(DevicesBaseCols.AttribTable, DeviceAttribType.Location);
                Attributes.DeviceAttribute.ChangeType = BuildIndex(DevicesBaseCols.AttribTable, DeviceAttribType.ChangeType);
                Attributes.DeviceAttribute.EquipType = BuildIndex(DevicesBaseCols.AttribTable, DeviceAttribType.EquipType);
                Attributes.DeviceAttribute.OSType = BuildIndex(DevicesBaseCols.AttribTable, DeviceAttribType.OSType);
                Attributes.DeviceAttribute.StatusType = BuildIndex(DevicesBaseCols.AttribTable, DeviceAttribType.StatusType);
                Attributes.SibiAttribute.StatusType = BuildIndex(SibiRequestCols.AttribTable, SibiAttribType.SibiStatusType);
                Attributes.SibiAttribute.ItemStatusType = BuildIndex(SibiRequestCols.AttribTable, SibiAttribType.SibiItemStatusType);
                Attributes.SibiAttribute.RequestType = BuildIndex(SibiRequestCols.AttribTable, SibiAttribType.SibiRequestType);
                PopulateDepartments();
            });
            BuildIdxs.Wait();
        }

        private static void PopulateDepartments()
        {
            DepartmentCodes = new Dictionary<string, string>();
            var selectDpmtQuery = "SELECT * FROM munis_departments";
            using (DataTable results = DBFactory.GetDatabase().DataTableFromQueryString(selectDpmtQuery))
            {
                foreach (DataRow row in results.Rows)
                {
                    DepartmentCodes.Add(row["asset_location_code"].ToString(), row["munis_department_code"].ToString());
                }
            }
        }

        public static string DepartmentOf(string location)
        {
            if (DepartmentCodes.ContainsKey(location))
            {
                return DepartmentCodes[location];
            }
            return string.Empty;
        }

        private static DBCode[] BuildIndex(string codeType, string typeName)
        {
            try
            {
                using (DataTable results = DBFactory.GetDatabase().DataTableFromQueryString(Queries.SelectAttributeCodes(codeType, typeName)))
                {
                    List<DBCode> tmpArray = new List<DBCode>();
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

                        tmpArray.Add(new DBCode(DisplayValue, r[ComboCodesBaseCols.CodeValue].ToString(), Convert.ToInt32(r[ComboCodesBaseCols.Id]), attribColor));
                    }
                    return tmpArray.ToArray();
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