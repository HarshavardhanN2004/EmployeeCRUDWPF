using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows;

namespace EmployeeCRUD
{
    public partial class Delete : Window
    {
        string cs = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;

        int empId;

        public Delete(int id)
        {
            InitializeComponent();
            empId = id;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(cs))
                {
                    SqlCommand cmd = new SqlCommand("sp_DeleteEmployee", con);

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Id", empId);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Employee Deleted Successfully");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Delete Error: " + ex.Message);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}