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
    public class ParticipantUserController : Controller
    {
        private readonly PostgreDbContext _db;
        private readonly UserManager<User> _userManager;

        public ParticipantUserController(PostgreDbContext db, UserManager<User> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [HttpGet("participant-user/table-data-view")]
        public async Task<IActionResult> GetAll(int page = 1)
        {
            try
            {
                var data = await _db.ParticipantUsers
                    .Include(x => x.Entity)
                    .Include(x => x.Divition)
                    .Include(x => x.Department)
                    .Include(x => x.CompanyFunction)
                    .Include(x => x.Position)
                    .Include(x=> x.User)
                    .OrderBy(x=> x.Name)
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
                                row.EmployeeNumber,
                                row.Name,
                                row.Email,
                                row.Phone,
                                row.Sex ? "Laki-laki" : "Perempuan",
                                row.User.UserName,
                                row.Entity.Name,
                                row.Position.Name,
                                row.CompanyFunction.Name,
                                row.Divition.Name,
                                row.Department.Name
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

        [HttpGet("participant-user/table-paging-view")]
        public IActionResult GetPaging(int page = 1)
        {
            try
            {
                var total = _db.ParticipantUsers.Count();
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

        [HttpGet("participant-user/form-view")]
        public async Task<IActionResult> GetFormAsync(string id = "")
        {
            try
            {
                ParticipantUser userFromDb = null;
                if(id != "")
                {
                    userFromDb = await _db.ParticipantUsers
                        .Include(x => x.Entity)
                        .Include(x => x.Divition)
                        .Include(x => x.Department)
                        .Include(x => x.CompanyFunction)
                        .Include(x => x.Position)
                        .Include(x => x.User)
                        .FirstOrDefaultAsync(e => e.UserId == id);
                }

                var entities = await _db.Entities.OrderBy(x => x.Name).ToDictionaryAsync(x => x.ID.ToString(), y => y.Name);
                var positions = await _db.Position.OrderBy(x => x.Name).ToDictionaryAsync(x => x.ID.ToString(), y => y.Name);
                var departments = await _db.Departments.OrderBy(x => x.Name).ToDictionaryAsync(x => x.ID.ToString(), y => y.Name);
                var divitions = await _db.Divitions.OrderBy(x => x.Name).ToDictionaryAsync(x => x.ID.ToString(), y => y.Name);
                var functions = await _db.CompanyFunctions.OrderBy(x => x.Name).ToDictionaryAsync(x => x.ID.ToString(), y => y.Name);

                List<FormModel> FormModels = new List<FormModel>();
                FormModels.Add(new FormModel { Label = "UserId", Name = "UserId", InputType = InputType.HIDDEN, Value = userFromDb == null ? "" : userFromDb.UserId });
                FormModels.Add(new FormModel { Label = "No Karyawan", Name = "EmployeeNumber", InputType = InputType.TEXT, Value = userFromDb == null ? "" : userFromDb.EmployeeNumber });
                FormModels.Add(new FormModel { Label = "Nama", Name = "Name", InputType = InputType.TEXT, Value = userFromDb == null ? "" : userFromDb.Name, IsRequired = true });
                FormModels.Add(new FormModel { Label = "Email", Name = "Email", InputType = InputType.EMAIL, Value = userFromDb == null ? "" : userFromDb.Email });
                FormModels.Add(new FormModel { Label = "Telp", Name = "Phone", InputType = InputType.TEXT, Value = userFromDb == null ? "" : userFromDb.Phone });
                FormModels.Add(new FormModel { Label = "Entitas", Name = "Entity", InputType = InputType.DROPDOWN, Options = entities, Value = userFromDb == null ? "" : userFromDb.Entity.ID.ToString(), IsRequired = true, FormPosition = FormPosition.RIGHT });
                FormModels.Add(new FormModel { Label = "Posisi", Name = "Position", InputType = InputType.DROPDOWN, Options = positions, Value = userFromDb == null ? "" : userFromDb.Entity.ID.ToString(), IsRequired = true, FormPosition = FormPosition.RIGHT });
                FormModels.Add(new FormModel { Label = "Fungsi", Name = "CompanyFunction", InputType = InputType.DROPDOWN, Options = functions, Value = userFromDb == null ? "" : userFromDb.CompanyFunction.ID.ToString(), IsRequired = true, FormPosition = FormPosition.RIGHT });
                FormModels.Add(new FormModel { Label = "Divisi", Name = "Divition", InputType = InputType.DROPDOWN, Options = divitions, Value = userFromDb == null ? "" : userFromDb.Divition.ID.ToString(), IsRequired = true, FormPosition = FormPosition.RIGHT });
                FormModels.Add(new FormModel { Label = "Departemen", Name = "Department", InputType = InputType.DROPDOWN, Options = departments, Value = userFromDb == null ? "" : userFromDb.Department.ID.ToString(), IsRequired = true, FormPosition = FormPosition.RIGHT });
                
                FormModels.Add(new FormModel { Label = "User Name", Name = "UserName", InputType = InputType.TEXT, Value = userFromDb == null ? "" : userFromDb.User.UserName, IsRequired = true });
                FormModels.Add(new FormModel { Label = "Password " + (userFromDb != null ? "(Kosongkan jika tidak ingin mengganti password)" : ""), Name = "Password", InputType = InputType.PASSWORD, Value = "", IsRequired = id == "" });

                ViewData["Forms"] = FormModels;
                ViewData["ColumnNumber"] = 2;

                return PartialView("~/Views/Shared/_FormView.cshtml");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        [HttpGet("participant-user")]
        public IActionResult Index()
        {
            ViewData["Title"] = "Pengguna Umum";
            List<ColumnModel> ColumnModels = new List<ColumnModel>();
            ColumnModels.Add(new ColumnModel { Label = "No Karyawan", Name = "EmployeeNumber", Style = "width: 10%; min-width: 100px" });
            ColumnModels.Add(new ColumnModel { Label = "Nama", Name = "Name", Style = "width: 15%; min-width: 150px" });
            ColumnModels.Add(new ColumnModel { Label = "Email", Name = "Email" });
            ColumnModels.Add(new ColumnModel { Label = "Telp", Name = "Phone" });
            ColumnModels.Add(new ColumnModel { Label = "Jenis Kelamin", Name = "Sex" });
            ColumnModels.Add(new ColumnModel { Label = "User Name", Name = "UserName" });
            ColumnModels.Add(new ColumnModel { Label = "Entitas", Name = "Entity" });
            ColumnModels.Add(new ColumnModel { Label = "Posisi", Name = "Position" });
            ColumnModels.Add(new ColumnModel { Label = "Fungsi", Name = "CompanyFunction" });
            ColumnModels.Add(new ColumnModel { Label = "Divisi", Name = "Divition" });
            ColumnModels.Add(new ColumnModel { Label = "Departmen", Name = "Department" });


            ViewData["Columns"] = ColumnModels;
            ViewData["Script"] = "participant-user.js";
            ViewData["ModalStye"] = "modal-xl";

            return View("~/Views/Shared/_Index.cshtml");
        }

        [HttpPost("participant-user/save")]
        public async Task<IActionResult> Save(ParticipantUser participantUser)
        {
            try
            {
                ParticipantUser userFromDb = await _db.ParticipantUsers
                    .Include(x=>x.Entity)
                    .Include(x=>x.User)
                    .FirstOrDefaultAsync(e => e.UserId == participantUser.UserId);

                if (userFromDb == null)
                {
                    var user = new User { UserName = participantUser.User.UserName };
                    var result = await _userManager.CreateAsync(user, participantUser.User.PasswordHash);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "Pengguna Umum");

                        participantUser.Entity = await _db.Entities.FirstOrDefaultAsync(e => e.ID == participantUser.Entity.ID);
                        participantUser.Position = await _db.Position.FirstOrDefaultAsync(e => e.ID == participantUser.Position.ID);
                        participantUser.Divition = await _db.Divitions.FirstOrDefaultAsync(e => e.ID == participantUser.Divition.ID);
                        participantUser.Department = await _db.Departments.FirstOrDefaultAsync(e => e.ID == participantUser.Department.ID);
                        participantUser.CompanyFunction = await _db.CompanyFunctions.FirstOrDefaultAsync(e => e.ID == participantUser.CompanyFunction.ID);

                        participantUser.User = user;
                        _db.ParticipantUsers.Add(participantUser);
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
                    userFromDb.Entity = await _db.Entities.FirstOrDefaultAsync(e => e.ID == participantUser.Entity.ID);
                    userFromDb.Position = await _db.Position.FirstOrDefaultAsync(e => e.ID == participantUser.Position.ID);
                    userFromDb.Divition = await _db.Divitions.FirstOrDefaultAsync(e => e.ID == participantUser.Divition.ID);
                    userFromDb.Department = await _db.Departments.FirstOrDefaultAsync(e => e.ID == participantUser.Department.ID);
                    userFromDb.CompanyFunction = await _db.CompanyFunctions.FirstOrDefaultAsync(e => e.ID == participantUser.CompanyFunction.ID);

                    if (participantUser.User.PasswordHash != null)
                    {
                        var token = await _userManager.GeneratePasswordResetTokenAsync(userFromDb.User);
                        var result = await _userManager.ResetPasswordAsync(userFromDb.User, token, participantUser.User.PasswordHash);
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

                    userFromDb.Name = participantUser.Name;
                    userFromDb.Email = participantUser.Email;
                    userFromDb.Phone = participantUser.Phone;
                    userFromDb.EmployeeNumber = participantUser.EmployeeNumber;

                    _db.ParticipantUsers.Update(userFromDb);
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

        [HttpPost("participant-user/delete")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var userFromDb = await _db.ParticipantUsers
                    .Include(x=>x.User)
                    .FirstOrDefaultAsync(e => e.UserId == id);

                if (userFromDb == null)
                {
                    return Json(new { success = false, message = "Data tidak ditemukan" });
                }

                _db.ParticipantUsers.Remove(userFromDb);
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
