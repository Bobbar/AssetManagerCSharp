using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Linq;
using System.Drawing;
namespace AssetManager
{
    static class AttribIndexFunctions
    {

        public static void FillComboBox(AttributeDataStruct[] IndexType, ComboBox combo)
        {
            combo.Items.Clear();
            combo.Text = "";
            AddAutoSizeDropWidthHandler(combo);
            foreach (AttributeDataStruct ComboItem in IndexType)
            {
                combo.Items.Add(ComboItem.DisplayValue);
            }
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
                foreach (string s in senderComboBox.Items)
                {
                    newWidth = Convert.ToInt32(gfx.MeasureString(s, senderComboBox.Font).Width) + vertScrollBarWidth;
                    if (correctWidth < newWidth)
                    {
                        correctWidth = newWidth;
                    }
                }
            }
            senderComboBox.DropDownWidth = correctWidth;
        }

        public static void FillToolComboBox(AttributeDataStruct[] IndexType, ref ToolStripComboBox cmb)
        {
            cmb.Items.Clear();
            cmb.Text = "";
            int i = 0;
            foreach (AttributeDataStruct ComboItem in IndexType)
            {
                cmb.Items.Insert(i, ComboItem.DisplayValue);
                i += 1;
            }
        }

        public static string GetDBValue(AttributeDataStruct[] codeIndex, int index)
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

        public static string GetDisplayValueFromCode(AttributeDataStruct[] codeIndex, string code)
        {
            foreach (AttributeDataStruct item in codeIndex)
            {
                if (item.Code == code)
                    return item.DisplayValue;
            }
            return string.Empty;
        }

        public static string GetDisplayValueFromIndex(AttributeDataStruct[] codeIndex, int index)
        {
            if (index > -1)
            {
                return codeIndex[index].DisplayValue;
            }
            return string.Empty;
        }

        public static int GetComboIndexFromCode(AttributeDataStruct[] codeIndex, string code)
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
            });
            BuildIdxs.Wait();
        }

        public static AttributeDataStruct[] BuildIndex(string codeType, string typeName)
        {
            try
            {
                using (DataTable results = DBFactory.GetDatabase().DataTableFromQueryString(Queries.SelectAttributeCodes(codeType, typeName)))
                {
                    List<AttributeDataStruct> tmpArray = new List<AttributeDataStruct>();
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

                        tmpArray.Add(new AttributeDataStruct(DisplayValue, r[ComboCodesBaseCols.CodeValue].ToString(), Convert.ToInt32(r[ComboCodesBaseCols.ID]), attribColor));
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
