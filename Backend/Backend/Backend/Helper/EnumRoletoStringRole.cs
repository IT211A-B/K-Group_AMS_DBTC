using Backend.Backend.Helper.Enum;

namespace Backend.Backend.Helper
{
    public static class EnumRoletoStringRole
    {
        public static string ConvertEnumRoletoStringRole(this PosEnum.PosStatus role)
        {
            string result = "";
            switch (role)
            {
                case PosEnum.PosStatus.STU:
                    result = "Student";
                    break;
                case PosEnum.PosStatus.TEA:
                    result = "Teacher";
                    break;
                case PosEnum.PosStatus.ADM:
                    result = "Admin";
                    break;
            }
            return result;
        }
    }
}
