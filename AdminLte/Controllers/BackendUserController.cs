using AdminLte.Data;
using AdminLte.Helpers;
using AdminLte.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminLte.Controllers
{
    [Authorize(Roles = "Pengguna Khusus")]
    [CustomAuthFilter("Access_PengaturanPengguna_PenggunaKhusus")]
    public class BackendUserController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<User> _userManager;

        public BackendUserController(ApplicationDbContext db, UserManager<User> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [HttpGet("backend-user/table-data-view")]
        public async Task<IActionResult> GetAll(int page = 1)
        {
            try
            {
                int entityID = 0;
                byte[] bytes;
                if (HttpContext.Session.TryGetValue("User_Entity", out bytes))
                {
                    string value = Encoding.ASCII.GetString(bytes);
                    int.TryParse(value, out entityID);
                }

                var data = await _db.BackendUsers
                    .Include(x=>x.Entity)
                    .Include(x=>x.User)
                    .Include(x=>x.UserAccess)
                    .Where(x=> entityID == 0 || x.Entity == null ? true : x.Entity.ID == entityID)
                    .OrderBy(x=>x.Name)
                    .Skip((page-1)*10)
                    .Take(10)
                    .ToListAsync();

                var rows = new List<RowModel>();
                foreach(var row in data)
                {
                    rows.Add(
                        new RowModel { 
                            IDString = row.UserId.ToString(), 
                            Value = new string[] 
                            { 
                                row.Name,
                                row.Entity == null ? "" : row.Entity.Name,
                                row.User.UserName,
                                row.UserAccess == null ? "" : row.UserAccess.Name
                            }
                        }
                    );
                }

                ViewData["Rows"] = rows;
                ViewData["Page"] = page;

                return PartialView("~/Views/Shared/_TableData.cshtml");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        [HttpGet("backend-user/table-paging-view")]
        public IActionResult GetPaging(int page = 1)
        {
            try
            {
                int entityID = 0;
                byte[] bytes;
                if (HttpContext.Session.TryGetValue("User_Entity", out bytes))
                {
                    string value = Encoding.ASCII.GetString(bytes);
                    int.TryParse(value, out entityID);
                }

                var total = _db.BackendUsers
                    .Where(x => entityID == 0 || x.Entity == null ? true : x.Entity.ID == entityID)
                    .Count();
                ViewData["Total"] = total;
                ViewData["Page"] = page;

                return PartialView("~/Views/Shared/_PagingView.cshtml");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        [HttpGet("backend-user/form-view")]
        public async Task<IActionResult> GetFormAsync(string id = "")
        {
            try
            {
                BackendUser userFromDb = null;
                if(id != "")
                {
                    userFromDb = await _db.BackendUsers
                        .Include(x=>x.User)
                        .Include(x=>x.Entity)
                        .FirstOrDefaultAsync(e => e.UserId == id);
                }

                int entityID = 0;
                byte[] bytes;
                if (HttpContext.Session.TryGetValue("User_Entity", out bytes))
                {
                    string value = Encoding.ASCII.GetString(bytes);
                    int.TryParse(value, out entityID);
                }

                var entityList = await _db.Entities
                    .Where(x => x.Level <= 1 && (entityID == 0 ? true : x.ID == entityID))
                    .OrderBy(x => x.Name).ToListAsync();
                var entities = Entity.getEntities(entityList, 0, 0);

                var userAccesses = await _db.UserAccesses
                    .OrderBy(x => x.Name)
                    .ToDictionaryAsync(x=>x.ID.ToString(), y=>y.Name);

                List<FormModel> FormModels = new List<FormModel>();
                FormModels.Add(new FormModel { Label = "UserId", Name = "UserId", InputType = InputType.HIDDEN, Value = userFromDb == null ? "" : userFromDb.UserId });
                FormModels.Add(new FormModel { Label = "Nama", Name = "Name", InputType = InputType.TEXT, Value = userFromDb == null ? "" : userFromDb.Name, IsRequired = true });
                FormModels.Add(new FormModel { Label = "Holding/ Sub-Holding", Name = "Entity", InputType = InputType.DROPDOWN, Options = entities, Value = userFromDb == null || userFromDb.Entity == null ? (entityID == 0 ? "" : entityID.ToString()) : userFromDb.Entity.ID.ToString(), IsRequired = false, IsDisable = entityID != 0 });
                FormModels.Add(new FormModel { Label = "Hak Akses", Name = "UserAccess", InputType = InputType.DROPDOWN, Options = userAccesses, Value = userFromDb == null || userFromDb.UserAccess == null ? "" : userFromDb.UserAccess.ID.ToString(), IsRequired = true });
                FormModels.Add(new FormModel { Label = "User Name", Name = "UserName", InputType = InputType.TEXT, Value = userFromDb == null ? "" : userFromDb.User.UserName, IsRequired = true });
                FormModels.Add(new FormModel { Label = "Password", Name = "Password", Note = (userFromDb != null ? "Kosongkan jika tidak ingin mengganti password" : ""), InputType = InputType.PASSWORD, Value = "", IsRequired = id == "" });

                ViewData["Forms"] = FormModels;

                return PartialView("~/Views/Shared/_FormView.cshtml");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        [HttpGet("backend-user")]
        public IActionResult Index()
        {
            ViewData["Title"] = "Pengguna Khusus";
            List<ColumnModel> ColumnModels = new List<ColumnModel>();
            ColumnModels.Add(new ColumnModel { Label = "Nama", Name = "Name", Style = "width: 15%; min-width: 200px" });
            ColumnModels.Add(new ColumnModel { Label = "Holding/ Sub-Holding", Name = "Entity" });
            ColumnModels.Add(new ColumnModel { Label = "User Name", Name = "UserName" });
            ColumnModels.Add(new ColumnModel { Label = "Hak Akses", Name = "UserAccess" });


            ViewData["Columns"] = ColumnModels;
            ViewData["Script"] = "backend-user.js";

            return View("~/Views/Shared/_Index.cshtml");
        }

        [HttpPost("backend-user/save")]
        public async Task<IActionResult> Save(BackendUser backendUser)
        {
            try
            {
                BackendUser userFromDb = await _db.BackendUsers
                    .Include(x=>x.Entity)
                    .Include(x=>x.User)
                    .FirstOrDefaultAsync(e => e.UserId == backendUser.UserId);

                if (userFromDb == null)
                {
                    var user = new User { UserName = backendUser.User.UserName };
                    var result = await _userManager.CreateAsync(user, backendUser.User.PasswordHash);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "Pengguna Khusus");

                        backendUser.Entity = await _db.Entities.FirstOrDefaultAsync(e => e.ID == backendUser.Entity.ID);
                        backendUser.UserAccess = await _db.UserAccesses.FirstOrDefaultAsync(e => e.ID == backendUser.UserAccess.ID);

                        backendUser.User = user;
                        _db.BackendUsers.Add(backendUser);
                        _db.SaveChanges();

                        return Json(new { success = true, message = "Data berhasil disimpan" });
                    }
                    else
                    {
                        var msg = "";
                        foreach (var error in result.Errors)
                        {
                            msg += error.Description + "<br/>";
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return Json(new { success = false, message = "Terjadi kesalahan. Err : " + msg });
                    }
                }
                else
                {
                    backendUser.Entity = await _db.Entities.FirstOrDefaultAsync(e => e.ID == backendUser.Entity.ID);
                    backendUser.UserAccess = await _db.UserAccesses.FirstOrDefaultAsync(e => e.ID == backendUser.UserAccess.ID);

                    if (backendUser.User.PasswordHash != null)
                    {
                        var token = await _userManager.GeneratePasswordResetTokenAsync(userFromDb.User);
                        var result = await _userManager.ResetPasswordAsync(userFromDb.User, token, backendUser.User.PasswordHash);
                        if (result.Succeeded)
                        {
                        }
                        else
                        {
                            var msg = "";
                            foreach (var error in result.Errors)
                            {
                                msg += error.Description + "<br/>";
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                            return Json(new { success = false, message = "Terjadi kesalahan. Err : " + msg });
                        }
                    }

                    userFromDb.Name = backendUser.Name;
                    userFromDb.Entity = backendUser.Entity;
                    userFromDb.UserAccess = backendUser.UserAccess;
                    _db.BackendUsers.Update(userFromDb);
                    _db.SaveChanges();
                    return Json(new { success = true, message = "Data berhasil diperbarui" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new { success = false, message = "Terjadi kesalahan. Err : " + ex.Message });
            }
        }

        [HttpPost("backend-user/delete")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var userFromDb = await _db.BackendUsers
                    .Include(x=>x.User)
                    .FirstOrDefaultAsync(e => e.UserId == id);

                if (userFromDb == null)
                {
                    return Json(new { success = false, message = "Data tidak ditemukan" });
                }

                _db.BackendUsers.Remove(userFromDb);
                await _userManager.RemoveFromRoleAsync(userFromDb.User, "Pengguna Khusus");
                await _userManager.DeleteAsync(userFromDb.User);
                await _db.SaveChangesAsync();
                return Json(new { success = true, message = "Data berhasil dihapus" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new { success = false, message = "Terjadi kesalahan. Err : " + ex.Message });
            }
        }
    }
}
