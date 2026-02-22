using Hastane.Filters;
using Hastane.Services;
using MaterialDesignThemes.Wpf;
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
using System.Xml.Linq;

namespace Hastane.Views
{
    /// <summary>
    /// Interaction logic for DoctorList.xaml
    /// </summary>
    public partial class HastaSorgu : UserControl
    {
        private string editId;
        private string editKimlikNo;
        private string editAd;
        private string editSoyad;
        private int editCinsiyetId;
        private string editTelefon;
        private string editEposta;
        private int editKanGrubuId;

        private string updateQuery = @"
            UPDATE Hasta SET 
            KimlikNo = @KimlikNo,
            Ad = @Ad,
            Soyad = @Soyad,
            CinsiyetId = @CinsiyetId,
            Telefon = @Telefon,
            Eposta = @Eposta,
            KanGrubuId = @KanGrubuId
            WHERE Id = @Id
        ";

        public HastaSorgu()
        {
            InitializeComponent();
            LoadStaticDataAsync();
            SorgulaAsync();
            AddEditColumnToDataGrid();
        }

        private HastaSelect GetFiltre()
        {
            HastaSelect filtre = new HastaSelect();
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
            if (cmbKanGrubu.SelectedIndex > 0)
                filtre.KanGrubuId = cmbKanGrubu.SelectedIndex;
            return filtre;
        }

        private async Task SorgulaAsync()
        {
            HastaSelect filter = GetFiltre();  
            SqlParameter[] parameters;
            string query = filter.GetQuery(out parameters);
            DataTable table = await DatabaseService.ExecuteQueryAsync(query, parameters);
            dgHasta.ItemsSource = table.DefaultView;
        }

        private async Task LoadStaticDataAsync()
        {
            cmbCinsiyet.ItemsSource = await DataService.GetCinsiyetlerAsync();
            cmbKanGrubu.ItemsSource = await DataService.GetKanGruplariAsync();
            cmbEditCinsiyet.ItemsSource = await DataService.GetCinsiyetlerAsync(false);
            cmbEditKanGrubu.ItemsSource = await DataService.GetKanGruplariAsync(false);
        }

        private void ClearForm()
        {
            txtKimlikNo.Text = string.Empty;
            txtAd.Text = string.Empty;
            txtSoyad.Text = string.Empty;
            cmbCinsiyet.SelectedIndex = -1;
            txtTelefon.Text = string.Empty;
            txtEposta.Text = string.Empty;
            cmbKanGrubu.SelectedIndex = -1;
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

        private void dgHasta_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column is DataGridColumn && e.PropertyName == "DogumTarihi")
            {
                e.Column = new System.Windows.Controls.DataGridTextColumn()
                {
                    Header = e.Column.Header,
                    Binding = new Binding(e.PropertyName)
                    {
                        StringFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern
                    }
                };
            }
        }

        private void AddEditColumnToDataGrid()
        {
            DataGridTemplateColumn editColumn = new DataGridTemplateColumn();
            editColumn.Header = "";

            DataTemplate buttonTemplate = new DataTemplate(typeof(Button));
            FrameworkElementFactory buttonFactory = new FrameworkElementFactory(typeof(Button));

            FrameworkElementFactory packIconFactory = new FrameworkElementFactory(typeof(PackIcon));
            packIconFactory.SetValue(PackIcon.KindProperty, PackIconKind.Edit);
            packIconFactory.SetValue(PackIcon.HeightProperty, 16d);
            packIconFactory.SetValue(PackIcon.WidthProperty, 16d);

            buttonFactory.SetValue(Button.StyleProperty, FindResource("MaterialDesignIconButton") as Style);
            buttonFactory.SetValue(Button.WidthProperty, 16d);
            buttonFactory.SetValue(Button.HeightProperty, 16d);

            buttonFactory.AppendChild(packIconFactory);

            buttonFactory.AddHandler(Button.ClickEvent, new RoutedEventHandler(btnEdit_ClickAsync));

            buttonTemplate.VisualTree = buttonFactory;
            editColumn.CellTemplate = buttonTemplate;

            dgHasta.Columns.Add(editColumn);
        }

