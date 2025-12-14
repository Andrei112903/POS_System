using System;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace POS_System.Services
{
    public class AuthenticationService
    {
        private readonly string connectionString;

        public AuthenticationService()
        {
            connectionString = ConfigurationManager.ConnectionStrings["POS_DB"].ConnectionString;
        }

        public void InitializeDatabase()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                
                // Create Users table if not exists
                string createTableQuery = @"CREATE TABLE IF NOT EXISTS Users (
                                            Id INT AUTO_INCREMENT PRIMARY KEY,
                                            Username VARCHAR(50) NOT NULL UNIQUE,
                                            Password VARCHAR(255) NOT NULL,
                                            Role VARCHAR(50) DEFAULT 'Cashier'
                                          )";
                using (MySqlCommand cmd = new MySqlCommand(createTableQuery, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // Check if Role column exists (for migration)
                try {
                    string checkCol = "SELECT Role FROM Users LIMIT 1";
                    using (MySqlCommand cmd = new MySqlCommand(checkCol, conn)) { cmd.ExecuteNonQuery(); }
                } catch {
                    // Column likely missing, add it
                    string addCol = "ALTER TABLE Users ADD COLUMN Role VARCHAR(50) DEFAULT 'Cashier'";
                    using (MySqlCommand cmd = new MySqlCommand(addCol, conn)) { cmd.ExecuteNonQuery(); }
                }

                // Ensure Admin exists
                string checkAdminQuery = "SELECT COUNT(*) FROM Users WHERE Username = 'admin'";
                using (MySqlCommand cmd = new MySqlCommand(checkAdminQuery, conn))
                {
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    if (count == 0)
                    {
                        string insertAdmin = "INSERT INTO Users (Username, Password, Role) VALUES ('admin', 'admin', 'Administrator')";
                        using (MySqlCommand insertCmd = new MySqlCommand(insertAdmin, conn)) { insertCmd.ExecuteNonQuery(); }
                    }
                    else
                    {
                        // Update role/password for existing admin
                        string updateAdmin = "UPDATE Users SET Role = 'Administrator', Password = 'admin' WHERE Username = 'admin'";
                        using (MySqlCommand updateCmd = new MySqlCommand(updateAdmin, conn)) { updateCmd.ExecuteNonQuery(); }
                    }
                }

                // Ensure Cashier exists
                string checkCashierQuery = "SELECT COUNT(*) FROM Users WHERE Username = 'cashier'";
                using (MySqlCommand cmd = new MySqlCommand(checkCashierQuery, conn))
                {
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    if (count == 0)
                    {
                        string insertCashier = "INSERT INTO Users (Username, Password, Role) VALUES ('cashier', '12345', 'Cashier')";
                        using (MySqlCommand insertCmd = new MySqlCommand(insertCashier, conn)) { insertCmd.ExecuteNonQuery(); }
                    }
                }
            }
        }

        public bool Login(string username, string password, string requiredRole)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM Users WHERE Username = @user AND Password = @pass AND Role = @role";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@user", username);
                    cmd.Parameters.AddWithValue("@pass", password);
                    cmd.Parameters.AddWithValue("@role", requiredRole);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        public void AddUser(string username, string password, string role)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO Users (Username, Password, Role) VALUES (@user, @pass, @role)";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@user", username);
                    cmd.Parameters.AddWithValue("@pass", password);
                    cmd.Parameters.AddWithValue("@role", role);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public System.Data.DataTable GetAllUsers()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT Id, Username, Password, Role FROM Users";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        System.Data.DataTable dt = new System.Data.DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }
    }
}
