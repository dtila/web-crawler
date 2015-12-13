using MovieCrawler.Domain.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace MovieCrawler.Domain
{
    /// <summary>
    /// The factory class used to create movie or stream crawlers
    /// </summary>
   
    public static class SharedRegex
    {
        public static readonly Regex MovieYearRegex = new Regex(@"(.+)\((\d+)\)", RegexOptions.Compiled);
        public static readonly Regex EnglishPageMatchRegex = new Regex(@"Page (\d+) of (\d+)", RegexOptions.Compiled);
    }

    public static class MovieHelpers
    {
        public static bool Extract(string rawTitle, out string title, out int? year)
        {
            title = null;
            year = null;
            if (string.IsNullOrEmpty(rawTitle))
                return false;

            var match = SharedRegex.MovieYearRegex.Match(rawTitle);
            if (!match.Success)
                return false;

            title = match.Groups[1].Value;
            year = int.Parse(match.Groups[2].Value);
            return true;
        }

        public static MovieGenre RetrieveMovieGenres(IEnumerable<string> genres)
        {
            var movieGenre = MovieGenre.None;
            foreach (var category in genres)
                movieGenre |= GetMovieGenreFromDescription(category);
            return movieGenre;
        }

        private static MovieGenre GetMovieGenreFromDescription(string category)
        {
            MovieGenre genre;
            if (TryGetMovieGenreFromEnDescription(category, out genre))
                return genre;

            if (TryGetMovieGenreFromRoDescription(category, out genre))
                return genre;
            throw new NotImplementedException(string.Format("Unable to recognize the '{0}' category name", category));
        }

        private static bool TryGetMovieGenreFromRoDescription(string category, out MovieGenre genre)
        {
            genre = MovieGenre.None;

            if (category.EqualsIgnoreCase("actiune")) genre = MovieGenre.Action;
            else if (category.EqualsIgnoreCase("animatie")) genre = MovieGenre.Animated;
            else if (category.EqualsIgnoreCase("aventura")) genre = MovieGenre.Adventure;
            else if (category.EqualsIgnoreCase("biografie")) genre = MovieGenre.Biography;
            else if (category.EqualsIgnoreCase("comedie")) genre = MovieGenre.Commedy;
            else if (category.EqualsIgnoreCase("crima")) genre = MovieGenre.Crime;
            else if (category.EqualsIgnoreCase("documentar")) genre = MovieGenre.Documentary;
            else if (category.EqualsIgnoreCase("dragoste")) genre = MovieGenre.Love;
            else if (category.EqualsIgnoreCase("drama")) genre = MovieGenre.Thriller;
            else if (category.EqualsIgnoreCase("familie")) genre = MovieGenre.Family;
            else if (category.EqualsIgnoreCase("fantastic")) genre = MovieGenre.Fantesy;
            else if (category.EqualsIgnoreCase("fantezie")) genre = MovieGenre.Fantesy;
            else if (category.EqualsIgnoreCase("groaza")) genre = MovieGenre.Horror;
            else if (category.EqualsIgnoreCase("istoric")) genre = MovieGenre.History;
            else if (category.EqualsIgnoreCase("mister")) genre = MovieGenre.Mistery;
            else if (category.EqualsIgnoreCase("muzical")) genre = MovieGenre.Musical;
            else if (category.EqualsIgnoreCase("razboi")) genre = MovieGenre.War;
            else if (category.EqualsIgnoreCase("romantic")) genre = MovieGenre.Romantic;
            else if (category.EqualsIgnoreCase("sci-fi")) genre = MovieGenre.Sci_Fi;
            else if (category.EqualsIgnoreCase("sport")) genre = MovieGenre.Sport;
            else if (category.EqualsIgnoreCase("western")) genre = MovieGenre.Western;

            if (genre != MovieGenre.None)
                return true;

            if (category.EqualsIgnoreCase("filme fara categorie"))
                return true;
            return false;
        }

        private static bool TryGetMovieGenreFromEnDescription(string category, out MovieGenre genre)
        {
            genre = MovieGenre.None;

            if (category.EqualsIgnoreCase("sci-fi")) genre = MovieGenre.Western;

            if (genre != MovieGenre.None)
                return true;

            return Enum.TryParse(category, true, out genre);
        }
    }

    public static class StringHelpers
    {
        public static bool EqualsIgnoreCase(this string @string, string value)
        {
            return String.Compare(@string, value, CultureInfo.InvariantCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) == 0;
        }
    }
}
