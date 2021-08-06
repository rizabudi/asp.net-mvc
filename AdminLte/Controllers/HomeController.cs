using AdminLte.Data;
using AdminLte.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AdminLte.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly PostgreDbContext _db;
        public HomeController(ILogger<HomeController> logger, PostgreDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        [AllowAnonymous]
        [HttpGet("")]
        public async Task<IActionResult> IndexAsync()
        {
            var surveys = await _db.QuestionPackages
                       .Include(x => x.Assesment)
                       .Include(x => x.QuestionPackageLines)
                       .OrderBy(x => x.Assesment.Name)
                       .ThenBy(x => x.Name)
                       .ToListAsync();

            ViewData["Surveys"] = surveys;
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
