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
    public class SurveyParticipantController : Controller
    {
        private readonly PostgreDbContext _db;
        private readonly UserManager<User> _userManager;
        public SurveyParticipantController(PostgreDbContext db, UserManager<User> user)
        {
            _db = db;
            _userManager = user;
        }

        [HttpGet("survey-participant")]
        public async Task<IActionResult> IndexAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;

            var participants = await _db.Participants
                .Include(x=>x.ParticipantUser)
                .Include(x=>x.QuestionPackage)
                .Include(x=>x.Schedule)
                .Include(x => x.Schedule.Assesment)
                .Include(x => x.Schedule.Entity)
                .Where(x => x.ParticipantUser.UserId == userId)
                .OrderBy(x=>x.Schedule.Start)
                .ToListAsync();

            return View(participants);
        }

        [HttpGet("survey-participant/detail")]
        [Route("survey-participant/detail/{participantID:int}")]
        public async Task<IActionResult> DetailAsync(int participantID, int mode = 0, int id = 0)
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;

            var participant = await _db.Participants
                .Include(x => x.ParticipantUser)
                .Include(x => x.QuestionPackage.Assesment)
                .Include(x => x.ParticipantAnswerSheets)
                .ThenInclude(x => x.ParticipantAnswerSheetSections)
                .ThenInclude(x => x.Section)
                .Where(x => x.ParticipantUser.UserId == userId && x.ID == participantID)
                .OrderBy(x => x.Schedule.Start)
                .FirstOrDefaultAsync();

            if(participant == null)
            {
                return Redirect("/home/errors/404");
            }

            ParticipantAnswerSheet participantAnswerSheet = participant.ParticipantAnswerSheets.FirstOrDefault(x => !x.IsFinish);
            if (participantAnswerSheet == null)
            {
                participantAnswerSheet = new ParticipantAnswerSheet();
                participantAnswerSheet.Participant = participant;
                participantAnswerSheet.IsFinish = false;

                _db.ParticipantAnswerSheets.Add(participantAnswerSheet);
                _db.SaveChanges();
            }

            ViewData["Title"] = participant.QuestionPackage.Assesment.Name + " - " + participant.QuestionPackage.Name;
            ViewData["BreadCrump"] = new Dictionary<string, string>()
            {
                {"Daftar Survey Tersedia", "/survey-participant"}
            };

            var questions = await _db.QuestionPackageLines
                .Include(x => x.Question)
                .Include(x => x.Question.Section)
                .Where(x => x.QuestionPackage.ID == participant.QuestionPackage.ID)
                .Select(x => x.Question)
                .OrderBy(x => x.Section.Sequence)
                .ThenBy(x => x.Sequence)
                .ToListAsync();
            var sections = questions.Select(x => x.Section).Distinct().OrderBy(x=>x.Sequence).ToList();

            var indexs = new Dictionary<Section, List<List<Question>>>();
            foreach (Section sct in sections)
            {
                ParticipantAnswerSheetSection newParticipantAnswerSheetSection = participantAnswerSheet.ParticipantAnswerSheetSections.FirstOrDefault(x => x.Section.ID == sct.ID);
                if(newParticipantAnswerSheetSection == null)
                {
                    newParticipantAnswerSheetSection = new ParticipantAnswerSheetSection();
                    newParticipantAnswerSheetSection.IsFinish = false;
                    newParticipantAnswerSheetSection.Section = sct;
                    newParticipantAnswerSheetSection.ParticipantAnswerSheet = participantAnswerSheet;

                    participantAnswerSheet.ParticipantAnswerSheetSections.Add(newParticipantAnswerSheetSection);

                    _db.ParticipantAnswerSheetSections.Add(newParticipantAnswerSheetSection);
                    _db.SaveChanges();
                }

                var list = new List<List<Question>>();
                var listI = new List<Question>();
                var i = 1;
                foreach (Question ques in sct.Questions)
                {
                    if (i == 1)
                    {
                        listI = new List<Question>();
                    }
                    listI.Add(ques);
                    if (i == 5)
                    {
                        i = 0;
                        list.Add(listI);
                    }
                    i++;
                }
                if (i != 1)
                {
                    list.Add(listI);
                }
                indexs.Add(sct, list);
            }

            ParticipantAnswerSheetSection participantAnswerSheetSection = participantAnswerSheet.ParticipantAnswerSheetSections.FirstOrDefault(x => !x.IsFinish);
            Section section = null;

            if(participantAnswerSheetSection != null)
            {
                section = participantAnswerSheetSection.Section;
            }

            Question question = null;
            Question questionNext = null;
            Question questionPrevious = null;

            var questionAnswereds = await _db.ParticipantAnswerSheetLines
                .Where(x => x.ParticipantAnswerSheet.ID == participantAnswerSheet.ID)
                .Select(x => x.Question)
                .Distinct()
                .ToListAsync();

            if (section != null)
            {
                if (id == 0 && questions.Where(x=>x.Section.ID == section.ID).Count() > 0)
                {
                    question = questions.FirstOrDefault(x=>x.Section.ID == section.ID && questionAnswereds.Contains(x));
                    if(question == null)
                    {
                        question = questions.FirstOrDefault(x => x.Section.ID == section.ID);
                    }
                }
                else
                {
                    question = await _db.Questions
                        .FirstOrDefaultAsync(x => x.ID == id && x.Section.ID == section.ID);
                    if(!questionAnswereds.Contains(question))
                    {
                        question = questions
                            .FirstOrDefault(x => x.Section.ID == section.ID && !questionAnswereds.Contains(x));
                    }
                }

                var questionIndex = questions.IndexOf(question);
                if (questionIndex + 1 < questions.Count)
                {
                    questionNext = questions[questionIndex + 1];
                }
                if (questionIndex - 1 >= 0)
                {
                    questionPrevious = questions[questionIndex - 1];
                }
            }

            if (question != null)
            {
                ViewData["Answer"] = await _db.ParticipantAnswerSheetLines.Where(x => x.Question.ID == question.ID && x.ParticipantAnswerSheet.ID == participantAnswerSheet.ID).ToListAsync();
                ViewData["QuestionAnswer"] = await _db.QuestionAnswer.Where(x => x.Question.ID == question.ID).ToListAsync();
                ViewData["QuestionAnswerMatrix"] = await _db.QuestionAnswer.Where(x => x.MatrixQuestion.ID == question.ID).ToListAsync();
            } 
            else
            {
                ViewData["Answer"] = new List<ParticipantAnswerSheetLine>();
                ViewData["QuestionAnswer"] = new List<QuestionAnswer>();
                ViewData["QuestionAnswerMatrix"] = new List<QuestionAnswer>();
            }

            ViewData["Question"] = question;
            ViewData["QuestionAnswered"] = questionAnswereds;
            ViewData["QuestionNext"] = questionNext;
            ViewData["QuestionPrevious"] = questionPrevious;
            ViewData["Section"] = section;
            ViewData["Indexs"] = indexs;
            ViewData["Mode"] = mode;
            ViewData["Id"] = id;
            ViewData["SideBarCollapse"] = true;
            ViewData["ParticipantID"] = participantID;
            ViewData["Script"] = "survey-participant-detail.js";

            return View(participantAnswerSheet); 
        }

        [HttpPost("[action]")]
        [Route("survey-participant/save")]
        public async Task<IActionResult> Save([FromBody] ParticipantAnswerSheetLine[] participantAnswerSheetLines)
        {
            try
            {
                if (participantAnswerSheetLines.Length > 0)
                {
                    Question question = await _db.Questions.FirstOrDefaultAsync(x => x.ID == participantAnswerSheetLines.First().Question.ID);
                    ParticipantAnswerSheet participantAnswerSheet = await _db.ParticipantAnswerSheets.FirstOrDefaultAsync(x => x.ID == participantAnswerSheetLines.First().ParticipantAnswerSheet.ID);

                    var oldAnswers = await _db.ParticipantAnswerSheetLines.Where(x => x.ParticipantAnswerSheet.ID == participantAnswerSheet.ID && x.Question.ID == question.ID).ToListAsync();
                    _db.ParticipantAnswerSheetLines.RemoveRange(oldAnswers);

                    foreach (ParticipantAnswerSheetLine participantAnswerSheetLine in participantAnswerSheetLines)
                    {
                        participantAnswerSheetLine.Question = question;
                        participantAnswerSheetLine.ParticipantAnswerSheet = participantAnswerSheet;

                        _db.ParticipantAnswerSheetLines.Add(participantAnswerSheetLine);
                    }
                }
                _db.SaveChanges();

                return Json(new { success = true, message = "Jawaban berhasil disimpan" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new { success = false, message = "Terjadi kesalahan. Err : " + ex.Message });
            }
        }

    }
}
