using Hastane.Services;
using MaterialDesignThemes.Wpf;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for HastaDetay.xaml
    /// </summary>
    public partial class HastaDetay : Page
    {
        private string randevuQuery = @"
                   SELECT
                    R.Id,
                    R.TarihSaat,
                    RD.Ad AS RandevuDurumu,
                    B.Ad AS Bolum,
                    CONCAT(D.Ad, ' ', D.Soyad) AS Doktor
                FROM
                    Randevu AS R
                INNER JOIN 
                    RandevuDurumu AS RD ON R.RandevuDurumuId = RD.Id
                INNER JOIN
                    Bolum AS B ON R.BolumId = B.Id
                INNER JOIN 
                    Doktor AS D ON R.DoktorId = D.Id
                WHERE R.HastaId = @HastaId
            ";

        private string receteQuery = @"
                   SELECT
                    R.Id,
                    R.OlusturmaTarihi,
                    CONCAT(D.Ad, ' ', D.Soyad) AS Doktor
                FROM
                    Recete AS R
                INNER JOIN 
                    Doktor AS D ON R.DoktorId = D.Id
                WHERE R.HastaId = @HastaId
            ";

        private string ameliyatQuery = @"
                   SELECT
                    A.Id,
                    A.Ad,
                    A.Aciklama,
                    A.TarihSaat,
                    CONCAT(D.Ad, ' ', D.Soyad) AS Doktor
                FROM
                    Ameliyat AS A
                INNER JOIN 
                    Doktor AS D ON A.DoktorId = D.Id
                WHERE A.HastaId = @HastaId
            ";

        private string testQuery = @"
                   SELECT
                    T.Id,
                    T.Ad,
                    TS.Sonuc,
                    TS.TarihSaat
                FROM
                    TestSonucu AS TS
                INNER JOIN 
                    Test AS T ON TS.TestId = T.Id
                WHERE TS.HastaId = @HastaId
            ";

        public HastaDetay()
        {
            InitializeComponent();
        }

        private async Task GetData(int? hastaId = null)
        {
            int id = hastaId ?? Convert.ToInt32(txtId.Text);
            var randevuTask = DatabaseService.ExecuteQueryAsync(randevuQuery, [new SqlParameter("@HastaId", id)]);
            var receteTask = DatabaseService.ExecuteQueryAsync(receteQuery, [new SqlParameter("@HastaId", id)]);
            var ameliyatTask = DatabaseService.ExecuteQueryAsync(ameliyatQuery, [new SqlParameter("@HastaId", id)]);
            var testTask = DatabaseService.ExecuteQueryAsync(testQuery, [new SqlParameter("@HastaId", id)]);

            await Task.WhenAll(randevuTask, receteTask, ameliyatTask, testTask);

            var randevuTable = await randevuTask;
            var receteTable = await receteTask;
            var ameliyatTable = await ameliyatTask;
            var testTable = await testTask;

            CheckDataGrid(randevuTable, dgRandevular, exRandevu, btnDeleteRandevu_ClickAsync);
            CheckDataGrid(receteTable, dgReceteler, exRecete, btnDeleteRecete_ClickAsync);
            CheckDataGrid(ameliyatTable, dgAmeliyatlar, exAmeliyat, btnDeleteAmeliyat_ClickAsync);
            CheckDataGrid(testTable, dgTestler, exTest, btnDeleteTestSonucu_ClickAsync);
        }

        private void CheckDataGrid(DataTable table, DataGrid dg, Expander ex, RoutedEventHandler handler)
        {
            if (table != null && table.Rows.Count > 0)
            {
                dg.Columns.Clear();
                dg.ItemsSource = table.DefaultView;
                ex.IsExpanded = true;
                AddDeleteColumnToDG(dg, handler);
            }
            else
            {
                dg.ItemsSource = null;
                dg.Columns.Clear();
                ex.IsExpanded = false;
            }
        }

        private async void btnSorgula_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtId.Text))
            {
                MessageBox.Show("Lutfen bir Id girin.", "Hata", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            await GetData();
        }

        private void AddDeleteColumnToDG(DataGrid dataGrid, RoutedEventHandler action)
        {
            DataGridTemplateColumn deleteColumn = new DataGridTemplateColumn();
            deleteColumn.Header = "";

            DataTemplate buttonTemplate = new DataTemplate(typeof(Button));
            FrameworkElementFactory buttonFactory = new FrameworkElementFactory(typeof(Button));

            FrameworkElementFactory packIconFactory = new FrameworkElementFactory(typeof(PackIcon));
            packIconFactory.SetValue(PackIcon.KindProperty, PackIconKind.Delete);
            packIconFactory.SetValue(PackIcon.WidthProperty, 16d);
            packIconFactory.SetValue(PackIcon.HeightProperty, 16d);

            buttonFactory.SetValue(Button.StyleProperty, FindResource("MaterialDesignIconButton") as Style);
            buttonFactory.SetValue(Button.WidthProperty, 16d);
            buttonFactory.SetValue(Button.HeightProperty, 16d);

            buttonFactory.AppendChild(packIconFactory);

            buttonFactory.AddHandler(Button.ClickEvent, new RoutedEventHandler(action));

            buttonTemplate.VisualTree = buttonFactory;
            deleteColumn.CellTemplate = buttonTemplate;

            dataGrid.Columns.Add(deleteColumn);
        }

        private async void btnDeleteRandevu_ClickAsync(object sender, RoutedEventArgs e)
        {
            await ExecuteDelete(sender, e, "Randevu");
        }

        private async void btnDeleteRecete_ClickAsync(object sender, RoutedEventArgs e)
        {
            await ExecuteDelete(sender, e, "Recete");
        }

        private async void btnDeleteAmeliyat_ClickAsync(object sender, RoutedEventArgs e)
        {
            await ExecuteDelete(sender, e, "Ameliyat");
        }

        private async void btnDeleteTestSonucu_ClickAsync(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            DataRowView rowView = clickedButton?.DataContext as DataRowView;

            if (rowView != null && rowView["Id"] != null)
            {
                string query = @"
                    DELETE FROM TestSonucu WHERE
                        Sonuc = @Sonuc AND
                        TestId = @TestId AND
                        HastaId = @HastaId AND
                        TarihSaat = @TarihSaat
                ";

                var testler = await DataService.GetTestlerAsync(false);


                SqlParameter[] parameters = [
                    new("@Sonuc", rowView["Sonuc"].ToString()),
                    new("@TestId", Convert.ToInt32(rowView["Id"])),
                    new("@HastaId", Convert.ToInt32(txtId.Text)),
                    new("@TarihSaat", Convert.ToDateTime(rowView["TarihSaat"]))
                    ];

                if (MessageBox.Show("Silmek istediginize emin misiniz?", "Dogrulama", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    await DatabaseService.ExecuteNonQueryAsync(query, parameters);
                    await GetData();
                }
            }
        }

        private async Task ExecuteDelete(object sender, RoutedEventArgs e, string tableName)
        {
            Button clickedButton = sender as Button;
            DataRowView rowView = clickedButton?.DataContext as DataRowView;

            if (rowView != null && rowView["Id"] != null)
            {
                if (MessageBox.Show("Silmek istediginize emin misiniz?", "Dogrulama", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    await DatabaseService.ExecuteNonQueryAsync(
                        $"DELETE FROM {tableName} Where Id = @Id",
                        [new("@Id", Convert.ToInt32(rowView["Id"]))]);
                    await GetData();
                }
            }
        }
    }
}