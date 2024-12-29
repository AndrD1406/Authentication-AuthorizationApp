using Microsoft.AspNetCore.Mvc;
using TaskAuthenticationAuthorization.Models;
using TaskAuthenticationAuthorization.ViewModels;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace TaskAuthenticationAuthorization.Controllers
{
    public class AccountController : Controller
    {
        private ShoppingContext db;
        public AccountController(ShoppingContext context)
        {
            db = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if(ModelState.IsValid)
            {
                User user = await db.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user == null)
                {
                    user = new Models.User 
                    {
                        Email = model.Email,
                        Password = model.Password,
                        LastName = model.LastName,
                        FirstName = model.FirstName,
                        Address = model.Address,
                        BuyerType = BuyerType.Regular
                    };
                    Role userRole = await db.Roles.FirstOrDefaultAsync(r => r.Name == "buyer");
                    if(userRole != null)
                    {
                        user.Role = userRole;
                    }
                    db.Users.Add(user);
                    await db.SaveChangesAsync();
                    await Authenticate(user);
                    return RedirectToAction("Index", "Home");
                }
                else
                { 
                    ModelState.AddModelError("", "Incorrect login and(or) password");
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await db.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);
                if(user != null)
                {
                    await Authenticate(user);
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Incorrect login and(or) password");
            }
            return View(model);
        }

        private async Task Authenticate(User user)
        {
            // creating one claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role?.Name),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };
            
            if (user.BuyerType.HasValue)
            {
                claims.Add(new Claim("BuyerType", ((int)user.BuyerType.Value).ToString()));
            }

            // creating ClaimsIdentity object
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            // setting authenticational cookies
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        [Authorize(Roles = "admin")]
        public IActionResult Index()
        {
            var users = db.Users;
            var models = users.Include(u => u.Role);
            return View(models);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await db.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            // Populate ViewBag with roles and buyer types
            ViewBag.RoleId = new SelectList(db.Roles, "Id", "Name", user.RoleId);
            ViewBag.BuyerType = new SelectList(Enum.GetValues(typeof(BuyerType)).Cast<BuyerType>().Select(bt => new SelectListItem
            {
                Value = ((int)bt).ToString(),
                Text = bt.ToString()
            }), "Value", "Text", user.BuyerType);

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, [Bind("RoleId","BuyerType")] User user)
        {
            Console.WriteLine();
            if (id != user.Id)
            {
                return NotFound();
            }

            var userToUpdate = await db.Users.FindAsync(id);
            if (userToUpdate == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    userToUpdate.RoleId = user.RoleId;
                    userToUpdate.BuyerType = user.BuyerType;

                    db.Update(userToUpdate);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!db.Users.Any(e => e.Id == id))
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
            return View(user);
        }

    }
}
