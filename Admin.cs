using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POS_System
{
    public partial class @void : Form
    {
        private POS_System.Services.AuthenticationService _authService = new POS_System.Services.AuthenticationService();

        public @void()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string password = textBox1.Text.Trim();
                
                // User said "enter admin account". 
                // Admin.Designer.cs shows only ONE textbox (textBox1) and label "Admin Password".
                // It seems it only asks for password, assuming username 'admin'?
                // OR does the user want username AND password?
                // Let's check Admin.Designer.cs again. Label1 says "Admin Password". 
                // There is only textBox1. 
                // So we should assume username 'admin' and check password.
                
                if (_authService.Login("admin", password, "Administrator"))
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Invalid Admin Password!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
