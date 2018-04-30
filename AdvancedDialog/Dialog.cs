using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace AdvancedDialog
{
    public partial class Dialog : Form
    {
        private bool isMessageBox = false;
        private List<Control> customControls = new List<Control>();
        private bool startFullSize = false;

        public Dialog(Form parentForm, bool maximized = false)
        {
            InitializeComponent();
            startFullSize = maximized;

            if (parentForm != null)
            {
                this.Icon = parentForm.Icon;
            }
            else
            {
                this.Icon = Properties.Resources.inventory_icon_orange;
            }
        }

        public void AddButton(string name, string text, Action clickAction)
        {
            var button = new Button();
            button.Name = name;
            button.Text = text;
            button.Tag = clickAction;
            AddControl(button);
        }

        public void AddCheckBox(string name, string label)
        {
            var check = new CheckBox();
            check.Name = name;
            check.Tag = label;
            AddControl(check);
        }

        public void AddCustomControl(string name, string label, Control control)
        {
            control.Tag = label;
            AddControl(control);
        }

        public void AddLabel(string text, bool bold = false)
        {
            var label = new Label();
            label.Text = text;

            if (bold)
            {
                label.Font = new Font(label.Font, FontStyle.Bold);
            }
            AddControl(label);
        }

        public void AddRichTextBox(string name, string label, string text = null)
        {
            var rtb = new RichTextBox();
            rtb.Name = name;
            rtb.Tag = label;
            if (!string.IsNullOrEmpty(text)) rtb.Text = text;
            AddControl(rtb);
        }

        public void AddTextBox(string name, string label, string text = null)
        {
            var textbox = new TextBox();
            textbox.Name = name;
            textbox.Tag = label;
            if (!string.IsNullOrEmpty(text)) textbox.Text = text;
            AddControl(textbox);
        }

        private void AddControl(Control control)
        {
            customControls.Add(control);
        }

        private FlowLayoutPanel ControlPanel()
        {
            var panel = new FlowLayoutPanel();
            panel.AutoSize = true;
            panel.WrapContents = false;
            panel.FlowDirection = FlowDirection.TopDown;
            panel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panel.Padding = new Padding(0, 0, 10, 10);
            return panel;
        }

        public DialogResult DialogMessage(string prompt, MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.Information, string title = null, Form parentForm = null)
        {
            try
            {
                isMessageBox = true;

                if (title == null)
                {
                    this.Text = "Dialog Prompt";
                }
                else
                {
                    this.Text = title;
                }

                AddRichTextBox("PromptText", "Prompt", prompt);

                this.Size = this.MinimumSize;

                SetButtonsAndIcons(buttons, icon);

                if (parentForm != null)
                {
                    return this.ShowDialog(parentForm);
                }
                else
                {
                    return this.ShowDialog(ActiveForm);
                }
            }
            finally
            {
                this.Dispose();
            }
        }

        private void SetButtonsAndIcons(MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            switch (buttons)
            {
                case MessageBoxButtons.OK:
                    tblOkCancel.Visible = true;
                    Cancel_Button.Visible = false;
                    break;

                case MessageBoxButtons.OKCancel:
                    tblOkCancel.Visible = true;
                    break;

                case MessageBoxButtons.YesNo:
                    tblOkCancel.Visible = false;
                    tblYesNo.Visible = true;
                    break;

                default:
                    SetButtonsAndIcons(MessageBoxButtons.OK, icon);
                    break;
            }

            switch (icon)
            {
                case MessageBoxIcon.Stop:
                    pbIcon.Image = Properties.Resources.CriticalErrorIcon;
                    break;

                case MessageBoxIcon.Question:
                    pbIcon.Image = Properties.Resources.QuestionIcon;
                    break;

                case MessageBoxIcon.Exclamation:
                    pbIcon.Image = Properties.Resources.ExclamationIcon;
                    break;

                case MessageBoxIcon.Information:
                    pbIcon.Image = Properties.Resources.InformationIcon;
                    break;
            }
        }

        public object GetControlValue(string name)
        {
            foreach (var control in customControls)
            {
                if (control.Name == name)
                {
                    if (control is ComboBox)
                    {
                        var combo = (ComboBox)control;
                        return combo.SelectedIndex;
                    }
                    else if (control is TextBox || control is RichTextBox)
                    {
                        return control.Text;
                    }
                    else if (control is CheckBox)
                    {
                        var check = (CheckBox)control;
                        return check.Checked;
                    }
                }
            }
            throw new Exception("A control with that name was not found.");
        }

        public void SetControlValue(string name, object value)
        {
            foreach (var control in customControls)
            {
                if (control.Name == name)
                {
                    if (control is ComboBox)
                    {
                        var combo = (ComboBox)control;
                        combo.SelectedIndex = (int)value;
                        return;
                    }
                    else if (control is TextBox || control is RichTextBox)
                    {
                        control.Text = (string)value;
                        return;
                    }
                    else if (control is CheckBox)
                    {
                        var check = (CheckBox)control;
                        check.Checked = (bool)value;
                        return;
                    }
                }
            }
            throw new Exception("A control with that name was not found.");
        }

        private Label NewControlLabel(string text)
        {
            var label = new Label();
            label.AutoSize = true;
            label.Padding = new Padding(0, 10, 5, 0);
            label.Text = text;
            return label;
        }

        private void ButtonClick(object sender, EventArgs e)
        {
            var clickedButton = (Button)sender;
            var clickAction = (Action)clickedButton.Tag;
            clickAction();
        }

        private void LoadControls()
        {
            pnlControls.SuspendLayout();
            foreach (var control in customControls)
            {
                if (control is ComboBox)
                {
                    var combo = (ComboBox)control;
                    var panel = ControlPanel();
                    combo.Size = combo.PreferredSize;
                    combo.Padding = new Padding(5, 5, 5, 10);
                    panel.Controls.Add(NewControlLabel(combo.Tag.ToString()));
                    panel.Controls.Add(combo);
                    pnlControls.Controls.Add(panel);
                }
                else if (control is TextBox)
                {
                    var textBox = (TextBox)control;
                    var panel = ControlPanel();

                    if (textBox.Name.Contains("pass"))
                    {
                        textBox.UseSystemPasswordChar = true;
                    }
                    panel.Controls.Add(NewControlLabel(textBox.Tag.ToString()));
                    textBox.Width = 150;
                    panel.Controls.Add(textBox);
                    pnlControls.Controls.Add(panel);
                }
                else if (control is CheckBox)
                {
                    var checkBox = (CheckBox)control;
                    checkBox.AutoSize = true;
                    checkBox.Text = checkBox.Tag.ToString();
                    pnlControls.Controls.Add(checkBox);
                }
                else if (control is Label)
                {
                    var label = (Label)control;
                    label.AutoSize = true;
                    label.Padding = new Padding(5, 5, 5, 10);
                    pnlControls.Controls.Add(label);
                }
                else if (control is RichTextBox)
                {
                    var rtb = (RichTextBox)control;
                    var panel = ControlPanel();

                    if (isMessageBox)
                    {
                        pnlControls.Visible = false;
                        rtb.ReadOnly = true;
                        rtb.Margin = new Padding(5, 10, 5, 0);
                        rtb.BackColor = pnlControls_Main.BackColor;
                        rtb.Dock = DockStyle.Fill;
                        rtb.TabStop = false;
                        rtb.LinkClicked += ClickedLink;
                        pnlControls_Main.Controls.Add(rtb);
                    }
                    else
                    {
                        rtb.Width = 150;
                        rtb.Height = 80;
                        if (rtb.Tag != null) panel.Controls.Add(NewControlLabel(rtb.Tag.ToString()));
                        panel.Controls.Add(rtb);
                        pnlControls.Controls.Add(panel);
                    }
                }
                else if (control is Button)
                {
                    var button = (Button)control;
                    button.AutoSize = true;
                    button.Click += ButtonClick;
                    pnlControls.Controls.Add(button);
                }
                else
                {
                    var panel = ControlPanel();
                    panel.Controls.Add(NewControlLabel(control.Tag.ToString()));
                    panel.Controls.Add(control);
                    pnlControls.Controls.Add(panel);
                }
            }
            pnlControls.ResumeLayout();
        }

        private void DisposeControls()
        {
            customControls.ForEach((c) => { c.Dispose(); });
            customControls.Clear();
        }

        private void MaximizeForm()
        {
            pnlControls_Main.AutoSize = false;
            pnlMaster.AutoSize = false;
            this.AutoSize = false;
            this.Size = this.MaximumSize;
        }

        private void ClickedLink(object sender, LinkClickedEventArgs e)
        {
            Process.Start(new Uri(e.LinkText).AbsolutePath);
        }

        private void Dialog_Load(object sender, EventArgs e)
        {
            LoadControls();

            if (!isMessageBox)
            {
                pnlIcon.Visible = false;
                pnlControls_Main.Width = pnlMaster.Width;
                pnlControls_Main.Height = pnlMaster.Height - pnlButtons.Height - 10;
            }

            if (startFullSize) MaximizeForm();
            pnlControls_Main.Refresh();
            pnlMaster.Refresh();
            this.Update();
        }

        private void Dialog_ResizeBegin(object sender, EventArgs e)
        {
            pnlControls_Main.AutoSize = false;
            pnlMaster.AutoSize = false;
            this.AutoSize = false;
        }

        private void Cancel_Button_Click(object sender, System.EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Dispose();
        }

        private void No_Button_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }

        private void OK_Button_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void Yes_Button_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && components != null)
                    components.Dispose();

                DisposeControls();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}