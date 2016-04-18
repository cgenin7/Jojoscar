using System.Collections.Generic;
using Novacode;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using JojoscarMVCCommun;
using JojoscarMVCBusinessLogic;

namespace JojoscarMVC
{
    public class FormatVotes
    {
        public static void CreateDoc(string destFileName, List<CategoryModel> dbCategories, List<GuestModel> guests)
        {
            // Create a document in memory:
            DocX doc = DocX.Create(destFileName);

            GuestModel choix = Calculation.CalculateJojoscarChoice(guests);

            Paragraph header = doc.InsertParagraph();
            header.Alignment = Alignment.center;
            header.Append("Le choix des Jojoscar pour la 14ème soirée des JOJOSCAR\r\n").Color(Color.DarkRed).FontSize(16).Bold();

            AddChoixInDoc(choix, dbCategories, doc);

            doc.InsertSectionPageBreak();

            List<GuestModel> statistics = Calculation.CalculateStats(guests);
            AddStatistics(statistics, choix, dbCategories, doc);

            doc.InsertSectionPageBreak();

            foreach (GuestModel guest in guests)
            {
                header = doc.InsertParagraph();
                header.Alignment = Alignment.center;
                header.Append("Vos choix pour la 14ème soirée des JOJOSCAR\r\n").Color(Color.DarkRed).FontSize(16).Bold();
                header.Append(guest.FirstName + " " + guest.LastName + " - " + guest.AccessCode + (guest.Penality ? " - Pénalité " + Calculation.PENALITY + " points" : "")).FontSize(16).Bold();

                AddVotesInDoc(guest, dbCategories, doc);
            }
            // Save to the output directory:
            try
            {
                doc.Save();

                // Open in Word
                Process.Start("WINWORD.EXE", destFileName);
            }
            catch { } // CGe

        }

        public static Dictionary<string, CategoryNomineeModel> GetCategoriesFromPhp(string phpInfo, List<CategoryModel> categories, out List<CategoryNomineeModel> infosList)
        {
            Dictionary<string, CategoryNomineeModel> infos = new Dictionary<string, CategoryNomineeModel>();
            infosList = new List<CategoryNomineeModel>();

            StringReader file = new StringReader(phpInfo);
            string line;

            while ((line = file.ReadLine()) != null)
            {
                // line = "VALUE="Q1 - A - American Hustle" NAME=..."
                string strToFind = "VALUE=\"Q";
                int index = line.IndexOf(strToFind);
                if (index >= 0)
                {
                    // line = "1 - A - American Hustle" NAME=..."
                    line = line.Substring(index + strToFind.Length);
                    index = line.IndexOf("\"");
                    if (index >= 0)
                    {
                        // line = "1 - A - American Hustle"
                        line = line.Substring(0, index);
                        index = line.IndexOf(" ");
                        if (index > 0)
                        {
                            int quategoryNb;
                            if (int.TryParse(line.Substring(0, index), out quategoryNb))
                            {
                                index = line.IndexOf("-");
                                if (index >= 0)
                                {
                                    // line = " A - American Hustle"
                                    line = line.Substring(index + 1);
                                    index = line.IndexOf("-");
                                    if (index > 0)
                                    {
                                        string voteLetter = line.Substring(0, index).Trim();
                                        // line = "American Hustle"
                                        line = line.Substring(index + 1).Trim();
                                        string voteDetails = line;

                                        CategoryNomineeModel info = new CategoryNomineeModel() {Category = categories.Find(c => c.CategoryNb == quategoryNb), Letter = voteLetter, Description = voteLetter + " - " + voteDetails };
                                        infos.Add(info.Category.CategoryName + " - " + info.Letter, info);
                                        infosList.Add(info);
                                    }
                                }
                            }
                        }
                    }
                }

            }

            return infos;
        }

