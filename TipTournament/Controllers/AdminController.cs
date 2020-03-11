using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using HtmlAgilityPack;
using log4net;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using TipTournament.App_Start;
using TipTournament.Models;

namespace TipTournament.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        public const string urlAddress = "https://www.idnes.cz/fotbal/databanka/euro-2020-los.Umli48513";
        private PointsCounter pointsCounter = new PointsCounter();
        private static readonly ILog log = LogManager.GetLogger(typeof(AdminController));


        public ActionResult Index()
        {
            ViewBag.Message = "Admin page";
            ViewBag.Users = HttpContext.GetOwinContext()
                .GetUserManager<Identity.ApplicationUserManager>()
                .Users.ToList();
            ViewBag.Matches = MatchesController.GetMatches();
            return View();
        }


        private List<Record> LoadData()
        {
            log.Info("Loading data...");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            List<Record> matches = new List<Record>();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();

                var html = string.Empty;
                using (var readStream = response.CharacterSet == null
                    ? new StreamReader(receiveStream)
                    : new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet)))
                {
                    html = readStream.ReadToEnd();
                }

                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);
                HtmlNode htmlNode = htmlDocument.GetElementbyId("table-los");
                HtmlNodeCollection collection = htmlNode.SelectNodes("//td[@class='tac']");
                int counter = 0;
                string one = String.Empty;
                string two = String.Empty;
                string result = String.Empty;
                foreach (HtmlNode node in collection)
                {
                    if (counter == 0)
                    {
                        one = node.InnerText;
                        counter++;
                    }
                    else if (counter == 1)
                    {
                        two = node.InnerText;
                        counter++;
                    }
                    else if (counter == 2)
                    {
                        result = node.InnerText;
                        counter = 0;
                        matches.Add(new Record() { One = one, Two = two, Result = result });
                    }
                }
            }
            log.Info(string.Format("{0} matches loaded", matches.Count));
            return matches;
        }

        public ActionResult GameImport()
        {
            try
            {
                List<Record> loadData = this.LoadData();

                int counter = 1;
                foreach (Record r in loadData)
                {
                    ResultModel resultModel = new ResultModel() {Id = counter, ValueOne = -1, ValueTwo = -1, Date = r.Result};
                    ResultController.AddResult(resultModel);
                    MatchesController.AddMatch(new MatchModel() {TeamOne = r.One, TeamTwo = r.Two, ResultId = counter,
                        Result = resultModel});

                }
                ResultController.SaveChanges();
                MatchesController.SaveChanges();
                
            }
            catch (Exception e)
            {
                ViewBag.Message = e.Message;
                return View("Index");
            }
            ViewBag.Message = "Succesfull load." + Environment.NewLine + MatchesController.Count() + " were loaded";
            return View("GameImportView");
        }

        public string Import()
        {
            Dictionary<int, ResultModel> newResults = new Dictionary<int, ResultModel>();
            try
            {
                List<Record> loadData = this.LoadData();



                foreach (Record r in loadData)
                {

                    if (!MatchesController.MatchExists(r.One, r.Two))
                        continue;

                    MatchModel match = MatchesController.GetMatch(r.One, r.Two);

                    int resultId = match.ResultId;
                    int matchId = match.Id;

                    ResultModel result = ResultController.GetResult(resultId);
                    if (this.ParseForResult(result, r.Result))
                        newResults.Add(matchId, result);
                }
                if (newResults.Count > 0)
                {
                    pointsCounter.CountPoints(newResults);
                    ResultController.SaveChanges();
                }

            }
            catch (Exception e)
            {
                log.Error("Error in importing results");
                return e.Message;
            }
            return newResults.Count.ToString();
        }


        public ActionResult ResultImport()
        {
            string result = Import();
            int count = 0;
            if (!Int32.TryParse(result, out count))
            {
                ViewBag.Message = result;
                return View("Index");
            }
            ViewBag.Message = "Succesfull load." + Environment.NewLine + count + " were updated";
            return View("ResultImportView");
        }

        private bool ParseForResult(ResultModel model, string result)
        {
            if (model.IsImported == 1 || result.Length <= 12)
                return false;

            var helpArray = result.Split(' ');
            string[] array = helpArray[0].Split(':');
            int one = -1, two = -1;
            try
            {
                one = Int32.Parse(array[0]);
                two = Int32.Parse(array[1]);
            }
            catch (Exception)
            {
                //cannot parse due to its not result but still a date
                return false;
            }

            try
            {
                //is after the match
                var datestr = helpArray[1].Substring(1);
                var date = datestr.Split('.');
                var datetime = new DateTime(2018, Int32.Parse(date[1]), Int32.Parse(date[0]));

                if (datetime == DateTime.Now.Date)
                {
                    var timestr = helpArray[2].Remove(helpArray[2].IndexOf(')'));
                    var time = timestr.Split(':');
                    var timespan = new TimeSpan(Int32.Parse(time[0]), Int32.Parse(time[1]), 0);
                    //game is not over yet
                    if (DateTime.Now.AddHours(-2).TimeOfDay < timespan)
                        return false;
                }
            }
            catch (Exception ex)
            {
                log.Error("Error when parsing starting date for match. With Exception:" + ex);
            }

            //here I know its definetly new information for me
            model.ValueOne = one;
            model.ValueTwo = two;
            model.IsImported = 1;
            return true;

        }

        internal class Record
        {
            public string One { get; set; }
            public string Two { get; set; }
            public string Result { get; set; }
        }

        public ActionResult SetPayment()
        {
            ViewBag.Users = new Users(HttpContext.GetOwinContext()
                   .GetUserManager<Identity.ApplicationUserManager>()
                   .Users.ToList());
            return View();
        }


        public ActionResult SubmitPayment(string userId)
        {

            PayedController.AddPayment(userId);

            return RedirectToAction("SetPayment", "Admin");
        }

        public ActionResult DeleteMatch(int matchId)
        {
            var match = MatchesController.GetMatches().First(x => x.Id == matchId);

            if (ResultController.GetResult(match.ResultId).IsImported == 1)
            {
                ResultController.DeleteResultForMatch(match.ResultId);
            }

            MatchesController.DeleteMatch(matchId);
            return RedirectToAction("Index", "Admin");
        }

        public ActionResult DeleteUser(string userId, string userName)
        {
            log.Info($"Deleting user with userNAme: {userName}");

            RankingController.DeleteUser(userId);
            PayedController.RemoveUser(userId);
            EstimatedResultController.RemoveUser(userId);

            var user = HttpContext.GetOwinContext()
                .GetUserManager<Identity.ApplicationUserManager>()
                .Users.First(x => x.Id.Equals(userId));

            HttpContext.GetOwinContext()
                .GetUserManager<Identity.ApplicationUserManager>()
                .Delete(user);

            return RedirectToAction("Index", "Admin");
        }
    }
}