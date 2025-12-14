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
    public partial class StockView : Form
    {
        private POS_System.Model.StockRepository stockRepository = new POS_System.Model.StockRepository();

        public StockView()
        {
            InitializeComponent();
            LoadData(); // Load data on startup
            SetupEvents();
        }

        private void SetupEvents()
        {
            // Search functionality
            txtsearch.TextChanged += TextBox1_TextChanged;
            button1.Click += (s, e) => PerformSearch(); 
            
            // Delete functionality
            btnView.Click += BtnDelete_Click; // "DELETE" button
            // removeItemToolStripMenuItem.Click += RemoveItemToolStripMenuItem_Click; // Context menu removed
            
            // Grid Formatting
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void LoadData()
        {
            // Populate Grid
            var allStocks = stockRepository.GetAllItems();
            dataGridView1.DataSource = allStocks;
            UpdateLabels();
        }

        private void UpdateLabels()
        {
            // Stock Item Count
            label9.Text = stockRepository.GetItemCount().ToString();

            // Purchasing Value
            label10.Text = $"Php. {stockRepository.GetTotalPurchasingValue():F2}";

            // Low Stock Item Count 
            // Note: User's label says "Low Stock Item Count" but format was "php. 00.0". 
            // I will assume it should be a count, but if they want value of low stock, I'd need another query.
            // Based on "Count" in text, I'll show the integer count of items <= 5.
            label1.Text = stockRepository.GetLowStockCount().ToString(); 
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            PerformSearch();
        }

        private void PerformSearch()
        {
            string query = txtsearch.Text.Trim();
            if (query == "seach product" || string.IsNullOrWhiteSpace(query))
            {
                LoadData(); // Reset if empty
            }
            else
            {
                var results = stockRepository.SearchStocks(query);
                dataGridView1.DataSource = results;
            }
        }

        // Handle DELETE button (btnView from designer seems to be the DELETE button based on text)
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            DeleteSelectedStock();
        }

        /*
        private void RemoveItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteSelectedStock();
        }
        */

        private void DeleteSelectedStock()
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Get the bound object
                if (dataGridView1.SelectedRows[0].DataBoundItem is POS_System.Model.Stock selectedItem)
                {
                    var confirm = MessageBox.Show($"Are you sure you want to delete {selectedItem.Product}?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (confirm == DialogResult.Yes)
                    {
                        stockRepository.DeleteStock(selectedItem.ItemCode); // Use repository to delete
                        MessageBox.Show("Item deleted successfully.");
                        LoadData(); // Refresh grid and labels
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select an item to delete.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Work_Station work_Station = new Work_Station();
            work_Station.Show();

        }
    }
}
