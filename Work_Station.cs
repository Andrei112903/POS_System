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
    public partial class Work_Station : Form
    {
        private POS_System.Model.StockRepository stockRepository = new POS_System.Model.StockRepository();

        public Work_Station()
        {
            InitializeComponent();
        }

        private void Work_Station_Load(object sender, EventArgs e)
        {
            LoadChartData();
            LoadDashboardMetrics();
        }

        private void LoadDashboardMetrics()
        {
            try
            {
                DateTime today = DateTime.Today;

                // Sales Count
                int count = stockRepository.GetDailySalesCount(today);
                lblSalesCount.Text = count.ToString();

                // Total Sales Today
                double totalToday = stockRepository.GetDailySalesTotal(today);
                lblTotalSalestoday.Text = $"php {totalToday:F2}";

                // Total Sales Cash
                double totalCash = stockRepository.GetDailySalesTotalByType(today, "Cash");
                lblTotalSalescash.Text = $"php {totalCash:F2}";

                // Total Sales Credit
                double totalCredit = stockRepository.GetDailySalesTotalByType(today, "Credit");
                lblTotalSalescredit.Text = $"php {totalCredit:F2}";

                // Low Stock Alert (Threshold < 10)
                int lowStockCount = stockRepository.GetLowStockCount(10);
                label2.Text = lowStockCount.ToString("D2"); // Format as 00, 01, etc.
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error loading dashboard metrics: " + ex.Message);    
            }
        }

        private void LoadChartData()
        {
            try
            {
                var topStocks = stockRepository.GetTopStocks(10); // Get top 10 items by quantity

                chart1.Series.Clear();
                var series = chart1.Series.Add("Stock Levels");
                series.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;
                series.XValueMember = "Product";
                series.YValueMembers = "Quantity";

                // Bind data
                // Need to project to a format the chart can easily bind to, or add points manually
                foreach (var stock in topStocks)
                {
                    series.Points.AddXY(stock.Product, stock.Quantity);
                }

                chart1.Titles.Clear();
                chart1.Titles.Add("Inventory Overview (Top 10 Items)");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading chart: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            stock stck = new stock();
            stck.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
            Accounts accounts = new Accounts();
            accounts.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
             this.Hide();
             Supplier supplier = new Supplier();
             supplier.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            POS pOS = new POS();
            pOS.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            LoadChartData(); // Refresh chart
            LoadDashboardMetrics(); // Refresh metrics
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.Hide();
            Administrator administrator = new Administrator();
            administrator.Show();
        }
    }
}
