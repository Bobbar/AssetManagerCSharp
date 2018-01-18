using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using AssetManager.UserInterface.CustomControls;

namespace AssetManager.UserInterface.Forms.Sibi
{
    public partial class SibiNotesForm : ExtendedForm
    {

        private SibiRequestMapObject sibiRequest;

        public SibiRequestMapObject Request
        {
            get { return sibiRequest; }
        }

        public string Note
        {
            get { return NotesTextBox.Rtf.Trim(); }
        }

        public SibiNotesForm(ExtendedForm parentForm, SibiRequestMapObject request)
        {
            InitializeComponent();
            this.ParentForm = parentForm;
            sibiRequest = request;
            ShowDialog(parentForm);
        }

        public SibiNotesForm(ExtendedForm parentForm, string noteUID)
        {
            InitializeComponent();
            this.ParentForm = parentForm;
            FormUID = noteUID;
            ViewNote(noteUID);
        }

        private void ClearAll()
        {
            NotesTextBox.Clear();
        }

        private void ViewNote(string noteUID)
        {
            string noteText;
            string noteTimeStamp;

            try
            {
                OkButton.Visible = false;
                NotesTextBox.Clear();
                using (var results = DBFactory.GetDatabase().DataTableFromQueryString(Queries.SelectNoteByGuid(noteUID)))
                {
                    noteText = results.Rows[0][SibiNotesCols.Note].ToString();
                    noteTimeStamp = results.Rows[0][SibiNotesCols.DateStamp].ToString();
                }
                this.Text += " - " + noteTimeStamp;
                OtherFunctions.SetRichTextBox(NotesTextBox, noteText);
                NotesTextBox.ReadOnly = true;
                NotesTextBox.BackColor = Color.White;
                Show();
                Activate();
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
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
            this.Dispose();
        }

        private void NotesTextBox_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process.Start(e.LinkText);
        }

    }
}
