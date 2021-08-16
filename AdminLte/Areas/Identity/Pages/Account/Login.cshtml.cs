using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AdminLte.Models;
using AdminLte.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace AdminLte.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly PostgreDbContext _db;

        public LoginModel(SignInManager<User> signInManager, 
            ILogger<LoginModel> logger,
            UserManager<User> userManager,
            PostgreDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _db = db;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            public string Username { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Username, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var participantUser = await _db.ParticipantUsers.Include(x=>x.User).FirstOrDefaultAsync(x => x.User.UserName == Input.Username);
                    _logger.LogInformation("User logged in.");

                    if(participantUser != null)
                    {
                        return LocalRedirect(Url.Content("~/profile?isEdit=true"));
                    } 
                    else
                    {
                        var backendUser = await _db.BackendUsers
                            .Include(x=>x.User)
                            .Include(x => x.UserAccess)
                            .Include(x => x.Entity)
                            .FirstOrDefaultAsync(x => x.User.UserName == Input.Username);

                        if(backendUser.UserAccess == null)
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
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
