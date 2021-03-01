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
                .Where(x => x.ParticipantUser.UserId == userId && x.ID == participantID)
                .OrderBy(x => x.Schedule.Start)
                .FirstOrDefaultAsync();

            if(participant == null)
            {
                return Redirect("/home/errors/404");
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

            Section section = null;

            Question question = null;
            Question questionNext = null;
            Question questionPrevious = null;

            if (mode == 0)
            {
                if (id == 0 && sections.Count > 0)
                {
                    section = sections[0];

                }
                else
                {
                    section = sections.Where(x => x.ID == id).FirstOrDefault();
                }
                if (section != null)
                {
                    var sectionQuestions = questions.Where(x => x.Section.ID == section.ID).OrderBy(x => x.Sequence).ToList();
                    if (sectionQuestions.Count > 0)
                    {
                        question = sectionQuestions[0];
                        if (sectionQuestions.Count > 1)
                        {
                            questionNext = sectionQuestions[1];
                        }
                    }
                }
            }
            else
            {
                if (id == 0 && questions.Count > 0)
                {
                    question = questions[0];
                    section = question.Section;
                }
                else
                {
                    question = await _db.Questions
                        .FirstOrDefaultAsync(x => x.ID == id);
                    if(question != null)
                    {
                        section = question.Section;
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
                ViewData["QuestionAnswer"] = await _db.QuestionAnswer.Where(x => x.Question.ID == question.ID).ToListAsync();
                ViewData["QuestionAnswerMatrix"] = await _db.QuestionAnswer.Where(x => x.MatrixQuestion.ID == question.ID).ToListAsync();
            } 
            else
            {
                ViewData["QuestionAnswer"] = new List<QuestionAnswer>();
                ViewData["QuestionAnswerMatrix"] = new List<QuestionAnswer>();
            }

            ViewData["Question"] = question;
            ViewData["QuestionNext"] = questionNext;
            ViewData["QuestionPrevious"] = questionPrevious;
            ViewData["Section"] = section;
            ViewData["Indexs"] = indexs;
            ViewData["Mode"] = mode;
            ViewData["Id"] = id;

            return View(participant);
        }
    }
}
