using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Window1 W1;
        public Window2 W2;
        public Window3 W3;
        public Window4 W4;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            W1 = new Window1();
            W2 = new Window2();
            W3 = new Window3();
            W4 = new Window4();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            W1.Show();
            groupBox1.Controls.Clear();
            groupBox1.Controls.Add(W1);
            textBox1.Text = "chuangkou1";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            W2.Show();
            groupBox1.Controls.Clear();
            groupBox1.Controls.Add(W2);
            textBox1.Text = "chuangkou2";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            W3.Show();
            groupBox1.Controls.Clear();
            groupBox1.Controls.Add(W3);
            textBox1.Text = "chuangkou3";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            W4.Show();
            groupBox1.Controls.Clear();
            groupBox1.Controls.Add(W4);
            textBox1.Text = "chuangkou4";
        }
    }
}
