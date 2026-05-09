using System.Text;

namespace Backend.Backend.Helper
{
    public static class ThreeFirstLetterCapital
    {
        /// <summary>
        /// The Name Will Be Converted Into an 3 First Word Ackronym
        /// </summary>
        /// <param name="Word"></param>
        /// <returns></returns>
        public static string CapitalTheFirstThreeLetterInWord(this string Word)
        {
            StringBuilder Ackronym = new StringBuilder();
            for (int i = 0; i < 3; i++)
            {
                Ackronym.Append(char.ToUpper(Word[i]));
            }

            return Ackronym.ToString();
        }
    }
}
