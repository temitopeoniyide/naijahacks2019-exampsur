using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExamSpur.Models
{


    public class AttarchTopicsToQuestion
    {

        public Int64 SubjectId { get; set; }
        public int TopicID { get; set; }
         public Int64 questionId { get; set; }


        public AttarchTopicsToQuestion(Int64 subjectID, Int64 questionID, int topicID)
        {
            this.SubjectId = subjectID;
            this.TopicID = topicID;
            this.questionId = questionID;
            
        }
        public object updateTopicsToQuestionTable()
        {
            try
            {
                //Int64 userid = (Int64)HttpContext.Current.Session["userid"];

                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var model = db.C_tblQuestion.Where(o => o.SubjectID == SubjectId && o.QuestionID == questionId && o.TopicId == null).FirstOrDefault();
                    model.TopicId = TopicID;
                    db.C_tblQuestion.Attach(model);
                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    
                    return new { value = 1 };
                }

            }
            catch (Exception Ex)
            {

                return new { value = 0, msg = "Error Occured" };
            }


        }
    }

    public class TopicViewModel
    {
        public int subjectID { get; set; }

        public TopicViewModel(int subjectId)
        {
            this.subjectID = subjectId;
        }

        public object loadTopicsModel()
        {
            try
            {
                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var topics =(from s in db.C_tblTopics join q in db.C_tblQuestion on s.TopicId
                                  equals q.TopicId where s.SubjectId == subjectID
                                  group q by new {s.TopicId,s.TopicName } into g
                                  select new {TopicId= g.Key.TopicId,
                                      TopicName = g.Key.TopicName,
                                      count =g.Count()}).ToList().Where(o=>o.count>=10).ToList();
                    var Qids = (from s in db.C_tblQuestion where s.SubjectID == subjectID && s.TopicId == null select new { s.QuestionID }).ToList();
                    // var saveQ = new C_tblQuestion();
                    var Qstring = string.Join(",", Qids);

                    return new { value = 1, resp = topics, qtring=Qstring };
                }
            }
            catch (Exception ex)
            {
                return new { value = 0, resp = "Error Occured. Try again Later" };

            }

        }
        public object loadTopicsModel2()
        {
            try
            {
                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var topics = (from s in db.C_tblTopics
                                  join q in db.C_tblQuestion on s.TopicId
          equals q.TopicId
                                  where s.SubjectId == subjectID
                                  group q by new { s.TopicId, s.TopicName } into g
                                  select new
                                  {
                                      TopicId = g.Key.TopicId,
                                      TopicName = g.Key.TopicName,
                                      count = g.Count()
                                  }).ToList().Where(o => o.count >= 10).ToList();
                   // var Qids = (from s in db.C_tblQuestion where s.SubjectID == subjectID && s.TopicId == null select new { s.QuestionID }).ToList();
                    // var saveQ = new C_tblQuestion();
                   // var Qstring = string.Join(",", Qids);

                    return new { value = 1, resp = topics};
                }
            }
            catch (Exception ex)
            {
                return new { value = 0, resp = "Error Occured. Try again Later" };

            }

        }
    }



    public class TopicViewModel2
    {
        public int subjectID { get; set; }

        public TopicViewModel2(int subjectId)
        {
            this.subjectID = subjectId;
        }

        public object loadTopicsModel2()
        {
            try
            {
                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.LazyLoadingEnabled = true;
                    db.Configuration.ProxyCreationEnabled = false;
                    var topics =(from t in  db.C_tblTopics where t.SubjectId == subjectID select new {t.TopicId,t.TopicName }).ToList();
                    var Qids = (from s in db.C_tblQuestion where s.SubjectID == subjectID && s.TopicId == null select new { s.QuestionID }).ToList();
                    // var saveQ = new C_tblQuestion();
                    var Qstring = string.Join(",", Qids);

                    return new { value = 1, resp = topics, qtring = Qstring };

                    // return new { value = 1, resp = topics, qtring = Qstring };
                }
            }
            catch (Exception ex)
            {
                return new { value = 0, resp = "Error Occured. Try again Later" };

            }

        }
    }

    public class DisplayTopicViewModel
    {
        public int questionID { get; set; }

        public DisplayTopicViewModel(int qid)
        {
            this.questionID = qid;
        }

        public object DisplayTopicToQuestionssModel()
        {
            try
            {
                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var question = db.C_tblQuestion.Find(questionID);
                    return new { value = 1, resp = question };
                }
            }
            catch (Exception ex)
            {
                return new { value = 0, resp = "Error Occured. Try again Later" };

            }

        }
    }
}