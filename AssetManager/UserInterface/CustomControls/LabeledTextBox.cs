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
    public partial class LabeledTextBox : UserControl
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

        [Browsable(true)]
        public override string Text
        {
            get
            {
                return textBox.Text;
            }
            set
            {
                textBox.Text = value;
            }
        }



        public LabeledTextBox()
        {
            InitializeComponent();
            this.Height = label.Height + textBox.Height + label.Padding.Top + label.Padding.Bottom;
        }
    }
}
