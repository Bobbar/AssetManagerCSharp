using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Linq;
using System.Drawing;
using AssetManager.Helpers;
using AssetManager.Data.Communications;
using AssetManager.Data.Classes;
namespace AssetManager.Data.Functions
{
    static class AttributeFunctions
    {

        public static void FillComboBox(CodeAttribute[] IndexType, ComboBox combo)
        {
            combo.SuspendLayout();
            combo.BeginUpdate();
            combo.DataSource = null;
            combo.Text = "";
            AddAutoSizeDropWidthHandler(combo);
            combo.DisplayMember = nameof(CodeAttribute.DisplayValue);
            combo.ValueMember = nameof(CodeAttribute.Code);
            combo.DataSource = IndexType;
            combo.SelectedIndex = -1;
            combo.EndUpdate();
            combo.ResumeLayout();
        }

        public static void AddAutoSizeDropWidthHandler(ComboBox combo)
        {
            combo.DropDown -= AdjustComboBoxWidth;
            combo.DropDown += AdjustComboBoxWidth;
        }

        public static void AdjustComboBoxWidth(object sender, EventArgs e)
        {
            var senderComboBox = (ComboBox)sender;
            int correctWidth = senderComboBox.DropDownWidth;
            int newWidth = 0;
            using (Graphics gfx = senderComboBox.CreateGraphics())
            {
                int vertScrollBarWidth = 0;
                if (senderComboBox.Items.Count > senderComboBox.MaxDropDownItems)
                {
                    vertScrollBarWidth = SystemInformation.VerticalScrollBarWidth;
                }
                else
                {
                    vertScrollBarWidth = 0;
                }
                foreach (var s in senderComboBox.Items)
                {
                    newWidth = Convert.ToInt32(gfx.MeasureString(s.ToString(), senderComboBox.Font).Width) + vertScrollBarWidth;
                    if (correctWidth < newWidth)
                    {
                        correctWidth = newWidth;
                    }
                }
            }
            senderComboBox.DropDownWidth = correctWidth;
        }

        public static void FillToolComboBox(CodeAttribute[] attribType, ref ToolStripComboBox cmb)
        {
            cmb.Items.Clear();
            cmb.Text = "";
            int i = 0;
            foreach (CodeAttribute ComboItem in attribType)
            {
                cmb.Items.Insert(i, ComboItem.DisplayValue);
                i += 1;
            }
        }

        public static string GetDBValue(CodeAttribute[] codeIndex, int index)
        {
            try
            {
                if (index > -1)
                {
                    return codeIndex[index].Code;
                }
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string GetDisplayValueFromCode(CodeAttribute[] codeIndex, string code)
        {
            foreach (CodeAttribute item in codeIndex)
            {
                if (item.Code == code)
                    return item.DisplayValue;
            }
            return string.Empty;
        }

        public static string GetDisplayValueFromIndex(CodeAttribute[] codeIndex, int index)
        {
            if (index > -1)
            {
                return codeIndex[index].DisplayValue;
            }
            return string.Empty;
        }

        public static int GetComboIndexFromCode(CodeAttribute[] codeIndex, string code)
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
                GlobalInstances.DeviceAttribute.Locations = BuildIndex(DevicesBaseCols.AttribTable, DeviceAttribType.Location);
                GlobalInstances.DeviceAttribute.ChangeType = BuildIndex(DevicesBaseCols.AttribTable, DeviceAttribType.ChangeType);
                GlobalInstances.DeviceAttribute.EquipType = BuildIndex(DevicesBaseCols.AttribTable, DeviceAttribType.EquipType);
                GlobalInstances.DeviceAttribute.OSType = BuildIndex(DevicesBaseCols.AttribTable, DeviceAttribType.OSType);
                GlobalInstances.DeviceAttribute.StatusType = BuildIndex(DevicesBaseCols.AttribTable, DeviceAttribType.StatusType);
                GlobalInstances.SibiAttribute.StatusType = BuildIndex(SibiRequestCols.AttribTable, SibiAttribType.SibiStatusType);
                GlobalInstances.SibiAttribute.ItemStatusType = BuildIndex(SibiRequestCols.AttribTable, SibiAttribType.SibiItemStatusType);
                GlobalInstances.SibiAttribute.RequestType = BuildIndex(SibiRequestCols.AttribTable, SibiAttribType.SibiRequestType);
                PopulateDepartments();
            });
            BuildIdxs.Wait();
        }

        private static void PopulateDepartments()
        {
            GlobalInstances.DepartmentCodes = new Dictionary<string, string>();
            var selectDpmtQuery = "SELECT * FROM munis_departments";
            using (DataTable results = DBFactory.GetDatabase().DataTableFromQueryString(selectDpmtQuery))
            {
                foreach (DataRow row in results.Rows)
                {
                    GlobalInstances.DepartmentCodes.Add(row["asset_location_code"].ToString(), row["munis_department_code"].ToString());
                }
            }
        }

        public static string DepartmentOf(string location)
        {
            if (GlobalInstances.DepartmentCodes.ContainsKey(location))
            {
                return GlobalInstances.DepartmentCodes[location];
            }
            return string.Empty;
        }

        private static CodeAttribute[] BuildIndex(string codeType, string typeName)
        {
            try
            {
                using (DataTable results = DBFactory.GetDatabase().DataTableFromQueryString(Queries.SelectAttributeCodes(codeType, typeName)))
                {
                    List<CodeAttribute> tmpArray = new List<CodeAttribute>();
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

                        tmpArray.Add(new CodeAttribute(DisplayValue, r[ComboCodesBaseCols.CodeValue].ToString(), Convert.ToInt32(r[ComboCodesBaseCols.ID]), attribColor));
                    }
                    return tmpArray.ToArray();
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
                return null;
            }
        }

    }
}
