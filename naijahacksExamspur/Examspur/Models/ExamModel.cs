using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ExamSpur.Models
{
    public class ExamModel
    {
    }

    public class SubjectViewModel {
        public Int64 ExamId { get; set; }
        public SubjectViewModel( Int64 examid) {
            this.ExamId = examid;
        }

        public object loadSubject() {
            try
            {
                using (var db = new EXAMSPURDBEntities()) {
                    db.Configuration.ProxyCreationEnabled = false;
                    var subject = db.C_tblSubjectGeneral.Where(o => o.ExamID == ExamId).ToList();
                    return new { value = 1, resp = subject };
                }
            }
            catch (Exception ex) {
                return new { value = 0, resp = "Error Occured. Try again Later" };

            }

        }
    }


    public class TopicsssViewModel
    {
        public Int64 SubjectId { get; set; }


        public TopicsssViewModel(Int64 subjectid)
        {
            this.SubjectId = subjectid;
        }

        public object loadTopicssss()
        {
            try
            {
                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var topics = (from t in db.C_tblTopics where t.SubjectId == SubjectId  orderby t.TopicName ascending select new { t.TopicId, t.TopicName }).ToList();
                    return new { value = 1, resp = topics };
                }
            }
            catch (Exception ex)
            {
                return new { value = 0, resp = "Error Occured. Try again Later" };

            }

        }
    }


    public class GenerateQuestions {
        public int examId { get; set; }
        public int examMode { get; set; }
        public string subjectIds { get; set; }
        public Int64 studentId { get; set; }

        public GenerateQuestions( int examid, int exammode, string subjectids, Int64 studentid) {
            examId = examid;
            examMode = exammode;
            subjectIds = subjectids;
            studentId = studentid;

        }
        public object saveExamInstanceAndGeneratequestionUTME()
        {
            try
            {
                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var respQ = new List<string>();
                    var choosenAnswer = new List<string>();
                    var examInfo = new C_tblExamTaken();
                    examInfo.ExamID = examId;
                    examInfo.ExamMode = examMode;
                    examInfo.StudentID = studentId;
                    examInfo.Status = 1;
                    examInfo.ActiveSubject = Convert.ToInt32(subjectIds.Split(',')[0]);
                    examInfo.TimeUsed = 0;
                    examInfo.TotalTime = 7200; //seconds
                    examInfo.StartTimestamp = DateTime.Now;
                    db.C_tblExamTaken.Add(examInfo);
                    db.SaveChanges();
                    var student = db.C_tblStudentReg.Find(studentId);
                  
                    foreach (var item in subjectIds.Split(',')) {
                        var citem = Convert.ToInt32(item);
                       var sub = db.C_tblSubjectGeneral.Find(citem);
                        var sub2 = db.C_tblQuestion.Where(o => o.SubjectID == citem).ToList();
                    //    var maxNoOFQuestionAvail = sub2.Max(o => o.QuestionNumber);
                   //     var minxNoOFQuestionAvail = sub2.Min(o => o.QuestionNumber);
                        var maxQ = (int)sub.NoOfQuestions;
                        var randomQuestion = new int[maxQ];
                        var listAnswer = new string[maxQ];
                        Random random = new Random();
                        for (int i = 0; i < maxQ; i++)
                        {


                        ln: int randomNumber = random.Next((int)0, (int)sub2.Count() - 1);
                         var q= (int)sub2.ElementAt(randomNumber).QuestionNumber;
                            //if (!randomQuestion.Contains(q)) goto ln;
                            if (!randomQuestion.Contains(q))
                            {
                                var questiondetail = sub2.ElementAt(randomNumber);
                                if (questiondetail == null) goto ln;
                                if (questiondetail.HasImage >= 1)
                                {
                                    var questionwithImage = sub2.Where(o => o.ImageId == questiondetail.ImageId).OrderBy(o => o.QuestionNumber).ToList();
                                    for (int j = 0; j < questionwithImage.Count; j++)
                                    {
                                       
                                        randomQuestion[i] = (int)questionwithImage[j].QuestionNumber;
                                        listAnswer[i] = questionwithImage[j].Answer;
                                       
                                        
                                        if (i == maxQ - 1) { break; }
                                        if (j == 8) goto ln;
                                        if (j< questionwithImage.Count-1) { i++; }
                                    }

                                }
                                else
                                {
                                    randomQuestion[i] = q;
                                    listAnswer[i] = questiondetail.Answer;
                                }
                            }
                            else
                            {
                                goto ln;
                            }
                        }
                                var saveQ = new C_tblQuestionList();
                                saveQ.ExamID = examId;
                                saveQ.ExamTakenId = examInfo.ExamTakenID;
                                saveQ.QuestionList = string.Join(",", randomQuestion);
                                saveQ.CorrectAnswerList = string.Join(",", listAnswer);
                                saveQ.SubjectID = citem;
                            saveQ.LastQuestionID = 0;
                                saveQ.TimeElapsed = 0;
                            db.C_tblQuestionList.Add(saveQ);
                            db.SaveChanges();
                            respQ.Add(item+"%"+string.Join("-", randomQuestion));
                           // choosenAnswer.Add(string.Join(",", saveQ.ChoosenAnswerList));
                        }
                    if (student.Unit != 99999) student.Unit -= 1;
                    db.C_tblStudentReg.Attach(student);
                    db.Entry(student).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    HttpContext.Current.Session["unitAvailable"] = student.Unit;
                    return new { value = 1, Qlist =string.Join(",",respQ),exammode= examMode,examtakenid=examInfo.ExamTakenID };
                }
            }
            catch (Exception ex)
            {
                return new { value = 0, resp = "Error Occured. Try again Later" };
                
            }

        }
    }


    public class GenerateQuestionsByTopic
    {
      
        public string topicId { get; set; }
        public Int64 studentId { get; set; }

        public GenerateQuestionsByTopic( string topicid, Int64 userId)
        {
            
            topicId = topicid;
            studentId = userId;
           
        }
        public object saveExamInstanceAndGeneratequestionbyTopic()
        {
            try
            {
                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var respQ = new List<string>();
                    var choosenAnswer = new List<string>();
                    var examInfo = new C_tblExamTaken();
                    examInfo.ExamID = 1003;

                    examInfo.StudentID = studentId;
                    examInfo.Status = 1;
                    examInfo.ActiveSubject = Convert.ToInt32(topicId.Split(',')[0]);
                    examInfo.TimeUsed = 0;
                    examInfo.ExamMode = 2;
                    examInfo.TotalTime = 600 * topicId.Split(',').Count(); //seconds
                    examInfo.StartTimestamp = DateTime.Now;
                    db.C_tblExamTaken.Add(examInfo);
                    db.SaveChanges();
                    foreach (var item in topicId.Split(','))
                    {
                        var citem = Convert.ToInt32(item);
                      //  var sub = db.C_tblTopics.Find(citem);
                        var sub2 = db.C_tblQuestion.Where(o => o.TopicId == citem).ToList();
                       // var maxNoOFQuestionAvail = sub2.Max(o => o.QuestionNumber);
                        //var minxNoOFQuestionAvail = sub2.Min(o => o.QuestionNumber);
                      
                        var count=sub2.Count();
                        var maxQ =10;
                        var randomQuestion = new int[maxQ];
                        var listAnswer = new string[maxQ];
                        Random random = new Random();
                        for (int i = 0; i < maxQ; i++)
                        {
                            int randomNumber = 0;
                            var questiondetail = new C_tblQuestion();
                            if (count>10)
                            {
                             
                            ln: randomNumber = random.Next((int)0, (int)count - 1);
                                if (!randomQuestion.Contains((int)sub2.ElementAt(randomNumber).QuestionNumber))
                                {
                                     questiondetail = sub2.ElementAt(randomNumber);
                                    if (questiondetail == null) goto ln;
                                    if (questiondetail.HasImage >= 1)
                                    {
                                        var questionwithImage = db.C_tblQuestion.Where(o => o.ImageId == questiondetail.ImageId && o.TopicId == citem).OrderBy(o => o.QuestionNumber).ToList();
                                        for (int j = 0; j < questionwithImage.Count; j++)
                                        {

                                            randomQuestion[i] = (int)questionwithImage[j].QuestionNumber;
                                            listAnswer[i] = questionwithImage[j].Answer;


                                            if (i == maxQ - 1) { break; }
                                            if (j == 8) goto ln;
                                            if (j < questionwithImage.Count - 1) { i++; }
                                        }

                                    }
                                    else
                                    {
                                        randomQuestion[i] = (int)sub2.ElementAt(randomNumber).QuestionNumber;
                                        listAnswer[i] = questiondetail.Answer;
                                    }
                                }
                                else
                                {
                                    goto ln;
                                }
                            }
                            else {

                                randomNumber = i;
                                questiondetail = sub2.ElementAt(randomNumber);
                      
                                if (questiondetail.HasImage >= 1)
                                {
                                    var questionwithImage = db.C_tblQuestion.Where(o => o.ImageId == questiondetail.ImageId && o.TopicId == citem).OrderBy(o => o.QuestionNumber).ToList();
                                    for (int j = 0; j < questionwithImage.Count; j++)
                                    {

                                        randomQuestion[i] = (int)questionwithImage[j].QuestionNumber;
                                        listAnswer[i] = questionwithImage[j].Answer;


                                        if (i == maxQ - 1) { break; }
                                        if (j == 8) break;
                                        if (j < questionwithImage.Count - 1) { i++; }
                                    }

                                }
                                else
                                {
                                    randomQuestion[i] = (int)sub2.ElementAt(randomNumber).QuestionNumber;
                                    listAnswer[i] = questiondetail.Answer;
                                }

                            }
                      //      if (randomNumber < minxNoOFQuestionAvail || randomNumber > maxNoOFQuestionAvail) goto ln;
                         
                           
                        }
                        
                        
                        
                        var saveQ = new C_tblQuestionListTopic();
                        saveQ.ExamID = 1003;
                        saveQ.ExamTakenId = examInfo.ExamTakenID;
                        saveQ.QuestionList = string.Join(",", randomQuestion);
                        saveQ.CorrectAnswerList = string.Join(",", listAnswer);
                        saveQ.TopicID = citem;
                        saveQ.LastQuestionID = 0;
                        saveQ.TimeElapsed = 0;
                        db.C_tblQuestionListTopic.Add(saveQ);
                        db.SaveChanges();
                        respQ.Add(item + "%" + string.Join("-", randomQuestion));
                        // choosenAnswer.Add(string.Join(",", saveQ.ChoosenAnswerList));
                    }
                    var student = db.C_tblStudentReg.Find(studentId);
                    if (student.Unit != 99999) student.Unit -= 1;
                    db.C_tblStudentReg.Attach(student);
                    db.Entry(student).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    HttpContext.Current.Session["unitAvailable"] = student.Unit;
                    return new { value = 1, Qlist = string.Join(",", respQ), examtakenid = examInfo.ExamTakenID };
                }
            }
            catch (Exception ex)
            {
                return new { value = 0, resp = "Error Occured. Try again Later" };

            }

        }
    }
    public class CheckExam
    {

        public Int64 ExamId { get; set; }
        public CheckExam(Int64 examid)
        {
            this.ExamId = examid;
        }
        public object CheckforOngoingExam()
        {
            try
            {
                Int64 userid = (Int64)HttpContext.Current.Session["userid"];
                var mode = new int?[] { 1, 2 };
                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var ExamTaken = db.C_tblExamTaken.Where(o => o.ExamID == ExamId && o.StudentID == userid && o.Status == 1 && mode.Contains(o.ExamMode)).FirstOrDefault();
                    if (ExamTaken == null) return (new { ExamTaken });
                    else
                    {
                        var model = (from s in db.C_tblSubjectGeneral
                                     join l in db.C_tblQuestionList on s.SubjectID equals l.SubjectID
                                     where l.ExamTakenId == ExamTaken.ExamTakenID
                                     select new
                                     {
                                         SubjectId = l.SubjectID,
                                         l.QuestionList,
                                         l.ChoosenAnswerList,
                                         s.SubjectName,
                                         l.ExamTakenId,
                                         l.LastQuestionID,
                                         timeUsed = ExamTaken.TimeUsed,
                                         activeSubject = ExamTaken.ActiveSubject,
                                         exammode = ExamTaken.ExamMode,
                                     }

                                     ).ToList();

                        return new { value = 1, Model = model.ToList(),ExamTaken=ExamTaken };
                    }
                }

            }
            catch (Exception Ex)
            {

                return new { value = 0, msg = "Error Occured" };
            }


        }
        public object CheckforOngoingExamTopic()
        {
            try
            {
                Int64 userid = (Int64)HttpContext.Current.Session["userid"];
                var mode = new int?[] { 1, 2 };
                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var ExamTaken = db.C_tblExamTaken.Where(o => o.ExamID == ExamId && o.StudentID == userid && o.Status == 1 && mode.Contains(o.ExamMode)).FirstOrDefault();
                    if (ExamTaken == null) return (new { ExamTaken });
                    else
                    {
                        var model = (from s in db.C_tblTopics
                                     join l in db.C_tblQuestionListTopic on s.TopicId equals l.TopicID
                                     where l.ExamTakenId == ExamTaken.ExamTakenID
                                     select new
                                     {
                                         TopicId = l.TopicID,
                                         l.QuestionList,
                                         l.ChoosenAnswerList,
                                         s.TopicName,
                                         l.ExamTakenId,
                                         l.LastQuestionID,
                                         timeUsed = ExamTaken.TimeUsed,
                                         activeSubject = ExamTaken.ActiveSubject,
                                         exammode = ExamTaken.ExamMode,
                                     }

                                     ).ToList();

                        return new { value = 1, Model = model.ToList(), ExamTaken = ExamTaken };
                    }
                }

            }
            catch (Exception Ex)
            {

                return new { value = 0, msg = "Error Occured" };
            }


        }

    }
    public class checklogout {
        public object checkBeforeLogout()
        {
            Int64 userid = (Int64)HttpContext.Current.Session["userid"];
            using (var db = new EXAMSPURDBEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var ExamTaken = db.C_tblExamTaken.Where(o => o.StudentID == userid && o.Status == 1 && o.ExamMode == 1).FirstOrDefault();
                if (ExamTaken == null) return (new { onexam = 0 });
                else return new { onexam = 1 };
            }
        }


    }


    public class GetNextQuestionbyTopic
    {

        public Int64 topicId { get; set; }
        public Int64 QuestionNo { get; set; }
        public GetNextQuestionbyTopic(Int64 Topicid, Int64 questionno)
        {
            this.topicId = Topicid;
            this.QuestionNo = questionno;
        }
        public object QuestionbyTopic()
        {
            try
            {
                //Int64 userid = (Int64)HttpContext.Current.Session["userid"];

                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var model = db.C_tblQuestion.Where(o => o.TopicId == topicId && o.QuestionNumber == QuestionNo).FirstOrDefault();
                    return model;
                }

            }
            catch (Exception Ex)
            {

                return new { value = 0, msg = "Error Occured" };
            }


        }
    }
    public class GetNextQuestion
    {

        public Int64 SubjectId { get; set; }
        public Int64 QuestionNo { get; set; }
        public GetNextQuestion(Int64 subjectid,Int64 questionno)
        {
            this.SubjectId = subjectid;
            this.QuestionNo = questionno;
        }
        public object Question()
        {
            try
            {
                //Int64 userid = (Int64)HttpContext.Current.Session["userid"];

                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var model = db.C_tblQuestion.Where(o => o.SubjectID == SubjectId && o.QuestionNumber == QuestionNo).FirstOrDefault();
                    return model;
                }

            }
            catch (Exception Ex)
            {

                return new { value = 0, msg = "Error Occured" };
            }


        }
    }
    public class questionHerem
    {
        public string subjectName { get; set; }
        public long lastImageId { get; set; }
        public int count { get; set; }
    }
    public class UpadateAnswers
    {

        public Int64 SubjectId { get; set; }
        public Int64 ExamTakenID { get; set; }
        public int LastQuestionId { get; set; }

        public string Answers { get; set; }


        public int CurrentTime { get; set; }
     
        
        public UpadateAnswers(Int64 subjectid, Int64 examtakenid,string answers,int curentTime,int lastQuestionId)
        {
            this.SubjectId = subjectid;
            this.ExamTakenID = examtakenid;
            this.Answers = answers;
            this.CurrentTime = curentTime;
            this.LastQuestionId = lastQuestionId;
        }
        public object update()
        {
            try
            {
                //Int64 userid = (Int64)HttpContext.Current.Session["userid"];

                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var model = db.C_tblQuestionList.Where(o => o.SubjectID == SubjectId && o.ExamTakenId==ExamTakenID).FirstOrDefault();
                    model.ChoosenAnswerList = Answers;
                    model.LastQuestionID = LastQuestionId;

                    db.C_tblQuestionList.Attach(model);
                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                   var info= db.C_tblExamTaken.Find(ExamTakenID);
                    info.TimeUsed = CurrentTime;
                    info.ActiveSubject = (int?)SubjectId;
                    db.Entry(info).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return new {value=1 };
                }

            }
            catch (Exception Ex)
            {

                return new { value = 0, msg = "Error Occured" };
            }


        }
        public object updateTopic()
        {
            try
            {
                //Int64 userid = (Int64)HttpContext.Current.Session["userid"];

                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var model = db.C_tblQuestionListTopic.Where(o => o.TopicID == SubjectId && o.ExamTakenId == ExamTakenID).FirstOrDefault();
                    model.ChoosenAnswerList = Answers;
                    model.LastQuestionID = LastQuestionId;
                    db.C_tblQuestionListTopic.Attach(model);
                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    var info = db.C_tblExamTaken.Find(ExamTakenID);
                    info.TimeUsed = CurrentTime;
                    info.ActiveSubject = (int?)SubjectId;
    
                    db.Entry(info).State = System.Data.Entity.EntityState.Modified;
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
    public class SubmitExam {
      public Int64 ExamTakenId { get; set; }
        public SubmitExam(long examtakenid) {
            this.ExamTakenId = examtakenid;
        }
        public object SaveExamAndCalcResultUTME() {
            try {
              
                using (var db = new EXAMSPURDBEntities()) {
                    db.Configuration.ProxyCreationEnabled = false;
                    var examInfo = db.C_tblExamTaken.Find(ExamTakenId);
                    if (examInfo == null) return new { value = 2, msg = "Cannot submit Exam at the moment" };
                    examInfo.Status = 2;
                    examInfo.EndTimestamp = DateTime.Now;
                    db.C_tblExamTaken.Attach(examInfo);
                    db.Entry(examInfo).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    var examD = db.C_tblQuestionList.Where(o => o.ExamTakenId == ExamTakenId).ToList();
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
                            for (var i = 0; i < choosenList.Split(',').Length; i++)
                            {
                                listAnswer[i] = choosenList.Split(',')[i];


                            }
                            for (var i = 0; i < answerList.Split(',').Length; i++)
                            {
                                if (answerList.Split(',')[i] == string.Join(",", listAnswer).Split(',')[i]) correctCount++;
                                else if (answerList.Split(',')[i] == "F") correctCount++;
                                else incorrectList.Add(qList.Split(',')[i] + "!" + string.Join(",", listAnswer).Split(',')[i]);
                                if (string.Join(",", listAnswer).Split(',')[i] != "" && string.Join(",", listAnswer).Split(',')[i] != null) attemptedCount++;
                            }
                            item.NoCorrect = correctCount;
                            item.NoAttempted = attemptedCount;
                            item.IncorrectAnswerList = String.Join(",", incorrectList);
                            Task<object> b = saveExamResult(item);
                        }
                    }
                    return new { value = 1 };
                }
            } catch (Exception ex) { return new { value = 0, msg = ex.Message }; };

        }
        public static async Task<object> saveExamResult(C_tblQuestionList item) {
            try
            {

                using (var db = new EXAMSPURDBEntities())
                {
                    db.C_tblQuestionList.Attach(item);

                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    var a = await db.SaveChangesAsync();
                    return a;
                }
            }
            catch (Exception ex) {
                return null;
            }
        }
    }

    public class GetCorrection {

        public int SubjectId { get; set; }
        public Int64 ExamTakenid { get; set; }
        public GetCorrection(int subjectid,  Int64 examtakenid) {
            this.ExamTakenid = examtakenid;
            this.SubjectId = subjectid;
        }
        public object getListOfCorrection() {
            try {
                using (var db = new EXAMSPURDBEntities())
                {
                    var questions = new QuestionForCoorrectionViewModel();
                    var data = new List<long?>() { };
                    var data2 = new List<string>() { };
                    db.Configuration.ProxyCreationEnabled = false;
                    var model = db.C_tblQuestionList.Where(o => o.SubjectID == SubjectId && o.ExamTakenId == ExamTakenid).FirstOrDefault();
                    if (model == null) return new { value = 0, msg = "Cannot Find the Correction to display" };
                    else
                    {
                        var list = model.QuestionList.Split(',');
                        var qlist = new List<C_tblQuestion>();

                        foreach (var item in list)
                        {
                            var qn = Convert.ToInt32(item);
                            var b = (C_tblQuestion)db.C_tblQuestion.Where(o => o.QuestionNumber == qn && o.SubjectID==SubjectId).FirstOrDefault();
                            qlist.Add(b);

                        }
                        if (model.ChoosenAnswerList != null)
                        {

                            var ans = model.ChoosenAnswerList.Split(',');
                            foreach (var item in ans)
                            {
                                data2.Add(item);
                            }
                        }
                        //var q = db.C_tblQuestionUTMETrial.Where(o => data.Contains(o.QuestionNumber));
                        questions.correctionQ = qlist.ToList();
                        questions.choosenAns = data2;
                        return new { value = 1, msg = questions };
                    }
                }
            }
            catch (Exception ex) {
                return new { value = 0,msg= "cannot load Correction at the moment. Try again" };
            }

        }
        public object getListOfCorrectionByTopic()
        {
            try
            {
                using (var db = new EXAMSPURDBEntities())
                {
                    var questions = new QuestionForCoorrectionViewModel();
                    var data = new List<long?>() { };
                    var data2 = new List<string>() { };
                    db.Configuration.ProxyCreationEnabled = false;
                    var model = db.C_tblQuestionListTopic.Where(o => o.TopicID == SubjectId && o.ExamTakenId == ExamTakenid).FirstOrDefault();
                    if (model == null) return new { value = 0, msg = "Cannot Find the Correction to display" };
                    else
                    {
                        var list = model.QuestionList.Split(',');
                        var qlist = new List<C_tblQuestion>();

                        foreach (var item in list)
                        {
                            var qn = Convert.ToInt32(item);
                            var b = (C_tblQuestion)db.C_tblQuestion.Where(o => o.QuestionNumber == qn).FirstOrDefault();
                            qlist.Add(b);

                        }
                        if (model.ChoosenAnswerList != null)
                        {

                            var ans = model.ChoosenAnswerList.Split(',');
                            foreach (var item in ans)
                            {
                                data2.Add(item);
                            }
                        }
                        //var q = db.C_tblQuestionUTMETrial.Where(o => data.Contains(o.QuestionNumber));
                        questions.correctionQ = qlist.ToList();
                        questions.choosenAns = data2;
                        return new { value = 1, msg = questions };
                    }
                }
            }
            catch (Exception ex)
            {
                return new { value = 0, msg = "cannot load Correction at the moment. Try again" };
            }

        }
        public object getListOfCorrectionWaec()
        {
            try
            {
                using (var db = new EXAMSPURDBEntities())
                {
                    var questions = new QuestionForCoorrectionViewModel();
                    var data = new List<long?>() { };
                    var data2 = new List<string>() { };
                    db.Configuration.ProxyCreationEnabled = false;
                    var model = db.C_tblQuestionList.Where(o => o.SubjectID == SubjectId && o.ExamTakenId == ExamTakenid).FirstOrDefault();
                    if (model == null) return new { value = 0, msg = "Cannot Find the Correction to display" };
                    else
                    {
                        var list = model.QuestionList.Split(',');
                        var qlist = new List<C_tblQuestion>();

                        foreach (var item in list)
                        {
                            var qn = Convert.ToInt32(item);
                            var b = (C_tblQuestion)db.C_tblQuestion.Where(o => o.QuestionNumber == qn && o.SubjectID==SubjectId).FirstOrDefault();
                            qlist.Add(b);

                        }
                        if (model.ChoosenAnswerList != null)
                        {

                            var ans = model.ChoosenAnswerList.Split(',');
                            foreach (var item in ans)
                            {
                                data2.Add(item);
                            }
                        }
                        //var q = db.C_tblQuestionUTMETrial.Where(o => data.Contains(o.QuestionNumber));
                        questions.correctionQ = qlist.ToList();
                        questions.choosenAns = data2;
                        return new { value = 1, msg = questions };
                    }
                }
            }
            catch (Exception ex)
            {
                return new { value = 0, msg = "cannot load Correction at the moment. Try again" };
            }

        }
    }

    public class QuestionForCoorrectionViewModel {
        public List<C_tblQuestion> correctionQ { get; set; }
        public List<string> choosenAns { get; set; }
       

    }
    public class QuestionForCoorrectionWaecViewModel
    {
        public List<C_tblQuestionWAEC> correctionQ { get; set; }
        public List<string> choosenAns { get; set; }


    }
    public static class setTimer
    {
        public static object updateTimer(int timeInSec, Int64 ExamTakenID)
        {
            try
            {
                //Int64 userid = (Int64)HttpContext.Current.Session["userid"];

                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    var info = db.C_tblExamTaken.Find(ExamTakenID);
                    info.TimeUsed = timeInSec;
                    db.Entry(info).State = System.Data.Entity.EntityState.Modified;
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

    public static class LoadAllSubjects
    {
        public static object loadSub()
        {
            using (var db = new EXAMSPURDBEntities())
            {
                db.Configuration.LazyLoadingEnabled = true;
                db.Configuration.ProxyCreationEnabled = false;
                var model = db.C_tblSubjectGeneral.Where(o => o.ExamID == 1000).ToList();
               // var model = db.C_tblSubjectGeneral.ToList();
               


                return new { value =1, subject = model };
                

            }

        }

    }


    public class SubmitExamByTopic
    {
        public Int64 ExamTakenId { get; set; }

        public SubmitExamByTopic(long examtakenid)
        {
            this.ExamTakenId = examtakenid;
        }


        public object SaveExamAndCalcResultTOPICS()
        {
            try
            {

                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var examInfo = db.C_tblExamTaken.Find(ExamTakenId);
                    if (examInfo == null) return new { value = 2, msg = "Cannot submit Exam at the moment" };
                    examInfo.Status = 2;
                    examInfo.EndTimestamp = DateTime.Now;
                    db.C_tblExamTaken.Attach(examInfo);
                    db.Entry(examInfo).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    var examD = db.C_tblQuestionListTopic.Where(o => o.ExamTakenId == ExamTakenId).ToList();
                    foreach (var item in examD)
                    {
                        var incorrectList = new List<string>();
                        int correctCount = 0;
                        var attemptedCount = 0;
                        var answerList = item.CorrectAnswerList;
                        var choosenList = item.ChoosenAnswerList;
                        var qList = GetQList(item);
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
                            for (var i = 0; i < choosenList.Split(',').Length; i++)
                            {
                                listAnswer[i] = choosenList.Split(',')[i];


                            }
                            for (var i = 0; i < answerList.Split(',').Length; i++)
                            {
                                if (answerList.Split(',')[i] == string.Join(",", listAnswer).Split(',')[i]) correctCount++;
                                else if (answerList.Split(',')[i] == "F") correctCount++;
                                else incorrectList.Add(qList.Split(',')[i] + "!" + string.Join(",", listAnswer).Split(',')[i]);
                                if (string.Join(",", listAnswer).Split(',')[i] != "" && string.Join(",", listAnswer).Split(',')[i] != null) attemptedCount++;
                            }
                            item.NoCorrect = correctCount;
                            item.NoAttempted = attemptedCount;
                            item.IncorrectAnswerList = String.Join(",", incorrectList);
                            Task<object> b = saveExamResult(item);
                        }
                    }
                    return new { value = 1 };
                }
            }
            catch (Exception ex) { return new { value = 0, msg = ex.Message }; };

        }

        private static string GetQList(C_tblQuestionListTopic item)
        {
            return item.QuestionList;
        }

        public static async Task<object> saveExamResult(C_tblQuestionListTopic item)
        {
            try
            {

                using (var db = new EXAMSPURDBEntities())
                {
                    db.C_tblQuestionListTopic.Attach(item);

                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    var a = await db.SaveChangesAsync();
                    return a;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }

}