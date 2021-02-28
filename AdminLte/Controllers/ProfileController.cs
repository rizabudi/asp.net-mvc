using AdminLte.Data;
using AdminLte.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminLte.Controllers
{
    public class ProfileController : Controller
    {
        private readonly PostgreDbContext _db;
        private readonly UserManager<User> _userManager;
        public ProfileController(PostgreDbContext db, UserManager<User> user)
        {
            _db = db;
            _userManager = user;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> IndexAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var participantUser = await _db.ParticipantUsers
                .Include(x=>x.Entity)
                .Include(x => x.Entity)
                .Include(x => x.Position)
                .Include(x => x.CompanyFunction)
                .Include(x => x.Divition)
                .Include(x => x.Department)
                .FirstOrDefaultAsync(x => x.UserId == user.Id);

            if(participantUser == null)
            {
                return Redirect("/home/errors/404");
            }

            return View(participantUser);
        }
    }
}
