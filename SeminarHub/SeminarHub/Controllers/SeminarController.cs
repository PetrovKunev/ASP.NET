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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Join(int id)
        {
            var seminar = await _context.Seminars
                .Include(s => s.SeminarParticipants)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (seminar == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            // Проверка дали семинарът вече е в колекцията на потребителя
            bool isAlreadyJoined = seminar.SeminarParticipants
                .Any(sp => sp.ParticipantId == userId);

            if (isAlreadyJoined)
            {
                // Ако семинарът вече е добавен, пренасочване към /Seminar/All
                return RedirectToAction(nameof(All));
            }

            // Добавяне на потребителя към участниците в семинара
            seminar.SeminarParticipants.Add(new SeminarParticipant
            {
                SeminarId = seminar.Id,
                ParticipantId = userId
            });

            await _context.SaveChangesAsync();

            // Пренасочване към /Seminar/Joined след успешното добавяне
            return RedirectToAction(nameof(Joined));
        }


        [HttpGet]
        public async Task<IActionResult> Joined()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var seminars = await _context.Seminars
                .Where(s => s.SeminarParticipants.Any(sp => sp.ParticipantId == userId))
                .Select(s => new SeminarViewModel
                {
                    Id = s.Id,
                    Topic = s.Topic,
                    Lecturer = s.Lecturer,
                    Category = s.Category.Name,
                    DateAndTime = s.DateAndTime,
                    Organizer = s.Organizer.UserName ?? string.Empty
                })
                .ToListAsync();

            return View(seminars);
        }

        // POST: Seminar/Leave?id={id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Leave(int id)
        {
            var seminar = await _context.Seminars
                .Include(s => s.SeminarParticipants)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (seminar == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            // Намери участника в семинара и го премахни
            var seminarParticipant = seminar.SeminarParticipants
                .FirstOrDefault(sp => sp.ParticipantId == userId);

            if (seminarParticipant != null)
            {
                seminar.SeminarParticipants.Remove(seminarParticipant);
                await _context.SaveChangesAsync();
            }

            // Пренасочване обратно към страницата Seminar/Joined
            return RedirectToAction(nameof(Joined));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var seminar = await _context.Seminars
                .FirstOrDefaultAsync(s => s.Id == id);

            if (seminar == null)
            {
                return BadRequest();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            // Проверка дали текущият потребител е създател на семинара
            if (seminar.OrganizerId != userId)
            {
                return Unauthorized();
            }

            // Подготвяме модел за изтриване, който да се покаже на изгледа
            var model = new SeminarDeleteViewModel
            {
                Id = seminar.Id,
                Topic = seminar.Topic
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var seminar = await _context.Seminars
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted); // Намираме само активни семинари

            if (seminar == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            // Проверка дали потребителят е създателят на семинара
            if (seminar.OrganizerId != userId)
            {
                return Unauthorized();
            }

            // Вместо да изтриваме физически, маркираме като изтрит
            seminar.IsDeleted = true;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

    }
}
