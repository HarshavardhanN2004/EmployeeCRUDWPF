using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows;

namespace EmployeeCRUD
{
    public partial class SalaryByDepartment : Window
    {
        string cs = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;

        public SalaryByDepartment()
        {
            InitializeComponent();
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDepartment.Text))
            {
                MessageBox.Show("Please enter a department name");
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(cs))
                {
                    con.Open();

                    string query = "SELECT * FROM vw_SalaryByDepartment where Department like @Department ORDER BY Name, SalaryYear, SalaryMonth";
                               
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Department", "%" + txtDepartment.Text + "%");

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (!reader.HasRows)
                    {
                        MessageBox.Show("No salary records found for this department");
                        dataGrid.ItemsSource = null;
                        reader.Close();
                        return;
                    }

                    List<SalaryRecord> salaryList = new List<SalaryRecord>();

                    while (reader.Read())
                    {
                        int monthNumber = Convert.ToInt32(reader["SalaryMonth"]);
                        string monthName = new DateTime(2000, monthNumber, 1).ToString("MMMM");

                        salaryList.Add(new SalaryRecord
                        {
                            EmpId = Convert.ToInt32(reader["EmpId"]),
                            Name = reader["Name"].ToString(),
                            Department = reader["Department"].ToString(),
                            MonthName = monthName,
                            SalaryYear = Convert.ToInt32(reader["SalaryYear"]),
                            WorkingDays = reader["WorkingDays"] == DBNull.Value ? 0 : Convert.ToInt32(reader["WorkingDays"]),
                            BasicSalary = Convert.ToDecimal(reader["BasicSalary"]),
                            HRA = Convert.ToDecimal(reader["HRA"]),
                            DA = Convert.ToDecimal(reader["DA"]),
                            GrossSalary = Convert.ToDecimal(reader["GrossSalary"]),
                            Deduction = Convert.ToDecimal(reader["Deduction"]),
                            NetSalary = Convert.ToDecimal(reader["NetSalary"])
                        });
                    }

                    reader.Close();

                    dataGrid.ItemsSource = salaryList;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
    public class SalaryRecord
    {
        public int EmpId { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public string MonthName { get; set; }  
        public int SalaryYear { get; set; }
        public int WorkingDays { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal HRA { get; set; }
        public decimal DA { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal Deduction { get; set; }
        public decimal NetSalary { get; set; }
    }
}