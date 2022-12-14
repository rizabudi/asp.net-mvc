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
    [Authorize(Roles = "Pengguna Umum")]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<User> _userManager;
        public ProfileController(ApplicationDbContext db, UserManager<User> user)
        {
            _db = db;
            _userManager = user;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> IndexAsync(bool isEdit = false)
        {
            var user = await _userManager.GetUserAsync(User);
            var participantUser = await _db.ParticipantUsers
                .Include(x=>x.Entity)
                .Include(x => x.SubEntity)
                .Include(x => x.Position)
                .Include(x => x.CompanyFunction)
                .Include(x => x.Divition)
                .Include(x => x.Department)
                .Include(x => x.JobLevel)
                .FirstOrDefaultAsync(x => x.UserId == user.Id);

            if(participantUser == null)
            {
                return Redirect("~/home/errors/404");
            }

            ViewData["IsEdit"] = isEdit;
            return View(participantUser);
        }

        [HttpGet("profile/form-view")]
        public async Task<IActionResult> GetFormAsync()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                ParticipantUser participantUserFromDb = null;
                if (user != null)
                {
                    participantUserFromDb = await _db.ParticipantUsers
                        .Include(x => x.Entity)
                        .Include(x => x.SubEntity)
                        .Include(x => x.Position)
                        .Include(x => x.CompanyFunction)
                        .Include(x => x.Divition)
                        .Include(x => x.Department)
                        .Include(x => x.JobLevel)
                        .FirstOrDefaultAsync(x => x.UserId == user.Id);
                }
                List<FormModel> FormModels = new List<FormModel>();

                var entityList = await _db.Entities
                    .Where(x => x.Level <= 1)
                    .OrderBy(x => x.Name)
                    .ToListAsync();
                var entities = Entity.getEntities(entityList, 0, 0, true);
                var subEntities = new Dictionary<string, string>();
                if (participantUserFromDb != null)
                {
                    var subEntityList = await _db.Entities
                        .Where(x => x.ParentEntity.ID == participantUserFromDb.Entity.ID)
                        .OrderBy(x => x.Name)
                        .ToListAsync();
                    subEntities = Entity.getEntities(subEntityList, participantUserFromDb.Entity.ID, participantUserFromDb.Entity.Level + 1, true);
                }

                var positions = await _db.Position.OrderBy(x => x.Name).ToDictionaryAsync(x => x.ID.ToString(), y => y.Name);
                var departments = await _db.Departments.OrderBy(x => x.Name).ToDictionaryAsync(x => x.ID.ToString(), y => y.Name);
                var divitions = await _db.Divitions.OrderBy(x => x.Name).ToDictionaryAsync(x => x.ID.ToString(), y => y.Name);
                var functions = await _db.CompanyFunctions.OrderBy(x => x.Name).ToDictionaryAsync(x => x.ID.ToString(), y => y.Name);
                var jobLevels = await _db.JobLevels.OrderBy(x => x.Level).ToDictionaryAsync(x => x.ID.ToString(), y => y.Name);
                var sexs = new Dictionary<string, string> {
                    {"1", "Laki-laki"},
                    {"0", "Perempuan"},
                };

                FormModels.Add(new FormModel { Label = "UserId", Name = "UserId", InputType = InputType.HIDDEN, Value = participantUserFromDb == null ? "" : user.Id.ToString() });
                FormModels.Add(new FormModel { Label = "No Pekerja", Name = "EmployeeNumber", InputType = InputType.TEXT, Value = participantUserFromDb == null ? "" : participantUserFromDb.EmployeeNumber, IsRequired = true, IsDisable = true });
                FormModels.Add(new FormModel { Label = "Nama", Name = "Name", InputType = InputType.TEXT, Value = participantUserFromDb == null ? "" : participantUserFromDb.Name, IsRequired = true });
                FormModels.Add(new FormModel { Label = "Email", Name = "Email", Note = "dihimbau menggunakan email @pertamina.com", InputType = InputType.EMAIL, Value = participantUserFromDb == null ? "" : participantUserFromDb.Email, IsRequired = true });
                //FormModels.Add(new FormModel { Label = "No Telp", Name = "Phone", InputType = InputType.TEXT, Value = participantUserFromDb == null ? "" : participantUserFromDb.Phone, IsRequired = true });
                FormModels.Add(new FormModel { Label = "Jenis Kelamin", Name = "Sex", InputType = InputType.DROPDOWN, Options = sexs, Value = participantUserFromDb == null ? "" : participantUserFromDb.Sex.ToString(), IsRequired = true });
                //FormModels.Add(new FormModel { Label = "Tanggal Lahir", Name = "BirthDate", InputType = InputType.DATE, Value = participantUserFromDb == null || participantUserFromDb.BirthDate == null ? "" : participantUserFromDb.BirthDate.Value.ToString("yyyy-MM-dd"), IsRequired = true });
                FormModels.Add(new FormModel { Label = "Usia", Name = "Age", Note = "tahun", InputType = InputType.NUMBER_POSITIVE, Value = participantUserFromDb == null || participantUserFromDb.Age == null ? "" : participantUserFromDb.Age.Value.ToString(), IsRequired = true });
                FormModels.Add(new FormModel { Label = "Masa kerja di PT. Pertamina", Name = "WorkDuration", Note = "pembulatan ke atas dalam tahun", InputType = InputType.NUMBER_POSITIVE, Value = participantUserFromDb == null || participantUserFromDb.WorkDuration == null ? "" : participantUserFromDb.WorkDuration.Value.ToString(), IsRequired = true });

                FormModels.Add(new FormModel { Label = "Holding/ Sub-Holding", Name = "Entity", InputType = InputType.DROPDOWN, Options = entities, Value = participantUserFromDb == null || participantUserFromDb.Entity == null ? "" : participantUserFromDb.Entity.ID.ToString(), IsRequired = true, FormPosition = FormPosition.RIGHT });
                FormModels.Add(new FormModel { Label = "Direktorat/ Fungsi/ Anak Perusahaan/ Afiliasi", Name = "SubEntity", InputType = InputType.DROPDOWN, Options = subEntities, Value = participantUserFromDb == null || participantUserFromDb.SubEntity == null ? "" : participantUserFromDb.SubEntity.ID.ToString(), IsRequired = true, FormPosition = FormPosition.RIGHT });
                //FormModels.Add(new FormModel { Label = "Posisi", Name = "Position", InputType = InputType.DROPDOWN, Options = positions, Value = participantUserFromDb == null || participantUserFromDb.Position == null ? "" : participantUserFromDb.Position.ID.ToString(), IsRequired = false, FormPosition = FormPosition.RIGHT });
                //FormModels.Add(new FormModel { Label = "Fungsi", Name = "CompanyFunction", InputType = InputType.DROPDOWN, Options = functions, Value = participantUserFromDb == null || participantUserFromDb.CompanyFunction == null ? "" : participantUserFromDb.CompanyFunction.ID.ToString(), IsRequired = false, FormPosition = FormPosition.RIGHT });
                //FormModels.Add(new FormModel { Label = "Divisi", Name = "Divition", InputType = InputType.DROPDOWN, Options = divitions, Value = participantUserFromDb == null || participantUserFromDb.Divition == null ? "" : participantUserFromDb.Divition.ID.ToString(), IsRequired = false, FormPosition = FormPosition.RIGHT });
                //FormModels.Add(new FormModel { Label = "Departemen", Name = "Department", InputType = InputType.DROPDOWN, Options = departments, Value = participantUserFromDb == null || participantUserFromDb.Department == null ? "" : participantUserFromDb.Department.ID.ToString(), IsRequired = false, FormPosition = FormPosition.RIGHT });
                FormModels.Add(new FormModel { Label = "Level Jabatan", Name = "JobLevel", InputType = InputType.DROPDOWN, Options = jobLevels, Value = participantUserFromDb == null || participantUserFromDb.JobLevel == null ? "" : participantUserFromDb.JobLevel.ID.ToString(), IsRequired = true, FormPosition = FormPosition.RIGHT });

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

        [HttpPost("profile/save")]
        public async Task<IActionResult> Save(ParticipantUser participantUser)
        {
            try
            {
                ParticipantUser userFromDb = await _db.ParticipantUsers
                    .Include(x => x.Entity)
                    .Include(x => x.Position)
                    .Include(x => x.Divition)
                    .Include(x => x.Department)
                    .Include(x => x.CompanyFunction)
                    .Include(x => x.JobLevel)
                    .FirstOrDefaultAsync(e => e.UserId == participantUser.UserId);

                userFromDb.Entity = await _db.Entities.FirstOrDefaultAsync(e => e.ID == participantUser.Entity.ID);
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

                userFromDb.Name = participantUser.Name;
                userFromDb.Email = participantUser.Email;
                userFromDb.Phone = participantUser.Phone;
                userFromDb.BirthDate = participantUser.BirthDate;
                userFromDb.Age = participantUser.Age;
                userFromDb.WorkDuration = participantUser.WorkDuration;
                userFromDb.Sex = participantUser.Sex;

                _db.ParticipantUsers.Update(userFromDb);
                _db.SaveChanges();

                return Json(new { success = true, message = "Profil berhasil disimpan" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new { success = false, message = "Terjadi kesalahan. Err : " + ex.Message });
            }
        }

        [HttpGet("profile/entity-select-view")]
        public async Task<IActionResult> GetForSelects(int entity = 0, int selectedID = 0)
        {
            try
            {
                var entityList = await _db.Entities.Where(x => x.ParentEntity.ID == entity)
                    .OrderBy(x => x.Name)
                    .ToListAsync();
                var entities = Entity.getEntities(entityList, 0, 0, true);
                var data = entities.ToList();

                var rows = new List<RowModel>();
                foreach (var row in data)
                {
                    rows.Add(new RowModel { ID = int.Parse(row.Key), Value = new string[] { row.Value } });
                }

                ViewData["Rows"] = rows;
                ViewData["SelectedID"] = selectedID;

                return PartialView("~/Views/Shared/_Select.cshtml");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
    }
}
