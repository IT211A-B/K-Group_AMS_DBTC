namespace Backend.Backend.Helper
{
    public static class UsersFirstName
    {
        /// <summary>
        /// Will Get User's First Name
        /// </summary>
        /// <param name="fullname"></param>
        /// <returns></returns>
        public static string GetUsersFirstName(this string fullname)
        {
            string firstname = "";
            foreach (char letter in fullname)
            {
                if (char.IsWhiteSpace(letter))
                    break;
                firstname += letter;
            }
            return firstname;
        }
    }
}
