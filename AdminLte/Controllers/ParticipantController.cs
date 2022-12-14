using AdminLte.Data;
using AdminLte.Helpers;
using AdminLte.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AdminLte.Controllers
{
    [Authorize(Roles = "Pengguna Khusus")]
    [CustomAuthFilter("Access_Penjadwalan_PenjadwalanPeserta")]
    public class ParticipantController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ParticipantController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet("participant/table-data-view")]
        public async Task<IActionResult> GetAll(int page = 1, int scheduleID = 0, int finish = 0, string search = "")
        {
            try
            {

                var data = await _db.Participants
                    .Include(x => x.Schedule)
                    .Include(x => x.ParticipantUser)
                    .Include(x => x.ParticipantUser.User)
                    .Include(x => x.ParticipantUser.Entity)
                    .Include(x => x.ParticipantUser.SubEntity)
                    .Include(x => x.ParticipantUser.Position)
                    .Include(x => x.ParticipantUser.CompanyFunction)
                    .Include(x => x.ParticipantUser.Divition)
                    .Include(x => x.ParticipantUser.Department)
                    .Include(x => x.ParticipantUser.JobLevel)
                    .Include(x => x.QuestionPackage)
                    .Include(x => x.QuestionPackage.Assesment)
                    .Where(x =>
                        x.Schedule.ID == scheduleID &&
                        (finish == 1 ? x.FinishedAt != null : (finish == 2 ? x.StartedAt != null && x.FinishedAt == null : (finish == 3 ? x.StartedAt == null && x.FinishedAt == null : true))) &&
                        (
                            EF.Functions.Like(x.ParticipantUser.EmployeeNumber, $"%{search}%") ||
                            EF.Functions.Like(x.ParticipantUser.Name, $"%{search}%") ||
                            EF.Functions.Like(x.ParticipantUser.Entity.Name, $"%{search}%")
                        )
                     )
                    .OrderBy(x => x.ParticipantUser.Name)
                    .Skip((page - 1) * 10)
                    .Take(10)
                    .ToListAsync();

                var rows = new List<RowModel>();
                foreach (var row in data)
                {
                    rows.Add(new RowModel
                    {
                        ID = row.ID,
                        Value = new string[]
                        {
                            "HTML:<b>" + row.ParticipantUser.EmployeeNumber + " - " + row.ParticipantUser.Name + "</b><br/>" +
                            "Holding/ Sub-Holding : " + (row.ParticipantUser.Entity == null ? "-" : row.ParticipantUser.Entity.Name) + "<br/>" +
                            "Direktorat/ Fungsi/ Anak Perusahaan : " + (row.ParticipantUser.SubEntity == null ? "-" : row.ParticipantUser.SubEntity.Name) + "<br/>" +
                            //"Posisi : " + (row.ParticipantUser.Position == null ? "-" : row.ParticipantUser.Position.Name) + "<br/>" +
                            //"Fungsi : " + (row.ParticipantUser.CompanyFunction == null ? "-" : row.ParticipantUser.CompanyFunction.Name) + "<br/>" +
                            //"Divisi : " + (row.ParticipantUser.Divition == null ? "-" : row.ParticipantUser.Divition.Name) + "<br/>" +
                            //"Departemen : " + (row.ParticipantUser.Department == null ? "-" : row.ParticipantUser.Department.Name) + "<br/>" +
                            "Level Jabatan : " + (row.ParticipantUser.JobLevel == null ? "-" : row.ParticipantUser.JobLevel.Name),
                            row.QuestionPackage.Assesment.Name + " - " + row.QuestionPackage.Name,
                            row.IsCanRetake ? "Iya, " + row.MaxRetake + " Kali" : "Tidak",
                            row.StartedAt != null ? "HTML:Mulai : " + row.StartedAt.Value.ToString("yyyy-MM-dd HH:mm") + "<br/>" +
                            (row.FinishedAt != null ? "Selesai : " + row.FinishedAt.Value.ToString("yyyy-MM-dd HH:mm") : "") : ""
                        }
                    });
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

        [HttpGet("participant/download")]
        public async Task<IActionResult> GetDownload(int scheduleID = 0, int finish = 0, string search = "")
        {
            try
            {
                var schedule = await _db.Schedules
                    .Include(x=>x.Period)
                    .Where(x=>x.ID == scheduleID)
                    .FirstAsync();

                var data = await _db.Participants
                    .Include(x => x.Schedule)
                    .Include(x => x.ParticipantUser)
                    .Include(x => x.ParticipantUser.User)
                    .Include(x => x.ParticipantUser.Entity)
                    .Include(x => x.ParticipantUser.SubEntity)
                    .Include(x => x.ParticipantUser.Position)
                    .Include(x => x.ParticipantUser.CompanyFunction)
                    .Include(x => x.ParticipantUser.Divition)
                    .Include(x => x.ParticipantUser.Department)
                    .Include(x => x.ParticipantUser.JobLevel)
                    .Include(x => x.QuestionPackage)
                    .Include(x => x.QuestionPackage.Assesment)
                    .Where(x =>
                        x.Schedule.ID == scheduleID &&
                        (finish == 1 ? x.FinishedAt != null : (finish == 2 ? x.StartedAt != null && x.FinishedAt == null : (finish == 3 ? x.StartedAt == null && x.FinishedAt == null : true))) &&
                        (
                            EF.Functions.ILike(x.ParticipantUser.EmployeeNumber, $"%{search}%") ||
                            EF.Functions.ILike(x.ParticipantUser.Name, $"%{search}%") ||
                            EF.Functions.ILike(x.ParticipantUser.Entity.Name, $"%{search}%")
                        )
                     )
                    .OrderBy(x=>x.ParticipantUser.Entity.ID)
                    .OrderBy(x => x.ParticipantUser.EmployeeNumber)
                    .ToListAsync();

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                var stream = new MemoryStream();
                using (var package = new ExcelPackage(stream))
                {
                    var sheet1 = package.Workbook.Worksheets.Add("Daftar Peserta");
                    switch(finish) {
                        case 0:
                            sheet1.Cells["A1"].Value = "EXPORT EXCEL RESPONDEN";
                            break;
                        case 1:
                            sheet1.Cells["A1"].Value = "EXPORT EXCEL RESPONDEN YANG SUDAH MENGISI";
                            break;
                        case 2:
                            sheet1.Cells["A1"].Value = "EXPORT EXCEL RESPONDEN YANG SEDANG MENGISI";
                            break;
                        case 3:
                            sheet1.Cells["A1"].Value = "EXPORT EXCEL RESPONDEN YANG BELUM MENGISI";
                            break;
                    }

                    sheet1.Cells["A2"].Value = "PERIODE PENGISIAN: " + schedule.Assesment.Name + " - " + schedule.Period.Name;
                    sheet1.Cells["A4"].Value = "No.";
                    sheet1.Cells["B4"].Value = "Nomor Pekerja";
                    sheet1.Cells["C4"].Value = "Nama Pekerja";
                    sheet1.Cells["D4"].Value = "Holding/Subholding";
                    sheet1.Cells["E4"].Value = "Direktorat/Fungsi/Anak Perusahaan/Afiliasi";
                    sheet1.Cells["F4"].Value = "Waktu Mulai";
                    sheet1.Cells["G4"].Value = "Waktu Selesai";
                    sheet1.Cells["H4"].Value = "Status";

                    sheet1.Cells[1, 1, 4, 8].Style.Font.Bold = true;

                    var no = 1;
                    foreach (var row in data)
                    {
                        sheet1.Cells[no + 4, 1].Value = no;
                        sheet1.Cells[no + 4, 2].Value = row.ParticipantUser.EmployeeNumber;
                        sheet1.Cells[no + 4, 3].Value = row.ParticipantUser.Name;
                        sheet1.Cells[no + 4, 4].Value = row.ParticipantUser.Entity == null ? "-" : row.ParticipantUser.Entity.Name;
                        sheet1.Cells[no + 4, 5].Value = row.ParticipantUser.SubEntity == null ? "-" : row.ParticipantUser.SubEntity.Name;
                        sheet1.Cells[no + 4, 6].Value = row.StartedAt == null ? "" : row.StartedAt.Value.ToString("yyyy-MM-dd HH:mm");
                        sheet1.Cells[no + 4, 7].Value = row.FinishedAt == null ? "" : row.FinishedAt.Value.ToString("yyyy-MM-dd HH:mm");
                        sheet1.Cells[no + 4, 8].Value = row.FinishedAt != null ? "Selesai Mengerjakan" : (row.StartedAt != null ? "Proses Mengerjakan" : "Belum Mengerjakan");

                        no++;
                    }

                    package.Save();
                    stream.Position = 0;
                    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Participants-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx"); // this will be the actual export.
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        [HttpGet("participant/table-paging-view")]
        public IActionResult GetPaging(int page = 1, int scheduleID = 0, int finish = 0, string search = "")
        {
            try
            {
                var total = _db.Participants
                    .Where(x =>
                        x.Schedule.ID == scheduleID &&
                        (finish == 1 ? x.FinishedAt != null : (finish == 2 ? x.StartedAt != null && x.FinishedAt == null : (finish == 3 ? x.StartedAt == null && x.FinishedAt == null : true)))
                        &&
                        (
                            EF.Functions.Like(x.ParticipantUser.EmployeeNumber, $"%{search}%") ||
                            EF.Functions.Like(x.ParticipantUser.Name, $"%{search}%") ||
                            EF.Functions.Like(x.ParticipantUser.Entity.Name, $"%{search}%")
                        )
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

        [HttpGet("participant/form-view")]
        public async Task<IActionResult> GetFormAsync(int id = 0, int scheduleID = 0)
        {
            try
            {
                var schedule = await _db.Schedules
                    .Include(x=>x.Participants)
                    .ThenInclude(x=>x.ParticipantUser)
                    .Include(x => x.Period)
                    .FirstOrDefaultAsync(x => x.ID == scheduleID);

                if (schedule == null)
                {
                    return null;
                }

                Participant participantFromDb = null;
                if(id != 0)
                {
                    participantFromDb = await _db.Participants
                        .Include(x=>x.ParticipantUser)
                        .Include(x=>x.QuestionPackage)
                        .FirstOrDefaultAsync(e => e.ID == id);
                }

                var questionPackages = await _db.QuestionPackages
                    .Include(x => x.Assesment)
                    .Include(x => x.QuestionPackagePeriods)
                    .ThenInclude(x => x.Period)
                    .Where(x=>x.QuestionPackagePeriods.Select(x=>x.Period).Contains(schedule.Period))
                    .OrderBy(x => x.Assesment.Name)
                    .ThenBy(x => x.Name)
                    .ToDictionaryAsync(x => x.ID.ToString(), y => y.Assesment.Name + " - " + y.Name);

                var participantExists = schedule.Participants
                    .Where(x=>id == 0 ? true : x.ID != id)
                    .Select(x => x.ParticipantUser.UserId).ToList();
                var participantUsers = await _db.ParticipantUsers
                    .Where(x => !participantExists.Contains(x.UserId))
                    .OrderBy(x=>x.Name)
                    .ToDictionaryAsync(x => x.UserId.ToString(), y => y.Name + " (" + y.EmployeeNumber + ")");

                List<FormModel> FormModels = new List<FormModel>();
               
                FormModels.Add(new FormModel { Label = "ID", Name = "ID", InputType = InputType.HIDDEN, Value = participantFromDb == null ? "0" : participantFromDb.ID.ToString() });
                FormModels.Add(new FormModel { Label = "Peserta", Name = "ParticipantUser", InputType = InputType.DROPDOWN, Options = participantUsers, Value = participantFromDb == null ? "" : participantFromDb.ParticipantUser.UserId.ToString(), IsRequired = true });
                FormModels.Add(new FormModel { Label = "Survei", Name = "QuestionPackage", InputType = InputType.DROPDOWN, Options = questionPackages, Value = participantFromDb == null ? "" : participantFromDb.QuestionPackage.ID.ToString(), IsRequired = true });
                FormModels.Add(new FormModel { Label = "Dapat Mengulang", Name = "IsCanRetake", InputType = InputType.YESNO, Value = participantFromDb == null ? "" : participantFromDb.IsCanRetake ? "1" : "0" });
                FormModels.Add(new FormModel { Label = "Maksimal Mengulang", Name = "MaxRetake", InputType = InputType.NUMBER, Value = participantFromDb == null || !participantFromDb.IsCanRetake ? "0" : participantFromDb.MaxRetake.ToString() });

                ViewData["Forms"] = FormModels;

                return PartialView("~/Views/Shared/_FormView.cshtml");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        [HttpGet("participant")]
        [Route("participant/{scheduleID:int}")]
        public async Task<IActionResult> IndexAsync(int scheduleID, int finish = 0)
        {
            var schedule = await _db.Schedules.FirstOrDefaultAsync(x => x.ID == scheduleID);

            if (schedule == null)
            {
                return Redirect("~/home/errors/404");
            }
            ViewData["Title"] = "Daftar Peserta | " + schedule.Name;
            if (finish == 1)
            {
                ViewData["Title"] = "Daftar Peserta Selesai | " + schedule.Name;
            }
            else if (finish == 2)
            {
                ViewData["Title"] = "Daftar Peserta Belum Selesai | " + schedule.Name;
            }
            else if (finish == 3)
            {
                ViewData["Title"] = "Daftar Peserta Belum Mengerjakan | " + schedule.Name;
            }

            ViewData["Title"] = "Daftar Peserta " + (finish == 1 ? "Selesai" : "") + " | " + schedule.Name;

            List<ColumnModel> ColumnModels = new List<ColumnModel>();
            ColumnModels.Add(new ColumnModel { Label = "Peserta", Name = "ParticipantUser", Style = "width: 35%; min-width: 350px" });
            ColumnModels.Add(new ColumnModel { Label = "Survei", Name = "QuestionPackage" });
            ColumnModels.Add(new ColumnModel { Label = "Dapat Mengulang", Name = "RetakeInfo" });
            ColumnModels.Add(new ColumnModel { Label = "Info Pengerjaan", Name = "Info" }); 

            ViewData["Columns"] = ColumnModels;
            ViewData["Script"] = "participant.js";
            ViewData["Values"] = new Dictionary<string, string>()
            {
                {"Schedule", scheduleID.ToString()},
                {"Finish", finish.ToString()}
            };
            ViewData["BreadCrump"] = new Dictionary<string, string>()
            {
                {"Penjadwalan Peserta", "schedule"}
            };

            return View("~/Views/Shared/_Index.cshtml");
        }

        [HttpPost("participant/save")]
        public async Task<IActionResult> Save(Participant participant)
        {
            try
            {
                Participant participantFromDb = await _db.Participants.FirstOrDefaultAsync(e => e.ID == participant.ID);

                if (participantFromDb == null)
                {
                    participant.ParticipantUser = await _db.ParticipantUsers.FirstOrDefaultAsync(e => e.UserId == participant.ParticipantUser.UserId);
                    participant.QuestionPackage = await _db.QuestionPackages.FirstOrDefaultAsync(e => e.ID == participant.QuestionPackage.ID);
                    participant.Schedule = await _db.Schedules.FirstOrDefaultAsync(e => e.ID == participant.Schedule.ID);
                    participant.StartedAt = null;
                    participant.FinishedAt = null;

                    _db.Participants.Add(participant);
                    _db.SaveChanges();

                    return Json(new { success = true, message = "Data berhasil disimpan" });
                }
                else
                {
                    participantFromDb.ParticipantUser = await _db.ParticipantUsers.FirstOrDefaultAsync(e => e.UserId == participant.ParticipantUser.UserId);
                    participantFromDb.QuestionPackage = await _db.QuestionPackages.FirstOrDefaultAsync(e => e.ID == participant.QuestionPackage.ID);
                    participantFromDb.Schedule = await _db.Schedules.FirstOrDefaultAsync(e => e.ID == participant.Schedule.ID);
                    participantFromDb.IsCanRetake = participant.IsCanRetake;
                    participantFromDb.MaxRetake = participant.MaxRetake;

                    _db.Participants.Update(participantFromDb);
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

        [HttpPost("participant/delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var participantFromDb = await _db.Participants.FirstOrDefaultAsync(e => e.ID == id);

                if (participantFromDb == null)
                {
                    return Json(new { success = false, message = "Data tidak ditemukan" });
                }

                _db.Participants.Remove(participantFromDb);
                await _db.SaveChangesAsync();
                return Json(new { success = true, message = "Data berhasil dihapus" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new { success = false, message = "Terjadi kesalahan. Err : " + ex.Message });
            }
        }

        public async Task<IActionResult> ReportAsync(int id)
        {
            var participantAnswerSheet = _db.ParticipantAnswerSheets
                .Where(x => x.Participant.ID == id && x.IsFinish)
                .OrderByDescending(x=>x.FinishedAt)
                .LastOrDefault();

            if(participantAnswerSheet == null)
            {
                return View();
            }

            var answers = await _db.ParticipantAnswerSheetLines
                .Include(x=>x.Question)
                .ThenInclude(x=>x.Section)
                .Where(x => x.ParticipantAnswerSheet.ID == participantAnswerSheet.ID)
                .ToListAsync();

            var sections = answers.Select(x => x.Question.Section);
            foreach(Section section in sections)
            {
            }

            return View();
        }
    }
}
