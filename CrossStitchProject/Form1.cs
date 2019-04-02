using System;
using System.Windows.Forms;

namespace CrossStitchProject
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fileLabel.Text = openFileDialog1.FileName;
            }
        }

        private void PreviewButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(openFileDialog1.FileName)) return;
            previewButton.Enabled = false;
            Cursor.Current = Cursors.WaitCursor;
            var crossStitcher = new CrossStitcher
                (openFileDialog1.FileName, ColorCheckBox.Checked, ditherCB.Checked, FolderNameBox.Text);
            var preview = crossStitcher.GenerateStitchBitmap();
            previewButton.Enabled = true;
            Cursor.Current = Cursors.Default;

            using(var previewForm = new Form())
            {
                previewForm.StartPosition = FormStartPosition.CenterScreen;
                previewForm.Width = preview.Width+50;
                previewForm.Height = preview.Height+50;
                var pb = new PictureBox
                {
                    Dock = DockStyle.Fill,
                    Image = preview
                };
                previewForm.Controls.Add(pb);
                previewForm.ShowDialog();
            }
        }

        private void CrossStitchButton_Click(object sender, EventArgs e)
        {
            crossStitchButton.Enabled = false;
            Cursor.Current = Cursors.WaitCursor;
            var crossStitcher = new CrossStitcher
                (openFileDialog1.FileName, ColorCheckBox.Checked, ditherCB.Checked, FolderNameBox.Text);
            crossStitcher.GenerateCrossStitch();
            crossStitchButton.Enabled = true;
            Cursor.Current = Cursors.Default;
        }
    }
}