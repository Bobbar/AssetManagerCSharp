using AssetManager.Data.Classes;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace AssetManager.Helpers
{
    public static class ComboBoxExtensions
    {
        public static void FillComboBox(this ComboBox combo, CodeAttribute[] IndexType)
        {
            combo.SuspendLayout();
            combo.BeginUpdate();
            combo.DataSource = null;
            combo.Text = "";
            AddAutoSizeDropWidthHandler(combo);
            combo.DisplayMember = nameof(CodeAttribute.DisplayValue);
            combo.ValueMember = nameof(CodeAttribute.Code);
            combo.BindingContext = new BindingContext();
            combo.DataSource = IndexType;
            combo.SelectedIndex = -1;
            combo.EndUpdate();
            combo.ResumeLayout();
        }

        private static void AddAutoSizeDropWidthHandler(ComboBox combo)
        {
            combo.DropDown -= AdjustComboBoxWidth;
            combo.DropDown += AdjustComboBoxWidth;
        }

        private static void AdjustComboBoxWidth(object sender, EventArgs e)
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
    }
}