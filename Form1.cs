// Vaunix example program
// Calling the LDA DLL from a C# program
// RD Updated 4/27/2019
// This example uses the 64 bit LDA DLL
// It is intended to show how to call the API, and is a very basic example of using the API
// The example does not include the kind of error handling that would be present in a production application
// It also does not implement the kind of classes which might be used in an actual application
// This example shows the use of the newer "HR" functions which specify attenuation in .05 db units.
//
// This example shows both accessing the LDA API functions directly, by LabBrickWrapper.FunctionName
// and also using wrapper functions in the LabBrickWrapper class. Typically it would be a better design to
// choose one approach for an application, not mix the two approaches. I am showing both approaches here
// as examples of API access, not overall application architecture.
// 



using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;
using System.Configuration;

namespace LabBrickAttTest
{
    public partial class Form1 : Form
    {
        Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        private int m_NumberOfDevices = 0;
        public LabBrickWrapper m_LabBrick = null;

        private int m_dllversion = 0;
        public bool LDA_Open = false;
        bool chek_device = true;

        private DeviceRun deviceRun01;
        private DeviceRun deviceRun02;
        private DeviceRun deviceRun03;
        private DeviceRun deviceRun04;

        public Form1()
        {
            InitializeComponent();

            m_LabBrick = new LabBrickWrapper();

            // show our DLL version
            m_dllversion = m_LabBrick.GetDLLVersion();
            Dll_Version.Text = "DLL Version " + m_dllversion.ToString("X3", CultureInfo.CurrentCulture);


            // Set test mode false to look for actual hardware
            m_LabBrick.SetTestMode(false);

            deviceRun01 = new DeviceRun(this, numericUpDown01,txtboxstartdb01,txtboxstopdb01,txtboxchannelnumber01,txtboxtimeinterval01,btnstart01,btnstop01);
            deviceRun02 = new DeviceRun(this, numericUpDown02, txtboxstartdb02, txtboxstopdb02, txtboxchannelnumber02, txtboxtimeinterval02, btnstart02, btnstop02);
            deviceRun03 = new DeviceRun(this, numericUpDown03, txtboxstartdb03, txtboxstopdb03, txtboxchannelnumber03, txtboxtimeinterval03, btnstart03, btnstop03);
            deviceRun04 = new DeviceRun(this, numericUpDown04, txtboxstartdb04, txtboxstopdb04, txtboxchannelnumber04, txtboxtimeinterval04, btnstart04, btnstop04);

            //Form1 form, NumericUpDown numericupdown,  TextBox txtboxstartdb, TextBox txtboxstopdb,TextBox channelnumber,TextBox txtboxtimeinterval,Button btnstart, Button btnstop
        }




        // search for devices and open the first one we find
        private void btnDeviceConnect_Click(object sender, EventArgs e)
        {
            int status = 0;
            byte[] temp = new byte[32];
            float ftemp = 0;

            m_NumberOfDevices = m_LabBrick.GetNumberOfDevices();    // How many devices did we find?
            NumberOfDevices.Text = m_NumberOfDevices.ToString();
            
            if (m_NumberOfDevices > 0 && !LDA_Open)
            {
                status = m_LabBrick.GetDevices();

                status = m_LabBrick.InitDevice(m_LabBrick.MyDevices[0]);    // we will just use the first device
                LDA_Open = true;
                this.groupBox2.BackColor = System.Drawing.SystemColors.ActiveCaption;
                //lblChannelNo.Text = "Channel No : " + Convert.ToString(  m_LabBrick.GetChannelNumber(m_LabBrick.MyDevices[0]) );

                // lets show the name of the device
                status = m_LabBrick.GetModelNameA(m_LabBrick.MyDevices[0], temp);

                string LDA_Name = Encoding.UTF8.GetString(temp, 0, temp.Length);
                label2.Text = LDA_Name;

                // get some parameters about the device
                status = LabBrickWrapper.fnLDA_GetMaxAttenuationHR(m_LabBrick.MyDevices[0]);
                
                if (status >= 0)
                {
                    ftemp = (float)status / 20.00F;
                    Max_Atten_Label.Text = "Maximum Attenuation: " + ftemp.ToString("F2", CultureInfo.CurrentCulture) + " dB";
                    
                }
                else
                {
                    // Do something about the error.
                    this.groupBox2.BackColor = System.Drawing.SystemColors.ControlDark;
                }

                //float m_atten = (float)numericUpDown01.Value;
                //m_LabBrick.SetAttenuationInDb(m_LabBrick.MyDevices[0], m_atten);
                // initialize our attenuation control with the existing device setting
                //numericUpDown01.Value = (decimal) m_LabBrick.GetAttenuationHR(m_LabBrick.MyDevices[0]) / 20M;

                this.btnDeviceConnect.Enabled = false;

            }
        }

