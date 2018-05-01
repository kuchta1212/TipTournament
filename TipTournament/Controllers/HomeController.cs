using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using TipTournament.App_Start;
using TipTournament.Models;

namespace TipTournament.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Login()
        {
            string userId = User.Identity.GetUserId();
            if(!RankingController.IsUserSaved(userId))
                RankingController.AddUser(userId);

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Index()
        {
            if (!Request.IsAuthenticated)
                return View();

            try
            {
                ViewBag.Matches = MatchesController.GetMatches();
                ViewBag.EstimatedResults = new EstimatedResultForUser
                   (EstimatedResultController.GetEstimatedResultModelsForUser(User.Identity.GetUserId()));
                ViewBag.Ranking = RankingController.GetRanking();
                ViewBag.Users = new Users(HttpContext.GetOwinContext()
                    .GetUserManager<Identity.ApplicationUserManager>()
                    .Users.ToList());

                ViewBag.Message = "OK";
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }

            return View();
        }

        public ActionResult SetEstimatedResults()
        {
            try
            {
                ViewBag.Matches = MatchesController.GetMatches();

                ViewBag.EstimatedResults = new EstimatedResultForUser
                    (EstimatedResultController.GetEstimatedResultModelsForUser(User.Identity.GetUserId()));
                ViewBag.Message = "OK";
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }
            return View("SetResultsView");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult SubmitResults()
        {
            int one = Int32.Parse(Request["Num1"]);
            int two = Int32.Parse(Request["Num2"]);
            int id = Int32.Parse(Request["MatchId"]);

            string userId = User.Identity.GetUserId();


            EstimatedResultController.AddEstimatedResult(userId, id, one, two);

            return RedirectToAction("SetEstimatedResults", "Home");
        }

        public ActionResult AllTips()
        {
            TipsTable tipsTable = new TipsTable(HttpContext.GetOwinContext()
                    .GetUserManager<Identity.ApplicationUserManager>()
                    .Users.ToList());

            ViewBag.Table = tipsTable;

            

            return View();
        }

        public ActionResult Rules()
        {
            return View();
        }
    }
}