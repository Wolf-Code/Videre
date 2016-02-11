using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VidereLib.Data
{
    public class MovieInformation
    {
        public string Name { set; get; }

        public ushort Year { set; get; }

        public string IMDBID { set; get; }

        public string Poster { set; get; }

        public string Hash { set; get; }

        public decimal Rating { set; get; }
    }
}
