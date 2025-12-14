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

namespace POS_System
{
    public partial class PurchasingSummary : Form
    {
        private StockRepository stockRepository = new StockRepository();

        public PurchasingSummary()
        {
            InitializeComponent();
            SetupEventHandlers();
        }

        private void SetupEventHandlers()
        {
            button1.Click += BtnGetData_Click;
        }

        private void BtnGetData_Click(object sender, EventArgs e)
        {
            try
            {
                // Determine which tab is active
                if (tabControl1.SelectedTab == tabPage1) // Purchasing Summary
                {
                    LoadPurchasingSummary();
                }
                else if (tabControl1.SelectedTab == tabPage2) // Invoice Summary
                {
                    LoadInvoiceSummary();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadPurchasingSummary()
        {
            // For Purchasing Summary, we show the current Stock Inventory
            // Ideally this would be a historical purchase log, but we only store current stock
            // So we display all stock items and their total purchasing value.
            
            var stocks = stockRepository.GetAllItems();
            
            // Bind to Grid
            dataGridView1.DataSource = stocks;

            // Calculate Totals
            int totalItems = stocks.Count;
            double totalValue = stockRepository.GetTotalPurchasingValue();

            // Display in Red Panels
            textBox2.Text = totalItems.ToString(); // Total Item Purchase
            textBox3.Text = $"Php. {totalValue:F2}"; // Purchase Value
            
            // Month Box (optional, just showing current month as "snapshot")
            textBox1.Text = DateTime.Now.ToString("MMMM");
        }

        private void LoadInvoiceSummary()
        {
            DateTime start = dateTimePicker1.Value;
            DateTime end = dateTimePicker2.Value;

            // Fetch Sales from Database
            DataTable salesData = stockRepository.GetSales(start, end);

            // Bind to Grid
            dataGridView2.DataSource = salesData;

            // Calculate Totals
            decimal totalSales = 0;
            if (salesData.Rows.Count > 0)
            {
                // Sum the TotalAmount column
                totalSales = salesData.AsEnumerable().Sum(row => row.Field<decimal>("TotalAmount"));
            }

            // Display in Red Panels
            // Panel 5 is Month
            textBox4.Text = start.ToString("MMMM"); 

            // Panel 6 is Purchase Value (Should be Sales Value)
            // Label says "Purchase Value" which is a bit confusing but we reuse the label or it's named "Purchase Value" in designer
            // We'll set the value to Total Sales
            textBox5.Text = $"Php. {totalSales:F2}";
        }


        // Boilerplate / unused events
        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            stock stock = new stock();
            stock.Show();
        }
    }
}
