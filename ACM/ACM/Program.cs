using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ACM
{
    class Program
    {
        /// <summary>
        /// True - Use IPC Archives
        /// False - Use Uva Archives
        /// </summary>
        private const bool IPC = false;
        private const string default_link = IPC ? "https://icpcarchive.ecs.baylor.edu/{0}" : "https://uva.onlinejudge.org/{0}";

        private static Regex reg = new Regex("<tr class=\"sectiontableentry[1|2]\">.*?<a href=\"(index.php.*?)\">(.*?)</a>");

        private static List<Item> problems = new List<Item>();

        static void Main(string[] args)
        {
            const string root_link = "index.php?option=com_onlinejudge&Itemid=8&category=0";

            Task.Run(() => getItems(root_link)).Wait();

            using (StreamWriter sw = new StreamWriter("results.csv"))
            {
                sw.WriteLine("Id,Name,Category,Tags,Link");

                for (int i = 0; i < problems.Count; i++)
                {
                    Item problem = problems[i];
                    sw.WriteLine($"{ i },\"{ problem.Name }\",\"{ problem.Category }\",\"{ problem.Category }\",{ problem.Link }");
                }
            }
        }

        private static async Task getItems(string location, string currentCategory = "")
        {
            using (WebClient wc = new WebClient())
            {
                string rootData = wc.DownloadString(string.Format(default_link, location));
                rootData = rootData.Replace("\n", "").Replace("\r", "").Replace("\t", "");

                // Find tables
                MatchCollection matches = reg.Matches(rootData);

                List<Task> subTasks = new List<Task>();

                foreach (Match match in matches)
                {
                    string name = WebUtility.HtmlDecode(match.Groups[2].Value);
                    string link = WebUtility.HtmlDecode(match.Groups[1].Value);

                    if (link.Contains("show_problem"))
                    {
                        lock (problems)
                            problems.Add(new Item() { Link = string.Format(default_link, link), Name = name.Replace("\"", ""), Category = currentCategory });
                    }
                    else
                        subTasks.Add(getItems(link, $"{ currentCategory }/{ name }"));
                }

                await Task.WhenAll(subTasks.ToArray());
            }
        }

        struct Item
        {
            public string Link;
            public string Name;
            public string Category;
        }
    }
}
