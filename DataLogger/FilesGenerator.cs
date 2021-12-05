using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Windows.Forms;

namespace DataLogger
{
    class FilesGenerator
    {        
        public static void createJSON(SaveFileDialog saveFileDialog, List<Clinic> clinicsList)
        {
            string strResultJson = "";
            foreach (var clinic in clinicsList)
            {   
                strResultJson += JsonConvert.SerializeObject(clinic) + "\n";
            }

            // zapisywanie pliku w sciezce
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
               string fileName = saveFileDialog.FileName;
               using (StreamWriter streamWriter = new StreamWriter(fileName))
                {
                    streamWriter.WriteLine(strResultJson);
                }

            }
            Console.WriteLine("Storrrrred");
        }
    }
}
