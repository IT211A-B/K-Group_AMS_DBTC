using Role = Backend.Backend.Helper.Enum.PosEnum.PosStatus;

namespace Backend.Backend.Helper
{
    public static class AddRole
    {
        // Add role through email, using this method
        public static (Role? role, int status_code) AddRoleAccordingToEmail(this string email)
        {
            Role? role;
            int status_code;
            if(email.Contains("@sup.user"))
            {
                role = Role.SUP;
                status_code = 200;
            } else if (email.Contains("@.dbtc-cebu"))
            {
                role = Role.STU;
                status_code = 200;
            } else if (email.Contains("@.local"))
            {
                role = Role.TEA;
                status_code = 200;
            } else if (email.Contains("@.admin"))
            {
                role = Role.ADM;
                status_code = 200;
            } else
            {
                role = null;
                status_code = 404;
            }

            return (role, status_code);
        }
    }
}
