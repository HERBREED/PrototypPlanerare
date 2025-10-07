using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using PrototypPlanerare.Data;

namespace PrototypPlanerare
{
    public partial class App : Application
    {
        private Window m_window;  // <-- this is the field those errors were about

        public App()
        {
            this.InitializeComponent();
            this.RequestedTheme = ApplicationTheme.Light;  // <-- force Light
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            using (var db = new AppDbContext())
            {
                db.Database.Migrate();
                System.Diagnostics.Debug.WriteLine("[EF] ConnString: " + db.Database.GetDbConnection().ConnectionString);
                DbSeeder.Seed(db);
            }

            m_window = new MainWindow();
            m_window.Activate();
        }
    }
}
