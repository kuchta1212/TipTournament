using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TipTournament.App_Start;
using TipTournament.Models;

namespace TipTournament.Controllers
{
    public class TipsTable
    {

        private List<TipsTableRow> rows;

        public int Rows { get; private set; }
        public int Columns { get; private set; }

        public TipsTable(List<ApplicationUser> users)
        {
            this.rows = new List<TipsTableRow>();
            this.CreateRows(users);
        }

        private void CreateRows(List<ApplicationUser> users)
        {
            int row = 0;
            int col = 0;

            //ViewBag.EstimatedResults = new EstimatedResultForUser(EstimatedResultController.GetEstimatedResultModels());
            List<MatchModel> matches =  MatchesController.GetMatches();
            Dictionary<int, string> helpUsers = new Dictionary<int, string>();
            
            //table header
            TipsTableRow header = new TipsTableRow(row);
            header.AddCell(col, string.Empty);
            col++;
            header.AddCell(col, string.Empty);
            col++;
            foreach (ApplicationUser user in users )
            {
                helpUsers.Add(col, user.Id);
                header.AddCell(col, user.UserName);
                col++;
            }
            this.rows.Add(header);
            row++;

            int colOverAll = users.Count + 2;

            //table data
            foreach (MatchModel match in matches)
            {
                col = 2;

                TipsTableRow tipRow = new TipsTableRow(row);
                tipRow.AddCell(0, match.TeamOne);
                tipRow.AddCell(1, match.TeamTwo);
                for (int i = 2; i < colOverAll; i++)
                {
                    string userId = helpUsers[i];
                    var model = EstimatedResultController.GetEstimatedResultModelForUserAndMatch(userId, match.Id);

                    string content = model != null ? model.ToString() : string.Empty;
                    tipRow.AddCell(col, content);
                    col++;
                }
                this.rows.Add(tipRow);
                row++;
            }

            this.Rows = this.rows.Count;
            this.Columns = colOverAll; 
        }


        public string GetCellAt(int x, int y)
        {
            return this.rows.First(item => item.Index == x).GetCell(y);
        }




        private class TipsTableRow
        {
            public int Index { get; private set; }
            private List<TipsTableCell> cells;

            public TipsTableRow(int index)
            {
                this.Index = index;
                this.cells = new List<TipsTableCell>();
            }

            public void AddCell(int y, string content)
            {
                this.cells.Add(new TipsTableCell(y, content));
            }

            public string GetCell(int y)
            {
                return this.cells.First(item => item.Index == y).ToString();
            }
        }

        private class TipsTableCell
        {
            public int Index { get; private set; }
            private string content;

            public TipsTableCell(int index, string content)
            {
                this.Index = index;
                this.content = content;
            }

            public override string ToString()
            {
                return this.content;
            }

        }
    }


}