namespace Thetis
{
    partial class frmTmp1
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
            this.txtVFOAMSD = new System.Windows.Forms.TextBoxTS();
            this.txtVFOALSD = new System.Windows.Forms.TextBoxTS();
            this.SuspendLayout();
            // 
            // txtVFOAMSD
            // 
            this.txtVFOAMSD.BackColor = System.Drawing.Color.Black;
            this.txtVFOAMSD.Cursor = System.Windows.Forms.Cursors.Default;
            this.txtVFOAMSD.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F);
            this.txtVFOAMSD.ForeColor = System.Drawing.Color.Olive;
            this.txtVFOAMSD.Location = new System.Drawing.Point(327, 80);
            this.txtVFOAMSD.Name = "txtVFOAMSD";
            this.txtVFOAMSD.ShortcutsEnabled = false;
            this.txtVFOAMSD.Size = new System.Drawing.Size(216, 38);
            this.txtVFOAMSD.TabIndex = 117;
            this.txtVFOAMSD.Text = "12345";
            this.txtVFOAMSD.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtVFOALSD
            // 
            this.txtVFOALSD.BackColor = System.Drawing.Color.Black;
            this.txtVFOALSD.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtVFOALSD.Cursor = System.Windows.Forms.Cursors.Default;
            this.txtVFOALSD.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F);
            this.txtVFOALSD.ForeColor = System.Drawing.Color.Olive;
            this.txtVFOALSD.Location = new System.Drawing.Point(499, 89);
            this.txtVFOALSD.Name = "txtVFOALSD";
            this.txtVFOALSD.Size = new System.Drawing.Size(44, 24);
            this.txtVFOALSD.TabIndex = 118;
            this.txtVFOALSD.Text = "000";
            this.txtVFOALSD.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // frmTmp1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.txtVFOALSD);
            this.Controls.Add(this.txtVFOAMSD);
            this.Name = "frmTmp1";
            this.Text = "frmTmp1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBoxTS txtVFOAMSD;
        private System.Windows.Forms.TextBoxTS txtVFOALSD;
    }
}