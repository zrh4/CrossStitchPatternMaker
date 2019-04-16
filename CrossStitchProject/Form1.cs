using System;
using System.Drawing;
using System.IO;
using System.Linq;
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
            var filename = openFileDialog1.FileName;
            var colorPattern = ColorCheckBox.Checked;
            var ditherImage = ditherCB.Checked;
            var projectName = FolderNameBox.Text;

            var b = new Bitmap(filename);
            var crossStitcher = new CrossStitcher
                (b, colorPattern, ditherImage, projectName);
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
            var filename = openFileDialog1.FileName;
            var colorPattern = ColorCheckBox.Checked;
            var ditherImage = ditherCB.Checked;
            var projectName = FolderNameBox.Text;

            try
            {
                var b = new Bitmap(filename);
                var crossStitcher = new CrossStitcher
                    (b, colorPattern, ditherImage, projectName);
                crossStitcher.GenerateCrossStitch();
            }
            catch (Exception ex)
            {
               DisplayException("Error occurred during generation",ex); 
            }
            finally
            {
                crossStitchButton.Enabled = true;
                Cursor.Current = Cursors.Default;
            }
        }

        private void DisplayException(string message, Exception e)
        {
            MessageBox.Show(e.ToString(), message, MessageBoxButtons.OK);
        }
    }
}