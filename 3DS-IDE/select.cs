using System;
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
    public partial class select : Form
    {
        IDE ideform = new IDE();
        public select()
        {
            InitializeComponent();
        }

        private void select_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.shouldNew = true;
            Properties.Settings.Default.Save();
            ideform.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.shouldNew = false;
            Properties.Settings.Default.Save();
            ideform.Show();
            this.Hide();
        }
    }
}
