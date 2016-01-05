using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Specialized;
using System.Collections;

namespace BingRewardsBot
{
    
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && textBox2.Text != "")
            {
                Console.WriteLine(textBox1.Text + textBox2.Text);
                BingRewardsBot.Properties.Settings.Default.Usernames.Add(textBox1.Text);
                BingRewardsBot.Properties.Settings.Default.Passwords.Add(textBox2.Text);
                BingRewardsBot.Properties.Settings.Default.Save();
                foreach (string s in BingRewardsBot.Properties.Settings.Default.Usernames)
                {
                    Console.WriteLine(s);
                }

            }
            this.Hide();
        }
    }
}
