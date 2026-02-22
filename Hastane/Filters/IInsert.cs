using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hastane.Filters
{
    internal interface IInsert
    {
        string IntoString();
        string ValuesString(out SqlParameter[] parameters);
        string GetQuery(out SqlParameter[] parameters);
    }
}
