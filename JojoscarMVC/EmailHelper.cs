using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net;
using JojoscarMVCBusinessLogic;

namespace JojoscarMVC
{
    public class EmailHelper
    {
        MailMessage _mail = null;

        public EmailHelper(string mailHost, int mailPort, string username, string password)
        {
            _mail = new MailMessage();
            Priority = MailPriority.High;
            m_mailHost = mailHost;
            m_mailPort = mailPort;
            m_userName = username;
            m_password = password;
        }
 
        public string MailHost {get; private set;}
 
        public string Subject { get; set; }
           
        public string HTMLBody { get; set; }
 
        public string TextBody { get; set; }
           
        public MailPriority Priority { get; set; }

        public string From { get; set; }

        public void AddTo(string to)
        {
            if (!string.IsNullOrEmpty(to))
                _mail.To.Add(to);
        }
 
        public void AddCC(string cc)
        {
            if (!string.IsNullOrEmpty(cc))
                _mail.CC.Add(cc);
        }
 
        public void AddBcc(string bCC)
        {
            if (!string.IsNullOrEmpty(bCC))
                _mail.Bcc.Add(bCC);
        }
 
        public bool SendMail()
        {
            return SendMail(null);
        }
 
        public bool SendMail(List<string> attachmentsFileNames)
        {
            try
            {
                BasicValidation();

                SmtpClient SmtpServer = new SmtpClient(m_mailHost, m_mailPort);
                SmtpServer.Credentials = new NetworkCredential(m_userName, m_password);
                SmtpServer.EnableSsl = true;
                _mail.From = new MailAddress(From);
                _mail.Subject = Subject;
                _mail.Priority = Priority;
             
                _mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                //mail.AlternateViews=
                if (HTMLBody.Length > 0)
                {
                    _mail.IsBodyHtml = true;
                    _mail.Body = HTMLBody;
                }
                else
                {
                    _mail.IsBodyHtml = false;
                    _mail.Body = TextBody;
                }

                _mail.Attachments.Clear();
                if (attachmentsFileNames != null)
                {
                    foreach (string attachmentFileName in attachmentsFileNames)
                    {
                        _mail.Attachments.Add(new Attachment(attachmentFileName));
                    }
                }
                SmtpServer.Send(_mail);
            }
            catch (Exception ex)
            {
                Logger.LogError("Could not send email: " + ex);
                return false;
            }
            return true;
        }
 
        private void BasicValidation()
        {
            bool err = false;
 
            if (string.IsNullOrEmpty(HTMLBody))
                err = true;
            else if (_mail.To.Count == 0)
                err = true;
            else if (string.IsNullOrEmpty(Subject))
                err = true;
 
            if (err)
                throw new Exception("Unexpected error. Unexpected Email's Parameters");
        }

        private string m_mailHost;
        private int m_mailPort;
        private string m_userName;
        private string m_password;
    }
}
