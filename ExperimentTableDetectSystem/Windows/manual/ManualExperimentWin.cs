using ExperimentTableDetectSystem.service;
using ExperimentTableDetectSystem.util;
using ExperimentTableDetectSystem.Windows.auto;
using ExperimentTableDetectSystem.Windows.manual;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;


namespace ExperimentTableDetectSystem.Windows
{
    public partial class ManualExperimentWin : MetroFramework.Forms.MetroForm
    {
        #region 字段
        private string valveid;

        public string Valveid
        {
            get
            {
                return valveid;
            }

            set
            {
                valveid = value;
            }
        }

        public string Company
        {
            get
            {
                return company;
            }

            set
            {
                company = value;
            }
        }



        double[] nowAlldata = new double[59];
        private string company;
        DBHelper dbhelper;
        PeakHelper peakHelperer;
        DataStoreManager dataStoreManager;
        ConfigManager config;
        RecordClickTime recordClickTime;

        #endregion

        #region singleton 
        private static ManualExperimentWin instance;
        public static ManualExperimentWin getInstance()
        {
            if (instance == null || instance.IsDisposed)
            {
                instance = new ManualExperimentWin();
            }
            return instance;
        }
        #endregion
        public ManualExperimentWin()
        {
            InitializeComponent();
            dbhelper = DBHelper.GetInstance();
            peakHelperer = PeakHelper.GetInstance();
            dataStoreManager = DataStoreManager.GetInstance();
            config = ConfigManager.GetInstance();
            recordClickTime = RecordClickTime.GetInstance();
        }
        #region 弹窗控制
        Boolean check56 = false;
        Boolean check74 = false;
        Boolean check63 = false;
        Boolean check28 = false;
        Boolean check71 = false;
        Boolean check72 = false;
        Boolean check64 = false;
        Boolean check29 = false;
        Boolean check30 = false;
        Boolean check31 = false;
        Boolean check57 = false;
        Boolean check75 = false;
        Boolean check58 = false;
        Boolean check70 = false;
        Boolean check76 = false;
        #endregion

        double mainPump1Pressure = 0;
        double mainPump2Pressure = 0;
        double pump1Flow = 0;
        double pump2Flow = 0;
        double steerPressure = 0;
        double leakageFlow = 0;


