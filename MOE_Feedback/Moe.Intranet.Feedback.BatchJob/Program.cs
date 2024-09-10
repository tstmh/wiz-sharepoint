using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Microsoft.SharePoint;
using System.Configuration;
using Microsoft.SharePoint.Utilities;
using System.IO;
using System.Net.Mail;
using System.Net;
using Microsoft.SharePoint.ApplicationPages.Calendar.Exchange;

namespace Moe.Intranet.eCards.BatchJob
{
    class Program
    {
        static void Main(string[] args)
        {
            SendECard();
        }

        public static void SendECard()
        {

            string filePath = System.IO.Path.GetFullPath(ConfigurationManager.AppSettings.Get("LogPath"));
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    string webURL = ConfigurationManager.AppSettings.Get("WebURLs");

                    using (StreamWriter writer = new StreamWriter(filePath, true))
                    {
                        writer.WriteLine("webURL :" + webURL + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                        writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                    }

                    using (SPSite _site = new SPSite(webURL))
                    {
                        using (StreamWriter writer = new StreamWriter(filePath, true))
                        {
                            writer.WriteLine("_site : _site object created " + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                            writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                        }

                        using (SPWeb _web = _site.OpenWeb())
                        {

                            using (StreamWriter writer = new StreamWriter(filePath, true))
                            {
                                writer.WriteLine("_web : _web open web " + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                                writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                            }

                        //EmaileCard(_web, "julius.velasco@wizvision.com", "sender", "subject", "feedback", "url", "slnp_spfarm@schools.local");
                        //    EmaileCard(_web, "julius.velasco@wizvision.com", "sender", "subject", "feedback", "url", "slnp_spfarm@schools.local");
                        //    EmaileCard(_web, "julius.velasco@wizvision.com", "sender", "subject", "feedback", "url", "slnp_spfarm@schools.local");
                        //    EmaileCard(_web, "julius.velasco@wizvision.com", "sender", "subject", "feedback", "url", "slnp_spfarm@schools.local");
                        //    EmaileCard(_web, "julius.velasco@wizvision.com", "sender", "subject", "feedback", "url", "slnp_spfarm@schools.local");
                        //    EmaileCard(_web, "julius.velasco@wizvision.com", "sender", "subject", "feedback", "url", "slnp_spfarm@schools.local");
                        //    EmaileCard(_web, "julius.velasco@wizvision.com", "sender", "subject", "feedback", "url", "slnp_spfarm@schools.local");
                        //    EmaileCard(_web, "julius.velasco@wizvision.com", "sender", "subject", "feedback", "url", "slnp_spfarm@schools.local");
                            EmaileCard(_web, "slnp_spfarm@schools.local", "sender", "subject", "feedback", "url", "slnp_spfarm@schools.local");
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                // string filePath = System.IO.Path.GetFullPath(@"C:\Error.txt");

                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine("Message :" + ex.Message + "<br/>" + Environment.NewLine + "StackTrace :" + ex.StackTrace +
                       "" + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                    writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                }
                //lblMsg.Text = ex.Message;
            }
        }

        private static void EmaileCard(SPWeb _web, string _to, string _sendername, string _subj, string _feedback, string _pageurl, string _emailaddress)
        {
            string filePath = System.IO.Path.GetFullPath(ConfigurationManager.AppSettings.Get("LogPath"));
            try
            {
                System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage()
                {
                    From = new MailAddress("noreply@moe.gov.sg"),
                    Subject = string.Concat("[MOE Intranet Feedback] ", _subj)
                };
                string str = "Dear Admin,<br/><br/>";
                str = string.Concat(new string[] { str, _sendername, " ", _emailaddress, " has sent feedback for your review<br/><br/>" });
                str = string.Concat(str, "Page URL : ", _pageurl, "<br/><br/>");
                str = string.Concat(str, "Your Feedback : ", _feedback, "<br/><br/>");
                str = string.Concat(str, "This is an automatically generated email. Please do not reply.");
                string[] strArrays = _to.Split(new char[] { ';' });
                for (int i = 0; i < (int)strArrays.Length; i++)
                {
                    string str1 = strArrays[i];
                    mailMessage.To.Add(str1.Trim());
                }
                mailMessage.Body = str;
                mailMessage.IsBodyHtml = true;
                string address = _web.Site.WebApplication.OutboundMailServiceInstance.Server.Address;
                SmtpClient smtpClient = new SmtpClient(address)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(_web.Site.WebApplication.OutboundMailUserName, _web.Site.WebApplication.OutboundMailPassword)
                };

                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                string eee = ex.Message;

                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine("Message :" + ex.Message + "<br/>" + Environment.NewLine + "StackTrace :" + ex.StackTrace +
                       "" + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                    writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="imgURL"></param>
        /// <returns></returns>
        private static string GeteCardHTML(string msg, ref string imgURL)
        {
            imgURL = imgURL.Substring(0, imgURL.LastIndexOf(","));
            imgURL = imgURL.Trim();
            //imgURL = imgURL.Replace("/_t", "");
            //string extn = "_" + imgURL.Substring(imgURL.LastIndexOf(".") + 1);
            //imgURL = imgURL.Replace(extn, "");

            StringBuilder sb = new StringBuilder();
            sb.Append("<html><head></head><body><table><tr><td>");
            sb.Append(msg + "</td></tr><tr><td>");
            sb.Append("</td></tr>");
            return sb.ToString();
        }
    }

    public static class LocalConstants
    {
        public const string eCards_SenderName = "Sender Name";
        public const string eCards_SenderEmail = "Sender Email";
        public const string eCards_RecipientName = "Recipient Name";
        public const string eCards_RecipientEmail = "Recipient Email";
        public const string eCards_GreetingTitle = "Greeting Title";
        public const string eCards_GreetingMessage = "Greeting Message";
        public const string eCards_Image = "Image";
        public const string eCards_SendOn = "Send On";
        public const string eCards_SendCardVia = "Send Card Via";
        public const string eCards_eCardDelivered = "eCardDelivered";
    }
}