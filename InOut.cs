using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System; // system garbage collector
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices; // Marshal
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;   //microsoft Excel

namespace SimpleTask
{

    static class InOut
    {

        public class Element
        {
            public string Title { get; set; }
            public string Type { get; set; }
            public string Text { get; set; }
            [JsonProperty("Color Index")]
            public int ColorIndex { get; set; }
            public int ClickCount { get; set; }
        }

        public class JSONGroup
        {
            public string Title { get; set; }
            public string Notes { get; set; }
            public List<Element> Elements { get; set; }
            public JSONGroup()
            {
                Elements = new List<Element>();
            }
        }

        public class RootObject
        {
           [JsonProperty("Simpletask data")]
            public List<JSONGroup> Simpletaskdata { get; set; }
        }

        const string SettingsFile = "SimpleTaskSettings.txt";
       
        public static void ReadFromJSON(List<group> Groups, List<Color> ButtonColor)
        {
            string asdf = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "SimpleTaskData.txt");

            RootObject rt = JsonConvert.DeserializeObject<RootObject>(asdf);

            for(int i = 0; i < rt.Simpletaskdata.Count; i++)
            {
                group g = new group();

                g.GroupName = rt.Simpletaskdata[i].Title;
                g.Notes = rt.Simpletaskdata[i].Notes;
                

                for(int j = 0; j < rt.Simpletaskdata[i].Elements.Count; j++)
                {
                    DataButton db = new DataButton();
                    db.Title = rt.Simpletaskdata[i].Elements[j].Title;
                    db.Type = rt.Simpletaskdata[i].Elements[j].Type;
                    db.Text = rt.Simpletaskdata[i].Elements[j].Text;
                    db.ClickCount = rt.Simpletaskdata[i].Elements[j].ClickCount;

                    int index = rt.Simpletaskdata[i].Elements[j].ColorIndex;
                    db.Color = ButtonColor[index];

                    g.DataButtons.Add(db);
                }
                Groups.Add(g);
            }


        }
        public static void OutputToJSON(List<group> Groups, List<Color> ButtonColor)
        {
            JObject json =
                new JObject(
                    new JProperty("Simpletask data",
                    new JArray(
                        from g in Groups
                        select new JObject(
                            new JProperty("Title", g.GroupName),
                            new JProperty("Notes", g.Notes),
                            new JProperty("Elements",
                            new JArray(
                                from b in g.DataButtons
                                select new JObject(
                                    new JProperty("Title", b.Title),
                                    new JProperty("Type", b.Type),
                                    new JProperty("Text", b.Text),
                                    new JProperty("ClickCount", b.ClickCount),
                                    new JProperty("Color Index", ButtonColor.IndexOf(b.Color))
                                    )))))));

            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "SimpleTaskData.txt", json.ToString());

        }
        
        public static void ReadSettingsFromTextFile(ref int[] defaultColorIndex, ref bool[] searchSettings, ref int LastClosedPanelIndex, ref Size buttonSize)
        {

            string[] lines = File.ReadAllLines(SettingsFile); // read all lines to string array

            LastClosedPanelIndex = int.Parse(lines[0]);

            string line = lines[1];

            string[] values = line.Split(' ');

            defaultColorIndex[0] = 0;
            defaultColorIndex[0] = int.Parse(values[0]);
            defaultColorIndex[1] = int.Parse(values[1]);
            defaultColorIndex[2] = int.Parse(values[2]);

            line = lines[2];
            values = line.Split(' ');

            for (int i = 0; i < 8; i++)
            {
                if (values[i] == "0")
                    searchSettings[i] = false;
                else
                    searchSettings[i] = true;
            }

            line = lines[3];
            values = line.Split(' ');
            buttonSize.Width = int.Parse(values[0]);
            buttonSize.Height = int.Parse(values[1]);

        }

        public static void OutPutSettingsToTextFile(int[] defaultColorIndex, bool[] searchSettings, int LastClosedPanelIndex, Size buttonSize)
        {
            using (StreamWriter file = new StreamWriter(SettingsFile))
            {
                file.WriteLine(LastClosedPanelIndex);

                for (int i = 0; i < 3; i++)
                {
                    file.Write("{0} ", defaultColorIndex[i]);
                }

                file.Write("\n");

                for (int i = 0; i < 8; i++)
                {
                    if (searchSettings[i])
                    {
                        file.Write("1 ");

                    }
                    else
                    {
                        file.Write("0 ");
                    }

                }

                file.Write("\n{0} {1}", buttonSize.Width, buttonSize.Height);

            }

        }

      

    }
}
