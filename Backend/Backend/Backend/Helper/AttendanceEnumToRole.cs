using Backend.Backend.Helper.Enum;

namespace Backend.Backend.Helper
{
    public static class AttendanceEnumToRole
    {
        public static string ConvertEnumAttendancetoStringAttendance(this AttendanceEnum.AttStatus role)
        {
            string result = "";
            switch (role)
            {
                case AttendanceEnum.AttStatus.Absent:
                    result = "Absent";
                    break;
                case AttendanceEnum.AttStatus.Present:
                    result = "Present";
                    break;
                case AttendanceEnum.AttStatus.Late:
                    result = "Late";
                    break;
                case AttendanceEnum.AttStatus.Excused:
                    result = "Excused";
                    break;
                case AttendanceEnum.AttStatus.Unassigned:
                    result = "Unassigned";
                    break;
            }
            return result;
        }
    }
}
