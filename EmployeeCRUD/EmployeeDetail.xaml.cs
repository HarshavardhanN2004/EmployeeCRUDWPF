using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows;

namespace EmployeeCRUD
{
    public partial class EmployeeDetails : Window
    {
        string cs = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;

        int empId;

        public EmployeeDetails(int id, string name, string age,
                               string dept, string joiningDate, string salary)
        {
            InitializeComponent();

            empId = id;

            txtName.Text = name;
            txtAge.Text = age;
            txtDept.Text = dept;
            txtJoiningDate.Text = joiningDate;
            txtSalary.Text = salary;
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            UpdateEmployee win = new UpdateEmployee(
                empId,
                txtName.Text,
                txtAge.Text,
                txtDept.Text,
                txtJoiningDate.Text,
                txtSalary.Text
            );

            win.ShowDialog();

            this.Close();
        }
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            Delete win = new Delete(empId);
            win.ShowDialog();

            this.Close();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}