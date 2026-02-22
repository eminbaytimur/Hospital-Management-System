using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Hastane.Services
{
    internal static class DataService
    {
        private static async Task<List<string>?> GetAdSutunuAsync(string tableName)
        {
            var query = $"SELECT Ad FROM {tableName} ORDER BY Id ASC";

            DataTable table = await DatabaseService.ExecuteQueryAsync(query);
            if (table == null || table.Rows.Count == 0)
                return null;

            var list = new List<string>();
            foreach (DataRow row in table.Rows)
                list.Add(row["Ad"].ToString());

            return list;
        }

        public static async Task<List<string>?> GetKanGruplariAsync(bool extended = true) => extended ? ["Tumu", .. await GetAdSutunuAsync("KanGrubu")] : await GetAdSutunuAsync("KanGrubu");
        public static async Task<List<string>?> GetCinsiyetlerAsync(bool extended = true) => extended ? ["Tumu", ..await GetAdSutunuAsync("Cinsiyet")] : await GetAdSutunuAsync("Cinsiyet");
        public static async Task<List<string>?> GetBolumlerAsync(bool extended = true) => extended ? ["Tumu", ..await GetAdSutunuAsync("Bolum")] : await GetAdSutunuAsync("Bolum");
        public static async Task<List<string>?> GetRandevuDurumlariAsync(bool extended = true) => extended ? ["Tumu", ..await GetAdSutunuAsync("RandevuDurumu")] : await GetAdSutunuAsync("RandevuDurumu");
        public static async Task<List<string>?> GetTestlerAsync(bool extended = true) => extended ? ["Tumu", ..await GetAdSutunuAsync("Test")] : await GetAdSutunuAsync("Test");
        public static async Task<List<string>?> GetIlaclarAsync(bool extended = true) => extended ? ["Tumu", ..await GetAdSutunuAsync("Ilac")] : await GetAdSutunuAsync("Ilac");

        public static bool IsNationalIdValid(string nationalId)
        {
            return Regex.IsMatch(nationalId, "^[0-9]+$") && nationalId.Length == 11;
        }

        public static bool IsPhoneValid(string phone)
        {
            if (phone.Length != 12) 
                return false;

            string pattern = @"^\d{3}-\d{3}-\d{4}$";
            return Regex.IsMatch(phone, pattern, RegexOptions.IgnoreCase);
        }

        public static bool IsEmailValid(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }
    }
}
