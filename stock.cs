using POS_System.Model;
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
    public partial class stock : Form
    {
        private StockRepository stockRepository = new StockRepository();
        private SupplierRepository supplierRepository = new SupplierRepository();

        public stock()
        {
            InitializeComponent();
            
            // Fix for invisible text on white background
            dataGridView1.ForeColor = Color.Black;
            dataGridView1.DefaultCellStyle.ForeColor = Color.Black;
            dataGridView1.RowsDefaultCellStyle.ForeColor = Color.Black;
            dataGridView1.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;

            txtQty.TextChanged += ComputeValue;
            txtPrice.TextChanged += ComputeValue;
            txtItem.KeyPress += txtItem_KeyPress;
            txtItem.TextChanged += txtItem_TextChanged_Validate;
            
            // Edit functionality subscripts
            dataGridView1.CellClick += DataGridView1_CellClick;
            button2.Click += BtnEdit_Click;

            LoadSuppliers();
        }

        private void LoadSuppliers()
        {
            try
            {
                comboBox1.Items.Clear();
                var suppliers = supplierRepository.GetAllSuppliers();
                foreach (var supplier in suppliers)
                {
                    comboBox1.Items.Add(supplier.Name);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading suppliers: " + ex.Message);
            }
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                // Assuming column order matches the class properties or are auto-generated
                // Ideally access by column name if known, but auto-gen relies on binding
                
                // Populate fields
                // Since we bound to List<Stock>, the rows are bound to Stock objects
                if (row.DataBoundItem is Stock item)
                {
                    txtItem.Text = item.ItemCode;
                    txtDescription.Text = item.Product;
                    txtQty.Text = item.Quantity.ToString();
                    txtPrice.Text = item.PurchasingPrice.ToString("F2");
                    txtSelling.Text = item.SellingPrice.ToString("F2");
                    // txtOrdernumber.Text = item.OrderNumber;
                    comboBox1.Text = item.Supplier;
                    
                    // Disable Item Code during edit strictly speaking, or just allow overwrite 
                    // For now keeping simple as per request
                }
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                 if (string.IsNullOrWhiteSpace(txtItem.Text)) return;

                 Stock updatedStock = new Stock(
                    txtItem.Text,
                    txtDescription.Text,
                    double.TryParse(txtQty.Text, out double qty) ? qty : 0,
                    double.TryParse(txtPrice.Text, out double price) ? price : 0,
                    double.TryParse(txtSelling.Text, out double selling) ? selling : 0,
                    "", // OrderNumber removed from UI
                    comboBox1.Text 
                );

                stockRepository.UpdateStock(txtItem.Text, updatedStock);
                
                UpdateSummary();
                MessageBox.Show("Stock item updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating item: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // InitializeEvents removed as it is now in constructor

        private void txtItem_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Block non-digit inputs
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtItem_TextChanged_Validate(object sender, EventArgs e)
        {
            // Remove any non-numeric characters (handles paste)
            if (System.Text.RegularExpressions.Regex.IsMatch(txtItem.Text, "[^0-9]"))
            {
                txtItem.Text = System.Text.RegularExpressions.Regex.Replace(txtItem.Text, "[^0-9]", "");
                txtItem.SelectionStart = txtItem.Text.Length; // Restore cursor position
            }
        }

        private void ComputeValue(object sender, EventArgs e)
        {
            if (double.TryParse(txtQty.Text, out double qty) && double.TryParse(txtPrice.Text, out double price))
            {
                txtValue.Text = (qty * price).ToString("F2");
            }
            else
            {
                txtValue.Text = "0.00";
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrWhiteSpace(txtItem.Text) || string.IsNullOrWhiteSpace(txtDescription.Text))
                {
                    MessageBox.Show("Please fill in all required fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Create new stock object from UI
                Stock newStock = new Stock(
                    txtItem.Text,
                    txtDescription.Text,
                    double.TryParse(txtQty.Text, out double qty) ? qty : 0,
                    double.TryParse(txtPrice.Text, out double price) ? price : 0,
                    double.TryParse(txtSelling.Text, out double selling) ? selling : 0,
                    "", // OrderNumber removed from UI
                    comboBox1.Text 
                );

                // Add to manager
                stockRepository.AddStock(newStock);

                // Update UI
                UpdateSummary();
                MessageBox.Show("Stock item added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding item: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateSummary()
        {
            lblStockItem.Text = stockRepository.GetItemCount().ToString();
            lblPurchasingValue.Text = $"Php. {stockRepository.GetTotalPurchasingValue():F2}";
            
            // Refresh Grid with a new list reference to force full UI update
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = new List<Stock>(stockRepository.GetAllItems());
            dataGridView1.Refresh();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearInputs();
        }

        private void ClearInputs()
        {
            txtItem.Clear();
            txtDescription.Clear();
            txtQty.Clear();
            txtPrice.Clear();
            txtValue.Clear();
            txtSelling.Clear();
            // txtOrdernumber.Clear();
            comboBox1.SelectedIndex = -1;
            comboBox1.Text = "";
        }

        // Navigation and other handlers
        private void btnPurchasing_Click(object sender, EventArgs e)
        {
            this.Hide();
            PurchasingSummary summary = new PurchasingSummary();
            summary.Show();
        }

        private void fullScreenViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StockView stockview = new StockView();
            stockview.Show();
        }

        private void lowStockValueReminderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Low_Stock_Value_Reminder lowstock = new Low_Stock_Value_Reminder();
            lowstock.Show();
        }

        // Unused / Placeholder methods kept to avoid designer errors until cleaned up
        private void TxtPrice_TextChanged(object sender, EventArgs e) { }
        private void textBox2_TextChanged(object sender, EventArgs e) { }
        private void textBox6_TextChanged(object sender, EventArgs e) { }
        private void label8_Click(object sender, EventArgs e) { }
        private void btnView_Click(object sender, EventArgs e)
        {
            this.Hide();
            StockView stockView = new StockView();
            stockView.Show();
        }
        private void txtQty_TextChanged(object sender, EventArgs e) { }
        private void txtPrice_TextChanged(object sender, EventArgs e) { }
        private void txtValue_TextChanged(object sender, EventArgs e) { }
        private void txtOrdernumber_TextChanged(object sender, EventArgs e) { }

        private void txtItem_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnPrintStock_Click(object sender, EventArgs e)
        {

        }

        private void dashboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            Work_Station work = new Work_Station();
            work.Show();
        }
    }
}
