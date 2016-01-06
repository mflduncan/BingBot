using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace BingRewardsBot
{
    public partial class Form1 : Form
    {
        Thread[] bot_thread;
        BotController[] bot_controller;
        TorWrapper tor = new TorWrapper();
        string[] words, accounts;
        int progress;
        bot_arguments[] args;
        int arg_index = 0;
        bool running = true;
        int botsRan = 0;

        struct bot_arguments
        {
            public string username, password;
            public List<string> queries;
            public int mode;
        }

        public Form1()
        {
            InitializeComponent();
            /*Properties.Settings.Default.Usernames.Clear();
            Properties.Settings.Default.Passwords.Clear();
            Properties.Settings.Default.Save();*/
            FormClosing +=Form1_FormClosing;
            populate_list();
            try
            {
                words = File.ReadAllLines("wordlist.txt");
                accounts = File.ReadAllLines("accounts.txt");
            }
            catch
            {
                MessageBox.Show("Error: Could not open wordlist and/or account file!");
                Application.Exit();
            }
            progressBar1.Maximum = accounts.Length * 2 * 30;
            init_args();
            //start_bots();
            
            //Thread update_thread = new Thread(update_loop);
            //update_thread.Start();
        }

        public void populate_list()
        {
            listView1.Items.Clear();
            Console.WriteLine("Populating list...");
            List<string> usernames = new List<string>();
            List<string> passwords = new List<string>();
            foreach(string s in BingRewardsBot.Properties.Settings.Default.Usernames)
            {
                usernames.Add(s);
            }
            foreach(string s in BingRewardsBot.Properties.Settings.Default.Passwords)
            {
                passwords.Add(s);
            }
            for (int i = 0; i < usernames.Count; i++)
            {
                ListViewItem item = new ListViewItem(usernames[i]);
                item.SubItems.Add("1");
                listView1.Items.Add(item);
            }
        }

        public void init_args()
        {
            args = new bot_arguments[accounts.Length * 2];
            for (int i = 0; i < args.Length; i++)
            { 
                string[] usr = accounts[i/2].Split(':');

                args[i].username = usr[0];
                args[i].password = usr[1];
                args[i].queries = get_random_words(30, words);
                args[i++].mode = 0;

                args[i].username = usr[0];
                args[i].password = usr[1];
                args[i].queries = get_random_words(20, words);
                args[i].mode = 1;
            }
        }


        private List<string> get_random_words(int size, string[] source)
        {
            // Check that it isn't asking for more words than are in the source file
            if (source.Length < size)
            {
                throw new Exception("Not enough elements in source to have " + size + " unique elements in list");
            }
            List<string> word_list = new List<string>();
            Random rand_int = new Random();
            while (word_list.Count < size)
            {
                // Pick a random index which we will use in the array for a random word
                int rand = rand_int.Next(0, source.Length);

                // If the word isn't in the list already, add it
                if (!word_list.Contains(source[rand]))
                {
                    word_list.Add(source[rand]);
                }
            }
            return word_list;
        }



        //Closing operations


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            close_all_bots();
        }
        private void close_all_bots()
        {
            running = false;
            for (int i = 0; i < bot_thread.Length; i++) // abort all threads
            {
                if (bot_thread[i] != null)
                {
                    bot_thread[i].Abort();
                }
            }

            for (int i = 0; i < bot_controller.Length; i++) //quit all bots
            {
                if (bot_controller[i] != null)
                {
                    bot_controller[i].quit();
                }
            }
        }


        //Background worker

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (botsRan % Convert.ToInt32(proxyUpDown.Value) == 0)
            {
                tor.SendCommand("SIGNAL NEWNYM\r\n"); //request a new ip
            }
            for (int i = 0; i < bot_thread.Length && running; i++)
            {
                if (bot_thread[i] == null) //if the thread is new
                {
                    Console.WriteLine("Starting with arg_index = " + Convert.ToString(arg_index));
                    bot_controller[i] = new BotController(args[arg_index].username, args[arg_index].password, args[arg_index].queries, args[arg_index++].mode);
                    bot_thread[i] = new Thread(bot_controller[i].start);
                    bot_thread[i].Start();
                    botsRan++;
                }
                
                else if (!bot_thread[i].IsAlive && arg_index != args.Length) //if the thread is dead
                {
                    Console.WriteLine("Starting with arg_index = " + Convert.ToString(arg_index));
                    bot_controller[i] = new BotController(args[arg_index].username, args[arg_index].password, args[arg_index].queries, args[arg_index++].mode);
                    bot_thread[i] = new Thread(bot_controller[i].start);
                    bot_thread[i].Start();
                    botsRan++;
                }
            }
            progress = Convert.ToInt32(BotController.progress);
            Thread.Sleep(250);
        }


        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar1.Value = progress;
            backgroundWorker1.RunWorkerAsync();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        //Buttons

        private void stopButton_Click(object sender, EventArgs e) //STOP BUTTON
        {
            startButton.Enabled = true;
            stopButton.Enabled = false;
            close_all_bots();
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            Form2 f2 = new Form2();
            f2.Show();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            stopButton.Enabled = true;
            startButton.Enabled = false;
            bot_thread = new Thread[Convert.ToInt32(threadUpDown.Value)];
            bot_controller = new BotController[Convert.ToInt32(threadUpDown.Value)];

            backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
            backgroundWorker1.RunWorkerAsync();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
