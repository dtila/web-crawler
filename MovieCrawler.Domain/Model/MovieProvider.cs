using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieCrawler.Domain.Model
{
    public class MovieProvider
    {
        public string Name { get; private set; }
        public byte LastPageProcessed { get; private set; }

        public MovieProvider(string name)
        {
            this.Name = name;
        }
    }
}
