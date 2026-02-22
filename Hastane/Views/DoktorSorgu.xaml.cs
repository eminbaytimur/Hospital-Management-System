using Hastane.Filters;
using Hastane.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for DoctorList.xaml
    /// </summary>
    public partial class DoktorSorgu : UserControl
    {
        public DoktorSorgu()
        {
            InitializeComponent();
            LoadStaticDataAsync(); 
            SorgulaAsync();
        }

        private DoktorSelect GetFiltre()
        {
            DoktorSelect filtre = new DoktorSelect();
            if (!string.IsNullOrWhiteSpace(txtKimlikNo.Text))
                filtre.KimlikNo = txtKimlikNo.Text;
            if (!string.IsNullOrWhiteSpace(txtAd.Text))
                filtre.Ad = txtAd.Text;
            if (!string.IsNullOrWhiteSpace(txtSoyad.Text))
                filtre.Soyad = txtSoyad.Text;
            if (cmbCinsiyet.SelectedIndex > 0)
                filtre.CinsiyetId = cmbCinsiyet.SelectedIndex;
            if (!string.IsNullOrWhiteSpace(txtTelefon.Text))
                filtre.Telefon = txtTelefon.Text;
            if (!string.IsNullOrWhiteSpace(txtEposta.Text))
                filtre.Eposta = txtEposta.Text;
            if (cmbBolum.SelectedIndex > 0)
                filtre.BolumId = cmbBolum.SelectedIndex;
            return filtre;
        }

        private async Task SorgulaAsync()
        {
            DoktorSelect filter = GetFiltre();
            SqlParameter[] parameters;
            string query = filter.GetQuery(out parameters);
            DataTable table = await DatabaseService.ExecuteQueryAsync(query, parameters);
            dgDoktor.ItemsSource = table.DefaultView;
        }

        private async Task LoadStaticDataAsync()
        {
            cmbCinsiyet.ItemsSource = await DataService.GetCinsiyetlerAsync();
            cmbBolum.ItemsSource = await DataService.GetBolumlerAsync();
        }

        private void ClearForm()
        {
            txtKimlikNo.Text = string.Empty;
            txtAd.Text = string.Empty;
            txtSoyad.Text = string.Empty;
            cmbCinsiyet.SelectedIndex = -1;
            txtTelefon.Text = string.Empty;
            txtEposta.Text = string.Empty;
            cmbBolum.SelectedIndex = -1;
        }

        private void txtKimlikNo_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
        }

        private void txtTelefon_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textbox = sender as TextBox;
            if (textbox == null)
                return;

            string digitsOnly = new string(textbox.Text.Where(c => char.IsDigit(c)).ToArray());
            string formattedText = "";

            for (int i = 0; i < digitsOnly.Length; i++)
            {
                if (i == 3 || i == 6)
                    formattedText += "-";
                formattedText += digitsOnly[i];
            }

            int caretPosition = textbox.SelectionStart;
            textbox.Text = formattedText;
            textbox.SelectionStart = Math.Max(caretPosition, textbox.Text.Length);
        }

        private async void btnYenile_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
            await LoadStaticDataAsync();
        }

        private async void btnSorgula_Click(object sender, RoutedEventArgs e)
        {
            await SorgulaAsync();
        }

        private void dgDoktor_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column is DataGridColumn && e.PropertyName == "DogumTarihi")
            {
                e.Column = new DataGridTextColumn()
                {
                    Header = e.Column.Header,
                    Binding = new Binding(e.PropertyName)
                    {
                        StringFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern
                    }
                };
            }
            else if (e.Column is DataGridColumn && (e.PropertyName == "PhotoPath"))
            {
                e.Column.Visibility = Visibility.Collapsed;
            }
        }
    }
}
