using ExperimentTableDetectSystem.service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ExperimentTableDetectSystem.Windows.manual
{
    public partial class ManualSelectWin : MetroFramework.Forms.MetroForm
    {
        public static int steer = 0;
        public static int main = 0;
        public static int overB3 = 0;
        public static int overA3 = 0;
        public static int overB4 = 0;
        public static int overA4 = 0;
        public static int change = 0;
        #region singleton
        private static ManualSelectWin instance;
        //   private static object obj = new object();
        public static ManualSelectWin getInstance()
        {
            if (instance == null || instance.IsDisposed)
            { 
                instance = new ManualSelectWin();
            }
            return instance;
        }
        #endregion
        #region 字段
        PeakHelper peakHelper;

        #endregion
        public ManualSelectWin()
        {
            InitializeComponent();
        }

        private void btnStartTest_Click(object sender, EventArgs e)
        {
            select();
            ManualExperimentWin win = ManualExperimentWin.getInstance();
            win.Show();
            this.Close();
        }

        private void select()
        {
            #region 0x112
            TPCANMsg canmsg112 = new TPCANMsg();
            canmsg112.ID = 0x112;
            canmsg112.LEN = Convert.ToByte(8);
            canmsg112.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_STANDARD;
            canmsg112.DATA = new byte[8];

            if (chkSteerYiliu.Checked == true)
            {
                canmsg112.DATA[0] = 1;
                steer = 1;
            }
            else { 
                canmsg112.DATA[0] = 0; 
                steer = 0; 
            }


            if (chkMainYiliu.Checked == true)
            {
                canmsg112.DATA[1] = 1; 
                main = 1;
            }
            else { 
                canmsg112.DATA[1] = 0; 
                main = 0;
            }


            if (chkChangeLeakage.Checked == true)
            {
                canmsg112.DATA[2] = 1; 
                change = 1;
            }
            else { 
                canmsg112.DATA[2] = 0; 
                change =0; 
            }


            if (chkOverflowB3.Checked == true)
            {
                canmsg112.DATA[3] = 1; 
                overB3 = 1;
            }
            else { 
                canmsg112.DATA[3] = 0; 
                overB3 =0; 
            }


            if (chkOverflowA3.Checked == true)
            {
                canmsg112.DATA[4] = 1; 
                overA3 = 1;
            }
            else { 
                canmsg112.DATA[4] = 0; 
                overA3 = 0; 
            }


            if (chkOverflowB4.Checked == true)
            {
                canmsg112.DATA[5] = 1; 
                overB4 = 1;
            }
            else { 
                canmsg112.DATA[5] = 0; 
                overB4 = 0;
            }


            if (chkOverflowA4.Checked == true)
            {
                canmsg112.DATA[6] = 1; 
                overA4 = 1;
            }
            else { 
                canmsg112.DATA[6] = 0;
                overA4 = 0; 
            }



            #endregion
            TPCANStatus sts = peakHelper.write(canmsg112);
            if (sts == TPCANStatus.PCAN_ERROR_OK)
            {
                 DialogResult dr = MessageBox.Show("实验项目选择成功，即将开始手动实验");

            }
        }

        private void ManualSelectWin_Load(object sender, EventArgs e)
        {
            peakHelper = PeakHelper.GetInstance();
        }

    }
}
