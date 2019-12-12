using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ExamSpur.Models
{
    public class ExamModelWaec
    {
    }

    public class SubjectViewModelWaec
    {
        public Int64 ExamId { get; set; }
        public SubjectViewModelWaec(Int64 examid)
        {
            this.ExamId = examid;
        }

        public object loadSubject()
        {
            try
            {
                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var subject = db.C_tblSubjectGeneral.Where(o => o.ExamID == ExamId).ToList();
                    return new { value = 1, resp = subject };
                }
            }
            catch (Exception ex)
            {
                return new { value = 0, resp = "Error Occured. Try again Later" };

            }

        }
    }
    public class GenerateQuestionsWaec
    {
        public int examId { get; set; }
        public int examMode { get; set; }
        public string subjectIds { get; set; }
        public Int64 studentId { get; set; }

        public GenerateQuestionsWaec(int examid, int exammode, string subjectids, Int64 studentid)
        {
            examId = examid;
            examMode = exammode;
            subjectIds = subjectids;
            studentId = studentid;

        }
        public object saveExamInstanceAndGeneratequestionWAEC()
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
                    examInfo.TotalTime = 5400; //seconds
                    examInfo.StartTimestamp = DateTime.Now;
                    db.C_tblExamTaken.Add(examInfo);
                    db.SaveChanges();
                    var student = db.C_tblStudentReg.Find(studentId);
                    
                    foreach (var item in subjectIds.Split(','))
                    {
                        var citem = Convert.ToInt32(item);
                        var sub = db.C_tblSubjectGeneral.Find(citem);
                        var sub2 = db.C_tblQuestion.Where(o => o.SubjectID == citem).ToList();
                   //     var maxNoOFQuestionAvail = sub2.Max(o => o.QuestionNumber);
                  //     var minxNoOFQuestionAvail = sub2.Min(o => o.QuestionNumber);
                        var maxQ = (int)sub.NoOfQuestions;
                        var randomQuestion = new int[maxQ];
                        var listAnswer = new string[maxQ];
                        Random random = new Random();
                        for (int i = 0; i < maxQ; i++)
                        {

                          
                        ln: int randomNumber = random.Next((int)0, (int)sub2.Count()-1);
                      var q = (int)sub2.ElementAt(randomNumber).QuestionNumber;
                     //       if (!randomQuestion.Contains(q)) goto ln;
                            if (!randomQuestion.Contains(randomNumber))
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
                                        if (j < questionwithImage.Count - 1) { i++; }
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
                        respQ.Add(item + "%" + string.Join("-", randomQuestion));
                        // choosenAnswer.Add(string.Join(",", saveQ.ChoosenAnswerList));
                    }
                    if (student.Unit != 99999) student.Unit -= 1;
                    db.C_tblStudentReg.Attach(student);
                    db.Entry(student).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    HttpContext.Current.Session["unitAvailable"] = student.Unit;
                    return new { value = 1, Qlist = string.Join(",", respQ), exammode = examMode, examtakenid = examInfo.ExamTakenID };
                }
            }
            catch (Exception ex)
            {
                return new { value = 0, resp = "Error Occured. Try again Later" };

            }

        }
    }
    public class CheckExamWaec
    {

        public Int64 ExamId { get; set; }
        public CheckExamWaec(Int64 examid)
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
    public class checklogoutWaec
    {
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
    public class GetNextQuestionWaec
    {

        public Int64 SubjectId { get; set; }
        public Int64 QuestionNo { get; set; }
        public GetNextQuestionWaec(Int64 subjectid, Int64 questionno)
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
    public class UpadateAnswersWaec
    {

        public Int64 SubjectId { get; set; }
        public Int64 ExamTakenID { get; set; }
        public int LastQuestionId { get; set; }
        public string Answers { get; set; }


        public int CurrentTime { get; set; }


        public UpadateAnswersWaec(Int64 subjectid, Int64 examtakenid, string answers, int curentTime,int lastQuestionId)
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
                    var model = db.C_tblQuestionList.Where(o => o.SubjectID == SubjectId && o.ExamTakenId == ExamTakenID).FirstOrDefault();
                    model.ChoosenAnswerList = Answers;
                    model.LastQuestionID = LastQuestionId;
                    db.C_tblQuestionList.Attach(model);
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
    public class SubmitExamWaec
    {
        public Int64 ExamTakenId { get; set; }
        public SubmitExamWaec(long examtakenid)
        {
            this.ExamTakenId = examtakenid;
        }
        public object SaveExamAndCalcResultWAEC()
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
                            Task<object> b = SubmitExamWaec.saveExamResult(item);
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
        public static async Task<object> saveExamResult(C_tblQuestionList item)
        {
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
            catch (Exception ex)
            {
                return null;
            }
        }
    }
    public class GetCorrectionWaec
    {

        public int SubjectId { get; set; }
        public Int64 ExamTakenid { get; set; }
        public GetCorrectionWaec(int subjectid, Int64 examtakenid)
        {
            this.ExamTakenid = examtakenid;
            this.SubjectId = subjectid;
        }
        public object getListOfCorrection()
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

    public static class setTimerWaec
    {
        public static object updateTimerWaec(int timeInSec, Int64 ExamTakenID)
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
}