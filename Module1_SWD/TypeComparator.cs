using System;

namespace Module1_SWD
{
    public class TypeComparator
    {
        public static bool IsText(Object objToCheck)
        {
            return objToCheck is string;
        }

        public static bool IsNumeric(string objToCheck)
        {
            return Int32.TryParse(objToCheck, out Int32 num);
        }

        public static bool IsFractionalNumeric(string objToCheck)
        {
            return Decimal.TryParse(objToCheck, out Decimal num);
        }
    }
}