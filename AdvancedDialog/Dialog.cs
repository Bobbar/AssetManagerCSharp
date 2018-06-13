using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace AdvancedDialog
{
    public partial class Dialog : Form
    {
        #region Fields

        private List<Control> customControls = new List<Control>();
        private bool isMessageBox = false;
        private bool startFullSize = false;
        private Form parentForm = null;

        #endregion Fields

        #region Constructors

        public Dialog(Form parentForm, bool maximized = false)
        {
            InitializeComponent();

            startFullSize = maximized;

            if (parentForm != null)
            {
                this.parentForm = parentForm;
                this.Icon = parentForm.Icon;
            }
            else
            {
                this.Icon = Properties.Resources.inventory_icon_orange;
            }
        }

        #endregion Constructors

        #region Methods

        public Button AddButton(string name, string text, Action clickAction)
        {
            var button = new Button();
            button.Name = name;
            button.Text = text;
            button.Tag = clickAction;
            AddControl(button);
            return button;
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
                        control.Text = value.ToString();
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

        private void AddControl(Control control)
        {
            customControls.Add(control);
        }

        private void ButtonClick(object sender, EventArgs e)
        {
            var clickedButton = (Button)sender;
            var clickAction = (Action)clickedButton.Tag;
            clickAction();
        }

        private void ClickedLink(object sender, LinkClickedEventArgs e)
        {
            Process.Start(new Uri(e.LinkText).AbsolutePath);
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

        private void Dialog_Load(object sender, EventArgs e)
        {
            // Suspend layout.
            SetLayout(this, true);

            // Create and add controls.
            LoadControls();

            // Set the window size to max size if specified.
            if (startFullSize) MaximizeForm();

            // Resume layout.
            SetLayout(this, false);

            // Disable autosize on the controls panel.
            ControlsMainPanel.AutoSize = false;

            if (!isMessageBox)
            {
                // Hide the icon on non-message box dialogs.
                IconPanel.Visible = false;

                if (this.AutoSize)
                {
                    // Perform autosize, then disable autosizing.
                    SetSize();
                }
                else
                {
                    // Just disable autosize and use specified sizes.
                    MasterPanel.AutoSize = false;
                    this.AutoSize = false;
                }

                // Set the controls panel width to match its parent.
                ControlsMainPanel.Width = MasterPanel.Width;
            }
            else
            {
                // Perform autosize, then disable autosizing.
                SetSize();

                // Set the controls panel width to fit next to the icon panel.
                ControlsMainPanel.Width = MasterPanel.Width - IconPanel.Width;
            }

            // Set the controls panel height to fit above the buttons panel.
            ControlsMainPanel.Height = MasterPanel.Height - ButtonsPanel.Height - 10;

            // Center to parent form.
            CenterToParentForm();
        }

        /// <summary>
        /// Refreshes layout and records the resulting size from autosize. Then disables autosize and sets the size back to the previous size.
        /// </summary>
        private void SetSize()
        {
            // Since disabling autosize will cause the controls
            // to revert to a different size, we need to record
            // the size before disabling and re-set it afterwards.

            // Refresh and force a layout event.
            this.Refresh();
            this.PerformLayout();

            // Record the resulting size.
            var size = this.Size;

            // Disable autosize.
            MasterPanel.AutoSize = false;
            this.AutoSize = false;

            // Re-set the size.
            this.Size = size;
        }

        private void SetLayout(Control control, bool suspend)
        {
            foreach (Control ctl in control.Controls)
            {
                if (suspend)
                {
                    ctl.SuspendLayout();
                }
                else
                {
                    ctl.ResumeLayout();
                }

                if (control.HasChildren)
                {
                    SetLayout(ctl, suspend);
                }
            }
        }

        /// <summary>
        /// Centers this instance to its parent form if that location is does not clip the active screen. Otherwise, will center to screen.
        /// </summary>
        private void CenterToParentForm()
        {
            if (parentForm != null)
            {
                var newX = parentForm.Location.X + ((parentForm.Width / 2) - this.Width / 2);
                var newY = parentForm.Location.Y + ((parentForm.Height / 2) - this.Height / 2);
                var newRect = new Rectangle(newX, newY, this.Width, this.Height);
                // The work area containing the parent form.
                var workArea = Screen.GetWorkingArea(parentForm);//.Location);

                // Make sure the new location is not off screen.
                if (workArea.Contains(newRect))
                {
                    // Center to parent form.
                    this.Location = new Point(newX, newY);
                }
                else
                {
                    // Center to screen.
                    this.CenterToScreen();
                }
            }
        }

        private void DisposeControls()
        {
            customControls.ForEach((c) => { c.Dispose(); });
            customControls.Clear();
        }

        private void LoadControls()
        {
            ControlsPanel.SuspendLayout();
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
                    ControlsPanel.Controls.Add(panel);
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
                    ControlsPanel.Controls.Add(panel);
                }
                else if (control is CheckBox)
                {
                    var checkBox = (CheckBox)control;
                    checkBox.AutoSize = true;
                    checkBox.Text = checkBox.Tag.ToString();
                    ControlsPanel.Controls.Add(checkBox);
                }
                else if (control is Label)
                {
                    var label = (Label)control;
                    label.AutoSize = true;
                    label.Padding = new Padding(5, 5, 5, 10);
                    ControlsPanel.Controls.Add(label);
                }
                else if (control is RichTextBox)
                {
                    var rtb = (RichTextBox)control;
                    var panel = ControlPanel();

                    if (isMessageBox)
                    {
                        ControlsPanel.Visible = false;
                        rtb.ReadOnly = true;
                        rtb.Margin = new Padding(5, 10, 5, 0);
                        rtb.BackColor = ControlsMainPanel.BackColor;
                        rtb.Dock = DockStyle.Fill;
                        rtb.TabStop = false;
                        rtb.LinkClicked += ClickedLink;
                        ControlsMainPanel.Controls.Add(rtb);
                    }
                    else
                    {
                        rtb.Width = 150;
                        rtb.Height = 80;
                        if (rtb.Tag != null) panel.Controls.Add(NewControlLabel(rtb.Tag.ToString()));
                        panel.Controls.Add(rtb);
                        ControlsPanel.Controls.Add(panel);
                    }
                }
                else if (control is Button)
                {
                    var button = (Button)control;
                    button.AutoSize = true;
                    button.Click += ButtonClick;
                    ControlsPanel.Controls.Add(button);
                }
                else
                {
                    var panel = ControlPanel();
                    panel.Controls.Add(NewControlLabel(control.Tag.ToString()));
                    panel.Controls.Add(control);
                    ControlsPanel.Controls.Add(panel);
                }
            }
            ControlsPanel.ResumeLayout();
        }

        private void MaximizeForm()
        {
            ControlsMainPanel.AutoSize = false;
            MasterPanel.AutoSize = false;
            this.AutoSize = false;
            this.Size = this.MaximumSize;
        }

        private Label NewControlLabel(string text)
        {
            var label = new Label();
            label.AutoSize = true;
            label.Padding = new Padding(0, 10, 5, 0);
            label.Text = text;
            return label;
        }

        private void CancelButtonUI_Click(object sender, System.EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void YesButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        private void NoButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void SetButtonsAndIcons(MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            switch (buttons)
            {
                case MessageBoxButtons.OK:
                    OkCancelPanel.Visible = true;
                    CancelButtonUI.Visible = false;
                    break;

                case MessageBoxButtons.OKCancel:
                    OkCancelPanel.Visible = true;
                    break;

                case MessageBoxButtons.YesNo:
                    OkCancelPanel.Visible = false;
                    YesNoPanel.Visible = true;
                    break;

                default:
                    SetButtonsAndIcons(MessageBoxButtons.OK, icon);
                    break;
            }

            switch (icon)
            {
                case MessageBoxIcon.Stop:
                    IconBox.Image = Properties.Resources.CriticalErrorIcon;
                    break;

                case MessageBoxIcon.Question:
                    IconBox.Image = Properties.Resources.QuestionIcon;
                    break;

                case MessageBoxIcon.Exclamation:
                    IconBox.Image = Properties.Resources.ExclamationIcon;
                    break;

                case MessageBoxIcon.Information:
                    IconBox.Image = Properties.Resources.InformationIcon;
                    break;
            }
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

        #endregion Methods
    }
}