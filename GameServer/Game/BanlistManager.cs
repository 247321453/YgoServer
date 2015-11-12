using System.Collections.Generic;
using System.IO;

namespace YGOCore.Game
{
    public static class BanlistManager
    {
        private static List<Banlist> Banlists { get; set; }

        public static void Init(string fileName)
        {
            Banlists = new List<Banlist>();
            Banlist current = null;
            StreamReader reader = new StreamReader(fileName);
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (line == null)
                    continue;
                if (line.StartsWith("#"))
                    continue;
                if (line.StartsWith("!"))
                {
                	current = new Banlist(line.Replace("!",""));
                    Banlists.Add(current);
                    continue;
                }
                if (!line.Contains(" "))
                    continue;
                if (current == null)
                    continue;
                string[] data = line.Split(' ');
                int id = int.Parse(data[0]);
                int count = int.Parse(data[1]);
                current.Add(id, count);
            }
        }

        public static int GetIndex(uint hash)
        {
            for (int i = 0; i < Banlists.Count; i++)
                if (Banlists[i].Hash == hash)
                    return i;
            return 0;
        }
               public static string GetName(int lflist){
        	if(lflist>=0&&lflist<Banlists.Count){
        		return Banlists[lflist].Name;
        	}
        	return "";
        }
        public static Banlist GetBanlist(int lflist){
        	if(lflist>=0&&lflist<Banlists.Count){
        		return Banlists[lflist];
        	}
        	return Banlist.Emtry;
        }
    }
}