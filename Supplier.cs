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
    public partial class Supplier : Form
    {
        private POS_System.Model.SupplierRepository _repository = new POS_System.Model.SupplierRepository();

        public Supplier()
        {
            InitializeComponent();
            LoadSuppliers();
            SetupEvents();
        }

        private void SetupEvents()
        {
            button1.Click += Button1_Click; // Save
            button2.Click += Button2_Click; // Clear
            button3.Click += Button3_Click; // Back
        }

        private void LoadSuppliers()
        {
            try
            {
                var suppliers = _repository.GetAllSuppliers();
                dataGridView1.DataSource = suppliers;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading suppliers: " + ex.Message);
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            string name = textBox1.Text.Trim();
            string address = textBox2.Text.Trim();
            string email = textBox3.Text.Trim();
            string contact = textBox4.Text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Supplier Name is required.");
                return;
            }

            try
            {
                var supplier = new POS_System.Model.Supplier(name, address, email, contact);
                _repository.AddSupplier(supplier);
                MessageBox.Show("Supplier saved successfully!");
                ClearFields();
                LoadSuppliers();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving supplier: " + ex.Message);
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void ClearFields()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox1.Focus();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            Work_Station ws = new Work_Station();
            ws.Show();
        }
    }
}
