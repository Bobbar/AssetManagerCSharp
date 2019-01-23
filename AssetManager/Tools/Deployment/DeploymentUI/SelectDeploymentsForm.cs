using AssetManager.UserInterface.CustomControls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace AssetManager.Tools.Deployment
{
    public partial class SelectDeploymentsForm : ExtendedForm
    {
        public List<TaskInfo> SelectedDeployments
        {
            get
            {
                var selected = new List<TaskInfo>();

                foreach (ListViewItem item in SelectListBox.CheckedItems)
                {
                    selected.Add((TaskInfo)item.Tag);
                }

                return selected;
            }
        }

        private List<TaskInfo> _deployments;

        public SelectDeploymentsForm()
        {
            InitializeComponent();
        }

        public SelectDeploymentsForm(ExtendedForm parent, List<TaskInfo> deployments) : base(parent)
        {
            _deployments = deployments;

            InitializeComponent();

            PopulateDeployments();
        }

        private void PopulateDeployments()
        {
            SelectListBox.Items.Clear();

            foreach (var d in _deployments)
            {
                var item = new ListViewItem(d.TaskName);
                item.Tag = d;

                SelectListBox.Items.Add(item);
            }

            SelectListBox.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void SelectAllButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < SelectListBox.Items.Count; i++)
            {
                SelectListBox.Items[i].Checked = true;
            }
        }

        private void SelectNoneButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < SelectListBox.Items.Count; i++)
            {
                SelectListBox.Items[i].Checked = false;
            }
        }

        private void SelectListBox_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            var item = (TaskInfo)e.Item.Tag;

            DescriptionTextBox.Text = item.Description;
        }

        private void SelectListBox_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (e.Item.Checked)
            {
                e.Item.ForeColor = Color.Green;
            }
            else
            {
                e.Item.ForeColor = SelectListBox.ForeColor;
            }
        }
    }
}