using Hastane.Filters;
using Hastane.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for HastaEkle.xaml
    /// </summary>
    public partial class HastaEkle : UserControl
    {
        public HastaEkle()
        {
            InitializeComponent();
            LoadStaticDataAsync();
        }

        private HastaInsert GetInsert()
        {
            return new HastaInsert()
            {
                KimlikNo = txtKimlikNo.Text,
                Ad = txtAd.Text,
                Soyad = txtSoyad.Text,
                DogumTarihi = DateOnly.FromDateTime(dpDogumTarihi.SelectedDate.Value),
                CinsiyetId = cmbCinsiyet.SelectedIndex + 1, // Database Id starts from 1, 
                Telefon = txtTelefon.Text,
                Eposta = txtEposta.Text,
                KanGrubuId = cmbKanGrubu.SelectedIndex + 1
            };
        }

        private async Task InsertAsync()
        {
            HastaInsert insert = GetInsert();
            SqlParameter[] parameters;
            string query = insert.GetQuery(out parameters);
            await DatabaseService.ExecuteNonQueryAsync(query, parameters);
        }

        private async Task LoadStaticDataAsync()
        {
            cmbCinsiyet.ItemsSource = await DataService.GetCinsiyetlerAsync(false);
            cmbKanGrubu.ItemsSource = await DataService.GetKanGruplariAsync(false);
        }

        private void ClearForm()
        {
            txtKimlikNo.Text = string.Empty;
            txtAd.Text = string.Empty;
            txtSoyad.Text = string.Empty;
            dpDogumTarihi.SelectedDate = null;
            cmbCinsiyet.SelectedIndex = -1;
            txtTelefon.Text = string.Empty;
            txtEposta.Text = string.Empty;
            cmbKanGrubu.SelectedIndex = -1;
        }

        private bool IsFormValid()
        {
            if (!DataService.IsNationalIdValid(txtKimlikNo.Text))
            {
                MessageBox.Show("Gecersiz KimlikNo");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtAd.Text))
            {
                MessageBox.Show("Gecersiz Ad");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtSoyad.Text))
            {
                MessageBox.Show("Gecersiz Soyad");
                return false;
            }
            if (dpDogumTarihi.SelectedDate == null)
            {
                MessageBox.Show("Gecersiz Dogum Tarihi");
                return false;
            }
            if (cmbCinsiyet.SelectedIndex < 0)
            {
                MessageBox.Show("Gecersiz Cinsiyet");
                return false;
            }
            if (!DataService.IsPhoneValid(txtTelefon.Text))
            {
                MessageBox.Show("Gecersiz Telefon Numarasi");
                return false;
            }
            if (!DataService.IsEmailValid(txtEposta.Text))
            {
                MessageBox.Show("Gecersiz Eposta");
                return false;
            }
            if (cmbKanGrubu.SelectedIndex < 0)
            {
                MessageBox.Show("Gecersiz Kan Grubu");
                return false;
            }
            return true;
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

        private void btnIptal_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private async void btnKaydet_Click(object sender, RoutedEventArgs e)
        {
            if (IsFormValid())
            {
                await InsertAsync();
                ClearForm();
            }
        }
    }
}