        private async void btnEdit_ClickAsync(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            DataRowView rowView = clickedButton?.DataContext as DataRowView;

            if (rowView != null)
            {
                editId = rowView["Id"].ToString();
                editKimlikNo = rowView["KimlikNo"].ToString();
                editAd = rowView["Ad"].ToString();
                editSoyad = rowView["Soyad"].ToString();
                var cinsiyetler = await DataService.GetCinsiyetlerAsync(false);
                editCinsiyetId = cinsiyetler.IndexOf(rowView["Cinsiyet"].ToString());
                editTelefon = rowView["Telefon"].ToString();
                editEposta = rowView["Eposta"].ToString();
                var kanGruplari = await DataService.GetKanGruplariAsync(false);
                editKanGrubuId = kanGruplari.IndexOf(rowView["KanGrubu"].ToString());

                FillEditItems();
            }
        }

        private void FillEditItems()
        {
            txtEditKimlikNo.IsEnabled = true;
            txtEditAd.IsEnabled = true;
            txtEditSoyad.IsEnabled = true;
            cmbEditCinsiyet.IsEnabled = true;
            txtEditTelefon.IsEnabled = true;
            txtEditEposta.IsEnabled = true;
            cmbEditKanGrubu.IsEnabled = true;
            btnGuncelle.IsEnabled = true;
            btnIptal.IsEnabled = true;

            txtEditId.Text = editId;
            txtEditKimlikNo.Text = editKimlikNo;
            txtEditAd.Text = editAd;
            txtEditSoyad.Text = editSoyad;
            cmbEditCinsiyet.SelectedIndex = editCinsiyetId;
            txtEditTelefon.Text = editTelefon;
            txtEditEposta.Text = editEposta;
            cmbEditKanGrubu.SelectedIndex = editKanGrubuId;
        }

        private SqlParameter[] GetParameters()
        {
            return
            [
                new("@KimlikNo", txtEditKimlikNo.Text),
                new("@Ad", txtEditAd.Text),
                new("@Soyad", txtEditSoyad.Text),
                new("@CinsiyetId", cmbEditCinsiyet.SelectedIndex +  1),
                new("@Telefon", txtEditTelefon.Text),
                new("@Eposta", txtEditEposta.Text),
                new("@KanGrubuId", cmbEditKanGrubu.SelectedIndex + 1),
                new("@Id", Convert.ToInt32(txtEditId.Text)),
            ];
        }

        private void DisableEditFields()
        {
            txtEditKimlikNo.IsEnabled = false;
            txtEditAd.IsEnabled = false;
            txtEditSoyad.IsEnabled = false;
            cmbEditCinsiyet.IsEnabled = false;
            txtEditTelefon.IsEnabled = false;
            txtEditEposta.IsEnabled = false;
            cmbEditKanGrubu.IsEnabled = false;
            btnGuncelle.IsEnabled = false;
            btnIptal.IsEnabled = false;
        }

        private async void btnGuncelle_Click(object sender, RoutedEventArgs e)
        {
            if (!DataService.IsNationalIdValid(txtEditKimlikNo.Text))
            {
                MessageBox.Show("Gecersiz KimlikNo");
                return;
            }
            if (!DataService.IsPhoneValid(txtEditTelefon.Text))
            {
                MessageBox.Show("Gecersiz Telefon");
                return;
            }
            if (!DataService.IsEmailValid(txtEditEposta.Text))
            {
                MessageBox.Show("Gecersiz Eposta");
                return;
            }

            if (await DatabaseService.ExecuteNonQueryAsync(updateQuery, GetParameters()) > 0)
                DisableEditFields();
            else
                MessageBox.Show("Guncelleme sirasinda hata meydana geldi");

            await SorgulaAsync();
        }

        private void btnIptal_Click(object sender, RoutedEventArgs e)
        {
            FillEditItems();
            DisableEditFields();
        }
    }
}