        private void btnDeviceDisConnect_Click(object sender, EventArgs e)
        {
            //if (LDA_Open)
            {
                m_LabBrick.CloseDevice(m_LabBrick.MyDevices[0]);
                LDA_Open = false;
                this.btnDeviceConnect.Enabled = true;
                this.groupBox2.BackColor = System.Drawing.Color.LightCoral;

                label2.Invoke((MethodInvoker)delegate ()
                {
                    label2.Text = "LDA Name: ";
                });

                Max_Atten_Label.Invoke((MethodInvoker)delegate ()
                {
                    Max_Atten_Label.Text = "Maximum Attenuation: ";
                });

                NumberOfDevices.Invoke((MethodInvoker)delegate ()
                {
                    NumberOfDevices.Text = "0";
                });


                //btnstop01.PerformClick();
                //btnstop02.PerformClick();
                //btnstop03.PerformClick();
                //btnstop04.PerformClick();

            }
        }

 
        private void Form1_Load(object sender, EventArgs e)
        {
            string retstr = "";
            try
            {
                retstr = ConfigurationManager.AppSettings["Startdb01"];
                if( !retstr.Equals("") && retstr != null ){ this.txtboxstartdb01.Text = retstr; }
                retstr = ConfigurationManager.AppSettings["Startdb02"];
                if (!retstr.Equals("") && retstr != null) { this.txtboxstartdb02.Text = retstr; }
                retstr = ConfigurationManager.AppSettings["Startdb03"];
                if (!retstr.Equals("") && retstr != null) { this.txtboxstartdb03.Text = retstr; }
                retstr = ConfigurationManager.AppSettings["Startdb04"];
                if (!retstr.Equals("") && retstr != null) { this.txtboxstartdb04.Text = retstr; }

                retstr = ConfigurationManager.AppSettings["Stopdb01"];
                if (!retstr.Equals("") && retstr != null) { this.txtboxstopdb01.Text = retstr; }
                retstr = ConfigurationManager.AppSettings["Stopdb02"];
                if (!retstr.Equals("") && retstr != null) { this.txtboxstopdb02.Text = retstr; }
                retstr = ConfigurationManager.AppSettings["Stopdb03"];
                if (!retstr.Equals("") && retstr != null) { this.txtboxstopdb03.Text = retstr; }
                retstr = ConfigurationManager.AppSettings["Stopdb04"];
                if (!retstr.Equals("") && retstr != null) { this.txtboxstopdb04.Text = retstr; }

                retstr = ConfigurationManager.AppSettings["Stepdb01"];
                if (!retstr.Equals("") && retstr != null) { this.txtboxstepdb01.Text = retstr; }
                retstr = ConfigurationManager.AppSettings["Stepdb02"];
                if (!retstr.Equals("") && retstr != null) { this.txtboxstepdb02.Text = retstr; }
                retstr = ConfigurationManager.AppSettings["Stepdb03"];
                if (!retstr.Equals("") && retstr != null) { this.txtboxstepdb03.Text = retstr; }
                retstr = ConfigurationManager.AppSettings["Stepdb04"];
                if (!retstr.Equals("") && retstr != null) { this.txtboxstepdb04.Text = retstr; }

                retstr = ConfigurationManager.AppSettings["Timeinterval01"];
                if (!retstr.Equals("") && retstr != null) { this.txtboxtimeinterval01.Text = retstr; }
                retstr = ConfigurationManager.AppSettings["Timeinterval02"];
                if (!retstr.Equals("") && retstr != null) { this.txtboxtimeinterval02.Text = retstr; }
                retstr = ConfigurationManager.AppSettings["Timeinterval03"];
                if (!retstr.Equals("") && retstr != null) { this.txtboxtimeinterval03.Text = retstr; }
                retstr = ConfigurationManager.AppSettings["Timeinterval04"];
                if (!retstr.Equals("") && retstr != null) { this.txtboxtimeinterval04.Text = retstr; }

                retstr = ConfigurationManager.AppSettings["Reverse01"];
                if (!retstr.Equals("") && retstr != null) { this.chkboxReverse01.Checked = bool.Parse(retstr); }
                retstr = ConfigurationManager.AppSettings["Reverse02"];
                if (!retstr.Equals("") && retstr != null) { this.chkboxReverse02.Checked = bool.Parse(retstr); }
                retstr = ConfigurationManager.AppSettings["Reverse03"];
                if (!retstr.Equals("") && retstr != null) { this.chkboxReverse03.Checked = bool.Parse(retstr); }
                retstr = ConfigurationManager.AppSettings["Reverse04"];
                if (!retstr.Equals("") && retstr != null) { this.chkboxReverse04.Checked = bool.Parse(retstr); }

            }catch(Exception ex)
            {

            }

            btnDeviceConnect.PerformClick();

            Thread mthr = new Thread(new ThreadStart(check_Device));
            mthr.Start();


        }

