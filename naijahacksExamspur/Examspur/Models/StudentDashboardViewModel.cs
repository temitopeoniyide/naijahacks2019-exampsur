using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExamSpur.Models
{
    public class StudentDashboardViewModel
    {
        public topInfo topInfo { get; set; }
        public List<distinctExamTaken> distinctExam { get; set; }
        public List<ExamDetails> ExamDetails { get; set; }

    }
    public class topInfo {
        public int? unitAvail { get; set; }
        public long? merchantid { get; set; }
        public string merchantName { get; set; }
        public int noOfExams { get; set; }

    }
    public class distinctExamTaken {
        public int? examid { get; set; }
        public string name { get; set; }
    }
    public class ExamDetails
    {
        public string examid { get; set; }
        public string examdate { get; set; }
        public string score { get; set; }
    }
    public static class getdashinfo
    {
        public static StudentDashboardViewModel getdashboardInfo()
        {
            try
            {

                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    var userid = (long)HttpContext.Current.Session["userid"];
                    var model = (from d in db.C_tblStudentReg

                                 where d.StudentID == userid
                                 select new topInfo
                                 {
                                     unitAvail =d.Unit,
                                     merchantName = "",
                                     merchantid = d.MerchantID,
                                     noOfExams = db.C_tblExamTaken.Where(o => o.StudentID == userid && o.Status == 2).Count()
                                 }).FirstOrDefault();
                    model.merchantName = (model.merchantid == null) ? "" : db.C_tblMerchant.Find(model.merchantid).CenterName;
                    var model2 = (from d in db.C_tblScore
                                  join e in db.C_tblExamType on d.ExamID equals e.ExamID
                                  where d.StudentID == userid
                                  select new distinctExamTaken { examid = d.ExamID, name = e.ExamName }).Distinct().ToList();
                    var mode = ((model2.Count == 0) ? 0 : model2.ToList()[0].examid);
                    var model3 = (from c in db.C_tblScore join d in db.C_tblExamTaken on c.ExamCode equals d.ExamTakenID where c.StudentID == userid
                                  where c.ExamID == mode select new ExamDetails { examid = c.ExamCode.ToString(), examdate = d.StartTimestamp.Value.ToString(), score = c.Score.ToString() }
                                 ).ToList();
                    var a = new StudentDashboardViewModel();
                    a.distinctExam = model2;
                    a.ExamDetails = model3;
                    a.topInfo = model;
                    return a;
                }
            }
            catch (Exception Ex)
            {
                return null;

            }

        }

        public static object getexam(int examid)
        {
            try
            {

                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    var userid = (long)HttpContext.Current.Session["userid"];
                    var model3 = (from c in db.C_tblScore
                                  join d in db.C_tblExamTaken on c.ExamCode equals d.ExamTakenID
                                  where c.StudentID == userid
                                  where c.ExamID == examid
                                  select new ExamDetails { examid = c.ExamCode.ToString(), examdate = d.StartTimestamp.Value.ToString(), score = c.Score.ToString() }
                                ).ToList();

                    return model3;
                }
            }
            catch (Exception ex) {
                return null;
            }
        }
    }
    public class MerchantDashboardViewModel
    {
        public topInfoMerchant topInfo { get; set; }
        public List<distinctExamTakenMerchant> distinctExam { get; set; }
        public List<summaryTableMerchant> ExamDetails { get; set; }

    }


    public class topInfoMerchant
    {
        public string unitAvail { get; set; }
        public long? merchantid { get; set; }
        public int totalExamTaken { get; set; }
        public int totalStudent { get; set; }
        public DateTime? expdate { get; set; }
        public string logo { get; set; }

    }
    public class distinctExamTakenMerchant
    {
        public int? examid { get; set; }
        public string name { get; set; }
    }
    public class summaryTableMerchant
    {
        public string Sclass { get; set; }
        public int aboveAvg { get; set; }
        public int Avg { get; set; }
        public int belowAvg { get; set; }
        public int? classid { get; set; }
        public int? examid { get; set; }
        public summaryTableMerchant(string sclass, int aboveavg, int avg, int belowavg, int? classid, int? examid)
        {
            this.Sclass = sclass;
            aboveAvg = aboveavg;
            Avg = avg;
            belowAvg = belowavg;
            this.classid = classid;
            this.examid = examid;

        }
    }
    public class Classgroup
    {
        public int? classId { get; set; }
        public int count { get; set; }
    }
    public static class getdashinfoMerchant
    {
        public static MerchantDashboardViewModel getdashboardinfo()
        {
            try
            {
                var summary = new List<summaryTableMerchant>();
                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var userid = Convert.ToInt64(HttpContext.Current.Session["merchantid"]);
                    var m = db.C_tblMerchant.Find(userid);
                    var listofstudent = db.C_tblStudentReg.Where(o => o.MerchantID == userid);
                    var model = new topInfoMerchant();


                    model.unitAvail = (m.UnitAvailable == 99999) ? "Unlimited" : m.UnitAvailable.ToString();
                    model.expdate = m.Expdate;
                    model.merchantid = userid;
                    model.totalExamTaken = (from f in db.C_tblExamTaken join e in listofstudent on f.StudentID equals e.StudentID join g in db.C_tblMerchant on e.MerchantID equals g.MerchantID select new { f.ExamTakenID }).Count();

                    model.totalStudent = listofstudent.Where(o => o.MerchantID == userid).Count();
                    model.logo = m.logo;
                    model.expdate = m.Expdate;


                    var model2 = (from d in db.C_tblScore
                                  join e in db.C_tblExamType on d.ExamID equals e.ExamID
                                  join f in listofstudent on d.StudentID equals f.StudentID
                                  where f.MerchantID == userid
                                  select new distinctExamTakenMerchant { examid = d.ExamID, name = e.ExamName }).Distinct().ToList();
                    var mode = ((model2.Count == 0) ? 0 : model2.ToList()[0].examid);
                    var model3 = (from c in db.C_tblStudentReg
                                  where c.MerchantID == userid
                                  join e in db.C_tblExamTaken on c.StudentID equals e.StudentID
                                  where e.ExamID == mode
                                  group c by new { c.ClassID } into g
                                  select new Classgroup
                                  {

                                      classId = g.Key.ClassID,
                                      count = g.Count()
                                  }
                                 ).ToList();
                    if (model3.Count() > 0)
                    {
                        foreach (var s in model3)
                        {
                            if (mode == 1000)
                            {
                                var f = (from student in db.C_tblStudentReg join score in db.C_tblScore on student.StudentID equals score.StudentID where student.ClassID == s.classId where score.ExamID == mode select new { score = score.Score });
                                summary.Add(new summaryTableMerchant(sclass: db.C_tblclass.Find(s.classId).Class,
                                    aboveavg: (f.Where(o => o.score >= 250).Count() / s.count * 100),
                                   belowavg: (f.Where(o => o.score < 200).Count() / s.count * 100),
                                       avg: Convert.ToInt16(f.Where(o => o.score > 200 && o.score < 250).Count() / s.count * 100),
                                       classid: s.classId,
                                       examid: mode

                                    ));
                            }
                            else if (mode == 1002)
                            {
                                var f = (from student in db.C_tblStudentReg join score in db.C_tblScore on student.StudentID equals score.StudentID where student.ClassID == s.classId where score.ExamID == mode select new { score = score.Score });
                                summary.Add(new summaryTableMerchant(sclass: db.C_tblclass.Find(s.classId).Class,
                                    aboveavg: (f.Where(o => o.score >= 35).Count() / s.count * 100),
                                   belowavg: (f.Where(o => o.score < 25).Count() / s.count * 100),
                                       avg: Convert.ToInt16(f.Where(o => o.score > 25 && o.score < 35).Count() / s.count * 100),
                                       classid: s.classId,
                                       examid: mode

                                    ));


                            }
                        }
                    }
                    else
                    {
                        summary = null;
                    }
                    var a = new MerchantDashboardViewModel();
                    a.topInfo = model;
                    a.distinctExam = model2;
                    a.ExamDetails = summary;
                    return a;
                }
            }
            catch (Exception Ex)
            {
                return null;

            }

        }
        public static MerchantDashboardViewModel getdashboardinfo(int? examid)
        {
            try
            {
                var summary = new List<summaryTableMerchant>();
                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var userid = Convert.ToInt64(HttpContext.Current.Session["merchantid"]);
                    var m = db.C_tblMerchant.Find(userid);
                    var listofstudent = db.C_tblStudentReg.Where(o => o.MerchantID == userid);
                    var model = new topInfoMerchant();


                    model.unitAvail = (m.UnitAvailable == 99999) ? "Unlimited" : m.UnitAvailable.ToString();
                    model.expdate = m.Expdate;
                    model.merchantid = userid;
                    model.totalExamTaken = (from f in db.C_tblExamTaken join e in listofstudent on f.StudentID equals e.StudentID join g in db.C_tblMerchant on e.MerchantID equals g.MerchantID select new { f.ExamTakenID }).Count();

                    model.totalStudent = listofstudent.Where(o => o.MerchantID == userid).Count();
                    model.logo = m.logo;
                    model.expdate = m.Expdate;


                    var mode = examid;
                    var model3 = (from c in db.C_tblStudentReg
                                  where c.MerchantID == userid
                                  join e in db.C_tblExamTaken on c.StudentID equals e.StudentID
                                  where e.ExamID == mode
                                  group c by new { c.ClassID } into g
                                  select new Classgroup
                                  {

                                      classId = g.Key.ClassID,
                                      count = g.Count()
                                  }
                                 ).ToList();
                    if (model3.Count() > 0)
                    {
                        foreach (var s in model3)
                        {
                            if (mode == 1000)
                            {
                                var f = (from student in db.C_tblStudentReg join score in db.C_tblScore on student.StudentID equals score.StudentID where student.ClassID == s.classId where score.ExamID == mode select new { score = score.Score });
                                summary.Add(new summaryTableMerchant(sclass: db.C_tblclass.Find(s.classId).Class,
                                    aboveavg: (f.Where(o => o.score >= 250).Count() / s.count * 100),
                                   belowavg: (f.Where(o => o.score < 200).Count() / s.count * 100),
                                       avg: Convert.ToInt16(f.Where(o => o.score > 200 && o.score < 250).Count() / s.count * 100),
                                       classid: s.classId,
                                       examid:mode

                                    ));
                            }
                            else if (mode == 1002)
                            {
                                var f = (from student in db.C_tblStudentReg join score in db.C_tblScore on student.StudentID equals score.StudentID where student.ClassID == s.classId where score.ExamID == mode select new { score = score.Score });
                                summary.Add(new summaryTableMerchant(sclass: db.C_tblclass.Find(s.classId).Class,
                                    aboveavg: (f.Where(o => o.score >= 35).Count() / s.count * 100),
                                   belowavg: (f.Where(o => o.score < 25).Count() / s.count * 100),
                                       avg: Convert.ToInt16(f.Where(o => o.score > 25 && o.score < 35).Count() / s.count * 100),
                                       classid: s.classId,
                                       examid:mode

                                    ));


                            }
                        }
                    }
                    else
                    {
                        summary = null;
                    }
                    var a = new MerchantDashboardViewModel();
                    a.topInfo = model;
                    a.distinctExam = null;
                    a.ExamDetails = summary;
                    return a;
                }
            }
            catch (Exception Ex)
            {
                return null;

            }

        }

        public static object getexam(int examid)
        {
            try
            {

                using (var db = new EXAMSPURDBEntities())
                {
                    var userid = (long)HttpContext.Current.Session["userid"];
                    var model3 = (from c in db.C_tblScore
                                  join d in db.C_tblExamTaken on c.ExamCode equals d.ExamTakenID
                                  where c.StudentID == userid
                                  where c.ExamID == examid
                                  select new ExamDetails { examid = c.ExamCode.ToString(), examdate = d.StartTimestamp.Value.ToString(), score = c.Score.ToString() }
                                ).ToList();

                    return model3;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }


    public static class dettach{
    
    public static object dettachStudent()
        {
            try {
                int num = new Random().Next(1000, 9999);
                var studentid = (long)HttpContext.Current.Session["userid"];
                using (var db = new EXAMSPURDBEntities()) {

                    var student = db.C_tblStudentReg.Find(studentid);
                    student.MerchantID = null;student.secretPin = Base64Encode(num.ToString());
                    student.Unit = (student.Unit==99999)?0:student.Unit;
                    db.C_tblStudentReg.Attach(student);
                    db.Entry(student).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return new { value = 1 };
                }


            } catch (Exception ex) { return new { value = 0, msg = "Something went wrong. we are working to fix this at the moment. Try again shorly" }; }


        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
    

    }