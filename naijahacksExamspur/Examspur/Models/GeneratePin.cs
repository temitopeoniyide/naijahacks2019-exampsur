using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace ExamSpur.Models
{
    public class PinBatch {
        public int? batchid { get; set; }
        public DateTime gendate { get; set; }
        public int count { get; set; }
        public int used { get; set; }
        public decimal? paid { get; set; }
        public int remitted { get; set; }
    }
    public static  class GeneratePin
    {
        public static object generatePinPostPaid()
        {
            try
            {
                using (var db = new EXAMSPURDBEntities())
                {
                    var nw = DateTime.Now;
                ly: var batchid = new Random().Next(100000, 999999);
                    if (db.tblPinSubscriptions.Where(o => o.BatchId == batchid).Count() != 0) goto ly;
                    var pins = new List<tblPinSubscription>();
                    for (int i = 0; i < 12; i++)
                    {
                    ln: var pin = new Random().Next(100000, 999999).ToString() + new Random().Next(100000, 999999).ToString();
                        if (db.tblPinSubscriptions.Where(o => o.PinNumber == pin).Count() != 0) goto ln;
                        if (pins.Where(o => o.PinNumber == pin).Count() != 0) goto ln;

                        pins.Add(new tblPinSubscription
                        {

                            BatchId = batchid,
                            DateGenerated = nw,
                            PinNumber = pin,
                            PinStatus = false,
                            Username = HttpContext.Current.Session["email"].ToString(),

                        });

                    }
                    db.tblPinSubscriptions.AddRange(pins);
                    db.SaveChanges();
                    return new { value = 1 };
                }

            }

            catch (Exception ex)
            {

                return new { value = 0, msg = "Something went wrong Try again Later" };
            }


        }
        public static string generatePinAtso(int qty)
        {
            try {
using(var db = new EXAMSPURDBEntities())
                {
                    var nw = DateTime.Now;
                ly: var batchid = new Random().Next(100000, 999999);
                    if (db.tblPinSubscriptions.Where(o => o.BatchId == batchid).Count() != 0) goto ly;
                    var pins = new List<tblPinSubscription>();
                    for (int i=0; i < qty; i++)
                    {
                      ln:  var pin = new Random().Next(100000, 999999).ToString()+ new Random().Next(100000, 999999).ToString();
                        if (db.tblPinSubscriptions.Where(o => o.PinNumber == pin).Count() != 0) goto ln;
                        if (pins.Where(o => o.PinNumber == pin).Count() != 0) goto ln;

                        pins.Add(new tblPinSubscription {

                            BatchId = batchid,
                            DateGenerated = nw,
                            PinNumber = pin,
                             PinStatus=false,
                               Username=HttpContext.Current.Session["email"].ToString(),

                        });

                    }
                    db.tblPinSubscriptions.AddRange(pins);
                    db.SaveChanges();
                    return  batchid.ToString();
                }

            }

            catch(Exception ex)
            {

                return "0";
            }
        }
        public static List<PinBatch> loadPinBatchByUsername(string username)
        {
            try
            {
                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                 l2:   var model = (from a in db.tblPinSubscriptions where a.Username == username group a by new { a.BatchId, a.DateGenerated } into g select new PinBatch {
                        batchid = g.Key.BatchId,
                        gendate = g.Key.DateGenerated,
                        count = g.Count(),
                        used = g.Where(o => o.PinStatus == true).Count(),
                        paid= db.C_tblSubscription.Where(O => O.Username == g.Key.BatchId.ToString()&& O.paymentStatus==true).ToList().Sum(o => o.AmountPaid),
                        remitted = (db.C_tblSubscription.Where(O => O.Username == g.Key.BatchId.ToString()&& O.paymentStatus==true).ToList() .Sum(o=>o.AmountPaid) == 6000) ? 1 : 0
                    }).ToList();
                    if(model.Count==0 || model.Where(o => o.remitted == 0).Count() == 0)
                    {
                        generatePinPostPaid();
                        goto l2;
                    }
                                      return model;
                }

            }

            catch (Exception ex)
            {

                return null;
            }



        }
        public static List<PinBatch> loadPinBatchByUsernameAdmin(string username)
        {
            try
            {
                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                l2: var model = (from a in db.tblPinSubscriptions
                                 where a.Username == username
                                 group a by new { a.BatchId, a.DateGenerated } into g
                                 select new PinBatch
                                 {
                                     batchid = g.Key.BatchId,
                                     gendate = g.Key.DateGenerated,
                                     count = g.Count(),
                                     used = g.Where(o => o.PinStatus == true).Count(),
                                     paid = db.C_tblSubscription.Where(O => O.Username == g.Key.BatchId.ToString() && O.PlanID==107 && O.paymentStatus == true).ToList().Sum(o => o.AmountPaid),
                                     remitted = (db.C_tblSubscription.Where(O => O.Username == g.Key.BatchId.ToString() && O.paymentStatus == true).ToList().Sum(o => o.AmountPaid) == 6000) ? 1 : 0
                                 }).ToList();
                    
                    return model;
                }

            }

            catch (Exception ex)
            {

                return null;
            }



        }
        public static List<PinInfo> loadPinByBatch(int batchid)
        {
            try
            {
                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var model =(from o in db.tblPinSubscriptions where  o.BatchId == batchid where  o.PinStatus == false select new PinInfo {PinNumber= o.PinNumber}).ToList();
                       
                    return model;
                }

            }

            catch (Exception ex)
            {

                return null;
            }



        }
        public static object RemitPay(int batchid,decimal amount, int unit , string paymentRef)
        {
            try
            {
                using (var db = new EXAMSPURDBEntities())
                {
                    var pay = new C_tblSubscription();
                    pay.AmountPaid = amount;
                    pay.paymentRef = paymentRef;
                    pay.paymentStatus = true;
                    pay.Status = false;
                    pay.SubscriptionDate = DateTime.Now;
                    pay.Unit = unit;
                    pay.Username = batchid.ToString();
                    pay.PlanID = 107;
                    db.C_tblSubscription.Add(pay);
                    db.SaveChanges();

                    var paid = db.C_tblSubscription.Where(O => O.Username == batchid.ToString() && O.paymentStatus == true).ToList().Sum(o => o.AmountPaid);
                    if (paid == 6000)
                    {
                        generatePinPostPaid();
                    }
                    return new { value = 1};
                }

            }

            catch (Exception ex)
            {

                return new {value=0,msg="Something Went Wrong.Try Again later" };
            }



        }
        public static object PayAtso(decimal amount, int unit, string paymentRef)
        {
            try
            {
                using (var db = new EXAMSPURDBEntities())
                {
                    var pay = new C_tblSubscription();
                    pay.AmountPaid = amount;
                    pay.paymentRef = paymentRef;
                    pay.paymentStatus = true;
                    pay.Status = false;
                    pay.SubscriptionDate = DateTime.Now;
                    pay.Unit = unit;
                    pay.Username = "";
                    pay.PlanID = 3;
                    db.C_tblSubscription.Add(pay);
                    db.SaveChanges();


                lv: var resp = generatePinAtso(unit);
                    if (resp == "0") goto lv;
                            pay.Username = resp;
                    db.C_tblSubscription.Attach(pay);
                    db.Entry(pay).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return new { value = 1,batchid=resp };
                }

            }

            catch (Exception ex)
            {

                return new { value = 0, msg = "Something Went Wrong.Try Again later" };
            }



        }
        public static object PayCenter(decimal amount, int unit, string paymentRef)
        {
            try
            {
                using (var db = new EXAMSPURDBEntities())
                {
                    var marketerid = (Guid)HttpContext.Current.Session["userid"];

                    var pay = new C_tblSubscription();
                    pay.AmountPaid = amount;
                    pay.paymentRef = paymentRef;
                    pay.paymentStatus = true;
                    pay.Status = false;
                    pay.SubscriptionDate = DateTime.Now;
                    pay.Unit = unit;
                    pay.Username ="";
                    pay.PlanID = 107;
                    pay.AgentStatus = false;
                    pay.RefCode = db.C_tblMarketer.Find(marketerid).referralcode;
                    db.C_tblSubscription.Add(pay);
                    db.SaveChanges();


                    lv: var resp = generatePinAtso(unit);
                    if (resp == "0") goto lv;
                    pay.Username = resp;
                    db.C_tblSubscription.Attach(pay);
                    db.Entry(pay).State = System.Data.Entity.EntityState.Modified;
                   
                    db.SaveChanges();
                    return new { value = 1, batchid = resp };
                }

            }

            catch (Exception ex)
            {

                return new { value = 0, msg = "Something Went Wrong.Try Again later" };
            }



        }

    }

    public class PinInfo
    {

        public string PinNumber { get; set; }
    }

    public class resp
    {
        public int value { get; set; }

        public int batchid { get; set; }
    }
}