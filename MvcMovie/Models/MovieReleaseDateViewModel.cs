using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace MvcMovie.Models
{
    public class MovieReleaseDateViewModel
    {
        public List<Movie> Movies;
        public SelectList ReleaseDates;
        public string MovieReleaseDate { get; set; }
        public string SearchString { get; set; }
    }
}