        void check_Device()
        {
            byte[] temp = new byte[32];
            while (chek_device)
            {
                try
                {
                    int numberOfDevices = m_LabBrick.GetNumberOfDevices();

                    if (numberOfDevices > 0)
                    {
                        if (LDA_Open == false)
                        {
                            btnDeviceConnect.Invoke((MethodInvoker)delegate ()
                            { 
                                btnDeviceConnect.PerformClick();
                            });
                        }
                    }
                    else
                    {
                        btnDeviceDisConnect.Invoke((MethodInvoker)delegate ()
                        {
                            btnDeviceDisConnect.PerformClick();
                        });
                        
                    }

                    Thread.Sleep(1000);
                }catch(Exception ex)
                {
                    LDA_Open = false;
                    txtboxmessage.Invoke((MethodInvoker)delegate ()
                    {
                        txtboxmessage.Text = txtboxmessage.Text + ex.ToString() + System.Environment.NewLine;
                    });
                }
            }
        }


        private void txtboxmessage_TextChanged(object sender, EventArgs e)
        {
            //Console.WriteLine(txtboxmessage.Text);
        }


        private void btnstart01_Click(object sender, EventArgs e)
        {

            if (LDA_Open)
            {
                deviceRun01.setParam(txtboxstartdb01.Text, txtboxstopdb01.Text, txtboxstepdb01.Text, txtboxtimeinterval01.Text);
                deviceRun01.threadRun();
            }

        }

        private void btnstop01_Click(object sender, EventArgs e)
        {

            deviceRun01.threadStop();
            btnstart01.Enabled = true;

        }

        private void btnstart02_Click(object sender, EventArgs e)
        {
            if (LDA_Open)
            {
                deviceRun02.setParam(txtboxstartdb02.Text, txtboxstopdb02.Text, txtboxstepdb02.Text, txtboxtimeinterval02.Text);
                deviceRun02.threadRun();
            }
        }

        private void btnstop02_Click(object sender, EventArgs e)
        {
            deviceRun02.threadStop();
            btnstart02.Enabled = true;
        }

        private void btnstart03_Click(object sender, EventArgs e)
        {
            if (LDA_Open)
            {
                deviceRun03.setParam(txtboxstartdb03.Text, txtboxstopdb03.Text, txtboxstepdb03.Text, txtboxtimeinterval03.Text);
                deviceRun03.threadRun();
            }
        }

        private void btnstop03_Click(object sender, EventArgs e)
        {
            deviceRun03.threadStop();
            btnstart03.Enabled = true;
        }

        private void btnstart04_Click(object sender, EventArgs e)
        {
            if (LDA_Open)
            {
                deviceRun04.setParam(txtboxstartdb04.Text, txtboxstopdb04.Text, txtboxstepdb04.Text, txtboxtimeinterval04.Text);
                deviceRun04.threadRun();
            }
        }

        private void btnstop04_Click(object sender, EventArgs e)
        {
            deviceRun04.threadStop();
            btnstart04.Enabled = true;
        }

        private void chkboxReverse01_CheckedChanged(object sender, EventArgs e)
        {
            if (chkboxReverse01.Checked)
            {
                deviceRun01.m_checked = true;
            }
            else
            {
                deviceRun01.m_checked = false;
            }
        }//chkboxReverse01_CheckedChanged

        private void chkboxReverse02_CheckedChanged(object sender, EventArgs e)
        {
            if (chkboxReverse02.Checked)
            {
                deviceRun02.m_checked = true;
            }
            else
            {
                deviceRun02.m_checked = false;
            }
        }

        private void chkboxReverse03_CheckedChanged(object sender, EventArgs e)
        {
            if (chkboxReverse03.Checked)
            {
                deviceRun03.m_checked = true;
            }
            else
            {
                deviceRun03.m_checked = false;
            }
        }

