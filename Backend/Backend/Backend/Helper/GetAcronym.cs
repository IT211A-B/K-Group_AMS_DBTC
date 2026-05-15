using System.Text;

namespace Backend.Backend.Helper
{
    public static class GetAcronym
    {
        /// <summary>
        /// Convert Into Acronym Every First Capital Letter Each Word
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public static string GetAllCapitalLettersPerWord(this string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return string.Empty;

            StringBuilder acronym = new StringBuilder();

            string[] words = title.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (string word in words)
            {
                if (!string.IsNullOrEmpty(word) && char.IsLetter(word[0]) && char.IsUpper(word[0]))
                {
                    acronym.Append(char.ToUpper(word[0]));
                }
            }

            return acronym.ToString();
        }
    }
}
