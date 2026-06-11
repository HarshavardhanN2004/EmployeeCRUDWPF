using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows;

namespace EmployeeCRUD
{
    public partial class AddEmployee : Window
    {
        string cs = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;

        public AddEmployee()
        {
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!IsValid()) return;

            try
            {
                using (SqlConnection con = new SqlConnection(cs))
                {
                    SqlCommand cmd = new SqlCommand("sp_InsertEmployee", con);

                   
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Name", txtName.Text);
                    cmd.Parameters.AddWithValue("@Age", int.Parse(txtAge.Text));
                    cmd.Parameters.AddWithValue("@Department", txtDept.Text);
                    cmd.Parameters.AddWithValue("@Salary", decimal.Parse(txtSalary.Text));
                    cmd.Parameters.AddWithValue("@JoiningDate",
                                                 dpJoiningDate.SelectedDate.Value);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Employee Added Successfully");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private bool IsValid()
        {
            if (txtName.Text == "")
            {
                MessageBox.Show("Enter Name");
                return false;
            }

            if (txtAge.Text == "")
            {
                MessageBox.Show("Enter Age");
                return false;
            }

            if (txtDept.Text == "")
            {
                MessageBox.Show("Enter Department");
                return false;
            }

            if (dpJoiningDate.SelectedDate == null)
            {
                MessageBox.Show("Select Joining Date");
                return false;
            }

            if (dpJoiningDate.SelectedDate > DateTime.Today)
            {
                MessageBox.Show("Joining Date cannot be greater than today's date");
                return false;
            }

            if (txtSalary.Text == "")
            {
                MessageBox.Show("Enter Salary");
                return false;
            }

            int age;

            if (!int.TryParse(txtAge.Text, out age))
            {
                MessageBox.Show("Age must be a number");
                return false;
            }

            if (age < 18)
            {
                MessageBox.Show("Age should be greater than or equal to 18");
                return false;
            }

            if (age > 80)
            {
                MessageBox.Show("Age should not be greater than 80");
                return false;
            }

            if (age < 0)
            {
                MessageBox.Show("Negative age is not allowed");
                return false;
            }

            decimal salary;

            if (!decimal.TryParse(txtSalary.Text, out salary))
            {
                MessageBox.Show("Salary must be a number");
                return false;
            }

            if (salary < 0)
            {
                MessageBox.Show("Negative salary is not allowed");
                return false;
            }

            if (salary < 5000)
            {
                MessageBox.Show("Minimum salary should be 5000");
                return false;
            }

            return true;
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }   
    }
}