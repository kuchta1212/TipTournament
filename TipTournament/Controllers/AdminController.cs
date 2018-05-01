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
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using TipTournament.App_Start;
using TipTournament.Models;

namespace TipTournament.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private const string urlAddress = "http://fotbal.idnes.cz/databanka.aspx?t=los&id=1000414";
        private PointsCounter pointsCounter = new PointsCounter();

        public ActionResult Index()
        {
            ViewBag.Message = "Init page";
            return View();
        }


        private List<Record> LoadData()
        {
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                        List<Record> matches = new List<Record>();

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            Stream receiveStream = response.GetResponseStream();
                            StreamReader readStream = null;
            
                            readStream = response.CharacterSet == null
                                ? new StreamReader(receiveStream)
                                : new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
            
                            string html = readStream.ReadToEnd();
//            using (StreamReader sr = new StreamReader("C:\\Users\\kucha\\Desktop\\idnes.txt"))
//            {
//                String html = sr.ReadToEnd();
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
            if (model.IsImported==1)
                return false;

            string[] array = result.Split(':');
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

            //here I know its definetly new information for me

            model.ValueOne = one;
            model.ValueTwo = two;
            model.IsImported = 1;
            return true;

        }

        private class Record
        {
            public string One { get; set; }
            public string Two { get; set; }
            public string Result { get; set; }
        }

        public ActionResult SetPayment()
        {
            ViewBag.Payment = PayedController.GetPayedList();
            ViewBag.Users = new Users(HttpContext.GetOwinContext()
                   .GetUserManager<Identity.ApplicationUserManager>()
                   .Users.ToList());
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public ActionResult SubmitPayment()
        {
            int id = Int32.Parse(Request["PaymentId"]);
            var isPayed = Request["isPayed"];

            


            PayedController.AddPayment(id);

            return RedirectToAction("SetPayment", "Admin");
        }
    }
}