        private void chkboxReverse04_CheckedChanged(object sender, EventArgs e)
        {
            if (chkboxReverse04.Checked)
            {
                deviceRun04.m_checked = true;
            }
            else
            {
                deviceRun04.m_checked = false;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {


            if (ConfigurationManager.AppSettings["Startdb01"] == null) { config.AppSettings.Settings.Add("Startdb01", txtboxstartdb01.Text);  }
            else { config.AppSettings.Settings["Startdb01"].Value = txtboxstartdb01.Text; }
            if (ConfigurationManager.AppSettings["Startdb02"] == null) { config.AppSettings.Settings.Add("Startdb02", txtboxstartdb02.Text); }
            else { config.AppSettings.Settings["Startdb02"].Value = txtboxstartdb02.Text; }
            if (ConfigurationManager.AppSettings["Startdb03"] == null) { config.AppSettings.Settings.Add("Startdb03", txtboxstartdb03.Text); }
            else { config.AppSettings.Settings["Startdb03"].Value = txtboxstartdb03.Text; }
            if (ConfigurationManager.AppSettings["Startdb04"] == null) { config.AppSettings.Settings.Add("Startdb04", txtboxstartdb04.Text); }
            else { config.AppSettings.Settings["Startdb04"].Value = txtboxstartdb04.Text; }

            if (ConfigurationManager.AppSettings["Stopdb01"] == null) { config.AppSettings.Settings.Add("Stopdb01", txtboxstopdb01.Text); }
            else { config.AppSettings.Settings["Stopdb01"].Value = txtboxstopdb01.Text; }
            if (ConfigurationManager.AppSettings["Stopdb02"] == null) { config.AppSettings.Settings.Add("Stopdb02", txtboxstopdb02.Text); }
            else { config.AppSettings.Settings["Stopdb02"].Value = txtboxstopdb02.Text; }
            if (ConfigurationManager.AppSettings["Stopdb03"] == null) { config.AppSettings.Settings.Add("Stopdb03", txtboxstopdb03.Text); }
            else { config.AppSettings.Settings["Stopdb03"].Value = txtboxstopdb03.Text; }
            if (ConfigurationManager.AppSettings["Stopdb04"] == null) { config.AppSettings.Settings.Add("Stopdb04", txtboxstopdb04.Text); }
            else { config.AppSettings.Settings["Stopdb04"].Value = txtboxstopdb04.Text; }

            if (ConfigurationManager.AppSettings["Stepdb01"] == null) { config.AppSettings.Settings.Add("Stepdb01", txtboxstepdb01.Text); }
            else { config.AppSettings.Settings["Stepdb01"].Value = txtboxstepdb01.Text; }
            if (ConfigurationManager.AppSettings["Stepdb02"] == null) { config.AppSettings.Settings.Add("Stepdb02", txtboxstepdb02.Text); }
            else { config.AppSettings.Settings["Stepdb02"].Value = txtboxstepdb02.Text; }
            if (ConfigurationManager.AppSettings["Stepdb03"] == null) { config.AppSettings.Settings.Add("Stepdb03", txtboxstepdb03.Text); }
            else { config.AppSettings.Settings["Stepdb03"].Value = txtboxstepdb03.Text; }
            if (ConfigurationManager.AppSettings["Stepdb04"] == null) { config.AppSettings.Settings.Add("Stepdb04", txtboxstepdb04.Text); }
            else { config.AppSettings.Settings["Stepdb04"].Value = txtboxstepdb04.Text; }

            if (ConfigurationManager.AppSettings["Timeinterval01"] == null) { config.AppSettings.Settings.Add("Timeinterval01", txtboxtimeinterval01.Text); }
            else { config.AppSettings.Settings["Timeinterval01"].Value = txtboxtimeinterval01.Text; }
            if (ConfigurationManager.AppSettings["Timeinterval02"] == null) { config.AppSettings.Settings.Add("Timeinterval02", txtboxtimeinterval02.Text); }
            else { config.AppSettings.Settings["Timeinterval02"].Value = txtboxtimeinterval02.Text; }
            if (ConfigurationManager.AppSettings["Timeinterval03"] == null) { config.AppSettings.Settings.Add("Timeinterval03", txtboxtimeinterval03.Text); }
            else { config.AppSettings.Settings["Timeinterval03"].Value = txtboxtimeinterval03.Text; }
            if (ConfigurationManager.AppSettings["Timeinterval04"] == null) { config.AppSettings.Settings.Add("Timeinterval04", txtboxtimeinterval04.Text); }
            else { config.AppSettings.Settings["Timeinterval04"].Value = txtboxtimeinterval04.Text; }


            if (ConfigurationManager.AppSettings["Reverse01"] == null) { config.AppSettings.Settings.Add("Reverse01", chkboxReverse01.Checked.ToString().ToLower()); }
            else { config.AppSettings.Settings["Reverse01"].Value = chkboxReverse01.Checked.ToString().ToLower(); }
            if (ConfigurationManager.AppSettings["Reverse02"] == null) { config.AppSettings.Settings.Add("Reverse02", chkboxReverse02.Checked.ToString().ToLower()); }
            else { config.AppSettings.Settings["Reverse02"].Value = chkboxReverse02.Checked.ToString().ToLower(); }
            if (ConfigurationManager.AppSettings["Reverse03"] == null) { config.AppSettings.Settings.Add("Reverse03", chkboxReverse03.Checked.ToString().ToLower()); }
            else { config.AppSettings.Settings["Reverse03"].Value = chkboxReverse03.Checked.ToString().ToLower(); }
            if (ConfigurationManager.AppSettings["Reverse04"] == null) { config.AppSettings.Settings.Add("Reverse04", chkboxReverse04.Checked.ToString().ToLower()); }
            else { config.AppSettings.Settings["Reverse04"].Value = chkboxReverse04.Checked.ToString().ToLower(); }


            config.Save(ConfigurationSaveMode.Full);
            
            //deviceRun01.threadStop();
            //deviceRun02.threadStop();
            //deviceRun03.threadStop();
            //deviceRun04.threadStop();

            chek_device = false;
            Thread.Sleep(500);
            this.btnDeviceDisConnect.PerformClick();

            Environment.Exit(0);
            //Application.Exit();

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
 
        }

        private void numericUpDown01_ValueChanged(object sender, EventArgs e)
        {
            float tmp_attenuation = 0;

            if (LDA_Open)
            {
                tmp_attenuation = (float)numericUpDown01.Value;
                m_LabBrick.SetChannelNumber(m_LabBrick.MyDevices[0], Convert.ToInt32(txtboxchannelnumber01.Text));
                m_LabBrick.SetAttenuationInDb(m_LabBrick.MyDevices[0], tmp_attenuation);
            }
        }

        private void numericUpDown02_ValueChanged(object sender, EventArgs e)
        {
            float tmp_attenuation = 0;

            if (LDA_Open)
            {
                tmp_attenuation = (float)numericUpDown02.Value;
                m_LabBrick.SetChannelNumber(m_LabBrick.MyDevices[0], Convert.ToInt32(txtboxchannelnumber02.Text));
                m_LabBrick.SetAttenuationInDb(m_LabBrick.MyDevices[0], tmp_attenuation);
            }
        }

        private void numericUpDown03_ValueChanged(object sender, EventArgs e)
        {
            float tmp_attenuation = 0;

            if (LDA_Open)
            {
                tmp_attenuation = (float)numericUpDown03.Value;
                m_LabBrick.SetChannelNumber(m_LabBrick.MyDevices[0], Convert.ToInt32(txtboxchannelnumber03.Text));
                m_LabBrick.SetAttenuationInDb(m_LabBrick.MyDevices[0], tmp_attenuation);
            }
        }

        private void numericUpDown04_ValueChanged(object sender, EventArgs e)
        {
            float tmp_attenuation = 0;

            if (LDA_Open)
            {
                tmp_attenuation = (float)numericUpDown04.Value;
                m_LabBrick.SetChannelNumber(m_LabBrick.MyDevices[0], Convert.ToInt32(txtboxchannelnumber04.Text));
                m_LabBrick.SetAttenuationInDb(m_LabBrick.MyDevices[0], tmp_attenuation);
            }
        }

        private void btnreset_Click(object sender, EventArgs e)
        {
            this.txtboxstartdb01.Text = "10";
            this.txtboxstartdb02.Text = "10";
            this.txtboxstartdb03.Text = "10";
            this.txtboxstartdb04.Text = "10";

            this.txtboxstopdb01.Text = "30";
            this.txtboxstopdb02.Text = "30";
            this.txtboxstopdb03.Text = "30";
            this.txtboxstopdb04.Text = "30";

            this.txtboxstepdb01.Text = "1";
            this.txtboxstepdb02.Text = "1";
            this.txtboxstepdb03.Text = "1";
            this.txtboxstepdb04.Text = "1";

            this.txtboxtimeinterval01.Text = "1000";
            this.txtboxtimeinterval02.Text = "1000";
            this.txtboxtimeinterval03.Text = "1000";
            this.txtboxtimeinterval04.Text = "1000";

            this.chkboxReverse01.Checked = false;
            this.chkboxReverse02.Checked = false;
            this.chkboxReverse03.Checked = false;
            this.chkboxReverse04.Checked = false;

        }
    }
}
