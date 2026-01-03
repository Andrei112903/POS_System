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
            this.Load += PurchasingSummary_Load;
        }

        private void PurchasingSummary_Load(object sender, EventArgs e)
        {
            // Data will not load initially as per user request.
            // User must click "Get Data" to display records.
            this.Text = "Purchasing Summary"; 
        }

        private void SetupEventHandlers()
        {
            button1.Click += BtnGetData_Click;
            tabControl1.SelectedIndexChanged += TabControl1_SelectedIndexChanged;
        }

        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabPage1)
            {
                LoadPurchasingSummary();
            }
            else if (tabControl1.SelectedTab == tabPage2)
            {
                LoadInvoiceSummary();
            }
        }

        private void BtnGetData_Click(object sender, EventArgs e)
        {
            try
            {
                // Determine which tab is active
                if (tabControl1.SelectedTab == tabPage1) // Purchasing Summary
                {
                    LoadPurchasingSummary();
                    MessageBox.Show("Inventory Stock Refreshed.\n\nNote: The 'Purchasing Summary' displays current inventory and is not affected by the date range.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (tabControl1.SelectedTab == tabPage2) // Invoice Summary
                {
                    LoadInvoiceSummary();
                    MessageBox.Show("Invoice Summary Refreshed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            
            // Project to ViewModel
            var summaryItems = stocks.Select(s => new PurchasingSummaryItem
            {
                ItemCode = s.ItemCode,
                Product = s.Product,
                Quantity = s.Quantity,
                PurchasingPrice = s.PurchasingPrice,
                PurchasingValue = s.PurchasingValue,
                SellingPrice = s.SellingPrice,
                TotalSellingItem = s.Quantity * s.SellingPrice,
                TotalGross = (s.Quantity * s.SellingPrice) - s.PurchasingValue,
                Supplier = s.Supplier
            }).ToList();

            // Bind to Grid
            dataGridView1.DataSource = summaryItems;

            // Format Grid Columns
            if (dataGridView1.Columns.Count > 0)
            {
                dataGridView1.Columns["ItemCode"].HeaderText = "Item Code";
                dataGridView1.Columns["Product"].HeaderText = "Product";
                dataGridView1.Columns["Quantity"].HeaderText = "Quantity";
                dataGridView1.Columns["PurchasingPrice"].HeaderText = "Purchasing Price";
                dataGridView1.Columns["PurchasingPrice"].DefaultCellStyle.Format = "N2";
                dataGridView1.Columns["PurchasingValue"].HeaderText = "Purchasing Value";
                dataGridView1.Columns["PurchasingValue"].DefaultCellStyle.Format = "N2";
                dataGridView1.Columns["SellingPrice"].HeaderText = "Selling Price";
                dataGridView1.Columns["SellingPrice"].DefaultCellStyle.Format = "N2";
                dataGridView1.Columns["TotalSellingItem"].HeaderText = "Total Selling Item";
                dataGridView1.Columns["TotalSellingItem"].DefaultCellStyle.Format = "N2";
                dataGridView1.Columns["TotalGross"].HeaderText = "Total Gross";
                dataGridView1.Columns["TotalGross"].DefaultCellStyle.Format = "N2";
                dataGridView1.Columns["Supplier"].HeaderText = "Supplier";
            }

            // Calculate Totals
            double totalQuantity = stocks.Sum(s => s.Quantity);
            double totalValue = stockRepository.GetTotalPurchasingValue();
            double totalGross = summaryItems.Sum(s => s.TotalGross);

            // Display in Red Panels
            textBox2.Text = totalQuantity.ToString(); // Total Item Purchase (Sum of Quantities)
            textBox3.Text = $"Php. {totalValue:F2}"; // Purchase Value
            
            // Restore Month Box
            label1.Text = "Month:";
            textBox1.Text = DateTime.Now.ToString("MMMM");

            // Display Total Gross Profit in the new right-most panel (textBox6)
            label8.Text = "Gross Profit :";
            textBox6.Text = $"Php. {totalGross:F2}";
        }

        public class PurchasingSummaryItem
        {
            public string ItemCode { get; set; }
            public string Product { get; set; }
            public double Quantity { get; set; }
            public double PurchasingPrice { get; set; }
            public double PurchasingValue { get; set; }
            public double SellingPrice { get; set; }
            public double TotalSellingItem { get; set; }
            public double TotalGross { get; set; }
            public string Supplier { get; set; }
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
