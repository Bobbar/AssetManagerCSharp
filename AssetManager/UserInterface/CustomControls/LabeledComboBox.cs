using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssetManager.UserInterface.CustomControls
{
    public partial class LabeledComboBox : UserControl
    {
        public string LabelText
        {
            get
            {
                return label.Text;
            }
            set
            {
                label.Text = value;
            }
        }

        public ComboBox.ObjectCollection Items
        {
            get
            {
                return comboBox.Items;
            }

        }

        public new string Text
        {
            get
            {
                return comboBox.Text;
            }
            set
            {
                comboBox.Text = value;
            }
        }

        public event EventHandler DropDown
        {
            add { comboBox.DropDown += value; }
            remove { comboBox.DropDown -= value; }
        }

        public int SelectedIndex
        {
            get
            {
                return comboBox.SelectedIndex;
            }
            set
            {
                comboBox.SelectedIndex = value;
            }
        }

        public new Graphics CreateGraphics()
        {
            return comboBox.CreateGraphics();
        }

        public new Color BackColor
        {
            get
            {
                return comboBox.BackColor;
            }
            set
            {
                comboBox.BackColor = value;
            }
        }

        public new bool Enabled
        {
            get
            {
                return comboBox.Enabled;
            }
            set
            {
                comboBox.Enabled = value;
            }
        }

        public ComboBox ComboBox
        {
            get
            {
                return comboBox;
            }
        }

        public LabeledComboBox()
        {
            InitializeComponent();
            this.Height = label.Height + comboBox.Height + label.Padding.Top + label.Padding.Bottom;
        }


    }
}
