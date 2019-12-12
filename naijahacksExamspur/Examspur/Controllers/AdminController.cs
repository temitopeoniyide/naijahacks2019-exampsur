using ExamSpur.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExamSpur.Controllers
{
    public class AdminController : Controller
    {
        // GET: Customer
        public ActionResult Customer()
        {
            return View();
        }


       

        public ActionResult PrintPin(int id)
        { 
           
            return View(GeneratePin.loadPinByBatch(id));
        }
        public ActionResult PinDashboardAtso(string id)
        {
            try
            {
                var username = (string)Session["email"];
                var resp = (username == null) ? null : GeneratePin.loadPinBatchByUsernameAdmin(username);
                return View(resp);
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        public ActionResult PinDashboardPostpaid(string id)
        {
            try
            {
                var username = (string)Session["email"];
                var resp = (username == null) ? null : GeneratePin.loadPinBatchByUsername(username);
                return View(resp);
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        public ActionResult PinPrepaid()
        {
            var username = (string)Session["email"];
            var resp = (username == null) ? null : GeneratePin.loadPinBatchByUsernameAdmin(username);
            return View(resp);
        }

        // GET: Customer
        public ActionResult TopicalPerformance()
        {
            return View();
        }
        public ActionResult ResultSummaryTopic()
        {
            var model = ResultViewModel.resultByTopic();
            return View(model);
        }
        public ActionResult ResultDetailsTopic(String examId)
        {
           
                var a = new resultDetailTopic(Convert.ToInt64(examId));
                var model = a.result();
                return PartialView("ResultDetailsTopic", model);
            
            
        }
        
             [HttpPost]
        public JsonResult loadResultByTopic(int classId,int topicId)
        {
         
            var response = StudentResultSummary.ClassResultSummaryByTopic(classId,topicId);
            return Json(response);
        }
        [HttpPost]
        public JsonResult generatePinPostPaid()
        {

            var response = GeneratePin.generatePinPostPaid();
            return Json(response);
        }
        [HttpPost]

        public JsonResult RemitPayPostPaid(int batchid, int  unit,decimal amount , string paymentRef)
        {
            var response = GeneratePin.RemitPay(batchid,amount,unit,paymentRef);
            return Json(response);

        }
        [HttpPost]

        public JsonResult PayAndGenerate(int unit, decimal amount, string paymentRef)
        {
            var response = GeneratePin.PayAtso(amount, unit, paymentRef);
            return Json(response);

        }
        [HttpPost]

        public JsonResult PayAndGenerateCenter(int unit, decimal amount, string paymentRef)
        {
            var response = GeneratePin.PayCenter(amount, unit, paymentRef);
            return Json(response);

        }
        [HttpPost]
        public JsonResult generatePinAtso(int qty)
        {

            var response = GeneratePin.generatePinAtso(qty);
            return Json(response);
        }

        [HttpPost]
        public JsonResult loadTopics2(int subjectId)
        {
            var reg = new TopicViewModel2(subjectId);
            var response = reg.loadTopicsModel2();
            return Json(response);
        }

        [HttpPost]
        public ActionResult ResultSummaryTopicForMerchant(Int64 userid)
        {
            var model = ResultViewModel.resultByTopicForMerchant(userid);
            return Json(model);
        }
        [HttpPost]
        public ActionResult ResultSummaryAjax()
        {
            var model = ResultViewModel.resultByTopic();
            return Json(model);
        }
        [HttpPost]
        public JsonResult correctionTopic(int topicid, Int64 examtakenid)
        {
            var correction = new GetCorrection(topicid, examtakenid);
            var response = correction.getListOfCorrectionByTopic();
            return Json(response);
        }
    }
}