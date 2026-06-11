using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows;

namespace EmployeeCRUD
{
    public partial class UpdateEmployee : Window
    {
        string cs = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;
        int empId;

        public UpdateEmployee(int id, string name, string age, string dept, string joiningDate, string salary)
        {
            InitializeComponent();

            empId = id;

            txtName.Text = name;
            txtAge.Text = age;
            txtDept.Text = dept;
            txtSalary.Text = salary;

            dpJoiningDate.SelectedDate = Convert.ToDateTime(joiningDate);
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
           
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Name is required");
                txtName.Focus();
                return;
            }

            int age;

            if (string.IsNullOrWhiteSpace(txtAge.Text))
            {
                MessageBox.Show("Age is required");
                txtAge.Focus();
                return;
            }

            if (!int.TryParse(txtAge.Text, out age))
            {
                MessageBox.Show("Age must be a number");
                txtAge.Focus();
                return;
            }

            if (age < 18)
            {
                MessageBox.Show("Age should be greater than or equal to 18");
                txtAge.Focus();
                return;
            }

            if (age > 80)
            {
                MessageBox.Show("Age should not be greater than 80");
                txtAge.Focus();
                return;
            }

            if (age < 0)
            {
                MessageBox.Show("Negative age is not allowed");
                txtAge.Focus();
                return;
            }



            if (string.IsNullOrWhiteSpace(txtDept.Text))
            {
                MessageBox.Show("Department is required");
                txtDept.Focus();
                return;
            }

            if (dpJoiningDate.SelectedDate == null)
            {
                MessageBox.Show("Please select Joining Date");
                dpJoiningDate.Focus();
                return;
            }

            if (dpJoiningDate.SelectedDate > DateTime.Today)
            {
                MessageBox.Show("Joining Date cannot be greater than today's date");
                dpJoiningDate.Focus();
                return;
            }

            decimal salary;

            if (string.IsNullOrWhiteSpace(txtSalary.Text))
            {
                MessageBox.Show("Salary is required");
                txtSalary.Focus();
                return;
            }

            if (!decimal.TryParse(txtSalary.Text, out salary))
            {
                MessageBox.Show("Salary must be a number");
                txtSalary.Focus();
                return;
            }

            if (salary < 0)
            {
                MessageBox.Show("Negative salary is not allowed");
                txtSalary.Focus();
                return;
            }

            if (salary < 5000)
            {
                MessageBox.Show("Minimum salary should be 5000");
                txtSalary.Focus();
                return;
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_UpdateEmployee", con);

                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Id", empId);
                cmd.Parameters.AddWithValue("@Name", txtName.Text);
                cmd.Parameters.AddWithValue("@Age", int.Parse(txtAge.Text));
                cmd.Parameters.AddWithValue("@Department", txtDept.Text);
                cmd.Parameters.AddWithValue("@Salary", decimal.Parse(txtSalary.Text));
                cmd.Parameters.AddWithValue("@JoiningDate",
                                             dpJoiningDate.SelectedDate.Value);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Updated Successfully");

            this.Close();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}