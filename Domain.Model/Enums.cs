using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieCrawler.ApplicationServices.Domain.Model
{
    public enum VideoResolution : byte
    {
        Unknown,
        _240,
        _360,
        _480,
        _720,
        _1080
    }

    public enum VideoType : byte
    {
        Unknown,
        FLV,
        MP4
    }

    [Flags]
    public enum MovieGenre : int
    {
        None = 0,
        Action = 1 << 1,
        Animated = 1 << 2,
        Adventure = 1 << 3,
        Biography = 1 << 4,
        Commedy = 1 << 5,
        Crime = 1 << 6,
        Documentary = 1 << 7,
        Love = 1 << 8,
        Thriller = 1 << 9,
        Family = 1 << 10,
        Fantesy = 1 << 11,
        // Film fara categorie ???
        Horror = 1 << 12,
        History = 1 << 13,
        Mystery = 1 << 14,
        Musical = 1 << 15,
        War = 1 << 16,
        Romantic = 1 << 17,
        Sci_Fi = 1 << 18,
        Sport = 1 << 19,
        Western = 1 << 20
    }
}
