using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using POS_System.Model;
using POS_System.Services;

namespace POS_System
{
    public partial class POS : Form
    {
        private POS_System.Model.StockRepository stockRepository = new POS_System.Model.StockRepository();
        private TransactionService _transactionService = new TransactionService();
        private List<CartItem> cart = new List<CartItem>();

        public POS()
        {
            InitializeComponent();
            SetupEvents();
            InitializeGrid();
        }

        private void InitializeGrid()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.Add("ItemCode", "Item Code");
            dataGridView1.Columns.Add("Description", "Product"); // Renamed Description to Product in UI context if preferred, but Model uses Description/Product
            dataGridView1.Columns.Add("Price", "Price");
            dataGridView1.Columns.Add("Quantity", "Qty");
            dataGridView1.Columns.Add("Total", "Total");
            
            // Format
            dataGridView1.Columns["Price"].DefaultCellStyle.Format = "F2";
            dataGridView1.Columns["Total"].DefaultCellStyle.Format = "F2";
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void SetupEvents()
        {
            // Keypad
            button1.Click += (s, e) => AppendKey("1");
            button2.Click += (s, e) => AppendKey("2");
            button3.Click += (s, e) => AppendKey("3");
            button4.Click += (s, e) => AppendKey("4");
            button5.Click += (s, e) => AppendKey("5");
            button6.Click += (s, e) => AppendKey("6");
            button7.Click += (s, e) => AppendKey("7");
            button8.Click += (s, e) => AppendKey("8");
            button9.Click += (s, e) => AppendKey("9");
            button10.Click += (s, e) => AppendKey("0");
            
            // Actions
            button12.Click += (s, e) => ClearInput(); // Clear
            button11.Click += (s, e) => ProcessEnter(); // Enter
            button13.Click += (s, e) => VoidTransaction(); // Void
            // button14 is Invoice - currently just a placeholder or could save transaction
            
            // TextBox
            textBox1.KeyDown += TextBox1_KeyDown;
        }

        private void AppendKey(string key)
        {
            textBox1.Text += key;
            textBox1.Focus();
            textBox1.SelectionStart = textBox1.Text.Length;
        }

        private void ClearInput()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox1.Focus();
        }

        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ProcessEnter();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void ProcessEnter()
        {
            string code = textBox1.Text.Trim();
            if (string.IsNullOrEmpty(code)) return;

            var stock = stockRepository.GetStockByCode(code);
            if (stock != null)
            {
                // Populate display fields
                textBox2.Text = stock.Product;
                textBox3.Text = stock.SellingPrice.ToString("F2");

                // Add to cart
                AddToCart(stock);
                
                // Clear barcode for next scan
                textBox1.Clear();
                textBox1.Focus();
            }
            else
            {
                MessageBox.Show("Item not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.SelectAll();
            }
        }

        private void AddToCart(POS_System.Model.Stock stock)
        {
            var existing = cart.FirstOrDefault(x => x.ItemCode == stock.ItemCode);
            if (existing != null)
            {
                existing.Quantity++;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ItemCode = stock.ItemCode,
                    Description = stock.Product,
                    Price = stock.SellingPrice,
                    Quantity = 1
                });
            }
            RefreshGrid();
        }

        private void RefreshGrid()
        {
            dataGridView1.Rows.Clear();
            double grandTotal = 0;

            foreach (var item in cart)
            {
                double total = item.Price * item.Quantity;
                dataGridView1.Rows.Add(item.ItemCode, item.Description, item.Price, item.Quantity, total);
                grandTotal += total;
            }

            label5.Text = $"php {grandTotal:F2}";
        }

        private void VoidTransaction()
        {
             // Open Admin Auth Form (@void)
             @void adminAuth = new @void();
             if (adminAuth.ShowDialog() == DialogResult.OK)
             {
                 // Auth Success, proceed to void
                 if (MessageBox.Show("Authorization Successful. Are you sure you want to void this transaction?", "Confirm Void", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                 {
                     cart.Clear();
                     RefreshGrid();
                     ClearInput();
                 }
             }
        }

        // Placeholder for Invoice/Payment
        private void button14_Click(object sender, EventArgs e)
        {
             if (cart.Count == 0)
             {
                 MessageBox.Show("Cart is empty.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                 return;
             }
             
             // Prepare data for receipt
             StringBuilder items = new StringBuilder();
             StringBuilder prices = new StringBuilder();
             
             foreach(var item in cart)
             {
                 items.AppendLine($"{item.Description} x{item.Quantity}");
                 prices.AppendLine($"{(item.Price * item.Quantity):F2}");
             }
             
             string total = label5.Text; // "php 123.00"
             string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

             // Record Sale in Database via Service
             try
             {
                 _transactionService.ProcessTransaction(cart, "Cash");
             }
             catch(Exception ex)
             {
                MessageBox.Show("Failed to process transaction: " + ex.Message);
             }

             receipt receiptForm = new receipt(items.ToString(), prices.ToString(), total, date, true); // Auto-print enabled
             receiptForm.Show();

             // clear cart after printing? User might want to see it, but usually invoice implies done.
             // Keeping it consistent with previous logic that cleared it, but maybe after closing receipt?
             // For now, let's clear it as per original logic, assuming invoice = paid.
             cart.Clear();
             RefreshGrid();
             ClearInput();
        }

        // Unused designer events
        private void label2_Click(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void label4_Click(object sender, EventArgs e) { }
        private void textBox5_TextChanged(object sender, EventArgs e) { }
        private void label4_Click_1(object sender, EventArgs e) { }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void button13_Click(object sender, EventArgs e) { }

        private void dashBoardToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void dashboardToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Work_Station work_Station = new Work_Station();
            work_Station.Show();
        }
    }
}
