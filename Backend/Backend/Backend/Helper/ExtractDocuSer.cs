using Backend.Backend.Helper.Enum;
using System.Runtime.CompilerServices;

namespace Backend.Backend.Helper
{
    public static class ExtractDocuSer
    {
        /// <summary>
        /// Extract The Datas from Document Series
        /// </summary>
        /// <param name="DocumentSeries"></param>
        /// <returns></returns>
        public static (int? ExtractedId, PosEnum.PosStatus ExtractedPosition, int? ExtractedDate ,int statusCode) ExtractDataFromDocumentSeries(this string DocumentSeries)
        {
            PosEnum.PosStatus posStatus = PosEnum.PosStatus.STU; //initializer

            string[] Data = DocumentSeries.Split('-');

            for (int position = 0; Data.Length > 0; position++)
            {
                if (string.IsNullOrEmpty(Data[position - 1]))
                    return (null, posStatus, null, 404); // Null or Empty ID
            }

            switch (Data[0])
            {
                case "STU":
                    posStatus = PosEnum.PosStatus.STU;
                    break;
                case "ADM":
                    posStatus = PosEnum.PosStatus.ADM; 
                    break;
                case "TEA":
                    posStatus = PosEnum.PosStatus.TEA;
                    break;
            }

            int id = int.Parse(Data[2]);
            int year = int.Parse(Data[1]);

            return (id, posStatus, year, 200);
        }
    }
}
