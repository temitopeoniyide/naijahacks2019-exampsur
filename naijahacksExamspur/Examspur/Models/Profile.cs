using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace ExamSpur.Views.Models
{
    public class Profile
    {

        public  object loadstateandbank()
        {
            try
            {
                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var state = db.C_tblstate.ToList();
                    var bank = db.C_tblbank.ToList();
                    return new { value = 1, state, bank };
                }
            }
            catch (Exception Ex) { return new { value = 0 }; }
        }


        public  object loadProfileMarketer()
        {
            var a = (Guid)HttpContext.Current.Session["userid"];
            try
            {
                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var profile = db.C_tblMarketer.Find(a);

                    return new { value = 1, firstname = profile.Firstname, lastname = profile.Lastname, email = profile.email, phone = profile.phoneno, address = profile.address, stateid = profile.stateId, acct = profile.accountno, bankcode = profile.bankcode, acctname = profile.BankAccountName,code=profile.referralcode};
                }
            }
            catch (Exception Ex) { return new { value = 0 }; }
        }
    }
    public class UpdateProfileMarketer {
        public string Address { get; set; }
        public int Stateid { get; set; }
        public string Bankcode { get; set; }
        public string Accountnumber { get; set; }
        public string Bankaccountname { get; set; }
        public UpdateProfileMarketer(string address,int stateid,string accountnumber, string bankcode,string bankaccountname) {
            Address = address;
            Stateid = stateid;
            Bankaccountname = bankaccountname;
            Accountnumber = accountnumber;
            Bankcode = bankcode;
        }
        public object update() {
            try
            {
                var a = (Guid)HttpContext.Current.Session["userid"];
                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var profile = db.C_tblMarketer.Find(a);
                    profile.address = Address;
                    profile.accountno = Accountnumber;
                    profile.BankAccountName = Bankaccountname;
                    profile.bankcode = Bankcode;
                    profile.stateId = Stateid;
                    db.C_tblMarketer.Attach(profile);
                    db.Entry(profile).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    HttpContext.Current.Session["bank"] = Bankcode;
                    return new { value = 1 };
                }
            }
            catch (Exception Ex) { return new { value = 0 }; }
        }

    }
    public class AssignUnitInd
    {
        public int StudentID { get; set; }
        public int Unit { get; set; }
        public AssignUnitInd(int studentid, int unit)
        {
            this.StudentID = studentid;
            this.Unit = unit;

        }
        public object Assign() {
            try
            {
                 var merchantid = Convert.ToInt64(HttpContext.Current.Session["merchantid"]);
                var userid = Convert.ToInt64(HttpContext.Current.Session["userid"]);
                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    var student = db.C_tblStudentReg.Find(StudentID);
                    var checksub = db.C_tblMerchant.Find(merchantid);
                    if (checksub.UnitAvailable == 99999) return new { value = 0, msg = "Your students are already on unlimited" };
                    if (checksub.UnitAvailable < Unit) return new { value = 0, msg = "Insufficient Unit" };
                    if (checksub.UnitAvailable == 99999 && checksub.Expdate.Value.Date < DateTime.Now.Date)
                    {
                        student.Unit = 99999;
                        db.C_tblStudentReg.Attach(student);
                        db.Entry(student).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        return new { value = 1, msg = "Unlimited Access has been granted to this student" };
                    }
                    else if (checksub.UnitAvailable >= 1)
                    {
                        student.Unit += Unit*5;
                        db.C_tblStudentReg.Attach(student);
                        checksub.UnitAvailable -= Unit;
                        var usage = new C_tblMerchantUsage();
                        usage.MerchantID = merchantid;
                        usage.StudentID = student.StudentID;
                        usage.Unit = Unit;
                        usage.MerchantUserID = (int?)userid;
                        db.C_tblMerchantUsage.Add(usage);
                        db.C_tblMerchant.Attach(checksub);
                        db.Entry(checksub).State = System.Data.Entity.EntityState.Modified; db.SaveChanges();
                        db.Entry(student).State  = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        return new { value = 1, msg = Unit + " Unit(s) assigned Successfully"};
                    }
                    else {
                        return new { value = 0, msg = "Insufficient Unit Available" };


                    }
                }
            }
            catch (Exception ex) {
                return new { value = 0, msg = "Error occured, we are working to fix this ASAP. Try again later" };
            }

                    }
                }
        public class AssignUnit
    {
        public int ClassID { get; set; }
        public int Unit { get; set; }
        public AssignUnit(int classid , int unit)
        {
            this.ClassID = classid;
            this.Unit = unit;

        }
        public object assign() {
            try {
                var merchantid =Convert.ToInt64(HttpContext.Current.Session["merchantid"]);
                using (var db = new EXAMSPURDBEntities()) {
                    db.Configuration.ProxyCreationEnabled = false;

                    var checksub = db.C_tblMerchant.Find(merchantid);
                    if (checksub.UnitAvailable == 99999) return new { value = 0, msg = "Your students are already on unlimited access" };
                   
                    if (checksub.UnitAvailable == 99999 && checksub.Expdate.Value.Date < DateTime.Now.Date)
                    {
                        var students = db.C_tblStudentReg.Where(o => o.MerchantID == merchantid).ToList();
                        students.ForEach(o => o.Unit = 99999);
                        db.SaveChanges();
                        return new { value = 1, msg = "You are on unlimited access subscription. Unlimted access has been assigned to all your students." };
                    }
                    else if (checksub.UnitAvailable > 0 && checksub.UnitAvailable != 99999)
                    {
                        var students = db.C_tblStudentReg.Where(o => o.MerchantID == merchantid && o.ClassID == ClassID).ToList();
                        var total = students.Count();
                        var avilunit = checksub.UnitAvailable;
                        if (avilunit < Unit*total) return new { value = 0, msg = "Insufficient Unit to cater for all students in the class" };
                        if ((total * Unit) > avilunit)
                        {
                            students.Take(Convert.ToInt16(Math.Floor((decimal)(int)avilunit / Unit))).ToList().ForEach(o => o.Unit += (5 * Unit));
                            db.Entry(students).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            checksub.UnitAvailable -= Convert.ToInt16(Math.Floor((decimal)(int)avilunit / Unit));
                            db.Entry(checksub).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            foreach (var item in students)
                            {
                                var Usage = new C_tblMerchantUsage();
                                // Usage.AssignedDate = DateTime.Now.Date;
                                Usage.MerchantID = merchantid;
                                Usage.StudentID = item.StudentID;
                                Usage.MerchantUserID = (int?)HttpContext.Current.Session["userid"];
                                Usage.Unit = Unit;
                              
                                db.C_tblMerchantUsage.Add(Usage);
                                db.SaveChanges();

                            }
                            return new { value = 1, msg = Convert.ToInt16(Math.Floor((decimal)(int)avilunit / Unit)) + " students has been assigned" + Unit + " unit(s) each out of" + total + " students" };

                        }
                        else
                        {
                            students.ForEach(o => o.Unit += (Unit * 5));
                           
                            db.SaveChanges();
                            checksub.UnitAvailable -= total * Unit;
                            db.Entry(checksub).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            foreach (var item in students)
                            {
                                var Usage = new C_tblMerchantUsage();
                                // Usage.AssignedDate = DateTime.Now.Date;
                                Usage.MerchantID = merchantid;
                                Usage.StudentID = item.StudentID;
                                Usage.MerchantUserID = Convert.ToInt32(HttpContext.Current.Session["userid"]);
                                Usage.Unit = Unit;
                                db.C_tblMerchantUsage.Add(Usage);
                                db.SaveChanges();

                            }
                            return new { value = 1, msg = Unit + " unit(s) has be allocated to all students in this class" };

                        }
                    }
                    else {
                        return new { value = 0, msg ="Insufficient unit available" };

                    }

                }

            }


            catch (Exception ex) {
                return new { value = 0 };

            }
        }
    }
        public class ValidateAccount
    {
        public string BankCode { get; set; }
        public string AccountNo { get; set; }
        public ValidateAccount(string bankcode, string accountno)
        {
            this.BankCode = bankcode;
            this.AccountNo = accountno;

        }
        public object getaccountname()
        {
            try
            {
                using (var client = new WebClient())
                {
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    client.Headers.Add("Accept", "application/json");
                    client.Headers.Add("Content-Type", "application/json");
                    client.Headers.Add("Authorization", "Bearer sk_live_8948b90fbdf7bda8eda046584176e4b56211d3d6");
                    var resp = client.DownloadString("https://api.paystack.co/bank/resolve?account_number=" + AccountNo + "&bank_code=" + BankCode + "");
                    var newresp = JObject.Parse(resp.ToString());
                    return new { status = (bool)newresp["status"], name = (string)newresp["data"]["account_name"] };

                }

            }
            catch (Exception ex)
            {
                return new {status=false };
            }
        }
    }

    public class UpdateProfileMerchant {
        public string Address { get; set; }
        public int Stateid { get; set; }
        public string City { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public string Logo { get; set; }

        public UpdateProfileMerchant(string address, int stateid, string city, string name, string logo,string email,string phone)
        {
            Address = address;
            Stateid = stateid;
            City = city;
            Email = email;
            Phone = phone;
            Logo = logo;
            Name = name;
        }
        public object update()
        {
            try
            {
                var a = (long)HttpContext.Current.Session["merchantid"];
                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var profile = db.C_tblMerchant.Find(a);
                    profile.Address = Address;
                    profile.CenterName = Name;
                    profile.city = City;
                    profile.Email = Email;
                    profile.stateId = Stateid;
                    if (Logo != "")
                    {
                        profile.logo = Logo;
                    }
                    profile.Phone = Phone;
                    db.C_tblMerchant.Attach(profile);
                    db.Entry(profile).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return new { value = 1 };
                }
            }
            catch (Exception Ex) { return new { value = 0, msg="Error Occured, we are working to fix this ASAP. try again Later"}; }
        }

    }
}