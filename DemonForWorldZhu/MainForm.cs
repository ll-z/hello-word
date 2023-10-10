using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DemonForWorldZhu
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            DynamicAttribute.Main1();
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)  //单击鼠标左键才响应
            {
                if (e.Node.Level == 1)                               //判断子节点才响应
                {
                    textBox1.Text = e.Node.Text;                    //文件框中显示鼠标点击的节点名称
                }
            }

        }
    }
}
