using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExamSpur.Models
{
    public class OurPartnersViewModel
    {


        public List<PayoutPartner> paidOut { get; set; }
        public List<PartnersPendingPayout> Pending { get; set; }
        public decimal? totalPayout { get; set; }
        public decimal? totalPending { get; set; }



        public OurPartnersViewModel(List<PayoutPartner> paidout, List<PartnersPendingPayout> pending, decimal? totalpayout, decimal? totalpending)
        {
            paidOut = paidout;
            Pending = pending;
            totalPayout = totalpayout;
            totalPending = totalpending;

        }

    }


    public class PartnersPendingPayout
    {
        public string username { get; set; }
        public decimal? Atsocommission { get; set; }
        public decimal? commission { get; set; }
        public decimal? AmountPaid { get; set; }
        public string paydate { get; set; }
        public PartnersPendingPayout(string username, decimal? atsocommission, decimal? commission, decimal? amountpaid, string paydate)
        {
            this.username = username;
            this.Atsocommission = atsocommission;
            this.commission = commission;
            this.AmountPaid = amountpaid;
            this.paydate = paydate;
        }
    }
    public static class PartnersgetinfoModel
    {
        public static List<OurPartnersViewModel> getinfo()
        {


            using (var db = new EXAMSPURDBEntities())
            {
                var pending = db.C_tblSubscription.Where(o => o.PlanID == 3 && o.paymentStatus==true && o.Status == false).ToList();
                var pending2 = db.C_tblSubscription.Where(o => o.PlanID == 6 && o.paymentStatus == true  && o.Status == false).ToList();
                  //pending.AddRange(pending2);
                var paidOut = db.PayoutPartners.Where(o => o.PayeeID == 2).OrderByDescending(o => o.Paydate).ToList();
                var totalpaid = paidOut.Sum(o => o.AmountPaidOut);
                var totalPending = pending.Sum(o => o.AmountPaid * (decimal?)0.2)+ pending2.Sum(o => o.AmountPaid * (decimal?)0.15);
                var list = new List<PartnersPendingPayout>();
                foreach (var item in pending )
                {

                    var a = db.C_tblMerchant.Find(item.MerchantID);
                    list.Add(new PartnersPendingPayout(a.CenterName, item.AmountPaid * (decimal?)0.3, item.AmountPaid * (decimal?)0.2, item.AmountPaid, item.SubscriptionDate.ToString()));


                }
                foreach (var item in pending2)
                {

                    var a = db.C_tblMerchant.Find(item.MerchantID);
                    list.Add(new PartnersPendingPayout(a.CenterName, 0, item.AmountPaid * (decimal?)0.15, item.AmountPaid, item.SubscriptionDate.ToString()));


                }

                var markertemodel2 = new List<OurPartnersViewModel>();
                markertemodel2.Add(new OurPartnersViewModel(paidOut, list, totalpaid, totalPending));
                return markertemodel2;
            }
        }

    }
}