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

            foreach (var kvp in Entries)
            {
                Console.WriteLine(kvp.Key);
                Console.WriteLine("------------");
                Console.WriteLine(kvp.Value.Content);
                Console.WriteLine("____________");
            }

        }

        private static void Load()
        {
            var aregex = new Regex("<[^>]*>", RegexOptions.Compiled);
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
                        case "pubDate":
                            rdr.Read();
                                //entry.DateTime = DateTime.ParseExact(rdr.Value, "ddd, dd MMM yyyy HH:mm:ss zz00", CultureInfo.InvariantCulture);
                            break;
                        case "description":
                            if (rdr.Read())
                            {
                                entry.Content = aregex.Replace(rdr.Value, "").Trim();
                                entry.Content = Regex.Replace(entry.Content, @"(\r\n){2,}", "\r\n");
                                for (int i = 0; i < 1024; i++)
                                    entry.Content = entry.Content.Replace("  ", " ");
                            }
                            if(entry.Title == string.Empty)
                                entry.Title = "Untitled" +Entries.Count;
                            Entries.TryAdd(entry.Title, entry);
                            break;
                    }
                }
            }
        }
    }
}