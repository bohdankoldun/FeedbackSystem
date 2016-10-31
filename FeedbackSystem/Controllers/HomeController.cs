using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using FeedbackSystem.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;

namespace FeedbackSystem.Controllers
{

    [RequireHttps]
    public class HomeController : Controller
    {
        private ApplicationDbContext db;

        private UserManager<ApplicationUser> UserManager;

        public HomeController()
        {
            db = new ApplicationDbContext();
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }

        public ActionResult Index()
        {
            ViewBag.IsAuthenticated = Request.IsAuthenticated;

            if (Request.IsAuthenticated)
            {
                ApplicationUser currentUser = UserManager.FindById(User.Identity.GetUserId());
                ViewBag.CountVote = currentUser.CountVote;
            }

            return View(GetIdeasList());
        }


        public ActionResult Ideas()
        {
            ViewBag.IsAuthenticated = Request.IsAuthenticated;

            if (Request.IsAuthenticated)
            {
                ApplicationUser currentUser = UserManager.FindById(User.Identity.GetUserId());
                ViewBag.CountVote = currentUser.CountVote;
            }

            return PartialView(GetIdeasList());
        }

        public JsonResult Vote(string Id, bool vote)
        {
            if (!Request.IsAuthenticated)
            {
                //отменяем голос, если пользователь не авторизован
                return Json(new { success = false, Message = "You cannot vote because you are not authorized. Please, log in!" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                ApplicationUser currentUser = UserManager.FindById(User.Identity.GetUserId());
                Vote _vote = db.Votes.Where(v => v.IdeaId == Id && v.UserId == currentUser.Id).FirstOrDefault();

                if (_vote == null)
                {
                    //отменяем голос, если голоса закончились
                    if (currentUser.CountVote <= 0 || currentUser.CountVote == null)
                        return Json(new { success = false, Message = currentUser.UserName + ". You don't have votes!" }, JsonRequestBehavior.AllowGet);

                    //защитываем голос и записываем его в бд
                    Vote newVote = new Vote { Id = Guid.NewGuid().ToString(), IdeaId = Id, UserId = currentUser.Id, _Vote = vote };
                    db.Votes.Add(newVote);
                    //отнимаем использованный голос у пользователя
                    currentUser.CountVote = currentUser.CountVote - 1;
                    //сохраняем изменения
                    db.SaveChanges();

                    return Json(new { success = true, Message = currentUser.UserName + ". Your vote counted." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //отменяем голос, если пользователь уже голосовал за эту идею
                    return Json(new { success = false, Message = currentUser.UserName + "! You have used your vote to the idea." }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        public JsonResult AddNewIdea(Idea newIdea)
        {
            if (Request.IsAuthenticated)
            {
                if (newIdea.DescriptionIdea == null)
                    return Json(new { success = false, Message = "Field is empty. Describe your idea." });

                ApplicationUser currentUser = UserManager.FindById(User.Identity.GetUserId());

                newIdea.Id = Guid.NewGuid().ToString();
                newIdea.User = currentUser;
                newIdea.UserId = currentUser.Id;
                newIdea.Date = DateTime.Now;

                //добавляем новую идею в бд
                db.Ideas.Add(newIdea);
                db.SaveChanges();

                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, Message = "You are not authorized." });
            }

        }

        /// <summary>
        /// функция для получения списка всех идей из базы данных
        /// </summary>
        /// <returns>список всех идей</returns>
        private List<IdeaToView> GetIdeasList()
        {
            var Ideas = db.Ideas.OrderBy(idea => idea.Date).ToList();
            var IdeasToView = new List<IdeaToView>();

            foreach (Idea idea in Ideas)
            {
                //подсчитываем количество голосов
                int _PositiveVotes = db.Votes.Count(vote => vote.IdeaId == idea.Id && vote._Vote == true);
                int _NegativeVotes = db.Votes.Count(vote => vote.IdeaId == idea.Id && vote._Vote == false);

                //находим создателя идеи
                ApplicationUser user = UserManager.FindById(idea.UserId);

                //создаем и добавляем идею в список
                var IdeaToView = new IdeaToView { Id = idea.Id, DescriptionIdea = idea.DescriptionIdea, Date = String.Format("{0:d/M/yyyy  HH:mm}", idea.Date), Username = user.UserName, PositiveVotes = _PositiveVotes, NegativeVotes = _NegativeVotes };
                IdeasToView.Add(IdeaToView);
            }

            return IdeasToView;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}