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
    public partial class Login : Form
    {
        private AuthenticationService _authService = new AuthenticationService();

        public Login()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
             // Optional: Init DB here if needed, but Admin form does it too.
             // Best to do it here too just in case it's the first run.
             try { _authService.InitializeDatabase(); } catch { }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Login Button (Cashier)
            string username = textBox1.Text.Trim();
            string password = textBox2.Text.Trim();

            if (_authService.Login(username, password, "Cashier"))
            {
                // Set Session
                Model.UserSession.CurrentUserName = username;

                this.Hide();
                POS pos = new POS();
                pos.Show();
            }
            else
            {
                MessageBox.Show("Invalid Cashier Credentials!");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Admin Button - Open Admin Login form
            this.Hide();
            Administrator admin = new Administrator();
            admin.Show();
        }



        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
