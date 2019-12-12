using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Threading;
using ExamSpur.Models;
using System.ComponentModel;

namespace ExamSpur
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static DateTime start = DateTime.Now;
        public static DateTime end = DateTime.Now.AddDays(-2);
        public static DateTime end2 = DateTime.Now.AddDays(-2);

        Thread t1 = new Thread(new ThreadStart(sendmails));
        Thread t2 = new Thread(new ThreadStart(notifyexp));
        protected void Application_Start()
        {
            t1.Start();
            t2.Start();
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            //ErrorLogService.LogError(ex);
            //if (ExceptionContainsErrorCode(ex, 404))
            //{

            //    Response.Redirect("/Home/ErrorPage");
            //}
            //else {
            //    Response.Redirect("/Home/Error500");
            //}
        }
        //protected void Application_BeginRequest()
        //{
        //    if (!Context.Request.IsSecureConnection)
        //        Response.Redirect(Context.Request.Url.ToString().Replace("http:", "https:"));
        //}
        private bool ExceptionContainsErrorCode(Exception e, int ErrorCode)
        {
            Win32Exception winEx = e as Win32Exception;
            if (winEx != null && ErrorCode == winEx.ErrorCode)
                return true;

            if (e.InnerException != null)
                return ExceptionContainsErrorCode(e.InnerException, ErrorCode);

            return false;
        }
        //common service to be used for logging errors
        public static class ErrorLogService
        {
            public static void LogError(Exception ex)
            {
                //Email developers, call fire department, log to database etc.
            }
        }
       
        public static void sendmails()
        {
            while (true)
            {
                try
                {
                    if (DateTime.Now.Minute - start.Minute >= 1)
                    {
                        using (var db = new EXAMSPURDBEntities())
                        {
                            var dates = DateTime.Now.Date.AddDays(-1);
                            var emailtosend = db.tableMailAlerts.Where(o => o.status == 0 && dates <= (o.dropTimestamp.Value)).ToList();
                            foreach (var item in emailtosend)
                            {
                                var resp = new ExamSpurNotification(item.subject, item.body, item.receiverEmail, item.header1, item.header2);
                                var status = resp.sendEmail();
                                if ((bool)status)
                                {
                                    item.sentTimestamp = DateTime.Now;
                                    item.status = 2;
                                    db.tableMailAlerts.Attach(item);
                                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                            start = DateTime.Now;

                        }
                    }
                }
                catch (Exception ex)
                {
                    var a = ex.Message;

                }
            }
        }
        public static void notifyexp()
        {
            while (true)
            {
                if (end.Date != DateTime.Now.Date && DateTime.Now.Date.Hour == 23)
                {
                    try
                    {
                        using (var db = new EXAMSPURDBEntities())
                        {
                            var exp = db.C_tblMerchant.Where(o => o.Expdate.Value.Date <= DateTime.Now.Date).ToList();
                            foreach (var item in exp)
                            {
                                item.UnitAvailable = 0;
                                item.Expdate = null;
                                db.C_tblMerchant.Attach(item);
                                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();
                                var body = "<br /><p>Hello " + item.CenterName + ",</p><p>Your unlimited access subscription has expired. Your student will no longer have access to practise exams on ExamSpur.<br> Kindly Subscribe to restore access.</p><br><br><p>Regards,</p><p>Admin ExamSpur</p>";
                                var a = new ExamSpurNotification("Access Expiry Notification", body, item.Email, "Access", "Expiry");
                                a.sendEmail();
                                var studentAcc = db.C_tblStudentReg.Where(o => o.MerchantID == item.MerchantID).ToList();
                                studentAcc.ForEach(o => o.Unit = 0);
                                //    db.Entry(studentAcc).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();
                            }
                        };
                        end = DateTime.Now;
                    }

                    catch (Exception ex)
                    {

                    }

                }
                notify2weeks();
                Thread.Sleep(10000);
            }
        }
        public static void notify2weeks()
        {
            if (end2.Date != DateTime.Now.Date && DateTime.Now.Date.Hour == 12)
            {
                try
                {
                    using (var db = new EXAMSPURDBEntities())
                    {
                        var exp = db.C_tblMerchant.Where(o => (o.Expdate.Value.Date - DateTime.Now.Date).TotalDays == 14).ToList();
                        foreach (var item in exp)
                        {
                            var body = "<br /><p>Hello " + item.CenterName + ",</p><p>This is to notify you that your unlimited subscription will expire in <b style=\"color:#d42a2a\">2 weeks</b>, precisely on "+item.Expdate.Value.Date.ToLongDateString()+". <br>To avoid access disruption, kindly renew your subscription on or before  the expiry date.</p><br><br><p>Regards,</p><p>Admin ExamSpur</p>";
                            var a = new ExamSpurNotification("Access Expiry Notification", body, item.Email, "Prompt", "Notification!");
                            a.sendEmail();
                        }
                    }
                    end2 = DateTime.Now;
                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}
