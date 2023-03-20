//=================================================================
// scan.cs
// created by Darrin Kohn ke9ns
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
//
//=================================================================

//=================================================================

using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace Thetis
{
    public class ScanControl : Form
    {
        public static Console console;   // ke9ns mod  to allow console to pass back values to setup screen
        public Setup setupForm;   // ke9ns communications with setupform  (i.e. allow combometertype.text update from inside console.cs) 

        //   private ArrayList file_list;
        private readonly string wave_folder = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) + "\\Thetis";

        private Button button1;
        private Button button2;
        private Button button3;
        private Button button4;
        private Button button5;
        private Button button6;
        public TextBox lowFBox;
        private TextBox stepBox;
        private TextBox textBox3;
        public TextBox currFBox;
        private Label label2;
        private Label label1;
        public TextBox highFBox;
        private Label label3;
        private TextBox speedBox;
        private Label label4;
        private Label label5;
        private CheckBoxTS chkAlwaysOnTop;
#pragma warning disable CS0649 // Field 'ScanControl.components' is never assigned to, and will always have its default value null
        private IContainer components;
#pragma warning restore CS0649 // Field 'ScanControl.components' is never assigned to, and will always have its default value null


        #region Constructor and Destructor

        public ScanControl(Console c)
        {
            InitializeComponent();
            console = c;

            Common.RestoreForm(this, "ScanForm", true);



        }

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

        #endregion

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            ComponentResourceManager resources = new ComponentResourceManager(typeof(ScanControl));
            this.button1 = new Button();
            this.button2 = new Button();
            this.button3 = new Button();
            this.button4 = new Button();
            this.button5 = new Button();
            this.button6 = new Button();
            this.lowFBox = new TextBox();
            this.stepBox = new TextBox();
            this.textBox3 = new TextBox();
            this.currFBox = new TextBox();
            this.label2 = new Label();
            this.label1 = new Label();
            this.highFBox = new TextBox();
            this.label3 = new Label();
            this.speedBox = new TextBox();
            this.label4 = new Label();
            this.label5 = new Label();
            this.chkAlwaysOnTop = new CheckBoxTS();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(24, 372);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Start";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(121, 372);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "Stop";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(218, 372);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 2;
            this.button3.Text = "Pause";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(218, 316);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 3;
            this.button4.Text = "Scan SWL";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(24, 316);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 23);
            this.button5.TabIndex = 4;
            this.button5.Text = "Scan Band";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(121, 316);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(75, 23);
            this.button6.TabIndex = 5;
            this.button6.Text = "Scan Mem";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new EventHandler(this.button6_Click);
            // 
            // lowFBox
            // 
            this.lowFBox.BackColor = System.Drawing.Color.LightYellow;
            this.lowFBox.Font = new System.Drawing.Font("Courier New", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lowFBox.Location = new System.Drawing.Point(12, 151);
            this.lowFBox.Name = "lowFBox";
            this.lowFBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.lowFBox.Size = new System.Drawing.Size(172, 29);
            this.lowFBox.TabIndex = 6;
            this.lowFBox.Click += new EventHandler(this.lowFBox_Click);
            this.lowFBox.KeyDown += new KeyEventHandler(this.lowFBox_KeyDown);
            // 
            // stepBox
            // 
            this.stepBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stepBox.Location = new System.Drawing.Point(15, 209);
            this.stepBox.MaxLength = 5;
            this.stepBox.Name = "stepBox";
            this.stepBox.Size = new System.Drawing.Size(119, 29);
            this.stepBox.TabIndex = 7;
            this.stepBox.Text = "1.0";
            // 
            // textBox3
            // 
            this.textBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox3.Location = new System.Drawing.Point(12, 12);
            this.textBox3.Multiline = true;
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(555, 104);
            this.textBox3.TabIndex = 9;
            this.textBox3.TabStop = false;
            this.textBox3.Text = resources.GetString("textBox3.Text");
            // 
            // currFBox
            // 
            this.currFBox.BackColor = System.Drawing.Color.LightYellow;
            this.currFBox.Font = new System.Drawing.Font("Courier New", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.currFBox.Location = new System.Drawing.Point(201, 151);
            this.currFBox.Name = "currFBox";
            this.currFBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.currFBox.Size = new System.Drawing.Size(178, 29);
            this.currFBox.TabIndex = 10;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 135);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(108, 13);
            this.label2.TabIndex = 19;
            this.label2.Text = "Low Edge Frequency";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(394, 135);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "High Edge Frequency\r\n";
            // 
            // highFBox
            // 
            this.highFBox.BackColor = System.Drawing.Color.LightYellow;
            this.highFBox.Font = new System.Drawing.Font("Courier New", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.highFBox.Location = new System.Drawing.Point(397, 151);
            this.highFBox.Name = "highFBox";
            this.highFBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.highFBox.Size = new System.Drawing.Size(170, 29);
            this.highFBox.TabIndex = 21;
            this.highFBox.Click += new EventHandler(this.highFBox_Click);
            this.highFBox.KeyDown += new KeyEventHandler(this.highFBox_KeyDown);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(198, 135);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(142, 13);
            this.label3.TabIndex = 22;
            this.label3.Text = "Current Frequency Scanning";
            // 
            // speedBox
            // 
            this.speedBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.speedBox.Location = new System.Drawing.Point(15, 269);
            this.speedBox.MaxLength = 3;
            this.speedBox.Name = "speedBox";
            this.speedBox.Size = new System.Drawing.Size(119, 29);
            this.speedBox.TabIndex = 23;
            this.speedBox.Text = "200";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 193);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 13);
            this.label4.TabIndex = 24;
            this.label4.Text = "Step in Khz";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 253);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(79, 13);
            this.label5.TabIndex = 25;
            this.label5.Text = "Speed in mSec";
            // 
            // chkAlwaysOnTop
            // 
            this.chkAlwaysOnTop.Image = null;
            this.chkAlwaysOnTop.Location = new System.Drawing.Point(461, 372);
            this.chkAlwaysOnTop.Name = "chkAlwaysOnTop";
            this.chkAlwaysOnTop.Size = new System.Drawing.Size(104, 24);
            this.chkAlwaysOnTop.TabIndex = 59;
            this.chkAlwaysOnTop.Text = "Always On Top";
            this.chkAlwaysOnTop.CheckedChanged += new EventHandler(this.chkAlwaysOnTop_CheckedChanged);
            // 
            // ScanControl
            // 
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(580, 407);
            this.Controls.Add(this.chkAlwaysOnTop);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.speedBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.highFBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.currFBox);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.stepBox);
            this.Controls.Add(this.lowFBox);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(330, 446);
            this.Name = "ScanControl";
            this.Text = "Scanner";
            this.FormClosing += new FormClosingEventHandler(this.ScanControl_FormClosing);
            this.Load += new EventHandler(this.ScanControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }





        #endregion

        #region Properties



        #endregion

        #region Event Handlers







        #endregion

        private void ScanControl_Load(object sender, EventArgs e)
        {

        }

        private int band_index;
        public static byte ScanStop = 0; // 0=run 1= stop
        public static byte ScanRST = 0; // 1=pick up where you left off, 0=reset back to low_freq
        private string last_band;							// Used in bandstacking algorithm

        //=======================================================================================
        // ke9ns add scan just the Band stacking reg for the SWL bands
        private void button4_Click(object sender, EventArgs e)
        {

            /*
            BLMF, // ke9ns move down below vhf
        B120M,
        B90M,
        B61M,
        B49M,
        B41M,
        B31M,
        B25M,
        B22M,
        B19M,
        B16M,
        B14M,
        B13M,
        B11M,
        */

            last_band = "160M";

            if (last_band.Equals("160M"))
            {

                console.band_160m_index = (console.band_160m_index + 1) % console.band_160m_register;
            }
            last_band = "160M";

            band_index = console.band_160m_index;

            if (DB.GetBandStack(last_band, band_index, out string mode, out string filter, out double freq, out bool CTUN, out int ZoomFactor, out double CenterFreq))
            {
                filter = filter.Substring(0, 2); // ke9ns add for bandstack lockout

                console.SetBand(mode, filter, freq, CTUN, ZoomFactor, CenterFreq);
            }

            console.UpdateWaterfallLevelValues();

        } // button4_Click SWL bandstacking scanner 




        //=======================================================================================================================
        private void ScanControl_FormClosing(object sender, FormClosingEventArgs e)
        {

            this.Hide();
            e.Cancel = true;
            Common.SaveForm(this, "ScanForm");
            //  console.MemoryList.Save();



        }

        private void button6_Click(object sender, EventArgs e)
        {
            // Trace.WriteLine("memory list");
            if (console.MemoryList.List.Count == 0) return; // nothing in the list, exit
            Trace.WriteLine("memory list");

            //  int index = dataGridView1.CurrentCell.RowIndex;

            //  if (index < 0 || index > console.MemoryList.List.Count - 1) // index out of range
            //     return;




            //   comboFMMemory.DataSource = MemoryList.List;
            //   comboFMMemory.DisplayMember = "Name";
            //   comboFMMemory.ValueMember = "Name";

            //  MemoryRecord recordToRestore = new MemoryRecord((MemoryRecord)comboFMMemory.SelectedItem);
            //  if (!initializing)
            //     RecallMemory(recordToRestore);




        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }




        //===========================================================================================
        public static byte SP5_Active = 0;
        double freq1 = 0.0;
        public static double freq_Low = 0.0;
        public static double freq_High = 0.0;
        public static double freq_Last = 0.0;

        private void button5_Click(object sender, EventArgs e)
        {

            Trace.WriteLine("click    ");

            ScanStop = 0; // reset scan

            if (SP5_Active == 0)
            {

                SP5_Active = 1;

                // see console routine  if (rx1_band != old_band || initializing) for setting low and high settings

                Thread t = new Thread(new ThreadStart(SCANNER))
                {
                    Name = "Scanner Thread",
                    IsBackground = true,
                    Priority = ThreadPriority.BelowNormal
                };
                t.Start();

                Trace.WriteLine("good    ");

            } // SP_active = 0;
            else
            {

                SP5_Active = 0;
                Trace.WriteLine("OFF   ");

            } // SP_Active = 1




        } // button5 click



        //===============================================================================
        //===============================================================================
        // ke9ns SCANNER thread

        double step = 0.0001;
        int speed = 50;


        private /* async */ void SCANNER()
        {

            freq1 = freq_Low;

            try
            {
                step = Convert.ToDouble(stepBox.Text) / 1000;
            }
            catch (Exception)
            {
                step = 0.0001; // 1 khz
            }

            try
            {
                speed = Convert.ToInt16(speedBox.Text);
            }
            catch (Exception)
            {
                speed = 50; // 50msec
            }



            //   Trace.WriteLine("good1   ");

            double ii = freq_Low;

            if (ScanRST == 1)
            {
                ii = freq_Last;
            }


            for (; ii < freq_High; ii = ii + step)
            {

                currFBox.Text = ii.ToString("f6");

                console.VFOAFreq = ii; // convert to MHZ

                if (ScanStop == 1) break;
                //  await Task.Delay(speed/10);

                if (ScanStop == 1) break;
                // await Task.Delay(speed / 10);

                if (ScanStop == 1) break;
                // await Task.Delay(speed / 10);

                if (ScanStop == 1) break;
                //  await Task.Delay(speed / 10);

                if (ScanStop == 1) break;
                // await Task.Delay(speed / 10);

                if (ScanStop == 1) break;
                // await Task.Delay(speed / 10);

                if (ScanStop == 1) break;
                //  await Task.Delay(speed / 10);

                if (ScanStop == 1) break;
                // await Task.Delay(speed / 10);

                if (ScanStop == 1) break;
                //  await Task.Delay(speed / 10);

                if (ScanStop == 1) break;
                //   await Task.Delay(speed / 10);


                if (SP5_Active == 0) break;
            }

            if (ii >= freq_High)
            {
                ScanRST = 0; // reset back to start
                ii = freq_Low;

                Trace.WriteLine("finished ");


            }
            else
            {
                ScanRST = 1; // leave off where you left off
                freq_Last = ii + (step * 2); // need to jump past last signal that breaks squelch otherwise you cant move anymore
            }

        } // SCANNER


        //===============================================================================
        //===============================================================================


        // ke9ns override band edge setting 
        private void lowFBox_Click(object sender, EventArgs e)
        {
            double freq2 = 0.0;
            ScanRST = 0;
            try
            {
                freq2 = Convert.ToDouble(lowFBox.Text);

                if (freq2 < freq_Low) freq2 = freq_Low;

            }
            catch (Exception)
            {
                freq2 = freq_Low;


            }

            freq_Low = freq2;
            lowFBox.Text = freq_Low.ToString("f6");
        }

        private void highFBox_Click(object sender, EventArgs e)
        {
            double freq3 = 0.0;
            ScanRST = 0;
            try
            {
                freq3 = Convert.ToDouble(highFBox.Text);
                if (freq3 > freq_High) freq3 = freq_High;
            }
            catch (Exception)
            {
                freq3 = freq_High;


            }

            freq_High = freq3;
            highFBox.Text = freq_High.ToString("f6");

        }

        private void lowFBox_KeyDown(object sender, KeyEventArgs e)
        {
            double freq2 = 0.0;

            if (e.KeyData == Keys.Enter)
            {
                ScanRST = 0;
                try
                {
                    freq2 = Convert.ToDouble(lowFBox.Text);

                    if (freq2 < freq_Low) freq2 = freq_Low;

                }
                catch (Exception)
                {
                    freq2 = freq_Low;


                }

                freq_Low = freq2;
                lowFBox.Text = freq_Low.ToString("f6");
            } // wait for enter key

        } // lowFBox_KeyDown

        private void highFBox_KeyDown(object sender, KeyEventArgs e)
        {
            double freq3 = 0.0;

            if (e.KeyData == Keys.Enter)
            {
                ScanRST = 0;
                try
                {
                    freq3 = Convert.ToDouble(highFBox.Text);
                    if (freq3 > freq_High) freq3 = freq_High;
                }
                catch (Exception)
                {
                    freq3 = freq_High;


                }

                freq_High = freq3;
                highFBox.Text = freq_High.ToString("f6");
            } // wait for enter key

        }// highFBox_KeyDown

        private void chkAlwaysOnTop_CheckedChanged(object sender, EventArgs e)
        {
            this.TopMost = chkAlwaysOnTop.Checked;
        }



    } // scancontrol


} // powersdr
