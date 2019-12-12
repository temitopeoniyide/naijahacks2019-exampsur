using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExamSpur.Models
{
    public class MerchantSubViewModel
    {
        public Int16 Id { get; set; }
        public string PlanType { get; set; }
        public decimal? PlanAmount { get; set; }
        public string PlanRange { get; set; }
        //public List<C_tblSubscriptionPlan> plans { get; set; }
    }
    public class MerchantSubHistory
    {
        public String Username { get; set; }
        public string PlanType { get; set; }
        public decimal? PlanAmount { get; set; }
        public string PaymentRef { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentDate { get; set; }

    }
    public static  class GetMerchantSub {
        public static object GetSub()
        {
            using (var db = new EXAMSPURDBEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var model = new List<C_tblSubscriptionPlan>();
                model = db.C_tblSubscriptionPlan.Where(o => o.UserType == 2).ToList();
                var model2 = new List<MerchantSubViewModel>();
                foreach (var item in model) {
                    model2.Add(new MerchantSubViewModel{ Id=item.Id, PlanType=item.PlanType, PlanAmount=item.PlanAmount, PlanRange= item.PlanRange });
                }
                return model2;
            }
        }

        public static object MerchantSubHistory()
        {
            try {
                using (var db = new EXAMSPURDBEntities()) {
                    db.Configuration.ProxyCreationEnabled = false;
                    var merchantid = Convert.ToInt64(HttpContext.Current.Session["merchantid"]);
                    var subs = db.C_tblSubscription.Where(o => o.MerchantID == merchantid).ToList();
                    var model =(from a in subs select
                    new MerchantSubHistory {
                        Username = a.Username,
                        PaymentDate = a.SubscriptionDate.Date.ToString(),
                        PaymentRef = a.paymentRef,
                        PaymentStatus = (a.paymentStatus == true) ? "Succesful" : "Failed",
                        PlanAmount = db.C_tblSubscriptionPlan.Find(a.PlanID).PlanAmount,
                        PlanType = db.C_tblSubscriptionPlan.Find(a.PlanID).PlanType,

                    }).ToList();
                    return model;
                }
               

            } catch (Exception ex) { return null; }

        }
    }
    public class NewPaymentMerchant
    {
        public int PlanId { get; set; }
        public NewPaymentMerchant(int planid)
        {
            this.PlanId = planid;
        }
        public object saveNewPayment()
        {
            try
            {
                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var paymentObj = new C_tblSubscription();
                    paymentObj.MerchantID = Convert.ToInt64(HttpContext.Current.Session["merchantid"]);
                    paymentObj.Username = HttpContext.Current.Session["email"].ToString();
                    paymentObj.paymentStatus = false;
                    paymentObj.PlanID = (short?)PlanId;
                    paymentObj.SubscriptionDate = DateTime.Now;
                    db.C_tblSubscription.Add(paymentObj);
                    db.SaveChanges();
                  
                    var a = db.C_tblSubscriptionPlan.Find(PlanId);
                    return new { value = 1, subId = paymentObj.SubscriptionID, amount = a.PlanAmount, min = a.PlanRange.Split('-')[0], max = a.PlanRange.Split('-')[1], name = a.PlanType };
                }
            }  
            catch (Exception ex)
            {
                return new { value = 0, subId = "Error Occured!!. Please try again" };
            }

        }


    }
    public static class GetPrice
    {
        public static object GetPiceByID(int planID)
        {
            try
            {
                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var model = db.C_tblSubscriptionPlan.Find(planID);
                    return new { value = 1, amount = model.PlanAmount, min = model.PlanRange.Split('-')[0], max = model.PlanRange.Split('-')[1], name = model.PlanType };
                }
            }
            catch (Exception Ex)
            {
                return new { value = 0, msg = "Something went wrong. Try again later" };
            }
        }
        public class UpdatePaymentMerchant
        {
            public int SubId { get; set; }
            public String PaymentRef { get; set; }
            public UpdatePaymentMerchant(int subid, String paymentref)
            {
                this.SubId = subid;
                this.PaymentRef = paymentref;
            }
            public object UpdatePay()
            {
                try
                {
                    using (var db = new EXAMSPURDBEntities())
                    {
                        db.Configuration.ProxyCreationEnabled = false;
                        var paymentObj = db.C_tblSubscription.Find(SubId);
                        paymentObj.paymentRef = PaymentRef;
                        paymentObj.paymentStatus = true;
                        paymentObj.SubscriptionDate = DateTime.Now;
                        db.C_tblSubscription.Attach(paymentObj);
                        db.Entry(paymentObj).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        var unit = db.C_tblSubscriptionPlan.Find(paymentObj.PlanID).Unit;
                        var userid = (long?)HttpContext.Current.Session["userid"];
                        var user = db.C_tblMerchant.Find(userid);
                        user.UnitAvailable += unit;
                        db.C_tblMerchant.Attach(user);
                        db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        return new { value = 1, subId = paymentObj.SubscriptionID };
                    }
                }
                catch (Exception ex)
                {
                    return new { value = 0, subId = "Error Occured!!. Please try again" };
                }

            }
        }
    }
    public class UpdatePaymentM
    {
        public int SubId { get; set; }
        public int Unit { get; set; }
        public String PaymentRef { get; set; }
        public UpdatePaymentM(int subid, String paymentref,int unit)
        {
            this.SubId = subid;
            this.PaymentRef = paymentref;
            this.Unit = unit;
        }
        public object UpdatePay()
        {
            try
            {
                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var paymentObj = db.C_tblSubscription.Find(SubId);
                    var type = db.C_tblSubscriptionPlan.Find(paymentObj.PlanID);
                    paymentObj.paymentRef = PaymentRef;
                    paymentObj.paymentStatus = true;
                    paymentObj.SubscriptionDate = DateTime.Now;
                    paymentObj.Username = HttpContext.Current.Session["email"].ToString();
                    paymentObj.Unit = Unit;
                    paymentObj.Status = false;
                    paymentObj.AmountPaid = type.PlanAmount*(decimal?) Unit;
                
                    db.C_tblSubscription.Attach(paymentObj);
                    db.Entry(paymentObj).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    var mid = Convert.ToInt64(HttpContext.Current.Session["merchantid"]);
                   
                    var merchant = db.C_tblMerchant.Find(mid);
                    if (type.PlanRange.Contains("months")) { merchant.UnitAvailable =99999;
                        merchant.Expdate = DateTime.Now.Date.AddMonths(Convert.ToInt16(type.PlanRange.Split('-')[0]));
                      
                        var students = db.C_tblStudentReg.Where(o => o.MerchantID == mid).ToList();
                        students.ForEach(o => o.Unit = 99999);
                     
                        db.SaveChanges();
                    }
                    else merchant.UnitAvailable += Unit;


                    db.C_tblMerchant.Attach(merchant);
                    db.Entry(merchant).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    var m = db.C_tblMarketer.Where(o => o.referralcode.ToLower() == merchant.RefCode.ToLower()).FirstOrDefault();
                    if (m != null) {
                        var paylist = new MarketerPaylist();
                        paylist.commission = (decimal?)type.PlanAmount*Unit * (decimal)0.1;
                        paylist.transport = (decimal?)type.PlanAmount *Unit* (decimal)0.05;
                        paylist.subdate = paymentObj.SubscriptionDate;
                        paylist.marketerid = m.MarketerID;
                       // paylist.transport = 0;
                        paylist.usertype = 2;
                        paylist.userid = merchant.MerchantID;
                        db.MarketerPaylists.Add(paylist);
                        db.SaveChanges();
                    }
                    if (type.PlanRange.Contains("months"))
                    {
                        var body = "<p>Hello " + merchant.CenterName + " </p><p>Your Subcription for the " + type.PlanType + " was successful. Your institution now has unlimited access to the Exam module for the next " + type.PlanRange + " </p>";
                        body += "<p>Your access Expiry:" + DateTime.Now.Date.AddMonths(Convert.ToInt16(type.PlanRange.Split('-')[0])).ToLongDateString() + "</p> <br><br> Regards,<br>Admin ExamSpur";
                        var a = sendmail.Sendmail(merchant.Email, body, "Subscription Notification", "Subscription", "Successful");

                    }
                    else {
                        var body = "<p>Hello " + merchant.CenterName + " </p><p>Your Subcription for the " + type.PlanType + " was successful. Your institution now has "+merchant.UnitAvailable +" units </p>";
                        body += " <br><br> Regards,<br>Admin ExamSpur";
                        var a = sendmail.Sendmail(merchant.Email, body, "Subscription Notification", "Subscription", "Successful");


                    }
                    return new { value = 1, subId = paymentObj.SubscriptionID };
                }
            }
            catch (Exception ex)
            {
                return new { value = 0, subId = "Error Occured!!. Please try again" };
            }

        }
    }
}