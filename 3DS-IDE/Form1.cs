using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _3DS_IDE
{
    public partial class Form1 : Form
    {
        void cmd(string command)
        {
            System.Diagnostics.Process.Start("CMD.exe", command);
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Text = "Integrity check";
            timer1.Stop();
            timer2.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Start();
            if (Directory.Exists(@"C:\3DS-IDE\devkitPro"))
            {
                this.pictureBox1.Image = _3DS_IDE.Properties.Resources.check_icon;
            }
            else
            {
                this.pictureBox1.Image = _3DS_IDE.Properties.Resources.Error_icon;
            }
            if (Directory.Exists(@"C:\3DS-IDE\devkitPro\devkitARM"))
            {
                this.pictureBox4.Image = _3DS_IDE.Properties.Resources.check_icon;
            }
            else
            {
                this.pictureBox4.Image = _3DS_IDE.Properties.Resources.Error_icon;
            }
            if (Directory.Exists(@"C:\3DS-IDE\magicctrulib"))
            {
                this.pictureBox3.Image = _3DS_IDE.Properties.Resources.check_icon;
            }
            else
            {
                this.pictureBox3.Image = _3DS_IDE.Properties.Resources.Error_icon;
            }
            if (Directory.Exists(@"C:\3DS-IDE\tools"))
            {
                this.pictureBox2.Image = _3DS_IDE.Properties.Resources.check_icon;
            }
            else
            {
                this.pictureBox2.Image = _3DS_IDE.Properties.Resources.Error_icon;
            }
            timer1.Stop();
            this.Text = "Finished";
            button1.Visible = true;
            
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            this.Text = "Integrity check.";
            timer2.Stop();
            timer3.Start();
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            this.Text = "Integrity check..";
            timer3.Stop();
            timer4.Start();
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            this.Text = "Integrity check...";
            timer4.Stop();
            timer1.Start();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("To make sure everything works\nthe right way\nset CTRULIB\nDEVKITARM\nand DEVKITPRO\nin your environmental values!");
            this.Hide();
            select Form45 = new select();
            Form45.Show();
            //string text = System.IO.File.ReadAllText(@"C:\Users\Public\TestFolder\WriteText.txt"); just savin'
        }
    }
}
