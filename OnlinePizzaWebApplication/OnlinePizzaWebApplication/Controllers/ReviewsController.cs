using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlinePizzaWebApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace OnlinePizzaWebApplication.Controllers
{
    [Authorize]
    public class ReviewsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ReviewsController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //public IActionResult MyAction()
        //{
        //    var userId = _userManager.GetUserId(HttpContext.User);

        //    var model = GetSomeModelByUserId(userId);

        //    return View(model);
        //}

        // GET: Reviews
        [Authorize]
        public async Task<IActionResult> Index()
        {
            //var userIdTest = "eaff05c2-49b1-4bdc-bd24-8f7a34571428";
            //var userId = _userManager.GetUserId(HttpContext.User);

            var user = await _userManager.GetUserAsync(HttpContext.User);

            var reviews = _context.Reviews.Include(r => r.Pizza).ToList().Where(r => r.User == user);

            return View(reviews);
        }

        // GET: Reviews
        [AllowAnonymous]
        public async Task<IActionResult> ListAll()
        {
            var reviews = await _context.Reviews.Include(r => r.Pizza).Include(r => r.User).ToListAsync();
            return View(reviews);
        }

        // GET: Reviews/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reviews = await _context.Reviews
                .Include(r => r.Pizza)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (reviews == null)
            {
                return NotFound();
            }

            return View(reviews);
        }

        // GET: Reviews/Create
        public IActionResult Create()
        {
            ViewData["PizzaId"] = new SelectList(_context.Pizzas, "Id", "Name");
            return View();
        }

        // POST: Reviews/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Grade,Date,PizzaId")] Reviews reviews)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(HttpContext.User);
                reviews.UserId = userId;

                _context.Add(reviews);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["PizzaId"] = new SelectList(_context.Pizzas, "Id", "Name", reviews.PizzaId);
            return View(reviews);
        }

        // GET: Reviews/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reviews = await _context.Reviews.SingleOrDefaultAsync(m => m.Id == id);
            var userId = _userManager.GetUserId(HttpContext.User);

            if (reviews == null)
            {
                return NotFound();
            }

            if (reviews.UserId != userId)
            {
                return BadRequest("You do not have permissions to edit this review.");
            }

            ViewData["PizzaId"] = new SelectList(_context.Pizzas, "Id", "Name", reviews.PizzaId);
            return View(reviews);
        }

        // POST: Reviews/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Grade,Date,PizzaId")] Reviews reviews)
        {
            if (id != reviews.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    _context.Update(reviews);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    var userId = _userManager.GetUserId(HttpContext.User);
                    if (!ReviewsExists(reviews.Id))
                    {
                        return NotFound();
                    }
                    else if (reviews.UserId != userId)
                    {
                        return BadRequest("You do not have permissions to edit this review.");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            ViewData["PizzaId"] = new SelectList(_context.Pizzas, "Id", "Name", reviews.PizzaId);
            return View(reviews);
        }

        // GET: Reviews/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reviews = await _context.Reviews
                .Include(r => r.Pizza)
                .SingleOrDefaultAsync(m => m.Id == id);
            var userId = _userManager.GetUserId(HttpContext.User);

            if (reviews == null)
            {
                return NotFound();
            }

            if (reviews.UserId != userId)
            {
                return BadRequest("You do not have permissions to edit this review.");
            }

            return View(reviews);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reviews = await _context.Reviews.SingleOrDefaultAsync(m => m.Id == id);
            _context.Reviews.Remove(reviews);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool ReviewsExists(int id)
        {
            return _context.Reviews.Any(e => e.Id == id);
        }
    }
}