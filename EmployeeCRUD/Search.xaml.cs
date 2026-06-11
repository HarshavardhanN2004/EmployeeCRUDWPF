using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows;

namespace EmployeeCRUD
{
    public partial class Search : Window
    {
        string cs = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;

        public Search()
        {
            InitializeComponent();
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                MessageBox.Show("Enter name to search");
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(cs))
                {
                    string query = "SELECT Id, Name, Age, Department, Salary, " +
                                    "CONVERT(VARCHAR(10), JoiningDate, 103) AS JoiningDate " +
                                     "FROM Employees WHERE Name LIKE @Name";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Name", "%" + txtSearch.Text + "%");

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();

                        adapter.Fill(dt);

                        if (dt.Rows.Count == 0)
                        {
                            MessageBox.Show("No records found");
                            dataGrid.ItemsSource = null;
                        }
                        else
                        {
                            dataGrid.ItemsSource = dt.DefaultView;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Search Error: " + ex.Message);
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }   
    }
}