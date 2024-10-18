using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeminarHub.Data;
using SeminarHub.Models;
using System.Security.Claims;

namespace SeminarHub.Controllers
{
    public class SeminarController : Controller
    {
        private readonly SeminarHubDbContext _context;

        public SeminarController(SeminarHubDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var seminarViewModels = await _context.Seminars
               .Where(s => s.IsDeleted == false)
               .Select(s => new SeminarViewModel
               {
                  Id = s.Id,
                  Topic = s.Topic,
                  Lecturer = s.Lecturer,
                  Category = s.Category.Name, // Използва се името на категорията
                  DateAndTime = s.DateAndTime,
                  Organizer = s.Organizer.UserName ?? string.Empty // Използва се UserName на организатора
               })
               .ToListAsync();

            return View(seminarViewModels);
        }

        [HttpGet]
        public IActionResult Add()
        {
            var categories = _context.Categories.ToList(); // Взимаме всички категории

            var viewModel = new SeminarCreateViewModel
            {
                Categories = categories
            };

            return View(viewModel); // Подаваме модел към изгледа
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(SeminarCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Ако има грешки, презареждаме категорията и връщаме изгледа
                model.Categories = _context.Categories.ToList();
                return View(model);
            }

            // Създаваме нов обект Seminar и попълваме полетата от модела
            var seminar = new Seminar
            {
                Topic = model.Topic,
                Lecturer = model.Lecturer,
                Details = model.Details,
                DateAndTime = model.DateAndTime,
                Duration = model.Duration,
                CategoryId = model.CategoryId,
                OrganizerId = User.FindFirstValue(ClaimTypes.NameIdentifier) // Взимаме текущия потребител
            };

            await _context.Seminars.AddAsync(seminar);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var seminar = await _context.Seminars
                .FirstOrDefaultAsync(s => s.Id == id && s.OrganizerId == User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (seminar == null)
            {
                return Unauthorized();
            }

            var viewModel = new SeminarCreateViewModel
            {
                Topic = seminar.Topic,
                Lecturer = seminar.Lecturer,
                Details = seminar.Details,
                DateAndTime = seminar.DateAndTime,
                Duration = seminar.Duration,
                CategoryId = seminar.CategoryId,
                Categories = _context.Categories.ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SeminarCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = _context.Categories.ToList();
                return View(model);
            }

            var seminar = await _context.Seminars.FirstOrDefaultAsync(s => s.Id == id && s.OrganizerId == User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (seminar == null)
            {
                return Unauthorized();
            }

            seminar.Topic = model.Topic;
            seminar.Lecturer = model.Lecturer;
            seminar.Details = model.Details;
            seminar.DateAndTime = model.DateAndTime;
            seminar.Duration = model.Duration;
            seminar.CategoryId = model.CategoryId;

            await _context.SaveChangesAsync();

            return RedirectToAction("All");
        }

        [HttpPost]
        public async Task<IActionResult> Join(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            // Проверка дали потребителят вече е присъединен към семинара
            var existingSubscription = await _context.SeminarParticipants
                .FirstOrDefaultAsync(sp => sp.SeminarId == id && sp.ParticipantId == userId);

            if (existingSubscription == null)
            {
                var seminarParticipant = new SeminarParticipant
                {
                    SeminarId = id,
                    ParticipantId = userId
                };

                _context.SeminarParticipants.Add(seminarParticipant);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("All");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var seminar = await _context.Seminars
                .Include(s => s.Category)
                .Include(s => s.Organizer)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (seminar == null)
            {
                return NotFound();
            }

            var viewModel = new SeminarViewModel
            {
                Id = seminar.Id,
                Topic = seminar.Topic,
                Lecturer = seminar.Lecturer,
                Details = seminar.Details,
                DateAndTime = seminar.DateAndTime,
                Category = seminar.Category.Name,
                Organizer = seminar.Organizer.UserName ?? string.Empty
            };

            return View(viewModel);
        }


        [HttpGet]
        public async Task<IActionResult> Joined()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            // Взимаме семинарите, към които потребителят се е присъединил
            var joinedSeminars = await _context.SeminarParticipants
                .Where(sp => sp.ParticipantId == userId)
                .Select(sp => new SeminarViewModel
                {
                    Id = sp.Seminar.Id,
                    Topic = sp.Seminar.Topic,
                    Lecturer = sp.Seminar.Lecturer,
                    DateAndTime = sp.Seminar.DateAndTime,
                    Organizer = sp.Seminar.Organizer.UserName
                })
                .ToListAsync();

            return View(joinedSeminars);
        }


    }
}