        private void ManualExperimentWin_Load(object sender, EventArgs e)
        {
            this.tableLayoutPanel1.BackColor = Color.FromArgb(255, 50, 161, 206);
            this.valveid = ManualNumberInput.id;
            this.company = ManualNumberInput.company;
            lblValveId.Font= new Font("宋体", 18);
            lblValveId.Text = "编号:" + this.valveid;
            txtinfo.Text = this.company;
            //获得配置项的值，赋予报警参数? ? ?
            timer1.Enabled = true;
            //    timer2.Enabled = false;
            peakHelperer.StartTimer(100);

            DataStoreManager.productId = this.valveid;
            DataStoreManager.n = ManualNumberInput.n;
            DataStoreManager.menjiaType = ManualNumberInput.menjiaType;
            DataStoreManager.valveType = ManualNumberInput.valveType;
            dataStoreManager.StartTimer1(100, 100);

            TPCANMsg canmsg108 = new TPCANMsg();
            canmsg108.ID = 0x108;
            canmsg108.LEN = Convert.ToByte(8);
            canmsg108.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_STANDARD;
            canmsg108.DATA = new byte[8];
            canmsg108.DATA[4] = 1;

            try
            {
                TPCANStatus sts3 = peakHelperer.write(canmsg108);
                if (sts3 == TPCANStatus.PCAN_ERROR_OK)
                {
                   // MessageBox.Show("手动实验开始" );
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("手动实验开始信号发送失败" + ex.Message);
            }

        }

 
        private void timer1_Tick(object sender, EventArgs e)
        {
            refreshData();
        }


        int count = 0;
        double[] pump1 = new double[5];
        double[] pump2 = new double[5];
        double[] steerPump = new double[5];
        double[] showdata = new double[77];
        private void refreshData()
        {
            for (int i = 0; i < 77; i++)
            {
                showdata[i] = peakHelperer.AllValue[i];
            }
            mainPump1Pressure = (pump1[0]+ pump1[1]+ pump1[2]+ pump1[3]+ pump1[4])/5;
            mainPump2Pressure = (pump2[0] + pump2[1] + pump2[2] + pump2[3] + pump2[4]) / 5;
            pump1Flow = showdata[36];
            pump2Flow = showdata[37];
            steerPressure = (steerPump[0] + steerPump[1] + steerPump[2] + steerPump[3] + steerPump[4]) / 5;
            leakageFlow = showdata[39];

            pump1[count] = showdata[10];
            pump2[count] = showdata[11];
            steerPump[count] = showdata[12];

        
            if (count < 4)
            {
                count++;
            }
            else
            {
                count = 0;
            }

            txttankTemp.Text = showdata[44].ToString();
            txtpump1OutTemp.Text = showdata[45].ToString();
            txtpump2OutTemp.Text = showdata[46].ToString();

            txtmainPumpP1.Text = mainPump1Pressure.ToString();
            txtMainPumpP2.Text = mainPump2Pressure.ToString();
            txtpumpFlow1.Text = showdata[36].ToString();
            txtpumpFlow2.Text = showdata[37].ToString();
            txtSteerPressure.Text = steerPressure.ToString();
            txtsteeringFlow.Text = showdata[38].ToString();
            txtLeakageFlow.Text = showdata[39].ToString();
            txtbackFlow.Text = showdata[41].ToString();

            //showData[56] = 0就没在进行这个实验
            //showData[56] = 1就正在进行这个实验
            if (showdata[56] == 1)
            {
                txtTestCourse.Text = "转向溢流阀调定";
                txtsteeringFlow.Text  = "0";
            }
            else
            {
                txtsteeringFlow.Text = showdata[38].ToString();
            }
            if (showdata[56] == 2 && ManualSelectWin.steer==1)
            {
                //showData[56] = 2 到达设定压力
                if (!check56)
                {
                    check56 = true;
                    DialogResult _dr = MessageBox.Show("请微调转向溢流阀压力，确认后点击按钮继续", "提示", MessageBoxButtons.OKCancel);
                    if (_dr == DialogResult.OK) {
                        recordClickTime.setSteerOverFlowCLickTime(DateTime.Now);
                        Console.WriteLine("转向溢流阀压力调定ok:"+ recordClickTime.getSteerOverFlowCLickTime());
                        Thread.Sleep(1000);//延时1秒 记录数据
                        DialogResult dr = MessageBox.Show("转向溢流阀调定完成，压力调低后点击确定进入启闭特性测试", "提示", MessageBoxButtons.OKCancel);
                        if (dr == DialogResult.OK)
                        {
                            #region 0x113
                            TPCANMsg canmsg113 = new TPCANMsg();
                            canmsg113.ID = 0x113;
                            canmsg113.LEN = Convert.ToByte(8);
                            canmsg113.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_STANDARD;
                            canmsg113.DATA = new byte[8];
                            canmsg113.DATA[7] = 1;
                            TPCANStatus res1 = peakHelperer.write(canmsg113);
                            #endregion
                        }
                    }
                }
            }

            if (showdata[74] == 1)
            {
                txtTestCourse.Text = "转向阀启闭特性试验";

            }
            if (showdata[74] == 2 && ManualSelectWin.steer == 1)
            {
                if (!check74) {
                    check74 = true;
                    DialogResult dr = MessageBox.Show("转向阀启闭特性试验已做完，请将压力调高后点击确定进行下一步", "提示", MessageBoxButtons.OKCancel);
                    if (dr == DialogResult.OK)
                    {
                        #region 0x113
                        TPCANMsg canmsg113 = new TPCANMsg();
                        canmsg113.ID = 0x113;
                        canmsg113.LEN = Convert.ToByte(8);
                        canmsg113.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_STANDARD;
                        canmsg113.DATA = new byte[8];
                        canmsg113.DATA[0] = 1;
                        TPCANStatus res1 = peakHelperer.write(canmsg113);
                        #endregion
                    }
                }

            }


            if (showdata[63] == 1)
            {
                txtTestCourse.Text = "过载阀B3口调定试验";

            }
            if (showdata[63] == 2&&ManualSelectWin.overB3 == 1)
            {
            
                if (!check63)
                {
                    check63 = true;
                    DialogResult _dr = MessageBox.Show("请微调过载阀B3压力，确认后点击按钮继续", "提示", MessageBoxButtons.OKCancel);
                    if (_dr == DialogResult.OK) {
                        recordClickTime.setB3ClickTime(DateTime.Now);
                        Console.WriteLine("过载阀B3调定ok： " + recordClickTime.getB3ClickTime());
                        Thread.Sleep(1000);
                        DialogResult dr = MessageBox.Show("过载阀B3口调定完成，请将压力调低后点击确定进入启闭特性测试", "提示", MessageBoxButtons.OKCancel);
                        if (dr == DialogResult.OK)
                        {
                            #region 0x114
                            TPCANMsg canmsg114 = new TPCANMsg();
                            canmsg114.ID = 0x114;
                            canmsg114.LEN = Convert.ToByte(8);
                            canmsg114.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_STANDARD;
                            canmsg114.DATA = new byte[8];
                            canmsg114.DATA[0] = 1;
                            TPCANStatus res1 = peakHelperer.write(canmsg114);
                            #endregion
                        }
                    }
                }
            }

            if (showdata[28] == 1)
            {
                txtTestCourse.Text = "过载阀B3口启闭特性试验";

            }
            if (showdata[28] == 2 && ManualSelectWin.overB3 == 1)
            {
                if (!check28)
                {
                    check28 = true;
                     DialogResult dr = MessageBox.Show("过载阀B3口启闭特性试验已做完，请将压力调高后点击确定进入下一步", "提示", MessageBoxButtons.OKCancel);
                        if (dr == DialogResult.OK){
                            #region 0x113
                            TPCANMsg canmsg113 = new TPCANMsg();
                            canmsg113.ID = 0x113;
                            canmsg113.LEN = Convert.ToByte(8);
                            canmsg113.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_STANDARD;
                            canmsg113.DATA = new byte[8];
                            canmsg113.DATA[4] = 1;
                            TPCANStatus res1 = peakHelperer.write(canmsg113);
                            #endregion
                        }
                }
            }

            if (showdata[71] == 1)
            {
                txtTestCourse.Text = "过载阀A3口调定试验";
            }
            if (showdata[71] == 2 && ManualSelectWin.overA3 == 1)
            {
                if (!check71)
                {
                    check71 = true;
                    DialogResult _dr = MessageBox.Show("请微调过载阀A3压力，确认后点击按钮继续", "提示", MessageBoxButtons.OKCancel);
                    if (_dr == DialogResult.OK) {
                        recordClickTime.setA3ClickTime(DateTime.Now);
                        Console.WriteLine("过载阀A3调定ok："+recordClickTime.getA3ClickTime());
                        Thread.Sleep(1000);//延时1秒 记录数据
                        DialogResult dr = MessageBox.Show("过载阀A3口调定完成，请将压力调低后点击确定进入启闭特性测试", "提示", MessageBoxButtons.OKCancel);
                        if (dr == DialogResult.OK)
                        {
                            #region 0x114
                            TPCANMsg canmsg114 = new TPCANMsg();
                            canmsg114.ID = 0x114;
                            canmsg114.LEN = Convert.ToByte(8);
                            canmsg114.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_STANDARD;
                            canmsg114.DATA = new byte[8];
                            canmsg114.DATA[1] = 1;
                            TPCANStatus res1 = peakHelperer.write(canmsg114);
                            #endregion
                        }
                    }
                    
                }
            }

            if (showdata[29] == 1)
            {
                txtTestCourse.Text = "过载阀A3口启闭特性试验";

            }
            if (showdata[29] == 2 && ManualSelectWin.overA3 == 1)
            {
                if (!check29)
                {
                    check29 = true;
                    DialogResult dr = MessageBox.Show("过载阀A3口启闭特性试验已做完，请将压力调高后点击确定进入下一步", "提示", MessageBoxButtons.OKCancel);
                    if (dr == DialogResult.OK)
                    {
                        #region 0x113
                        TPCANMsg canmsg113 = new TPCANMsg();
                        canmsg113.ID = 0x113;
                        canmsg113.LEN = Convert.ToByte(8);
                        canmsg113.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_STANDARD;
                        canmsg113.DATA = new byte[8];
                        canmsg113.DATA[1] = 1;
                        TPCANStatus res1 = peakHelperer.write(canmsg113);
                        #endregion
                    }
                }
            }

            if (showdata[64] == 1)
            {
                txtTestCourse.Text = "过载阀B4口调定试验";
            }
            if (showdata[64] == 2 && ManualSelectWin.overB4 == 1)
            {
                if (!check64)
                {
                    check64 = true;
                    DialogResult _dr = MessageBox.Show("请微调过载阀B4压力，确认后点击按钮继续", "提示", MessageBoxButtons.OKCancel);
                    if (_dr == DialogResult.OK) {
                        recordClickTime.setB4ClickTime(DateTime.Now);
                        Console.WriteLine("过载阀B4调定ok：" + recordClickTime.getB4ClickTime());
                        Thread.Sleep(1000);//延时1秒 记录数据
                        DialogResult dr = MessageBox.Show("过载阀B4口调定完成，请将压力调低后点击确定进入启闭特性测试", "提示", MessageBoxButtons.OKCancel);
                        if (dr == DialogResult.OK)
                        {
                            #region 0x114
                            TPCANMsg canmsg114 = new TPCANMsg();
                            canmsg114.ID = 0x114;
                            canmsg114.LEN = Convert.ToByte(8);
                            canmsg114.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_STANDARD;
                            canmsg114.DATA = new byte[8];
                            canmsg114.DATA[2] = 1;
                            TPCANStatus res1 = peakHelperer.write(canmsg114);
                            #endregion
                        }
                    }                    
                }
            }

            if (showdata[30] == 1)
            {
                txtTestCourse.Text = "过载阀B4口启闭特性试验";
            }
            if (showdata[30] == 2 && ManualSelectWin.overB4 == 1)
            {
                if (!check30)
                {
                    check30 = true;
                    DialogResult dr = MessageBox.Show("过载阀B4口启闭特性试验已做完，请将压力调高后点击确定进入下一步", "提示", MessageBoxButtons.OKCancel);
                    if (dr == DialogResult.OK)
                    {
                        #region 0x113
                        TPCANMsg canmsg113 = new TPCANMsg();
                        canmsg113.ID = 0x113;
                        canmsg113.LEN = Convert.ToByte(8);
                        canmsg113.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_STANDARD;
                        canmsg113.DATA = new byte[8];
                        canmsg113.DATA[5] = 1;
                        TPCANStatus res1 = peakHelperer.write(canmsg113);
                        #endregion
                    }
                }
            }

            if (showdata[72] == 1)
            {
                txtTestCourse.Text = "过载阀A4口调定试验";
            }
            if (showdata[72] == 2 && ManualSelectWin.overA4 == 1)
            {
                if (!check72){
                    check72 = true;
                    DialogResult _dr = MessageBox.Show("请微调过载阀A4压力，确认后点击按钮继续", "提示", MessageBoxButtons.OKCancel);
                    if (_dr == DialogResult.OK) {
                        recordClickTime.setA4ClickTime(DateTime.Now);
                        Console.WriteLine("过载阀A4调定ok：" + recordClickTime.getA4ClickTime());

                        Thread.Sleep(1000);//延时1秒 记录数据
                        DialogResult dr = MessageBox.Show("过载阀A4口调定完成，请将压力调低后点击确定进入启闭特性测试", "提示", MessageBoxButtons.OKCancel);
                        if (dr == DialogResult.OK)
                        {
                            #region 0x114
                            TPCANMsg canmsg114 = new TPCANMsg();
                            canmsg114.ID = 0x114;
                            canmsg114.LEN = Convert.ToByte(8);
                            canmsg114.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_STANDARD;
                            canmsg114.DATA = new byte[8];
                            canmsg114.DATA[3] = 1;
                            TPCANStatus res1 = peakHelperer.write(canmsg114);
                            #endregion
                        }

                    }
                    
                }
            }


            if (showdata[31] == 1)
            {
                txtTestCourse.Text = "过载阀A4口启闭特性试验";

            }
            if (showdata[31] == 2 && ManualSelectWin.overA4 == 1)
            {
               
                if (!check31)
                {
                    check31 = true;
                    DialogResult dr = MessageBox.Show("过载阀A4口启闭特性试验已做完，请将压力调高后点击确定进入下一步", "提示", MessageBoxButtons.OKCancel);
                    if (dr == DialogResult.OK)
                    {
                        #region 0x113
                        TPCANMsg canmsg113 = new TPCANMsg();
                        canmsg113.ID = 0x113;
                        canmsg113.LEN = Convert.ToByte(8);
                        canmsg113.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_STANDARD;
                        canmsg113.DATA = new byte[8];
                        canmsg113.DATA[2] = 1;
                        TPCANStatus res1 = peakHelperer.write(canmsg113);
                        #endregion
                    }
                }
            }


            if (showdata[57] == 1)
            {
                txtTestCourse.Text = "主溢流阀调定";

            }
            if (showdata[57] == 2 && ManualSelectWin.main == 1)
            {
                if (!check57)
                {
                    check57 = true;
                    DialogResult _dr = MessageBox.Show("请微调主溢流阀压力，确认后点击按钮继续", "提示", MessageBoxButtons.OKCancel);
                    if (_dr == DialogResult.OK) {
                        recordClickTime.setMainOverFlowCLickTime(DateTime.Now);
                        Console.WriteLine("主溢流阀调定ok：" + recordClickTime.getMainOverFlowCLickTime());
                        Thread.Sleep(1000);//延时1秒 记录数据
                        DialogResult dr = MessageBox.Show("主溢流阀调定完成，请将压力调低后点击确定进入启闭特性测试", "提示", MessageBoxButtons.OKCancel);
                        if (dr == DialogResult.OK)
                        {
                            #region 0x114
                            TPCANMsg canmsg114 = new TPCANMsg();
                            canmsg114.ID = 0x114;
                            canmsg114.LEN = Convert.ToByte(8);
                            canmsg114.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_STANDARD;
                            canmsg114.DATA = new byte[8];
                            canmsg114.DATA[4] = 1;
                            TPCANStatus res1 = peakHelperer.write(canmsg114);
                            #endregion
                        }
                    }
                    
                }
            }


            if (showdata[75] == 1)
            {
                txtTestCourse.Text = "主溢流阀启闭特性试验";

            }
            if (showdata[75] == 2 && ManualSelectWin.main == 1)
            {
                if (!check75)
                {
                    check75 = true;
                    DialogResult dr = MessageBox.Show("主溢流阀启闭特性试验已做完，请将压力调高后点击确定进入下一步", "提示", MessageBoxButtons.OKCancel);
                    if (dr == DialogResult.OK)
                    {
                        #region 0x113
                        TPCANMsg canmsg113 = new TPCANMsg();
                        canmsg113.ID = 0x113;
                        canmsg113.LEN = Convert.ToByte(8);
                        canmsg113.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_STANDARD;
                        canmsg113.DATA = new byte[8];
                        canmsg113.DATA[3] = 1;
                        TPCANStatus res1 = peakHelperer.write(canmsg113);
                        #endregion
                        if (ManualSelectWin.change == 1)
                        {
                            txtTestCourse.Text = "换向泄漏压力调定过程";
                        }
                        txtTestCourse.ForeColor = Color.Red;
                    }
                }
            }

            if (showdata[58] == 1) {
                txtTestCourse.Text = "换向压力调定试验";
            }
            if (showdata[58] == 2 && ManualSelectWin.change == 1)
            {
                if (!check58)
                {
                    check58 = true;
                    DialogResult _dr = MessageBox.Show("请微调换向压力，确认后点击按钮继续", "提示", MessageBoxButtons.OKCancel);
                    if (_dr == DialogResult.OK) {
                        recordClickTime.setChangeDirOverFlowCLickTime(DateTime.Now);
                        Console.WriteLine("换向压力调定ok：" + recordClickTime.getChangeDirOverFlowCLickTime());

                        Thread.Sleep(1000);//延时1秒 记录数据
                        DialogResult dr = MessageBox.Show("换向压力调定完成后，点击确定进行下一步", "提示", MessageBoxButtons.OKCancel);
                        if (dr == DialogResult.OK)
                        {
                            #region 0x113
                            TPCANMsg canmsg113 = new TPCANMsg();
                            canmsg113.ID = 0x113;
                            canmsg113.LEN = Convert.ToByte(8);
                            canmsg113.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_STANDARD;
                            canmsg113.DATA = new byte[8];
                            canmsg113.DATA[6] = 1;
                            TPCANStatus res1 = peakHelperer.write(canmsg113);
                            #endregion
                        }
                    }
                   
                }
            }

            
            if (showdata[73] == 1)
            {
                txtTestCourse.Text = "换向泄漏上升口试验";
                txtTestCourse.ForeColor = Color.Black;
            }
            if (showdata[65] == 1)
            {
                txtTestCourse.Text = "换向泄漏前倾口试验";
                txtTestCourse.ForeColor = Color.Black;

            }
            if (showdata[67] == 1)
            {
                txtTestCourse.Text = "换向泄漏B3口试验";
                txtTestCourse.ForeColor = Color.Black;

            }
            if (showdata[68] == 1)
            {
                txtTestCourse.Text = "换向泄漏A3口试验";
                txtTestCourse.ForeColor = Color.Black;

            }
            if (showdata[69] == 1)
            {
                txtTestCourse.Text = "换向泄漏B4口试验";
                txtTestCourse.ForeColor = Color.Black;

            }
            if (showdata[70] == 1)
            {
       
                txtTestCourse.Text = "换向泄漏A4口试验";
                txtTestCourse.ForeColor = Color.Black;

            }
            if (showdata[66] == 1)
            {
                txtTestCourse.Text = "换向泄漏后倾口试验";
                txtTestCourse.ForeColor = Color.Black;

            }
            
            if (showdata[70] == 2)
                //手动实验部分结束
            {
                if (!check70)
                {
                    check70 = true;
                    DialogResult finaldr = MessageBox.Show("请调高系统主溢流阀压力回到核定压力，点击确定进行下一步", "提示", MessageBoxButtons.OKCancel);
                    if (finaldr == DialogResult.OK)
                    {
                        //发送消息给下位机
                        //
                        TPCANMsg canmsg115 = new TPCANMsg();
                        canmsg115.ID = 0x115;
                        canmsg115.LEN = Convert.ToByte(8);
                        canmsg115.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_STANDARD;
                        canmsg115.DATA = new byte[8];
                        canmsg115.DATA[0] = 1;
                        peakHelperer.write(canmsg115);

                    }
               
                 }

                //下位机手动实验卸荷结束之后 才关闭手动实验
                if (showdata[76] == 2)
                {
                    timer1.Enabled = false;
                    //txtTestCourse.Text = "换向泄漏压力调定过程";
                    

                    TPCANMsg canmsg115 = new TPCANMsg();
                    canmsg115.ID = 0x115;
                    canmsg115.LEN = Convert.ToByte(8);
                    canmsg115.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_STANDARD;
                    canmsg115.DATA = new byte[8];
                    canmsg115.DATA[0] = 0;
                    peakHelperer.write(canmsg115);

                    #region 0x113
                    TPCANMsg canmsg113 = new TPCANMsg();
                    canmsg113.ID = 0x113;
                    canmsg113.LEN = Convert.ToByte(8);
                    canmsg113.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_STANDARD;
                    canmsg113.DATA = new byte[8];
                    for (int i = 0; i < 8; i++)
                    {
                        canmsg113.DATA[i] = 0;
                    }
                    TPCANStatus res1 = peakHelperer.write(canmsg113);
                    #endregion
                    #region 0x114
                    TPCANMsg canmsg114 = new TPCANMsg();
                    canmsg114.ID = 0x114;
                    canmsg114.LEN = Convert.ToByte(8);
                    canmsg114.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_STANDARD;
                    canmsg114.DATA = new byte[8];
                    for (int i = 0; i < 8; i++)
                    {
                        canmsg114.DATA[i] = 0;
                    }
                    #endregion
                    TPCANStatus res2 = peakHelperer.write(canmsg114);
                    if(!check76) {
                        check76 = true;
                        DialogResult dr = MessageBox.Show("手动试验已做完,下面进行自动测试。");
                        if (dr == DialogResult.OK)
                        {
                            ManualEnd();
                            TPCANMsg canmsg108 = new TPCANMsg();
                            canmsg108.ID = 0x108;
                            canmsg108.LEN = Convert.ToByte(8);
                            canmsg108.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_STANDARD;
                            canmsg108.DATA = new byte[8];
                            canmsg108.DATA[4] = 0;
                            canmsg108.DATA[5] = 1;
                            peakHelperer.write(canmsg108);
                            Thread.Sleep(500);

                            canmsg108.DATA[4] = 0;
                            canmsg108.DATA[5] = 0;
                            peakHelperer.write(canmsg108);
                            canmsg115.DATA[0] = 0;
                            peakHelperer.write(canmsg115);
                            this.Close();
                            AutoExperimentWin win = AutoExperimentWin.getInstance();
                            win.Show();
                        }
                    }
                    
                }


            }
        }

        private void ManualExperimentWin_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (dataStoreManager != null)
            {
                dataStoreManager.StopTimer1();
            }
            if (peakHelperer != null)
            {
                peakHelperer.StopTimer();
            }
            

        }


