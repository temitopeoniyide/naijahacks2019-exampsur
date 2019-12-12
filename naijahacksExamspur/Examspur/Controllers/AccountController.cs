using ExamSpur.Models;
using ExamSpur.Views.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExamSpur.Controllers
{
    public class AccountController : Controller
    {
        // GET: Login
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult PartnersLogin()
        {
            return View();
        }


        public ActionResult ChangePasswordAssociation()
        {
            return View();
        }
        // GET: SignUp
        public ActionResult SignUp()
        {
            return View();
        }
        // GET: AdminManagement
        public ActionResult AdminManagement()
        {
            return View();
        }

        public ActionResult AdminLog()
        {
            return View();
        }

        public ActionResult SubscribeByPin()
        {
            return View();
        }

        public ActionResult ChangePassword()
        {
            return View();
        }
        public ActionResult ChangePasswordM()
        {
            return View();
        }
        public ActionResult ChangePasswordPartner()
        {
            return View();
        }
        // GET: Subscription
        public ActionResult Subscription()
        {
            return View();
        }

        public ActionResult LoginQuestion()
        {
            return View();
        }
        public ActionResult MerchantAccount()
        {
            return View();
        }
        public ActionResult StudentAccountTest()
        {
            return View();
        }
        public ActionResult PartnerAccount()
        {
            return View();
        }
        public ActionResult StudentAccount(string id)
           
        {
            if (id != null)
            {
                var email = Encryptor.base64Decode(HttpUtility.UrlDecode(id)).Split('~')[0];
                using (var db = new EXAMSPURDBEntities())
                {
                    var user = db.C_tblStudentReg.Where(o => o.Email == email).FirstOrDefault();
                    if (user != null)
                    {
                       Session["firstname"] = user.FirstName;
                       Session["lastname"] = user.LastName;
                       Session["email"] = user.Email;
                       Session["phone"] = user.Phone;
                       Session["userid"] = user.StudentID;
                        var name = (user.MerchantID == null) ? "" : db.C_tblMerchant.Find(user.MerchantID).CenterName; Session["merchantid"] = user.MerchantID;
                       Session["totalexam"] = db.C_tblExamTaken.Where(O => O.StudentID == user.StudentID && O.Status == 2).Count();

                       Session["merchantName"] = name;
                       Session["unitAvailable"] = user.Unit;
                       Session["userType"] = 1;
                       Session["secretpin"] = RegisterModel.Base64Decode(user.secretPin);
                        Response.Redirect("/Account/Subscription");
                    }

                }
            }
            return View();
        }
        public ActionResult MerchantSubscription()
        {
            var model = GetMerchantSub.GetSub();
            return View(model);
        }
        // GET: Merchant
        public ActionResult Merchant()
        {
            return View();
        }


        [HttpPost]
        public JsonResult Question(string Question, string OptionA, string OptionB, string OptionC, string OptionD, string OptionE, string answer, string solution,int frameWork,int imageno,int newImage)
        {
            var reg = new AddQuestion(Question, OptionA, OptionB, OptionC, OptionD, OptionE, answer, solution, frameWork,imageno, newImage);
            var response = reg.saveNewQuestion();
            return Json(response);
        }

        [HttpPost]
        public JsonResult loadQuestion()
        {
            var resp = LoadThisQuestion.loadQuest();
            return Json(resp);
        }

        [HttpPost]
        public JsonResult LoginAtso(string username, string password)
        {
            var response =new LoginTutorial(username, password).Login();
            return Json(response);
        }


        [HttpPost]
        public JsonResult Register(string firstName, string lastName, string email, string phone, string password, string code) {
            var reg = new RegisterModel(firstName, lastName, email, phone, password, "1", code);
            var response = reg.saveNewStudent();
            return Json(response);
        }
        [HttpPost]
        public JsonResult RegisterMarketer(string firstName, string lastName, string email, string phone, string password, Int16 partnerType,string center, string apass)
        {
            var reg = new RegisterMarketerModel(firstName, lastName, email, phone, password, partnerType, center,apass);
            var response = reg.saveNewMarketer();
            return Json(response);
        }
        [HttpPost]
        public JsonResult AddMerchantUser(string firstName, string lastName, string email, string phone,int role)
        {
            var reg = new AddMerchantUser(firstName, lastName, email, phone, role);
            var response = reg.addUser();
            return Json(response);
        }
        [HttpPost]
        public JsonResult RegisterMerchant(string firstName, string lastName, string email, String institution, String regcode, string phone, string password)
        {
            var reg = new RegisterMerchantModel(firstName, lastName, email, phone, institution, regcode, password);
            var response = reg.saveNewMerchant();
            return Json(response);
        }

        [HttpPost]
        public JsonResult AttachStudent(string email,  string pin,int classid)
        {
            var reg = new AttachStudent(email, pin,classid);
            var response = reg.saveNewStudent();
            return Json(response);
        }
        [HttpPost]
        public JsonResult RegisterStudentM(string firstName, string lastName, string classId, string phone)
        {
            var reg = new RegisterModel(firstName, lastName, classId, phone, "2");
            var response = reg.saveNewStudent();
            return Json(response);
        }
        [HttpPost]
        public JsonResult Login(string email, string password)
        {
            var reg = new LoginModel(email, password);
            var response = reg.Login();
            return Json(response);
        }
        [HttpPost]
        public JsonResult UpdateProfile(string name, string email,string phone, string city, string logo, string address,int state )
        {
            var reg = new UpdateProfileMerchant(address,state,city,name,logo,email,phone);
            var response = reg.update();
            return Json(response);
        }
        [HttpPost]
        public JsonResult getState()
        {
            var states = new getState();
            var response = states.loadState();
            return Json(response);
        }
        [HttpPost]
        public JsonResult getProfile()
        {
            var profile = new getProfile();
            var response = profile.loadProfile();
            return Json(response);
        }
        [HttpPost]
        public JsonResult LoginMerchant(string email, string password)
        {
            var reg = new LoginModel(email, password);
            var response = reg.LoginMerchant();
            return Json(response);
        }
        [HttpPost]
        public JsonResult LoginMarketer(string email, string password)
        {
            var reg = new LoginModel(email, password);
            var response = reg.LoginMarketer();
            return Json(response);
        }
        [HttpPost]
        public JsonResult ForgotPassword(string email)
        {
            var reg = new ForgotPasswordViewModel(email);
            var response = reg.ResetPasword();
            return Json(response);
        }
        [HttpPost]
        public JsonResult ForgotPasswordMerchant(string email)
        {
            var reg = new ForgotPasswordViewModel(email);
            var response = reg.ResetPaswordMerchant();
            return Json(response);
        }
        [HttpPost]
        public JsonResult ForgotPasswordMarketer(string email)
        {
            var reg = new ForgotPasswordViewModel(email);
            var response = reg.ResetPaswordMarketer();
            return Json(response);
        }

        [HttpPost]
        public JsonResult LoginQuestion(string firstName,int examId)
        {
            var response=0;
            if (firstName== "Archibong" || firstName == "Archibong"|| firstName == "Archibong"|| firstName == "Archibong"|| firstName == "Archibong"|| firstName == "Archibong"|| firstName == "Archibong"|| firstName == "Archibong"|| firstName == "Archibong")
            {
                Session["username"] = firstName;
                Session["examid"] = examId;

                response = 1;


            }
            return Json(response);


        }

        [HttpPost]
        public JsonResult subscribeByPin( string PinNumber)
        {
            var reg = new SubscribeByPinNumber(PinNumber, (long)Session["userid"]);
           
            var response = reg.CheckPin();
            return Json(response);


        }


        [HttpPost]
        public JsonResult ChangePasswordAsso(string oldpass, string newpass)
        {
            var reg = new ChangePasswordModel2(oldpass, newpass);
            //object response = null;
            var response = reg.ChangePasswordAsso();
            return Json(response);


        }



        [HttpPost]
        public JsonResult ChangePassword(string email, string oldpass, string newpass)
        {
            var reg = new ChangePasswordModel(email, oldpass, newpass);
            object response = null;
            if (Session["userType"].ToString() == "1") response = reg.ChangePassword();
            if (Session["userType"].ToString() == "2") response = reg.ChangePasswordMerchant();
            if (Session["userType"].ToString() == "3") response = reg.ChangePasswordMarketer();
            return Json(response);


        }



        [HttpPost]
        public JsonResult ChangePasswordM(string email, string oldpass, string newpass)
        {
            var reg = new ChangePasswordModel(email, oldpass, newpass);
            object response = null;
            if (Session["userType"].ToString() == "1") response = reg.ChangePassword();
            if (Session["userType"].ToString() == "2") response = reg.ChangePasswordMerchant();
            if (Session["userType"].ToString() == "3") response = reg.ChangePasswordMarketer();
            return Json(response);


        }
        [HttpPost]
        public JsonResult logoutcheck() {
            var a = new checklogout();
            return Json(a.checkBeforeLogout());
        }
        [HttpPost]
        public JsonResult logout()
        {
            Session.Abandon();
            return Json(new { value = 1 });
        }
        [HttpPost]
        public JsonResult Subscriptions(int usertype)
        {
            var sub = new SubscriptionViewModel(usertype);
            var response = sub.getSubscriptions();
            return Json(response);
        }
        [HttpPost]
        public JsonResult NewPayment(int planid)
        {
            if (Session["userType"].ToString() == "1")
            {
                var sub = new NewPayment(planid);
                var response = sub.saveNewPayment();
                return Json(response);
            }
            else {
                var sub = new NewPaymentMerchant(planid);
                var response = sub.saveNewPayment();
                return Json(response);
            }
        }
        [HttpPost]
        public JsonResult UpdatePayment(int subid,String paymentRef)
        {
            var sub = new UpdatePayment(subid,paymentRef);
            var response = sub.UpdatePay();
            return Json(response);
        }
        [HttpPost]
        public JsonResult UpdatePaymentM(int subid, String paymentRef, int unit)
        {
            var sub = new UpdatePaymentM(subid, paymentRef,unit);
            var response = sub.UpdatePay();
            return Json(response);
        }
        [HttpPost]
        public JsonResult Getprice(int id)
        {  
            var response = GetPrice.GetPiceByID(id);
            return Json(response);
        }
    }
}