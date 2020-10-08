// ReSharper disable UnusedVariable
namespace Module1_SWD
{
    public class TypeComparator
    {
        public static bool IsText(object objToCheck)
        {
            return objToCheck is string;
        }

        public static bool IsNumeric(string objToCheck)
        {
            return int.TryParse(objToCheck, out var num);
        }

        public static bool IsFractionalNumeric(string objToCheck)
        {
            return decimal.TryParse(objToCheck, out var num);
        }
    }
}