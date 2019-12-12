using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using static ExamSpur.Models.Email;

namespace ExamSpur.Models
{

   


    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public LoginModel(string email, string password)
        {
            this.Email = email;
            this.Password = password;
        }
        public object Login()
        {
            try
            {
                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    db.Configuration.ProxyCreationEnabled = false;
                    var user = db.C_tblStudentReg.Where(o => o.Email == Email || o.Phone == Email).FirstOrDefault();
                    if (user != null)
                    {
                        if (user.Status == true)
                        {
                            if (user.Password == Encryptor.EncodePassword(Password, ConfigurationManager.AppSettings["Salt"]))

                            {

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
                                return (new { value = 1, userID = user.StudentID, msg = "Login Successful", unitAvailable = user.Unit });
                            }
                            else
                            {
                                return (new { value = 0, msg = "Password Incorrect!! Try Again" });
                            }
                        }
                        else
                        {
                            return (new { value = 0, msg = "Your account has been de-activated. Kindly Contact the administrator" });
                        }

                    }
                    else
                    {
                        return (new { value = 0, msg = "Email or phone does not exist" });
                    }
                }
            }
            catch (Exception Ex)
            {
                var a = Ex.Message;
                return (new { value = 0, msg = "Ooops!! Cannot Login at this time try again Later" });
            }

        }

        public object LoginMerchant()
        {
            try
            {
                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    db.Configuration.ProxyCreationEnabled = false;
                    var user = db.C_tblmerchantUser.Where(o => o.email == Email || o.phone == Email).FirstOrDefault();
                    if (user != null)
                    {
                        if (user.status == true)
                        {
                            if (user.password == Encryptor.EncodePassword(Password, ConfigurationManager.AppSettings["Salt"]))

                            {

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

                                return (new { value = 1, msg = "Login Successful" });
                            }
                            else
                            {
                                return (new { value = 0, msg = "Password Incorrect!! Try Again" });
                            }
                        }
                        else
                        {
                            return (new { value = 0, msg = "Your account has been de-activated. Kindly Contact the administrator" });
                        }

                    }
                    else
                    {
                        return (new { value = 0, msg = "Email or phone does not exist" });
                    }
                }
            }
            catch (Exception Ex)
            {
                var a = Ex.Message;
                return (new { value = 0, msg = "Ooops!! Cannot Login at this time try again Later" });
            }

        }
        public object LoginMarketer()
        {
            try
            {
                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    db.Configuration.ProxyCreationEnabled = false;
                    var user = db.C_tblMarketer.Where(o => o.email == Email || o.phoneno == Email).FirstOrDefault();
                    if (user != null)
                    {
                        if (user.status == true)
                        {
                            if (user.password == Encryptor.EncodePassword(Password, ConfigurationManager.AppSettings["Salt"]))

                            {

                                HttpContext.Current.Session["firstname"] = user.Firstname;
                                HttpContext.Current.Session["lastname"] = user.Lastname;
                                HttpContext.Current.Session["email"] = user.email;
                                HttpContext.Current.Session["phone"] = user.phoneno;
                                HttpContext.Current.Session["userid"] = user.MarketerID;
                                HttpContext.Current.Session["userType"] = 3;
                                HttpContext.Current.Session["ptype"] =user.partnerType;

                                HttpContext.Current.Session["bank"] = (user.bankcode == null) ? "" : user.bankcode;


                                return (new { value = 1, msg = "Login Successful",ptype=user.partnerType });
                            }
                            else
                            {
                                return (new { value = 0, msg = "Password Incorrect!! Try Again" });
                            }
                        }
                        else
                        {
                            return (new { value = 0, msg = "Your account has been de-activated. Kindly Contact the administrator" });
                        }

                    }
                    else
                    {
                        return (new { value = 0, msg = "Email or phone does not exist" });
                    }
                }
            }
            catch (Exception Ex)
            {
                var a = Ex.Message;
                return (new { value = 0, msg = "Ooops!! Cannot Login at this time try again Later" });
            }

        }
    }

    public class ForgotPasswordViewModel
    {

        public string Email { get; set; }
        public ForgotPasswordViewModel(string email)
        {
            this.Email = email;
        }
        public object ResetPasword()
        {
            using (var db = new EXAMSPURDBEntities())
            {
                try
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    db.Configuration.ProxyCreationEnabled = false;
                    var user = db.C_tblStudentReg.Where(o => o.Email == Email).FirstOrDefault();
                    if (user != null)
                    {
                        var newpass = Membership.GeneratePassword(8, 0);
                        user.Password = Encryptor.EncodePassword(newpass, ConfigurationManager.AppSettings["Salt"]);
                        db.C_tblStudentReg.Attach(user);
                        db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        var body = "<p style=\"text-align:justify\">Hello " + user.FirstName + ",<br /> Your password reset was successful.</p> <br />" +
                          "<p style=\"text-align:justify\">Your new password is <b>" + newpass + "</b>. <br /><br />Please remember to chance your password once you are logged in.<br />" +
                  "<br /><p style=\"text-align:justify\"> Regards,</p><p>Admin ExamSpur</p>";
                        var resp = new ExamSpurNotification("Password Reset Notification", body, Email, "Password Reset", "Successful!");

                        var a = resp.sendEmail();
                        return new { value = 1, msg = "Password Reset Successful. A new Password has been sent to your Email" };
                    }
                    else
                    {
                        return new
                        {
                            value = 0,
                            msg = "Email does not exist."
                        };
                    }
                }
                catch (Exception Ex)
                {
                    return new
                    {
                        value = 0,
                        msg = "Something Went Wrong. Kindly try again later"
                    };
                }
            }
        }
        public object ResetPaswordMerchant()
        {
            using (var db = new EXAMSPURDBEntities())
            {
                try
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    db.Configuration.ProxyCreationEnabled = false;
                    var user = db.C_tblmerchantUser.Where(o => o.email == Email || o.phone == Email).FirstOrDefault();
                    if (user != null)
                    {
                        var newpass = Membership.GeneratePassword(8, 0);
                        user.password = Encryptor.EncodePassword(newpass, ConfigurationManager.AppSettings["Salt"]);
                        db.C_tblmerchantUser.Attach(user);
                        db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        var body = "<p style=\"text-align:justify\">Hello " + user.firstname + ",<br /> Your password reset was successful.</p> <br />" +
                          "<p style=\"text-align:justify\">Your new password is <b>" + newpass + "</b>. <br /><br />Please remember to chance your password once you are logged in.<br />" +
                  "<br /><p style=\"text-align:justify\"> Regards,</p><p>Admin ExamSpur</p>";
                        var resp = new ExamSpurNotification("Password Reset Notification", body, Email, "Password Reset", "Successful!");

                        var a = resp.sendEmail();
                        return new { value = 1, msg = "Password Reset Successful. A new Password has been sent to your Email" };
                    }
                    else
                    {
                        return new
                        {
                            value = 0,
                            msg = "Email does not exist."
                        };
                    }
                }
                catch (Exception Ex)
                {
                    return new
                    {
                        value = 0,
                        msg = "Something Went Wrong. Kindly try again later"
                    };
                }
            }
        }
        public object ResetPaswordMarketer()
        {
            using (var db = new EXAMSPURDBEntities())
            {
                try
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    db.Configuration.ProxyCreationEnabled = false;
                    var user = db.C_tblMarketer.Where(o => o.email == Email || o.phoneno == Email).FirstOrDefault();
                    if (user != null)
                    {
                        var newpass = Membership.GeneratePassword(8, 0);
                        user.password = Encryptor.EncodePassword(newpass, ConfigurationManager.AppSettings["Salt"]);
                        db.C_tblMarketer.Attach(user);
                        db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        var body = "<p style=\"text-align:justify\">Hello " + user.Firstname + ",<br /> Your password reset was successful.</p> <br />" +
                          "<p style=\"text-align:justify\">Your new password is <b>" + newpass + "</b>. <br /><br />Please remember to chance your password once you are logged in.<br />" +
                  "<br /><p style=\"text-align:justify\"> Regards,</p><p>Admin ExamSpur</p>";
                        var resp = new ExamSpurNotification("Password Reset Notification", body, Email, "Password Reset", "Successful!");

                        var a = resp.sendEmail();
                        return new { value = 1, msg = "Password Reset Successful. A new Password has been sent to your Email" };
                    }
                    else
                    {
                        return new
                        {
                            value = 0,
                            msg = "Email does not exist."
                        };
                    }
                }
                catch (Exception Ex)
                {
                    return new
                    {
                        value = 0,
                        msg = "Something Went Wrong. Kindly try again later"
                    };
                }
            }
        }
    }
    public class ChangePasswordModel
    {
        public string Username { get; set; }
        public string oPassword { get; set; }
        public string nPassword { get; set; }
        public ChangePasswordModel(string username, string oldpassword, string newpassword)
        {
            this.Username = username;
            this.oPassword = oldpassword;
            this.nPassword = newpassword;
        }
        public object ChangePassword()
        {
            try
            {
                using (EXAMSPURDBEntities db = new EXAMSPURDBEntities())
                {

                    db.Configuration.LazyLoadingEnabled = false;
                    db.Configuration.ProxyCreationEnabled = false;

                    var user = db.C_tblStudentReg.Where(o => o.Email == Username).FirstOrDefault();
                    if (user != null)
                    {
                        if (user.Password == Encryptor.EncodePassword(oPassword, ConfigurationManager.AppSettings["Salt"]))
                        {
                            user.Password = Encryptor.EncodePassword(nPassword, ConfigurationManager.AppSettings["Salt"]);
                            db.C_tblStudentReg.Attach(user);
                            db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            return new { value = 1, msg = "Password changed Successfully" };
                        }
                        else
                        {
                            return new { value = 0, msg = "Old Password Incorrect. Try Again!!!" };
                        }
                    }
                    else
                    {
                        return new { value = 0, msg = "Invalid Username/Email" };
                    }
                }
            }
            catch (Exception ex)
            {
                return new { value = 0, msg = "Error Occured!!! Try again Later." };
            }
        }
        public object ChangePasswordMerchant()
        {
            try
            {
                using (EXAMSPURDBEntities db = new EXAMSPURDBEntities())
                {

                    db.Configuration.ProxyCreationEnabled = false;

                    var user = db.C_tblmerchantUser.Where(o => o.email == Username).FirstOrDefault();
                    if (user != null)
                    {
                        if (user.password == Encryptor.EncodePassword(oPassword, ConfigurationManager.AppSettings["Salt"]))
                        {
                            user.password = Encryptor.EncodePassword(nPassword, ConfigurationManager.AppSettings["Salt"]);
                            db.C_tblmerchantUser.Attach(user);
                            db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            return new { value = 1, msg = "Password changed Successfully" };
                        }
                        else
                        {
                            return new { value = 0, msg = "Old Password Incorrect. Try Again!!!" };
                        }
                    }
                    else
                    {
                        return new { value = 0, msg = "Invalid Username/Email" };
                    }
                }
            }
            catch (Exception ex)
            {
                return new { value = 0, msg = "Error Occured!!! Try again Later." };
            }
        }
        public object ChangePasswordMarketer()
        {
            try
            {
                using (EXAMSPURDBEntities db = new EXAMSPURDBEntities())
                {

                    db.Configuration.ProxyCreationEnabled = false;

                    var user = db.C_tblMarketer.Where(o => o.email == Username).FirstOrDefault();
                    if (user != null)
                    {
                        if (user.password == Encryptor.EncodePassword(oPassword, ConfigurationManager.AppSettings["Salt"]))
                        {
                            user.password = Encryptor.EncodePassword(nPassword, ConfigurationManager.AppSettings["Salt"]);
                            db.C_tblMarketer.Attach(user);
                            db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            return new { value = 1, msg = "Password changed Successfully" };
                        }
                        else
                        {
                            return new { value = 0, msg = "Old Password Incorrect. Try Again!!!" };
                        }
                    }
                    else
                    {
                        return new { value = 0, msg = "Invalid Username/Email" };
                    }
                }
            }
            catch (Exception ex)
            {
                return new { value = 0, msg = "Error Occured!!! Try again Later." };
            }
        }




    }



    public class ChangePasswordModel2
    {

        public string oPassword { get; set; }
        public string nPassword { get; set; }
        public ChangePasswordModel2(string oldpassword, string newpassword)
        {
   
            this.oPassword = oldpassword;
            this.nPassword = newpassword;
        }
        public object ChangePasswordAsso()
        {
            try
            {
                using (EXAMSPURDBEntities db = new EXAMSPURDBEntities())
                {

                    db.Configuration.LazyLoadingEnabled = false;
                    db.Configuration.ProxyCreationEnabled = false;
                    var Username = HttpContext.Current.Session["email"].ToString();
                    var user = db.C_tblLogin.Where(o => o.UserName == Username).FirstOrDefault();
                    if (user != null)
                    {
                        if (user.Password == oPassword)
                        {
                            user.Password = nPassword;
                            db.C_tblLogin.Attach(user);
                            db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            return new { value = 1, msg = "Password changed Successfully" };
                        }
                        else
                        {
                            return new { value = 0, msg = "Old Password Incorrect. Try Again!!!" };
                        }
                    }
                   else
                    {
                        return new { value = 0, msg = "Invalid Username/Email" };
                    }
                }
            }
            catch (Exception ex)
            {
                return new { value = 0, msg = "Error Occured!!! Try again Later." };
            }
        }



    }







    public class LoginTutorial
    {

        public string Password2 { get; set; }
        public string UserName2 { get; set; }



        public LoginTutorial(string username, string password)
        {

            this.UserName2 = username;
            this.Password2 = password;



        }
        public object Login()
        {

            try
            {

                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    db.Configuration.ProxyCreationEnabled = false;
                    var user = db.C_tblLogin.Where(o => o.UserName == UserName2 && o.Password == Password2).FirstOrDefault();

                    if (user == null) return new { value = 0, msg = "Invalid Username " };
                    else if (user.Password == Password2)
                    {
                        //var bytepassword = System.Text.Encoding.UTF8.GetBytes(Password2);
                        //var encpass = Encryptor.EncodePassword(Password2.ToLower(), ConfigurationManager.AppSettings["Salt"]);


                        //if (user.Password == encpass)
                        //{
                        HttpContext.Current.Session["email"] = UserName2;
                        HttpContext.Current.Session["firstname"] = "Admin";
                        return new { value = 1 };
                    }
                    else
                    {
                        return new { value = 0, msg = "Password incorrect" };
                    }


                }



            }
            catch (Exception Ex)
            {

                return new { value = 0, msg = "something went wrong try again later." };

            }




        }

    }
}
