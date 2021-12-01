using AdminLte.Data;
using AdminLte.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AdminLte.Helpers;

namespace AdminLte.Controllers
{
    [Authorize(Roles = "Pengguna Khusus")]
    [CustomAuthFilter("Access_PengaturanPengguna_HakAkses")]
    public class RoleController : Controller
    {
        private readonly ApplicationDbContext _db;
        public RoleController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet("role/table-data-view")]
        public async Task<IActionResult> GetAll(int page = 1)
        {
            try
            {
                var data = await _db.UserAccesses
                    .OrderBy(x => x.Name)
                    .Skip((page - 1) * 10)
                    .Take(10)
                    .ToListAsync();

                var rows = new List<RowModel>();
                foreach (var row in data)
                {
                    rows.Add(new RowModel { ID = row.ID, Value = new string[] { row.Name } });
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

        [HttpGet("role/table-paging-view")]
        public IActionResult GetPaging(int page = 1)
        {
            try
            {
                var total = _db.UserAccesses.Count();
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

        [HttpGet("role/form-view")]
        public async Task<IActionResult> GetFormAsync(int id = 0)
        {
            try
            {
                UserAccess userAccessFromDb = null;
                if (id != 0)
                {
                    userAccessFromDb = await _db.UserAccesses.FirstOrDefaultAsync(e => e.ID == id);
                }
                List<FormModel> FormModels = new List<FormModel>();

                FormModels.Add(new FormModel { Label = "ID", Name = "ID", InputType = InputType.HIDDEN, Value = userAccessFromDb == null ? "0" : userAccessFromDb.ID.ToString() });
                FormModels.Add(new FormModel { Label = "Nama", Name = "Name", InputType = InputType.TEXT, Value = userAccessFromDb == null ? "" : userAccessFromDb.Name, IsRequired = true });

                var accessRow = new Dictionary<string, string>
                {
                    {"Access_MasterData_JenisSurvei", "Master Data - Jenis Survei"},
                    {"Access_MasterData_Konstruk", "Master Data - Konstruk"},
                    {"Access_MasterData_DimensiVertical", "Master Data - Dimensi Vertikal"},
                    {"Access_MasterData_DimensiHorizontal", "Master Data - Dimensi Horisontal"},
                    {"Access_MasterData_StrukturOrganisasi_Entitas", "Master Data - Struktur Organisasi - Entias"},
                    {"Access_MasterData_StrukturOrganisasi_LevelJabatan", "Master Data - Struktur Organisasi - Level Jabatan"},
                    {"Access_MasterData_Periode", "Master Data - Periode"},
                    {"Access_MasterData_Pertanyaan", "Master Data - Pertanyaan"},
                    {"Access_MasterData_DaftarSurvei", "Daftar Survei"},
                    {"Access_Penjadwalan_PenjadwalanSurvei", "Penjadwalan - Penjadwalan Survei"},
                    {"Access_Penjadwalan_PenjadwalanPeserta", "Penjadwalan - Penjadwalan Peserta"},
                    {"Access_PengaturanPengguna_HakAkses", "Pengaturan Pengguna - Hak Akses"},
                    {"Access_PengaturanPengguna_PenggunaUmum", "Pengaturan Pengguna - Pengguna Umum"},
                    {"Access_PengaturanPengguna_PenggunaKhusus", "Pengaturan Pengguna - Pengguna Khusus"},
                };

                var accessValue = new Dictionary<string, bool>
                {
                    {"Access_MasterData_JenisSurvei", userAccessFromDb == null ? false : userAccessFromDb.Access_MasterData_JenisSurvei},
                    {"Access_MasterData_Konstruk", userAccessFromDb == null ? false : userAccessFromDb.Access_MasterData_Konstruk},
                    {"Access_MasterData_DimensiVertical", userAccessFromDb == null ? false : userAccessFromDb.Access_MasterData_DimensiVertical},
                    {"Access_MasterData_DimensiHorizontal", userAccessFromDb == null ? false : userAccessFromDb.Access_MasterData_DimensiHorizontal},
                    {"Access_MasterData_StrukturOrganisasi_Entitas", userAccessFromDb == null ? false : userAccessFromDb.Access_MasterData_StrukturOrganisasi_Entitas},
                    {"Access_MasterData_StrukturOrganisasi_LevelJabatan", userAccessFromDb == null ? false : userAccessFromDb.Access_MasterData_StrukturOrganisasi_LevelJabatan},
                    {"Access_MasterData_Periode", userAccessFromDb == null ? false : userAccessFromDb.Access_MasterData_Periode},
                    {"Access_MasterData_Pertanyaan", userAccessFromDb == null ? false : userAccessFromDb.Access_MasterData_Pertanyaan},
                    {"Access_MasterData_DaftarSurvei", userAccessFromDb == null ? false : userAccessFromDb.Access_MasterData_DaftarSurvei},
                    {"Access_Penjadwalan_PenjadwalanSurvei", userAccessFromDb == null ? false : userAccessFromDb.Access_Penjadwalan_PenjadwalanSurvei},
                    {"Access_Penjadwalan_PenjadwalanPeserta", userAccessFromDb == null ? false : userAccessFromDb.Access_Penjadwalan_PenjadwalanPeserta},
                    {"Access_PengaturanPengguna_HakAkses", userAccessFromDb == null ? false : userAccessFromDb.Access_PengaturanPengguna_HakAkses},
                    {"Access_PengaturanPengguna_PenggunaUmum", userAccessFromDb == null ? false : userAccessFromDb.Access_PengaturanPengguna_PenggunaUmum},
                    {"Access_PengaturanPengguna_PenggunaKhusus", userAccessFromDb == null ? false : userAccessFromDb.Access_PengaturanPengguna_PenggunaKhusus},
                };

                foreach (string key in accessRow.Keys)
                {
                    FormModels.Add(new FormModel { Label = accessRow[key], Name = key, InputType = InputType.YESNO, Value = accessValue[key] ? "1" : "0", IsRequired = true });
                }

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

        [HttpGet("role")]
        public IActionResult Index()
        {
            ViewData["Title"] = "Hak Akses";
            List<ColumnModel> ColumnModels = new List<ColumnModel>();
            ColumnModels.Add(new ColumnModel { Label = "Nama", Name = "Name", Style = "width: 15%; min-width: 200px" });

            ViewData["Columns"] = ColumnModels;
            ViewData["Script"] = "user-access.js";
            ViewData["ModalStye"] = "modal-xl";

            return View("~/Views/Shared/_Index.cshtml");
        }

        [HttpPost("role/save")]
        public async Task<IActionResult> Save(UserAccess userAccess)
        {
            try
            {
                UserAccess userAccessFromDb = await _db.UserAccesses.FirstOrDefaultAsync(e => e.ID == userAccess.ID);

                if (userAccessFromDb == null)
                {
                    _db.UserAccesses.Add(userAccess);
                    _db.SaveChanges();

                    return Json(new { success = true, message = "Data berhasil disimpan" });
                }
                else
                {
                    userAccessFromDb.Name = userAccess.Name;
                    userAccessFromDb.Access_MasterData_JenisSurvei = userAccess.Access_MasterData_JenisSurvei;
                    userAccessFromDb.Access_MasterData_Konstruk = userAccess.Access_MasterData_Konstruk;
                    userAccessFromDb.Access_MasterData_DimensiVertical = userAccess.Access_MasterData_DimensiVertical;
                    userAccessFromDb.Access_MasterData_DimensiHorizontal = userAccess.Access_MasterData_DimensiHorizontal;
                    userAccessFromDb.Access_MasterData_StrukturOrganisasi_Entitas = userAccess.Access_MasterData_StrukturOrganisasi_Entitas;
                    userAccessFromDb.Access_MasterData_StrukturOrganisasi_LevelJabatan = userAccess.Access_MasterData_StrukturOrganisasi_LevelJabatan;
                    userAccessFromDb.Access_MasterData_Periode = userAccess.Access_MasterData_Periode;
                    userAccessFromDb.Access_MasterData_Pertanyaan = userAccess.Access_MasterData_Pertanyaan;
                    userAccessFromDb.Access_MasterData_DaftarSurvei = userAccess.Access_MasterData_DaftarSurvei;
                    userAccessFromDb.Access_Penjadwalan_PenjadwalanSurvei = userAccess.Access_Penjadwalan_PenjadwalanSurvei;
                    userAccessFromDb.Access_Penjadwalan_PenjadwalanPeserta = userAccess.Access_Penjadwalan_PenjadwalanPeserta;
                    userAccessFromDb.Access_PengaturanPengguna_HakAkses = userAccess.Access_PengaturanPengguna_HakAkses;
                    userAccessFromDb.Access_PengaturanPengguna_PenggunaUmum = userAccess.Access_PengaturanPengguna_PenggunaUmum;
                    userAccessFromDb.Access_PengaturanPengguna_PenggunaKhusus = userAccess.Access_PengaturanPengguna_PenggunaKhusus;
                    _db.UserAccesses.Update(userAccessFromDb);
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

        [HttpPost("role/delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userAccessFromDb = await _db.UserAccesses.FirstOrDefaultAsync(e => e.ID == id);

                if (userAccessFromDb == null)
                {
                    return Json(new { success = false, message = "Data tidak ditemukan" });
                }

                _db.UserAccesses.Remove(userAccessFromDb);
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
