using AdminLte.Data;
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
    public class SurveyController : Controller
    {
        private readonly PostgreDbContext _db;
        public SurveyController(PostgreDbContext db)
        {
            _db = db;
        }

        [HttpGet("survey/table-data-view")]
        public async Task<IActionResult> GetAll(int page = 1)
        {
            try
            {
                var data = await _db.QuestionPackages
                    .Include(x=>x.Assesment)
                    .Include(x=>x.QuestionPackageLines)
                    .OrderBy(x => x.Assesment.Name)
                    .ThenBy(x=>x.Name)
                    .Skip((page-1)*10)
                    .Take(10)
                    .ToListAsync();

                var rows = new List<RowModel>();
                foreach(var row in data)
                {
                    var questions = row.QuestionPackageLines.Count();
                    rows.Add(new RowModel { 
                        ID = row.ID, 
                        Value = new string[] { 
                            row.Assesment.Name,
                            row.Name,
                            "HTML:<a href='/survey-question/" + row.ID + "'><i class='fa fa-question'></i> " + questions + " Soal</a>",
                            "HTML:<a href='/survey/result/" + row.ID + "'><i class='fa fa-file'></i> Hasil Survei</a>"
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

        [HttpGet("survey/table-paging-view")]
        public IActionResult GetPaging(int page = 1)
        {
            try
            {
                var total = _db.QuestionPackages.Count();
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

        [HttpGet("survey/form-view")]
        public async Task<IActionResult> GetFormAsync(int id = 0)
        {
            try
            {
                QuestionPackage questionPackageFromDb = null;
                if(id != 0)
                {
                    questionPackageFromDb = await _db.QuestionPackages.FirstOrDefaultAsync(e => e.ID == id);
                }
                var assesments = await _db.Assesments.OrderBy(x => x.Name).ToDictionaryAsync(x => x.ID.ToString(), y => y.Name);

                List<FormModel> FormModels = new List<FormModel>();
               
                FormModels.Add(new FormModel { Label = "ID", Name = "ID", InputType = InputType.HIDDEN, Value = questionPackageFromDb == null ? "0" : questionPackageFromDb.ID.ToString() });
                FormModels.Add(new FormModel { Label = "Jenis Survei", Name = "Assesment", InputType = InputType.DROPDOWN, Options = assesments, Value = questionPackageFromDb == null ? "" : questionPackageFromDb.Assesment.ID.ToString(), IsRequired = true });
                FormModels.Add(new FormModel { Label = "Nama", Name = "Name", InputType = InputType.TEXT, Value = questionPackageFromDb == null ? "" : questionPackageFromDb.Name, IsRequired = true });

                ViewData["Forms"] = FormModels;

                return PartialView("~/Views/Shared/_FormView.cshtml");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        [HttpGet("survey")]
        public IActionResult Index()
        {
            ViewData["Title"] = "Daftar Survei";
            List<ColumnModel> ColumnModels = new List<ColumnModel>();
            ColumnModels.Add(new ColumnModel { Label = "Jenis Survei", Name = "Assesment", Style = "width: 15%; min-width: 200px" });
            ColumnModels.Add(new ColumnModel { Label = "Nama", Name = "Name" });
            ColumnModels.Add(new ColumnModel { Label = "Daftar Soal", Name = "Questions" });
            ColumnModels.Add(new ColumnModel { Label = "Hasil Survei", Name = "Result" });

            ViewData["Columns"] = ColumnModels;
            ViewData["Script"] = "survey.js";

            return View("~/Views/Shared/_Index.cshtml");
        }

        [HttpPost("survey/save")]
        public async Task<IActionResult> Save(QuestionPackage questionPackage)
        {
            try
            {
                QuestionPackage questionPackageFromDb = await _db.QuestionPackages.FirstOrDefaultAsync(e => e.ID == questionPackage.ID);

                if (questionPackageFromDb == null)
                {
                    questionPackage.Assesment = await _db.Assesments.FirstOrDefaultAsync(e => e.ID == questionPackage.Assesment.ID);

                    _db.QuestionPackages.Add(questionPackage);
                    _db.SaveChanges();

                    return Json(new { success = true, message = "Data berhasil disimpan" });
                }
                else
                {
                    questionPackageFromDb.Assesment = await _db.Assesments.FirstOrDefaultAsync(e => e.ID == questionPackage.Assesment.ID);
                    questionPackageFromDb.Name = questionPackage.Name;
                    _db.QuestionPackages.Update(questionPackageFromDb);
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

        [HttpPost("survey/delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var questionPackageFromDb = await _db.QuestionPackages.FirstOrDefaultAsync(e => e.ID == id);

                if (questionPackageFromDb == null)
                {
                    return Json(new { success = false, message = "Data tidak ditemukan" });
                }

                _db.QuestionPackages.Remove(questionPackageFromDb);
                await _db.SaveChangesAsync();
                return Json(new { success = true, message = "Data berhasil dihapus" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new { success = false, message = "Terjadi kesalahan. Err : " + ex.Message });
            }
        }


        [HttpGet("survey/result")]
        [Route("survey/result/{surveyID:int}")]
        public async Task<IActionResult> ResultAsync(int surveyID)
        {
            var questionPackage = await _db.QuestionPackages.FirstOrDefaultAsync(x => x.ID == surveyID);

            if(questionPackage == null)
            {
                return Redirect("/home/errors/404");
            }

            var participants = await _db.Participants
                .Include(x=>x.ParticipantUser)
                .Where(x => x.FinishedAt != null && x.QuestionPackage.ID == surveyID)
                .OrderBy(x=>x.ParticipantUser.Name)
                .ToListAsync();

            var questions = await _db.QuestionPackageLines
                .Include(x=>x.Question)
                .ThenInclude(x=>x.QuestionAnswerMatrixs)
                .Include(x => x.Question.Section)
                .Where(x => x.QuestionPackage.ID == surveyID)
                .Select(x=>x.Question)
                .ToListAsync();

            var verticalDimentions = await _db.VerticalDimentions
                .Include(x=>x.Section)
                .Include(x=>x.SubVerticalDimentions)
                .ToListAsync();

            var horizontalDimentions = await _db.HorizontalDimentions
                .Include(x => x.Section)
                .ToListAsync();

            var vwCulturePerRows = await _db.VwCulturePerRow
                .ToListAsync();
            var vwPerformancePerRows = await _db.VwPerformancePerRow
                .ToListAsync();
            var vwEngagementPerRows = await _db.VwEngagementPerRow
                .ToListAsync();

            var answerCultures = new Dictionary<int, List<VwCulturePerRow>>();
            foreach (VwCulturePerRow row in vwCulturePerRows)
            {
                var id = row.ParticipantID;
                if (!answerCultures.ContainsKey(id))
                {
                    answerCultures.Add(id, new List<VwCulturePerRow>());
                }
                answerCultures[id].Add(row);
            }

            var answerPerformances = new Dictionary<int, List<VwPerformancePerRow>>();
            foreach (VwPerformancePerRow row in vwPerformancePerRows)
            {
                var id = row.ParticipantID;
                if (!answerPerformances.ContainsKey(id))
                {
                    answerPerformances.Add(id, new List<VwPerformancePerRow>());
                }
                answerPerformances[id].Add(row);
            }

            var answerEngagements = new Dictionary<int, List<VwEngagementPerRow>>();
            foreach (VwEngagementPerRow row in vwEngagementPerRows)
            {
                var id = row.ParticipantID;
                if (!answerEngagements.ContainsKey(id))
                {
                    answerEngagements.Add(id, new List<VwEngagementPerRow>());
                }
                answerEngagements[id].Add(row);
            }

            ViewData["Survey"] = questionPackage;
            ViewData["Participants"] = participants;
            ViewData["VerticalDimentions"] = verticalDimentions;
            ViewData["HorizontalDimentions"] = horizontalDimentions;
            ViewData["Questions"] = questions;
            ViewData["AnswerCultures"] = answerCultures;
            ViewData["AnswerPerformances"] = answerPerformances;
            ViewData["AnswerEngagements"] = answerEngagements;

            ViewData["SideBarCollapse"] = true;


            return View();
        }

        [HttpGet("survey/download")]
        [Route("survey/download/{surveyID:int}/{construct:int}")]
        public async Task<IActionResult> DownloadAysnc(int surveyID, Construct construct)
        {
            var questionPackage = await _db.QuestionPackages.FirstOrDefaultAsync(x => x.ID == surveyID);

            if (questionPackage == null)
            {
                return Redirect("/home/errors/404");
            }

            var participants = await _db.Participants
                .Include(x => x.ParticipantUser)
                .Where(x => x.FinishedAt != null && x.QuestionPackage.ID == surveyID)
                .OrderBy(x => x.ParticipantUser.Name)
                .ToListAsync();

            var questions = await _db.QuestionPackageLines
                .Include(x => x.Question)
                .ThenInclude(x => x.QuestionAnswerMatrixs)
                .Include(x => x.Question.Section)
                .Where(x => x.QuestionPackage.ID == surveyID && x.Question.Section.Construct == construct)
                .Select(x => x.Question)
                .ToListAsync();

            var verticalDimentions = await _db.VerticalDimentions
                .Include(x => x.Section)
                .Include(x => x.SubVerticalDimentions)
                .Where(x => x.Section.Construct == construct)
                .ToListAsync();

            var horizontalDimentions = await _db.HorizontalDimentions
                .Include(x => x.Section)
                .Where(x => x.Section.Construct == construct)
                .ToListAsync();

            var stream = new MemoryStream();
            if (construct == Construct.CULTURE)
            {
                var vwCulturePerRows = await _db.VwCulturePerRow
                    .ToListAsync();
                var answerCultures = new Dictionary<int, List<VwCulturePerRow>>();
                foreach (VwCulturePerRow row in vwCulturePerRows)
                {
                    var id = row.ParticipantID;
                    if (!answerCultures.ContainsKey(id))
                    {
                        answerCultures.Add(id, new List<VwCulturePerRow>());
                    }
                    answerCultures[id].Add(row);
                }

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(stream))
                {
                    // Sheet 1
                    var sheet1 = package.Workbook.Worksheets.Add("Hasil Jawaban");
                    sheet1.Cells["A1:A3"].Merge = true;
                    sheet1.Cells["A1:A3"].Value = "No";
                    sheet1.Cells["B1:B3"].Merge = true;
                    sheet1.Cells["B1:B3"].Value = "Subjek";
                    sheet1.Cells["C1"].Value = "Dimensi";
                    sheet1.Cells["C2"].Value = "Sub Dimensi";
                    sheet1.Cells["C3"].Value = "Item";

                    sheet1.Cells[1, 1, 3, 93].Style.Font.Bold = true;

                    int col = 4;
                    int subCol = 4;
                    int itemIndex = 1;
                    foreach(VerticalDimention vd in verticalDimentions)
                    {
                        sheet1.Cells[1, col, 1, col + 14].Merge = true;
                        sheet1.Cells[1, col, 1, col + 14].Value = vd.Name;

                        foreach(SubVerticalDimention svd in vd.SubVerticalDimentions)
                        {
                            sheet1.Cells[2, subCol, 2, subCol + 4].Merge = true;
                            sheet1.Cells[2, subCol, 2, subCol + 4].Value = svd.Name;
                            sheet1.Cells[3, subCol, 3, subCol + 4].Merge = true;
                            sheet1.Cells[3, subCol, 3, subCol + 4].Value = itemIndex;

                            subCol += 5;
                            itemIndex++;
                        }

                        col += 15;
                    }

                    int row = 4;
                    int no = 1;
                    foreach(Participant participant in participants)
                    {
                        if (!answerCultures.ContainsKey(participant.ID))
                        {
                            continue;
                        }
                        var answer = answerCultures[participant.ID];

                        sheet1.Cells[row, 1, row + 4, 1].Merge = true;
                        sheet1.Cells[row, 1].Value = no;
                        sheet1.Cells[row, 2, row + 4, 2].Merge = true;
                        sheet1.Cells[row, 2].Value = participant.ParticipantUser.Name;
                        
                        sheet1.Cells[row, 3].Value = "Respon";
                        sheet1.Cells[row + 1, 3].Value = "Urutan";
                        sheet1.Cells[row + 2, 3].Value = "Nilai";
                        sheet1.Cells[row + 3, 3].Value = "Bobot";
                        sheet1.Cells[row + 4, 3].Value = "Bobot x Nilai";
                        sheet1.Cells[row + 4, 3, row + 4, 3 + 90].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        sheet1.Cells[row + 4, 3, row + 4, 3 + 90].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#A9D08E"));

                        col = 4;
                        foreach(Question question in questions)
                        {
                            itemIndex = 1;
                            foreach(QuestionAnswer qa in question.QuestionAnswerMatrixs)
                            {
                                var asw = answer.FirstOrDefault(x => x.QuestionID == question.ID && x.MatrixRowAnswerID == qa.ID);
                                if(asw != null)
                                {
                                    sheet1.Cells[row, col].Value = itemIndex;
                                    sheet1.Cells[row + 1, col].Value = asw.urutan;
                                    sheet1.Cells[row + 2, col].Value = asw.nilai;
                                    sheet1.Cells[row + 3, col].Value = asw.bobot;
                                    sheet1.Cells[row + 4, col].Formula = "=" + sheet1.Cells[row + 2, col].Address + "*" + sheet1.Cells[row + 3, col].Address;
                                }
                                itemIndex++;
                                col++;
                            }
                        }

                        row += 5;
                        no++;
                    }

                    // Sheet 2
                    var sheet2 = package.Workbook.Worksheets.Add("Ringkasan");
                    sheet2.Cells["A1:A3"].Merge = true;
                    sheet2.Cells["A1:A3"].Value = "No";
                    sheet2.Cells["B1:B3"].Merge = true;
                    sheet2.Cells["B1:B3"].Value = "Subjek";

                    sheet2.Cells[1, 1, 3, 46].Style.Font.Bold = true;
                    col = 3;
                    subCol = 3;
                    foreach (VerticalDimention vd in verticalDimentions)
                    {
                        sheet2.Cells[1, col, 1, col + 6].Merge = true;
                        sheet2.Cells[1, col, 1, col + 6].Value = vd.Name;

                        sheet2.Cells[2, col, 2, col + 2].Merge = true;
                        sheet2.Cells[2, col, 2, col + 2].Value = "Skor Situasi";
                        sheet2.Cells[2, col + 3, 2, col + 5].Merge = true;
                        sheet2.Cells[2, col + 3, 2, col + 5].Value = "Index Situasi";

                        foreach (SubVerticalDimention svd in vd.SubVerticalDimentions)
                        {
                            sheet2.Cells[3, subCol].Value = svd.Name;
                            subCol++;
                        }
                        foreach (SubVerticalDimention svd in vd.SubVerticalDimentions)
                        {
                            sheet2.Cells[3, subCol].Value = svd.Name;
                            subCol++;
                        }

                        sheet2.Cells[2, col + 6, 3, col + 6].Merge = true;
                        sheet2.Cells[2, col + 6, 3, col + 6].Value = "Indexs Value Subjek";

                        col += 7;
                        subCol++;
                    }
                    sheet2.Cells[1, col, 3, col].Merge = true;
                    sheet2.Cells[1, col, 3, col].Value = "Index Akhlak Subjek";

                    row = 4;
                    no = 1;
                    foreach (Participant participant in participants)
                    {
                        if (!answerCultures.ContainsKey(participant.ID))
                        {
                            continue;
                        }
                        var answer = answerCultures[participant.ID];

                        sheet2.Cells[row, 1].Value = no;
                        sheet2.Cells[row, 2].Value = participant.ParticipantUser.Name;

                        col = 3;
                        subCol = 3;
                        var averageCols = "";
                        int colIndex = 0;
                        foreach (VerticalDimention vd in verticalDimentions)
                        {
                            foreach (SubVerticalDimention svd in vd.SubVerticalDimentions)
                            {
                                sheet2.Cells[row, subCol].Formula = "=SUM('Hasil Jawaban'!" + sheet1.Cells[8 + (row - 4) * 5, 4 + colIndex * 5].Address + ":" + sheet1.Cells[8 + (row - 4) * 5, 8 + colIndex * 5].Address + ")";
                                subCol++;
                                colIndex++;
                            }
                            foreach (SubVerticalDimention svd in vd.SubVerticalDimentions)
                            {
                                sheet2.Cells[row, subCol].Formula = "=((" + sheet2.Cells[row, subCol-3].Address + "-39)/76)*100";
                                sheet2.Cells[row, subCol].Style.Numberformat.Format = "0.00";
                                subCol++;
                            }

                            sheet2.Cells[row, col + 6].Formula = "=AVERAGE(" + sheet2.Cells[row, col + 3].Address + ":" + sheet2.Cells[row, col + 5].Address + ")";
                            sheet2.Cells[row, col + 6].Style.Numberformat.Format = "0.00";

                            if (averageCols == "")
                            {
                                averageCols += sheet2.Cells[row, col + 6].Address;
                            }
                            else
                            {
                                averageCols += "," + sheet2.Cells[row, col + 6].Address;
                            }

                            col += 7;
                            subCol++;
                        }
                        sheet2.Cells[row, col].Formula = "=AVERAGE(" + averageCols + ")";
                        sheet2.Cells[row, col].Style.Numberformat.Format = "0.00";

                        row++;
                        no++;
                    }

                    sheet2.Cells[row, 1, row, 2].Merge = true;
                    sheet2.Cells[row, 1, row, 2].Value = "Index Value";
                    subCol = 3;
                    col = 3;
                    foreach (VerticalDimention vd in verticalDimentions)
                    {
                        sheet2.Cells[row, col, row, col + 2].Merge = true;
                        subCol += 3;
                        foreach (SubVerticalDimention svd in vd.SubVerticalDimentions)
                        {
                            sheet2.Cells[row, subCol].Formula = "=AVERAGE(" + sheet2.Cells[4, subCol].Address + ":" + sheet2.Cells[3+participants.Count, subCol].Address + ")";
                            sheet2.Cells[row, subCol].Style.Numberformat.Format = "0.00";
                            subCol++;
                        }

                        sheet2.Cells[row, col + 6].Formula = "=AVERAGE(" + sheet2.Cells[4, subCol].Address + ":" + sheet2.Cells[3 + participants.Count, subCol].Address + ")";
                        sheet2.Cells[row, col + 6].Style.Numberformat.Format = "0.00";

                        col += 7;
                        subCol++;
                    }

                    sheet2.Cells[row, col].Formula = "=AVERAGE(" + sheet2.Cells[4, col].Address + ":" + sheet2.Cells[3 + participants.Count, col].Address + ")";
                    sheet2.Cells[row, col].Style.Numberformat.Format = "0.00";

                    sheet2.Cells[row, 1, row, 45].Style.Font.Bold = true;

                    // Sheet 3
                    var sheet3 = package.Workbook.Worksheets.Add("Laporan");
                    row = 4 + participants.Count;
                    no = 1;
                    col = 6;
                    sheet3.Cells[no, 1].Value = no;
                    sheet3.Cells[no, 2].Value = "Indeks Akhlak";
                    sheet3.Cells[no, 3].Formula = "='Ringkasan'!" + sheet2.Cells[row, 45].Address;
                    sheet3.Cells[no, 3].Style.Numberformat.Format = "0.00";

                    foreach (VerticalDimention vd in verticalDimentions)
                    {
                        no++;
                        sheet3.Cells[no, 1].Value = no;
                        sheet3.Cells[no, 2].Value = "-- Indeks " + vd.Name;
                        sheet3.Cells[no, 3].Formula = "='Ringkasan'!" + sheet2.Cells[row, col+3].Address;
                        sheet3.Cells[no, 3].Style.Numberformat.Format = "0.00";

                        foreach (SubVerticalDimention svd in vd.SubVerticalDimentions)
                        {
                            no++;
                            sheet3.Cells[no, 1].Value = no;
                            sheet3.Cells[no, 2].Value = "---- Indeks " + svd.Name;
                            sheet3.Cells[no, 3].Formula = "='Ringkasan'!" + sheet2.Cells[row, col].Address;
                            sheet3.Cells[no, 3].Style.Numberformat.Format = "0.00";
                            col++;
                        }

                        col+=4;
                    }

                    package.Save();
                }
                stream.Position = 0;

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Culture-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx"); // this will be the actual export.
            } 
            else if(construct == Construct.ENGAGEMENT)
            {
                var vwEngagementPerRows = await _db.VwEngagementPerRow
                    .ToListAsync();

                var answerEngagements = new Dictionary<int, List<VwEngagementPerRow>>();
                foreach (VwEngagementPerRow row in vwEngagementPerRows)
                {
                    var id = row.ParticipantID;
                    if (!answerEngagements.ContainsKey(id))
                    {
                        answerEngagements.Add(id, new List<VwEngagementPerRow>());
                    }
                    answerEngagements[id].Add(row);
                }

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(stream))
                {
                    // Sheet 1
                    var sheet1 = package.Workbook.Worksheets.Add("Hasil Jawaban");
                    sheet1.Cells["A1:A2"].Merge = true;
                    sheet1.Cells["A1:A2"].Value = "No";
                    sheet1.Cells["B1"].Value = "Dimensi";
                    sheet1.Cells["B2"].Value = "Subjek / Item";

                    int col = 2;
                    foreach(HorizontalDimention hd in horizontalDimentions)
                    {
                        sheet1.Cells[1, col + 1, 1, col + 20].Merge = true;
                        sheet1.Cells[1, col + 1, 1, col + 20].Value = hd.Name;
                        for (int i = 1; i <= 20; i++)
                        {
                            sheet1.Cells[2, col + i].Value = i;
                        }
                    }

                    sheet1.Cells[1, 1, 2, 82].Style.Font.Bold = true;

                    var no = 1;
                    var row = 3;
                    foreach(Participant participant in participants)
                    {
                        sheet1.Cells[row, 1].Value = no;
                        sheet1.Cells[row, 2].Value = participant.ParticipantUser.Name;

                        if (!answerEngagements.ContainsKey(participant.ID))
                        {
                            continue;
                        }
                        var answer = answerEngagements[participant.ID];

                        col = 3;
                        foreach (Question question in questions)
                        {
                            foreach (QuestionAnswer qa in question.QuestionAnswerMatrixs)
                            {
                                var asw = answer.FirstOrDefault(x => x.QuestionID == question.ID && x.MatrixRowAnswerID == qa.ID);
                                if (asw != null)
                                {
                                    sheet1.Cells[row, col].Value = asw.nilai;
                                }
                                col++;
                            }
                        }

                        no++;
                        row++;
                    }

                    sheet1.Cells[row, 1, row, 2].Merge = true;
                    sheet1.Cells[row, 1, row, 2].Value = "Rata-rata";
                    sheet1.Cells[row, 1, row, 82].Style.Font.Bold = true;

                    col = 3;
                    foreach (Question question in questions)
                    {
                        foreach (QuestionAnswer qa in question.QuestionAnswerMatrixs)
                        {
                            sheet1.Cells[row, col].Formula = "=AVERAGE(" + sheet1.Cells[3,col].Address + ":" + sheet1.Cells[2+participants.Count,col].Address + ")";
                            sheet1.Cells[row, col].Style.Numberformat.Format = "0.00";
                            col++; 
                        }
                    }

                    // Sheet 2
                    var sheet2 = package.Workbook.Worksheets.Add("Ringkasan");
                    sheet2.Cells["A1:A2"].Merge = true;
                    sheet2.Cells["A1:A2"].Value = "No";
                    sheet2.Cells["B1:B2"].Merge = true;
                    sheet2.Cells["B1:B2"].Value = "Subjek";

                    sheet2.Cells["C1:F1"].Merge = true;
                    sheet2.Cells["C1:F1"].Value = "Skor Subjek";
                    sheet2.Cells["G1:J1"].Merge = true;
                    sheet2.Cells["G1:J1"].Value = "Index Subjek";
                    sheet2.Cells["K1:K2"].Merge = true;
                    sheet2.Cells["K1:K2"].Value = "Index Engagement Subjek";

                    sheet2.Cells["A1:K2"].Style.Font.Bold = true;

                    row = 2;
                    col = 3;
                    foreach(HorizontalDimention hd in horizontalDimentions)
                    {
                        sheet2.Cells[row, col].Value = hd.Name;
                        sheet2.Cells[row, col+4].Value = hd.Name;
                        col++;
                    }

                    row = 3;
                    no = 1;
                    foreach(Participant participant in participants)
                    {
                        sheet2.Cells[row, 1].Value = no;
                        sheet2.Cells[row, 2].Value = participant.ParticipantUser.Name;
                        
                        col = 3;
                        var colIndex = 1;
                        var averageCols = "";
                        foreach (HorizontalDimention hd in horizontalDimentions)
                        {
                            sheet2.Cells[row, col].Formula = "=AVERAGE('Hasil Jawaban'!" + sheet1.Cells[row,3+((colIndex - 1) * 20)].Address + ":" + sheet1.Cells[row, 2 + (colIndex * 20)].Address + ")";
                            sheet2.Cells[row, col + 4].Formula = "=(" + sheet2.Cells[row, col].Address + "/ 6)*100";

                            if (averageCols == "")
                            {
                                averageCols += sheet2.Cells[row, col + 4].Address;
                            }
                            else
                            {
                                averageCols += "," + sheet2.Cells[row, col + 4].Address;
                            }

                            colIndex++;
                            col++;
                        }

                        sheet2.Cells[row, col + 4].Formula = "=AVERAGE("+ averageCols +")";
                        sheet2.Cells[row, 3, row, col + 4].Style.Numberformat.Format = "0.00";

                        row++;
                        no++;
                    }

                    sheet2.Cells[row, 1, row, 2].Merge = true;
                    sheet2.Cells[row, 1, row, 2].Value = "Index Value";

                    sheet2.Cells[row, 3, row, 6].Merge = true;
                    col = 7;
                    foreach (HorizontalDimention hd in horizontalDimentions)
                    {
                        sheet2.Cells[row, col].Formula = "=AVERAGE(" + sheet2.Cells[3, col].Address + ":" + sheet2.Cells[2 + participants.Count, col].Address + ")";
                        sheet2.Cells[row, col].Style.Numberformat.Format = "0.00";
                        col++;
                    }
                    sheet2.Cells[row, col].Formula = "=AVERAGE(" + sheet2.Cells[3, col].Address + ":" + sheet2.Cells[2 + participants.Count, col].Address + ")";
                    sheet2.Cells[row, col].Style.Numberformat.Format = "0.00";

                    sheet2.Cells[row, 1, row, 11].Style.Font.Bold = true;

                    // Sheet 3
                    var sheet3 = package.Workbook.Worksheets.Add("Laporan 1");
                    row = 3 + participants.Count;
                    no = 1;
                    sheet3.Cells[no, 1].Value = no;
                    sheet3.Cells[no, 2].Value = "Indeks Engagement";
                    sheet3.Cells[no, 3].Formula = "='Ringkasan'!" + sheet2.Cells[row, 11].Address;
                    sheet3.Cells[no, 3].Style.Numberformat.Format = "0.00";

                    col = 7;
                    foreach (HorizontalDimention hd in horizontalDimentions)
                    {
                        no++;
                        sheet3.Cells[no, 1].Value = no;
                        sheet3.Cells[no, 2].Value = "-- Indeks " + hd.Name;
                        sheet3.Cells[no, 3].Formula = "='Ringkasan'!" + sheet2.Cells[row, col].Address;
                        sheet3.Cells[no, 3].Style.Numberformat.Format = "0.00";

                        col++;
                    }

                    // Sheet 4
                    var sheet4 = package.Workbook.Worksheets.Add("Laporan 2");
                    sheet4.Cells["A1"].Value = "No";
                    sheet4.Cells["B1"].Value = "Faktor";
                    sheet4.Cells["C1"].Value = "Skor";
                    sheet4.Cells["D1"].Value = "Index";
                    sheet4.Cells["A1:D1"].Style.Font.Bold = true;

                    row = 2;
                    col = 3;
                    no = 1;
                    foreach(VerticalDimention vd in verticalDimentions)
                    {
                        foreach (SubVerticalDimention svd in vd.SubVerticalDimentions)
                        {
                            sheet4.Cells[row, 1].Value = no;
                            sheet4.Cells[row, 2].Value = svd.Name;

                            var averages = "";
                            for(int i = 0; i<4; i++)
                            {
                                if(averages != "")
                                {
                                    averages += ",";
                                }
                                averages += "'Hasil Jawaban'!" + sheet1.Cells[3+participants.Count, col + (i * 20)].Address;
                            }
                            sheet4.Cells[row, 3].Formula = "=AVERAGE(" + averages + ")";
                            sheet4.Cells[row, 4].Formula = "=(" + sheet4.Cells[row, 3].Address + "/6)*100";
                            sheet4.Cells[row, 3].Style.Numberformat.Format = "0.00";
                            sheet4.Cells[row, 4].Style.Numberformat.Format = "0.00";

                            row++;
                            col++;
                            no++;
                        }
                    }

                    var cells = sheet4.Cells[1, 1, row, 4];
                    cells.AutoFilter = true;

                    package.Save();
                }

                stream.Position = 0;
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Engagement-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx"); // this will be the actual export.
            }
            else if(construct == Construct.PERFORMANCE)
            {
                var vwPerformancePerRows = await _db.VwPerformancePerRow
                    .ToListAsync();
                var answerPerformances = new Dictionary<int, List<VwPerformancePerRow>>();
                foreach (VwPerformancePerRow row in vwPerformancePerRows)
                {
                    var id = row.ParticipantID;
                    if (!answerPerformances.ContainsKey(id))
                    {
                        answerPerformances.Add(id, new List<VwPerformancePerRow>());
                    }
                    answerPerformances[id].Add(row);
                }

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(stream))
                {
                    // Sheet 1
                    var sheet1 = package.Workbook.Worksheets.Add("Hasil Jawaban");
                    sheet1.Cells["A1:A4"].Merge = true;
                    sheet1.Cells["A1:A4"].Value = "No";
                    sheet1.Cells["B1:B4"].Merge = true;
                    sheet1.Cells["B1:B4"].Value = "Subjek";

                    sheet1.Cells["C1:H1"].Merge = true;
                    sheet1.Cells["C1:H1"].Value = "Kinerja Organisasi";

                    int row = 2;
                    int col = 3;
                    foreach(VerticalDimention vd in verticalDimentions)
                    {
                        sheet1.Cells[row, col, row, col + 5].Merge = true;
                        sheet1.Cells[row, col, row, col + 5].Value = vd.Name;
                        foreach (SubVerticalDimention svd in vd.SubVerticalDimentions)
                        {
                            sheet1.Cells[row + 1, col, row + 1, col + 1].Merge = true;
                            sheet1.Cells[row + 1, col, row + 1, col + 1].Value = svd.Name;

                            for(int i = 1; i<=2; i++)
                            {
                                sheet1.Cells[row + 2, col].Value = col - 2;
                                col++;
                            }
                        }
                    }

                    sheet1.Cells[1, col, 4, col].Merge = true;
                    sheet1.Cells[1, col, 4, col].Value = "Skor Kinerja Subjek";
                    sheet1.Cells[1, col + 1, 4, col + 1].Merge = true;
                    sheet1.Cells[1, col + 1, 4, col + 1].Value = "Index Kinerja Subjek";

                    sheet1.Cells[1, 1, 4, 16].Style.Font.Bold = true;

                    row = 5;
                    int no = 1;
                    foreach (Participant participant in participants)
                    {
                        sheet1.Cells[row, 1].Value = no;
                        sheet1.Cells[row, 2].Value = participant.ParticipantUser.Name;

                        if (!answerPerformances.ContainsKey(participant.ID))
                        {
                            continue;
                        }
                        var answer = answerPerformances[participant.ID];

                        col = 3;
                        foreach (Question question in questions)
                        {
                            foreach (QuestionAnswer qa in question.QuestionAnswerMatrixs)
                            {
                                var asw = answer.FirstOrDefault(x => x.QuestionID == question.ID && x.MatrixRowAnswerID == qa.ID);
                                if (asw != null)
                                {
                                    sheet1.Cells[row, col].Value = asw.nilai;
                                }
                                col++;
                            }
                        }
                        sheet1.Cells[row, col].Formula = "=AVERAGE(" + sheet1.Cells[row, 3, row, 14].Address + ")";
                        sheet1.Cells[row, col].Style.Numberformat.Format = "0.00";
                        sheet1.Cells[row, col+1].Formula = "=("+ sheet1.Cells[row, col].Address + "/6)*100";
                        sheet1.Cells[row, col+1].Style.Numberformat.Format = "0.00";

                        no++;
                        row++;
                    }

                    sheet1.Cells[row, 1, row+1, 2].Merge = true;
                    sheet1.Cells[row, 1, row+1, 2].Value = "Index Kinerja Organisasi";

                    sheet1.Cells[row, 3, row, 8].Merge = true;
                    sheet1.Cells[row, 3, row, 8].Formula = "=AVERAGE(" + sheet1.Cells[5, 3, 4 + participants.Count, 8].Address + ")";
                    sheet1.Cells[row, 3, row, 8].Style.Numberformat.Format = "0.00";

                    sheet1.Cells[row, 9, row, 14].Merge = true;
                    sheet1.Cells[row, 9, row, 14].Formula = "=AVERAGE(" + sheet1.Cells[5, 9, 4 + participants.Count, 14].Address + ")";
                    sheet1.Cells[row, 9, row, 14].Style.Numberformat.Format = "0.00";

                    sheet1.Cells[row + 1, 3, row+1, 8].Merge = true;
                    sheet1.Cells[row + 1, 3, row + 1, 8].Formula = "=(" + sheet1.Cells[row, 3].Address + "/6)*100";
                    sheet1.Cells[row + 1, 3, row + 1, 8].Style.Numberformat.Format = "0.00";

                    sheet1.Cells[row + 1, 9, row + 1, 14].Merge = true;
                    sheet1.Cells[row + 1, 9, row + 1, 14].Formula = "=(" + sheet1.Cells[row, 9].Address + "/6)*100";
                    sheet1.Cells[row + 1, 9, row + 1, 14].Style.Numberformat.Format = "0.00";

                    sheet1.Cells[row + 1, 16].Formula = "=AVERAGE(" + sheet1.Cells[5, 16, 4 + participants.Count, 16].Address + ")";
                    sheet1.Cells[row + 1, 16].Style.Numberformat.Format = "0.00";

                    sheet1.Cells[row, 1, row + 1, 16].Style.Font.Bold = true;

                    // Sheet 2
                    var sheet2 = package.Workbook.Worksheets.Add("Laporan");
                    row = 3 + participants.Count;
                    no = 1;
                    sheet2.Cells[no, 1].Value = no;
                    sheet2.Cells[no, 2].Value = "Indeks Kinerja Organisasi	";
                    sheet2.Cells[no, 3].Formula = "='Hasil Jawaban'!" + sheet1.Cells[row + 3, 16].Address;
                    sheet2.Cells[no, 3].Style.Numberformat.Format = "0.00";

                    col = 3;
                    foreach (VerticalDimention vd in verticalDimentions)
                    {
                        no++;
                        sheet2.Cells[no, 1].Value = no;
                        sheet2.Cells[no, 2].Value = "-- Indeks " + vd.Name;
                        sheet2.Cells[no, 3].Formula = "='Hasil Jawaban'!" + sheet1.Cells[row + 3, col].Address;
                        sheet2.Cells[no, 3].Style.Numberformat.Format = "0.00";

                        col+=6;
                    }

                    package.Save();
                }

                stream.Position = 0;
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Performance-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx"); // this will be the actual export.
            }

            string excelName = $"Engagement-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName); // this will be the actual export.
        }
    }
}
