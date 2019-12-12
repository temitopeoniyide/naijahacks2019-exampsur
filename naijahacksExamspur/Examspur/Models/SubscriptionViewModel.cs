using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace ExamSpur.Models
{
    public class SubscriptionViewModel
    {
        public int Usertype { get; set; }
        public SubscriptionViewModel(int usertype) {
            this.Usertype = usertype;
        }


        public object getSubscriptions() {
            try {
               
                using (var db = new EXAMSPURDBEntities()) {
                    db.Configuration.ProxyCreationEnabled = false;
                    var subs = db.C_tblSubscriptionPlan.Where(o => o.UserType == Usertype).ToList();
                    return new { value = 1, resp = subs };
                }
            }
            catch (Exception ex)
            {
                return new { value = 0, resp = ex.Message };
            }
        }

    }
    public class NewPayment{
        public int PlanId { get; set; }
        public NewPayment(int planid)
        {
            this.PlanId = planid;
        }
        public object saveNewPayment() {
            try {
                using (var db = new EXAMSPURDBEntities()) {
                    db.Configuration.ProxyCreationEnabled = false;
                    var userid = Convert.ToInt64(HttpContext.Current.Session["userid"]);
                    if (db.C_tblStudentReg.Find(userid).Unit == 99999) return new { value = 2, subId = "Your are already on an unlimited Plan. You cannt subscribe" };
                    var paymentObj = new C_tblSubscription();
                    paymentObj.StudentID = Convert.ToInt64(HttpContext.Current.Session["userid"]);
                    paymentObj.paymentStatus = false;
                    paymentObj.PlanID = (short?)PlanId;
                    paymentObj.SubscriptionDate = DateTime.Now;
                    db.C_tblSubscription.Add(paymentObj);
                    db.SaveChanges();
                    var a = db.C_tblSubscriptionPlan.Find(PlanId);
                    return new { value = 1, subId = paymentObj.SubscriptionID, amount=a.PlanAmount, min=a.PlanRange.Split('-')[0], max = a.PlanRange.Split('-')[0],name= a.PlanType };
                }
            } catch (Exception ex) {
                return new { value = 0, subId = "Error Occured!!. Please try again" };
            }

        }

      
    }
    public class UpdatePayment
    {
        public int SubId { get; set; }
        public String PaymentRef { get; set; }
        public UpdatePayment(int subid,String paymentref)
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
                    var type = db.C_tblSubscriptionPlan.Find(paymentObj.PlanID);
                    paymentObj.paymentRef = PaymentRef;
                    paymentObj.paymentStatus = true;
                    paymentObj.SubscriptionDate = DateTime.Now;
                    paymentObj.Status = false;
                    paymentObj.AmountPaid = type.PlanAmount;
                    db.C_tblSubscription.Attach(paymentObj);
                    db.Entry(paymentObj).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    var plan = db.C_tblSubscriptionPlan.Find(paymentObj.PlanID);
                    var unit = plan.Unit;
                    var user = db.C_tblStudentReg.Find((long?)HttpContext.Current.Session["userid"]);
                    user.Unit += unit;
                    db.C_tblStudentReg.Attach(user);
                    db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                  db.SaveChanges();

                    var m = db.C_tblMarketer.Where(o => o.referralcode.ToLower() == user.RefCode.ToLower()).FirstOrDefault();
                    if (m != null)
                    {
                        var paylist = new MarketerPaylist();
                        paylist.commission = (decimal?)plan.PlanAmount * (decimal)0.1;
                        paylist.subdate = paymentObj.SubscriptionDate;
                        paylist.marketerid = m.MarketerID;
                        paylist.transport = 0;
                        paylist.usertype = 1;
                        paylist.userid = user.StudentID;
                        db.MarketerPaylists.Add(paylist);
                       db.SaveChanges();
                    }
                    HttpContext.Current.Session["unitAvailable"] = user.Unit;
                    //var STUID = (long?) HttpContext.Current.Session["userid"];

                    if (DateTime.Now.Month == 2 && DateTime.Now.Year == 2019)
                    {
                        using (var db2 = new NewExamspurDBEntities2())
                        {
                            if (db2.tblNewStudentRecords.Where(o => o.Email == user.Email).FirstOrDefault() != null) return new { value = 1, subId = paymentObj.SubscriptionID };

                            var contest = new tblNewStudentRecord();

                            contest.StudentID = user.StudentID;
                            contest.FullName = user.FirstName + "     " + user.LastName;
                            contest.Password = user.Password;//Encryptor.EncodePassword(Password, ConfigurationManager.AppSettings["Salt"]);
                            contest.Email = user.Email;
                            contest.PhoneNo = user.Phone;
                            contest.Status = true;
                            contest.Unit = 1;

                            var Name = user.FirstName + "     " + user.LastName;
                            var emails = user.Email;
                            db2.tblNewStudentRecords.Add(contest);
                            db2.SaveChanges();
                            var body = "<p style=\"text-align:justify\">Hello " + user.FirstName + ", <br />Congratulations, You are now qualified to participate in the Examspur Contest 2019 coming up on the 2nd of March 2019. <br /> <br />Welcome onboard, we wish you success ahead of the Contest. <br /> <br />Click <a href=\"https://examspur.com/Admin/LoginExamspurContest\">here</a> to participate in the contest when the Portal is open<br /> <br /><br/><br>For more informtion on the contest click <a href=\"https://www.examspur.com/contest/examspurcontest2019\">here</a><br> Regards, <br /> Admin ExamSpur. </p>";
                            var mailresp = sendmail.Sendmail(user.Email, body, "Examspur Notification", "Subscription", "Successful");

                        }


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
    public class SubscribeByPinNumber
    {

        public Int64 studentId { get; set; }
        public string pinnumber { get; set; }

        public SubscribeByPinNumber(string PinNumber, Int64 studentid)
        {

            this.pinnumber = PinNumber;
            this.studentId = studentid;
        }
        public object CheckPin()
        {
            try
            {
                using (EXAMSPURDBEntities db = new EXAMSPURDBEntities())
                {

                    db.Configuration.LazyLoadingEnabled = false;
                    db.Configuration.ProxyCreationEnabled = false;

                    var Pin = db.tblPinSubscriptions.Where(o => o.PinNumber == pinnumber).FirstOrDefault();
                    if (Pin != null)
                    {
                        if (Pin.PinStatus == true) return new { value = 0, msg = "Pin Number Already Used" };

                        var userid = Convert.ToInt64(HttpContext.Current.Session["userid"]);
                        if (db.C_tblStudentReg.Find(userid).Unit == 99999) return new { value = 2, msg = "Your are already on an unlimited Plan. You cannt subscribe" };
                        var pinObj = new C_tblSubscription();
                        pinObj.StudentID = Convert.ToInt64(HttpContext.Current.Session["userid"]);
                        pinObj.paymentStatus = true;
                        pinObj.PlanID = 3;
                        pinObj.Unit = 5;
                        pinObj.paymentRef = pinnumber;
                        pinObj.SubscriptionDate = DateTime.Now;
                        db.C_tblSubscription.Add(pinObj);
                        db.SaveChanges();
                        var user2 = db.C_tblStudentReg.Find(studentId);
                        user2.Unit += 5;
                        db.C_tblStudentReg.Attach(user2);
                        db.Entry(user2).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        HttpContext.Current.Session["unitAvailable"] = user2.Unit;



                        Pin.PinStatus = true;
                        db.tblPinSubscriptions.Attach(Pin);
                        db.Entry(Pin).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        if(DateTime.Now.Month == 2 && DateTime.Now.Year==2019)
                    {
                            using (var db2 = new NewExamspurDBEntities2())
                            {
                                if (db2.tblNewStudentRecords.Where(o => o.Email == user2.Email).FirstOrDefault() != null) return new { value = 1, msg = "Subscription Successful" };

                                var contest = new tblNewStudentRecord();

                                contest.StudentID = user2.StudentID;
                                contest.FullName = user2.FirstName + "     " + user2.LastName;
                                contest.Password = user2.Password;//Encryptor.EncodePassword(Password, ConfigurationManager.AppSettings["Salt"]);
                                contest.Email = user2.Email;
                                contest.PhoneNo = user2.Phone;
                                contest.Status = true;
                                contest.Unit = 1;

                                var Name = user2.FirstName + "     " + user2.LastName;
                                
                                db2.tblNewStudentRecords.Add(contest);
                                db2.SaveChanges();
                                var body = "<p style=\"text-align:justify\">Hello " + user2.FirstName + ", <br />Congratulations, You are now qualified to participate in the Examspur Contest 2019 coming up on the 2nd of March 2019. <br /> <br />Welcome onboard, we wish you success ahead of the Contest. <br /> <br />Click <a href=\"https://examspur.com/Admin/LoginExamspurContest\">here</a> to participate in the contest when the Portal is open<br /> <br /><br/><br>For more informtion on the contest click <a href=\"https://www.examspur.com/contest/examspurcontest2019\">here</a><br> Regards, <br /> Admin ExamSpur. </p>";
                                var mailresp = sendmail.Sendmail(user2.Email, body, "Examspur Notification", "Subscription", "Successful");

                            }





                        }
                        return new { value = 1, msg = "Subscription Successful" };
                    }

                    else { return new { value = 0, msg = "Oooops!!! Invalid Pin Number." }; }
                }
            }
            catch (Exception ex)
            {
                return new { value = 0, msg = "Error Occured!!! Try again Later." };
            }
        }



    }
}