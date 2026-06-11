using System;
using System.Windows;

namespace EmployeeCRUD
{
    public partial class SelectMonth : Window
    {
        public int SelectedMonth { get; set; }  
        public int SelectedYear { get; set; }   
        public bool Confirmed { get; set; } = false;  
        public SelectMonth()
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
                cmbMonth.Items.Add(new MonthItem
                {
                    Display = months[i],  
                    Value = i + 1        
                });
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
        private void Proceed_Click(object sender, RoutedEventArgs e)
        {
            if (cmbMonth.SelectedItem == null || cmbYear.SelectedItem == null)
            {
                MessageBox.Show("Please select both month and year");
                return;
            }

            SelectedMonth = ((MonthItem)cmbMonth.SelectedItem).Value;

            SelectedYear = (int)cmbYear.SelectedItem;

            Confirmed = true;

            this.Close();
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
    public class MonthItem
    {
        public string Display { get; set; }  
        public int Value { get; set; }
        public override string ToString()
        {
            return Display;
        }
    }
}