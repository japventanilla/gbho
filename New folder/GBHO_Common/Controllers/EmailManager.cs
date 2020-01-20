using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using System.IO;
using System.Collections;
using System.Configuration;
using System.Web.Configuration;
using System.Net.Configuration;

namespace GBHO_Common.Controllers
{
    /// <summary>
    /// Provides ability to send and manage emails
    /// </summary>
    public static class EmailManager
    {
        private const string mailSettingsPath = "system.net/mailSettings";

        /// <summary>
        /// Sends an email to the specified recipient(s)
        /// </summary>
        /// <param name="to">comma separated list of recipient emails</param>
        /// <param name="subject"></param>
        /// <param name="body"></param>        
        public static void sendEmail(string to, string subject, string body)
        {                        
            sendEmail(to, null, subject, body, null);
        }


        /// <summary>
        /// Sends an email to the specified recipient(s)
        /// </summary>
        /// <param name="to">comma separated list of recipient emails</param>
        /// <param name="cc">comma separated list of recipient emails</param>
        /// <param name="subject"></param>
        /// <param name="body"></param>        
        public static void sendEmail(string to, string cc, string subject, string body)
        {
            sendEmail(to, cc, subject, body, null);
        }



        /// <summary>
        /// Sends an email to the specified recipient(s)
        /// </summary>
        /// <param name="to">comma separated list of recipient emails</param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="attachments">byte array dictionary with key as the attachment names</param>
        public static void sendEmail(string to, string subject, string body, IDictionary<string, byte[]> attachments)
        {
            sendEmail(to, null, subject, body, attachments);
        }


        /// <summary>
        /// Sends an email to the specified recipient(s)
        /// </summary>
        /// <param name="to">comma separated list of recipient emails</param>
        /// <param name="cc">comma separated list of recipient emails</param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="attachments">byte array dictionary with key as the attachment names</param>    
        public static void sendEmail(string to, string cc, string subject, string body, IDictionary<string,byte[]> attachments)
        {
            string from = getConfiguredFromAddress();

            if (String.IsNullOrEmpty(from))
                throw new ArgumentNullException("from");

            if (String.IsNullOrEmpty(to))
                throw new ArgumentNullException("to");
            
            MailMessage message = new MailMessage();
            
            //get the to recipeients
            MailAddressCollection toList = GetAddresses(to);
            foreach(MailAddress address in toList)
                message.To.Add(address);

            //get the cc recipeients
            MailAddressCollection ccList = GetAddresses(cc);
            foreach (MailAddress address in ccList)
                message.To.Add(address);

            string myFrom = string.Format("{0}<{1}>", ConfigurationManager.AppSettings["WebsiteName"], from);
            message.From = new MailAddress(myFrom);
            message.Subject = subject;
            message.Body = body;

            //this is an assumption.
            message.IsBodyHtml = true;

            if (attachments != null)
            {
                //attach the byte array dictionary items as attachments to the email.
                foreach (KeyValuePair<string, byte[]> attachmentItem in attachments)
                    message.Attachments.Add(new Attachment(new MemoryStream((byte[])attachmentItem.Value), attachmentItem.Key));
            }
            
            SmtpClient client = new SmtpClient();            
            client.Send(message);
        }

        /// <summary>
        /// extracts email addresses from a string and returns a collection.
        /// </summary>
        /// <param name="emailAddress">comma separated list (exclude comma if only one address)</param>
        /// <returns></returns>
        private static MailAddressCollection GetAddresses(string emailAddress)
        {
            MailAddressCollection mailAddressCollection = new MailAddressCollection();

            if (!String.IsNullOrEmpty(emailAddress))
            {
                string[] recipients = emailAddress.Split(',');
           
                if (recipients != null)
                {
                    for (int i = 0; i < recipients.Length; i++)
                    {
                        if (!String.IsNullOrEmpty(recipients[i]))
                            mailAddressCollection.Add(recipients[i]);
                    }
                }
            }

            return mailAddressCollection;
        }

        /// <summary>
        /// Gets the from address from config.
        /// </summary>
        /// <returns></returns>
        public static string getConfiguredFromAddress()
        {
            string returnFromAddress = ConfigurationManager.AppSettings["DefaultFromEmailAddress"];

            try
            {
                //will this config file change ?
                Configuration configurationFile = WebConfigurationManager.OpenWebConfiguration("~\\Web.config");
                MailSettingsSectionGroup mailSettings = configurationFile.GetSectionGroup(mailSettingsPath) as MailSettingsSectionGroup;
                if (mailSettings != null)
                {
                    returnFromAddress = mailSettings.Smtp.From;
                }
            }
            catch
            {
                //TODO: remove this once a common email framework has been decided.
            }

            return returnFromAddress;
        }
    }
}
