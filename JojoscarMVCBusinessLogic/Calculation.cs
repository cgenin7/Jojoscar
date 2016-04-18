using JojoscarMVCCommun;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JojoscarMVCBusinessLogic
{
    public class Calculation
    {
        public static double CalculateAmmountToShare(List<GuestModel> guests, List<CategoryModel> categories)
        {
            double total = 0;

            if (IsAcademyChoiceFilled(categories))
            {
                guests.Where(g => g.IsEligibleToMoney).ToList().ForEach(g =>
                {
                    List<string> guestVotes = g.GetVotes();
                    total += guestVotes.Zip(categories, (gv, ca) => string.Compare(gv.Trim(), ca.AcademyChoiceLetter.Trim()) != 0 ? 0.25 : 0).Sum();
                });
            }
            return total;
        }

        public static GuestModel CalculateJojoscarChoice(List<GuestModel> guests)
        {
            GuestModel choix;
            List<GuestModel> stats;

            CalculateStats(out choix, out stats, guests);

            return choix;
        }

        public static List<GuestModel> CalculateStats(List<GuestModel> guests)
        {
            List<GuestModel> stats;
            GuestModel choix;

            CalculateStats(out choix, out stats, guests);

            return stats;
        }

        public static void CalculateStats(out GuestModel choix, out List<GuestModel> stats, List<GuestModel> guests, string choiceName = "Choix")
        {
            string[] possibleVotes = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" };
            int nbPossibleVotes = possibleVotes.Length;

            stats = new List<GuestModel>();

            for (int i = 0; i < nbPossibleVotes; i++)
            {
                GuestModel guest = new GuestModel();
                stats.Add(guest);
            }

            choix = new GuestModel();
            choix.FirstName = choiceName;

            for (int i = 0; i < NB_CATEGORIES; i++)
            {
                int [] totalNbVotes = new int[nbPossibleVotes];
                foreach (GuestModel guest in guests)
                {
                    List<string> votes = guest.GetVotes();

                    if (votes.Count > i)
                        FillTotalNbVotes(votes[i], possibleVotes, totalNbVotes);
                    else
                    {
                        Logger.LogError("Not enough votes");
                        break;
                    }
                }

                choix.AddVote(FillChoix(possibleVotes, totalNbVotes));
                for (int j = 0; j < possibleVotes.Length; j++)
                {
                    stats[j].AddVote(totalNbVotes[j].ToString());
                }
            }
		}

		public static bool CalculateResults(List<GuestModel> guests, List<CategoryModel> categories, TPercent percents, out List<ResultModel> results, bool excludeNotEligibleForMoney = true)
		{
            results = new List<ResultModel>();
            if (IsAcademyChoiceFilled(categories))
            {
                foreach (GuestModel guest in guests)
                {
                    var result = CalculateGuestResults(guest, categories, excludeNotEligibleForMoney);
                    if (result != null)
                    {
                        result.NbPointsWithoutPenality = result.NbPointsWithPenality;

                        if (guest.Penality)
                            result.NbPointsWithPenality -= PENALITY;

                        if (result.IsEligibleToMoney)
                            result.Remboursement = m_montant * result.NbResponses;
                        else
                            result.Remboursement = 0;
                        result.IsPresent = guest.IsPresent;
                        results.Add(result);
                    }
                }

                FillPositions(ref results, 1);
                FillRemb(percents, ref results);

                results = results.OrderBy(r => r.Position).ToList();
                return true;
            }
            return false;
		}

        private static ResultModel CalculateGuestResults(GuestModel guest, List<CategoryModel> categories, bool excludeNotEligibleForMoney)
        {
            if (!excludeNotEligibleForMoney || guest.IsEligibleToMoney)
            {
                if (!string.IsNullOrEmpty(guest.FirstName) && guest.IsPresent)
                {
                    ResultModel result = new ResultModel();

                    result.Name = guest.FirstName + " " + guest.LastName;
                    result.IsEligibleToMoney = guest.IsEligibleToMoney;
                    if (guest.GetVotes().Where(v => string.IsNullOrEmpty(v)).Count() > 0)
                    {
                        // CGe add error, invalid votes
                        return null;
                    }

                    var guestVotes = guest.GetVotes();

                    for (int i = 0; i < NB_CATEGORIES; i++)
                    {
                        if (categories[i] != null && guestVotes[i] != null)
                        {
                            if (String.Compare(categories[i].AcademyChoiceLetter.Trim(), guestVotes[i].Trim(), true) == 0)
                            {
                                result.NbResponses++;
                                result.NbPointsWithPenality += categories[i].NbPoints;
                            }
                        }
                    }

                   if (result.IsEligibleToMoney)
                        result.Remboursement = m_montant * result.NbResponses;
                    else
                        result.Remboursement = 0;
                    result.IsPresent = guest.IsPresent;
                    return result;
                }
            }
            return null;
        }

        public static List<GuestModel> FillNewNames(string sExcelData, out bool success)
        {
            success = false;
            try
            {
                if (!string.IsNullOrEmpty(sExcelData))
                {
                    Dictionary<int, GuestModel> newNames = new Dictionary<int, GuestModel>();

                    // Does not fill penalities
                    string sData = sExcelData.Replace('\t', ',');

                    StringReader reader = new StringReader(sData);
                    string sNames = reader.ReadLine();
                    string[] names = sNames.Split(',');
                    if (names != null)
                    {
                        int nb = 0;
                        for (int index = 0; index < names.Length; index += 2)
                        {
                            string name = names[index].Trim();
                            if (string.IsNullOrEmpty(name))
                            {
                                return null;
                            }
                            GuestModel tName = new GuestModel();

                            ExtractName(tName, name);

                            newNames.Add(nb++, tName);
                        }
                    }

                    int categoryNb = 0;
                    for (int i = 0; i < NB_CATEGORIES; i++)
                    {
                        string sVotes = reader.ReadLine();
                        string[] votes = sVotes.Split(',');
                        if (votes != null)
                        {
                            int nameNb = 0;
                            for (int index = 0; index < votes.Length; index += 2)
                            {
                                if (!newNames.ContainsKey(nameNb))
                                {
                                    return null;
                                }
                                GuestModel tName = newNames[nameNb++];
                                string vote = votes[index].Trim().ToUpper();
                                if (vote == "X") vote = string.Empty;
                                if (vote == string.Empty || IsVoteValid(vote))
                                {
                                    tName.GetVotes()[categoryNb] = vote;
                                }
                            }
                        }
                        categoryNb++;
                    }
                    List<GuestModel> result = new List<GuestModel>();
                    foreach (GuestModel name in newNames.Values)
                    {
                        result.Add(name);
                    }
                    success = true;
                    return result;
                }
            }
            catch { }
            return null;
        }


        /* EMAIL FORMAT :
         Below is the result of your feedback form.  It was submitted by
        Claire Génin (claire_genin@yahoo.ca) on Thursday, January 30, 2014 at 09:12:10
        ---------------------------------------------------------------------------

        Q1BestPicture: Q1 - A - American Hustle

        Q2Directing: Q2 - B - Gravity - Alfonso Cuarón

        Q3ActorLeading: Q3 - A - Christian Bale in American Hustle

        Q4ActressLeading: Q4 - C - Sandra Bullock in Gravity

        Q5ActorSupporting: Q5 - B - Bradley Cooper in American Hustle

        Q6ActressSupporting: Q6 - B - Jennifer Lawrence in American Hustle

        Q7ProductionDesign: Q7 - B - Gravity

        Q8Cinematography: Q8 - B - Gravity

        Q9CostumeDesign: Q9 - B - The Grandmaster

        Q10MusicOriginalScore: Q10 - B - Gravity - Steven Price

        Q11MusicOriginalSong: Q11 - A - Alone Yet Not Alone from ALONE YET NOT ALONE

        Q12FilmEditing: Q12 - A - American Hustle

        Q13Makeup: Q13 - B - Jackass Presents: Bad Grandpa

        Q14SoundMixing: Q14 - B - Gravity

        Q15SoundEditing: Q15 - C - Gravity

        Q16VisualEffects: Q16 - B - The Hobbit: The Desolation of Smaug

        Q17AdaptedScreenplay: Q17 - C - Philomena

        Q18OriginalScreenplay: Q18 - A - American Hustle

        Q19AnimatedFeature: Q19 - D - Frozen

        Q20ForeignFilm: Q20 - B - The Great Beauty (Italy)

        Q21DocumentaryFeature: Q21 - B - Cutie and the Boxer

        Q22DocumentaryShort: Q22 - D - The Lady in Number 6 - Music Saved My Life

        Q23ShortFilmAnimated: Q23 - B - Get a Horse!

        Q24ShortFilmLiveAction: Q24 - D - Pitääkö Mun Kaikki Hoitaa? (Do I Have to Take Care of Everything?)

        accesscode: 1234

        telephone: 450 419-4360

        commentaires: Mon commentaire ...

        Jojoscar-contact-form: Envoyer
         */
        public static GuestModel FillInfoFromEmailContent(GuestModel guest,  string emailContent, out bool success)
        {
            success = false;

            GetVoterNameAndEmail(guest, emailContent);

            StringReader reader = new StringReader(emailContent);
            string line;

            if (emailContent.Contains("Q1 -")) // email format
            {
                List<string> votes = new List<string>();
                try
                {
                    for (int i = 0; i < NB_CATEGORIES; i++)
                    {
                        string strToFind = "Q" + (i + 1).ToString() + " -";
                        do
                        {
                            line = reader.ReadLine();
                        }
                        while (line != null && !line.Contains(strToFind));

                        if (line == null)
                        {
                            return guest;
                        }
                        string specificVote = GetSelection(line, strToFind);
                        if (IsVoteValid(specificVote))
                        {
                            votes.Add(specificVote);
                        }
                        else 
                        {
                            return guest;
                        }
                    }
                    guest.SetVotes(votes);
                    success = true;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                }
            }
            
            guest.AccessCode = GetAccessCode(emailContent, reader);
            
            return guest;
        }
        
        public static float Montant
        {
            get { return m_montant; }
            set { m_montant = value; }
        }

        public static bool IsVoteValid(string vote)
        {
            if (vote != null)
            {
                vote = vote.Trim().ToUpper();
                return (vote == "A" || vote == "B" || vote == "C" || vote == "D" || vote == "E"
                    || vote == "F" || vote == "G" || vote == "H" || vote == "I" || vote == "J");
            }
            return false;
        }

        public static bool IsAcademyChoiceFilled(List<CategoryModel> categories)
        {
            return categories.Where(c => !IsVoteValid(c.AcademyChoiceLetter)).Count() == 0;
        }

        #region Private methods

        private static void ExtractName(GuestModel guest, string sName)
        {
            sName = sName.Trim();
            string[] splitName = sName.Split(' ');
            if (splitName != null && splitName.Length == 2)
            {
                guest.FirstName = splitName[0];
                guest.LastName = splitName[1];
            }
            else
                guest.FirstName = sName;
        }

        private static int GetAccessCode(string sNameDetails, StringReader reader)
        {
            string sAccessCodeName = "accesscode:";
            if (sNameDetails.Contains(sAccessCodeName))
            {
                string line;
                do
                {
                    line = reader.ReadLine();
                }
                while (line != null && !line.Contains(sAccessCodeName));
                if (line != null)
                {
                    int index = line.IndexOf(sAccessCodeName);
                    if (index >= 0)
                        line = line.Substring(index + sAccessCodeName.Length);
                    int code = 0;
                    if (int.TryParse(line.Trim(), out code))
                        return code;
                }
            }
            return 0;
        }

        private static void GetVoterNameAndEmail(GuestModel guest, string emailContent)
        {
            if (guest.FirstName == null)
                guest.FirstName = "";
            if (guest.LastName == null)
                guest.LastName = "";

            StringReader reader = new StringReader(emailContent);

            string sPreviousString = "It was submitted by";
            string sNextString = "(";
            string sLastString = ")";

            if (emailContent.Contains(sPreviousString))
            {
                string line;
                do
                {
                    line = reader.ReadLine();
                }
                while (line != null && !line.Contains(sPreviousString));
                if (line != null)
                {
                    int prevIndex = line.IndexOf(sPreviousString);
                    int nextIndex = line.IndexOf(sNextString);
                    if (nextIndex < 0)
                        line += reader.ReadLine();
                    nextIndex = line.IndexOf(sNextString);
                    if (nextIndex > prevIndex)
                    {
                        int startIndex = prevIndex + sPreviousString.Length;
                        string sName = line.Substring(startIndex, nextIndex - startIndex);
                        if (string.IsNullOrEmpty(guest.FirstName) && sName != null)
                        {
                            ExtractName(guest, sName);
                        }
                        int lastIndex = line.IndexOf(sLastString);
                        if (lastIndex > nextIndex)
                            guest.Email = line.Substring(nextIndex + 1, lastIndex - (nextIndex + 1)).Trim();
                    }
                }
            }
        }

		private static int FindMax(int[] Nbs)
        {
            if (Nbs == null || Nbs.Length <= 0)
                return 0;
            if (Nbs.Length == 1)
                return Nbs[0];
            if (Nbs.Length == 2)
                return Math.Max(Nbs[0], Nbs[1]);
            int[] NewNbs = new int[Nbs.Length - 1];
            NewNbs[0] = Math.Max(Nbs[0], Nbs[1]);
            for (int i = 2; i < Nbs.Length; i++)
            {
                NewNbs[i - 1] = Nbs[i];
            }
            return FindMax(NewNbs);
        }

        private static string FillChoix(string[] possibleVotes, int[] totalNbVotes)
        {
            string sChoix = "";
            
            int max = FindMax(totalNbVotes);

            if (max == 0) // no responses entered.
            {
                sChoix = "";
            }
            else
            {
                for (int i = 0; i < totalNbVotes.Length; i++)
                {
                    if (totalNbVotes[i] == max)
                    {
                        if (i < possibleVotes.Length)
                        {
                            sChoix += possibleVotes[i];
                        }
                    }
                }
            }
            return sChoix;
        }

        private static void FillTotalNbVotes(string vote, string[] possibleVotes, int[] totalNbVotes)
        {
            if (!string.IsNullOrEmpty(vote))
            {
                bool bFound = false;
                for (int i = 0; i < possibleVotes.Length; i++)
                {
                    if (string.Compare(vote.Trim(), possibleVotes[i], true) == 0)
                    {
                        if (i < totalNbVotes.Length)
                        {
                            totalNbVotes[i]++;
                            bFound = true;
                        }
                    }
                }
                if (!bFound)
                {
                    Logger.LogError("Vote invalide entré");
                }
            }
        }

        private static void FillPositions(ref List<ResultModel> results, int curPos)
        {
            int maxNbResponses = 0;
            int maxNbPoints = -1000;

            for (int i = 0; i < results.Count; i++)
            {
                ResultModel result = results[i];

                if (result.IsPresent)
                {
                    if (result.Position == 0)
                    {
                        if (result.NbPointsWithPenality >= maxNbPoints)
                        {
                            if (result.NbPointsWithPenality > maxNbPoints)
                            {
                                maxNbPoints = result.NbPointsWithPenality;
                                maxNbResponses = result.NbResponses;
                            }
                            if (result.NbResponses > maxNbResponses)
                            {
                                maxNbResponses = result.NbResponses;
                            }
                            result.Position = curPos;
                            //results[i] = result;
                        }
                    }
                }
            }

            int nbNotSet = 0;
            int nbInThatPos = 0;
            for (int i = 0; i < results.Count; i++)
            {
                ResultModel result = results[i];
                if (result.IsPresent)
                {
                    if (result.Position == curPos)
                    {
                        if (result.NbResponses == maxNbResponses && result.NbPointsWithPenality == maxNbPoints)
                        {
                            nbInThatPos++;
                        }
                        else
                        {
                            result.Position = 0;
                            results[i] = result;
                        }
                    }
                    if (result.Position == 0)
                    {
                        nbNotSet++;
                    }
                }
            }
            if (nbNotSet > 0 && nbInThatPos > 0)
            {
                FillPositions(ref results, curPos + nbInThatPos);
            }
        }

        private static void FillRemb(TPercent percents, ref List<ResultModel> results)
        {
            float[] prices = new float[5];
            float total = 0;
            int[] nbInPos = new int[5];
            int[] totalGoodVotes = new int[5];

            for (int i = 0; i < results.Count; i++)
            {
                ResultModel result = results[i];
                if (result.Position > 0 && result.Position <= 5)
                {
                    totalGoodVotes[result.Position - 1] = result.NbResponses;
                }
                total += (NB_CATEGORIES - result.NbResponses) * Montant;

                for (int j = 0; j < 5; j++)
                {
                    if (result.Position == j + 1)
                    {
                        nbInPos[j]++;
                    }
                }
            }

            prices[0] = total * percents.First / 100;
            prices[1] = total * percents.Second / 100;
            prices[2] = total * percents.Third / 100;
            prices[3] = total * percents.Fourth / 100;
            prices[4] = total * percents.Fifth / 100;

            float[] finalPrices = new float[] { 0,0,0,0,0};
            
            int startPos = 0;

            for (int i = 0; i < 5; i++)
            {
                for (int j = startPos; j < (startPos + nbInPos[i]) && j < 5; j++)
                {
                    finalPrices[i] += prices[j];
                }

                if (nbInPos[i] > 1)
                {
                    finalPrices[i] /= nbInPos[i];
                }
                startPos += nbInPos[i];
            }

            for (int i = 0; i < results.Count; i++)
            {
                ResultModel result = results[i];
                for (int j = 0; j < 5; j++)
                {
                    if (result.Position == j+1)
                    {
                        result.Remboursement += finalPrices[j];
                        result.Remboursement = RoundToClosest25Cent(result.Remboursement);
                        results[i] = result;
                        break;
                    }
                }
            }
        }

        internal static float RoundToClosest25Cent(float remb)
        {
            int nb = (int)remb;
            double decimalPart = remb - nb;

            if (decimalPart <= 0.125)
            {
                decimalPart = 0;
            }
            else if (decimalPart <= 0.375)
            {
                decimalPart = 0.25;
            }
            else if (decimalPart <= 0.625)
            {
                decimalPart = 0.50;
            }
            else if (decimalPart <= 0.875)
            {
                decimalPart = 0.75;
            }
            else
            {
                decimalPart = 1;
            }

            return (float)(nb + decimalPart);
        }

        private static string GetSelection(string line, string strToFind)
        {
            int index = line.IndexOf(strToFind);

            if (index >= 0)
            {
                line = line.Substring(index + strToFind.Length);
                index = line.IndexOf("-");
                if (index >= 0)
                {
                    line = line.Substring(0, index);
                    return line.Trim();
                }
            }

            return "";
        }

        #endregion
        
        private static float m_montant = 0.25F;
        public static readonly int NB_CATEGORIES = 24;
        public static readonly int PENALITY = 12;
    }
}
