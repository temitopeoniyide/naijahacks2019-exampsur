using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using EASendMail;

namespace ExamSpur.Models
{
    public class Email
    {
        public string Subject { get; set; }
        public string EmailAdd { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Message { get; set; }
        public Email(string subject ,string email, string name, string phone, string message) {
            this.Subject = subject;
            this.EmailAdd = email;
            this.Name = name;
            this.Phone = phone;
            this.Message = message;

        }
        public static bool SendHtmlFormattedEmail(string subject, string body1,string recipient)

        {
            bool flag = false;

            string sender = ConfigurationManager.AppSettings["UserName"].ToString();
            string senderpassword = ConfigurationManager.AppSettings["Password"].ToString();
            string receiver = recipient;
            //  WebConfigurationManager.AppSettings("ReceiverEmail").ToString()
            int mailportnumber = int.Parse(ConfigurationManager.AppSettings["Port"].ToString());
            string mailserver = ConfigurationManager.AppSettings["Host"].ToString();
            //   Dim mailreceivers As String()
            string deliverystatus = "0";

            System.Net.Mail.MailMessage mailmsg = new System.Net.Mail.MailMessage();

            System.Net.Mime.ContentType mimeType = new System.Net.Mime.ContentType("text/html");
            string body = System.Web.HttpUtility.HtmlDecode(body1);
            mailmsg.SubjectEncoding = Encoding.UTF8;
            mailmsg.From = new System.Net.Mail.MailAddress(sender, "Examspur", Encoding.UTF8);
            string[] receivers = receiver.Split(',');
            for (int i = 0; i <= receiver.Split(',').Length - 1; i++)
            {
                if (receivers[i].Contains("@"))
                {
                    mailmsg.To.Add(new System.Net.Mail.MailAddress(receivers[i]));
                }
            }
            
            //Try
           
            try
            {
                System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient(mailserver, mailportnumber);
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(sender, senderpassword);
                // client.DeliveryFormat = SmtpDeliveryFormat.SevenBit,
                client.DeliveryMethod = SmtpDeliveryMethod.Network;

                mailmsg.IsBodyHtml = true;
                ////false if the message body contains code
                mailmsg.Priority = System.Net.Mail.MailPriority.High;
                mailmsg.Subject = subject;
                mailmsg.Body = body;
                client.Send(mailmsg);

                mailmsg.DeliveryNotificationOptions = System.Net.Mail.DeliveryNotificationOptions.OnSuccess;
                //Dim i As Integer = mailmsg.DeliveryNotificationOptions
                mailmsg.Attachments.Clear();
                mailmsg.Dispose();
                flag = true;



            }
            catch (Exception ex)
            {
                // Throw New SmtpFailedRecipientException("The following problem occurred when attempting to " + "send your email: " + ex.Message)
                return false;



            }
            finally
            {
                mailmsg = null;
                //  Dim fil2 As New FileInfo(attachementfilepath)
                //   If (fil2.Exists) Then

                //  fil2.Delete()
                //  End If
            }

            return true;
           
        }

        
    }
    public class ExamSpurNotification
    {
        public string subject { get; set; }
        public string messageBody { get; set; }
        public string Header1 { get; set; }
        public string Header2 { get; set; }
        public string recepientEmail { get; set; }
     public    ExamSpurNotification(string subject, string messagebody, string recepient, string header1, string header2)
        {
            this.subject = subject;
            this.messageBody = messagebody;
            this.recepientEmail = recepient;
            this.Header1 = header1;
            this.Header2 = header2;
        }
        public object sendEmail()
        {
            string body = string.Empty;
            try
            {
                using (StreamReader reader = new StreamReader(System.Web.Hosting.HostingEnvironment.MapPath("~/welcome.html")))

                {

                    body = reader.ReadToEnd();
                    body = body.Replace("*header1*", Header1).Replace("*header2*", Header2).Replace("*messagebody*", messageBody).Replace("*messagedate*", DateTime.Now.Date.ToShortDateString());
                }



                var a = Email.SendHtmlFormattedEmail(subject, body, recepientEmail);
                return a;
            }
            catch(Exception ex)
            {

                return false;
            }
            }
    }
}