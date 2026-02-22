using Hastane.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Hastane.Filters
{
    internal class DoktorSelect : ISelect
    {
        public string? KimlikNo { get; set; } = null;
        public string? Ad { get; set; } = null;
        public string? Soyad { get; set; } = null;
        public int? MinYas { get; set; } = null;
        public int? MaxYas { get; set; } = null;
        public int? CinsiyetId { get; set; } = null;
        public string? Telefon { get; set; } = null;
        public string? Eposta { get; set; } = null;
        public int? BolumId { get; set; } = null;

        public string WhereClauses(out SqlParameter[] parameters)
        {
            string query = "1 = 1";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            if (KimlikNo != null)
            {
                query += " AND D.KimlikNo LIKE @KimlikNo";
                sqlParameters.Add(new SqlParameter("@KimlikNo", $"{KimlikNo}%"));
            }

            if (Ad != null)
            {
                query += " AND D.Ad LIKE @Ad";
                sqlParameters.Add(new SqlParameter("@Ad", $"%{Ad}%"));
            }

            if (Soyad != null)
            {
                query += " AND D.Soyad LIKE @Soyad";
                sqlParameters.Add(new SqlParameter("@Soyad", $"%{Soyad}%"));
            }

            if (MinYas != null)
            {
                query += " AND DATEDIFF(YEAR, D.DogumTarihi, GETDATE()) > @MinYas";
                sqlParameters.Add(new SqlParameter("@MinYas", MinYas));
            }

            if (MaxYas != null)
            {
                query += " AND DATEDIFF(YEAR, D.DogumTarihi, GETDATE()) < @MaxYas";
                sqlParameters.Add(new SqlParameter("@MaxYas", MaxYas));
            }

            if (CinsiyetId != null)
            {
                query += " AND D.CinsiyetId = @CinsiyetId";
                sqlParameters.Add(new SqlParameter("@CinsiyetId", CinsiyetId));
            }

            if (Telefon != null)
            {
                query += " AND D.Telefon LIKE @Telefon";
                sqlParameters.Add(new SqlParameter("@Telefon", $"%{Telefon}%"));
            }

            if (Eposta != null)
            {
                query += " AND D.Eposta LIKE @Eposta";
                sqlParameters.Add(new SqlParameter("@Eposta", $"%{Eposta}%"));
            }

            if (BolumId != null)
            {
                query += " AND D.BolumId = @BolumId";
                sqlParameters.Add(new SqlParameter("@BolumId", BolumId));
            }

            parameters = sqlParameters.ToArray();
            return query;
        }

        public string GetQuery(out SqlParameter[] parameters)
        {
            string query = @$"
                SELECT
                    D.Id,
                    D.KimlikNo, 
                    D.Ad, 
                    D.Soyad, 
                    D.DogumTarihi, 
                    C.Ad AS Cinsiyet, 
                    D.Telefon, 
                    D.Eposta, 
                    B.Ad AS Bolum,
                    F.URI as PhotoPath
                FROM 
                    Doktor AS D
                INNER JOIN 
                    Cinsiyet AS C ON C.Id = D.CinsiyetId
                INNER JOIN 
                    Bolum AS B ON B.Id = D.BolumId
                LEFT JOIN
                    Fotograf F ON F.Id = D.FotografId
                WHERE {WhereClauses(out parameters)}
            ";
            return query;
        }
    }
}