        private static void AddChoixInDoc(GuestModel guest, List<CategoryModel> categories, DocX doc)
        {
            Paragraph paraVotes = doc.InsertParagraph();

            var votes = guest.GetVotes();

            for (int i = 0; i < 24; i++)
            {
                paraVotes.Append("\r\n\r\n" + categories[i].CategoryName + ":  ").Bold();

                string strToAppend = "";
                foreach (var categoryNominee in categories[i].CategoryNominees)
                {
                    if (votes[i].Contains(categoryNominee.Letter))
                    {
                        strToAppend += (string.IsNullOrEmpty(strToAppend) ? "" : ", " ) + categoryNominee.Description;
                    }
                }
                paraVotes.Append(strToAppend);
            }
        }

        private static void AddVotesInDoc(GuestModel guest, List<CategoryModel> categories, DocX doc)
        {
            Paragraph paraVotes = doc.InsertParagraph();

            var votes = guest.GetVotes();

            for (int i = 0; i < 24; i++)
            {
                foreach (var categoryNominee in categories[i].CategoryNominees)
                {
                    if (votes[i] == categoryNominee.Letter)
                    {
                        string strCategory = "\r\n\r\n" + categories[i].CategoryName + ":  ";
                        string strDescription = categoryNominee.Description;

                        if ((strCategory.Length + strDescription.Length) > 80)
                            strDescription = strDescription.Substring(0, 80 - strCategory.Length);
                        paraVotes.Append(strCategory).Bold();
                        paraVotes.Append(strDescription);
                    }
                }
            }
        }

        private static void AddStatistics(List<GuestModel> statistics, GuestModel choix, List<CategoryModel> categories, DocX doc)
        {
            Table statsTable = doc.AddTable(25, categories[0].CategoryNominees.Count + 1);

            foreach (Cell cell in statsTable.Rows[0].Cells)
                cell.FillColor = Color.LightGray;

            statsTable.Rows[0].Cells[0].Paragraphs[0].Append("CATEGORIES").Bold();

            var choixVotes = choix.GetVotes();
            foreach (var category in categories)
            {
                if (category.CategoryNb == 1)
                    AddHeaders(statsTable.Rows[0], category);
                string categoryName = category.CategoryName;
                
                statsTable.Rows[category.CategoryNb].Cells[0].Paragraphs[0].Append(categoryName);
                statsTable.Rows[category.CategoryNb].Cells[0].FillColor = Color.LightGray;

                foreach (var categoryNominee in category.CategoryNominees)
                {
                    int statisticIndex = GetIndexFromLetter(categoryNominee.Letter);

                    bool isChoice = choixVotes[category.CategoryNb - 1].Contains(categoryNominee.Letter);
                    if (statisticIndex != -1)
                        AddNbVotesInRow(statsTable.Rows, statistics[statisticIndex], category.CategoryNb, statisticIndex, isChoice);
                }
            }

            doc.InsertTable(statsTable);
        }

        private static void AddHeaders(Row row, CategoryModel category)
        {
            foreach (var categoryNominee in category.CategoryNominees)
            {
                row.Cells[GetIndexFromLetter(categoryNominee.Letter) + 1].Paragraphs[0].Append(categoryNominee.Letter).Bold();
            }
        }
        private static void AddNbVotesInRow(List<Row> rows, GuestModel statistic, int CategoryNb, int statisticIndex, bool isChoice)
        {
            rows[CategoryNb].Cells[statisticIndex+1].Paragraphs[0].Append(statistic.GetVotes()[CategoryNb - 1]);
            if (isChoice)
                rows[CategoryNb].Cells[statisticIndex + 1].Paragraphs[0].Color(Color.Green).Bold();
        }

        private static int GetIndexFromLetter(string letter)
        {
            switch (letter)
            {
                case "A":
                    return 0;
                case "B":
                    return 1;
                case "C":
                    return 2;
                case "D":
                    return 3;
                case "E":
                    return 4;
                case "F":
                    return 5;
                case "G":
                    return 6;
                case "H":
                    return 7;
                case "I":
                    return 8;
                case "J":
                    return 9;
            }
            return -1;
        }
    }
}
