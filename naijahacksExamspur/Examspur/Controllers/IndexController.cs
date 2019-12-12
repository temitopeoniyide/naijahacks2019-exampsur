using ExamSpur.Models;
using ExamSpur.Views.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ExamSpur.App_Start.checkSession;

namespace ExamSpur.Controllers
{
    //public class QuestionHere
    //{
    //    public string subjectName { get; set; }
    //    public string lastImageId { get; set; }

    //}


    public class IndexController : Controller
    {
        // GET: Home
        public ActionResult Home()
        {
            var a = getdashinfo.getdashboardInfo();
            return View(a);
        }
        public ActionResult SubscriptionHistory()
        {
            var resp = GetMerchantSub.MerchantSubHistory();
        
            return View(resp);
        }
        // GET: Instruction
        public ActionResult AdminDashboard() {

            return View();
        }

        public ActionResult ViewQuestion()
        {

            return View();
        }

        public ActionResult TOPICS()
        {

            return View();
        }

        public ActionResult EnterQuestionHere(int id)
        {
            Session["subjectid"] = id;
            var examid =Convert.ToInt64( Session["examid"]);
            using (var db = new EXAMSPURDBEntities())
            {
                var subjectname = db.C_tblSubjectGeneral.Find(id).SubjectName;
                //var examid = Convert.ToInt64(Session["examid"]);
                if (examid == 1000)
                {
                    var model = db.C_tblQuestion.Where(o => o.SubjectID == id).ToList();
                    var lastQuestionNumber = model.Max(o => o.QuestionNumber);
                    var totalQuestion = model.Count();
                    return View(new questionHerem { lastImageId = lastQuestionNumber, count = totalQuestion, subjectName = subjectname });
                }
                else
                {
                    var model = db.C_tblQuestionWAEC.Where(o => o.SubjectID == id).ToList();
                    var lastQuestionNumber = model.Max(o => o.QuestionNumber);
                    var totalQuestion = model.Count();
                    return View(new questionHerem { lastImageId = lastQuestionNumber, count = totalQuestion, subjectName = subjectname });

                }
            };
        }
        


        public ActionResult LoadSubject()
        {

            return View();
        }


        public ActionResult AddTopics()
        {

            return View();
        }


        public ActionResult LoadTopics()
        {

            return View();
        }

        public ActionResult AssociationDashBoard()
        {

            return View(ATSOgetinfoModel.getinfo());
        }

        public ActionResult OurPartnersDashboard()
        {

            return View(PartnersgetinfoModel.getinfo());
        }


        public ActionResult ElderDashBoar()
        {

            return View(EldergetinfoModel.getinfo());
        }

        public ActionResult mcadwareDashBoard()
        {

            return View(McadwaregetinfoModel.getinfo());
        }

       


        [SessionTimeout]
        public ActionResult UTME()
    {
        return View();
    }
        // GET: ExamHall
        [SessionTimeout]


     
        public ActionResult WAEC1()
        {
            return View();
        }
        // GET: ExamHall
        [SessionTimeout]


        public ActionResult Dashboard()
        {
            var a = getdashinfoMerchant.getdashboardinfo();
            return View(a);
        }
        [HttpPost]
        public ActionResult ChangeExamMerchant(int? examid)
        {
            var a = getdashinfoMerchant.getdashboardinfo(examid);
            return Json(a);
        }
        // GET: CorrectionTable
        public ActionResult UTMEPerformance()
        {
            return View();
        }
        // GET: ResultSummary
        [SessionTimeout]

        public ActionResult ResultSummary()
        {
            var model =  ResultViewModel.result();
            return View(model);
        }
        public ActionResult ResultSummaryWaec()
        {
            var model = ResultViewModelWaec.result();
            return View(model);
        }
        [HttpPost]
        public ActionResult ResultSummaryForMerchant(Int64 userid)
        {
            var model = ResultViewModel.resultForMerchant(userid);
            return Json(model);
        }
        [HttpPost]
        public ActionResult ResultSummaryWaecForMerchant(Int64 userid)
        {
            var model = ResultViewModelWaec.resultForMerchant(userid);
            return Json(model);
        }
        // GET:Result Summary


