using System;
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
            return GenerateSalt(12);
        }

        public static string HashPassword(string password)
        {
            return BCrypt.HashPassword(password, GetRandomSalt());
        }

        public static bool ValidatePassword(string password, string correctHash)
        {
            return Verify(password, correctHash);
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
            Form1 mainForm = new Form1();
            mainForm.Show();
            this.Hide();
        }
    }
}
