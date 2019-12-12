using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExamSpur.Models
{
    public class ResultDetailsViewModel
    {
        public string examid { get; set; }
        public int attempted { get; set; }
        public decimal score { get; set; }
        public string subject { get; set; }
        public string totalQuestion { get; set; }
        public string subjectID { get; set; }
        public int passed { get; set; }
        public string timeUsed { get; set; }
        public int timeAdv { get; set; }
        public int totalTime { get; set; }
        public string startTime { get; set; }

    }
    public class resultDetail {
public  Int64 ExamID { get; set; }
        public resultDetail(Int64 examId) {
            ExamID = examId;
        }
        public  object result()
        {
            using (var db = new EXAMSPURDBEntities()) {
                db.Configuration.ProxyCreationEnabled = false;
                var model = (from a in db.C_tblQuestionList join b in db.C_tblSubjectGeneral on a.SubjectID equals b.SubjectID join c in db.C_tblExamTaken on a.ExamTakenId equals c.ExamTakenID where a.ExamTakenId == ExamID
                             select new ResultDetailsViewModel {
                                 examid = c.ExamTakenID.ToString(),
                                 attempted = (int)a.NoAttempted,
                                 subject = b.SubjectName,
                                 totalQuestion = b.NoOfQuestions.ToString(),
                                 startTime = c.StartTimestamp.Value.ToString(),
                                 totalTime = (int)c.TotalTime,
                                 timeUsed = (Math.Floor(((decimal)(c.TotalTime) - (decimal)c.TimeUsed) / 3600)).ToString() + ":" + (Math.Floor(((decimal)c.TotalTime - (decimal)c.TimeUsed) % 3600 / 60)).ToString() + ":" + (Math.Floor(((decimal)c.TotalTime - (decimal)c.TimeUsed)) % 3600 % 60).ToString(),
                                 passed = (int)a.NoCorrect,
                                 score = Math.Round(a.SubjectID == 1014 ? Math.Round((decimal)a.NoCorrect * 100 / 60,1) : Math.Round((decimal)a.NoCorrect * 100 / 40,1),1),
                                 timeAdv = 100-(int)((c.TimeUsed)/ c.TotalTime * 100),
                                 subjectID = a.SubjectID.ToString()
                             }

                            );
                return model.ToList();

            }



        }
    }
    public class resultDetailTopic
    {
        public Int64 ExamID { get; set; }
        public resultDetailTopic(Int64 examId)
        {
            ExamID = examId;
        }
        public object result()
        {
            using (var db = new EXAMSPURDBEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var model = (from a in db.C_tblQuestionListTopic
                             join b in db.C_tblTopics on a.TopicID equals b.TopicId
                             join c in db.C_tblExamTaken on a.ExamTakenId equals c.ExamTakenID
                             where a.ExamTakenId == ExamID
                             select new ResultDetailsViewModel
                             {
                                 examid = c.ExamTakenID.ToString(),
                                 attempted = (int)a.NoAttempted,
                                 subject = b.TopicName,
                                 totalQuestion = "10",
                                 startTime = c.StartTimestamp.Value.ToString(),
                                 totalTime = (int)c.TotalTime,
                                 timeUsed = (Math.Floor(((decimal)(c.TotalTime) - (decimal)c.TimeUsed) / 3600)).ToString() + ":" + (Math.Floor(((decimal)c.TotalTime - (decimal)c.TimeUsed) % 3600 / 60)).ToString() + ":" + (Math.Floor(((decimal)c.TotalTime - (decimal)c.TimeUsed)) % 3600 % 60).ToString(),
                                 passed = (int)a.NoCorrect,
                                 score =  Math.Round((decimal)a.NoCorrect),
                                 timeAdv = 100 - (int)((c.TimeUsed) / c.TotalTime * 100),
                                 subjectID = a.TopicID.ToString()
                             }

                            );
                return model.ToList();

            }



        }
    }
    public class resultDetailT
    {
        public Int64 ExamID { get; set; }
        public resultDetailT(Int64 examId)
        {
            ExamID = examId;
        }
        public object result()
        {
            using (var db = new EXAMSPURDBEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var model = (from a in db.C_tblQuestionList
                             join b in db.C_tblSubjectGeneral on a.SubjectID equals b.SubjectID
                             join c in db.C_tblExamTaken on a.ExamTakenId equals c.ExamTakenID
                             where a.ExamTakenId == ExamID
                             select new ResultDetailsViewModel
                             {
                                 examid = c.ExamTakenID.ToString(),
                                 attempted = (int)a.NoAttempted,
                                 subject = b.SubjectName,
                                 totalQuestion = 5.ToString(),
                                 startTime = c.StartTimestamp.Value.ToString(),
                                 totalTime = (int)c.TotalTime,
                                 timeUsed = (Math.Floor(((decimal)(1200) - ((c.TimeUsed == null) ? 1200 : (decimal)c.TimeUsed)) / 3600)).ToString() + ":" + (Math.Floor(((decimal)1200 - ((c.TimeUsed == null) ? 1200 : (decimal)c.TimeUsed)) % 3600 / 60)).ToString() + ":" + (Math.Floor(((decimal)1200 - ((c.TimeUsed == null) ? 1200 : (decimal)c.TimeUsed))) % 3600 % 60).ToString(),
                                 passed = (int)a.NoCorrect,
                                 score = Math.Round(a.SubjectID == 1014 ? Math.Round(((decimal)a.NoCorrect * 100) / 5, 1) : Math.Round((decimal)a.NoCorrect * 100 / 5, 1), 1),
                                 timeAdv = ((int)100 - (int)((c.TimeUsed == null) ? 0 : c.TimeUsed / 1200 * 100)),
                                 subjectID = a.SubjectID.ToString() 
                             }

                            );
                return model.ToList();
                  
            }



        }
    }
    public class resultDetailWaec
    {
        public Int64 ExamID { get; set; }
        public resultDetailWaec(Int64 examId)
        {
            ExamID = examId;
        }
        public object result()
        {
            using (var db = new EXAMSPURDBEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var model = (from a in db.C_tblQuestionList
                             join b in db.C_tblSubjectGeneral on a.SubjectID equals b.SubjectID
                             join c in db.C_tblExamTaken on a.ExamTakenId equals c.ExamTakenID
                             where a.ExamTakenId == ExamID
                             select new ResultDetailsViewModel
                             {
                                 examid = c.ExamTakenID.ToString(),
                                 attempted = (int)a.NoAttempted,
                                 subject = b.SubjectName,
                                 totalQuestion = b.NoOfQuestions.ToString(),
                                 startTime = c.StartTimestamp.Value.ToString(),
                                 totalTime = (int)c.TotalTime,
                                 timeUsed = (Math.Floor(((decimal)(c.TotalTime) - (decimal)c.TimeUsed) / 3600)).ToString() + ":" + (Math.Floor(((decimal)c.TotalTime - (decimal)c.TimeUsed) % 3600 / 60)).ToString() + ":" + (Math.Floor(((decimal)c.TotalTime - (decimal)c.TimeUsed)) % 3600 % 60).ToString(),
                                 passed = (int)a.NoCorrect,
                                 score =  Math.Round((decimal)a.NoCorrect *2 , 1),
                                 timeAdv = 100 - (int)((c.TimeUsed) / c.TotalTime * 100),
                                 subjectID = a.SubjectID.ToString()
                             }

                            );
                return model.ToList();

            }



        }
    }

    public class resultDetailTWaec
    {
        public Int64 ExamID { get; set; }
        public resultDetailTWaec(Int64 examId)
        {
            ExamID = examId;
        }
        public object result()
        {
            using (var db = new EXAMSPURDBEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var model = (from a in db.C_tblQuestionList
                             join b in db.C_tblSubjectGeneral on a.SubjectID equals b.SubjectID
                             join c in db.C_tblExamTaken on a.ExamTakenId equals c.ExamTakenID
                             where a.ExamTakenId == ExamID
                             select new ResultDetailsViewModel
                             {
                                 examid = c.ExamTakenID.ToString(),
                                 attempted = (int)a.NoAttempted,
                                 subject = b.SubjectName,
                                 totalQuestion = 5.ToString(),
                                 startTime = c.StartTimestamp.Value.ToString(),
                                 totalTime = (int)c.TotalTime,
                                 timeUsed = (Math.Floor(((decimal)(1200) - ((c.TimeUsed == null) ? 1200 : (decimal)c.TimeUsed)) / 3600)).ToString() + ":" + (Math.Floor(((decimal)1200 - ((c.TimeUsed == null) ? 1200 : (decimal)c.TimeUsed)) % 3600 / 60)).ToString() + ":" + (Math.Floor(((decimal)1200 - ((c.TimeUsed == null) ? 1200 : (decimal)c.TimeUsed))) % 3600 % 60).ToString(),
                                 passed = (int)a.NoCorrect,
                                 score =  Math.Round((decimal)a.NoCorrect, 1),
                                 timeAdv = ((int)100 - (int)((c.TimeUsed == null) ? 0 : c.TimeUsed / 1200 * 100)),
                                 subjectID = a.SubjectID.ToString()
                             }

                            );
                return model.ToList();

            }



        }
    }
}