        public ActionResult ResultDetails(String examId, Int32 trial) {
            if (trial == 0) { 
            var a = new resultDetail(Convert.ToInt64(examId));
            var model = a.result();
            return PartialView("ResultDetails", model); }
            else{
                var a = new resultDetailT(Convert.ToInt64(examId));
                var model = a.result();
                return PartialView("ResultDetails", model);
            }
        }
        public ActionResult ResultDetailsWaec(String examId, Int32 trial)
        {
            if (trial == 0)
            {
                var a = new resultDetailWaec(Convert.ToInt64(examId));
                var model = a.result();
                return PartialView("ResultDetailsWaec", model);
            }
            else
            {
                var a = new resultDetailTWaec(Convert.ToInt64(examId));
                var model = a.result();
                return PartialView("ResultDetailsWaec", model);
            }
        }
        // resultDetail.ExamID =Convert.ToInt64(examId);


        // GET: PostUTME
        public ActionResult PostUTME()
        {
            return View();

        }

        public ActionResult WAECPerformance()
        {
            return View();
        }

        // GET: RegisterStudent
        public ActionResult RegisterStudent()
        {
            var a = getStudent.getstudent();
            return View(a);
        }
        [HttpPost]
        public ActionResult loadMechantstudent()
        {
            var a = getStudent.getstudent();
            return Json(a);
        }
        // GET: StudentResult
        public ActionResult StudentResult()
        {
           
            return View();
        }
        public ActionResult PartnerDashboard()
        {

            return View(getinfoModel.getinfo());
        }
        // GET: Website
        public ActionResult Website()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ResultSummaryAjax()
        {
            var model = ResultViewModel.result();
            return Json(model);
        }
        [HttpPost]
        public ActionResult ResultSummaryAjaxWaec()
        {
            var model = ResultViewModelWaec.result();
            return Json(model);
        }
        [HttpPost]
        public ActionResult ResultSummarybyExam()
        {
            var model = ResultViewModel.result();
            return Json(model);
        }

    
        [HttpPost]
        public JsonResult DisplayQuestionToAddToTopic(int qid)
        {
            var reg = new DisplayTopicViewModel(qid);
            var response = reg.DisplayTopicToQuestionssModel();
            return Json(response);
        }
        

        [HttpPost]
        public JsonResult AttarchQuestionToTopics(int subjectID, int questionID, int topicID)
        {
            var reg = new AttarchTopicsToQuestion(subjectID, questionID, topicID);
            var response = reg.updateTopicsToQuestionTable();
            return Json(response);
        }


        [HttpPost]
        public JsonResult loadTopics(int subjectId)
        {
            var reg = new TopicViewModel(subjectId);
            var response = reg.loadTopicsModel();
            return Json(response);
        }
        [HttpPost]
        public JsonResult loadTopics2(int subjectId)
        {
            var reg = new TopicViewModel(subjectId);
            var response = reg.loadTopicsModel2();
            return Json(response);
        }
        [HttpPost]
        public JsonResult loadAllSubj()
        {
            var resp = LoadAllSubjects.loadSub();
            return Json(resp);
        }



        [HttpPost]
        public JsonResult loadSubject(Int64 examId)
        {
            var reg = new SubjectViewModel(examId);
            var response = reg.loadSubject();
            return Json(response);
        }


        [HttpPost]
        public JsonResult loadTopicssss(int subjectId)
        {
            var reg = new TopicsssViewModel(subjectId);
            var response = reg.loadTopicssss();
            return Json(response);
        }


        [HttpPost]
        public JsonResult checkIfExamIsOngoing(Int64 examId)
        {
            var reg = new CheckExam(examId);
            var response = reg.CheckforOngoingExam();
            return Json(response);
        }
        [HttpPost]
        public JsonResult checkIfExamIsOngoingTopic(Int64 examId)
        {
            var reg = new CheckExam(examId);
            var response = reg.CheckforOngoingExamTopic();
            return Json(response);
        }
        [HttpPost]
        public JsonResult getQuestionDetail(Int64 subjectid,Int64 questionno)
        {
            var reg = new GetNextQuestion(subjectid,questionno);
            var response = reg.Question();
            return Json(response);
        }



        [HttpPost]
        public JsonResult getQuestionDetailbyTopic(Int64 topicid, Int64 questionno)
        {
            var reg = new GetNextQuestionbyTopic(topicid, questionno);
            var response = reg.QuestionbyTopic();
            return Json(response);
        }





