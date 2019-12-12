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
    public class EnterQuestionHere
    {
    }


    public class AddQuestion
    {

        public string Question { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }
        public string OptionE { get; set; }
        public string Answer { get; set; }
        public string Solution { get; set; }
        public int HasImage { get; set; }
        public int ImageId { get; set; }

        public int newImage { get; set; }


        public AddQuestion(string question,string optionA, string optionB, string optionC, string optionD, string optionE, string answer, string solution, int frameWork, int imageno,int newImage)
        {
            this.Question = question;
            this.OptionA = optionA;
            this.OptionB = OptionB;
            this.OptionC = optionC;
            this.OptionD = OptionD;
            this.OptionE = optionE;
            this.Answer = answer;
            this.Solution = solution;
            this.HasImage = frameWork;
            this.ImageId = imageno;
        }


        public object saveNewQuestion()
        {
            try
            {
                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var examid = Convert.ToInt64(HttpContext.Current.Session["examid"]);
                    var subjectid = Convert.ToInt32(HttpContext.Current.Session["subjectid"]);
                    if (examid == 1000)
                    {
                        var model = db.C_tblQuestion.Where(o => o.SubjectID == subjectid).ToList();
                        var lastQuestionNumber = model.Max(o => o.QuestionNumber);

                        if (newImage == 0&& HasImage==1)
                        {
                            if (ImageId != lastQuestionNumber) return new { value = 0, msg = "Enter valid Image Number" };
                        }
                        var Quest = new C_tblQuestion();

                        Quest.Question = Question;
                        Quest.OptionA = OptionA;
                        Quest.OptionB = OptionB;
                        Quest.OptionC = OptionC;
                        Quest.OptionD = OptionD;
                        Quest.OptionE = OptionE;
                        Quest.Answer = Answer;
                        Quest.Solution = Solution;
                        Quest.HasImage = HasImage;
                        Quest.SubjectID = subjectid;
                        Quest.QuestionNumber = lastQuestionNumber + 1;
                        Quest.ImageId = ImageId;
                        Quest.UserName = HttpContext.Current.Session["username"].ToString();

                        db.C_tblQuestion.Add(Quest);
                        db.SaveChanges();


                        return new { value = 1, msg = "Success",count= model.Count()+1 };
                    }
                    else
                    {

                        var model = db.C_tblQuestionWAEC.Where(o => o.SubjectID == subjectid).ToList();
                        var lastQuestionNumber = model.Max(o => o.QuestionNumber);

                        if (newImage == 0 && HasImage==1)
                        {
                            if (ImageId != lastQuestionNumber) return new { value = 0, msg = "Enter valid Image Number" };
                        }
                        var Quest = new C_tblQuestionWAEC();

                        Quest.Question = Question;
                        Quest.OptionA = OptionA;
                        Quest.OptionB = OptionB;
                        Quest.OptionC = OptionC;
                        Quest.OptionD = OptionD;
                        Quest.OptionE = OptionE;
                        Quest.Answer = Answer;
                        Quest.Solution = Solution;
                        Quest.HasImage = HasImage;
                        Quest.SubjectID = subjectid;
                        Quest.QuestionNumber = lastQuestionNumber + 1;
                        Quest.ImageId = ImageId;
                       Quest.UserName = HttpContext.Current.Session["username"].ToString();


                        db.C_tblQuestionWAEC.Add(Quest);
                        db.SaveChanges();


                        return new { value = 1, msg = "Success" };
                    }
                }
            }
            catch (Exception Ex)
            {
                return new { value = 0, msg = Ex.Message };
            }










        }

    }
    public static class LoadThisQuestion
    {
        public static object loadQuest()
        {
            using (var db = new EXAMSPURDBEntities())
            {

                
                //db.Configuration.LazyLoadingEnabled = true;
                db.Configuration.ProxyCreationEnabled = false;
                var quest = db.C_tblQuestion.Where(o=>o.SubjectID==1002).ToList();
               


                return new { questions = quest };


            }

        }

    }

}