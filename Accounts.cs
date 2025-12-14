using System;
using System.Windows.Forms;
using POS_System.Services;

namespace POS_System
{
    public partial class Accounts : Form
    {
        private AuthenticationService authService = new AuthenticationService();

        public Accounts()
        {
            InitializeComponent();
            SetupEvents();
            LoadUsers();
        }

        private void SetupEvents()
        {
            btnSave.Click += BtnSave_Click;
            btnClear.Click += BtnClear_Click;
            btnBack.Click += BtnBack_Click;
        }

        private void LoadUsers()
        {
            try
            {
                dataGridView1.DataSource = authService.GetAllUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading users: " + ex.Message);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            string username = txtUser.Text.Trim();
            string password = txtPass.Text.Trim();
            string role = cbRole.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
            {
                MessageBox.Show("Please fill all fields!");
                return;
            }

            try
            {
                authService.AddUser(username, password, role);
                MessageBox.Show("User added successfully!");
                ClearInputs();
                LoadUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding user. Username may already exist.\nDetails: " + ex.Message);
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            ClearInputs();
        }

        private void ClearInputs()
        {
            txtUser.Clear();
            txtPass.Clear();
            cbRole.SelectedIndex = -1;
            txtUser.Focus();
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            this.Hide();
            // Assuming going back to WorkStation? Or maybe Admin?
            // Since this is account management, likely WorkStation (dashboard).
            Work_Station ws = new Work_Station(); 
            ws.Show();
        }
    }
}
