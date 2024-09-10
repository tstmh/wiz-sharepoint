using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Mobile.Controls;
using Microsoft.SharePoint.Utilities;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Linq;

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
                    string listName = ConfigurationManager.AppSettings.Get("ECardListName");

                    using (StreamWriter writer = new StreamWriter(filePath, true))
                    {
                        writer.WriteLine("webURL :" + webURL + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                        writer.WriteLine("listName :" + listName + Environment.NewLine + "Date :" + DateTime.Now.ToString());
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

                            SPList eCardList = _web.Lists.TryGetList(listName);

                            using (StreamWriter writer = new StreamWriter(filePath, true))
                            {
                                writer.WriteLine("eCardList : " + eCardList.ContentTypes.ToString() + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                                writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                            }

                            foreach (SPListItem eCardItem in eCardList.Items)
                            {
                                try
                                {
                                    DateTime eCardDT = Convert.ToDateTime(eCardItem[LocalConstants.eCards_SendOn]);
                                    if (eCardDT.Date.Equals(System.DateTime.Now.Date))
                                    {


                                        string isDelivered = Convert.ToString(eCardItem[LocalConstants.eCards_eCardDelivered]);
                                        if (isDelivered.Equals("No", StringComparison.CurrentCultureIgnoreCase))
                                        {
                                            using (StreamWriter writer = new StreamWriter(filePath, true))
                                            {
                                                writer.WriteLine("Date matched and not delivered started : Date :" + DateTime.Now.ToString());
                                                writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                                            }

                                            string from_name = Convert.ToString(eCardItem[LocalConstants.eCards_SenderName]);
                                            string from_email = Convert.ToString(eCardItem[LocalConstants.eCards_SenderEmail]);
                                            string to = Convert.ToString(eCardItem[LocalConstants.eCards_RecipientEmail]);
                                            string subj = Convert.ToString(eCardItem[LocalConstants.eCards_GreetingTitle]);
                                            string msg = Convert.ToString(eCardItem[LocalConstants.eCards_GreetingMessage]);
                                            string imgURL = Convert.ToString(eCardItem[LocalConstants.eCards_Image]);
                                            EmaileCard(_web, to, subj, msg, imgURL, from_name, from_email);

                                            using (StreamWriter writer = new StreamWriter(filePath, true))
                                            {
                                                writer.WriteLine("Message sent to: " + to + " sent:  Date :" + DateTime.Now.ToString());
                                                writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                                            }
                                            eCardItem[LocalConstants.eCards_eCardDelivered] = "Yes";
                                            eCardItem.Update();
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    using (StreamWriter writer = new StreamWriter(filePath, true))
                                    {
                                        writer.WriteLine("Message :" + ex.Message + "<br/>" + Environment.NewLine + "StackTrace :" + ex.StackTrace +
                                           "" + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                                        writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                                    }

                                }
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine("Message :" + ex.Message + "<br/>" + Environment.NewLine + "StackTrace :" + ex.StackTrace +
                       "" + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                    writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                }
            }
        }

        private static void EmaileCard(SPWeb _web, string _to, string _subj, string _msg, string imgURL, string FromName, string FromEmail)
        {
            string filePath = System.IO.Path.GetFullPath(ConfigurationManager.AppSettings.Get("LogPath"));
            try
            {
                SmtpClient client = new SmtpClient(_web.Site.WebApplication.OutboundMailServiceInstance.Server.Address);
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(FromEmail);
                mailMessage.Subject = string.Format("[You have received an e-card from {0}]", FromName);

                if (_to.Contains(";"))
                {
                    _to = _to.Replace(";", ",");
                }

                string _htmlBody = "<table style=\" font-size: 20px; background-color: #F2F2F2; width: 800px; padding: 20px; border: 0px solid #D9D9D9;border-spacing:0px;border-collapse: collapse;\">";
                _htmlBody = string.Concat(_htmlBody, "<tr><td style=\"font-size:20px;\">", _msg, "</td></tr>");
                _htmlBody = string.Concat(_htmlBody, "<tr><td style=\" font-size: 20px; font-weight: bold;\">", FromName, "</td></tr></table>");

                mailMessage.Bcc.Add(_to);
                mailMessage.Body = _htmlBody;

               // System.Net.WebClient objWebClient = new System.Net.WebClient();
                imgURL = imgURL.Substring(0, imgURL.LastIndexOf(","));
                imgURL = imgURL.Trim();
                SPFile imgFile = _web.GetFile(imgURL);
                byte[] obj = (byte[])imgFile.OpenBinary();
                MemoryStream stream = new MemoryStream(obj);
                System.Net.Mail.LinkedResource imageResource = new System.Net.Mail.LinkedResource(stream);
                imageResource.ContentId = "HDIImage";
                System.Net.Mail.AlternateView plainTextView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(_htmlBody.Trim(), null, "text/plain");
                System.Net.Mail.AlternateView htmlView = System.Net.Mail.AlternateView.CreateAlternateViewFromString("<html><head></head><body><center><img src = cid:HDIImage />" + _htmlBody.Trim() + "</center></body></html>", null, "text/html");

                htmlView.LinkedResources.Add(imageResource);

                mailMessage.AlternateViews.Add(plainTextView);
                mailMessage.AlternateViews.Add(htmlView);


                mailMessage.IsBodyHtml = true;

                string address = _web.Site.WebApplication.OutboundMailServiceInstance.Server.Address;
                SmtpClient smtpClient = new SmtpClient(address)
                {
                    Credentials = new NetworkCredential(_web.Site.WebApplication.OutboundMailUserName, _web.Site.WebApplication.OutboundMailPassword)
                };
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine("Message :" + ex.Message + "<br/>" + Environment.NewLine + "StackTrace :" + ex.StackTrace +
                       "" + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                    writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                }
            }
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