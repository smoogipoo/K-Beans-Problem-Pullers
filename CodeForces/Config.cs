using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeForces
{
    class Config
    {
        private Dictionary<string, string> configs = new Dictionary<string, string>();

        private string filename;

        public Config(string filename)
        {
            this.filename = filename;

            using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
            using (StreamReader sr = new StreamReader(fs))
            {
                while (sr.Peek() != -1)
                {
                    string line = sr.ReadLine();

                    string[] split = line.Split(':');
                    if (split.Length < 2)
                        continue;

                    configs.Add(split[0], split[1]);
                }
            }
        }

        public T Get<T>(string key)
        {
            string val;
            if (!configs.TryGetValue(key, out val))
                configs[key] = val = string.Empty;

            return (T)Convert.ChangeType(val, typeof(T));
        }

        public void Set<T>(string key, T value)
        {
            configs[key] = value.ToString();
        }
    }
}
