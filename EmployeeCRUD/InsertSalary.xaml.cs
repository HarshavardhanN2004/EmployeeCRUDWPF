using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows;

namespace EmployeeCRUD
{
    public partial class InsertSalary : Window
    {
        string cs = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;

        public InsertSalary()
        {
            InitializeComponent();
            LoadMonths();
            LoadYears();
        }

        private void LoadMonths()
        {
            string[] months = {
                "January", "February", "March", "April",
                "May", "June", "July", "August",
                "September", "October", "November", "December"
            };

            for (int i = 0; i < months.Length; i++)
            {
                cmbMonth.Items.Add(new MonthItem { Display = months[i], Value = i + 1 });
            }

           
            cmbMonth.SelectedIndex = DateTime.Now.Month - 1;
        }

        private void LoadYears()
        {
            int currentYear = DateTime.Now.Year;

            for (int y = currentYear - 2; y <= currentYear; y++)
            {
                cmbYear.Items.Add(y);
            }

            cmbYear.SelectedItem = currentYear;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmpId.Text))
            {
                MessageBox.Show("Please enter Employee ID");
                return;
            }

            int empId;
            if (!int.TryParse(txtEmpId.Text, out empId))
            {
                MessageBox.Show("Employee ID must be a number");
                return;
            }

            if (cmbMonth.SelectedItem == null || cmbYear.SelectedItem == null)
            {
                MessageBox.Show("Please select month and year");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtSalary.Text))
            {
                MessageBox.Show("Please enter Basic Salary");
                return;
            }

            decimal basicSalary;
            if (!decimal.TryParse(txtSalary.Text, out basicSalary))
            {
                MessageBox.Show("Salary must be a number");
                return;
            }

            if (basicSalary <= 0)
            {
                MessageBox.Show("Salary must be greater than zero");
                return;
            }

            if (basicSalary < 5000)
            {
                MessageBox.Show("Minimum salary should be 5000");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtWorkingDays.Text))
            {
                MessageBox.Show("Please enter number of working days present");
                return;
            }

            int workingDays;
            if (!int.TryParse(txtWorkingDays.Text, out workingDays))
            {
                MessageBox.Show("Working days must be a number");
                return;
            }

            if (workingDays <= 0)
            {
                MessageBox.Show("Working days must be greater than zero");
                return;
            }

            int selectedMonth = ((MonthItem)cmbMonth.SelectedItem).Value;
            int selectedYear = (int)cmbYear.SelectedItem;

            int totalDays = DateTime.DaysInMonth(selectedYear, selectedMonth);

            if (workingDays > totalDays)
            {
                MessageBox.Show("Working days cannot be more than " + totalDays +
                                " days for the selected month");
                return;
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                string empQuery = "SELECT Name, JoiningDate FROM Employees WHERE Id = @Id";
                SqlCommand empCmd = new SqlCommand(empQuery, con);
                empCmd.Parameters.AddWithValue("@Id", empId);

                SqlDataReader reader = empCmd.ExecuteReader();

                if (!reader.HasRows)
                {
                    MessageBox.Show("Employee not found!");
                    reader.Close();
                    return;
                }

                reader.Read();
                string empName = reader["Name"].ToString();
                DateTime joiningDate = Convert.ToDateTime(reader["JoiningDate"]);
                reader.Close();

                DateTime selectedDate = new DateTime(selectedYear, selectedMonth, 1);

                DateTime joiningMonthStart = new DateTime(
                    joiningDate.Year, joiningDate.Month, 1);

                if (selectedDate < joiningMonthStart)
                {
                    MessageBox.Show("Cannot insert salary for " + empName +
                                    "!\nSelected month is before their joining date." +
                                    "\nJoining Date : " + joiningDate.ToString("dd-MM-yyyy") +
                                    "\nSalary can only be inserted from : " +
                                    joiningMonthStart.ToString("MMMM yyyy") + " onwards.");
                    return;
                }

                string checkQuery = "SELECT COUNT(*) FROM Salary " +
                                    "WHERE EmpId = @Id AND SalaryMonth = @Month AND SalaryYear = @Year";

                SqlCommand checkCmd = new SqlCommand(checkQuery, con);
                checkCmd.Parameters.AddWithValue("@Id", empId);
                checkCmd.Parameters.AddWithValue("@Month", selectedMonth);
                checkCmd.Parameters.AddWithValue("@Year", selectedYear);

                int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (count > 0)
                {
                    MessageBox.Show("Salary already inserted for " + empName + " for the selected month!");
                    return;
                }

                decimal perDaySalary = basicSalary / totalDays;

                decimal earnedSalary = perDaySalary * workingDays;

                decimal hra = earnedSalary * 20 / 100;  
                decimal da = earnedSalary * 10 / 100; 
                decimal pf = earnedSalary * 8 / 100; 
                decimal tax = earnedSalary * 5 / 100;  
                decimal gross = earnedSalary + hra + da;  
                decimal deduction = pf + tax;                 
                decimal net = gross - deduction;       

                string insertQuery = "INSERT INTO Salary " +
                                     "(EmpId, BasicSalary, HRA, DA, PF, Tax, GrossSalary, Deduction, NetSalary, SalaryMonth, SalaryYear, WorkingDays) " +
                                     "VALUES " +
                                     "(@EmpId, @Basic, @HRA, @DA, @PF, @Tax, @Gross, @Deduction, @Net, @Month, @Year, @WorkingDays)";

                SqlCommand insertCmd = new SqlCommand(insertQuery, con);
                insertCmd.Parameters.AddWithValue("@EmpId", empId);
                insertCmd.Parameters.AddWithValue("@Basic", earnedSalary);
                insertCmd.Parameters.AddWithValue("@HRA", hra);
                insertCmd.Parameters.AddWithValue("@DA", da);
                insertCmd.Parameters.AddWithValue("@PF", pf);
                insertCmd.Parameters.AddWithValue("@Tax", tax);
                insertCmd.Parameters.AddWithValue("@Gross", gross);
                insertCmd.Parameters.AddWithValue("@Deduction", deduction);
                insertCmd.Parameters.AddWithValue("@Net", net);
                insertCmd.Parameters.AddWithValue("@Month", selectedMonth);
                insertCmd.Parameters.AddWithValue("@Year", selectedYear);
                insertCmd.Parameters.AddWithValue("@WorkingDays", workingDays);

                insertCmd.ExecuteNonQuery();

                MessageBox.Show("Salary inserted successfully ");
                this.Close();
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}