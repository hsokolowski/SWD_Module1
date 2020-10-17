using System;
using System.Collections.Generic;
using System.Linq;

namespace Module1_SWD
{
    public class RecordUtils
    {
        public static KeyValuePair<string, List<object>> ChangeValuesToNumeric(
            string oldLabel,
            List<object> attributesToRecords,
            bool alphabetical)
        {
            List<object> copyListValues = CopyListValues(attributesToRecords);

            Dictionary<string, Int32> tmp = new Dictionary<string, int>();
            if (alphabetical)
            {
                copyListValues = copyListValues.OrderBy(q => q).ToList();
            }

            for (var i = 0; i < copyListValues.Count; i++)
            {
                tmp.Add((string)copyListValues[i], i + 1);
            }

            List<Object> result = new List<object>();
            for (var i = 0; i < attributesToRecords.Count; i++)
            {
                Int32 newValue;
                tmp.TryGetValue((string)attributesToRecords[i], out newValue);
                result.Add(newValue);
            }

            string newLabel = "N_" + oldLabel;
            return new KeyValuePair<string, List<object>>(newLabel, result);
        }


        private static List<object> CopyListValues(List<object> records)
        {
            List<object> result = new List<object>();
            foreach (Object o in records)
            {
                if (!result.Contains(o))
                {
                    result.Add(o);
                }
            }

            return result;
        }
    }
}
