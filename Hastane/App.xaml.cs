using System.Configuration;
using System.Data;
using System.Globalization;
using System.Windows;

namespace Hastane
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            CultureInfo turkishCulture = new CultureInfo("tr-TR")
            {
                DateTimeFormat = { ShortDatePattern = "dd.MM.yyyy", LongTimePattern = "HH:mm:ss" }
            };

            Thread.CurrentThread.CurrentCulture = turkishCulture;
            Thread.CurrentThread.CurrentUICulture = turkishCulture;
        }
    }

}
