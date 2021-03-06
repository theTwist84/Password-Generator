﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BCrypt.Net;

namespace Password_Generator
{
    public partial class Login : Form
    {
        private static string GetRandomSalt()
        {
            return BCrypt.Net.BCrypt.GenerateSalt(12);
        }

        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, GetRandomSalt());
        }

        public static bool ValidatePassword(string password, string correctHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, correctHash);
        }

        public static class Passing
        {
            public static string Passkey { get; set; }
        }

        public Login()
        {
            InitializeComponent();
            if (File.Exists("Config.txt"))
            {
                groupBox1.Text = "Login";
                label3.Text = "Please enter your login details below:";
                button1.Text = "Login";
            }
            else
            {
                groupBox1.Text = "Setup";
                label3.Text = "Please enter a Username and Password for security below:";
                button1.Text = "Create";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "Create")
            {
                if (textBox1.TextLength > 4)
                {
                    if (textBox2.TextLength > 1)
                    {
                        TextWriter tw = new StreamWriter("config.txt");
                        tw.WriteLine(textBox2.Text);
                        tw.WriteLine(HashPassword(textBox1.Text));
                        tw.Close();
                        Passing.Passkey = textBox1.Text;
                        Form1 mainForm = new Form1();
                        mainForm.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Please make sure your login information is has a Usename and the password is atleast 5 characters.", "Incorrect Usename/Password", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Please make sure your login information is has a Usename and the password is atleast 5 characters.", "Incorrect Usename/Password", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                if (File.Exists("config.txt"))
                {
                    TextReader tr = new StreamReader("config.txt");
                    string Username = tr.ReadLine();
                    string Hash = tr.ReadLine();
                    tr.Close();
                    if (Username == textBox2.Text)
                    {
                        if (ValidatePassword(textBox1.Text, Hash) == true)
                        {
                            Passing.Passkey = textBox1.Text;
                            Form1 mainForm = new Form1();
                            mainForm.Show();
                            mainForm.Key = textBox1.Text;
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Please make sure your login information is correct.", "Incorrect Usename/Password.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please make sure your login information is correct.", "Incorrect Usename/Password.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (e.CloseReason == CloseReason.WindowsShutDown) return;
            {
                if ((Application.OpenForms["Form1"] as Form1) != null)
                {
                }
                else
                {
                    switch (MessageBox.Show(this, "Are you sure you want to close?", "Closing", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation))
                    {
                        case DialogResult.No:
                            e.Cancel = true;
                            break;
                        default:
                            Environment.Exit(0);
                            break;
                    }
                }
            }
        }
    }
}