        private void ManualEnd()
          {
            TPCANMsg canmsg108 = new TPCANMsg();
            canmsg108.ID = 0x108;
            canmsg108.LEN = Convert.ToByte(8);
            canmsg108.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_STANDARD;
            canmsg108.DATA = new byte[8];
            canmsg108.DATA[4] = 0;
            canmsg108.DATA[5] = 0;

            try
            {
                TPCANStatus sts3 = peakHelperer.write(canmsg108);
                if (sts3 == TPCANStatus.PCAN_ERROR_OK)
                {
                   // MessageBox.Show("手动实验结束");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("手动实验结束信号发送失败" + ex.Message);
            }

        }


        private void timer2_Tick(object sender, EventArgs e)
        {
            double[] alarmdata = new double[6];
            alarmdata[0] = peakHelperer.AllValue[53];
            alarmdata[1] = peakHelperer.AllValue[54];
            alarmdata[2] = peakHelperer.AllValue[55];
            alarmdata[3] = peakHelperer.AllValue[32];
            alarmdata[4] = peakHelperer.AllValue[33];
            alarmdata[5] = peakHelperer.AllValue[34];
            string name = UserRightManager.user.userName;//得到操作人的名字
            if (alarmdata[0] == 1)
            {
                timer2.Enabled = false;
                DialogResult dr = MessageBox.Show("注意！主泵1压力已超限，请及时处理", "", MessageBoxButtons.OKCancel);
                if (dr == DialogResult.OK)
                {
                    timer2.Enabled = true;
                }
                string sql = string.Format("insert into Alarm values ('{0}','{1}','{2}')", name, DateTime.Now, "主泵1压力超限");
                dbhelper.ExecuteNonQuery(sql);
            }
            if (alarmdata[1] == 1)
            {
                timer2.Enabled = false;
                DialogResult dr = MessageBox.Show("注意！主泵2压力已超限，请及时处理", "", MessageBoxButtons.OKCancel);
                if (dr == DialogResult.OK)
                {
                    timer2.Enabled = true;
                }
                string sql = string.Format("insert into Alarm values ('{0}','{1}','{2}')", name, DateTime.Now, "主泵2压力超限");
                dbhelper.ExecuteNonQuery(sql);
            }
            if (alarmdata[2] == 1)
            {
                timer2.Enabled = false;
                DialogResult dr = MessageBox.Show("注意！内泄漏流量已超限，请及时处理", "", MessageBoxButtons.OKCancel);
                if (dr == DialogResult.OK)
                {
                    timer2.Enabled = true;
                }
                string sql = string.Format("insert into Alarm values ('{0}','{1}','{2}')", name, DateTime.Now, "内泄漏流量超限");
                dbhelper.ExecuteNonQuery(sql);
            }
            if (alarmdata[3] == 1)
            {
                timer2.Enabled = false;
                DialogResult dr = MessageBox.Show("注意！滤油器1报警，请及时处理", "", MessageBoxButtons.OKCancel);
                if (dr == DialogResult.OK)
                {
                    timer2.Enabled = true;
                }
                string sql = string.Format("insert into Alarm values ('{0}','{1}','{2}')", name, DateTime.Now, "滤油器1报警");
                dbhelper.ExecuteNonQuery(sql);
            }
            if (alarmdata[4] == 1)
            {
                timer2.Enabled = false;
                DialogResult dr = MessageBox.Show("注意！滤油器2报警，请及时处理", "", MessageBoxButtons.OKCancel);
                if (dr == DialogResult.OK)
                {
                    timer2.Enabled = true;
                }
                string sql = string.Format("insert into Alarm values ('{0}','{1}','{2}')", name, DateTime.Now, "滤油器2报警");
                dbhelper.ExecuteNonQuery(sql);
            }
            if (alarmdata[5] == 1)
            {
                timer2.Enabled = false;
                DialogResult dr = MessageBox.Show("注意！滤油器3报警，请及时处理", "", MessageBoxButtons.OKCancel);
                if (dr == DialogResult.OK)
                {
                    timer2.Enabled = true;
                }
                string sql = string.Format("insert into Alarm values ('{0}','{1}','{2}')", name, DateTime.Now, "滤油器3报警");
                dbhelper.ExecuteNonQuery(sql);
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            TPCANMsg canmsg108 = new TPCANMsg();
            canmsg108.ID = 0x108;
            canmsg108.LEN = Convert.ToByte(8);
            canmsg108.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_STANDARD;
            canmsg108.DATA = new byte[8];
            canmsg108.DATA[7] = 1;//中止试验

            #region 0x113
            TPCANMsg canmsg113 = new TPCANMsg();
            canmsg113.ID = 0x113;
            canmsg113.LEN = Convert.ToByte(8);
            canmsg113.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_STANDARD;
            canmsg113.DATA = new byte[8];
            for (int i = 0; i < 8; i++)
            {
                canmsg113.DATA[i] = 0;
            }
            #endregion
            #region 0x114
            TPCANMsg canmsg114 = new TPCANMsg();
            canmsg114.ID = 0x114;
            canmsg114.LEN = Convert.ToByte(8);
            canmsg114.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_STANDARD;
            canmsg114.DATA = new byte[8];
            for (int i = 0; i < 8; i++)
            {
                canmsg114.DATA[i] = 0;
            }
            #endregion

            TPCANMsg canmsg112 = new TPCANMsg();
            canmsg112.ID = 0x112;
            canmsg112.LEN = Convert.ToByte(8);
            canmsg112.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_STANDARD;
            canmsg112.DATA = new byte[8];
            for (int i = 0; i < 8; i++)
            {
                canmsg112.DATA[i] = 0;
            }

            try
            {
                TPCANStatus sts3 = peakHelperer.write(canmsg108);
                TPCANStatus sts4 = peakHelperer.write(canmsg112);
                TPCANStatus res1 = peakHelperer.write(canmsg113);
                TPCANStatus res2 = peakHelperer.write(canmsg114);
                if (sts3 == TPCANStatus.PCAN_ERROR_OK && sts4 == TPCANStatus.PCAN_ERROR_OK && res1 == TPCANStatus.PCAN_ERROR_OK && res2 == TPCANStatus.PCAN_ERROR_OK)
                {
                    MessageBox.Show("试验已被成功中止");
                    this.Close();


                }
                TestFinishedWin win = new TestFinishedWin();
                win.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("试验中止失败" + ex.Message);
            }
        }
 
    }
}
