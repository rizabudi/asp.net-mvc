using AdminLte.Data;
using AdminLte.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminLte.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly PostgreDbContext _db;
        public ScheduleController(PostgreDbContext db)
        {
            _db = db;
        }

        [HttpGet("schedule/table-data-view")]
        public async Task<IActionResult> GetAll(int page = 1)
        {
            try
            {
                var data = await _db.Schedules
                    .Include(x=>x.Entity)
                    .Include(x=>x.Assesment)
                    .Include(x=>x.Period)
                    .Include(x=>x.SubPeriod)
                    .Include(x => x.Participants)
                    .OrderBy(x=>x.Name)
                    .Skip((page-1)*10)
                    .Take(10)
                    .ToListAsync();

                var rows = new List<RowModel>();
                foreach(var row in data)
                {
                    var participants = row.Participants.Count();
                    var participantsFinish = row.Participants.Where(x=>x.FinishedAt != null).Count();
                    var participantsUnFinish = row.Participants.Where(x => x.StartedAt != null && x.FinishedAt == null).Count();
                    rows.Add(new RowModel { 
                        ID = row.ID, 
                        Value = new string[] { 
                            row.Name,
                            row.Entity.Name,
                            row.Assesment.Name,
                            "HTML:Periode : " + row.Period.Name +  " (" + row.Period.Start.ToString("yyyy-MM-dd") + " s/d " + row.Period.End.ToString("yyyy-MM-dd") + ")" +
                            (row.SubPeriod != null ? "<br/>Sub Periode : " + row.SubPeriod.Name +  " (" + row.SubPeriod.Start.ToString("yyyy-MM-dd") + " s/d " + row.SubPeriod.End.ToString("yyyy-MM-dd") + ")" : ""),
                            row.Start.ToString("yyyy-MM-dd") + " s/d " + row.End.ToString("yyyy-MM-dd"),
                            "HTML:<a href='/participant/" + row.ID + "'>" + participants + " Peserta</a>",
                            "HTML:<a href='/participant/" + row.ID + "?finish=1'>" + participantsFinish + " Peserta</a>",
                            "HTML:<a href='/participant/" + row.ID + "?finish=2'>" + participantsUnFinish + " Peserta</a>"
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

        [HttpGet("schedule/table-paging-view")]
        public IActionResult GetPaging(int page = 1)
        {
            try
            {
                var total = _db.Schedules.Count();
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

        [HttpGet("schedule/form-view")]
        public async Task<IActionResult> GetFormAsync(int id = 0)
        {
            try
            {
                Schedule scheduleFromDb = null;
                if(id != 0)
                {
                    scheduleFromDb = await _db.Schedules.FirstOrDefaultAsync(e => e.ID == id);
                }
                var assesments = await _db.Assesments.OrderBy(x => x.Name).ToDictionaryAsync(x => x.ID.ToString(), y => y.Name);
                var entities = await _db.Entities.OrderBy(x => x.Name).ToDictionaryAsync(x => x.ID.ToString(), y => y.Name);
                var periods = await _db.Periods
                    .OrderBy(x => x.Start)
                    .ToDictionaryAsync(x => x.ID.ToString(), y => y.Name + " (" + y.Start.ToString("yyyy-MM-dd") + " s/d " + y.End.ToString("yyyy-MM-dd") + ")");
                var subPeriods = await _db.SubPeriods
                    .Include(x => x.Period)
                    .Where(x => (id == 0 ? false : x.Period == scheduleFromDb.Period))
                    .OrderBy(x => x.Start)
                    .ToDictionaryAsync(x => x.ID.ToString(), y => y.Name + " (" + y.Start.ToString("yyyy-MM-dd") + " s/d " + y.End.ToString("yyyy-MM-dd") + ")");

                List<FormModel> FormModels = new List<FormModel>();
               
                FormModels.Add(new FormModel { Label = "ID", Name = "ID", InputType = InputType.HIDDEN, Value = scheduleFromDb == null ? "0" : scheduleFromDb.ID.ToString() });
                FormModels.Add(new FormModel { Label = "Nama", Name = "Name", InputType = InputType.TEXT, Value = scheduleFromDb == null ? "" : scheduleFromDb.Name, IsRequired = true });
                FormModels.Add(new FormModel { Label = "Jenis Survei", Name = "Assesment", InputType = InputType.DROPDOWN, Options = assesments, Value = scheduleFromDb == null ? "" : scheduleFromDb.Assesment.ID.ToString(), IsRequired = true });
                FormModels.Add(new FormModel { Label = "Entitas", Name = "Entity", InputType = InputType.DROPDOWN, Options = entities, Value = scheduleFromDb == null ? "" : scheduleFromDb.Entity.ID.ToString(), IsRequired = true });
                FormModels.Add(new FormModel { Label = "Periode", Name = "Period", InputType = InputType.DROPDOWN, Options = periods, Value = scheduleFromDb == null ? "" : scheduleFromDb.Period.ID.ToString(), IsRequired = true });
                FormModels.Add(new FormModel { Label = "Sub Periode", Name = "SubPeriod", InputType = InputType.DROPDOWN, Options = subPeriods, Value = scheduleFromDb == null || scheduleFromDb.SubPeriod == null ? "" : scheduleFromDb.SubPeriod.ID.ToString(), IsRequired = true });
                FormModels.Add(new FormModel { Label = "Tanggal Mulai & Selesai", Name = "Date", InputType = InputType.TEXT, Value = scheduleFromDb == null ? "" : scheduleFromDb.Start.ToString("yyyy-MM-dd") + " s/d " + scheduleFromDb.End.ToString("yyyy-MM-dd") });

                if(scheduleFromDb != null)
                {
                    DateTime minDate = scheduleFromDb.Period.Start;
                    DateTime maxDate = scheduleFromDb.Period.End;
                    if (scheduleFromDb.SubPeriod != null)
                    {
                        minDate = scheduleFromDb.SubPeriod.Start;
                        maxDate = scheduleFromDb.SubPeriod.End;
                    }

                    FormModels.Add(new FormModel { Label = "MinDate", Name = "MinDate", InputType = InputType.HIDDEN, Value = minDate.ToString("yyyy-MM-dd") });
                    FormModels.Add(new FormModel { Label = "MaxDate", Name = "MaxDate", InputType = InputType.HIDDEN, Value = maxDate.ToString("yyyy-MM-dd") });
                }

                ViewData["Forms"] = FormModels;

                return PartialView("~/Views/Shared/_FormView.cshtml");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        [HttpGet("schedule")]
        public IActionResult Index()
        {
            ViewData["Title"] = "Penjadwalan Peserta";
            List<ColumnModel> ColumnModels = new List<ColumnModel>();
            ColumnModels.Add(new ColumnModel { Label = "Nama", Name = "Name", Style = "width: 15%; min-width: 200px" });
            ColumnModels.Add(new ColumnModel { Label = "Entitas", Name = "Entity" });
            ColumnModels.Add(new ColumnModel { Label = "Jenis Survey", Name = "Name" });
            ColumnModels.Add(new ColumnModel { Label = "Periode", Name = "Period" });
            ColumnModels.Add(new ColumnModel { Label = "Tanggal Mulai & Selesai", Name = "Date" });
            ColumnModels.Add(new ColumnModel { Label = "Daftar Peserta", Name = "Participants" });
            ColumnModels.Add(new ColumnModel { Label = "Peserta Selesai", Name = "ParticipantsFinished" });
            ColumnModels.Add(new ColumnModel { Label = "Peserta Belum Selesai", Name = "ParticipantsUnFinished" });

            ViewData["Columns"] = ColumnModels;
            ViewData["Script"] = "schedule.js";

            return View("~/Views/Shared/_Index.cshtml");
        }

        [HttpPost("schedule/save")]
        public async Task<IActionResult> Save(Schedule schedule)
        {
            try
            {
                Schedule scheduleFromDb = await _db.Schedules.FirstOrDefaultAsync(e => e.ID == schedule.ID);

                if (scheduleFromDb == null)
                {
                    schedule.Assesment = await _db.Assesments.FirstOrDefaultAsync(e => e.ID == schedule.Assesment.ID);
                    schedule.Entity = await _db.Entities.FirstOrDefaultAsync(e => e.ID == schedule.Entity.ID);
                    schedule.Period = await _db.Periods.FirstOrDefaultAsync(e => e.ID == schedule.Period.ID);
                    schedule.SubPeriod = await _db.SubPeriods.FirstOrDefaultAsync(e => e.ID == schedule.SubPeriod.ID);

                    _db.Schedules.Add(schedule);
                    _db.SaveChanges();

                    return Json(new { success = true, message = "Data berhasil disimpan" });
                }
                else
                {
                    scheduleFromDb.Assesment = await _db.Assesments.FirstOrDefaultAsync(e => e.ID == schedule.Assesment.ID);
                    scheduleFromDb.Entity = await _db.Entities.FirstOrDefaultAsync(e => e.ID == schedule.Entity.ID);
                    scheduleFromDb.Period = await _db.Periods.FirstOrDefaultAsync(e => e.ID == schedule.Period.ID);
                    scheduleFromDb.SubPeriod = await _db.SubPeriods.FirstOrDefaultAsync(e => e.ID == schedule.SubPeriod.ID);

                    scheduleFromDb.Name = schedule.Name;
                    scheduleFromDb.Start = schedule.Start;
                    scheduleFromDb.End = schedule.End;

                    _db.Schedules.Update(scheduleFromDb);
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

        [HttpPost("schedule/delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var scheduleFromDb = await _db.Schedules.FirstOrDefaultAsync(e => e.ID == id);

                if (scheduleFromDb == null)
                {
                    return Json(new { success = false, message = "Data tidak ditemukan" });
                }

                _db.Schedules.Remove(scheduleFromDb);
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
