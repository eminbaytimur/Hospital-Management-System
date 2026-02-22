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
    /// Interaction logic for Sorgular.xaml
    /// </summary>
    public partial class Sorgu : Page
    {
        public Sorgu()
        {
            InitializeComponent();
            HighlightButton(btnDoktorSorgu);
            ContentFrame.Navigate(new Uri("Views/DoktorSorgu.xaml", UriKind.Relative));
        }

        private void Doktor_Sorgu(object sender, RoutedEventArgs e)
        {
            if (ContentFrame.Content is not Views.DoktorSorgu)
            {
                HighlightButton(btnDoktorSorgu);
                ContentFrame.Navigate(new Uri("Views/DoktorSorgu.xaml", UriKind.Relative));
            }
        }

        private void Hasta_Sorgu(object sender, RoutedEventArgs e)
        {
            if (ContentFrame.Content is not Views.HastaSorgu)
            {
                HighlightButton(btnHastaSorgu);
                ContentFrame.Navigate(new Uri("Views/HastaSorgu.xaml", UriKind.Relative));
            }
        }

        private void HighlightButton(System.Windows.Controls.Button button)
        {
            var buttonKey = "MaterialDesignFlatButton";
            var highlightedButtonKey = "MaterialDesignFlatSecondaryLightBgButton";
            var foregroundKey = "MaterialDesign.Brush.Primary.Dark.Foreground";
            var highlightedForegroundKey = "MaterialDesign.Brush.Primary.Light.Foreground";

            btnDoktorSorgu.Style = Application.Current.Resources[buttonKey] as Style;
            btnHastaSorgu.Style = Application.Current.Resources[buttonKey] as Style;
            btnDoktorSorgu.Foreground = Application.Current.Resources[foregroundKey] as Brush;
            btnHastaSorgu.Foreground = Application.Current.Resources[foregroundKey] as Brush;

            button.Style = Application.Current.Resources[highlightedButtonKey] as Style;
            button.Foreground = Application.Current.Resources[highlightedForegroundKey] as Brush;
        }
    }
}