using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawlerService.Database
{
    public partial class MoviesContext
    {
        public MoviesContext()
            : base(new SqlCeConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MoviesContext"].ConnectionString))
        {

        }
    }
}
