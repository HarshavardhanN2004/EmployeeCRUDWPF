using System;
using System.Windows;

namespace EmployeeCRUD
{
    public partial class SalarySlip : Window
    {
        public SalarySlip(int id, string name, string dept, DateTime joiningDate,
                          decimal basic, decimal hra, decimal da, decimal pf, decimal tax,
                          decimal gross, decimal deduction, decimal net,
                          int month, int year)
        {
            InitializeComponent();

            txtId.Text = id.ToString();
            txtName.Text = name;
            txtDept.Text = dept;
            txtJoiningDate.Text = joiningDate.ToString("dd-MM-yyyy");

            txtMonth.Text = new DateTime(year, month, 1).ToString("MMMM yyyy");

            txtBasic.Text = "₹ " + basic;
            txtHRA.Text = "₹ " + hra;
            txtDA.Text = "₹ " + da;
            txtPF.Text = "₹ " + pf;
            txtTax.Text = "₹ " + tax;
            txtGross.Text = "₹ " + gross;
            txtNet.Text = "₹ " + net;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}