using Backend.Backend.Model;

namespace Backend.Backend.Helper.Enum
{
    public class PosEnum
    {

        /// <summary>
        /// Status For Position in Document Series
        /// -------- POSITION-YEAR-ID ------------
        /// Position's Acronym
        /// STU - Student
        /// TEA - Teacher
        /// ADM - Admin
        /// SUP - Super User
        /// </summary>


        public enum PosStatus
        {
            STU,
            TEA,
            ADM
        }
    }
}
