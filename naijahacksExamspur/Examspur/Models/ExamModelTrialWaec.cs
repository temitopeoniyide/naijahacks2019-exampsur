using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ExamSpur.Models
{
    public class ExamModelTrialWaec
    {
    }
    public class GenerateQuestionsTrialWaec
    {
        public int examId { get; set; }
        public int examMode { get; set; }
        public string subjectIds { get; set; }
        public Int64 studentId { get; set; }

        public GenerateQuestionsTrialWaec(int examid, string subjectids)
        {
            examId = examid;

            subjectIds = subjectids;


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
                    examInfo.ExamMode = 3;
                    examInfo.StudentID = 1015;
                    examInfo.Status = 1;
                    examInfo.TimeUsed = 0;
                    examInfo.TotalTime = 300; //seconds
                    examInfo.StartTimestamp = DateTime.Now;
                    db.C_tblExamTaken.Add(examInfo);
                    db.SaveChanges();
                    foreach (var item in subjectIds.Split(','))
                    {
                        var citem = Convert.ToInt32(item);
                        var sub = db.C_tblSubjectGeneral.Find(citem);
                        var sub2 = db.C_tblQuestionWAECTrial.Where(o => o.SubjectID == citem);
                        var maxNoOFQuestionAvail = sub2.Max(o => o.QuestionNumber);
                        var minxNoOFQuestionAvail = sub2.Min(o => o.QuestionNumber);
                        var maxQ = (int)sub.NoOfQuestions;
                        var randomQuestion = new int[5];
                        var listAnswer = new string[5];

                        for (int i = 0; i < 5; i++)
                        {

                        ln: Random random = new Random();
                            int randomNumber = random.Next((int)minxNoOFQuestionAvail, (int)maxNoOFQuestionAvail);
                            if (randomNumber < minxNoOFQuestionAvail || randomNumber > maxNoOFQuestionAvail) goto ln;
                            if (!randomQuestion.Contains(randomNumber))
                            {
                                var questiondetail = db.C_tblQuestionWAECTrial.Where(o => o.QuestionNumber == randomNumber && o.SubjectID == citem).FirstOrDefault();
                                if (questiondetail == null) goto ln;
                                if (questiondetail.HasImage >= 1)
                                {
                                    var questionwithImage = db.C_tblQuestionWAECTrial.Where(o => o.ImageId == questiondetail.ImageId && o.SubjectID == citem).OrderBy(o => o.QuestionNumber).ToList();
                                    for (int j = 0; j < questionwithImage.Count; j++)
                                    {

                                        randomQuestion[i] = (int)questionwithImage[j].QuestionNumber;
                                        listAnswer[i] = questionwithImage[j].Answer;


                                        if (i == 5 - 1) { break; }
                                        if (j < questionwithImage.Count - 1) { i++; }
                                    }

                                }
                                else
                                {
                                    randomQuestion[i] = randomNumber;
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

                    return new { value = 1, Qlist = string.Join(",", respQ), exammode = 3, examtakenid = examInfo.ExamTakenID };
                }
            }
            catch (Exception ex)
            {
                return new { value = 0, resp = "Error Occured. Try again Later" };

            }

        }
    }
    public class CheckExamTWaec
    {

        public Int64 ExamId { get; set; }
        public CheckExamTWaec(Int64 examid)
        {
            this.ExamId = examid;
        }
        public object CheckforOngoingExam()
        {
            try
            {
                Int64 userid = (Int64)HttpContext.Current.Session["userid"];

                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var ExamTaken = db.C_tblExamTaken.Where(o => o.ExamID == ExamId && o.StudentID == userid && o.Status == 1 && o.ExamMode == 2).FirstOrDefault();
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
                                     }

                                     );

                        return new { value = 1, Model = model };
                    }
                }

            }
            catch (Exception Ex)
            {

                return new { value = 0, msg = "Error Occured" };
            }


        }
    }
    public class GetNextQuestionTrialWaec
    {

        public Int64 SubjectId { get; set; }
        public Int64 QuestionNo { get; set; }
        public GetNextQuestionTrialWaec(Int64 subjectid, Int64 questionno)
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
                    var model = db.C_tblQuestionWAECTrial.Where(o => o.SubjectID == SubjectId && o.QuestionNumber == QuestionNo).FirstOrDefault();
                    return model;
                }

            }
            catch (Exception Ex)
            {

                return new { value = 0, msg = "Error Occured" };
            }


        }
    }

    public class UpadateAnswersTWaec
    {

        public Int64 SubjectId { get; set; }
        public Int64 ExamTakenID { get; set; }

        public string Answers { get; set; }


        public int CurrentTime { get; set; }

        public UpadateAnswersTWaec(Int64 subjectid, Int64 examtakenid, string answers, int curentTime)
        {
            this.SubjectId = subjectid;
            this.ExamTakenID = examtakenid;
            this.Answers = answers;
            this.CurrentTime = curentTime;
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
                    db.C_tblQuestionList.Attach(model);
                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    var info = db.C_tblExamTaken.Find(ExamTakenID);
                    info.TimeUsed = CurrentTime;
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
    public class SubmitExamTrialWaec
    {
        public Int64 ExamTakenId { get; set; }
        public SubmitExamTrialWaec(long examtakenid)
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
                            Task<object> b = SubmitExamTrialWaec.saveExamResult(item);
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
                                else incorrectList.Add(qList.Split(',')[i] + "!" + answerList.Split(',')[i]);
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
    public class GetCorrectionTrialWAEC
    {

        public int SubjectId { get; set; }
        public Int64 ExamTakenid { get; set; }
        public GetCorrectionTrialWAEC(int subjectid, Int64 examtakenid)
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
                    var questions = new QuestionForCoorrectionViewModelTrialWaec();
                    var data = new List<long?>() { };
                    var data2 = new List<string>() { };
                    db.Configuration.ProxyCreationEnabled = false;
                    var model = db.C_tblQuestionList.Where(o => o.SubjectID == SubjectId && o.ExamTakenId == ExamTakenid).FirstOrDefault();
                    if (model == null) return new { value = 0, msg = "Cannot Find the Correction to display" };
                    else
                    {
                        var list = model.QuestionList.Split(',');
                        var qlist = new List<C_tblQuestionWAECTrial>();

                        foreach (var item in list)
                        {
                            var qn = Convert.ToInt32(item);
                            var b = (C_tblQuestionWAECTrial)db.C_tblQuestionWAECTrial.Where(o => o.QuestionNumber == qn).FirstOrDefault();
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
                        //var q = db.C_tblQuestionWAECTrial.Where(o => data.Contains(o.QuestionNumber));
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
    public class QuestionForCoorrectionViewModelTrialWaec
    {
        public List<C_tblQuestionWAECTrial> correctionQ { get; set; }
        public List<string> choosenAns { get; set; }


    }

}