        [HttpPost]
        public JsonResult getQuestionDetailTrial(Int64 subjectid, Int64 questionno)
        {
            var reg = new GetNextQuestionTrial (subjectid, questionno);
            var response = reg.Question();
            return Json(response);
        }
        public JsonResult getQuestionDetailWaec(Int64 subjectid, Int64 questionno)
        {
            var reg = new GetNextQuestionWaec(subjectid, questionno);
            var response = reg.Question();
            return Json(response);
        }
        [HttpPost]
        public JsonResult getQuestionDetailTrialWaec(Int64 subjectid, Int64 questionno)
        {
            var reg = new GetNextQuestionTrialWaec(subjectid, questionno);
            var response = reg.Question();
            return Json(response);
        }
       



        [HttpPost]
        public JsonResult loadStateandBank()
        {
            var resp = new Profile();
            var response = resp.loadstateandbank();
            return Json(response);
        }

        [HttpPost]
        public JsonResult LoadStudentProfile()
        {
            var resp = new LoadStudent();
            var response = resp.loadStudent();
            return Json(response);
        }


        [HttpPost]
        public JsonResult LoadMerchantProfile()
        {
            var resp = new LoadMerchant();
            var response = resp.loadMerchant();
            return Json(response);
        }


        [HttpPost]
        public JsonResult LoadMarketerProfile()
        {
            var resp = new LoadMarketers();
            var response = resp.loadMarketer();
            return Json(response);
        }






        [HttpPost]
        public JsonResult loadprofileMarketer()
        {
            var resp = new Profile();

            var response = resp.loadProfileMarketer();
            return Json(response);
        }

        [HttpPost]
        public JsonResult UpdateProfileMarketer(string address, int stateid, string accountno, string bankcode, string bankacctname)
        {
            var resp = new UpdateProfileMarketer(address,stateid, accountno, bankcode, bankacctname);
            return Json(resp.update());
        }


        [HttpPost]
        public JsonResult UpdatePayment(decimal? atso, decimal? partners, decimal? elder, decimal? trevenue,int maxid)
        {
            var resp = new UpdatePaymentPartner(atso,partners,elder, trevenue,Convert.ToInt64(maxid));
            return Json(resp.updatePay());
        }


        [HttpPost]
        public JsonResult validateAccount(string code,  string accountno)
        {
            var resp = new ValidateAccount(code,accountno);
            return Json(resp.getaccountname());
        }
        [HttpPost]
        public JsonResult loadclass() {
            var resp = loadClass.loadclassByMerchantID();
            return Json(resp);
        }
    
             [HttpPost]
        public JsonResult loadMechantuser()
        {
            var resp = loadMechantusers.loadUsers();
            return Json(resp);
        }

        [HttpPost]

        public JsonResult deleteMerchantUser(int id) {
            var resp = loadMechantusers.deleteUser(id);
            return Json(resp);

        }
        [HttpPost]

        public JsonResult updateRole(int userId,int roleId)
        {
            var resp = loadMechantusers.updateMerchantUserRole(userId,roleId);
            return Json(resp);

        }
        [HttpPost]
        public JsonResult loadClassCount()
        {
            var resp = loadClass.loadstudentCountByClass();
            return Json(resp);
        }
        [HttpPost]
        public JsonResult addclass(string className) {
            var resp = loadClass.addClassByMerchantID(className);
                return Json(resp);
        }
        [HttpPost]
        public JsonResult renameClass(int classID,String className)
        {
            var resp = loadClass.renameClass(classID,className);
            return Json(resp);
        }
        [HttpPost]
        public JsonResult deleteStudentByClass (int classID)
        {
            var resp = loadClass.deleteStudentByClass(classID);
            return Json(resp);
        }
        [HttpPost]
        public JsonResult deleteStudentByID(int studentID)
        {
            var resp = loadClass.deleteStudentByID(studentID);
            return Json(resp);
        }
        [HttpPost]
        public JsonResult detachStudent()
        {
            var resp = dettach.dettachStudent(); 
            return Json(resp);
        }
        [HttpPost]
        public JsonResult subHistory()
        {
            var resp = GetMerchantSub.MerchantSubHistory();
            return Json(resp);
        }
        [HttpPost]
        public JsonResult moveStudentByClass(int oldclassID, int newclassID)
        {
            var resp = loadClass.moveStudentByClass(oldclassID,newclassID);
            return Json(resp);
        }
        
