using System.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;

namespace LukeRSS
{
    class Program
    {
        public static Dictionary<string, Entry> Entries = new Dictionary<string, Entry>();

        static void Main(string[] args)
        {
            Load();
            Display();
        }

        private static void Load()
        {
            var htmlRegex = new Regex("<[^>]*>", RegexOptions.Compiled);
            var wc = new WebClient();
            var xml = wc.DownloadString("https://lukesmith.xyz/rss.xml");
            var entry = new Entry();

            using (var srdr = new StringReader(xml))
            using (var rdr = XmlReader.Create(srdr))
            {
                while (rdr.Read())
                {
                    if (!rdr.IsStartElement())
                        continue;

                    switch (rdr.Name)
                    {
                        case "item":
                            entry = new Entry();
                            break;
                        case "title":
                            if (rdr.Read())
                                entry.Title = rdr.Value;
                            break;
                        case "description":
                            if (rdr.Read())
                                entry.Content = htmlRegex.Replace(rdr.Value, "").Trim();
                            if (entry.Title == string.Empty)
                                entry.Title = "Untitled" + Entries.Count;
                            Entries.TryAdd(entry.Title, entry);
                            break;
                    }
                }
            }
        }
        private static void Display()
        {
            foreach (var kvp in Entries.Take(5))
            {
                Console.WriteLine(kvp.Key);
                Console.WriteLine("------------");
                Console.WriteLine(kvp.Value.Content);
                Console.WriteLine("____________");
            }
        }
    }
}