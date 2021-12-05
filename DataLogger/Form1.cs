using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataLogger
{

    public partial class Form1 : Form
    {
        public List<Clinic> ClinicList { get; set; }
        public Dictionary<string, string> dictOfUrls = new Dictionary<string, string>();
        public Dictionary<string, string> dictOfUrls2 = new Dictionary<string, string>();
        public Dictionary<string, string> dictOfUrls3 = new Dictionary<string, string>();
        private static int couter =0;

        public Form1()
        {
            ClinicList = GetClinicList();
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dictOfUrls.Add("www.nfz-wroclaw.pl", "https://www.nfz-wroclaw.pl/default2.aspx?obj=45223;57172&des=1;2");
            // lodz podwojona
            dictOfUrls.Add("www.nfz-lodz.pl", "https://www.nfz-lodz.pl/dlapacjentow/nfz-blizej-pacjenta/9099-rehabilitacja-po-covid-19-wykaz-placowek");
            dictOfUrls2.Add("www.nfz-lodz.pl", "https://www.nfz-lodz.pl/dlapacjentow/nfz-blizej-pacjenta/9099-rehabilitacja-po-covid-19-wykaz-placowek");
            dictOfUrls.Add("www.nfz-lublin.pl", "https://www.nfz-lublin.pl/komunikat/5351");
            dictOfUrls.Add("www.nfz-zielonagora.pl", "https://www.nfz-zielonagora.pl/PL/1051/7604/Rehabilitacja_po_COVID-19_-_wykaz_placowek/");
            dictOfUrls.Add("www.nfz-krakow.pl", "https://nfz-krakow.pl/dla-pacjenta/aktualnosci/rehabilitacja-po-covid-19-stacjonarna-wykaz-placowek-aktualizacja,450.html");
            dictOfUrls.Add("www.nfz-warszawa.pl", "http://www.nfz-warszawa.pl/dla-pacjenta/aktualnosci/rehabilitacja-po-covid-19-aktualizacja,475.html");
            dictOfUrls.Add("www.nfz-opole.pl", "http://www.nfz-opole.pl/aktualnosci/aktualnosci-i-komunikaty/rehabilitacja-po-covid-19-ambulatoryjna-i-domowa-wykaz-placowek-aktualizacja,2474.html");
            dictOfUrls2.Add("www.nfz-rzeszow.pl", "https://www.nfz-rzeszow.pl/pacjenci/aktualnosci/pacjenci-informacje-ogolne/program-rehabilitacji-po-przebytej-chorobie-covid-19-wykaz-placowek,art1873/");
            dictOfUrls2.Add("www.nfz-bialystok.pl", "https://www.nfz-bialystok.pl/podmioty-medyczne/rehabilitacja-po-covid-19-wykaz-placowek/");
            dictOfUrls.Add("www.nfz-gdansk.pl", "https://www.nfz-gdansk.pl/dla-pacjenta/kompleksowy-program-rehabilitacji-po-przebytej-chorobie-covid-19,8383");
            dictOfUrls.Add("www.nfz-katowice.pl", "https://www.nfz-katowice.pl/index.php/o-nas/aktualnosci-oddzialu/item/125018-rehabilitacja-po-covid-19-wykaz-placowek-w-wojewodztwie-slaskim");
            dictOfUrls.Add("www.nfz-kielce.pl", "http://www.nfz-kielce.pl/wazne-informacje/rehabilitacja-po-covid-19-lista-placowek-2/");
            dictOfUrls.Add("www.nfz-poznan.pl", "http://nfz-poznan.pl/page.php/1/0/show/16334/");
            foreach (var singleUrl in dictOfUrls)
            {
                linksList.Items.Add(singleUrl.Key);
            }
        }

        private List<Clinic> GetClinicList()
        {
            var list = new List<Clinic>();
            list.Add(new Clinic()
            {
                Name = "Szpital w Gorlicach",
                City = "Gorlice",
                Street = "Długa 8",
                PostalCode = "33-312 Gorlice",
                Contact = "997"
            });
            list.Add(new Clinic()
            {
                Name = "Dom Zdrojowy I",
                City = "Jelenia Góra",
                Street = "pl. Piastowski 38",
                PostalCode = "58-560",
                Contact = "75 64 26 550"
            });
            list.Add(new Clinic()
            {
                Name = "Centrum Medyczne Karpacz SA",
                City = "Karpacz",
                Street = "ul. Myśliwska 1",
                PostalCode = "58-540",
                Contact = "883 358 173"
            });
            list.Add(new Clinic()
            {
                Name = "Centrum Kompleksowej Rehabilitacji Sp. z o.o.",
                City = "Konstancin-Jeziorna",
                Street = "ul. Gąsiorowskiego 12/14",
                PostalCode = "05-510",
                Contact = "22 703 02 05"
            });
            list.Add(new Clinic()
            {
                Name = "Szpital Solec Sp. z o.o.",
                City = "Warszawa",
                Street = "Pileckiego 99",
                PostalCode = "02-781",
                Contact = "22 166 91 80"
            });
            return list;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<CheckBox> checkBoxes = new List<CheckBox>() { checkBox1, checkBox2, checkBox3, checkBox4, checkBox5 };
            var clinics = this.ClinicList;


            FillListWithSelectedLinks();
            RemoveDuplicated(linksSelected);

            int itemsOnList = linksSelected.Items.Count;
            if (itemsOnList == 0)
            {
                label1.Text = "Musisz wybrac jakikolwiek element";
            }
            else
            {
                label1.Text = "Ładowanie danych..." + itemsOnList.ToString();


                foreach (string link in linksList.SelectedItems)
                {
                    Task<string> task = GetHtmlAsync(dictOfUrls[link]);
                    linksSelected.Items.Add(link);
                }

                label1.Text = "Zakończono ładowanie danych";
                dataGridView1.DataSource = clinics;
                SetVisibilityOfColumns(checkBoxes);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FilesGenerator.createJSON(saveFileDialog1 ,ClinicList);
        }

        private async Task<string> GetHtmlAsync(string url)
        {
            // Call asynchronous network methods in a try/catch block to handle exceptions.
            try
            {
                string rowWithHTML;
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
                                GetNameOfClinic(rowText); // wyciagniecie nazwy kliniki
                            }
                        }
                    }
                    return document.DocumentNode.OuterHtml;
                });

            }
            catch (HttpRequestException e)
            {
                linksSelected.Items.Add("\nException Caught!");
                linksSelected.Items.Add("Message :");
                linksSelected.Items.Add(e.Message.ToString());
                return "None";
            }
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

        private void FillListWithSelectedLinks()
        {
            linksSelected.Items.Clear();
            CopyListBoxItems(linksList, linksSelected);
            RemoveDuplicated(linksSelected);
        }

        private void SetVisibilityOfColumns(List<CheckBox> checkBoxes)
        {
            foreach (var checkBox in checkBoxes)
            {
                if (!checkBox.Checked)
                    dataGridView1.Columns[checkBox.Text.ToString()].Visible = false;
                else
                    dataGridView1.Columns[checkBox.Text.ToString()].Visible = true;
            }
        }

        private void CopyListBoxItems(ListBox source, ListBox destination)
        {
            ListBox.SelectedObjectCollection sourceItems = source.SelectedItems;
            foreach (var item in sourceItems)
            {   
                destination.Items.Add(item);
            }
        }

        private void RemoveDuplicated(ListBox list)
        {
            var hash = new HashSet<string>();
            foreach (string str in list.Items)
            {
                hash.Add(str);
            }

            list.Items.Clear();
            
            foreach (string str in hash)
            {
                list.Items.Add(str);
            }
        }

        private void select_all_links_button_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < linksList.Items.Count; i++)
            {
                linksList.SetSelected(i, true);
            }
        }

        private string DeleteRowIndex(string data)
        {
            int firstLetter = 0;
            while(!Char.IsLetter(data[firstLetter]))
            {
                ++firstLetter;
            }
            return data.Substring(firstLetter);
        }

        private int GetNameOfClinic(string data)
        {
            int firstIndexOfStreet = data.IndexOf("ul.") == -1 ? data.IndexOf("al.") : data.IndexOf("ul.");
            int firstIndexOfPostalCode = firstIndexOfStreet;
            string name;
            string postalCode = "";
            string address = "";
            string phones = "";
            if (firstIndexOfStreet == -1) return -1;
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
            for(int i = phones.Length - 9; i > 0; i = i - 9)
            {
                phones = phones.Insert(i, ", ");
            }

            Console.WriteLine("All data:" + data);
            Console.WriteLine("Name:" + name);
            Console.WriteLine("postalCode:" + postalCode);
            Console.WriteLine("address:" + address);
            Console.WriteLine("phones:" + phones);

            if (firstIndexOfStreet < firstIndexOfPostalCode)
                return 1;
            return 1;
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
