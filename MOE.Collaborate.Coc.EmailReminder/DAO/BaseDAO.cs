using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using MOE.Collaborate.Coc.DTO;
using MOE.Collaborate.Coc.EmailReminder;
using MOE.Collaborate.Coc.EmailReminder.DAO;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Net.Mail;
using System.Net;
using System.Runtime.CompilerServices;

namespace MOE.Collaborate.Coc.EmailReminder.DAO
{ 
    public class BaseDAO
    {
        public BaseDAO()
        {
        }

        public void SendEmailNotification()
        {
            int num = 0;
            EmailTemplateDTO dateTime = new EmailTemplateDTO();
            UserDTO flag = new UserDTO();
            try
            {
                SPSecurity.RunWithElevatedPrivileges(() => {
                    object str;
                    object obj;
                    object str1;
                    object obj1;
                    using (SPSite sPSite = new SPSite(Program.SiteURL))
                    {
                        using (SPWeb sPWeb = sPSite.OpenWeb())
                        {
                            foreach (SPListItem item in sPWeb.Lists["Email Template"].Items)
                            {
                                dateTime = new EmailTemplateDTO();
                                EmailTemplateDTO reminder = dateTime;
                                object item1 = item["Title"];
                                if (item1 != null)
                                {
                                    str = item1.ToString();
                                }
                                else
                                {
                                    str = null;
                                }
                                if (str == null)
                                {
                                    str = "";
                                }
                                reminder.Title = (string)str;
                                EmailTemplateDTO emailTemplateDTO = dateTime;
                                object item2 = item["Title"];
                                if (item2 != null)
                                {
                                    obj = item2.ToString();
                                }
                                else
                                {
                                    obj = null;
                                }
                                if (obj == null)
                                {
                                    obj = "";
                                }
                                emailTemplateDTO.Subject = (string)obj;
                                dateTime.Body = item["Body"].ToString().Replace("[URL]", string.Concat(new string[] { "<a href='", Program.SiteURL, "'>", Program.SiteURL, "</a>" }));
                                dateTime.Reminder_Date_and_Time = Convert.ToDateTime(item["Send Reminder Date and Time"]);
                                dateTime.Active = Convert.ToBoolean(item["Active"]);
                                if ((!dateTime.Active ? false : dateTime.Reminder_Date_and_Time <= DateTime.Now))
                                {
                                    Logger.WriteLog(string.Concat("Start Sending Email for: ", dateTime.Title));
                                    Console.WriteLine(string.Concat("Start Sending Email for: ", dateTime.Title));
                                    sPWeb.AllowUnsafeUpdates = true;
                                    item["Active"] = false;
                                    item.Update();
                                    sPWeb.AllowUnsafeUpdates = false;
                                    foreach (SPListItem sPListItem in sPWeb.Lists["User Master List"].Items)
                                    {
                                        flag = new UserDTO();
                                        UserDTO userDTO = flag;
                                        object obj2 = sPListItem["Title"];
                                        if (obj2 != null)
                                        {
                                            str1 = obj2.ToString();
                                        }
                                        else
                                        {
                                            str1 = null;
                                        }
                                        if (str1 == null)
                                        {
                                            str1 = "";
                                        }
                                        userDTO.Name = (string)str1;
                                        UserDTO userDTO1 = flag;
                                        object item3 = sPListItem["Email"];
                                        if (item3 != null)
                                        {
                                            obj1 = item3.ToString();
                                        }
                                        else
                                        {
                                            obj1 = null;
                                        }
                                        if (obj1 == null)
                                        {
                                            obj1 = "";
                                        }
                                        userDTO1.Email = (string)obj1;
                                        flag.Completed_Survey = Convert.ToBoolean(sPListItem["Survey Complete"]);
                                        if (!flag.Completed_Survey)
                                        {
                                            StringDictionary stringDictionaries = new StringDictionary()
                                            {
                                                { "to", flag.Email ?? "" },
                                                { "subject", dateTime.Subject },
                                                { "content-type", "text/html" }
                                            };
                                            num++;
                                            try
                                            {
                                                MailMessage mailMessage = new MailMessage();
                                                mailMessage.From = new MailAddress(sPWeb.Site.WebApplication.OutboundMailSenderAddress);
                                                mailMessage.Subject = dateTime.Subject;
                                                mailMessage.IsBodyHtml = true;
                                                mailMessage.To.Add(flag.Email);
                                                mailMessage.Body = dateTime.Body;
                                                SmtpClient smtpClient = new SmtpClient(sPWeb.Site.WebApplication.OutboundMailServiceInstance.Server.Address)
                                                {
                                                    EnableSsl = true,
                                                    Credentials = new NetworkCredential(sPWeb.Site.WebApplication.OutboundMailUserName, sPWeb.Site.WebApplication.OutboundMailPassword)
                                                };
                                                smtpClient.Send(mailMessage);
                                                smtpClient.Dispose();
                                                Logger.WriteLog(string.Concat("mailMessage.Body: ", mailMessage.Body));
                                                Logger.WriteLog(string.Concat("Email sent to: ", flag.Email));
                                                Console.WriteLine(string.Concat(num.ToString(), ":Email sent to: ", flag.Email));
                                            }
                                            catch (Exception exception)
                                            {
                                                Logger.WriteLog(string.Concat("Email failed to sent to: ", flag.Email));
                                                Console.WriteLine(string.Concat("Email failed to sent to: ", flag.Email));
                                                Logger.WriteLog(exception.StackTrace);
                                            }
                                            Logger.WriteLog(string.Concat("SendEmailNotification Count: ", num.ToString()));
                                            Console.WriteLine(string.Concat("SendEmailNotification Count: ", num.ToString()));
                                        }
                                    }
                                }
                            }
                        }
                    }
                });
            }
            catch (Exception exception2)
            {
                Exception exception1 = exception2;
                Logger.WriteLog(string.Concat("SendEmailNotification: ", exception1.Message, " ", exception1.StackTrace));
            }
        }
    }
}