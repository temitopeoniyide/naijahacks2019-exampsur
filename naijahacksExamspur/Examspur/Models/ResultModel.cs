using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ExamSpur.Models
{
    public class ResultModel
    {
        public string score { get; set; }
        public string startdate { get; set; }
        public string examid { get; set; }
        public string exammode { get; set; }
        public string timeused { get; set; }
    }

    public static class ResultViewModel {
        public static List<ResultModel> result() {

            var studentId = (long?)HttpContext.Current.Session["userid"];
            try {
                using (var db = new EXAMSPURDBEntities())
                {

                    db.Configuration.ProxyCreationEnabled = false;
                    var pendingExam = db.C_tblExamTaken.Where(o => o.Status == 1 && o.ExamMode == 1 &&  o.ExamID == 1000);
                    foreach (var examInfo in pendingExam)
                    {
                       var a= updateExam(examInfo);
                        var examD = db.C_tblQuestionList.Where(o => o.ExamTakenId == examInfo.ExamTakenID).ToList();
                        foreach (var item in examD)
                        {
                            var incorrectList = new List<string>();
                            int correctCount = 0;
                            var attemptedCount = 0;
                            var answerList = item.CorrectAnswerList;

                            var choosenList = item.ChoosenAnswerList;
                          
                            var qList = item.QuestionList;
                            if (choosenList == null){
                                item.NoCorrect = 0;
                                item.NoAttempted = 0;
                                for (var i = 0; i < answerList.Split(',').Length; i++)
                                {
                                    incorrectList.Add(qList.Split(',')[i] + "!" + "");
                                }
                                item.IncorrectAnswerList = String.Join(",", incorrectList);
                                Task<object> b = SubmitExam.saveExamResult(item);
                            }
                            else {
                                var listAnswer = new string[answerList.Split(',').Length];
                                // if (answerList.Split(',').Length != choosenList.Split(',').Length) {
                                for (var i = 0; i < choosenList.Split(',').Length - 1; i++)
                                {
                                    listAnswer[i] = choosenList.Split(',')[i];


                                }
                                for (var i = 0; i < answerList.Split(',').Length; i++)
                                {
                                    if (answerList.Split(',')[i] == string.Join(",", listAnswer).Split(',')[i]) correctCount++;
                                    if (answerList.Split(',')[i] == "F") correctCount++;
                                    else incorrectList.Add(qList.Split(',')[i] + "!" + string.Join(",", listAnswer).Split(',')[i]);
                                    if (string.Join(",", listAnswer).Split(',')[i] != "") attemptedCount++;
                                }
                                item.NoCorrect = correctCount;
                                item.NoAttempted = attemptedCount;
                                item.IncorrectAnswerList = String.Join(",", incorrectList);
                                Task<object> b = SubmitExam.saveExamResult(item);
                            }
                        }
                    }
                    //var tt = ((decimal)(db.C_tblQuestionList.Where(o => o.SubjectID != 1014 && o.ExamTakenId == 21062).Sum(o => o.NoCorrect)) * (decimal)((100 * 3)) / 120);
                    var results = (from exam in db.C_tblExamTaken
                                  join result in db.C_tblQuestionList on exam.ExamTakenID equals result.ExamTakenId
                                  where exam.StudentID == studentId
                                  where exam.Status == 2 where exam.ExamID==1000
                                  orderby exam.StartTimestamp descending
                                  group result by new { exam.ExamTakenID, exam.ExamMode, exam.StartTimestamp, exam.TimeUsed, exam.TotalTime, } into g
                                  select new ResultModel
                                  {
                                      score = Math.Round(((((int)(db.C_tblQuestionList.Where(o => o.SubjectID != 1014 && o.ExamTakenId ==g.Key.ExamTakenID).Sum(o => o.NoCorrect)) * (decimal)((100 * 3)) / 120) + ((decimal)(db.C_tblQuestionList.Where(o => o.SubjectID == 1014 && o.ExamTakenId ==g.Key.ExamTakenID).FirstOrDefault().NoCorrect * 100)) / 60)),0).ToString(),
                                      examid = g.Key.ExamTakenID.ToString(),
                                      startdate = SqlFunctions.DateName("year", g.Key.StartTimestamp).Trim() + "-" +
                   SqlFunctions.StringConvert((double)g.Key.StartTimestamp.Value.Month).TrimStart() + "-" +
                   SqlFunctions.DateName("day", g.Key.StartTimestamp),
                                      timeused = (Math.Floor(((decimal)g.Key.TotalTime - (decimal)g.Key.TimeUsed) / 3600)).ToString() + ":" + (Math.Floor(((decimal)g.Key.TotalTime - (decimal)g.Key.TimeUsed) % 3600 / 60)).ToString() + ":" + (Math.Floor(((decimal)g.Key.TotalTime - (decimal)g.Key.TimeUsed) % 3600 % 60)).ToString(),
                                      exammode = g.Key.ExamMode.ToString(),
                                      
                    

                                  }).ToList();


                    foreach (var item in results.ToList()) {
                        var sc = db.C_tblScore.Where(o => o.ExamCode.ToString() == item.examid).FirstOrDefault();
                        if (sc == null)
                        {
                            var a = new C_tblScore();
                            a.StudentID = (long)studentId;
                            a.ExamCode = Convert.ToInt64(item.examid);
                            a.ExamID = db.C_tblExamTaken.Find(a.ExamCode).ExamID;
                            a.Score = Convert.ToInt32(item.score.Split('.')[0]);
                            db.C_tblScore.Add(a);
                            db.SaveChanges();
                        }
                    }
                  
                    return results.ToList();
                }

            } catch (Exception Ex) {

                return null;
            }

        }
        public static List<ResultModel> resultByTopic()
        {

            var studentId = (long?)HttpContext.Current.Session["userid"];
            try
            {
                using (var db = new EXAMSPURDBEntities())
                {

                    db.Configuration.ProxyCreationEnabled = false;
                    var pendingExam = db.C_tblExamTaken.Where(o => o.Status == 1 && o.ExamMode == 1 && o.ExamID == 1003);
                    foreach (var examInfo in pendingExam)
                    {
                        var a = updateExam(examInfo);
                        var examD = db.C_tblQuestionListTopic.Where(o => o.ExamTakenId == examInfo.ExamTakenID).ToList();
                        foreach (var item in examD)
                        {
                            var incorrectList = new List<string>();
                            int correctCount = 0;
                            var attemptedCount = 0;
                            var answerList = item.CorrectAnswerList;

                            var choosenList = item.ChoosenAnswerList;

                            var qList = item.QuestionList;
                            if (choosenList == null)
                            {
                                item.NoCorrect = 0;
                                item.NoAttempted = 0;
                                for (var i = 0; i < answerList.Split(',').Length; i++)
                                {
                                    incorrectList.Add(qList.Split(',')[i] + "!" + "");
                                }
                                item.IncorrectAnswerList = String.Join(",", incorrectList);
                                Task<object> b = SubmitExamByTopic.saveExamResult(item);
                            }
                            else
                            {
                                var listAnswer = new string[answerList.Split(',').Length];
                                // if (answerList.Split(',').Length != choosenList.Split(',').Length) {
                                for (var i = 0; i < choosenList.Split(',').Length - 1; i++)
                                {
                                    listAnswer[i] = choosenList.Split(',')[i];


                                }
                                for (var i = 0; i < answerList.Split(',').Length; i++)
                                {
                                    if (answerList.Split(',')[i] == string.Join(",", listAnswer).Split(',')[i]) correctCount++;
                                    if (answerList.Split(',')[i] == "F") correctCount++;
                                    else incorrectList.Add(qList.Split(',')[i] + "!" + string.Join(",", listAnswer).Split(',')[i]);
                                    if (string.Join(",", listAnswer).Split(',')[i] != "") attemptedCount++;
                                }
                                item.NoCorrect = correctCount;
                                item.NoAttempted = attemptedCount;
                                item.IncorrectAnswerList = String.Join(",", incorrectList);
                                Task<object> b = SubmitExamByTopic.saveExamResult(item);
                            }
                        }
                    }
                    //var tt = ((decimal)(db.C_tblQuestionList.Where(o => o.SubjectID != 1014 && o.ExamTakenId == 21062).Sum(o => o.NoCorrect)) * (decimal)((100 * 3)) / 120);
                    var results = (from exam in db.C_tblExamTaken
                                   join result in db.C_tblQuestionListTopic on exam.ExamTakenID equals result.ExamTakenId
                                   where exam.StudentID == studentId
                                   where exam.Status == 2
                                   where exam.ExamID == 1003
                                   orderby exam.StartTimestamp descending
                                   group result by new { exam.ExamTakenID, exam.ExamMode, exam.StartTimestamp, exam.TimeUsed, exam.TotalTime, } into g
                                   select new ResultModel
                                   {
                                       score = Math.Round((decimal)(db.C_tblQuestionListTopic.Where(o => o.ExamTakenId == g.Key.ExamTakenID).Sum(o => o.NoCorrect)),0).ToString(),
                                       examid = g.Key.ExamTakenID.ToString(),
                                       startdate = SqlFunctions.DateName("year", g.Key.StartTimestamp).Trim() + "-" +
                    SqlFunctions.StringConvert((double)g.Key.StartTimestamp.Value.Month).TrimStart() + "-" +
                    SqlFunctions.DateName("day", g.Key.StartTimestamp),
                                       timeused = (Math.Floor(((decimal)g.Key.TotalTime - (decimal)g.Key.TimeUsed) / 3600)).ToString() + ":" + (Math.Floor(((decimal)g.Key.TotalTime - (decimal)g.Key.TimeUsed) % 3600 / 60)).ToString() + ":" + (Math.Floor(((decimal)g.Key.TotalTime - (decimal)g.Key.TimeUsed) % 3600 % 60)).ToString(),
                                       exammode = g.Key.ExamMode.ToString(),


  
                                   }).ToList();


                    foreach (var item in results.ToList())
                    {
                        var sc = db.C_tblScore.Where(o => o.ExamCode.ToString() == item.examid).FirstOrDefault();
                        if (sc == null)
                        {
                            var a = new C_tblScore();
                            a.StudentID = (long)studentId;
                            a.ExamCode = Convert.ToInt64(item.examid);
                            a.ExamID = db.C_tblExamTaken.Find(a.ExamCode).ExamID;
                            a.Score = Convert.ToInt32(item.score.Split('.')[0]);
                            db.C_tblScore.Add(a);
                            db.SaveChanges();
                        }
                    }

                    return results.ToList();
                }

            }
            catch (Exception Ex)
            {

                return null;
            }

        }
        public static List<ResultModel> resultForMerchant(Int64 userid)
        {

            var studentId =userid;
            try
            {
                using (var db = new EXAMSPURDBEntities())
                {

                    db.Configuration.ProxyCreationEnabled = false;
                    var pendingExam = db.C_tblExamTaken.Where(o => o.Status == 1 && o.ExamMode == 1 && o.ExamID == 1000);
                    foreach (var examInfo in pendingExam)
                    {
                        var a = updateExam(examInfo);
                        var examD = db.C_tblQuestionList.Where(o => o.ExamTakenId == examInfo.ExamTakenID).ToList();
                        foreach (var item in examD)
                        {
                            var incorrectList = new List<string>();
                            int correctCount = 0;
                            var attemptedCount = 0;
                            var answerList = item.CorrectAnswerList;

                            var choosenList = item.ChoosenAnswerList;

                            var qList = item.QuestionList;
                            if (choosenList == null)
                            {
                                item.NoCorrect = 0;
                                item.NoAttempted = 0;
                                for (var i = 0; i < answerList.Split(',').Length; i++)
                                {
                                    incorrectList.Add(qList.Split(',')[i] + "!" + "");
                                }
                                item.IncorrectAnswerList = String.Join(",", incorrectList);
                                Task<object> b = SubmitExam.saveExamResult(item);
                            }
                            else
                            {
                                var listAnswer = new string[answerList.Split(',').Length];
                                // if (answerList.Split(',').Length != choosenList.Split(',').Length) {
                                for (var i = 0; i < choosenList.Split(',').Length - 1; i++)
                                {
                                    listAnswer[i] = choosenList.Split(',')[i];


                                }
                                for (var i = 0; i < answerList.Split(',').Length; i++)
                                {
                                    if (answerList.Split(',')[i] == string.Join(",", listAnswer).Split(',')[i]) correctCount++;
                                    if (answerList.Split(',')[i] == "F") correctCount++;
                                    else incorrectList.Add(qList.Split(',')[i] + "!" + string.Join(",", listAnswer).Split(',')[i]);
                                    if (string.Join(",", listAnswer).Split(',')[i] != "") attemptedCount++;
                                }
                                item.NoCorrect = correctCount;
                                item.NoAttempted = attemptedCount;
                                item.IncorrectAnswerList = String.Join(",", incorrectList);
                                Task<object> b = SubmitExam.saveExamResult(item);
                            }
                        }
                    }
                    //var tt = ((decimal)(db.C_tblQuestionList.Where(o => o.SubjectID != 1014 && o.ExamTakenId == 21062).Sum(o => o.NoCorrect)) * (decimal)((100 * 3)) / 120);
                    var results = (from exam in db.C_tblExamTaken
                                   join result in db.C_tblQuestionList on exam.ExamTakenID equals result.ExamTakenId
                                   where exam.StudentID == studentId
                                   where exam.Status == 2
                                   where exam.ExamID == 1000
                                   orderby exam.StartTimestamp descending
                                   group result by new { exam.ExamTakenID, exam.ExamMode, exam.StartTimestamp, exam.TimeUsed, exam.TotalTime, } into g
                                   select new ResultModel
                                   {
                                       score = Math.Round(((((int)(db.C_tblQuestionList.Where(o => o.SubjectID != 1014 && o.ExamTakenId == g.Key.ExamTakenID).Sum(o => o.NoCorrect)) * (decimal)((100 * 3)) / 120) + ((decimal)(db.C_tblQuestionList.Where(o => o.SubjectID == 1014 && o.ExamTakenId == g.Key.ExamTakenID).FirstOrDefault().NoCorrect * 100)) / 60)), 0).ToString(),
                                       examid = g.Key.ExamTakenID.ToString(),
                                       startdate = SqlFunctions.DateName("year", g.Key.StartTimestamp).Trim() + "-" +
                    SqlFunctions.StringConvert((double)g.Key.StartTimestamp.Value.Month).TrimStart() + "-" +
                    SqlFunctions.DateName("day", g.Key.StartTimestamp),
                                       timeused = (Math.Floor(((decimal)g.Key.TotalTime - (decimal)g.Key.TimeUsed) / 3600)).ToString() + ":" + (Math.Floor(((decimal)g.Key.TotalTime - (decimal)g.Key.TimeUsed) % 3600 / 60)).ToString() + ":" + (Math.Floor(((decimal)g.Key.TotalTime - (decimal)g.Key.TimeUsed) % 3600 % 60)).ToString(),
                                       exammode = g.Key.ExamMode.ToString(),



                                   }).ToList();


                    foreach (var item in results.ToList())
                    {
                        var sc = db.C_tblScore.Where(o => o.ExamCode.ToString() == item.examid).FirstOrDefault();
                        if (sc == null)
                        {
                            var a = new C_tblScore();
                            a.StudentID = (long)studentId;
                            a.ExamCode = Convert.ToInt64(item.examid);
                            a.ExamID = db.C_tblExamTaken.Find(a.ExamCode).ExamID;
                            a.Score = Convert.ToInt32(item.score.Split('.')[0]);
                            db.C_tblScore.Add(a);
                            db.SaveChanges();
                        }
                    }

                    return results.ToList();
                }

            }
            catch (Exception Ex)
            {

                return null;
            }

        }
        public static List<ResultModel> resultByTopicForMerchant(Int64 userid)
        {

            var studentId = userid;
            try
            {
                using (var db = new EXAMSPURDBEntities())
                {

                    db.Configuration.ProxyCreationEnabled = false;
                    var pendingExam = db.C_tblExamTaken.Where(o => o.Status == 1 && o.ExamMode == 1 && o.ExamID == 1000);
                    foreach (var examInfo in pendingExam)
                    {
                        var a = updateExam(examInfo);
                        var examD = db.C_tblQuestionListTopic.Where(o => o.ExamTakenId == examInfo.ExamTakenID).ToList();
                        foreach (var item in examD)
                        {
                            var incorrectList = new List<string>();
                            int correctCount = 0;
                            var attemptedCount = 0;
                            var answerList = item.CorrectAnswerList;

                            var choosenList = item.ChoosenAnswerList;

                            var qList = item.QuestionList;
                            if (choosenList == null)
                            {
                                item.NoCorrect = 0;
                                item.NoAttempted = 0;
                                for (var i = 0; i < answerList.Split(',').Length; i++)
                                {
                                    incorrectList.Add(qList.Split(',')[i] + "!" + "");
                                }
                                item.IncorrectAnswerList = String.Join(",", incorrectList);
                                Task<object> b = SubmitExamByTopic.saveExamResult(item);
                            }
                            else
                            {
                                var listAnswer = new string[answerList.Split(',').Length];
                                // if (answerList.Split(',').Length != choosenList.Split(',').Length) {
                                for (var i = 0; i < choosenList.Split(',').Length - 1; i++)
                                {
                                    listAnswer[i] = choosenList.Split(',')[i];


                                }
                                for (var i = 0; i < answerList.Split(',').Length; i++)
                                {
                                    if (answerList.Split(',')[i] == string.Join(",", listAnswer).Split(',')[i]) correctCount++;
                                    if (answerList.Split(',')[i] == "F") correctCount++;
                                    else incorrectList.Add(qList.Split(',')[i] + "!" + string.Join(",", listAnswer).Split(',')[i]);
                                    if (string.Join(",", listAnswer).Split(',')[i] != "") attemptedCount++;
                                }
                                item.NoCorrect = correctCount;
                                item.NoAttempted = attemptedCount;
                                item.IncorrectAnswerList = String.Join(",", incorrectList);
                                Task<object> b = SubmitExamByTopic.saveExamResult(item);
                            }
                        }
                    }
                    //var tt = ((decimal)(db.C_tblQuestionList.Where(o => o.SubjectID != 1014 && o.ExamTakenId == 21062).Sum(o => o.NoCorrect)) * (decimal)((100 * 3)) / 120);
                    var results = (from exam in db.C_tblExamTaken
                                   join result in db.C_tblQuestionListTopic on exam.ExamTakenID equals result.ExamTakenId
                                   where exam.StudentID == studentId
                                   where exam.Status == 2
                                   where exam.ExamID == 1003
                                   orderby exam.StartTimestamp descending
                                   group result by new { exam.ExamTakenID, exam.ExamMode, exam.StartTimestamp, exam.TimeUsed, exam.TotalTime, } into g
                                   select new ResultModel
                                   {
                                       score = Math.Round((decimal)(db.C_tblQuestionListTopic.Where(o => o.ExamTakenId == g.Key.ExamTakenID).Sum(o => o.NoCorrect)), 0).ToString(),
                                       examid = g.Key.ExamTakenID.ToString(),
                                       startdate = SqlFunctions.DateName("year", g.Key.StartTimestamp).Trim() + "-" +
                    SqlFunctions.StringConvert((double)g.Key.StartTimestamp.Value.Month).TrimStart() + "-" +
                    SqlFunctions.DateName("day", g.Key.StartTimestamp),
                                       timeused = (Math.Floor(((decimal)g.Key.TotalTime - (decimal)g.Key.TimeUsed) / 3600)).ToString() + ":" + (Math.Floor(((decimal)g.Key.TotalTime - (decimal)g.Key.TimeUsed) % 3600 / 60)).ToString() + ":" + (Math.Floor(((decimal)g.Key.TotalTime - (decimal)g.Key.TimeUsed) % 3600 % 60)).ToString(),
                                       exammode = g.Key.ExamMode.ToString(),



                                   }).ToList();


                    foreach (var item in results.ToList())
                    {
                        var sc = db.C_tblScore.Where(o => o.ExamCode.ToString() == item.examid).FirstOrDefault();
                        if (sc == null)
                        {
                            var a = new C_tblScore();
                            a.StudentID = (long)studentId;
                            a.ExamCode = Convert.ToInt64(item.examid);
                            a.ExamID = db.C_tblExamTaken.Find(a.ExamCode).ExamID;
                            a.Score = Convert.ToInt32(item.score.Split('.')[0]);
                            db.C_tblScore.Add(a);
                            db.SaveChanges();
                        }
                    }

                    return results.ToList();
                }

            }
            catch (Exception Ex)
            {

                return null;
            }

        }
        public static  object  updateExam(C_tblExamTaken examInfo) {
            using (var db = new EXAMSPURDBEntities()) { 
                db.Configuration.ProxyCreationEnabled = false;
                examInfo.Status = 2;
                examInfo.EndTimestamp = DateTime.Now;
                db.C_tblExamTaken.Attach(examInfo);
                db.Entry(examInfo).State = System.Data.Entity.EntityState.Modified;
                 var a =db.SaveChanges();
                return a;
            }
        }
    }
   


    public class ResultModelT
    {
        public string score { get; set; }
        public string startdate { get; set; }
        public string examid { get; set; }
        public string exammode { get; set; }
        public string timeused { get; set; }
    }

    public class ResultViewModelT
    {
        public static List<ResultModelT> result()
        {

            var studentId = (long?)HttpContext.Current.Session["userid"];
            try
            {
                using (var db = new EXAMSPURDBEntities())
                {

                    db.Configuration.ProxyCreationEnabled = false;
                    var pendingExam = db.C_tblExamTaken.Where(o => o.Status == 1 && o.ExamMode == 3 && o.ExamID == 1000);
                    foreach (var examInfo in pendingExam)
                    {
                        var a = updateExam(examInfo);
                        var examD = db.C_tblQuestionList.Where(o => o.ExamTakenId == examInfo.ExamTakenID).ToList();
                        foreach (var item in examD)
                        {
                            var incorrectList = new List<string>();
                            int correctCount = 0;
                            var attemptedCount = 0;
                            var answerList = item.CorrectAnswerList;

                            var choosenList = item.ChoosenAnswerList;

                            var qList = item.QuestionList;
                            if (choosenList == null)
                            {
                                item.NoCorrect = 0;
                                item.NoAttempted = 0;
                                for (var i = 0; i < answerList.Split(',').Length; i++)
                                {
                                    incorrectList.Add(qList.Split(',')[i] + "!" + "");
                                }
                                item.IncorrectAnswerList = String.Join(",", incorrectList);
                                Task<object> b = SubmitExam.saveExamResult(item);
                            }
                            else
                            {
                                var listAnswer = new string[answerList.Split(',').Length];
                                // if (answerList.Split(',').Length != choosenList.Split(',').Length) {
                                for (var i = 0; i < choosenList.Split(',').Length - 1; i++)
                                {
                                    listAnswer[i] = choosenList.Split(',')[i];


                                }
                                for (var i = 0; i < answerList.Split(',').Length; i++)
                                {
                                    if (answerList.Split(',')[i] == string.Join(",", listAnswer).Split(',')[i]) correctCount++;
                                    if (answerList.Split(',')[i] == "F") correctCount++;
                                    else incorrectList.Add(qList.Split(',')[i] + "!" + string.Join(",", listAnswer).Split(',')[i]);
                                    if (string.Join(",", listAnswer).Split(',')[i] != "") attemptedCount++;
                                }
                                item.NoCorrect = correctCount;
                                item.NoAttempted = attemptedCount;
                                item.IncorrectAnswerList = String.Join(",", incorrectList);
                                Task<object> b = SubmitExam.saveExamResult(item);
                            }
                        }
                    }

                    var results = from exam in db.C_tblExamTaken
                                  join result in db.C_tblQuestionList on exam.ExamTakenID equals result.ExamTakenId
                                  where exam.StudentID == studentId
                                  where exam.Status == 2
                                  where exam.ExamID == 1000
                                  orderby exam.StartTimestamp descending
                                  group result by new { exam.ExamTakenID, exam.ExamMode, exam.StartTimestamp, exam.TimeUsed, exam.TotalTime } into g
                                  select new ResultModelT
                                  {
                                      score = ((((g.Where(o => o.SubjectID != 1014).Sum(o => o.NoCorrect)) * (100 * 3 / 120)) + (g.Where(o => o.SubjectID == 1014).FirstOrDefault().NoCorrect)) * (100 / 60)).ToString(),
                                      examid = g.Key.ExamTakenID.ToString(),
                                      startdate = g.Key.StartTimestamp.Value.ToString(),
                                      timeused = (Math.Floor(((decimal)g.Key.TotalTime - (decimal)g.Key.TimeUsed) / 3600)).ToString() + ":" + (Math.Floor(((decimal)g.Key.TotalTime - (decimal)g.Key.TimeUsed) % 3600 / 60)).ToString() + ":" + (Math.Floor(((decimal)g.Key.TotalTime - (decimal)g.Key.TimeUsed) % 3600 % 60)).ToString(),
                                      exammode = g.Key.ExamMode.ToString()

                                  };
                    return results.ToList();
                }

            }
            catch (Exception Ex)
            {

                return null;
            }

        }
        public static object updateExam(C_tblExamTaken examInfo)
        {
            using (var db = new EXAMSPURDBEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                examInfo.Status = 2;
                examInfo.EndTimestamp = DateTime.Now;
                db.C_tblExamTaken.Attach(examInfo);
                db.Entry(examInfo).State = System.Data.Entity.EntityState.Modified;
                var a = db.SaveChanges();
                return a;
            }
        }
    }
    public class ResultModelWaec
    {
        public string score { get; set; }
        public string startdate { get; set; }
        public string examid { get; set; }
        public string exammode { get; set; }
        public string timeused { get; set; }
    }
    public static class ResultViewModelWaec
    {
        public static List<ResultModelWaec> result()
        {

            var studentId = (long?)HttpContext.Current.Session["userid"];
            try
            {
                using (var db = new EXAMSPURDBEntities())
                {

                    db.Configuration.ProxyCreationEnabled = false;
                    var pendingExam = db.C_tblExamTaken.Where(o => o.Status == 1 && o.ExamMode == 1 && o.ExamID == 1002);
                    foreach (var examInfo in pendingExam)
                    {
                        var a = updateExam(examInfo);
                        var examD = db.C_tblQuestionList.Where(o => o.ExamTakenId == examInfo.ExamTakenID).ToList();
                        foreach (var item in examD)
                        {
                            var incorrectList = new List<string>();
                            int correctCount = 0;
                            var attemptedCount = 0;
                            var answerList = item.CorrectAnswerList;

                            var choosenList = item.ChoosenAnswerList;

                            var qList = item.QuestionList;
                            if (choosenList == null)
                            {
                                item.NoCorrect = 0;
                                item.NoAttempted = 0;
                                for (var i = 0; i < answerList.Split(',').Length; i++)
                                {
                                    incorrectList.Add(qList.Split(',')[i] + "!" + "");
                                }
                                item.IncorrectAnswerList = String.Join(",", incorrectList);
                                Task<object> b = SubmitExamWaec.saveExamResult(item);
                            }
                            else
                            {
                                var listAnswer = new string[answerList.Split(',').Length];
                                // if (answerList.Split(',').Length != choosenList.Split(',').Length) {
                                for (var i = 0; i < choosenList.Split(',').Length - 1; i++)
                                {
                                    listAnswer[i] = choosenList.Split(',')[i];


                                }
                                for (var i = 0; i < answerList.Split(',').Length; i++)
                                {
                                    if (answerList.Split(',')[i] == string.Join(",", listAnswer).Split(',')[i]) correctCount++;
                                    if (answerList.Split(',')[i] == "F") correctCount++;
                                    else incorrectList.Add(qList.Split(',')[i] + "!" + string.Join(",", listAnswer).Split(',')[i]);
                                    if (string.Join(",", listAnswer).Split(',')[i] != "") attemptedCount++;
                                }
                                item.NoCorrect = correctCount;
                                item.NoAttempted = attemptedCount;
                                item.IncorrectAnswerList = String.Join(",", incorrectList);
                                Task<object> b = SubmitExamWaec.saveExamResult(item);
                            }
                        }
                    }
                    //var tt = ((decimal)(db.C_tblQuestionList.Where(o => o.SubjectID != 1014 && o.ExamTakenId == 21062).Sum(o => o.NoCorrect)) * (decimal)((100 * 3)) / 120);
                    var results = (from exam in db.C_tblExamTaken
                                   join result in db.C_tblQuestionList on exam.ExamTakenID equals result.ExamTakenId
                                   where exam.StudentID == studentId
                                   where exam.Status == 2
                                   where  exam.ExamID == 1002
                                   orderby exam.StartTimestamp descending
                                   group result by new { exam.ExamTakenID, exam.ExamMode, exam.StartTimestamp, exam.TimeUsed, exam.TotalTime, } into g
                                   select new ResultModelWaec
                                   {
                                       score = Math.Round((decimal)(db.C_tblQuestionList.Where(o =>  o.ExamTakenId == g.Key.ExamTakenID).FirstOrDefault().NoCorrect * 2), 0).ToString(),
                                       examid = g.Key.ExamTakenID.ToString(),
                                       startdate = SqlFunctions.DateName("year", g.Key.StartTimestamp).Trim() + "-" +
                    SqlFunctions.StringConvert((double)g.Key.StartTimestamp.Value.Month).TrimStart() + "-" +
                    SqlFunctions.DateName("day", g.Key.StartTimestamp),
                                       timeused = (Math.Floor(((decimal)g.Key.TotalTime - (decimal)g.Key.TimeUsed) / 3600)).ToString() + ":" + (Math.Floor(((decimal)g.Key.TotalTime - (decimal)g.Key.TimeUsed) % 3600 / 60)).ToString() + ":" + (Math.Floor(((decimal)g.Key.TotalTime - (decimal)g.Key.TimeUsed) % 3600 % 60)).ToString(),
                                       exammode = g.Key.ExamMode.ToString(),



                                   }).ToList();


                    foreach (var item in results.ToList())
                    {
                        var sc = db.C_tblScore.Where(o => o.ExamCode.ToString() == item.examid).FirstOrDefault();
                        if (sc == null)
                        {
                            var a = new C_tblScore();
                            a.StudentID = (long)studentId;
                            a.ExamCode = Convert.ToInt64(item.examid);
                            a.ExamID = db.C_tblExamTaken.Find(a.ExamCode).ExamID;
                            a.Score = Convert.ToInt32(item.score.Split('.')[0]);
                            db.C_tblScore.Add(a);
                            db.SaveChanges();
                        }
                    }

                    return results.ToList();
                }

            }
            catch (Exception Ex)
            {

                return null;
            }

        }
        public static List<ResultModelWaec> resultForMerchant(Int64 userid)
        {

            var studentId = userid;
            try
            {
                using (var db = new EXAMSPURDBEntities())
                {

                    db.Configuration.ProxyCreationEnabled = false;
                    var pendingExam = db.C_tblExamTaken.Where(o => o.Status == 1 && o.ExamMode == 1 && o.ExamID == 1002);
                    foreach (var examInfo in pendingExam)
                    {
                        var a = updateExam(examInfo);
                        var examD = db.C_tblQuestionList.Where(o => o.ExamTakenId == examInfo.ExamTakenID).ToList();
                        foreach (var item in examD)
                        {
                            var incorrectList = new List<string>();
                            int correctCount = 0;
                            var attemptedCount = 0;
                            var answerList = item.CorrectAnswerList;

                            var choosenList = item.ChoosenAnswerList;

                            var qList = item.QuestionList;
                            if (choosenList == null)
                            {
                                item.NoCorrect = 0;
                                item.NoAttempted = 0;
                                for (var i = 0; i < answerList.Split(',').Length; i++)
                                {
                                    incorrectList.Add(qList.Split(',')[i] + "!" + "");
                                }
                                item.IncorrectAnswerList = String.Join(",", incorrectList);
                                Task<object> b = SubmitExamWaec.saveExamResult(item);
                            }
                            else
                            {
                                var listAnswer = new string[answerList.Split(',').Length];
                                // if (answerList.Split(',').Length != choosenList.Split(',').Length) {
                                for (var i = 0; i < choosenList.Split(',').Length - 1; i++)
                                {
                                    listAnswer[i] = choosenList.Split(',')[i];


                                }
                                for (var i = 0; i < answerList.Split(',').Length; i++)
                                {
                                    if (answerList.Split(',')[i] == string.Join(",", listAnswer).Split(',')[i]) correctCount++;
                                    if (answerList.Split(',')[i] == "F") correctCount++;
                                    else incorrectList.Add(qList.Split(',')[i] + "!" + string.Join(",", listAnswer).Split(',')[i]);
                                    if (string.Join(",", listAnswer).Split(',')[i] != "") attemptedCount++;
                                }
                                item.NoCorrect = correctCount;
                                item.NoAttempted = attemptedCount;
                                item.IncorrectAnswerList = String.Join(",", incorrectList);
                                Task<object> b = SubmitExamWaec.saveExamResult(item);
                            }
                        }
                    }
                    //var tt = ((decimal)(db.C_tblQuestionList.Where(o => o.SubjectID != 1014 && o.ExamTakenId == 21062).Sum(o => o.NoCorrect)) * (decimal)((100 * 3)) / 120);
                    var results = (from exam in db.C_tblExamTaken
                                   join result in db.C_tblQuestionList on exam.ExamTakenID equals result.ExamTakenId
                                   where exam.StudentID == studentId
                                   where exam.Status == 2
                                   where exam.ExamID == 1002
                                   orderby exam.StartTimestamp descending
                                   group result by new { exam.ExamTakenID, exam.ExamMode, exam.StartTimestamp, exam.TimeUsed, exam.TotalTime, } into g
                                   select new ResultModelWaec
                                   {
                                       score = Math.Round((decimal)(db.C_tblQuestionList.Where(o => o.ExamTakenId == g.Key.ExamTakenID).FirstOrDefault().NoCorrect * 2), 0).ToString(),
                                       examid = g.Key.ExamTakenID.ToString(),
                                       startdate = SqlFunctions.DateName("year", g.Key.StartTimestamp).Trim() + "-" +
                    SqlFunctions.StringConvert((double)g.Key.StartTimestamp.Value.Month).TrimStart() + "-" +
                    SqlFunctions.DateName("day", g.Key.StartTimestamp),
                                       timeused = (Math.Floor(((decimal)g.Key.TotalTime - (decimal)g.Key.TimeUsed) / 3600)).ToString() + ":" + (Math.Floor(((decimal)g.Key.TotalTime - (decimal)g.Key.TimeUsed) % 3600 / 60)).ToString() + ":" + (Math.Floor(((decimal)g.Key.TotalTime - (decimal)g.Key.TimeUsed) % 3600 % 60)).ToString(),
                                       exammode = g.Key.ExamMode.ToString(),



                                   }).ToList();


                    foreach (var item in results.ToList())
                    {
                        var sc = db.C_tblScore.Where(o => o.ExamCode.ToString() == item.examid).FirstOrDefault();
                        if (sc == null)
                        {
                            var a = new C_tblScore();
                            a.StudentID = (long)studentId;
                            a.ExamCode = Convert.ToInt64(item.examid);
                            a.ExamID = db.C_tblExamTaken.Find(a.ExamCode).ExamID;
                            a.Score = Convert.ToInt32(item.score.Split('.')[0]);
                            db.C_tblScore.Add(a);
                            db.SaveChanges();
                        }
                    }

                    return results.ToList();
                }

            }
            catch (Exception Ex)
            {

                return null;
            }

        }
        public static object updateExam(C_tblExamTaken examInfo)
        {
            using (var db = new EXAMSPURDBEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                examInfo.Status = 2;
                examInfo.EndTimestamp = DateTime.Now;
                db.C_tblExamTaken.Attach(examInfo);
                db.Entry(examInfo).State = System.Data.Entity.EntityState.Modified;
                var a = db.SaveChanges();
                return a;
            }
        }
    }


    public class ResultModelTWaec
    {
        public string score { get; set; }
        public string startdate { get; set; }
        public string examid { get; set; }
        public string exammode { get; set; }
        public string timeused { get; set; }
    }

    public class ResultViewModelTWaec
    {
        public static List<ResultModelWaec> result()
        {

            var studentId = (long?)HttpContext.Current.Session["userid"];
            try
            {
                using (var db = new EXAMSPURDBEntities())
                {

                    db.Configuration.ProxyCreationEnabled = false;
                    var pendingExam = db.C_tblExamTaken.Where(o => o.Status == 1 && o.ExamMode == 3 && o.ExamID == 1002);
                    foreach (var examInfo in pendingExam)
                    {
                        var a = updateExamWaec(examInfo);
                        var examD = db.C_tblQuestionList.Where(o => o.ExamTakenId == examInfo.ExamTakenID).ToList();
                        foreach (var item in examD)
                        {
                            var incorrectList = new List<string>();
                            int correctCount = 0;
                            var attemptedCount = 0;
                            var answerList = item.CorrectAnswerList;

                            var choosenList = item.ChoosenAnswerList;

                            var qList = item.QuestionList;
                            if (choosenList == null)
                            {
                                item.NoCorrect = 0;
                                item.NoAttempted = 0;
                                for (var i = 0; i < answerList.Split(',').Length; i++)
                                {
                                    incorrectList.Add(qList.Split(',')[i] + "!" + "");
                                }
                                item.IncorrectAnswerList = String.Join(",", incorrectList);
                                Task<object> b = SubmitExamWaec.saveExamResult(item);
                            }
                            else
                            {
                                var listAnswer = new string[answerList.Split(',').Length];
                                // if (answerList.Split(',').Length != choosenList.Split(',').Length) {
                                for (var i = 0; i < choosenList.Split(',').Length - 1; i++)
                                {
                                    listAnswer[i] = choosenList.Split(',')[i];


                                }
                                for (var i = 0; i < answerList.Split(',').Length; i++)
                                {
                                    if (answerList.Split(',')[i] == string.Join(",", listAnswer).Split(',')[i]) correctCount++;
                                    if (answerList.Split(',')[i] == "F") correctCount++;
                                    else incorrectList.Add(qList.Split(',')[i] + "!" + string.Join(",", listAnswer).Split(',')[i]);
                                    if (string.Join(",", listAnswer).Split(',')[i] != "") attemptedCount++;
                                }
                                item.NoCorrect = correctCount;
                                item.NoAttempted = attemptedCount;
                                item.IncorrectAnswerList = String.Join(",", incorrectList);
                                Task<object> b = SubmitExamWaec.saveExamResult(item);
                            }
                        }
                    }

                    var results = from exam in db.C_tblExamTaken
                                  join result in db.C_tblQuestionList on exam.ExamTakenID equals result.ExamTakenId
                                  where exam.StudentID == studentId
                                  where exam.Status == 2
                                  where exam.ExamID == 1002
                                  orderby exam.StartTimestamp descending
                                  group result by new { exam.ExamTakenID, exam.ExamMode, exam.StartTimestamp, exam.TimeUsed, exam.TotalTime } into g
                                  select new ResultModelWaec
                                  {
                                      score = Math.Round((decimal)(db.C_tblQuestionList.Where(o => o.ExamTakenId == g.Key.ExamTakenID).FirstOrDefault().NoCorrect * 2), 0).ToString(),
                                      examid = g.Key.ExamTakenID.ToString(),
                                      startdate = g.Key.StartTimestamp.Value.ToString(),
                                      timeused = (Math.Floor(((decimal)g.Key.TotalTime - (decimal)g.Key.TimeUsed) / 3600)).ToString() + ":" + (Math.Floor(((decimal)g.Key.TotalTime - (decimal)g.Key.TimeUsed) % 3600 / 60)).ToString() + ":" + (Math.Floor(((decimal)g.Key.TotalTime - (decimal)g.Key.TimeUsed) % 3600 % 60)).ToString(),
                                      exammode = g.Key.ExamMode.ToString()

                                  };
                    return results.ToList();
                }

            }
            catch (Exception Ex)
            {

                return null;
            }

        }
        public static object updateExamWaec(C_tblExamTaken examInfo)
        {
            using (var db = new EXAMSPURDBEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                examInfo.Status = 2;
                examInfo.EndTimestamp = DateTime.Now;
                db.C_tblExamTaken.Attach(examInfo);
                db.Entry(examInfo).State = System.Data.Entity.EntityState.Modified;
                var a = db.SaveChanges();
                return a;
            }
        }
    }
}
