using Hastane.Filters;
using Hastane.Services;
using Microsoft.Data.SqlClient;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
    /// Interaction logic for DoktorEkle.xaml
    /// </summary>
    public partial class DoktorEkle : UserControl
    {
        private int imageId = -1;
        private string selectedImagePath;
        private string imagePathInDb;
        private string projectDirectory;
        private string imagesDirectory;

        public DoktorEkle()
        {
            InitializeComponent();
            projectDirectory = AppDomain.CurrentDomain.BaseDirectory;
            imagesDirectory = System.IO.Path.Combine(projectDirectory, "Resources", "Images");
            LoadStaticDataAsync();
        }

        private DoktorInsert GetInsert()
        {
            return new DoktorInsert()
            {
                KimlikNo = txtKimlikNo.Text,
                Ad = txtAd.Text,
                Soyad = txtSoyad.Text,
                DogumTarihi = DateOnly.FromDateTime(dpDogumTarihi.SelectedDate.Value),
                CinsiyetId = cmbCinsiyet.SelectedIndex + 1, // Database Id starts from 1, 
                Telefon = txtTelefon.Text,
                Eposta = txtEposta.Text,
                BolumId = cmbBolum.SelectedIndex + 1,
                FotografId = imageId
            };
        }

        private async Task InsertAsync()
        {
            DoktorInsert insert = GetInsert();
            SqlParameter[] parameters;
            string query = insert.GetQuery(out parameters);
            await DatabaseService.ExecuteNonQueryAsync(query, parameters);
        }

        private async Task LoadStaticDataAsync()
        {
            cmbCinsiyet.ItemsSource = await DataService.GetCinsiyetlerAsync(false);
            cmbBolum.ItemsSource = await DataService.GetBolumlerAsync(false);
            LoadWatermark();
        }

        private void LoadWatermark()
        {
            string imageName = "watermark.jpg";
            string imagePath = System.IO.Path.Combine(imagesDirectory, imageName);
            imgDoktor.ImageSource = new BitmapImage(new Uri(imagePath));
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
            cmbBolum.SelectedIndex = -1;
            LoadWatermark();
            selectedImagePath = string.Empty;
            imagePathInDb = string.Empty;
            imageId = -1;
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
            if (cmbBolum.SelectedIndex < 0)
            {
                MessageBox.Show("Gecersiz Bolum");
                return false;
            }
            if (imgDoktor == null || imgDoktor.ImageSource == null)
            {
                MessageBox.Show("Gecersiz Fotograf");
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
                if (!string.IsNullOrEmpty(selectedImagePath) && string.IsNullOrEmpty(imagePathInDb))
                {
                    await SaveImage();
                    imageId = await GetFotografId(imagePathInDb);
                }
                else
                    imageId = cmbCinsiyet.SelectedIndex == 0 ? 1 : 2;

                await InsertAsync();
                ClearForm();
            }
        }

        private async Task<int> GetFotografId(string path)
        {
            string query = "SELECT Id FROM Fotograf WHERE URI = @URI";
            SqlParameter[] parameters = [new("@URI", path)];
            DataTable table = await DatabaseService.ExecuteQueryAsync(query, parameters);
            if (table == null || table.Rows.Count == 0)
                return -1;

            return Convert.ToInt32(table.Rows[0]["Id"]);
        }

        private void btnSelectImg_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif";

            if (openFileDialog.ShowDialog() == true)
            {
                selectedImagePath = openFileDialog.FileName;
                imgDoktor.ImageSource = new BitmapImage(new Uri(selectedImagePath));
            }
        }

        private async Task<int> SaveImage()
        {
            if (!Directory.Exists(imagesDirectory))
            {
                Directory.CreateDirectory(imagesDirectory);
            }

            string newFileName = "img_" + Guid.NewGuid().ToString() + System.IO.Path.GetExtension(selectedImagePath);
            string newFilePath = System.IO.Path.Combine(imagesDirectory, newFileName);

            File.Copy(selectedImagePath, newFilePath);

            imagePathInDb = System.IO.Path.Combine("Resources", "Images", newFileName);

            return await SaveImagePathToDatabase(imagePathInDb);
        }

        private async Task<int> SaveImagePathToDatabase(string uri)
        {
            string query = "INSERT INTO Fotograf (URI) VALUES (@URI)";
            SqlParameter[] parameters = [new("@URI", uri)];
            return Convert.ToInt32(await DatabaseService.ExecuteScalarAsync(query, parameters));
        }

        private void cmbCinsiyet_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(selectedImagePath))
                return;

            string imageName = cmbCinsiyet.SelectedIndex == 0 ?
                "img_f489daf2-9f68-45a4-add0-a86b42ca1087.jpg"
                : "img_845e2a45-5382-454c-bb51-7809cc735502.jpg";

            string imagePath = System.IO.Path.Combine(imagesDirectory, imageName);
            imgDoktor.ImageSource = new BitmapImage(new Uri(imagePath));
        }
    }
}