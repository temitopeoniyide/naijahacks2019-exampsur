using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ExamSpur.Models
{
    public class QuestionModel
    {
        
            public string questionHere { get; set; }
            public string optionA { get; set; }
            public string optionB { get; set; }
            public string optionC { get; set; }
            public string optionD { get; set; }
           public string optionE { get; set; }
           public string Answer { get; set; }
           public string HasImage { get; set; }
           public string imageId { get; set; }
        public Int32 subjectId { get; set; }
        public QuestionModel(string question, string optiona, string optionb, string optionc, string optiond, string optione, string answer, string hasimage, string imageno,Int32 subjectId)
            {
                this.questionHere = question;
                this.optionA = optiona;
                this.optionB = optionb;
                this.optionC = optionc;
                this.optionD = optiond;
                this.optionE = optione;
                this.Answer = answer;
                this.HasImage = hasimage;
                this.imageId = imageno;
            this.subjectId = subjectId;

        }
            public object saveNewQuestion()
            {
                try
                {
                    using (var db = new EXAMSPURDBEntities())
                    {
                       


                        var student = new C_tblQuestion();
                        student.Question = questionHere;
                        student.OptionA = optionA;
                        student.OptionB = optionB;
                        student.OptionC = optionC;
                        student.OptionD = optionD;
                        student.OptionE = optionE;
                        student.Answer = Answer;
                    var maxQ = db.C_tblQuestion.Where(o => o.SubjectID == subjectId).Max(o => o.QuestionNumber);
                    student.QuestionNumber = maxQ==null?1:(maxQ) + 1;
                       student.SubjectID = subjectId;
                    student.trial = (false);
                    var hasimage = HasImage.ToString().ToLower() == "yes" ? 1 : 0;
                    student.HasImage = Convert.ToInt32(hasimage);
                        student.ImageId = Convert.ToInt32(imageId);
                   
                    

                        db.C_tblQuestion.Add(student);
                        db.SaveChanges();
                       
                        return new { value = 1, msg = "Saved Successfully" };
                    }
                }
                catch (Exception Ex)
                {
                    return new { value = 0, msg = Ex.Message };
                }
            }
        }

    }