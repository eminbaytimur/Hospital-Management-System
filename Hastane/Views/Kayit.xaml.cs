using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Hastane.Views
{
    /// <summary>
    /// Interaction logic for Kayit.xaml
    /// </summary>
    public partial class Kayit : Page
    {
        public Kayit()
        {
            InitializeComponent();
            HighlightButton(btnDoktor);
            ContentFrame.Navigate(new Uri("Views/DoktorEkle.xaml", UriKind.Relative));
        }

        private void Doktor_Ekle(object sender, RoutedEventArgs e)
        {
            if (ContentFrame.Content is not Views.DoktorEkle)
            {
                HighlightButton(btnDoktor);
                ContentFrame.Navigate(new Uri("Views/DoktorEkle.xaml", UriKind.Relative));
            }
        }

        private void Hasta_Ekle(object sender, RoutedEventArgs e)
        {
            if (ContentFrame.Content is not Views.HastaEkle)
            {
                HighlightButton(btnHasta);
                ContentFrame.Navigate(new Uri("Views/HastaEkle.xaml", UriKind.Relative));
            }
        }

        private void HighlightButton(System.Windows.Controls.Button button)
        {
            var buttonKey = "MaterialDesignFlatButton";
            var highlightedButtonKey = "MaterialDesignFlatSecondaryLightBgButton";
            var foregroundKey = "MaterialDesign.Brush.Primary.Dark.Foreground";
            var highlightedForegroundKey = "MaterialDesign.Brush.Primary.Light.Foreground";

            btnDoktor.Style = Application.Current.Resources[buttonKey] as Style;
            btnHasta.Style = Application.Current.Resources[buttonKey] as Style;
            btnDoktor.Foreground = Application.Current.Resources[foregroundKey] as Brush;
            btnHasta.Foreground = Application.Current.Resources[foregroundKey] as Brush;

            button.Style = Application.Current.Resources[highlightedButtonKey] as Style;
            button.Foreground = Application.Current.Resources[highlightedForegroundKey] as Brush;
        }
    }
}