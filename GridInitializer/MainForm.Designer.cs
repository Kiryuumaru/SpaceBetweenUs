
namespace GridInitializer
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            this.panelControl = new System.Windows.Forms.Panel();
            this.radioButtonBR = new System.Windows.Forms.RadioButton();
            this.radioButtonTR = new System.Windows.Forms.RadioButton();
            this.radioButtonTL = new System.Windows.Forms.RadioButton();
            this.radioButtonBL = new System.Windows.Forms.RadioButton();
            this.label8 = new System.Windows.Forms.Label();
            this.buttonSelectBR = new System.Windows.Forms.Button();
            this.buttonSelectTR = new System.Windows.Forms.Button();
            this.buttonSelectTL = new System.Windows.Forms.Button();
            this.buttonSelectBL = new System.Windows.Forms.Button();
            this.textBoxBRX = new System.Windows.Forms.TextBox();
            this.textBoxBRY = new System.Windows.Forms.TextBox();
            this.textBoxBRD = new System.Windows.Forms.TextBox();
            this.textBoxTRX = new System.Windows.Forms.TextBox();
            this.textBoxTRY = new System.Windows.Forms.TextBox();
            this.textBoxTRD = new System.Windows.Forms.TextBox();
            this.textBoxTLX = new System.Windows.Forms.TextBox();
            this.textBoxTLY = new System.Windows.Forms.TextBox();
            this.textBoxTLD = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxBLX = new System.Windows.Forms.TextBox();
            this.textBoxBLY = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxBLD = new System.Windows.Forms.TextBox();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.buttonPausePlay = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.imageBox = new Emgu.CV.UI.ImageBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.panelMain = new System.Windows.Forms.Panel();
            this.panelControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox)).BeginInit();
            this.panelMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelControl
            // 
            this.panelControl.Controls.Add(this.radioButtonBR);
            this.panelControl.Controls.Add(this.radioButtonTR);
            this.panelControl.Controls.Add(this.radioButtonTL);
            this.panelControl.Controls.Add(this.radioButtonBL);
            this.panelControl.Controls.Add(this.label8);
            this.panelControl.Controls.Add(this.buttonSelectBR);
            this.panelControl.Controls.Add(this.buttonSelectTR);
            this.panelControl.Controls.Add(this.buttonSelectTL);
            this.panelControl.Controls.Add(this.buttonSelectBL);
            this.panelControl.Controls.Add(this.textBoxBRX);
            this.panelControl.Controls.Add(this.textBoxBRY);
            this.panelControl.Controls.Add(this.textBoxBRD);
            this.panelControl.Controls.Add(this.textBoxTRX);
            this.panelControl.Controls.Add(this.textBoxTRY);
            this.panelControl.Controls.Add(this.textBoxTRD);
            this.panelControl.Controls.Add(this.textBoxTLX);
            this.panelControl.Controls.Add(this.textBoxTLY);
            this.panelControl.Controls.Add(this.textBoxTLD);
            this.panelControl.Controls.Add(this.label7);
            this.panelControl.Controls.Add(this.label6);
            this.panelControl.Controls.Add(this.label5);
            this.panelControl.Controls.Add(this.label4);
            this.panelControl.Controls.Add(this.textBoxBLX);
            this.panelControl.Controls.Add(this.textBoxBLY);
            this.panelControl.Controls.Add(this.label3);
            this.panelControl.Controls.Add(this.label2);
            this.panelControl.Controls.Add(this.label1);
            this.panelControl.Controls.Add(this.textBoxBLD);
            this.panelControl.Controls.Add(this.buttonBrowse);
            this.panelControl.Controls.Add(this.buttonPausePlay);
            this.panelControl.Controls.Add(this.buttonSave);
            this.panelControl.Location = new System.Drawing.Point(0, 317);
            this.panelControl.Margin = new System.Windows.Forms.Padding(0);
            this.panelControl.Name = "panelControl";
            this.panelControl.Padding = new System.Windows.Forms.Padding(5);
            this.panelControl.Size = new System.Drawing.Size(497, 166);
            this.panelControl.TabIndex = 3;
            // 
            // radioButtonBR
            // 
            this.radioButtonBR.AutoSize = true;
            this.radioButtonBR.Location = new System.Drawing.Point(350, 26);
            this.radioButtonBR.Name = "radioButtonBR";
            this.radioButtonBR.Size = new System.Drawing.Size(14, 13);
            this.radioButtonBR.TabIndex = 45;
            this.radioButtonBR.TabStop = true;
            this.radioButtonBR.UseVisualStyleBackColor = true;
            this.radioButtonBR.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // radioButtonTR
            // 
            this.radioButtonTR.AutoSize = true;
            this.radioButtonTR.Location = new System.Drawing.Point(279, 26);
            this.radioButtonTR.Name = "radioButtonTR";
            this.radioButtonTR.Size = new System.Drawing.Size(14, 13);
            this.radioButtonTR.TabIndex = 44;
            this.radioButtonTR.TabStop = true;
            this.radioButtonTR.UseVisualStyleBackColor = true;
            this.radioButtonTR.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // radioButtonTL
            // 
            this.radioButtonTL.AutoSize = true;
            this.radioButtonTL.Location = new System.Drawing.Point(208, 26);
            this.radioButtonTL.Name = "radioButtonTL";
            this.radioButtonTL.Size = new System.Drawing.Size(14, 13);
            this.radioButtonTL.TabIndex = 43;
            this.radioButtonTL.TabStop = true;
            this.radioButtonTL.UseVisualStyleBackColor = true;
            this.radioButtonTL.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // radioButtonBL
            // 
            this.radioButtonBL.AutoSize = true;
            this.radioButtonBL.Location = new System.Drawing.Point(136, 26);
            this.radioButtonBL.Name = "radioButtonBL";
            this.radioButtonBL.Size = new System.Drawing.Size(14, 13);
            this.radioButtonBL.TabIndex = 42;
            this.radioButtonBL.TabStop = true;
            this.radioButtonBL.UseVisualStyleBackColor = true;
            this.radioButtonBL.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(10, 25);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(95, 13);
            this.label8.TabIndex = 38;
            this.label8.Text = "Referenced Depth";
            // 
            // buttonSelectBR
            // 
            this.buttonSelectBR.Location = new System.Drawing.Point(323, 133);
            this.buttonSelectBR.Name = "buttonSelectBR";
            this.buttonSelectBR.Size = new System.Drawing.Size(67, 23);
            this.buttonSelectBR.TabIndex = 36;
            this.buttonSelectBR.Text = "Select";
            this.buttonSelectBR.UseVisualStyleBackColor = true;
            this.buttonSelectBR.Click += new System.EventHandler(this.ButtonSelect_Click);
            // 
            // buttonSelectTR
            // 
            this.buttonSelectTR.Location = new System.Drawing.Point(252, 133);
            this.buttonSelectTR.Name = "buttonSelectTR";
            this.buttonSelectTR.Size = new System.Drawing.Size(67, 23);
            this.buttonSelectTR.TabIndex = 35;
            this.buttonSelectTR.Text = "Select";
            this.buttonSelectTR.UseVisualStyleBackColor = true;
            this.buttonSelectTR.Click += new System.EventHandler(this.ButtonSelect_Click);
            // 
            // buttonSelectTL
            // 
            this.buttonSelectTL.Location = new System.Drawing.Point(181, 133);
            this.buttonSelectTL.Name = "buttonSelectTL";
            this.buttonSelectTL.Size = new System.Drawing.Size(67, 23);
            this.buttonSelectTL.TabIndex = 34;
            this.buttonSelectTL.Text = "Select";
            this.buttonSelectTL.UseVisualStyleBackColor = true;
            this.buttonSelectTL.Click += new System.EventHandler(this.ButtonSelect_Click);
            // 
            // buttonSelectBL
            // 
            this.buttonSelectBL.Location = new System.Drawing.Point(110, 133);
            this.buttonSelectBL.Name = "buttonSelectBL";
            this.buttonSelectBL.Size = new System.Drawing.Size(67, 23);
            this.buttonSelectBL.TabIndex = 33;
            this.buttonSelectBL.Text = "Select";
            this.buttonSelectBL.UseVisualStyleBackColor = true;
            this.buttonSelectBL.Click += new System.EventHandler(this.ButtonSelect_Click);
            // 
            // textBoxBRX
            // 
            this.textBoxBRX.Location = new System.Drawing.Point(324, 106);
            this.textBoxBRX.Name = "textBoxBRX";
            this.textBoxBRX.Size = new System.Drawing.Size(65, 20);
            this.textBoxBRX.TabIndex = 31;
            this.textBoxBRX.Text = "0";
            this.textBoxBRX.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox_KeyUp);
            this.textBoxBRX.Leave += new System.EventHandler(this.TextBox_Leave);
            // 
            // textBoxBRY
            // 
            this.textBoxBRY.Location = new System.Drawing.Point(324, 77);
            this.textBoxBRY.Name = "textBoxBRY";
            this.textBoxBRY.Size = new System.Drawing.Size(65, 20);
            this.textBoxBRY.TabIndex = 30;
            this.textBoxBRY.Text = "0";
            this.textBoxBRY.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox_KeyUp);
            this.textBoxBRY.Leave += new System.EventHandler(this.TextBox_Leave);
            // 
            // textBoxBRD
            // 
            this.textBoxBRD.Location = new System.Drawing.Point(324, 48);
            this.textBoxBRD.Name = "textBoxBRD";
            this.textBoxBRD.Size = new System.Drawing.Size(65, 20);
            this.textBoxBRD.TabIndex = 29;
            this.textBoxBRD.Text = "0";
            this.textBoxBRD.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox_KeyUp);
            this.textBoxBRD.Leave += new System.EventHandler(this.TextBox_Leave);
            // 
            // textBoxTRX
            // 
            this.textBoxTRX.Location = new System.Drawing.Point(253, 106);
            this.textBoxTRX.Name = "textBoxTRX";
            this.textBoxTRX.Size = new System.Drawing.Size(65, 20);
            this.textBoxTRX.TabIndex = 28;
            this.textBoxTRX.Text = "0";
            this.textBoxTRX.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox_KeyUp);
            this.textBoxTRX.Leave += new System.EventHandler(this.TextBox_Leave);
            // 
            // textBoxTRY
            // 
            this.textBoxTRY.Location = new System.Drawing.Point(253, 77);
            this.textBoxTRY.Name = "textBoxTRY";
            this.textBoxTRY.Size = new System.Drawing.Size(65, 20);
            this.textBoxTRY.TabIndex = 27;
            this.textBoxTRY.Text = "0";
            this.textBoxTRY.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox_KeyUp);
            this.textBoxTRY.Leave += new System.EventHandler(this.TextBox_Leave);
            // 
            // textBoxTRD
            // 
            this.textBoxTRD.Location = new System.Drawing.Point(253, 48);
            this.textBoxTRD.Name = "textBoxTRD";
            this.textBoxTRD.Size = new System.Drawing.Size(65, 20);
            this.textBoxTRD.TabIndex = 26;
            this.textBoxTRD.Text = "0";
            this.textBoxTRD.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox_KeyUp);
            this.textBoxTRD.Leave += new System.EventHandler(this.TextBox_Leave);
            // 
            // textBoxTLX
            // 
            this.textBoxTLX.Location = new System.Drawing.Point(182, 106);
            this.textBoxTLX.Name = "textBoxTLX";
            this.textBoxTLX.Size = new System.Drawing.Size(65, 20);
            this.textBoxTLX.TabIndex = 25;
            this.textBoxTLX.Text = "0";
            this.textBoxTLX.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox_KeyUp);
            this.textBoxTLX.Leave += new System.EventHandler(this.TextBox_Leave);
            // 
            // textBoxTLY
            // 
            this.textBoxTLY.Location = new System.Drawing.Point(182, 77);
            this.textBoxTLY.Name = "textBoxTLY";
            this.textBoxTLY.Size = new System.Drawing.Size(65, 20);
            this.textBoxTLY.TabIndex = 24;
            this.textBoxTLY.Text = "0";
            this.textBoxTLY.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox_KeyUp);
            this.textBoxTLY.Leave += new System.EventHandler(this.TextBox_Leave);
            // 
            // textBoxTLD
            // 
            this.textBoxTLD.Location = new System.Drawing.Point(182, 48);
            this.textBoxTLD.Name = "textBoxTLD";
            this.textBoxTLD.Size = new System.Drawing.Size(65, 20);
            this.textBoxTLD.TabIndex = 23;
            this.textBoxTLD.Text = "0";
            this.textBoxTLD.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox_KeyUp);
            this.textBoxTLD.Leave += new System.EventHandler(this.TextBox_Leave);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.Purple;
            this.label7.Location = new System.Drawing.Point(323, 6);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(68, 13);
            this.label7.TabIndex = 22;
            this.label7.Text = "Bottom Right";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.Blue;
            this.label6.Location = new System.Drawing.Point(259, 6);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(54, 13);
            this.label6.TabIndex = 21;
            this.label6.Text = "Top Right";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.Lime;
            this.label5.Location = new System.Drawing.Point(192, 6);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 13);
            this.label5.TabIndex = 20;
            this.label5.Text = "Top Left";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.Red;
            this.label4.Location = new System.Drawing.Point(114, 6);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 13);
            this.label4.TabIndex = 19;
            this.label4.Text = "Bottom Left";
            // 
            // textBoxBLX
            // 
            this.textBoxBLX.Location = new System.Drawing.Point(111, 106);
            this.textBoxBLX.Name = "textBoxBLX";
            this.textBoxBLX.Size = new System.Drawing.Size(65, 20);
            this.textBoxBLX.TabIndex = 9;
            this.textBoxBLX.Text = "0";
            this.textBoxBLX.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox_KeyUp);
            this.textBoxBLX.Leave += new System.EventHandler(this.TextBox_Leave);
            // 
            // textBoxBLY
            // 
            this.textBoxBLY.Location = new System.Drawing.Point(111, 77);
            this.textBoxBLY.Name = "textBoxBLY";
            this.textBoxBLY.Size = new System.Drawing.Size(65, 20);
            this.textBoxBLY.TabIndex = 8;
            this.textBoxBLY.Text = "0";
            this.textBoxBLY.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox_KeyUp);
            this.textBoxBLY.Leave += new System.EventHandler(this.TextBox_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(28, 51);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Depth (Meters)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(69, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Y Axis";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(69, 109);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "X Axis";
            // 
            // textBoxBLD
            // 
            this.textBoxBLD.Location = new System.Drawing.Point(111, 48);
            this.textBoxBLD.Name = "textBoxBLD";
            this.textBoxBLD.Size = new System.Drawing.Size(65, 20);
            this.textBoxBLD.TabIndex = 3;
            this.textBoxBLD.Text = "0";
            this.textBoxBLD.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox_KeyUp);
            this.textBoxBLD.Leave += new System.EventHandler(this.TextBox_Leave);
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Location = new System.Drawing.Point(395, 46);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(90, 23);
            this.buttonBrowse.TabIndex = 2;
            this.buttonBrowse.Text = "Browse Source";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.ButtonBrowse_Click);
            // 
            // buttonPausePlay
            // 
            this.buttonPausePlay.Location = new System.Drawing.Point(395, 75);
            this.buttonPausePlay.Name = "buttonPausePlay";
            this.buttonPausePlay.Size = new System.Drawing.Size(90, 23);
            this.buttonPausePlay.TabIndex = 1;
            this.buttonPausePlay.Text = "Play";
            this.buttonPausePlay.UseVisualStyleBackColor = true;
            this.buttonPausePlay.Click += new System.EventHandler(this.ButtonPausePlay_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(395, 104);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(90, 23);
            this.buttonSave.TabIndex = 0;
            this.buttonSave.Text = "Save Grid";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.ButtonSave_Click);
            // 
            // imageBox
            // 
            this.imageBox.BackColor = System.Drawing.Color.Black;
            this.imageBox.FunctionalMode = Emgu.CV.UI.ImageBox.FunctionalModeOption.Minimum;
            this.imageBox.Location = new System.Drawing.Point(0, 0);
            this.imageBox.Margin = new System.Windows.Forms.Padding(0);
            this.imageBox.Name = "imageBox";
            this.imageBox.Size = new System.Drawing.Size(497, 317);
            this.imageBox.TabIndex = 2;
            this.imageBox.TabStop = false;
            this.imageBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ImageBox_MouseClick);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "MP4|*.mp4";
            // 
            // panelMain
            // 
            this.panelMain.AutoSize = true;
            this.panelMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelMain.Controls.Add(this.imageBox);
            this.panelMain.Controls.Add(this.panelControl);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 0);
            this.panelMain.Margin = new System.Windows.Forms.Padding(0);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(497, 483);
            this.panelMain.TabIndex = 4;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(497, 483);
            this.Controls.Add(this.panelMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Grid Initializer (SpaceBetweenUs)";
            this.panelControl.ResumeLayout(false);
            this.panelControl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox)).EndInit();
            this.panelMain.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button buttonSave;
        private Emgu.CV.UI.ImageBox imageBox;
        private System.Windows.Forms.Panel panelControl;
        private System.Windows.Forms.Button buttonPausePlay;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.TextBox textBoxBLX;
        private System.Windows.Forms.TextBox textBoxBLY;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxBLD;
        private System.Windows.Forms.TextBox textBoxBRX;
        private System.Windows.Forms.TextBox textBoxBRY;
        private System.Windows.Forms.TextBox textBoxBRD;
        private System.Windows.Forms.TextBox textBoxTRX;
        private System.Windows.Forms.TextBox textBoxTRY;
        private System.Windows.Forms.TextBox textBoxTRD;
        private System.Windows.Forms.TextBox textBoxTLX;
        private System.Windows.Forms.TextBox textBoxTLY;
        private System.Windows.Forms.TextBox textBoxTLD;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Button buttonSelectBR;
        private System.Windows.Forms.Button buttonSelectTR;
        private System.Windows.Forms.Button buttonSelectTL;
        private System.Windows.Forms.Button buttonSelectBL;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.RadioButton radioButtonBR;
        private System.Windows.Forms.RadioButton radioButtonTR;
        private System.Windows.Forms.RadioButton radioButtonTL;
        private System.Windows.Forms.RadioButton radioButtonBL;
    }
}