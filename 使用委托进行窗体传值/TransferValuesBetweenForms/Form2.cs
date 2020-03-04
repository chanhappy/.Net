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
    /// <summary>
    /// 给Form1的ShowData方法定义个委托类型
    /// </summary>
    public delegate void DataTransport(string str);

    public partial class Form2 : Form
    {
        /// <summary>
        /// 定义一个变量存储Form1的ShowData方法
        /// </summary>
        public DataTransport _dataTransport;

        public Form2(DataTransport dataTransport)
        {
            _dataTransport = dataTransport;
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            //将textBox1的数据传输给Form1
            _dataTransport(textBox1.Text);
        }
    }
}
