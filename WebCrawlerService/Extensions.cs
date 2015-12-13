using Contracts.DTA;
using MovieCrawler.Domain.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawlerService
{
    public static class Extensions
    {
        public static Movie ToDTA(this Database.Movies movie)
        {
            return new Movie
            {
                Title = movie.Title,
                CoverImage = movie.Cover,
                Description = movie.Description,
                Genres = ((MovieCrawler.Domain.Model.MovieGenre)movie.Genres).ToDTA(),
                Streams = movie.MovieStreamSet.Select(li => li.ToDTA()).ToList()
            };
        }

        public static MovieStreamSet ToDTA(this Database.MovieStreamSet movieSet)
        {
            return new MovieStreamSet
            {
                Id = movieSet.Id,
                Captions = movieSet.Captions.Select(li => li.ToDTA()).ToList(),
                VideoStreams = movieSet.VideoStream.Select(li => li.ToDTA()).ToList()
            };
        }

        public static Contracts.DTA.MovieStream ToDTA(this Database.VideoStream stream)
        {
            return new Contracts.DTA.MovieStream
            {
                Id = stream.Id,
                Video = stream.VideoAddress,
                Resolution = stream.Resolution
            };
        }

        public static Contracts.DTA.MovieCaption ToDTA(this Database.Captions caption)
        {
            return new Contracts.DTA.MovieCaption
            {
                Language = caption.Language,
                Address = caption.Address
            };
        }

        public static Database.StreamCookie ToDBO(this Cookie cookie, Database.MovieStreamSet streamSet)
        {
            return new Database.StreamCookie
            {
                MovieStreamSet = streamSet,
                Value = SerializationUtils.SerializeCookie(cookie),
                ExpirationDate = cookie.Expires
            };
        }

        public static Database.StreamHeaders ToDBO(this KeyValuePair<string, string> kvp, Database.MovieStreamSet streamSet)
        {
            return new Database.StreamHeaders
            {
                MovieStreamSet = streamSet,
                Name = kvp.Key,
                Value = kvp.Value,
            };
        }

        public static IEnumerable<Contracts.DTA.MovieGenre> ToDTA(this MovieCrawler.Domain.Model.MovieGenre genreEnum)
        {
            foreach(MovieCrawler.Domain.Model.MovieGenre genre in Enum.GetValues(typeof(MovieCrawler.Domain.Model.MovieGenre)))
            {
                if (genre > 0 && genreEnum.HasFlag(genre))
                    yield return new Contracts.DTA.MovieGenre
                    {
                        Id = (int) Math.Floor(Math.Log((int)genre, 2)),
                        Name = genre.ToString()
                    };
            }
        }


        public static void SaveMovieInformation(this Database.MoviesContext context, Database.Movies dbMovie, MovieInfo movieInfo)
        {
            context.Movies.InsertOnSubmit(dbMovie);

            foreach (var streamSet in movieInfo.Streams)
            {
                var dbStreamSet = new Database.MovieStreamSet
                {
                    Movies = dbMovie,
                    AddressSource = movieInfo.Link.ToString(),
                };

                context.Captions.InsertAllOnSubmit(streamSet.Captions.Select(li => new Database.Captions
                {
                    MovieStreamSet = dbStreamSet,
                    Language = li.Language,
                    Address = li.Address
                }));

                context.VideoStream.InsertAllOnSubmit(streamSet.VideoStreams.Select(li => new Database.VideoStream
                {
                    MovieStreamSet = dbStreamSet,
                    Resolution = (byte)li.Resolution,
                    VideoAddress = li.AVStream,

                }));

                context.StreamCookie.InsertAllOnSubmit(streamSet.Cookies.Cast<System.Net.Cookie>().Select(li => new Database.StreamCookie
                {
                    MovieStreamSet = dbStreamSet,
                    Value = SerializationUtils.SerializeCookie(li),
                    ExpirationDate = li.Expires
                }));

                context.StreamHeaders.InsertAllOnSubmit(streamSet.Headers.Select(li => new Database.StreamHeaders
                {
                    MovieStreamSet = dbStreamSet,
                    Name = li.Key,
                    Value = li.Value,
                }));
            }
        }

        public static DbQuery<T> Include<T>(this DbQuery<T> set, Expression<Func<T, object>> expression)
        {
            var lambda = expression.Body as LambdaExpression;
            if (lambda == null)
                throw new InvalidOperationException("LoadWith<T> must be called only with lambda expressions");
            return set.Include(lambda.Name);
        }
    }
}
