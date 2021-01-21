
namespace GridInitializer
{
    partial class MainForms
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.Time_Label = new System.Windows.Forms.Label();
            this.Frame_lbl = new System.Windows.Forms.Label();
            this.Codec_lbl = new System.Windows.Forms.Label();
            this.Video_seek = new System.Windows.Forms.TrackBar();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Video_seek)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.comboBox1);
            this.groupBox1.Location = new System.Drawing.Point(8, 0);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(336, 64);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Source";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(255, 23);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 30);
            this.button1.TabIndex = 1;
            this.button1.Text = "Play";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Capture From Camera",
            "Capture From File"});
            this.comboBox1.Location = new System.Drawing.Point(8, 24);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(240, 27);
            this.comboBox1.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.pictureBox1);
            this.groupBox2.Location = new System.Drawing.Point(8, 64);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox2.Size = new System.Drawing.Size(336, 272);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Preview";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(8, 24);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(320, 240);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.Time_Label);
            this.groupBox3.Controls.Add(this.Frame_lbl);
            this.groupBox3.Controls.Add(this.Codec_lbl);
            this.groupBox3.Location = new System.Drawing.Point(8, 368);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox3.Size = new System.Drawing.Size(336, 96);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Properties";
            // 
            // Time_Label
            // 
            this.Time_Label.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Time_Label.Location = new System.Drawing.Point(168, 56);
            this.Time_Label.Name = "Time_Label";
            this.Time_Label.Size = new System.Drawing.Size(160, 30);
            this.Time_Label.TabIndex = 2;
            this.Time_Label.Text = "Time:";
            this.Time_Label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Frame_lbl
            // 
            this.Frame_lbl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Frame_lbl.Location = new System.Drawing.Point(8, 56);
            this.Frame_lbl.Name = "Frame_lbl";
            this.Frame_lbl.Size = new System.Drawing.Size(160, 30);
            this.Frame_lbl.TabIndex = 1;
            this.Frame_lbl.Text = "Frame:";
            this.Frame_lbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Codec_lbl
            // 
            this.Codec_lbl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Codec_lbl.Location = new System.Drawing.Point(8, 20);
            this.Codec_lbl.Name = "Codec_lbl";
            this.Codec_lbl.Size = new System.Drawing.Size(320, 30);
            this.Codec_lbl.TabIndex = 0;
            this.Codec_lbl.Text = "Codec:";
            this.Codec_lbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Video_seek
            // 
            this.Video_seek.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Video_seek.AutoSize = false;
            this.Video_seek.Location = new System.Drawing.Point(8, 342);
            this.Video_seek.Name = "Video_seek";
            this.Video_seek.Size = new System.Drawing.Size(336, 24);
            this.Video_seek.TabIndex = 25;
            this.Video_seek.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(354, 468);
            this.Controls.Add(this.Video_seek);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "VideoCapture";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Video_seek)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label Time_Label;
        private System.Windows.Forms.Label Frame_lbl;
        private System.Windows.Forms.Label Codec_lbl;
        private System.Windows.Forms.TrackBar Video_seek;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}