using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hastane.Filters
{
    internal class DoktorInsert : IInsert
    {
        public string KimlikNo { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public DateOnly DogumTarihi { get; set; }
        public int CinsiyetId { get; set; }
        public string Telefon { get; set; }
        public string Eposta { get; set; }
        public int BolumId { get; set; }
        public int FotografId { get; set; }

        public string IntoString()
        {
            string query = string.Empty;
            query += "KimlikNo";
            query += ", Ad";
            query += ", Soyad";
            query += ", DogumTarihi";
            query += ", CinsiyetId";
            query += ", Telefon";
            query += ", Eposta";
            query += ", BolumId";
            query += ", FotografId";

            return query;
        }

        public string ValuesString(out SqlParameter[] parameters)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            string query = string.Empty;

            query += "@KimlikNo";
            sqlParameters.Add(new("@KimlikNo", KimlikNo));

            query += ", @Ad";
            sqlParameters.Add(new("@Ad", Ad));

            query += ", @Soyad";
            sqlParameters.Add(new("@Soyad", Soyad));

            query += ", @DogumTarihi";
            sqlParameters.Add(new("@DogumTarihi", DogumTarihi));

            query += ", @CinsiyetId";
            sqlParameters.Add(new("@CinsiyetId", CinsiyetId));

            query += ", @Telefon";
            sqlParameters.Add(new("@Telefon", Telefon));

            query += ", @Eposta";
            sqlParameters.Add(new("@Eposta", Eposta));

            query += ", @BolumId";
            sqlParameters.Add(new("@BolumId", BolumId));

            query += ", @FotografId";
            sqlParameters.Add(new("@FotografId", FotografId));

            parameters = sqlParameters.ToArray();
            return query;
        }

        public string GetQuery(out SqlParameter[] parameters)
        {
            return $"INSERT INTO Doktor ({IntoString()}) VALUES ({ValuesString(out parameters)})";
        }
    }
}
