using Hastane.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// Interaction logic for HastaIslemleri.xaml
    /// </summary>
    public partial class HastaIslemleri : Page
    {
        private static List<Tuple<int, int, string>> doktorlarFull;
        public HastaIslemleri()
        {
            InitializeComponent();
            LoadStaticData();
        }

        private async Task LoadStaticData()
        {
            doktorlarFull = await GetDoktorlarFullAsync();

            cmbRandevuBolumler.ItemsSource = await DataService.GetBolumlerAsync(false);
            cmbAmeliyatBolumler.ItemsSource = cmbRandevuBolumler.ItemsSource;
        }

        private List<string> FilterDoctorlar(int bolumId)
        {
            return doktorlarFull.Where(d => d.Item2 == bolumId)
                .Select(d => d.Item3)
                .ToList();    
        }

        private async Task<List<Tuple<int, int, string>>> GetDoktorlarFullAsync()
        {
            string query = $@"
                SELECT
                    Id,
                    BolumId,
                    CONCAT(Ad, ' ', Soyad) AS TamAd
                FROM
                    Doktor
            ";
            DataTable table = await DatabaseService.ExecuteQueryAsync(query);

            List<Tuple<int, int, string>> doktorlar = new();
            foreach (DataRow row in table.Rows)
                doktorlar.Add(new(Convert.ToInt32(row["Id"]), Convert.ToInt32(row["BolumId"]), row["TamAd"].ToString()));

            return doktorlar;
        }

        private int GetDoktorId(string tamAd, int bolumId)
        {
            foreach (var doktorTuple in doktorlarFull)
                if (doktorTuple.Item2 == bolumId && doktorTuple.Item3 == tamAd)
                    return doktorTuple.Item1;

            return 0;
        }

        private void txtHastaId_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
        }

        private async void cmbRandevuBolumler_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbRandevuDoktorlar.ItemsSource = FilterDoctorlar(cmbRandevuBolumler.SelectedIndex + 1);
        }

        private DateTime GetTarihSaat(DateTime? tarih, DateTime? saat)
        {
            if (tarih.HasValue && saat.HasValue)
                return tarih.Value.Date.AddHours(saat.Value.Hour).AddMinutes(saat.Value.Minute);
            else
                return DateTime.Now.AddDays(7);
        }

        private async void btnRandevuEkle_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtHastaId.Text))
            {
                MessageBox.Show("Lutfen bir Id giriniz", "Hasta Id", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            if (cmbRandevuBolumler.SelectedIndex < 0)
            {
                MessageBox.Show("Lutfen bir bolum seciniz", "Bolum Secimi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cmbRandevuDoktorlar.SelectedIndex < 0)
            {
                MessageBox.Show("Lutfen bir doktor seciniz", "Doktor Secimi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int hastaId = Convert.ToInt32(txtHastaId.Text);
            int doktorId = GetDoktorId(cmbRandevuDoktorlar.SelectedValue.ToString(), cmbRandevuBolumler.SelectedIndex + 1);
            int bolumId = cmbRandevuBolumler.SelectedIndex + 1;
            int randevuDurumuId = 1; // varsayilan olarak "Beklemede" degeri atanir
            DateTime tarihSaat = GetTarihSaat(dpRandevuTarih.SelectedDate, tpRandevuSaat.SelectedTime);

            if (hastaId <= 0)
            {
                MessageBox.Show("Gecersiz deger: Hasta Id");
                return;
            }
            if (doktorId <= 0)
            {
                MessageBox.Show("Gecersiz Doktor secimi");
                return;
            }
            if (bolumId <= 0)
            {
                MessageBox.Show("Gecersiz Bolum secimi");
                return;
            }
                
            string query = $@"
                INSERT INTO Randevu
                (TarihSaat, RandevuDurumuId, HastaId, BolumId, DoktorId)
                VALUES
                (@TarihSaat, @RandevuDurumuId, @HastaId, @BolumId, @DoktorId)
            ";

            SqlParameter[] parameters = [
                new("@TarihSaat", tarihSaat),
                new("@RandevuDurumuId", randevuDurumuId),
                new("@HastaId", hastaId),
                new("@BolumId", bolumId),
                new("@DoktorId", doktorId)
                ];

            if (await DatabaseService.ExecuteNonQueryAsync(query, parameters) > 0)
                MessageBox.Show("Kayit basariyla eklendi");
            else
                MessageBox.Show("Kayit ekleme islemi sirasinda hata meydana geldi", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private async void btnAmeliyatEkle_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtHastaId.Text))
            {
                MessageBox.Show("Lutfen bir Id giriniz", "Hasta Id", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtAmeliyatAd.Text))
            {
                MessageBox.Show("Lutfen ameliyat adini giriniz", "Ameliyat Adi", MessageBoxButton.OK, MessageBoxImage.None);
                return;
            }

            if (cmbAmeliyatBolumler.SelectedIndex < 0)
            {
                MessageBox.Show("Lutfen bir bolum seciniz", "Bolum Secimi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cmbAmeliyatDoktorlar.SelectedIndex < 0)
            {
                MessageBox.Show("Lutfen bir doktor seciniz", "Doktor Secimi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string ad = txtAmeliyatAd.Text;
            int hastaId = Convert.ToInt32(txtHastaId.Text);
            int doktorId = GetDoktorId(cmbAmeliyatDoktorlar.SelectedValue.ToString(), cmbAmeliyatBolumler.SelectedIndex + 1);
            DateTime tarihSaat = GetTarihSaat(dpAmeliyatTarih.SelectedDate, tpAmeliyatSaat.SelectedTime);

            if (hastaId <= 0)
            {
                MessageBox.Show("Gecersiz deger: Hasta Id");
                return;
            }
            if (doktorId <= 0)
            {
                MessageBox.Show("Gecersiz Doktor secimi");
                return;
            }

            string query = $@"
                INSERT INTO Ameliyat
                (Ad, TarihSaat, HastaId, DoktorId)
                VALUES
                (@Ad, @TarihSaat, @HastaId, @DoktorId)
            ";

            SqlParameter[] parameters = [
                new("@Ad", ad),
                new("@TarihSaat", tarihSaat),
                new("@HastaId", hastaId),
                new("@DoktorId", doktorId)
                ];

            if (await DatabaseService.ExecuteNonQueryAsync(query, parameters) > 0)
                MessageBox.Show("Kayit basariyla eklendi");
            else
                MessageBox.Show("Kayit ekleme islemi sirasinda hata meydana geldi", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void cmbAmeliyatBolumler_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbAmeliyatDoktorlar.ItemsSource = FilterDoctorlar(cmbAmeliyatBolumler.SelectedIndex + 1);
        }
    }
}