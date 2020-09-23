using NPOI.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ExperimentTableDetectSystem.Windows.manual
{
    class RecordClickTime        
    {

        private volatile static RecordClickTime instance = null;

        public static RecordClickTime GetInstance() {
            if (instance == null) { 
                instance = new RecordClickTime();
            }
            return instance;
        }

        public RecordClickTime()
        {
            //初始化 避免出现查询数据库时的超出时间限制的错误
            //初始化的时间是调用getInstance的系统时间
            //在进行试验时会更新到真正的clickTime
            SteerOverFlowCLickTime = DateTime.Now;
            B3CLickTime = DateTime.Now;
            B4CLickTime = DateTime.Now;
            A3CLickTime = DateTime.Now;
            A4CLickTime = DateTime.Now;
            ChangeDirOverFlowCLickTime = DateTime.Now;
            MainOverFlowCLickTime = DateTime.Now;
    }


        private DateTime SteerOverFlowCLickTime;
        private DateTime B3CLickTime;
        private DateTime B4CLickTime;
        private DateTime A3CLickTime;
        private DateTime A4CLickTime;
        private DateTime ChangeDirOverFlowCLickTime;
        private DateTime MainOverFlowCLickTime;

        //转向溢流阀
        public void setSteerOverFlowCLickTime(DateTime datetime) {
            SteerOverFlowCLickTime = datetime;
        }
        public DateTime getSteerOverFlowCLickTime() {
            return SteerOverFlowCLickTime;
        }

        //B3
        public void setB3ClickTime(DateTime datetime)
        {
            B3CLickTime = datetime;
        }
        public DateTime getB3ClickTime()
        {
            return B3CLickTime;
        }


        //A3
        
        public void setA3ClickTime(DateTime datetime)
        {
            A3CLickTime = datetime;
        }
        public DateTime getA3ClickTime()
        {
            return A3CLickTime;
        }


        //B4
        
        public void setB4ClickTime(DateTime datetime)
        {
            B4CLickTime = datetime;
        }
        public DateTime getB4ClickTime()
        {
            return B4CLickTime;
        }

        //A4
        public void setA4ClickTime(DateTime datetime)
        {
            A4CLickTime = datetime;
        }
        public DateTime getA4ClickTime()
        {
            return A4CLickTime;
        }

        //主溢流阀
        public void setMainOverFlowCLickTime(DateTime datetime)
        {
            MainOverFlowCLickTime = datetime;
        }
        public DateTime getMainOverFlowCLickTime()
        {
            return MainOverFlowCLickTime;
        }


        //换向溢流阀
        public void setChangeDirOverFlowCLickTime(DateTime datetime)
        {
            ChangeDirOverFlowCLickTime = datetime;
        }
        public DateTime getChangeDirOverFlowCLickTime()
        {
            return ChangeDirOverFlowCLickTime;
        }

    }
}

