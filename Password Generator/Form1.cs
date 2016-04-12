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

namespace Password_Generator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            button2.Enabled = false;
        }

        Random rand = new Random();

        private const int Keysize = 256;

        private const int DerivationIterations = 1000;

        public const string Alphabet = "abcdefghijklmnopqrstuvwyxzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public const string AlphabetSym = "abcdefghijklmnopqrstuvwyxzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789*!#@%?&";

        public static string Encrypt(string plainText, string passPhrase)
        {
            var saltStringBytes = Generate256BitsOfRandomEntropy();
            var ivStringBytes = Generate256BitsOfRandomEntropy();
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                var cipherTextBytes = saltStringBytes;
                                cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }

        public static string Decrypt(string cipherText, string passPhrase)
        {
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
            var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                var plainTextBytes = new byte[cipherTextBytes.Length];
                                var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
        }

        private static byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[32];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }


        public string GenerateString()
        {
            char[] chars = new char[25];
            for (int i = 0; i < 25; i++)
            {
                chars[i] = Alphabet[rand.Next(Alphabet.Length)];
            }
            return new string(chars);
        }

        public string GenerateStringSym()
        {
            char[] chars = new char[25];
            for (int i = 0; i < 25; i++)
            {
                chars[i] = AlphabetSym[rand.Next(AlphabetSym.Length)];
            }
            return new string(chars);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text == "Generate")
            {
                button1.Text = "Generate";
            }
            else if (comboBox1.Text == "Fetch")
            {
                button1.Text = "Fetch";
                button2.Enabled = false;
            }

            else
            {
                button1.Text = "";
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(label3.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text == "Generate")
            {
                if (checkBox1.Checked == true)
                {
                    label3.Text = GenerateStringSym();
                }
                
                else if (checkBox1.Checked == false)
                {
                    label3.Text = GenerateString();
                }
                button2.Enabled = true;
            }

            else
            {
                string name = textBox1.Text + ".txt";
                if (File.Exists(name))
                {

                    TextReader tr = new StreamReader(name);
                    string password = tr.ReadLine();
                    string encryptedstring = tr.ReadLine();
                    tr.Close();
                    string decryptedstring = Decrypt(encryptedstring, password);
                    label3.Text = decryptedstring;
                    MessageBox.Show("Your password has been fetched and decrypted, you can click on it to add it to your clipboard.", "Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Random rnd = new Random();
            string plaintext = label3.Text;
            string password = (rnd.Next(100000, 999999)).ToString();
            string encryptedstring = Encrypt(plaintext, password);
            string name = textBox1.Text + ".txt";
            TextWriter tw = new StreamWriter(name);
            tw.WriteLine(password);
            tw.WriteLine(encryptedstring);
            tw.Close();
            MessageBox.Show("Password had been saved in an encrypted state.", "Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
