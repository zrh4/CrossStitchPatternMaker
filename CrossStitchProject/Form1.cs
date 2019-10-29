using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

namespace CrossStitchPatternMaker
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
            ShowWaitCursor();
            if (!previewWorker.IsBusy)
            {
                previewWorker.RunWorkerAsync();
            }
        }

        private void CrossStitchButton_Click(object sender, EventArgs e)
        {
            crossStitchButton.Enabled = false;
            ShowWaitCursor();
            if (!previewWorker.IsBusy)
            {
                patternWorker.RunWorkerAsync();
            }
        }
        private static void ShowWaitCursor()
        {
            Application.UseWaitCursor = true;
            Cursor.Current = Cursors.WaitCursor;
        }
        private static void ShowNormalCursor()
        {
            Application.UseWaitCursor = false;
            Cursor.Current = Cursors.Default;
        }
        private static void DisplayException(string message, Exception e)
        {
            MessageBox.Show(e.ToString(), message, MessageBoxButtons.OK);
        }

        private void PreviewWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            var filename = openFileDialog1.FileName;
            var colorPattern = ColorCheckBox.Checked;
            var ditherImage = ditherCB.Checked;
            var projectName = FolderNameBox.Text;

            try
            {
                var b = new Bitmap(filename);
                var crossStitcher = new CrossStitcher
                    (b, colorPattern, ditherImage, projectName);
                var preview = crossStitcher.GenerateStitchBitmap();
                e.Result = preview;
            }
            catch (Exception ex)
            {
                DisplayException("Error generating preview", ex);
            }
        }
        private void PreviewWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            previewButton.Enabled = true;
            ShowNormalCursor();
            if (e.Result == null) { return; }
            var preview = (Bitmap)e.Result;
            using (var previewForm = new Form())
            {
                previewForm.StartPosition = FormStartPosition.CenterScreen;
                previewForm.Width = preview.Width + 50;
                previewForm.Height = preview.Height + 50;
                var pb = new PictureBox
                {
                    Dock = DockStyle.Fill,
                    Image = preview
                };
                previewForm.Controls.Add(pb);
                previewForm.ShowDialog();
            }
        }

        private void PatternWorker_DoWork(object sender, DoWorkEventArgs e)
        {
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
                DisplayException("Error generating pattern", ex);
            }
        }
        private void PatternWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            crossStitchButton.Enabled = true;
            ShowNormalCursor();
        }
    }
}