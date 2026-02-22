using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Diagnostics;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Hastane.Filters;
using Hastane.Services;
using MaterialDesignColors;
using Microsoft.Data.SqlClient;
using static MaterialDesignThemes.Wpf.Theme;

namespace Hastane
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            HighlightButton(btnSorgu);
            ContentFrame.Navigate(new Uri("Views/Sorgu.xaml", UriKind.Relative));
        }

        private void Sorgu_Click(object sender, RoutedEventArgs e)
        {
            if (ContentFrame.Content is not Views.Sorgu)
            {
                HighlightButton(btnSorgu);
                ContentFrame.Navigate(new Uri("Views/Sorgu.xaml", UriKind.Relative));
            }
        }

        private void Kayit_Click(object sender, RoutedEventArgs e)
        {
            if (ContentFrame.Content is not Views.Kayit)
            {
                HighlightButton(btnKayit);
                ContentFrame.Navigate(new Uri("Views/Kayit.xaml", UriKind.Relative));
            }
        }

        private void HastaIslemleri_Click(object sender, RoutedEventArgs e)
        {
            if (ContentFrame.Content is not Views.HastaIslemleri)
            {
                HighlightButton(btnHastaIslemleri);
                ContentFrame.Navigate(new Uri("Views/HastaIslemleri.xaml", UriKind.Relative));
            }
        }

        private void HighlightButton(System.Windows.Controls.Button button)
        {
            var highlightedButtonKey = "MaterialDesignFlatDarkBgButton";
            var buttonKey = "MaterialDesignFlatButton";

            btnSorgu.Style = Application.Current.Resources[buttonKey] as Style;
            btnKayit.Style = Application.Current.Resources[buttonKey] as Style;
            btnHastaIslemleri.Style = Application.Current.Resources[buttonKey] as Style;
            btnHastaDetay.Style = Application.Current.Resources[buttonKey] as Style;

            button.Style = Application.Current.Resources[highlightedButtonKey] as Style;
        }

        private void HastaDetay_Click(object sender, RoutedEventArgs e)
        {
            if (ContentFrame.Content is not Views.HastaDetay)
            {
                HighlightButton(btnHastaDetay);
                ContentFrame.Navigate(new Uri("Views/HastaDetay.xaml", UriKind.Relative));
            }
        }
    }
}