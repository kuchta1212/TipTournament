using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using HtmlAgilityPack;
using Quartz;
using TipTournament.Models;
using log4net;

namespace TipTournament.Controllers
{
    public class ImportJob : IJob
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(ImportJob));

        public async Task Execute(IJobExecutionContext context)
        {
            log.Info("Starting automatic load...");
            var result = await Import();
            log.Info("Automatic load done!");
        }


        private async Task<List<AdminController.Record>> LoadData()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(AdminController.urlAddress);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            List<AdminController.Record> matches = new List<AdminController.Record>();

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
                        counter++;
                    }
                    else if (counter == 3)
                    {
                        result = node.InnerText;
                        counter = 0;
                        matches.Add(new AdminController.Record() { One = one, Two = two, Result = result });
                    }
                }
            }
            return matches;
        }


        public async Task<string> Import()
        {
            Dictionary<int, ResultModel> newResults = new Dictionary<int, ResultModel>();
            try
            {
                List<AdminController.Record> loadData = await this.LoadData();



                foreach (AdminController.Record r in loadData)
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
                    log.Info($"New results were loaded. Count: {newResults.Count}");
                    new PointsCounter().CountPoints(newResults);
                    ResultController.SaveChanges();
                }

            }
            catch (Exception e)
            {
                log.Error("error in loading data", e);
                return e.Message;
            }
            return newResults.Count.ToString();
        }

        private bool ParseForResult(ResultModel model, string result)
        {
            if (model.IsImported == 1)
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
    }
}