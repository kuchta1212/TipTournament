﻿using System;
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


        private Task<List<AdminController.Record>> LoadData()
        {
            var request = (HttpWebRequest)WebRequest.Create(AdminController.urlAddress);
            var response = (HttpWebResponse)request.GetResponse();
            var matches = new List<AdminController.Record>();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var receiveStream = response.GetResponseStream();

                var html = string.Empty;
                using (var readStream = response.CharacterSet == null
                    ? new StreamReader(receiveStream)
                    : new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet)))
                {
                    html = readStream.ReadToEnd();
                }

                
                //            using (StreamReader sr = new StreamReader("C:\\Users\\kucha\\Desktop\\idnes.txt"))
                //            {
                //                String html = sr.ReadToEnd();
                
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);
                var htmlNode = htmlDocument.GetElementbyId("table-los");
                var collection = htmlNode.SelectNodes("//td[@class='tac']");
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
            return Task.FromResult(matches);
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
                log.Error("Error when parsing starting date for match. With exception:"+ ex);
            }

            //here I know its definetly new information for me
            model.ValueOne = one;
            model.ValueTwo = two;
            model.IsImported = 1;
            return true;

        }
    }
}