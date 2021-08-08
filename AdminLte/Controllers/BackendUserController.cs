using AdminLte.Data;
using AdminLte.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminLte.Controllers
{
    [Authorize(Roles = "Pengguna Khusus")]
    public class BackendUserController : Controller
    {
        private readonly PostgreDbContext _db;
        private readonly UserManager<User> _userManager;

        public BackendUserController(PostgreDbContext db, UserManager<User> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [HttpGet("backend-user/table-data-view")]
        public async Task<IActionResult> GetAll(int page = 1)
        {
            try
            {
                var data = await _db.BackendUsers
                    .Include(x=>x.Entity)
                    .Include(x=>x.User)
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
                                row.Entity.Name ,
                                row.User.UserName
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
                var total = _db.BackendUsers.Count();
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

                var entityList = await _db.Entities
                    .Where(x=>x.Level <= 1)
                    .OrderBy(x => x.Name).ToListAsync();
                var entities = Entity.getEntities(entityList, 0, 0);

                List<FormModel> FormModels = new List<FormModel>();
                FormModels.Add(new FormModel { Label = "UserId", Name = "UserId", InputType = InputType.HIDDEN, Value = userFromDb == null ? "" : userFromDb.UserId });
                FormModels.Add(new FormModel { Label = "Nama", Name = "Name", InputType = InputType.TEXT, Value = userFromDb == null ? "" : userFromDb.Name, IsRequired = true });
                FormModels.Add(new FormModel { Label = "Holding/ Sub-Holding", Name = "Entity", InputType = InputType.DROPDOWN, Options = entities, Value = userFromDb == null ? "" : userFromDb.Entity.ID.ToString(), IsRequired = true });
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

                    if(backendUser.User.PasswordHash != null)
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
