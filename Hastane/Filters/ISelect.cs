using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Hastane.Filters
{
    internal interface ISelect
    {
        string WhereClauses(out SqlParameter[] parameters);
        string GetQuery(out SqlParameter[] parameters);
    }
}
