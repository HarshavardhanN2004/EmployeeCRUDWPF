using System;
using System.Configuration;
using System.Linq;
using System.Windows;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.IO;

namespace EmployeeCRUD
{
    public partial class MainWindow : Window
    {
        string cs = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;
        public MainWindow()
        {
            InitializeComponent();

            LoadEmployees();
        }
        private DataTable GetDepartmentReportData(string department)
        {
            DataTable dt = new DataTable();

            using SqlConnection con =
                new SqlConnection(cs);

            string query = @"SELECT s.EmpId,e.Name,e.Department,s.WorkingDays,s.GrossSalary,
                            s.Deduction,s.NetSalary,s.SalaryMonth,s.SalaryYear
                            FROM Salary s
                            INNER JOIN Employees e
                            ON s.EmpId = e.Id
                            WHERE e.Department = @Department
                            ORDER BY e.Name";

            SqlDataAdapter da = new SqlDataAdapter(query, con);

            da.SelectCommand.Parameters.AddWithValue("@Department",department);

            da.Fill(dt);

            return dt;
        }
        private void LoadEmployees()
        {
            try
            {
                SqlConnection con = new SqlConnection(cs);

                string query = "SELECT Id, Name, Age, Department, CONVERT(VARCHAR(10), JoiningDate, 103) AS JoiningDate, Salary FROM Employees";

                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                DataTable dt = new DataTable();

                adapter.Fill(dt);

                var sortedData = dt.AsEnumerable()
                                   .OrderBy(row => row["Name"].ToString());

                if (sortedData.Any())
                {
                    dataGrid.ItemsSource = sortedData.CopyToDataTable().DefaultView;
                }
                else
                {
                    dataGrid.ItemsSource = null;
                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Load Error : " + ex.Message);
            }
        }
        private void OpenAddForm_Click(object sender, RoutedEventArgs e)
        {
            AddEmployee win = new AddEmployee();

            win.ShowDialog();

            LoadEmployees();
        }
        private void OpenUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem == null)
            {
                MessageBox.Show("Select a row first");
                return;
            }
            DataRowView row = (DataRowView)dataGrid.SelectedItem;

            UpdateEmployee win = new UpdateEmployee(
                Convert.ToInt32(row["Id"]),
                row["Name"].ToString(),
                row["Age"].ToString(),
                row["Department"].ToString(),
                Convert.ToDateTime(row["JoiningDate"]).ToString("dd-MM-yyyy"),
                row["Salary"].ToString()
            );

            win.ShowDialog();

            LoadEmployees();
        }

