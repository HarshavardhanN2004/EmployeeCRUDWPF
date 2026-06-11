using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Data;

namespace EmployeeCRUD
{
    public class SalaryReportGenerator
    {
        public static void Generate(DataTable dt, string filePath)
        {
            decimal totalSalary = 0;

            foreach (DataRow row in dt.Rows)
            {
                if (row["NetSalary"] != DBNull.Value)
                {
                    totalSalary = totalSalary + Convert.ToDecimal(row["NetSalary"]);
                }
            }

            decimal averageSalary = 0;

            if (dt.Rows.Count > 0)
            {
                averageSalary = totalSalary / dt.Rows.Count;
            }

            Document.Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);

                    page.Header()
                    .Column(column =>
                    {
                        column.Item() .Text("Employee Salary Report") .FontSize(18);

                        column.Item().Text("Generated Date : "
                        + DateTime.Now.ToString("dd-MM-yyyy HH:mm"));

                    });

                    page.Content()
                    .Column(column =>
                    {
                        column.Item()
                        .Table(table =>
                        {

                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn();
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Border(1).Padding(5).Text("ID");
                                header.Cell().Border(1).Padding(5).Text("Name");
                                header.Cell().Border(1).Padding(5).Text("Department");
                                header.Cell().Border(1).Padding(5).Text("Month");
                                header.Cell().Border(1).Padding(5).Text("Days");
                                header.Cell().Border(1).Padding(5).Text("Gross");
                                header.Cell().Border(1).Padding(5).Text("Deduction");
                                header.Cell().Border(1).Padding(5).Text("Net Salary");

                            });

                            foreach (DataRow row in dt.Rows)
                            {
                                table.Cell().Border(1).Padding(5).Text(row["EmpId"].ToString());
                                table.Cell().Border(1).Padding(5).Text(row["Name"].ToString());
                                table.Cell().Border(1).Padding(5).Text(row["Department"].ToString());

                                int month = 1;
                                int year = DateTime.Now.Year;


                                if (row["SalaryMonth"] != DBNull.Value)
                                {
                                    month = Convert.ToInt32(row["SalaryMonth"]);
                                }


                                if (row["SalaryYear"] != DBNull.Value)
                                {
                                    year = Convert.ToInt32(row["SalaryYear"]);
                                }

                                string monthName = new DateTime(year, month, 1)
                                .ToString("MMMM yyyy");

                                table.Cell().Border(1).Padding(5).Text(monthName);

                                table.Cell().Border(1).Padding(5).Text(row["WorkingDays"].ToString());

                                decimal gross = 0;

                                if (row["GrossSalary"] != DBNull.Value)
                                {
                                    gross = Convert.ToDecimal(row["GrossSalary"]);
                                }

                                table.Cell() .Border(1) .Padding(5) .Text(gross.ToString("N2"));

                                decimal deduction = 0;

                                if (row["Deduction"] != DBNull.Value)
                                {
                                    deduction = Convert.ToDecimal(row["Deduction"]);
                                }

                                table.Cell().Border(1).Padding(5) .Text(deduction.ToString("N2"));

                                decimal net = 0;

                                if (row["NetSalary"] != DBNull.Value)
                                {
                                    net = Convert.ToDecimal(row["NetSalary"]);
                                }
                                table.Cell().Border(1).Padding(5).Text(net.ToString("N2"));
                            }
                        });

                        column.Item().PaddingTop(20);

                        column.Item().Text("REPORT SUMMARY").FontSize(16).Bold();

                        column.Item() .Text("Total Employees : " + dt.Rows.Count);

                        column.Item()  .Text("Total Salary Paid : ₹ "  + totalSalary.ToString("N2"));

                        column.Item().Text("Average Salary : ₹ " + averageSalary.ToString("N2"));

                    });

                    page.Footer() .AlignCenter() .Text( "Generated on : " + DateTime.Now.ToString("dd-MM-yyyy HH:mm"));
                });
            })
            .GeneratePdf(filePath);
        }
    }
}