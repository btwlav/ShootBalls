
namespace MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.balls1 = new Balls.Balls();
            this.SuspendLayout();
            // 
            // balls1
            // 
            this.balls1.ballWidth = 25;
            this.balls1.ColorsArr = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("balls1.ColorsArr")));
            this.balls1.Location = new System.Drawing.Point(310, 12);
            this.balls1.MaximumSize = new System.Drawing.Size(300, 500);
            this.balls1.Name = "balls1";
            this.balls1.Size = new System.Drawing.Size(300, 500);
            this.balls1.TabIndex = 0;
            this.balls1.Text = "balls1";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1136, 721);
            this.Controls.Add(this.balls1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private Balls.Balls balls1;
    }
}