         [HttpPost]
        public JsonResult loadResultByClass(int classId, int examId)
        {

            var resp = StudentResultSummary.ClassResultSummaryByClass(classId,examId);
            return Json(resp);
        }
        [HttpPost]
        public JsonResult loadResultByClassandSubjectId(int classId, int examId,int subjectId)
        {
            var resp = StudentResultSummary.ClassResultSummaryByClassAndSubject(classId, examId,subjectId);
            return Json(resp);
        }
        [HttpPost]
        public JsonResult assignUnit(int classId, int unit)
        {
            var resp = new AssignUnit(classId, unit);
            return Json(resp.assign());
        }
        [HttpPost]
        public JsonResult assignUnitInd(int studentid, int unit)
        {
            var resp = new AssignUnitInd(studentid, unit);
            return Json(resp.Assign());
        }
        [HttpPost] 
        public JsonResult getexam(int examid)
        {
            var resp = getdashinfo.getexam(examid);
            return Json(resp);
        }
        [HttpPost]
        public JsonResult correction(int subjectid, Int64 examtakenid)
        {
            var correction = new GetCorrection(subjectid, examtakenid);
            var response = correction.getListOfCorrection();
            return Json(response);
        }
        
        [HttpPost]
        public JsonResult correctionWaec(int subjectid, Int64 examtakenid)
        {
            var correction = new GetCorrection(subjectid, examtakenid);
            var response = correction.getListOfCorrectionWaec();
            return Json(response);
        }
        [HttpPost]
        public JsonResult correctionTrial(int subjectid, Int64 examtakenid)
        {
            var correction = new GetCorrectionTrial(subjectid, examtakenid);
            var response = correction.getListOfCorrection();
            return Json(response);
        }
        [HttpPost]
        public JsonResult correctionTrialWaec(int subjectid, Int64 examtakenid)
        {
            var correction = new GetCorrectionTrialWAEC(subjectid, examtakenid);
            var response = correction.getListOfCorrection();
            return Json(response);
        }
        [HttpPost]
        public JsonResult updateAnswers(Int64 subjectid, string answers, int currentTime, Int64 examTakenId, int LastQuestionID)
        {
            var reg = new UpadateAnswers(subjectid,examTakenId,answers,currentTime, LastQuestionID);
            var response = reg.update();
            return Json(response);
        }
        [HttpPost]
        public JsonResult updateAnswersTopic(Int64 topicid, string answers, int currentTime, Int64 examTakenId,int LastQuestionID)
        {
            var reg = new UpadateAnswers(topicid, examTakenId, answers, currentTime, LastQuestionID);
            var response = reg.updateTopic();
            return Json(response);
        }
        [HttpPost]
        public JsonResult updateAnswersWaec(Int64 subjectid, string answers, int currentTime, Int64 examTakenId, int LastQuestionID)
        {
            var reg = new UpadateAnswersWaec(subjectid, examTakenId, answers, currentTime, LastQuestionID);
            var response = reg.update();
            return Json(response);
        }
        [HttpPost]
        public JsonResult updateTimer(int currentTime, Int64 examTakenId)
        {
            var setTime = setTimer.updateTimer(currentTime, examTakenId);
           
            return Json(setTime);
        }
        [HttpPost]
        public JsonResult updateAnswersTrial(Int64 subjectid, string answers, int currentTime, Int64 examTakenId)
        {
            var reg = new UpadateAnswersT(subjectid, examTakenId, answers, currentTime);
            var response = reg.update();
            return Json(response);
        }
        [HttpPost]
        public JsonResult updateAnswersTrialWaec(Int64 subjectid, string answers, int currentTime, Int64 examTakenId)
        {
            var reg = new UpadateAnswersTWaec(subjectid, examTakenId, answers, currentTime);
            var response = reg.update();
            return Json(response);
        }
        [HttpPost]
        public JsonResult newQuestionList(int examId, int exammode, string subjectids)
        {
            var resp = new GenerateQuestions(examId,exammode,subjectids,(long)Session["userid"]);
            var response = resp.saveExamInstanceAndGeneratequestionUTME();
            return Json(response);
        }



        [HttpPost]
        public JsonResult newTopicQuestionList( string topicid)
        {
            var userId = Convert.ToInt64(Session["userid"].ToString());
            var resp = new GenerateQuestionsByTopic( topicid, userId);

            //var resp = new GenerateQuestionsByTopic(subjectID,examId, topicid, (long)Session["userid"]);
            var response = resp.saveExamInstanceAndGeneratequestionbyTopic();
            return Json(response);
        }




        [HttpPost]
        public JsonResult newQuestionListWaec(int examId, int exammode, string subjectids)
        {
            var resp = new GenerateQuestionsWaec(examId, exammode, subjectids, (long)Session["userid"]);
            var response = resp.saveExamInstanceAndGeneratequestionWAEC();
            return Json(response);
        }

