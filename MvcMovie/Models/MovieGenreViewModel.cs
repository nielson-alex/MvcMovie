using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace MvcMovie.Models
{
    public class MovieGenreViewModel
    {
        public List<Movie> Movies;
        public SelectList Genres;
        public string MovieGenre { get; set; }
        public SelectList ReleaseDates;
        public DateTime MovieReleaseDate { get; set; }
        public SelectList Ratings;
        public string MovieRating { get; set; }
        public string SearchString { get; set; }
    }
}