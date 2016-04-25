using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ACM
{
    class Program
    {
        private const string default_link = "https://icpcarchive.ecs.baylor.edu/{0}";
        private static Regex reg = new Regex("<tr class=\"sectiontableentry[1|2]\">.*?<a href=\"(index.php.*?)\">(.*?)</a>");

        static void Main(string[] args)
        {
            const string root_link = "index.php?option=com_onlinejudge&Itemid=8&category=0";

            Regex rootRegex = new Regex("<tr class=\"sectiontableentry[1|2]\">.*?<td><a href=\"(.*?)\">(.*?)</a>");

            List<Item> problems;

            using (WebClient client = new WebClient())
                problems = getItems(client, root_link);

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

        private static List<Item> getItems(WebClient wc, string location, string currentCategory = "")
        {
            string rootData = wc.DownloadString(string.Format(default_link, location));
            rootData = rootData.Replace("\n", "").Replace("\r", "").Replace("\t", "");

            // Find tables
            MatchCollection matches = reg.Matches(rootData);

            List<Item> problems = new List<Item>();

            foreach (Match match in matches)
            {
                string name = WebUtility.HtmlDecode(match.Groups[2].Value);
                string link = WebUtility.HtmlDecode(match.Groups[1].Value);

                if (link.Contains("show_problem"))
                    problems.Add(new Item() { Link = string.Format(default_link, link), Name = name, Category = currentCategory });
                else
                    problems.AddRange(getItems(wc, link, $"{ currentCategory }/{ name }"));
            }

            return problems;
        }

        struct Item
        {
            public string Link;
            public string Name;
            public string Category;
        }
    }
}
