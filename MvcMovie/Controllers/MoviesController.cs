using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcMovie.Models;

namespace MvcMovie.Controllers
{
    public class MoviesController : Controller
    {
        private readonly MvcMovieContext _context;

        public MoviesController(MvcMovieContext context)
        {
            _context = context;
        }

        // GET: Movies
        [HttpPost]
        public string Index(string searchString, bool notUsed)
        {
            return "From [HttpPost]Index: filter on " + searchString;
        }

        public async Task<IActionResult> Index(string movieGenre, DateTime movieReleaseDate, string movieRating, string searchString)
        {
            //IQueryable<string> genreQuery;
            //// Use LINQ to get list of genres.
            //if (!String.IsNullOrEmpty(ascending))
            //{
            //    genreQuery = from m in _context.Movie
            //                 orderby m.Genre ascending
            //                 select m.Genre;
            //}
            //else if (!String.IsNullOrEmpty(descending))
            //{
            //    genreQuery = from m in _context.Movie
            //                 orderby m.Genre descending
            //                 select m.Genre;
            //}

            // Use LINQ to get list of genres.
            IQueryable<string> genreQuery = from m in _context.Movie
                                            orderby m.Genre
                                            select m.Genre;

            IQueryable<string> releaseDateQuery = from m in _context.Movie
                                                  orderby m.ReleaseDate.ToString("yyyy-MM-dd")
                                                  select m.ReleaseDate.ToString("yyyy-MM-dd");

            IQueryable<string> ratingQuery = from m in _context.Movie
                                                  orderby m.Rating
                                                  select m.Rating;

            var movies = from m in _context.Movie
                         select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                movies = movies.Where(s => s.Title.Contains(searchString));
            }

            // If the genre is not empty, release date not empty, and rating not empty
            if (!(String.IsNullOrEmpty(movieGenre)) && movieReleaseDate != DateTime.MinValue && !(String.IsNullOrEmpty(movieRating)))
            {
                movies = movies.Where(x => x.Genre == movieGenre &&
                                           Convert.ToDateTime(x.ReleaseDate.ToString("yyyy-MM-dd")) == Convert.ToDateTime(movieReleaseDate.ToString("yyyy-MM-dd")) &&
                                           x.Rating == movieRating);
            }
            // If the genre is not empty, release date not empty, but rating is empty
            else if (!(String.IsNullOrEmpty(movieGenre)) && movieReleaseDate != DateTime.MinValue && String.IsNullOrEmpty(movieRating))
            {
                movies = movies.Where(x => x.Genre == movieGenre &&
                                           Convert.ToDateTime(x.ReleaseDate.ToString("yyyy-MM-dd")) == Convert.ToDateTime(movieReleaseDate.ToString("yyyy-MM-dd")));
            }
            // If the genre is not empty, release date is empty, and rating is empty
            else if (!(String.IsNullOrEmpty(movieGenre)) && movieReleaseDate == DateTime.MinValue && String.IsNullOrEmpty(movieRating))
            {
                movies = movies.Where(x => x.Genre == movieGenre);
            }
            // If the genre is empty, release date is not empty, and rating is not empty
            else if (String.IsNullOrEmpty(movieGenre) && movieReleaseDate != DateTime.MinValue && !(String.IsNullOrEmpty(movieRating)))
            {
                movies = movies.Where(x => Convert.ToDateTime(x.ReleaseDate.ToString("yyyy-MM-dd")) == Convert.ToDateTime(movieReleaseDate.ToString("yyyy-MM-dd")) &&
                                           x.Rating == movieRating);
            }
            // If the genre is empty, release date is not empty, and rating is empty
            else if (String.IsNullOrEmpty(movieGenre) && movieReleaseDate != DateTime.MinValue && String.IsNullOrEmpty(movieRating))
            {
                movies = movies.Where(x => Convert.ToDateTime(x.ReleaseDate.ToString("yyyy-MM-dd")) == Convert.ToDateTime(movieReleaseDate.ToString("yyyy-MM-dd")));
            }
            // If the genre is not empty, release date is empty, and rating is not empty
            else if (!(String.IsNullOrEmpty(movieGenre)) && movieReleaseDate == DateTime.MinValue && !(String.IsNullOrEmpty(movieRating)))
            {
                movies = movies.Where(x => x.Genre == movieGenre &&
                                      x.Rating == movieRating);
            }
            // If the genre is empty, release date is empty, and rating is not empty
            else if (String.IsNullOrEmpty(movieGenre) && movieReleaseDate == DateTime.MinValue && !(String.IsNullOrEmpty(movieRating)))
            {
                movies = movies.Where(x => x.Rating == movieRating);
            }

            var movieVM = new MovieGenreViewModel();
            movieVM.Genres = new SelectList(await genreQuery.Distinct().ToListAsync());
            movieVM.ReleaseDates = new SelectList(await releaseDateQuery.Distinct().ToListAsync());
            movieVM.Ratings = new SelectList(await ratingQuery.Distinct().ToListAsync());
            movieVM.Movies = await movies.ToListAsync();
            movieVM.SearchString = searchString;

            return View(movieVM);       
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.ID == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Title,ReleaseDate,Genre,Price,Rating")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Title,ReleaseDate,Genre,Price,Rating")] Movie movie)
        {
            if (id != movie.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.ID == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movie.FindAsync(id);
            _context.Movie.Remove(movie);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movie.Any(e => e.ID == id);
        }
    }
}
