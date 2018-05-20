using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using TipTournament.Models;

namespace TipTournament.Controllers
{
    public class PayedController
    {
        private static PayedDbContext payedDbContext = new PayedDbContext();

        public static List<PayedModel> GetPayedList()
        {
            return payedDbContext.Payed.ToList();
        }

        public static PayedModel PayedModel(string userId)
        {
            return GetPayedList().FirstOrDefault(item => item.UserId == userId);
        }

        public static bool Payed(string userId)
        {
            var model = PayedModel(userId);
            return model != null && model.IsPayed;
        }

        public static void AddPayment(int id)
        {
            PayedModel payedModel = GetPayedList().FirstOrDefault(item => item.Id == id);
            if (payedModel == null)
                return;
            payedModel.IsPayed = true;
            Save();
        }

        public static void RemoveUser(string userId)
        {
            var model = payedDbContext.Payed.FirstOrDefault(x => x.UserId.Equals(userId));
            if (model != null)
            {
                payedDbContext.Payed.Remove(model);
                payedDbContext.SaveChanges();
            }

        }

        public static void Save()
        {
            payedDbContext.SaveChanges();
        }
    }
}