        private void OpenDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem == null)
            {
                MessageBox.Show("Select a row first");
                return;
            }
            DataRowView row = (DataRowView)dataGrid.SelectedItem;

            int id = Convert.ToInt32(row["Id"]);

            Delete win = new Delete(id);

            win.ShowDialog();

            LoadEmployees();
        }
        private void OpenSearch_Click(object sender, RoutedEventArgs e)
        {
            Search win = new Search();

            win.ShowDialog();
        }
        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadEmployees();
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void dataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (dataGrid.SelectedItem == null)
                return;

            System.Data.DataRowView row =
                (System.Data.DataRowView)dataGrid.SelectedItem;

            EmployeeDetails win = new EmployeeDetails(
     Convert.ToInt32(row["Id"]),
     row["Name"].ToString(),
     row["Age"].ToString(),
     row["Department"].ToString(),
     Convert.ToDateTime(row["JoiningDate"]).ToString("dd-MM-yyyy"),
     row["Salary"].ToString()
 );

            win.ShowDialog();

            LoadEmployees();
        }

        private void InsertSalary_Click(object sender, RoutedEventArgs e)
        {
            InsertSalary win = new InsertSalary();
            win.ShowDialog();
        }

        private void GenerateSalarySlip_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem == null)
            {
                MessageBox.Show("Select an employee first");
                return;
            }

            DataRowView row = (DataRowView)dataGrid.SelectedItem;
            int employeeId = Convert.ToInt32(row["Id"]);

            SelectMonth selectWin = new SelectMonth();
            selectWin.ShowDialog();

            if (!selectWin.Confirmed) return;

            int selectedMonth = selectWin.SelectedMonth;
            int selectedYear = selectWin.SelectedYear;

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                string fetchQuery = "SELECT * FROM Salary " +
                                    "WHERE EmpId = @Id AND SalaryMonth = @Month AND SalaryYear = @Year";

                SqlCommand fetchCmd = new SqlCommand(fetchQuery, con);
                fetchCmd.Parameters.AddWithValue("@Id", employeeId);
                fetchCmd.Parameters.AddWithValue("@Month", selectedMonth);
                fetchCmd.Parameters.AddWithValue("@Year", selectedYear);

                SqlDataReader dr = fetchCmd.ExecuteReader();

                if (!dr.HasRows)
                {
                    MessageBox.Show("No salary found for this month. Please insert salary first!");
                    dr.Close();
                    return;
                }

                dr.Read();

                decimal basic = Convert.ToDecimal(dr["BasicSalary"]);
                decimal hra = Convert.ToDecimal(dr["HRA"]);
                decimal da = Convert.ToDecimal(dr["DA"]);
                decimal pf = Convert.ToDecimal(dr["PF"]);
                decimal tax = Convert.ToDecimal(dr["Tax"]);
                decimal gross = Convert.ToDecimal(dr["GrossSalary"]);
                decimal deduction = Convert.ToDecimal(dr["Deduction"]);
                decimal net = Convert.ToDecimal(dr["NetSalary"]);
                dr.Close();

                SalarySlip win = new SalarySlip(
                    employeeId,
                    row["Name"].ToString(),
                    row["Department"].ToString(),
                    Convert.ToDateTime(row["JoiningDate"]),
                    basic, hra, da, pf, tax, gross, deduction, net,
                    selectedMonth, selectedYear
                );

                win.ShowDialog();
            }
        }

        private void SalaryByDepartment_Click(object sender, RoutedEventArgs e)
        {
            SalaryByDepartment win = new SalaryByDepartment();
            win.ShowDialog();
        }

        private void MonthlyReport_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem == null)
            {
                MessageBox.Show("Please select an employee first from the list");
                return;
            }

            DataRowView row = (DataRowView)dataGrid.SelectedItem;
            int employeeId = Convert.ToInt32(row["Id"]);
            string empName = row["Name"].ToString();

            DataTable dt = GetAllMonthsSalaryForEmployee(employeeId);

            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("No salary records found for " + empName +
                                ". Please insert salary first!");
                return;
            }

            string filePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                $"{empName}_AllMonths_Salary.pdf");

            SalaryReportGenerator.Generate(dt, filePath);

            MessageBox.Show("Report generated successfully for " + empName + "!");

            Process.Start(new ProcessStartInfo(filePath)
            {
                UseShellExecute = true
            });
        }

        private DataTable GetAllMonthsSalaryForEmployee(int empId)
        {
            DataTable dt = new DataTable();

            try
            {
                using SqlConnection con = new SqlConnection(cs);

                string query = @"
            SELECT
                s.EmpId,
                e.Name,
                e.Department,
                s.WorkingDays,
                s.GrossSalary,
                s.Deduction,
                s.NetSalary,
                s.SalaryMonth,
                s.SalaryYear
            FROM Salary s
            INNER JOIN Employees e ON s.EmpId = e.Id
            WHERE s.EmpId = @EmpId
            ORDER BY s.SalaryYear, s.SalaryMonth";
               
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.SelectCommand.Parameters.AddWithValue("@EmpId", empId);

                da.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Report Error: " + ex.Message);
            }

            return dt;
        }
        private void DepartmentReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using SqlConnection con = new SqlConnection(cs);

                con.Open();

                string deptQuery =
                    "SELECT DISTINCT Department FROM Employees";

                SqlCommand cmd =
                    new SqlCommand(deptQuery, con);

                SqlDataReader dr = cmd.ExecuteReader();

                List<string> departments =
                    new List<string>();

                while (dr.Read())
                {
                    departments.Add(
                        dr["Department"].ToString());
                }

                dr.Close();

                foreach (string dept in departments)
                {
                    DataTable dt = GetDepartmentReportData(dept);

                    if (dt.Rows.Count == 0)
                        continue;

                    string safeDept =
                        dept.Replace(" ", "_");

                    string filePath =
                        Path.Combine(
                            Environment.GetFolderPath(
                                Environment.SpecialFolder.Desktop),
                            $"{safeDept}_Report.pdf");

                        SalaryReportGenerator.Generate(dt, filePath);

                    if (File.Exists(filePath))
                    {
                        Process.Start(new ProcessStartInfo(filePath)
                        {
                            UseShellExecute = true
                        });
                    }
                }

                MessageBox.Show(
                    $"{departments.Count} Department Reports Generated Successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}