using JojoscarMVC.Models;
using JojoscarMVCBusinessLogic;

namespace JojoscarMVC
{
    public class HandleVotes
    {
        public static bool SendReceivedVoteConfirmationEmail(GuestViewModel guest)
        {
            if (!string.IsNullOrEmpty(guest.Guest.Email))
            {
                if (SendConfirmationEmail(guest.Guest.Email, guest.Guest.AccessCode + " - Jojoscar 2016 - Merci pour vos votes.", GetVoteEmailBody(guest)))
                    return true;
            }
            return false;
        }
      
        private static bool SendConfirmationEmail(string emailAddress, string subject, string body)
        {
            EmailHelper emailHelper = new EmailHelper("smtp.gmail.com", 587, "comitejojoscar@gmail.com", "jojo2010");
            emailHelper.Subject = subject;
            emailHelper.From = "claire_genin@yahoo.ca";
            emailHelper.AddTo(emailAddress);
           /* CGe emailHelper.AddCC("comite@jojoscar.com"); */
            emailHelper.AddBcc("claire_genin@yahoo.ca");
            emailHelper.HTMLBody = body;
            return emailHelper.SendMail();
        }

        private static string GetVoteEmailBody(GuestViewModel guest)
        {
            string sVotes = guest.EmailContent;
            int index = sVotes.IndexOf("Q1");
            if (index > 0)
                sVotes = sVotes.Substring(index);
            index = sVotes.LastIndexOf("telephone");
            if (index > 0)
                sVotes = sVotes.Substring(0, index);
            sVotes = sVotes.Replace("accesscode", "Code d'accès");

            for (int i = Calculation.NB_CATEGORIES; i >= 0; i--)
                sVotes = sVotes.Replace("Q" + (i + 1), "");    

            string body = "<html>" +
                "<font size='4' color='#3333ff'>Merci!<br/><br/>" +
                "Nous avons bien reçu vos votes. <br/><br/>" +
                "Rendez-vous le <b>28 février 2016 à 18h45</b> pour " +
                "le 14e Gala Annuel des Jojoscar! <br/><br/>" +
                "Le comité organisateur des Jojoscar <br/>" +
                "<a rel='nofollow' target='_blank' href='http://www.jojoscar.com'>www.jojoscar.com</a> </font><br/>" +
                "<br/><br/><b>VOS VOTES, " + guest.Guest.FirstName.ToUpper() + " " + guest.Guest.LastName.ToUpper() + ": </b><br/><br/>" + 
                sVotes.Replace("\r\n", "<br/>") +
                "</html>";

            return body;
        }
    }
}
