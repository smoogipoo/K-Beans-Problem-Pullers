using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Google.Spreadsheets;

namespace CodeForces
{
    class Program
    {
        static void Main(string[] args)
        {
            bool writeToGoogle = false;
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-google":
                        writeToGoogle = true;
                        break;
                }
            }

            Config config = new Config("codeforces.cfg");
            string clientId = config.Get<string>("clientid");
            string clientSecret = config.Get<string>("clientSecret");
            string accessToken = config.Get<string>("accesstoken");
            string refreshToken = config.Get<string>("refreshtoken");



            WebClient wc = new WebClient();
            string data = wc.DownloadString("http://codeforces.com/api/problemset.problems");

            ApiRequestResult res = JsonConvert.DeserializeObject<ApiRequestResult>(data);
            res.result.problems.Reverse();

            using (StreamWriter sw = new StreamWriter("result.csv"))
            {
                sw.WriteLine("ProblemID,ProblemLink,Tags");
                foreach (Problem problem in res.result.problems)
                    sw.WriteLine($"{ problem.contestId }/{ problem.index },http://codeforces.com/problemset/problem/{ problem.contestId }/{ problem.index },\"{ string.Join(",", problem.tags) }\"");
            }
        }

    }

    class Problem
    {
        public int contestId;
        public string index;
        public string name;
        public ProblemType type;
        public float points;
        public List<string> tags;
    }

    class ProblemStatistics
    {
        public int contestId;
        public string index;
        public int solvedCount;
    }

    class ApiResult
    {
        public List<Problem> problems;
        public List<Problem> problemStatistics;
    }

    [Serializable]
    class ApiRequestResult
    {
        public string status;
        public ApiResult result;
    }

    enum ProblemType
    {
        PROGRAMMING,
        QUESTION
    }
}
