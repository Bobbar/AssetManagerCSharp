using AssetManager.Data;
using AssetManager.Data.Classes;
using AssetManager.Data.Communications;
using AssetManager.Helpers;
using AssetManager.UserInterface.CustomControls;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace AssetManager.UserInterface.Forms.Sibi
{
    public partial class SibiNotesForm : ExtendedForm
    {

        private SibiRequest sibiRequest;

        public SibiRequest Request
        {
            get { return sibiRequest; }
        }

        public string Note
        {
            get { return NotesTextBox.Rtf.Trim(); }
        }

        public SibiNotesForm(ExtendedForm parentForm, SibiRequest request) : base(parentForm, request)
        {
            InitializeComponent();
            sibiRequest = request;
            ShowDialog(parentForm);
        }

        public SibiNotesForm(ExtendedForm parentForm, string noteGuid) : base(parentForm, noteGuid)
        {
            InitializeComponent();
            ViewNote(noteGuid);
        }

        private void ViewNote(string noteGuid)
        {
            string noteText;
            string noteTimeStamp;

            try
            {
                OkButton.Visible = false;
                NotesTextBox.Clear();
                using (var results = DBFactory.GetDatabase().DataTableFromQueryString(Queries.SelectNoteByGuid(noteGuid)))
                {
                    noteText = results.Rows[0][SibiNotesCols.Note].ToString();
                    noteTimeStamp = results.Rows[0][SibiNotesCols.DateStamp].ToString();
                }
                this.Text += " - " + noteTimeStamp;
                NotesTextBox.TextOrRtf(noteText);
                NotesTextBox.ReadOnly = true;
                NotesTextBox.BackColor = Color.White;
                Show();
                Activate();
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Abort;
        }

        private void NotesTextBox_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process.Start(e.LinkText);
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (components != null)
                    {
                        components.Dispose();
                    }

                    sibiRequest?.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

    }
}
