using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POS_System.Services;

namespace POS_System
{
    public partial class Administrator : Form
    {
        private AuthenticationService _authService = new AuthenticationService();

        public Administrator()
        {
            InitializeComponent();
        }

        private void Administrator_Load(object sender, EventArgs e)
        {
            try
            {
                _authService.InitializeDatabase();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Connection Error: " + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Login form1 = new Login();
            form1.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
             // Login Button
            string username = textBox1.Text.Trim();
            string password = textBox2.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter username and password.");
                return;
            }

            try
            {
                if (_authService.Login(username, password, "Administrator"))
                {
                    this.Hide();
                    // Navigate to WorkStation as requested
                    Work_Station ws = new Work_Station();
                    ws.Show();
                }
                else
                {
                    MessageBox.Show("Invalid Username or Password.");
                }
            }
            catch(Exception ex)
            {
                 MessageBox.Show("Login Error: " + ex.Message);
            }
        }
    }
}
