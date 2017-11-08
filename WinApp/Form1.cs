using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Long5;
using Mars.Land;
using System.Configuration;


namespace WinApp
{
    public partial class Form1 : Form
    {
        private bool status = false;
        private ManageMod mm;
        private string cfgpath = "E:\\test\\FrameWork\\WinApp\\config";
        private string humandb;
        private string devicedb;
        private string dingweidb;
            



        public Form1()
        {

            InitializeComponent();
            Logging.setConfig();
            //cfgpath = ConfigurationManager.AppSettings["LocalHost"];

        }

        private void Start_Click(object sender, EventArgs e)
        {
            mm = new ManageMod();
            mm.SetCfgPath(cfgpath);
            if(status == false)
            {
                status = true;
                Runsts.Text = "Running";
                Start.Text = "Stop";
                mm.Start();
            }
            else
            {
                status = false;
                Runsts.Text = "Halt";
                Start.Text = "Strt";
            }
        }

        private void Testut_Click(object sender, EventArgs e)
        {
            //mt = new ManageMod();

            //mt.SetCfgPath("E:\\test\\FrameWork\\WinApp\\config\\Test\\Land");

            //mt.Start();
            int rlt = UnitTest.run();

            if (rlt == 0)
            {
                TestRlt.Text = "Pass ";
            }
            else
            {
                TestRlt.Text = "Fail " + rlt + "times";
            }

        }
    }
}
