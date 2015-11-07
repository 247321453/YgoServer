using System.IO;

namespace OcgWrapper.Managers
{
    public static class PathManager
    {
        private static string m_path;
        private static string m_scripts;
        private static string m_cards;

        public static void Init(string path, string scripts, string cards)
        {
            m_path = path;
            m_scripts = scripts;
            m_cards = cards;
        }

        internal static string GetScript(string name)
        {
            name = name.Replace("./script", m_scripts);
            return Path.Combine(m_path, name);
        }

        internal static string GetCardsDb()
        {
            return Path.Combine(Path.GetFullPath(m_path), m_cards);
        }
    }
}