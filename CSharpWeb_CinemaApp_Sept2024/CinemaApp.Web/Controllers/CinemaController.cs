﻿
namespace CinemaApp.Web.Controllers
{
    using CinemaApp.Data;
    using CinemaApp.Data.Models;
    using CinemaApp.Web.ViewModels.Cinema;
    using CinemaApp.Web.ViewModels.Movie;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    
    

    public class CinemaController : Controller
    {
        private readonly CinemaDbContext dbContext;

        public CinemaController(CinemaDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<CinemaIndexViewModel> cinemas = await this.dbContext
                .Cinemas
                .Select(c => new CinemaIndexViewModel
                {
                    Id = c.Id.ToString(),
                    Name = c.Name,
                    Location = c.Location
                })
                .OrderBy(c => c.Location)
                .ToArrayAsync();

            return this.View(cinemas);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CinemaCreateViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            Cinema cinema = new Cinema
            {
                Name = model.Name,
                Location = model.Location
            };

            await this.dbContext.Cinemas.AddAsync(cinema);
            await this.dbContext.SaveChangesAsync();

            return this.RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(string? id)
        {
            Guid cinemaId = Guid.Empty;
            bool isIdValid = isCinemaIdValid(id, ref cinemaId);

            if (!isIdValid)
            {
                return this.RedirectToAction(nameof(Index));
            }

            Cinema? cinema = await this.dbContext
                .Cinemas
                .Include(c => c.CinemaMovies)
            .ThenInclude(cm => cm.Movie)
                .FirstOrDefaultAsync(c => c.Id == cinemaId);

            if (cinema == null)
            {
                return this.RedirectToAction(nameof(Index));
            }

            CinemaDetailsViewModel model = new CinemaDetailsViewModel
            {
                Name = cinema.Name,
                Location = cinema.Location,
                Movies = cinema.CinemaMovies
                    .Select(cm => new CinemaMovieViewModel
                    {
                        Title = cm.Movie.Title,
                        Duration = cm.Movie.Duration
                    })
                    .ToArray()
            };

            return this.View(model);
        }

        private bool isCinemaIdValid(string? id, ref Guid cinemaId)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                return false;
            }

            bool isGuidValid = Guid.TryParse(id, out cinemaId);
            if (!isGuidValid)
            {
                return false;
            }

            return true;
        }
    }
}
