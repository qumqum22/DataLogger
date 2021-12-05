using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataLogger
{
    class WebManager
    {

        public async Task<List<Clinic>> GetHtmlAsync(string url)
        {
            // Call asynchronous network methods in a try/catch block to handle exceptions.

            string rowWithHTML;
            List<Clinic> clinics = new List<Clinic>();
            Clinic clinic;
            return await Task.Factory.StartNew(() =>
            {
                HtmlWeb web = new HtmlWeb();
                HtmlAgilityPack.HtmlDocument document = web.Load(url);
                HtmlNodeCollection allRows = document.DocumentNode.SelectNodes("//tr"); // wybranie wszystkich wierszy
                if (!(allRows is null))
                {
                    foreach (HtmlNode row in allRows)
                    {
                        rowWithHTML = row.InnerText;
                        // wstępna obróbka wiersza
                        string rowText = PrepareRow(rowWithHTML);

                        if (rowText.Length > 3 && rowText.Contains("ul.") || rowText.Contains("al."))
                        {
                            rowText = DeleteRowIndex(rowText); // usuwanie indeksu poczatkowego
                            clinic = GetNameOfClinic(rowText); // wyciagniecie nazwy kliniki
                            clinics.Add(clinic);
                        }
                    }
                }
                return clinics;
            });
        }

        private static string PrepareRow(string rowWithHTML)
        {
            string rowWithoutHTML = Regex.Replace(rowWithHTML, @"<[^>]+>|&nbsp;", " ").Trim(); // usuwanie tagów html
            rowWithoutHTML = Regex.Replace(rowWithoutHTML, @"&oacute;", "ó").Trim(); // zamiana &oacute na ó
            rowWithoutHTML = Regex.Replace(rowWithoutHTML, @"&Oacute;", "Ó").Trim(); // zamiana &Oacute na Ó

            string rowText = Regex.Replace(rowWithoutHTML, @"\s{2,}", " "); // usuwanie zwielokrotnionych spacji
            rowText = Regex.Replace(rowText, @" -", "-"); // usuwanie spacji przed myslnikiem
            rowText = Regex.Replace(rowText, @"- ", "-"); // usuwanie spacji po myslniku
            return rowText;
        }

        private string DeleteRowIndex(string data)
        {
            int firstLetter = 0;
            while (!Char.IsLetter(data[firstLetter]))
            {
                ++firstLetter;
            }
            return data.Substring(firstLetter);
        }

        private Clinic GetNameOfClinic(string data)
        {
            int firstIndexOfStreet = data.IndexOf("ul.") == -1 ? data.IndexOf("al.") : data.IndexOf("ul.");
            int firstIndexOfPostalCode = firstIndexOfStreet;
            string name;
            string city = "";
            string postalCode = "";
            string address = "";
            string phones = "";
            if (firstIndexOfStreet == -1) return new Clinic();
            Match match = Regex.Match(data, "[0-9][0-9]-[0-9][0-9][0-9]");
            if (match.Success)
            {
                firstIndexOfPostalCode = match.Index;
            }
            if (firstIndexOfStreet < firstIndexOfPostalCode) // True - pierwsza ulica , False - pierwszy kod pocztowy
            {
                name = data.Substring(0, firstIndexOfStreet);
                address = data.Substring(firstIndexOfStreet, firstIndexOfPostalCode - firstIndexOfStreet);
                var index = data.Length - 1;
                for (var i = index; i >= 0; i--)
                {
                    if (!(char.IsNumber(data[i]) || char.IsWhiteSpace(data[i])))
                    {
                        i++;
                        index = i;
                        break;
                    }
                }
                postalCode = data.Substring(firstIndexOfPostalCode, index - firstIndexOfPostalCode);
                phones = GetNumberOfClinic(data);
            }
            else
            {
                name = data.Substring(0, firstIndexOfPostalCode - 1);
                postalCode = data.Substring(firstIndexOfPostalCode, firstIndexOfStreet - firstIndexOfPostalCode);
                phones = GetNumberOfClinic(data);
            }

            phones = Regex.Replace(phones, @" ", ""); // usuwanie spacji po myslniku
            for (int i = phones.Length - 9; i > 0; i = i - 9)
            {
                phones = phones.Insert(i, ", ");
            }

            Console.WriteLine("All data:" + data);
            Console.WriteLine("Name:" + name);
            Console.WriteLine("postalCode:" + postalCode);
            Console.WriteLine("address:" + address);
            Console.WriteLine("phones:" + phones);
            Clinic clinic = new Clinic(name, city, address, postalCode, phones);
            if (firstIndexOfStreet < firstIndexOfPostalCode)
                return clinic;
            return clinic;
        }

        private string GetNumberOfClinic(string data)
        {
            var stack = new Stack<char>();
            var index = data.Length - 1;
            for (var i = index; i >= 0; i--)
            {
                if (!(char.IsNumber(data[i]) || char.IsWhiteSpace(data[i])))
                {
                    i++;
                    break;
                }
                stack.Push(data[i]);
            }
            var numbers = new string(stack.ToArray());
            return numbers;
        }
    }
}
