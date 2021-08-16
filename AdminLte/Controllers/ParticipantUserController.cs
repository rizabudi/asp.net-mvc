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
    [CustomAuthFilter("Access_PengaturanPengguna_PenggunaUmum")]
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
        public async Task<IActionResult> GetAll(int page = 1, string search = "", string sort = "", string order = "")
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

                var temps = _db.ParticipantUsers
                    .Include(x => x.Entity)
                    .Include(x => x.SubEntity)
                    .Include(x => x.Divition)
                    .Include(x => x.Department)
                    .Include(x => x.CompanyFunction)
                    .Include(x => x.Position)
                    .Include(x => x.JobLevel)
                    .Include(x=> x.User)
                    .Where(x=> (EF.Functions.ILike(x.EmployeeNumber, $"%{search}%") ||
                            EF.Functions.ILike(x.Name, $"%{search}%") ||
                            EF.Functions.ILike(x.Email, $"%{search}%") ||
                            EF.Functions.ILike(x.Phone, $"%{search}%")) && (entityID == 0 ? true : x.Entity.ID == entityID)
                     );

                IOrderedQueryable<ParticipantUser> temps2 = null;
                if (order == null || order == "" || order == "asc")
                {
                    if(sort == null || sort == "")
                    {
                        temps2 = temps.OrderBy(x => x.Name);
                    }
                    else if(sort == "EmployeeNumber")
                    {
                        temps2 = temps.OrderBy(x => x.EmployeeNumber);
                    }
                    else if (sort == "Name")
                    {
                        temps2 = temps.OrderBy(x => x.Name);
                    }
                    else if (sort == "Email")
                    {
                        temps2 = temps.OrderBy(x => x.Email);
                    }
                    else if (sort == "Phone")
                    {
                        temps2 = temps.OrderBy(x => x.Phone);
                    }
                    else if (sort == "Email")
                    {
                        temps2 = temps.OrderBy(x => x.Email);
                    }
                    else if (sort == "Sex")
                    {
                        temps2 = temps.OrderBy(x => x.Sex);
                    }
                    else if (sort == "UserName")
                    {
                        temps2 = temps.OrderBy(x => x.User.UserName);
                    }
                    else if (sort == "Entity")
                    {
                        temps2 = temps.OrderBy(x => x.Entity == null ? "-" : x.Entity.Name);
                    }
                    else if (sort == "JobLevel")
                    {
                        temps2 = temps.OrderBy(x => x.JobLevel == null ? "-" : x.JobLevel.Name);
                    }
                } 
                else
                {
                    if (sort == null || sort == "")
                    {
                        temps2 = temps.OrderByDescending(x => x.Name);
                    }
                    else if (sort == "EmployeeNumber")
                    {
                        temps2 = temps.OrderByDescending(x => x.EmployeeNumber);
                    }
                    else if (sort == "Name")
                    {
                        temps2 = temps.OrderByDescending(x => x.Name);
                    }
                    else if (sort == "Email")
                    {
                        temps2 = temps.OrderByDescending(x => x.Email);
                    }
                    else if (sort == "Phone")
                    {
                        temps2 = temps.OrderByDescending(x => x.Phone);
                    }
                    else if (sort == "Email")
                    {
                        temps2 = temps.OrderByDescending(x => x.Email);
                    }
                    else if (sort == "Sex")
                    {
                        temps2 = temps.OrderByDescending(x => x.Sex);
                    }
                    else if (sort == "UserName")
                    {
                        temps2 = temps.OrderByDescending(x => x.User.UserName);
                    }
                    else if (sort == "Entity")
                    {
                        temps2 = temps.OrderByDescending(x => x.Entity == null ? "-" : x.Entity.Name);
                    }
                    else if (sort == "JobLevel")
                    {
                        temps2 = temps.OrderByDescending(x => x.JobLevel == null ? "-" : x.JobLevel.Name);
                    }
                }

                var data = await temps2.Skip((page - 1) * 10)
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
                                row.Sex == -1 ? "" : (row.Sex == 1 ? "Laki-laki" : "Perempuan"),
                                row.User.UserName,
                                row.Entity == null ? "-" : row.Entity.Name,
                                row.SubEntity == null ? "-" : row.SubEntity.Name,
                                //row.Position == null ? "-" : row.Position.Name,
                                //row.CompanyFunction == null ? "-" : row.CompanyFunction.Name,
                                //row.Divition == null ? "-" : row.Divition.Name,
                                //row.Department == null ? "-" :  row.Department.Name,
                                row.JobLevel == null ? "-" :  row.JobLevel.Name
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
        public IActionResult GetPaging(int page = 1, string search = "")
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

                var total = _db.ParticipantUsers
                    .Where(x => (EF.Functions.ILike(x.EmployeeNumber, $"%{search}%") ||
                            EF.Functions.ILike(x.Name, $"%{search}%") ||
                            EF.Functions.ILike(x.Email, $"%{search}%") ||
                            EF.Functions.ILike(x.Phone, $"%{search}%")) && (entityID == 0 ? true : x.Entity.ID == entityID)
                     ).Count();
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
                int entityID = 0;
                byte[] bytes;
                if (HttpContext.Session.TryGetValue("User_Entity", out bytes))
                {
                    string value = Encoding.ASCII.GetString(bytes);
                    int.TryParse(value, out entityID);
                }

                var entityList = await _db.Entities
                    .Where(x => x.Level <= 1)
                    .OrderBy(x => x.Name)
                    .ToListAsync();
                var subEntities = new Dictionary<string, string>();

                ParticipantUser userFromDb = null;
                if(id != "")
                {
                    userFromDb = await _db.ParticipantUsers
                        .Include(x => x.Entity)
                        .Include(x => x.SubEntity)
                        .Include(x => x.Divition)
                        .Include(x => x.Department)
                        .Include(x => x.CompanyFunction)
                        .Include(x => x.Position)
                        .Include(x => x.JobLevel)
                        .Include(x => x.User)
                        .FirstOrDefaultAsync(e => e.UserId == id);

                    var subEntityList = await _db.Entities
                        .Where(x => x.ParentEntity.ID == userFromDb.Entity.ID)
                        .OrderBy(x => x.Name)
                        .ToListAsync();
                    subEntities = Entity.getEntities(subEntityList, userFromDb.Entity.ID, userFromDb.Entity.Level + 1);
                } 
                else
                {
                    if (entityID != 0)
                    {
                        entityList = await _db.Entities.Where(x => x.ID == entityID).ToListAsync();
                    }
                }

                var entities = Entity.getEntities(entityList, 0, 0);

                var positions = await _db.Position.OrderBy(x => x.Name).ToDictionaryAsync(x => x.ID.ToString(), y => y.Name);
                var departments = await _db.Departments.OrderBy(x => x.Name).ToDictionaryAsync(x => x.ID.ToString(), y => y.Name);
                var divitions = await _db.Divitions.OrderBy(x => x.Name).ToDictionaryAsync(x => x.ID.ToString(), y => y.Name);
                var functions = await _db.CompanyFunctions.OrderBy(x => x.Name).ToDictionaryAsync(x => x.ID.ToString(), y => y.Name);
                var jobLevels = await _db.JobLevels.OrderBy(x => x.Level).ToDictionaryAsync(x => x.ID.ToString(), y => y.Name);

                List<FormModel> FormModels = new List<FormModel>();
                FormModels.Add(new FormModel { Label = "UserId", Name = "UserId", InputType = InputType.HIDDEN, Value = userFromDb == null ? "" : userFromDb.UserId });
                FormModels.Add(new FormModel { Label = "No Pekerja", Name = "EmployeeNumber", InputType = InputType.TEXT, Value = userFromDb == null ? "" : userFromDb.EmployeeNumber });
                FormModels.Add(new FormModel { Label = "Nama", Name = "Name", InputType = InputType.TEXT, Value = userFromDb == null ? "" : userFromDb.Name, IsRequired = true });
                FormModels.Add(new FormModel { Label = "Email", Name = "Email", InputType = InputType.EMAIL, Value = userFromDb == null ? "" : userFromDb.Email });
                FormModels.Add(new FormModel { Label = "Telp", Name = "Phone", InputType = InputType.TEXT, Value = userFromDb == null ? "" : userFromDb.Phone });
                
                FormModels.Add(new FormModel { Label = "Holding/ Sub-Holding", Name = "Entity", InputType = InputType.DROPDOWN, Options = entities, Value = userFromDb == null || userFromDb.Entity == null ? (entityID == 0 ? "" : entityID.ToString()) : userFromDb.Entity.ID.ToString(), IsRequired = false, FormPosition = FormPosition.RIGHT, IsDisable = entityID != 0 });
                FormModels.Add(new FormModel { Label = "Direktorat/ Fungsi/ Anak Perusahaan", Name = "SubEntity", InputType = InputType.DROPDOWN, Options = subEntities, Value = userFromDb == null || userFromDb.SubEntity == null ? "" : userFromDb.SubEntity.ID.ToString(), IsRequired = true, FormPosition = FormPosition.RIGHT });
                //FormModels.Add(new FormModel { Label = "Posisi", Name = "Position", InputType = InputType.DROPDOWN, Options = positions, Value = userFromDb == null || userFromDb.Position == null ? "" : userFromDb.Position.ID.ToString(), IsRequired = false, FormPosition = FormPosition.RIGHT });
                //FormModels.Add(new FormModel { Label = "Fungsi", Name = "CompanyFunction", InputType = InputType.DROPDOWN, Options = functions, Value = userFromDb == null || userFromDb.CompanyFunction == null ? "" : userFromDb.CompanyFunction.ID.ToString(), IsRequired = false, FormPosition = FormPosition.RIGHT });
                //FormModels.Add(new FormModel { Label = "Divisi", Name = "Divition", InputType = InputType.DROPDOWN, Options = divitions, Value = userFromDb == null || userFromDb.Divition == null ? "" : userFromDb.Divition.ID.ToString(), IsRequired = false, FormPosition = FormPosition.RIGHT });
                //FormModels.Add(new FormModel { Label = "Departemen", Name = "Department", InputType = InputType.DROPDOWN, Options = departments, Value = userFromDb == null || userFromDb.Department == null ? "" : userFromDb.Department.ID.ToString(), IsRequired = false, FormPosition = FormPosition.RIGHT });
                FormModels.Add(new FormModel { Label = "Level Jabatan", Name = "JobLevel", InputType = InputType.DROPDOWN, Options = jobLevels, Value = userFromDb == null || userFromDb.JobLevel == null ? "" : userFromDb.JobLevel.ID.ToString(), IsRequired = false, FormPosition = FormPosition.RIGHT });

                FormModels.Add(new FormModel { Label = "User Name", Name = "UserName", InputType = InputType.TEXT, Value = userFromDb == null ? "" : userFromDb.User.UserName, IsRequired = true });
                FormModels.Add(new FormModel { Label = "Password", Name = "Password", Note = (userFromDb != null ? "Kosongkan jika tidak ingin mengganti password" : ""), InputType = InputType.PASSWORD, Value = "", IsRequired = id == "" });

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
            ColumnModels.Add(new ColumnModel { Label = "No Pekerja", Name = "EmployeeNumber", Style = "width: 10%; min-width: 100px" });
            ColumnModels.Add(new ColumnModel { Label = "Nama", Name = "Name", Style = "width: 15%; min-width: 150px" });
            ColumnModels.Add(new ColumnModel { Label = "Email", Name = "Email" });
            ColumnModels.Add(new ColumnModel { Label = "Telp", Name = "Phone" });
            ColumnModels.Add(new ColumnModel { Label = "Jenis Kelamin", Name = "Sex" });
            ColumnModels.Add(new ColumnModel { Label = "User Name", Name = "UserName" });
            ColumnModels.Add(new ColumnModel { Label = "Holding/ Sub-Holding", Name = "Entity" });
            ColumnModels.Add(new ColumnModel { Label = "Direktorat/ Fungsi/ Anak Perusahaan", Name = "SubEntity" });
            //ColumnModels.Add(new ColumnModel { Label = "Posisi", Name = "Position" });
            //ColumnModels.Add(new ColumnModel { Label = "Fungsi", Name = "CompanyFunction" });
            //ColumnModels.Add(new ColumnModel { Label = "Divisi", Name = "Divition" });
            //ColumnModels.Add(new ColumnModel { Label = "Departmen", Name = "Department" });
            ColumnModels.Add(new ColumnModel { Label = "Level Jabatan", Name = "JobLevel" });


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
                    .Include(x => x.Entity)
                    .Include(x => x.SubEntity)
                    .Include(x => x.Position)
                    .Include(x => x.Divition)
                    .Include(x => x.Department)
                    .Include(x => x.CompanyFunction)
                    .Include(x => x.JobLevel)
                    .FirstOrDefaultAsync(e => e.UserId == participantUser.UserId);

                if (userFromDb == null)
                {
                    var user = new User { UserName = participantUser.User.UserName };
                    var result = await _userManager.CreateAsync(user, participantUser.User.PasswordHash);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "Pengguna Umum");
                        if (participantUser.Entity != null && participantUser.Entity.ID != -1)
                        {
                            participantUser.Entity = await _db.Entities.FirstOrDefaultAsync(e => e.ID == participantUser.Entity.ID);
                        }
                        else
                        {
                            participantUser.Entity = null;
                        }
                        if (participantUser.SubEntity != null && participantUser.SubEntity.ID != -1)
                        {
                            participantUser.SubEntity = await _db.Entities.FirstOrDefaultAsync(e => e.ID == participantUser.SubEntity.ID);
                        }
                        else
                        {
                            participantUser.SubEntity = null;
                        }
                        if (participantUser.Position != null && participantUser.Position.ID != -1)
                        {
                            participantUser.Position = await _db.Position.FirstOrDefaultAsync(e => e.ID == participantUser.Position.ID);
                        }
                        else
                        {
                            participantUser.Position = null;
                        }
                        if (participantUser.Divition != null && participantUser.Divition.ID != -1)
                        {
                            participantUser.Divition = await _db.Divitions.FirstOrDefaultAsync(e => e.ID == participantUser.Divition.ID);
                        }
                        else
                        {
                            participantUser.Divition = null;
                        }
                        if (participantUser.Department != null && participantUser.Department.ID != -1)
                        {
                            participantUser.Department = await _db.Departments.FirstOrDefaultAsync(e => e.ID == participantUser.Department.ID);
                        }
                        else
                        {
                            participantUser.Department = null;
                        }
                        if (participantUser.CompanyFunction != null && participantUser.CompanyFunction.ID != -1)
                        {
                            participantUser.CompanyFunction = await _db.CompanyFunctions.FirstOrDefaultAsync(e => e.ID == participantUser.CompanyFunction.ID);
                        }
                        else
                        {
                            participantUser.CompanyFunction = null;
                        }
                        if (participantUser.JobLevel != null && participantUser.JobLevel.ID != -1)
                        {
                            participantUser.JobLevel = await _db.JobLevels.FirstOrDefaultAsync(e => e.ID == participantUser.JobLevel.ID);
                        }
                        else
                        {
                            participantUser.JobLevel = null;
                        }

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
                    if (participantUser.Entity != null && participantUser.Entity.ID != -1)
                    {
                        userFromDb.Entity = await _db.Entities.FirstOrDefaultAsync(e => e.ID == participantUser.Entity.ID);
                    }
                    else
                    {
                        userFromDb.Entity = null;
                    }
                    if (participantUser.SubEntity != null && participantUser.SubEntity.ID != -1)
                    {
                        userFromDb.SubEntity = await _db.Entities.FirstOrDefaultAsync(e => e.ID == participantUser.SubEntity.ID);
                    }
                    else
                    {
                        userFromDb.SubEntity = null;
                    }
                    if (participantUser.Position != null && participantUser.Position.ID != -1)
                    {
                        userFromDb.Position = await _db.Position.FirstOrDefaultAsync(e => e.ID == participantUser.Position.ID);
                    }
                    else
                    {
                        userFromDb.Position = null;
                    }
                    if (participantUser.Divition != null && participantUser.Divition.ID != -1)
                    {
                        userFromDb.Divition = await _db.Divitions.FirstOrDefaultAsync(e => e.ID == participantUser.Divition.ID);
                    }
                    else
                    {
                        userFromDb.Divition = null;
                    }
                    if (participantUser.Department != null && participantUser.Department.ID != -1)
                    {
                        userFromDb.Department = await _db.Departments.FirstOrDefaultAsync(e => e.ID == participantUser.Department.ID);
                    }
                    else
                    {
                        userFromDb.Department = null;
                    }
                    if (participantUser.CompanyFunction != null && participantUser.CompanyFunction.ID != -1)
                    {
                        userFromDb.CompanyFunction = await _db.CompanyFunctions.FirstOrDefaultAsync(e => e.ID == participantUser.CompanyFunction.ID);
                    }
                    else
                    {
                        userFromDb.CompanyFunction = null;
                    }
                    if (participantUser.JobLevel != null && participantUser.JobLevel.ID != -1)
                    {
                        userFromDb.JobLevel = await _db.JobLevels.FirstOrDefaultAsync(e => e.ID == participantUser.JobLevel.ID);
                    }
                    else
                    {
                        userFromDb.JobLevel = null;
                    }

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

        [AllowAnonymous]
        [HttpPost("participant-user/reset-password")]
        public async Task<IActionResult> ResetPassword(string userId = "")
        {
            var usersFromDb = await _db.Users
                    //.Include(x => x.Entity)
                    //.Include(x => x.Position)
                    //.Include(x => x.Divition)
                    //.Include(x => x.Department)
                    //.Include(x => x.CompanyFunction)
                    //.Include(x => x.JobLevel)
                    //.Include(x => x.User)
                    .Where(x => (userId == "" ? true : x.Id == userId) && x.PasswordHash == null)
                    .ToListAsync();

            var errors = new List<string>();
            var msg = "";
            foreach (User user in usersFromDb)
            {
                if(user.SecurityStamp == null)
                {
                    user.SecurityStamp = Guid.NewGuid().ToString();
                }
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, user.UserName + "@Pertamina");
                if (result.Succeeded)
                {
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        msg += error.Description + "<br/>";
                    }
                    errors.Add(user.UserName + " : " + msg);
                }
            }

            if(errors.Count > 0)
            {
                return Json(new { success = false, message = "Terjadi kesalahan. Err : " + msg });
            }
            else
            {
                return Json(new { success = false, message = "Berhasil" });
            }
        }
    }
}
