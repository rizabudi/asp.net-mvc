using AdminLte.Data;
using AdminLte.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminLte.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly PostgreDbContext _db;
        private readonly UserManager<User> _userManager;
        public HomeController(ILogger<HomeController> logger, PostgreDbContext db, UserManager<User> user)
        {
            _logger = logger;
            _db = db;
            _userManager = user;
        }

        [AllowAnonymous]
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("home")]
        public async Task<IActionResult> HomeAsync()
        {
            var surveys = await _db.QuestionPackages
                       .Include(x => x.Assesment)
                       .Include(x => x.QuestionPackageLines)
                       .OrderBy(x => x.Assesment.Name)
                       .ThenBy(x => x.Name)
                       .ToListAsync();

            ViewData["Surveys"] = surveys;

            var user = await _userManager.GetUserAsync(User);
            var backendUser = await _db.BackendUsers
                                .Include(x => x.User)
                                .Include(x => x.UserAccess)
                                .Include(x => x.Entity)
                                .FirstOrDefaultAsync(x => x.User.Id == user.Id);

            if(backendUser != null)
            {
                if (backendUser.UserAccess == null)
                {
                    backendUser.UserAccess = new UserAccess();
                }
                HttpContext.Session.Set("Access_MasterData_DaftarSurvei", Encoding.ASCII.GetBytes(backendUser.UserAccess.Access_MasterData_DaftarSurvei ? "1" : "0"));
                HttpContext.Session.Set("Access_MasterData_DimensiHorizontal", Encoding.ASCII.GetBytes(backendUser.UserAccess.Access_MasterData_DimensiHorizontal ? "1" : "0"));
                HttpContext.Session.Set("Access_MasterData_DimensiVertical", Encoding.ASCII.GetBytes(backendUser.UserAccess.Access_MasterData_DimensiVertical ? "1" : "0"));
                HttpContext.Session.Set("Access_MasterData_JenisSurvei", Encoding.ASCII.GetBytes(backendUser.UserAccess.Access_MasterData_JenisSurvei ? "1" : "0"));
                HttpContext.Session.Set("Access_MasterData_Konstruk", Encoding.ASCII.GetBytes(backendUser.UserAccess.Access_MasterData_Konstruk ? "1" : "0"));
                HttpContext.Session.Set("Access_MasterData_Periode", Encoding.ASCII.GetBytes(backendUser.UserAccess.Access_MasterData_Periode ? "1" : "0"));
                HttpContext.Session.Set("Access_MasterData_Pertanyaan", Encoding.ASCII.GetBytes(backendUser.UserAccess.Access_MasterData_Pertanyaan ? "1" : "0"));
                HttpContext.Session.Set("Access_MasterData_StrukturOrganisasi_Entitas", Encoding.ASCII.GetBytes(backendUser.UserAccess.Access_MasterData_StrukturOrganisasi_Entitas ? "1" : "0"));
                HttpContext.Session.Set("Access_MasterData_StrukturOrganisasi_LevelJabatan", Encoding.ASCII.GetBytes(backendUser.UserAccess.Access_MasterData_StrukturOrganisasi_LevelJabatan ? "1" : "0"));
                HttpContext.Session.Set("Access_PengaturanPengguna_HakAkses", Encoding.ASCII.GetBytes(backendUser.UserAccess.Access_PengaturanPengguna_HakAkses ? "1" : "0"));
                HttpContext.Session.Set("Access_PengaturanPengguna_PenggunaKhusus", Encoding.ASCII.GetBytes(backendUser.UserAccess.Access_PengaturanPengguna_PenggunaKhusus ? "1" : "0"));
                HttpContext.Session.Set("Access_PengaturanPengguna_PenggunaUmum", Encoding.ASCII.GetBytes(backendUser.UserAccess.Access_PengaturanPengguna_PenggunaUmum ? "1" : "0"));
                HttpContext.Session.Set("Access_Penjadwalan_PenjadwalanPeserta", Encoding.ASCII.GetBytes(backendUser.UserAccess.Access_Penjadwalan_PenjadwalanPeserta ? "1" : "0"));
                HttpContext.Session.Set("Access_Penjadwalan_PenjadwalanSurvei", Encoding.ASCII.GetBytes(backendUser.UserAccess.Access_Penjadwalan_PenjadwalanSurvei ? "1" : "0"));

                HttpContext.Session.Set("User_Entity", Encoding.ASCII.GetBytes(backendUser.Entity == null ? "0" : backendUser.Entity.ID.ToString()));

            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet("home/errors")]
        [Route("home/errors/{statusCode:int}")]
        public IActionResult Errors(int statusCode)
        {
            if (statusCode == 404)
            {
                return View("~/Views/Shared/_404.cshtml");
            }
            return View(statusCode);
        }
    }
}
