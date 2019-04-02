namespace CrossStitchProject
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.browseButton = new System.Windows.Forms.Button();
            this.previewButton = new System.Windows.Forms.Button();
            this.crossStitchButton = new System.Windows.Forms.Button();
            this.ColorCheckBox = new System.Windows.Forms.CheckBox();
            this.fileLabel = new System.Windows.Forms.TextBox();
            this.SaveLabel = new System.Windows.Forms.Label();
            this.FolderNameBox = new System.Windows.Forms.TextBox();
            this.ditherCB = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "Image files (*.png, *.bmp, *.tiff) | *.png; *.bmp; *.tiff";
            this.openFileDialog1.Title = "Select an image file";
            // 
            // browseButton
            // 
            this.browseButton.Location = new System.Drawing.Point(12, 4);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(75, 23);
            this.browseButton.TabIndex = 2;
            this.browseButton.Text = "Browse";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.BrowseButton_Click);
            // 
            // previewButton
            // 
            this.previewButton.Location = new System.Drawing.Point(12, 28);
            this.previewButton.Name = "previewButton";
            this.previewButton.Size = new System.Drawing.Size(75, 23);
            this.previewButton.TabIndex = 3;
            this.previewButton.Text = "Preview";
            this.previewButton.UseVisualStyleBackColor = true;
            this.previewButton.Click += new System.EventHandler(this.PreviewButton_Click);
            // 
            // crossStitchButton
            // 
            this.crossStitchButton.Location = new System.Drawing.Point(12, 57);
            this.crossStitchButton.Name = "crossStitchButton";
            this.crossStitchButton.Size = new System.Drawing.Size(75, 23);
            this.crossStitchButton.TabIndex = 4;
            this.crossStitchButton.Text = "Generate";
            this.crossStitchButton.UseVisualStyleBackColor = true;
            this.crossStitchButton.Click += new System.EventHandler(this.CrossStitchButton_Click);
            // 
            // ColorCheckBox
            // 
            this.ColorCheckBox.AutoSize = true;
            this.ColorCheckBox.Location = new System.Drawing.Point(179, 32);
            this.ColorCheckBox.Name = "ColorCheckBox";
            this.ColorCheckBox.Size = new System.Drawing.Size(92, 17);
            this.ColorCheckBox.TabIndex = 5;
            this.ColorCheckBox.Text = "Color pattern?";
            this.ColorCheckBox.UseVisualStyleBackColor = true;
            // 
            // fileLabel
            // 
            this.fileLabel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.fileLabel.Enabled = false;
            this.fileLabel.Location = new System.Drawing.Point(94, 6);
            this.fileLabel.Name = "fileLabel";
            this.fileLabel.Size = new System.Drawing.Size(177, 20);
            this.fileLabel.TabIndex = 6;
            // 
            // SaveLabel
            // 
            this.SaveLabel.AutoSize = true;
            this.SaveLabel.Location = new System.Drawing.Point(91, 62);
            this.SaveLabel.Name = "SaveLabel";
            this.SaveLabel.Size = new System.Drawing.Size(83, 13);
            this.SaveLabel.TabIndex = 7;
            this.SaveLabel.Text = "Save Project As";
            // 
            // FolderNameBox
            // 
            this.FolderNameBox.Location = new System.Drawing.Point(179, 59);
            this.FolderNameBox.Name = "FolderNameBox";
            this.FolderNameBox.Size = new System.Drawing.Size(89, 20);
            this.FolderNameBox.TabIndex = 8;
            // 
            // ditherCB
            // 
            this.ditherCB.AutoSize = true;
            this.ditherCB.Location = new System.Drawing.Point(94, 32);
            this.ditherCB.Name = "ditherCB";
            this.ditherCB.Size = new System.Drawing.Size(85, 17);
            this.ditherCB.TabIndex = 9;
            this.ditherCB.Text = "Dither image";
            this.ditherCB.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(277, 89);
            this.Controls.Add(this.ditherCB);
            this.Controls.Add(this.FolderNameBox);
            this.Controls.Add(this.SaveLabel);
            this.Controls.Add(this.fileLabel);
            this.Controls.Add(this.ColorCheckBox);
            this.Controls.Add(this.crossStitchButton);
            this.Controls.Add(this.previewButton);
            this.Controls.Add(this.browseButton);
            this.Name = "Form1";
            this.Text = "Cross Stitch Pattern Maker";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.Button previewButton;
        private System.Windows.Forms.Button crossStitchButton;
        private System.Windows.Forms.CheckBox ColorCheckBox;
        private System.Windows.Forms.TextBox fileLabel;
        private System.Windows.Forms.Label SaveLabel;
        private System.Windows.Forms.TextBox FolderNameBox;
        private System.Windows.Forms.CheckBox ditherCB;
    }
}

