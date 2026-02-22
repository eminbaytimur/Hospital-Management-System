using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hastane.Filters
{
    internal class HastaSelect : ISelect
    {
        public string? KimlikNo { get; set; } = null;
        public string? Ad { get; set; } = null;
        public string? Soyad { get; set; } = null;
        public int? MinYas { get; set; } = null;
        public int? MaxYas { get; set; } = null;
        public int? CinsiyetId { get; set; } = null;
        public string? Telefon { get; set; } = null;
        public string? Eposta { get; set; } = null;
        public int? KanGrubuId { get; set; } = null;

        public string WhereClauses(out SqlParameter[] parameters)
        {
            string query = "1 = 1";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            if (KimlikNo != null)
            {
                query += " AND H.KimlikNo LIKE @KimlikNo";
                sqlParameters.Add(new SqlParameter("@KimlikNo", $"{KimlikNo}%"));
            }

            if (Ad != null)
            {
                query += " AND H.Ad LIKE @Ad";
                sqlParameters.Add(new SqlParameter("@Ad", $"%{Ad}%"));
            }

            if (Soyad != null)
            {
                query += " AND H.Soyad LIKE @Soyad";
                sqlParameters.Add(new SqlParameter("@Soyad", $"%{Soyad}%"));
            }

            if (MinYas != null)
            {
                query += " AND DATEDIFF(YEAR, H.DogumTarihi, GETDATE()) > @MinYas";
                sqlParameters.Add(new SqlParameter("@MinYas", MinYas));
            }

            if (MaxYas != null)
            {
                query += " AND DATEDIFF(YEAR, H.DogumTarihi, GETDATE()) < @MaxYas";
                sqlParameters.Add(new SqlParameter("@MaxYas", MaxYas));
            }

            if (CinsiyetId != null)
            {
                query += " AND H.CinsiyetId = @CinsiyetId";
                sqlParameters.Add(new SqlParameter("@CinsiyetId", CinsiyetId));
            }

            if (Telefon != null)
            {
                query += " AND H.Telefon LIKE @Telefon";
                sqlParameters.Add(new SqlParameter("@Telefon", $"%{Telefon}%"));
            }

            if (Eposta != null)
            {
                query += " AND H.Eposta LIKE @Eposta";
                sqlParameters.Add(new SqlParameter("@Eposta", $"%{Eposta}%"));
            }

            if (KanGrubuId != null)
            {
                query += " AND H.KanGrubuId = @KanGrubuId";
                sqlParameters.Add(new SqlParameter("@KanGrubuId", KanGrubuId));
            }

            parameters = sqlParameters.ToArray();
            return query;
        }

        public string GetQuery(out SqlParameter[] parameters)
        {
            string query = @$"
                SELECT
                    H.Id,
                    H.KimlikNo, 
                    H.Ad, 
                    H.Soyad, 
                    H.DogumTarihi, 
                    C.Ad AS Cinsiyet, 
                    H.Telefon, 
                    H.Eposta, 
                    K.Ad AS KanGrubu,
                    COALESCE(R.RandevuSayisi, 0) AS RandevuSayisi,
                    COALESCE(T.TestSayisi, 0) AS TestSayisi,
                    COALESCE(A.AmeliyatSayisi, 0) AS AmeliyatSayisi
                FROM 
                    Hasta AS H
                INNER JOIN 
                    Cinsiyet AS C ON C.Id = H.CinsiyetId
                INNER JOIN 
                    KanGrubu AS K ON K.Id = H.KanGrubuId
                LEFT JOIN (
                    SELECT 
                        HastaId, 
                        COUNT(*) AS RandevuSayisi
                    FROM 
                        Randevu
                    GROUP BY 
                        HastaId
                ) R ON R.HastaId = H.Id
                LEFT JOIN (
                    SELECT 
                        HastaId, 
                        COUNT(*) AS TestSayisi
                    FROM 
                        TestSonucu
                    GROUP BY 
                        HastaId
                ) T ON T.HastaId = H.Id
                LEFT JOIN (
                    SELECT 
                        HastaId, 
                        COUNT(*) AS AmeliyatSayisi
                    FROM 
                        Ameliyat
                    GROUP BY 
                        HastaId
                ) A ON A.HastaId = H.Id
                WHERE {WhereClauses(out parameters)}
            ";
            return query;
        }
    }
}