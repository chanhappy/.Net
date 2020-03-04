using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TransferValuesBetweenForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            var f2 = new Form2(ShowData);
            f2.Show();
        }

        /// <summary>
        /// 接收Form2传来的数据，赋值给Label1
        /// </summary>
        /// <param name="str">接收Form2传来的数据</param>
        void ShowData(string str)
        {
            label1.Text = str;
        }
    }
}
