using System.Windows;
using QuestPDF.Infrastructure;

namespace EmployeeCRUD
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            base.OnStartup(e);
        }
    }
}