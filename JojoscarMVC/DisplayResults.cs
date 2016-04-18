using JojoscarMVC.Models;
using JojoscarMVCCommun;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace JojoscarMVC
{
    public class CDisplayResults
    {
        public static bool DisplayNextResult(DisplayResultViewModel model, List<ResultModel> results,
            List<ResultModel> resultsForMoney)
        {
            if (model.NbTimes <= 0)
                model.NbTimes = 1;
            
            if (results == null)
            {
                return false;
            }

            if (model.NbTimes <= 8)
            {
                if (model.NbTimes >= 1) // Prix citron
                {
                    model.LabelCitron = FormatPrixCitron(results);
                }
                if (model.NbTimes >= 2) // 5eme position
                {
                    model.LabelFifth = FormatResults(resultsForMoney, 5);
                    if (string.IsNullOrEmpty(model.LabelFifth) && model.NbTimes == 2)
                        model.NbTimes++;
                }
                if (model.NbTimes >= 3) // 4eme position
                {
                    model.LabelFourth = FormatResults(resultsForMoney, 4);
                    if (string.IsNullOrEmpty(model.LabelFourth) && model.NbTimes == 3)
                        model.NbTimes++;
                }
                if (model.NbTimes >= 4) // 3eme position
                {
                    model.LabelThird = FormatResults(resultsForMoney, 3);
                    if (string.IsNullOrEmpty(model.LabelThird) && model.NbTimes == 4)
                        model.NbTimes++;
                }
                if (model.NbTimes >= 5) // 2eme position
                {
                    model.LabelSecond = FormatResults(resultsForMoney, 2);
                    if (string.IsNullOrEmpty(model.LabelSecond) && model.NbTimes == 5)
                        model.NbTimes++;
                }
                if (model.NbTimes >= 6) // 1ere position
                {
                    model.LabelFirst = FormatResults(resultsForMoney, 1);
                    if (string.IsNullOrEmpty(model.LabelFirst) && model.NbTimes == 6)
                        model.NbTimes++;
                }
                if (model.NbTimes >= 7) // Trophée Jojoscar
                {
                    model.LabelTropheeJojoscar = FormatResultTrophee(results);
                }
                if (model.NbTimes >= 8) // Tableau des résultats
                {
                    model.Results = resultsForMoney;
                }
            }
            else
            {
                model.NbTimes = 0;
            }
            return true;
        }
        public static string FormatResults(List<ResultModel> results, int position)
        {
            string sRes = "";
            ResultModel finalRes = null;
            int nbWinners = 0;

            foreach (ResultModel result in results)
            {
                if (result.Position > 0)
                {
                    if (result.Position == position)
                    {
                        finalRes = result;
                        if (sRes != "")
                        {
                            sRes += Environment.NewLine;
                        }
                        sRes += result.Name;
                        nbWinners++;
                    }
                }
            }
            if (finalRes != null)
            {
                sRes += Environment.NewLine + finalRes.NbPointsWithPenality + " points - " + finalRes.NbResponses + " bonnes réponses - Bourse : " + finalRes.Remboursement.ToString("C");
            }

            return sRes;
        }

        public static string FormatPrixCitron(List<ResultModel> results)
        {
            int minNbResponses = int.MaxValue;
            int minNbPoints = int.MaxValue;
            string sRes = "";

            if (results.Count <= 0)
            {
                minNbResponses = 0;
                minNbPoints = 0;
            }
            else
            {
                foreach (ResultModel result in results)
                {
                    if (result.NbResponses == minNbResponses)
                    {
                        if (result.NbPointsWithoutPenality == minNbPoints)
                        {
                            if (!String.IsNullOrEmpty(sRes))
                            {
                                sRes += Environment.NewLine;
                            }
                            sRes += result.Name;
                        }
                        else if (result.NbPointsWithoutPenality < minNbPoints)
                        {
                            minNbResponses = result.NbResponses;
                            minNbPoints = result.NbPointsWithoutPenality;
                            sRes = result.Name;
                        }
                    }
                    else if (result.NbResponses < minNbResponses)
                    {
                        minNbResponses = result.NbResponses;
                        minNbPoints = result.NbPointsWithoutPenality;
                        sRes = result.Name;
                    }
                }
            }
            sRes += Environment.NewLine + minNbPoints + " points - " + minNbResponses + " bonnes réponses";
            return sRes;
        }

        public static string FormatResultTrophee(List<ResultModel> results)
        {
            int maxNbResponses = 0;
            int maxNbPoints = 0;
            string sRes = "";

            foreach (ResultModel result in results)
            {
                if (result.NbResponses == maxNbResponses)
                {
                    if (result.NbPointsWithPenality == maxNbPoints)
                    {
                        if (!string.IsNullOrEmpty(sRes))
                        {
                            sRes += Environment.NewLine;
                            sRes += result.Name;
                        }
                        else
                        {
                            sRes = result.Name;
                        }
                    }
                    else if (result.NbPointsWithPenality > maxNbPoints)
                    {
                        maxNbResponses = result.NbResponses;
                        maxNbPoints = result.NbPointsWithPenality;
                        sRes = result.Name;
                    }
                }
                else if (result.NbResponses > maxNbResponses)
                {
                    maxNbResponses = result.NbResponses;
                    maxNbPoints = result.NbPointsWithPenality;
                    sRes = result.Name;
                }
            }
            sRes += " " + maxNbResponses + " bonnes réponses";

            return sRes;
        }

        private static string EncryptString(string Message, string Passphrase)
        {
            Message = Message.Trim();
            byte[] Results;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
            // Step 1. We hash the passphrase using MD5 
            // We use the MD5 hash generator as the result is a 128 bit byte array 
            // which is a valid length for the TripleDES encoder we use below 
            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Passphrase));
            // Step 2. Create a new TripleDESCryptoServiceProvider object 
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();
            // Step 3. Setup the encoder 
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;
            // Step 4. Convert the input string to a byte[] 
            byte[] DataToEncrypt = UTF8.GetBytes(Message);
            // Step 5. Attempt to encrypt the string 
            try
            {
                ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
                Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
            }
            finally
            {
                // Clear the TripleDes and Hashprovider services of any sensitive information 
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }
            // Step 6. Return the encrypted string as a base64 encoded string 
            return Convert.ToBase64String(Results);
        }
    }
}
