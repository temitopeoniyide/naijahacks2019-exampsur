using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace ExamSpur.Views.Models
{
    public class LoadStudent
    {

        public object loadStudent()
        {
            try
            {
                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var student = db.C_tblStudentReg.ToList();
                  // student.FirstOrDefault().
                    return new { value = 1,student };
                }
            }
            catch (Exception Ex) { return new { value = 0 }; }
        }


        

    }





    public class LoadMerchant
    {

        public object loadMerchant()
        {
            try
            {
                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var student = db.C_tblMerchant.ToList();
                   // student.FirstOrDefault().
                    return new { value = 1, student };
                }
            }
            catch (Exception Ex) { return new { value = 0 }; }
        }




    }

    public class LoadMarketers
    {

        public object loadMarketer()
        {
            try
            {
                using (var db = new EXAMSPURDBEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var student = db.C_tblMarketer.ToList();
                     //student.FirstOrDefault().
                    return new { value = 1, student };
                }
            }
            catch (Exception Ex) { return new { value = 0 }; }
        }




    }

}