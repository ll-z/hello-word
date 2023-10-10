using DemonForWorldZhu.OOP_Object_Oriented_Programming_.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DemonForWorldZhu.OOP_Object_Oriented_Programming_
{
    public partial class OOPForm : Form
    {
        public OOPForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ClassDog dog = new ClassDog();

            textBox1.AppendText(string.Format("{0}\r\n", dog.Jiao()));
            textBox1.AppendText(string.Format("{0}\r\n", dog.Chi()));

            IJiao dog2 = new ClassDog();
            textBox1.AppendText(string.Format("{0}\r\n", dog2.Jiao()));
            //Console.ReadKey();
        }
    }
}
