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


    public class sendmail
    {
        public static object Sendmail(string email, string body, string subject, string header1, string header2)
        {
            using (var db = new EXAMSPURDBEntities())
            {
                var mail = new tableMailAlert();
                mail.body = body;
                mail.status = 0;
                mail.receiverEmail = email;
                mail.subject = subject;
                mail.dropTimestamp = DateTime.Now;
                mail.header1 = header1;
                mail.header2 = header2;
                db.tableMailAlerts.Add(mail);
                var a =  db.SaveChanges();
                return a;
            }

        }
    }
    public class RegisterStudentViewModel {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string Sclass { get; set; }
        public string date { get; set; }
        public Int64 id { get; set; }
        public int unit { get; set; }
    }
    public static class getStudent {
        public static object getstudent()
        {
            var merchantid = Convert.ToInt64(HttpContext.Current.Session["merchantid"]);
            using (var db = new EXAMSPURDBEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var model = db.C_tblStudentReg.Where(a => a.MerchantID == merchantid);
                var student = new List<RegisterStudentViewModel>();
                foreach (var item in model) {
                    var assigned = db.C_tblMerchantUsage.Where(o => o.StudentID == item.StudentID && o.MerchantID == merchantid).ToList();
                    student.Add(new RegisterStudentViewModel { unit =(assigned.Count()==0)?0: (int)assigned.Sum(o=>o.Unit),
                        firstName = item.FirstName,
                        lastName = item.LastName,
                        email = item.Email, Sclass = db.C_tblclass.Find(item.ClassID).Class,
                        date = item.RegDate.ToString(), id = item.StudentID });
                }
                return student;
            }
        } 
    }


   
    public class RegisterModel
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string Password { get; set; }
        public string Type { get; set; }
        public string Code { get; set; }
        public RegisterModel(string firstname, string lastname, string email, string phone, string password, string type, string code)
        {
            this.firstName = firstname;
            this.lastName = lastname;
            this.email = email;
            this.phone = phone;
            this.Password = password;
            this.Type = type;
            this.Code = code;
        }
        public RegisterModel(string firstname, string lastname, string email, string phone, string type)
        {
            this.firstName = firstname;
            this.lastName = lastname;
            this.email = email;
            this.phone = phone;
            this.Type = type;
        }
        public object saveNewStudent() {
            try
            {
                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var data = (Type.ToString() == "1") ? db.C_tblStudentReg.Where(o => o.Email == email).FirstOrDefault() : db.C_tblStudentReg.Include("C_tblMerchant").Where(o => o.Email == email).FirstOrDefault();
                    if (data != null && Type == "1") return new { value = 0, msg = "Email Already Exist. Kindly provide another Email or proceed to login" };
                    else if (Type == "2")
                    {
                        if (data != null) return new { value = 2, msg = "Email Already Exist. Kindly provide another Email or proceed to login" };
                        //   if (data.C_tblMerchant.UnitAvailable <= 0) return new { value = 2, msg = "Insufficient unit balance" };
                    }
                    //CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
                    //TextInfo textInfo = cultureInfo.TextInfo;
                    int num = new Random().Next(1000, 9999);
                    var user = new C_tblStudentReg();
                    user.FirstName = firstName;
                    user.LastName = lastName;
                    user.Password = (Type == "1") ? Encryptor.EncodePassword(Password, ConfigurationManager.AppSettings["Salt"]) : Encryptor.EncodePassword(lastName.ToLower(), ConfigurationManager.AppSettings["Salt"]);
                    user.Phone = phone;
                    user.MerchantID = (Type == "1") ? (long?)null : Convert.ToInt64(HttpContext.Current.Session["merchantid"]);
                    var check = db.C_tblStudentReg.Where(o => o.Email == firstName + "-" + lastName).FirstOrDefault();
                    if (check == null) user.Email = (Type == "1") ? email : (firstName + "-" + lastName).ToLower();
                    else {
                        var rand = new Random();
                        var rn = rand.Next(10, 99);
                        user.Email = (Type == "1") ? email : (firstName + "-" + lastName + rn.ToString()).ToLower();

                    }
                    user.ClassID = (Type == "1") ? (int?)null : Convert.ToInt16(email);
                    user.RefCode = (Type == "1") ? Code : null;
                    user.Status = true;
                    user.secretPin = Base64Encode(num.ToString());
                    var count = db.C_tblStudentReg.Count();
                   var merchantid= Convert.ToInt64(HttpContext.Current.Session["merchantid"]);
                    var merchant = db.C_tblMerchant.Find(merchantid);
                    if (Type == "1")
                    {
                        user.Unit = 0;
                    } 
                    else
                    if (merchant.UnitAvailable == 99999) {
                        user.Unit = 99999;
                    }
                  
                    else user.Unit=0;
                    user.RegDate = DateTime.Now;

                    db.C_tblStudentReg.Add(user);
                    db.SaveChanges();
                    if (Type == "2")
                    {
                        //var merchantids = Convert.ToInt64(HttpContext.Current.Session["merchantid"]);
                        //var userid = Convert.ToInt32(HttpContext.Current.Session["userid"]);
                        //var merchants = db.C_tblMerchant.Find(merchantid);
                        //merchant.UnitAvailable -= 1;
                        //db.C_tblMerchant.Attach(merchant);
                        //db.Entry(data).State = System.Data.Entity.EntityState.Modified;
                        //db.SaveChanges();
                        //var c = new C_tblMerchantUsage();

                        //c.StudentID = user.StudentID;
                        //c.MerchantID = merchantid;
                        //c.MerchantUserID = userid;
                        //db.C_tblMerchantUsage.Add(c);
                        //db.SaveChanges();
                    }
                  var encemail= Encryptor.base64Encode(email+"~"+ConfigurationManager.AppSettings["Salt"].ToString());
                    if (DateTime.Now.Month==2 && DateTime.Now.Year == 2019)
                    {
                        var body = "<p style=\"text-align:justify\">Hello " + firstName + ", <br />Congratulations, your registration was successful. <br /><br>Please be informed that the Examspur Contest 2019 is coming up on the 2nd of march 2019. <br><h3>Examspur Contest 2019 Prizes</h3><br><ul class=\"list-unstyled text-left\"> <li>5  <strong> Exam Access before the contest</strong></li> <li><strong>ExamSpur Contest 2019 Qualification  </strong></li><li><strong> Winner 300k + 1 HP Laptop </strong></li> <li> <strong> 1st runnerup 200k + 1 Mini Laptop </strong></li> <li>  <strong> 2nd runnerup 100k + 1 Android Phone </strong></li>  </ul><br> To qualify to participate in the contest kindly subscribe for examspur practice on or before 28th February 2019. To subscribe click <a href=\""+ HttpContext.Current.Server.UrlEncode("https://www.examspur.com/account/studentaccount/"+ encemail+"")+"\">here</a>. <br> For more information on the content kindly visit <a href=\"https://www.examspur.com/contest/examspurcontest2019\">Examspur Contest 2019</a>. <br />Welcome onboard, we wish you success ahead. <br /> <br /> Regards, <br /> Admin ExamSpur. </p>";
                        var mailresp = sendmail.Sendmail(email, body, "Welcome to ExamSpur", "Registration", "Successful");

                    }
                    else
                    {
                        var body = "<p style=\"text-align:justify\">Hello " + firstName + ", <br />Congratulations, your registration was successful. <br /> <br />Welcome onboard, we wish you success ahead. <br /> <br /> Regards, <br /> Admin ExamSpur. </p>";
                        var mailresp = sendmail.Sendmail(email, body, "Welcome to ExamSpur", "Registration", "Successful");

                    }

                    HttpContext.Current.Session["firstname"] = user.FirstName;
                    HttpContext.Current.Session["lastname"] = user.LastName;
                    HttpContext.Current.Session["email"] = user.Email;
                    HttpContext.Current.Session["phone"] = user.Phone;
                    HttpContext.Current.Session["userid"] = user.StudentID;
                    var name = (user.MerchantID == null) ? "" : db.C_tblMerchant.Find(user.MerchantID).CenterName;
                    HttpContext.Current.Session["merchantid"] = user.MerchantID;
                    HttpContext.Current.Session["totalexam"] = db.C_tblExamTaken.Where(O => O.StudentID == user.StudentID && O.Status == 2).Count();

                    HttpContext.Current.Session["merchantName"] = name;
                    HttpContext.Current.Session["unitAvailable"] = user.Unit;
                    HttpContext.Current.Session["userType"] = 1;
                    HttpContext.Current.Session["secretpin"] = RegisterModel.Base64Decode(user.secretPin);
                    return new { value = 1, userID = user.StudentID, msg = "Registration Successful" };
                }
            }
            catch (Exception Ex) {
                return new { value = 0, msg = Ex.Message };
            }
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
    public class AttachStudent
    {
        public string email { get; set; }
        public string Pin { get; set; }
        public int ClassId { get; set; }
        public AttachStudent(string email, string pin,int classid)
        {

            this.email = email;
            this.Pin = pin;
            this.ClassId = classid;
        }

        public object saveNewStudent()
        {
            try
            {
                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var data = db.C_tblStudentReg.Where(o => o.Email == email).FirstOrDefault();
                    if (Base64Decode(data.secretPin) == Pin)
                    {

                        //CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
                        //TextInfo textInfo = cultureInfo.TextInfo;
                        int num = new Random().Next(1000, 9999);
                        data.secretPin = Base64Encode(num.ToString());
                        data.MerchantID = Convert.ToInt64(HttpContext.Current.Session["merchantid"]);
                        data.ClassID = ClassId;
                        var m = db.C_tblMerchant.Find(data.MerchantID);
                        if (m.UnitAvailable == 99999 && m.Expdate < DateTime.Now.Date) data.Unit = 99999;
                        //data.Unit += 5;
                        db.C_tblStudentReg.Attach(data);
                        db.Entry(data).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        var merchantid = Convert.ToInt64(HttpContext.Current.Session["merchantid"]);
                        var userid = Convert.ToInt32(HttpContext.Current.Session["userid"]);
                        //   var merchant = db.C_tblMerchant.Find(merchantid);
                        //  merchant.UnitAvailable -= 1;
                        // db.C_tblMerchant.Attach(merchant);

                        //    db.Entry(data).State = System.Data.Entity.EntityState.Modified;
                        //  db.SaveChanges();
                        // var c = new C_tblMerchantUsage();

                        // c.StudentID = data.StudentID;
                        // c.MerchantID = merchantid;
                        // c.MerchantUserID = userid;
                        // db.C_tblMerchantUsage.Add(c);
                        // db.SaveChanges();
                        if (!data.Email.ToLower().Contains("examspur"))
                        {
                            var body = "<p>Hello " + data.FirstName + ", <br /> <br /> <br /> <p>You have been attached to </b>" + data.C_tblMerchant.CenterName + "</b> students list. This means you now have 5 Exam trial access credited to your account.</p><p style=\"color:red\">Kindly note that you can detach your account from this institution at any time using the detach link on your dashboard menu.</p> <br /> <br /> Regards, <br /> Admin ExamSpur. </p>";
                           var a  =new ExamSpurNotification("Welcome to ExamSpur", body, email,   "Registration", "Successful!");
                            a.sendEmail();
                        }
                        HttpContext.Current.Session["firstname"] = data.FirstName;
                        HttpContext.Current.Session["lastname"] = data.LastName;
                        HttpContext.Current.Session["email"] = data.Email;
                        HttpContext.Current.Session["phone"] = data.Phone;
                        HttpContext.Current.Session["userid"] = data.StudentID;
                        var name = (data.MerchantID == null) ? "" : db.C_tblMerchant.Find(data.MerchantID).CenterName;
                        HttpContext.Current.Session["merchantid"] = data.MerchantID;
                        HttpContext.Current.Session["totalexam"] = db.C_tblExamTaken.Where(O => O.StudentID == data.StudentID && O.Status == 2).Count();

                        HttpContext.Current.Session["merchantName"] = name;
                        HttpContext.Current.Session["unitAvailable"] = data.Unit;
                        HttpContext.Current.Session["userType"] = 1;
                        HttpContext.Current.Session["secretpin"] = RegisterModel.Base64Decode(data.secretPin);
                        return new { value = 1, msg = "Registration Successful" };
                    }
                    else {
                        return new { value = 0, msg = "Invalid Pin" };
                    }
                }
            }
            catch (Exception Ex)
            {
                return new { value = 0, msg = Ex.Message };
            }
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
    public class RegisterMerchantModel
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string Password { get; set; }
        public string institution { get; set; }
        public String regcode { get; set; }
        public RegisterMerchantModel(string firstname, string lastname, string email, string phone, String institution, String regcode, string password)
        {
            this.firstName = firstname;
            this.lastName = lastname;
            this.email = email;
            this.phone = phone;
            this.Password = password;
            this.institution = institution;
            this.regcode = regcode;

        }
        public object saveNewMerchant()
        {
            try
            {
                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var data = db.C_tblMerchant.Where(o => o.Email == email).FirstOrDefault();
                    if (data != null) return new { value = 0, msg = "Email Already Exist. Kindly provide another Email or proceed to login" };


                    //CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
                    //TextInfo textInfo = cultureInfo.TextInfo;

                    var center = new C_tblMerchant();
                    center.CenterName = institution;
                    center.UnitAvailable = 0;
                    center.Password = Encryptor.EncodePassword(Password, ConfigurationManager.AppSettings["Salt"]);
                    center.Phone = phone;
                    center.RegDate = DateTime.Now;
                    center.Email = email;
                    center.RefCode = regcode;

                    db.C_tblMerchant.Add(center);
                    db.SaveChanges();

                    var user = new C_tblmerchantUser();
                    user.firstname = firstName;
                    user.lastname = lastName;
                    user.phone = phone;
                    user.email = email;
                    user.password = Encryptor.EncodePassword(Password, ConfigurationManager.AppSettings["Salt"]);
                    user.merchantid = center.MerchantID;
                    user.role = 1;
                    user.status = true;
                    user.regdate = DateTime.Now;
                    db.C_tblmerchantUser.Add(user);
                    db.SaveChanges();


                    var body = "<br><p style=\"text-align:justify\">Hello " + firstName + ", <br />Congratulations, your institution <b>" + institution + "</b> has been registered successfully. You have been given the admin priviledge to subscribe and add students under your institution. <br /> <br />Please kindly update your profile once you are logged in as this will enable your institution to appear among the list of institutions on our website. <br /> <br /> Regards, <br /><b> Admin ExamSpur</b>. </p>";
                     var msg = new ExamSpurNotification("Registration Confirmation", body, email,"Registration", "Successful!");
                     msg.sendEmail();
                    HttpContext.Current.Session["firstname"] = user.firstname;
                    HttpContext.Current.Session["lastname"] = user.lastname;
                    HttpContext.Current.Session["email"] = user.email;
                    HttpContext.Current.Session["phone"] = user.phone;
                    HttpContext.Current.Session["userid"] = user.Userid;
                    HttpContext.Current.Session["merchantid"] = user.merchantid;
                    HttpContext.Current.Session["role"] = user.role;
                    HttpContext.Current.Session["unitAvailable"] = db.C_tblMerchant.Where(o => o.MerchantID == user.merchantid).FirstOrDefault().UnitAvailable;
                    HttpContext.Current.Session["OName"] = user.C_tblMerchant.CenterName;
                    HttpContext.Current.Session["userType"] = 2;

                    return new { value = 1, msg = "Registration Successful" };
                }
            }
            catch (Exception Ex)
            {
                return new { value = 0, msg ="Ooops!! Something went wrong. Refresh the page and try again" };
            }
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }

    public class RegisterMarketerModel
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string Password { get; set; }
        public string aPassword { get; set; }
        public string centername { get; set; }
        public Int16 PartType { get; set; }


        public RegisterMarketerModel(string firstname, string lastname, string email, string phone, string password, Int16 partnerType,string centername, string apassword)
        {
            this.firstName = firstname;
            this.lastName = lastname;
            this.email = email;
            this.phone = phone;
            this.Password = password;
            this.PartType = partnerType;
            this.centername = centername;
            this.aPassword = apassword;
        }
        public object saveNewMarketer()
        {
            try
            {
                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var data = db.C_tblMarketer.Where(o => o.email == email).FirstOrDefault();
                    if (data != null) return new { value = 0, msg = "Email Already Exist. Kindly provide another Email or proceed to login" };
                    if (PartType == 1)
                    {
                        var chk = db.C_tblMarketer.Where(o => o.referralcode.ToLower() == aPassword.ToLower()).FirstOrDefault();
                        if(chk==null) return new { value = 0, msg = "Invalid Agent password" };
                    }

                    //CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
                    //TextInfo textInfo = cultureInfo.TextInfo;
                    var gen = new Random();
                    Ln: var code = gen.Next(1000, 9999);
                    if (db.C_tblMarketer.Where(o => o.referralcode == "SPUR" + code.ToString()).FirstOrDefault() != null) goto Ln;
                    var marketer = new C_tblMarketer();

                    marketer.Firstname = firstName;
                    marketer.Lastname = lastName;
                    marketer.password = Encryptor.EncodePassword(Password, ConfigurationManager.AppSettings["Salt"]);
                    marketer.phoneno = phone;
                    marketer.regdate = DateTime.Now;
                    marketer.email = email;
                    marketer.referralcode =PartType==1?aPassword: "SPUR" + code.ToString();
                    marketer.MarketerID = Guid.NewGuid();
                    marketer.status = true;
                    marketer.partnerType = PartType;
                    marketer.CenterName = PartType == 1 ? centername : "";
                    db.C_tblMarketer.Add(marketer);

                    db.SaveChanges();

                    if (PartType == 2)
                    {
                        var body = "<br><p style=\"text-align:justify\">Congratulations <b>" + firstName + ",</b><br>Your registration was successful.<br>Your referral code is <b>" + "SPUR" + code.ToString() + ".</b> Please be sure to always issue your <b style=\"color:red\">code</b> to your clients <br /> <br />Please kindly update your profile once you are log in to complete your registration process on this website. <br /> <br /> Regards, <br /> Admin ExamSpur. </p>";
                        var msg = new ExamSpurNotification("Registration Confirmation", body, email, "Registration", "Successful!");
                        msg.sendEmail();
                        //Task<object> mailresp = sendmail.Sendmail(email, "Name: " + firstName + " " + lastName + "<br>email: " + email + " <br>phone:" + phone + "", "New Marketer Registration", "Action", "Required");
                      //  mailresp.Wait();
                        //msg.sendEmail();
                        HttpContext.Current.Session["firstname"] = marketer.Firstname;
                        HttpContext.Current.Session["lastname"] = marketer.Lastname;
                        HttpContext.Current.Session["email"] = marketer.email;
                        HttpContext.Current.Session["phone"] = marketer.phoneno;
                        HttpContext.Current.Session["userid"] = marketer.MarketerID;
                        HttpContext.Current.Session["userType"] = 3;
                        HttpContext.Current.Session["ptype"] = marketer.partnerType;

                        HttpContext.Current.Session["bank"] = (marketer.bankcode == null) ? "" : marketer.bankcode;

                    }
                    else
                    {
                        var body = "<br><p style=\"text-align:justify\">Congratulations <b>" + firstName + ",</b><br>Your registration was successful.<br> Please refer to your dashboard to generate and print your Voucher, <br /> Admin ExamSpur. </p>";
                        var msg = new ExamSpurNotification("Registration Confirmation", body, email, "Registration", "Successful!");
                        msg.sendEmail();
                        //Task<object> mailresp = sendmail.Sendmail(email, "Name: " + firstName + " " + lastName + "<br>email: " + email + " <br>phone:" + phone + "", "New Marketer Registration", "Action", "Required");
               //         mailresp.Wait();
                        //msg.sendEmail();
                        HttpContext.Current.Session["firstname"] = marketer.Firstname;
                        HttpContext.Current.Session["lastname"] = marketer.Lastname;
                        HttpContext.Current.Session["email"] = marketer.email;
                        HttpContext.Current.Session["phone"] = marketer.phoneno;
                        HttpContext.Current.Session["userid"] = marketer.MarketerID;
                        HttpContext.Current.Session["userType"] = 3;
                        HttpContext.Current.Session["ptype"] = marketer.partnerType;

                        HttpContext.Current.Session["bank"] = (marketer.bankcode == null) ? "" : marketer.bankcode;

                    }
                    return new { value = 1, msg = "Registration Successful" };

                }
            }
            catch (Exception Ex)
            {
                return new { value = 0, msg = Ex.Message };
            }
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
    public class AddMerchantUser
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public int Role { get; set; }

        public AddMerchantUser(string firstname, string lastname, string email, string phone, int role)
        {
            this.firstName = firstname;
            this.lastName = lastname;
            this.email = email;
            this.phone = phone;
            this.Role = role;
        }
        public object addUser()
        {
            try
            {
                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var data = db.C_tblmerchantUser.Where(o => o.email == email).FirstOrDefault();
                    if (data != null) return new { value = 0, msg = "User already Registered" };


                    //CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
                    //TextInfo textInfo = cultureInfo.TextInfo;
                    var merchantid = Convert.ToInt64(HttpContext.Current.Session["merchantid"]);
                    var user = new C_tblmerchantUser();

                    user.firstname = firstName;
                    user.lastname = lastName;
                    user.password = Encryptor.EncodePassword(lastName.ToLower(), ConfigurationManager.AppSettings["Salt"]);
                    user.phone = phone;
                    user.regdate = DateTime.Now;
                    user.email = email;
                    user.role = Role;
                    user.merchantid = merchantid;
                    user.status = true;
                    db.C_tblmerchantUser.Add(user);
                    db.SaveChanges();
                    var role = (Role == 1) ? "an<b> Admin</b>" : "a<b> Teacher</b>";
                    var body = "<br><p style=\"text-align:justify\">Hello <b>" + firstName + ",</b><br>You have been registered on ExamSpur by "+db.C_tblMerchant.Find(merchantid).CenterName+" as  " + role+ " <br>You now have access to view your stuent perfomance and their are of weakness on the platform.<br> Your login credentials are: <br>Username: <b style=\"color:#d42a2a\">" + email+"</b><br> Password: <b style=\"color:#d42a2a\">"+lastName.ToLower()+"</b> in lowercase <br /> <br />To login to your dashboard kindly click <a href=\"https://examspur.com/Account/MerchantAccount\">here</a><br /> <br /> Regards, <br /> Admin ExamSpur. </p>";
                    var msg = new ExamSpurNotification("Registration Confirmation", body, email, "Welcome", "Onboard!");
                    msg.sendEmail();
                    return new { value = 1, msg = "Registration Successful" };

                }
            }
            catch (Exception Ex)
            {
                return new { value = 0, msg = Ex.Message };
            }
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
    public class getState {

        public object loadState() {
            using (var db = new EXAMSPURDBEntities()) {
                db.Configuration.ProxyCreationEnabled = false;
                var model = db.C_tblstate.ToList();

                return model;
            }

        }
    }
    public class getProfile
    {

        public object loadProfile()
        {
            using (var db = new EXAMSPURDBEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;

                var mearchantId = Convert.ToInt64(HttpContext.Current.Session["merchantid"]);
                var model = db.C_tblMerchant.Find(mearchantId);

                return new { center = model.CenterName, email = model.Email, phone = model.Phone, address = model.Address, city = model.city, stateid = model.stateId, };
            }

        }
    }
    public static class loadClass{
     public static object loadclassByMerchantID()
        {
            try {
               var merchantId = Convert.ToInt64(HttpContext.Current.Session["merchantid"]);
                using (var db = new EXAMSPURDBEntities()) {
                    db.Configuration.ProxyCreationEnabled = false;

                    var allClass = db.C_tblclass.Where(o => o.merchantID == merchantId).ToList();
                    return allClass;
                }

            } catch(Exception ex)  {
                return null;
            }

        }
        public static object addClassByMerchantID(string Class)
        {
            try
            {
                var merchantId = Convert.ToInt64(HttpContext.Current.Session["merchantid"]);
                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    var aclass = new C_tblclass();
                    aclass.merchantID = merchantId;
                    aclass.Class = Class;
                    db.C_tblclass.Add(aclass);
                    db.SaveChanges();
                    return new {value=1 };
                }

            }
            catch (Exception ex)
            {
                return new { value=0 , msg="Something went wrong. Try again Later"};
            }

        }
        public static object renameClass(int ClassId, string newClassName)
        {
            try
            {
                var merchantId = Convert.ToInt64(HttpContext.Current.Session["merchantid"]);
                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    var Class = db.C_tblclass.Find(ClassId);
                    Class.Class = newClassName;
                    db.C_tblclass.Attach(Class);
                    db.Entry(Class).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return new { value = 1 };
                }

            }
            catch (Exception ex)
            {
                return new { value = 0, msg = "Something went wrong. Try again Later" };
            }

        }
        public static object deleteStudentByClass(int ClassId) {
            try 
            {
                var merchantId = Convert.ToInt64(HttpContext.Current.Session["merchantid"]);
                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    var student = db.C_tblStudentReg.Where(o=>o.ClassID==ClassId && o.MerchantID==merchantId).ToList();
                    if (student == null) return new { value = 0, msg = "mo student in this class" };
                    student.ForEach(o => o.MerchantID = null);
                    db.SaveChanges();
                    return new { value = 1 };
                }

            }
            catch (Exception ex)
            {
                return new { value = 0, msg = "Something went wrong. Try again Later" };
            }

        }
        public static object deleteStudentByID(int StudentID)
        {
            try
            {
                var merchantId = Convert.ToInt64(HttpContext.Current.Session["merchantid"]);
                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    var student = db.C_tblStudentReg.Where(o=>o.StudentID==StudentID && o.MerchantID==merchantId).FirstOrDefault();
                    student.MerchantID = null;
                    db.C_tblStudentReg.Attach(student);
                    db.Entry(student).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return new { value = 1 };
                }

            }
            catch (Exception ex)
            {
                return new { value = 0, msg = "Something went wrong. Try again Later" };
            }

        }
        public static object moveStudentByClass(int fromClassId , int toClassId)
        {
            try
            {
                var merchantId = Convert.ToInt64(HttpContext.Current.Session["merchantid"]);
                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    var Class = db.C_tblStudentReg.Where(o => o.ClassID == fromClassId && o.MerchantID == merchantId).ToList();
                    Class.ForEach(o => o.ClassID = toClassId);
                  
                    db.SaveChanges();
                    return new { value = 1 };
                }

            }
            catch (Exception ex)
            {
                return new { value = 0, msg = "Something went wrong. Try again Later" };
            }

        }
        public static object loadstudentCountByClass() {
            try
            {
                var merchantId = Convert.ToInt64(HttpContext.Current.Session["merchantid"]);
                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    var Class = (from a in db.C_tblStudentReg join b in db.C_tblclass on a.ClassID equals b.id where a.MerchantID==merchantId group a by new { b.id, b.Class } into student select new {
                        className = student.Key.Class,
                        noInClass = student.Count(),
                        classID=student.Key.id
                    }).ToList();
                    
                    return Class;
                }

            }
            catch (Exception ex)
            {
                return null;
            }

        }
    }

    public static class StudentResultSummary{
        public static object ClassResultSummaryByClass(int classID,int examid) {
            try
            {
                var merchantId = Convert.ToInt64(HttpContext.Current.Session["merchantid"]);
                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;


                    var summary = (from a in db.C_tblStudentReg join c in db.C_tblExamTaken on a.StudentID equals c.StudentID join b in db.C_tblScore on c.StudentID equals b.StudentID
                                   where a.MerchantID == merchantId && a.ClassID == classID && c.ExamID == examid orderby c.StartTimestamp descending group b by new { a.StudentID, a.FirstName, a.LastName, a.Email } into g select new {
                                       id = g.Key.StudentID,
                                       firstname = g.Key.FirstName,
                                       lastname = g.Key.LastName,
                                       email = g.Key.Email,
                                       Sclass = db.C_tblclass.Where(o => o.id == classID).FirstOrDefault().Class,
                                       Score = g.Where(o=>o.ExamID==examid).OrderByDescending(o => o.ExamCode) //Math.Round((decimal)((g.Take(5).Sum(o => o.Score) / g.Take(5).Count()) / 400) * 100,1).ToString()
                   }).ToList();
                    return summary.ToList();
                }
            }
            catch (Exception Ex) {
                return null;
            }

                }
        public static object ClassResultSummaryByClassAndSubject(int classID, int examid,int subjectid)
        {
            try
            {
                var merchantId = Convert.ToInt64(HttpContext.Current.Session["merchantid"]);
                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;


                    var summary = (from a in db.C_tblStudentReg
                                   join c in db.C_tblExamTaken on a.StudentID equals c.StudentID
                                   join b in db.C_tblQuestionList on c.ExamTakenID equals b.ExamTakenId where b.SubjectID == subjectid
                                   where a.MerchantID == merchantId && a.ClassID == classID && c.ExamID == examid
                                   orderby c.StartTimestamp descending
                                   group b by new { a.StudentID, a.FirstName, a.LastName, a.Email } into g
                                   select new
                                   {
                                       firstname = g.Key.FirstName,
                                       lastname = g.Key.LastName,
                                       email = g.Key.Email,
                                       Sclass = db.C_tblclass.Where(o => o.id == classID).FirstOrDefault().Class,
                                       score = g.OrderByDescending(o=> o.ExamTakenId)// (subjectid==1014) ? (((g.Sum(o=>o.NoCorrect) / 60) * 100) / g.Count()):((((g.Sum(o => o.NoCorrect)/40))*100) / g.Count())
                                   }).ToList();
                    return summary.ToList();
                }
            }
            catch (Exception Ex)
            {
                return null;
            }

        }
        public static object ClassResultSummaryByTopic(int classID, int topicId)
        {
            try
            {
                var merchantId = Convert.ToInt64(HttpContext.Current.Session["merchantid"]);
                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;


                    var summary = (from a in db.C_tblStudentReg
                                   join c in db.C_tblExamTaken on a.StudentID equals c.StudentID
                                   join b in db.C_tblQuestionListTopic on c.ExamTakenID equals b.ExamTakenId
                                   where b.TopicID == topicId
                                   where a.MerchantID == merchantId && a.ClassID == classID
                                   where c.Status == 2
                                   orderby c.StartTimestamp descending
                                   group b by new { a.StudentID, a.FirstName, a.LastName, a.Email } into g
                                   select new
                                   {
                                       firstname = g.Key.FirstName,
                                       lastname = g.Key.LastName,
                                       email = g.Key.Email,
                                       Sclass = db.C_tblclass.Where(o => o.id == classID).FirstOrDefault().Class,
                                       score = g.Average(o => o.NoCorrect)// (subjectid==1014) ? (((g.Sum(o=>o.NoCorrect) / 60) * 100) / g.Count()):((((g.Sum(o => o.NoCorrect)/40))*100) / g.Count())
                                   }).ToList();
                    return summary.ToList();
                }
            }
            catch (Exception Ex)
            {
                return null;
            }

        }
    }
    public static class loadMechantusers {
        public static object loadUsers()
        {
            using (var db = new EXAMSPURDBEntities()) {
                db.Configuration.ProxyCreationEnabled = false;
                var merchantid = Convert.ToInt64(HttpContext.Current.Session["merchantid"]);
                var users = db.C_tblmerchantUser.Where(o => o.merchantid == merchantid).ToList();
                return users;
            }


        }

        public static object deleteUser(int id) {
            try
            {
                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var merchantid = Convert.ToInt64(HttpContext.Current.Session["merchantid"]);
                    var user = db.C_tblmerchantUser.Find(id);
                    if (user != null)
                    {
                        db.C_tblmerchantUser.Remove(user);
                        db.SaveChanges();
                        return new { value = 1 };
                    }
                    else
                    {
                        return new { value = 0, msg = "User cannot be deleted at the moment. Try again later" };

                    }


                }
            }
            catch (Exception ex) {
                return new { value = 0, msg = "User cannot be deleted at the moment. Try again later" };

            }
        }
        public static object updateMerchantUserRole(int id,int roleid)
        {
            try
            {
                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var merchantid = Convert.ToInt64(HttpContext.Current.Session["merchantid"]);
                    var user = db.C_tblmerchantUser.Find(id);
                    if (user != null)
                    {
                        user.role = roleid;
                        db.C_tblmerchantUser.Attach(user);
                        db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        return new { value = 1 };
                    }
                    else
                    {
                        return new { value = 0, msg = "Cannot update user role at the moment. Try again later" };

                    }


                }
            }
            catch (Exception ex)
            {
                return new { value = 0, msg = "User cannot be deleted at the moment. Try again later" };

            }
        }
    }
}