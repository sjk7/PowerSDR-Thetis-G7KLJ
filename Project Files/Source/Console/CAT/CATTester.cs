using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace Thetis
{
    /// <summary>
    /// Summary description for tester.
    /// </summary>
    public class CATTester : Form
    {

        private Button btnExit;
        private TextBoxTS txtInput;
        private TextBoxTS txtResult;
        private readonly Console console;
        private CATParser parser;
        private LabelTS label1;
        private LabelTS label2;
        private DataSet ds;
        private DataGridView dataGrid1;
        private Button btnExecute;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components = null;

        public CATTester(Console c)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            console = c;
            parser = new CATParser(console);
            ds = new DataSet();
            Setup();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private void Setup()
        {
            ds.ReadXml(Application.StartupPath + "\\CATStructs.xml");
            dataGrid1.DataSource = ds;
            txtInput.Focus();
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            ComponentResourceManager resources = new ComponentResourceManager(typeof(CATTester));
            this.btnExit = new Button();
            this.txtInput = new TextBoxTS();
            this.txtResult = new TextBoxTS();
            this.label1 = new LabelTS();
            this.label2 = new LabelTS();
            this.dataGrid1 = new DataGridView();
            this.btnExecute = new Button();
            ((ISupportInitialize)(this.dataGrid1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnExit
            // 
            this.btnExit.Location = new Point(440, 336);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new Size(75, 23);
            this.btnExit.TabIndex = 2;
            this.btnExit.Text = "Exit";
            this.btnExit.Click += new EventHandler(this.btnExit_Click);
            // 
            // txtInput
            // 
            this.txtInput.Location = new Point(120, 240);
            this.txtInput.Name = "txtInput";
            this.txtInput.Size = new Size(168, 20);
            this.txtInput.TabIndex = 0;
            this.txtInput.KeyUp += new KeyEventHandler(this.txtInput_KeyUp);
            // 
            // txtResult
            // 
            this.txtResult.Location = new Point(120, 280);
            this.txtResult.Name = "txtResult";
            this.txtResult.Size = new Size(392, 20);
            this.txtResult.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.Image = null;
            this.label1.Location = new Point(16, 240);
            this.label1.Name = "label1";
            this.label1.Size = new Size(88, 23);
            this.label1.TabIndex = 4;
            this.label1.Text = "CAT Command";
            this.label1.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // label2
            // 
            this.label2.Image = null;
            this.label2.Location = new Point(16, 280);
            this.label2.Name = "label2";
            this.label2.Size = new Size(88, 23);
            this.label2.TabIndex = 5;
            this.label2.Text = "CAT Response";
            this.label2.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // dataGrid1
            // 
            this.dataGrid1.Location = new Point(8, 0);
            this.dataGrid1.Name = "dataGrid1";
            this.dataGrid1.Size = new Size(512, 224);
            this.dataGrid1.TabIndex = 6;
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new Point(312, 240);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new Size(75, 23);
            this.btnExecute.TabIndex = 7;
            this.btnExecute.Text = "Execute";
            this.btnExecute.Click += new EventHandler(this.btnExecute_Click);
            // 
            // CATTester
            // 
            this.AutoScaleBaseSize = new Size(5, 13);
            this.ClientSize = new Size(536, 382);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.dataGrid1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtResult);
            this.Controls.Add(this.txtInput);
            this.Controls.Add(this.btnExit);
            this.Icon = ((Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CATTester";
            this.Text = "CAT Command Tester";
            ((ISupportInitialize)(this.dataGrid1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion


        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void txtInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                CheckText();
            }
        }

        private void ExecuteCommand()
        {
            string answer = parser.Get(txtInput.Text);
            txtResult.Text = answer;
            txtInput.Clear();

        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            CheckText();
        }

        private void CheckText()
        {
            if (!txtInput.Text.EndsWith(";"))
                txtInput.Text += ";";
            ExecuteCommand();
        }



    }
}
