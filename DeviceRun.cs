using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LabBrickAttTest
{
    class DeviceRun
    {
        long startdb;
        long stopdb;
        float stepdb;
        long timeinterval;

        Thread thr;

        private float g_atten = 0;

        private bool gorun = false;
        public bool m_checked = false;

        private Form1 form;
        NumericUpDown numericupdown;
        TextBox txtboxstartdb;
        TextBox txtboxstopdb;
        TextBox channelnumber;
        TextBox txtboxtimeinterval;
        Button btnstart;
        Button btnstop;

        public DeviceRun(Form1 form, NumericUpDown numericupdown,  TextBox txtboxstartdb, TextBox txtboxstopdb,TextBox channelnumber,TextBox txtboxtimeinterval,Button btnstart, Button btnstop)
        {
            this.form = form;
            this.numericupdown = numericupdown;
            this.txtboxstartdb = txtboxstartdb;
            this.txtboxstopdb = txtboxstopdb;
            this.channelnumber = channelnumber;
            this.txtboxtimeinterval = txtboxtimeinterval;
            this.btnstart = btnstart;
            this.btnstop = btnstop;
        }

        public void setParam(string mstartdb, string mstopdb, string mstepdb, string mtimeinterval)
        {
            startdb = Convert.ToInt64(mstartdb);
            stopdb = Convert.ToInt64(mstopdb);
            stepdb = (float)Convert.ToDouble(mstepdb);
            timeinterval = Convert.ToInt64(mtimeinterval);

        }

        public void threadRun()
        {
            try
            {
                if (startdb > 90 || startdb < 0)
                {
                    MessageBox.Show("StartdB 90 이하여야 , 0 이상이어야 합니다.", "Alert");
                    this.txtboxstartdb.Focus();
                    return;
                }

                if (stopdb > 90 || stopdb < 0)
                {
                    MessageBox.Show("Stopdb 90 이하여야, 0 이상이어야 합니다.", "Alert");
                    this.txtboxstopdb.Focus();
                    return;
                }

                if (startdb >= stopdb)
                {
                    MessageBox.Show("StartdB 가 작아야 합니다.", "Alert");
                    txtboxstartdb.Focus();
                    return;
                }

                g_atten = (float)startdb;


                if (gorun == false)
                {
                    gorun = true;
                    if (thr == null)
                    {
                        //form.m_LabBrick.SetChannelNumber(form.m_LabBrick.MyDevices[0], Convert.ToInt32(channelnumber.Text));
                        thr = new Thread(programStart);
                        thr.Start();
                    }
                    else
                    {
                        threadStop();
                        //form.m_LabBrick.SetChannelNumber(form.m_LabBrick.MyDevices[0], Convert.ToInt32(channelnumber.Text));
                        thr = new Thread(programStart);
                        thr.Start();

                    }
                }
                btnstart.Enabled = false;
            }catch(Exception e)
            {
                btnstart.Invoke((MethodInvoker)delegate ()
                {
                    btnstart.Enabled = true;
                });
                threadStop();
                
                //form.txtboxmessage.Invoke((MethodInvoker)delegate ()
                //{
                //    form.txtboxmessage.Text = form.txtboxmessage.Text + e.ToString() + System.Environment.NewLine;
                //});
            }

        }

        public void programStart()
        {
            try
            {
                if (gorun)
                {
                    bool isReverse = false;
                    while (gorun)
                    {
                        try
                        {
                            if (form.LDA_Open)
                            {

                                if (m_checked == false)
                                {
                                    txtboxstopdb.Invoke((MethodInvoker)delegate ()
                                    {

                                        if (g_atten < (float)stopdb)
                                        {
                                            g_atten = g_atten + (float)stepdb;
                                        }
                                        else
                                        {
                                            g_atten = (float)startdb;
                                        }
                                    });
                                    form.m_LabBrick.SetChannelNumber(form.m_LabBrick.MyDevices[0], Convert.ToInt32(channelnumber.Text));
                                    form.m_LabBrick.SetAttenuationInDb(form.m_LabBrick.MyDevices[0], (float)g_atten);
                                    //m_LabBrick.SetAttenuationInDb(m_LabBrick.MyDevices[0], tmp_attenuation);

                                    numericupdown.Invoke((MethodInvoker)delegate ()
                                    {
                                        // initialize our attenuation control with the existing device setting
                                        //numericupdown.Value = (decimal)form.m_LabBrick.GetAttenuationHR(form.m_LabBrick.MyDevices[0]) / 20M;
                                        //numericupdown.Value = (decimal)form.m_LabBrick.GetAttenuationHR(form.m_LabBrick.MyDevices[0]) / 20M;
                                        numericupdown.Value = (decimal)form.m_LabBrick.GetAttenuationHR(form.m_LabBrick.MyDevices[0]) / 20M;
                                    });

                                    //Console.WriteLine(g_atten + " :: " + (decimal)form.m_LabBrick.GetAttenuationHR(form.m_LabBrick.MyDevices[0]) / 20M);

                                    //numericupdown.Value = (decimal)form.m_LabBrick.GetAttenuationHR(form.m_LabBrick.MyDevices[0]) / 20M;
                                    //numericUpDown1.Value = (decimal)m_LabBrick.GetAttenuationHR(m_LabBrick.MyDevices[0]) / 20M;

                                    int interVal = 0;
                                    txtboxtimeinterval.Invoke((MethodInvoker)delegate ()
                                    {
                                        interVal = Convert.ToInt32(timeinterval);
                                    });
                                    //Console.WriteLine(interVal + "," + g_atten + "," + numericupdown.Value);
                                    Thread.Sleep(interVal);
                                }//if (m_checked == false)
                                else if (m_checked == true)
                                {

                                    txtboxstopdb.Invoke((MethodInvoker)delegate ()
                                    {

                                        if (isReverse == false)
                                        {

                                            if (g_atten < (float)Convert.ToInt64(stopdb))
                                            {
                                                g_atten = g_atten + (float)Convert.ToDouble(stepdb);
                                            }
                                            else
                                            {
                                            //g_atten = (float)Convert.ToInt64(txtboxstartdb.Text);
                                            isReverse = true;
                                            }

                                        }
                                        if (isReverse == true)
                                        {

                                            if (g_atten > (float)Convert.ToInt64(startdb))
                                            {
                                                g_atten = g_atten - (float)Convert.ToDouble(stepdb);
                                            }
                                            else
                                            {
                                            //g_atten = (float)Convert.ToInt64(txtboxstartdb.Text);
                                            isReverse = false;
                                            }

                                        }

                                    });
                                    form.m_LabBrick.SetChannelNumber(form.m_LabBrick.MyDevices[0], Convert.ToInt32(channelnumber.Text));
                                    form.m_LabBrick.SetAttenuationInDb(form.m_LabBrick.MyDevices[0], g_atten);

                                    numericupdown.Invoke((MethodInvoker)delegate ()
                                    {
                                        // initialize our attenuation control with the existing device setting
                                        numericupdown.Value = (decimal)form.m_LabBrick.GetAttenuationHR(form.m_LabBrick.MyDevices[0]) / 20M;

                                    });

                                    int interVal = 0;
                                    txtboxtimeinterval.Invoke((MethodInvoker)delegate ()
                                    {
                                        interVal = Convert.ToInt32(txtboxtimeinterval.Text);
                                    //Console.WriteLine(interVal + "," + g_atten + "," + numericUpDown1.Value);

                                    });

                                    Thread.Sleep(interVal);
                                }//else if (m_checked == true)
                            }//if (LDA_Open)
                            else
                            {
                                //threadStop();
                                Thread.Sleep(1000);
                            }
                        }catch(Exception ex)
                        {
                            //threadStop();
                            Thread.Sleep(1000);
                        }
                    }//while
                }//if(gorun)
                else
                {
                    //return;
                }
            }catch(Exception ex)
            {
                threadStop();
            }
        }

        public void threadStop()
        {

            gorun = false;
            
            btnstart.Invoke((MethodInvoker)delegate ()
            {
                btnstart.Enabled = true;
            });

            if (thr != null)
            {
                thr.Abort();
            }
            thr = null;

        }

    }
}