        [HttpPost]
        public JsonResult newQuestionListTrial(int examId, string subjectids)
        {
            var resp = new GenerateQuestionsTrial(examId, subjectids);
            var response = resp.saveExamInstanceAndGeneratequestionUTME();
            return Json(response);
        }
        [HttpPost]
        public JsonResult newQuestionListTrialWaec(int examId, string subjectids)
        {
            var resp = new GenerateQuestionsTrialWaec(examId, subjectids);
            var response = resp.saveExamInstanceAndGeneratequestionWAEC();
            return Json(response);
        }
        [HttpPost]
        public JsonResult submitExamTopic(Int64 ExamTakenID)
        {
            var reg = new SubmitExamByTopic(ExamTakenID);
            var response = reg.SaveExamAndCalcResultTOPICS();
            return Json(response);
        }


        [HttpPost]
        public JsonResult submitExam(Int64 ExamTakenID)
        {
            var reg = new SubmitExam(ExamTakenID);
            var response = reg.SaveExamAndCalcResultUTME();
            return Json(response);
        }


        [HttpPost]
        public JsonResult submitExamTrial(Int64 ExamTakenID)
        {
            var reg = new SubmitExamTrial(ExamTakenID);
            var response = reg.SaveExamAndCalcResultUTME();
            return Json(response);
        }

        [HttpPost]
        public JsonResult submitExamWaec(Int64 ExamTakenID)
        {
            var reg = new SubmitExamWaec(ExamTakenID);
            var response = reg.SaveExamAndCalcResultWAEC();
            return Json(response);
        }
        [HttpPost]
        public JsonResult submitExamTrialWaec(Int64 ExamTakenID)
        {
            var reg = new SubmitExamTrialWaec(ExamTakenID);
            var response = reg.SaveExamAndCalcResultWAEC();
            return Json(response);
        }


        public ActionResult UploadFile()
        {
            try
            {
                HttpPostedFileBase hpf = HttpContext.Request.Files["file"] as HttpPostedFileBase;
                string tag = HttpContext.Request.Params["tags"];// The same param name that you put in extrahtml if you have some.
                string category = HttpContext.Request.Params["category"];

                DirectoryInfo di = Directory.CreateDirectory(Server.MapPath("~/QImages/"));// If you don't have the folder yet, you need to create.
                string sentFileName = Path.GetFileName(hpf.FileName); //it can be just a file name or a user local path! it depends on the used browser. So we need to ensure that this var will contain just the file name.
                using (var db = new EXAMSPURDBEntities())
                {
                    var examid = Convert.ToInt64(Session["examid"]);
                    var subjectid = Convert.ToInt64(Session["subjectid"]);
                    var model = db.C_tblQuestion.Where(o => o.SubjectID == subjectid).ToList();
                    var lastQuestionNumber = model.Max(o => o.QuestionNumber);

                    string savedFileName = Path.Combine(di.FullName, subjectid.ToString() +"/"+ (lastQuestionNumber+1).ToString()+".png");

                    if (System.IO.File.Exists(savedFileName)) System.IO.File.Delete(savedFileName);
                    hpf.SaveAs(savedFileName);


                    var msg = new { msg = "File Uploaded", filename = hpf.FileName, url = savedFileName, fileNameNew = (lastQuestionNumber + 1).ToString() };
                    return Json(msg);
                }
            }
            catch (Exception e)
            {
                //If you want this working with a custom error you need to change in file jquery.uploadfile.js, the name of 
                //variable customErrorKeyStr in line 85, from jquery-upload-file-error to jquery_upload_file_error 
                var msg = new { jquery_upload_file_error = e.Message };
                return Json(msg);
            }
        }
        [Route("{url}")]
        public ActionResult DownloadFile(string url)
        {
            return File(url, "images/*");
        }

        [HttpPost]
        public ActionResult DeleteFile(string url)
        {
            try
            {
                System.IO.File.Delete(url);
                var msg = new { msg = "File Deleted!" };
                return Json(msg);
            }
            catch (Exception e)
            {
                //If you want this working with a custom error you need to change the name of 
                //variable customErrorKeyStr in line 85, from jquery-upload-file-error to jquery_upload_file_error 
                var msg = new { jquery_upload_file_error = e.Message };
                return Json(msg);
            }
        }
    }

}



