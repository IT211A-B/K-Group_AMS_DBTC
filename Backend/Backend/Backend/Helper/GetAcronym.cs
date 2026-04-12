using System.Text;

namespace Backend.Backend.Helper
{
    public static class GetAcronym
    {
        // Convert Into Acronym Every First Capital Letter Each Word
        public static string GetAllCapitalLettersPerWord(this string Title)
        {
            // No new string every loop
            StringBuilder ackronym = new StringBuilder();

            string[] words = Title.Split(' ');
            foreach (string word in words)
            {
                if (!string.IsNullOrEmpty(word) && char.IsUpper(word[1]))
                {
                    ackronym.Append(word[0]);
                }
            }
            return ackronym.ToString();
        }
    }
}
