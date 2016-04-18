using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace JojoscarMVCCommun
{
    public class GuestModel
    {
        public GuestModel()
        {
            AllVotes = "";
            AccessCode = 0;
            IsEligibleToMoney = true;
            IsPresent = true;
            FirstName = "";
            LastName = "";
        }

        public int GuestID { get; set; }
        [DisplayName("Prénom")]
        public string FirstName { get; set; }
        [DisplayName("Nom")]
        public string LastName { get; set; }
        [DisplayName("Code d'accès")]
        public int AccessCode { get; set; }
        [DisplayName("Courriel")]
        public string Email { get; set; }
        [DisplayName("Paiement effectué")]
        public bool PaymentDone { get; set; }
        [DisplayName("Pénalité")]
        public bool Penality { get; set; }
        [DisplayName("Est présent")]
        public bool IsPresent { get; set; }
        [DisplayName("Plus de 18 ans")]
        public bool IsEligibleToMoney { get; set; }
        public string AllVotes { get; set; }

        public List<string> GetVotes()
        {
            string[] splitVotes = AllVotes.Split(';');
            return new List<string>(splitVotes);
        }

        public void SetVotes(List<string> votes)
        {
            AllVotes = "";

            foreach (string vote in votes)
                AllVotes += (String.IsNullOrEmpty(AllVotes) ? vote : ";" + vote);
        }

        public void AddVote(string vote)
        {
            AllVotes += (String.IsNullOrEmpty(AllVotes) ? vote : ";" + vote);
        }

        public TableModel Table { get; set; }
        public int? TableId { get; set; }
    }
}