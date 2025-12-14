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
    public partial class receipt : Form
    {
        private bool _autoPrint = false;

        public receipt(string items, string prices, string total, string date, bool autoPrint = false)
        {
            InitializeComponent();
            label1.Text = items;
            label4.Text = prices;
            label5.Text = total;
            label3.Text = "Date : " + date;
            _autoPrint = autoPrint;

            if (_autoPrint)
            {
                this.Shown += (s, e) => SaveReceipt();
            }
        }

        // Default constructor if needed by designer (though not strictly necessary for runtime if we always use the other)
        public receipt() 
        { 
            InitializeComponent(); 
        }

        private void SaveReceipt()
        {
            try
            {
                // Create Receipt folder if not exists
                string folderPath = System.IO.Path.Combine(Application.StartupPath, "Receipt");
                if (!System.IO.Directory.Exists(folderPath))
                {
                    System.IO.Directory.CreateDirectory(folderPath);
                }

                // Generate filename
                string fileName = $"Receipt_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                string fullPath = System.IO.Path.Combine(folderPath, fileName);

                // Capture form to bitmap
                using (Bitmap bmp = new Bitmap(this.Width, this.Height))
                {
                    this.DrawToBitmap(bmp, new Rectangle(0, 0, this.Width, this.Height));
                    bmp.Save(fullPath, System.Drawing.Imaging.ImageFormat.Png);
                }
                
                MessageBox.Show($"Receipt saved successfully to:\n{fullPath}", "Receipt Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save receipt: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label1_Click(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
    }
}
