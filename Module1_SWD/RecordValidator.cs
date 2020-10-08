using System;
using System.Collections.Generic;

namespace Module1_SWD
{
    public class RecordValidator
    {
        public static bool ValidAllAttributes(Dictionary<string, List<object>> recordsByAttribute)
        {
            bool valid = true;
            foreach (var keyValuePair in recordsByAttribute)
            {
                if (!IsValid(keyValuePair.Value))
                {
                    Console.WriteLine(keyValuePair.Key + " is not valid");
                    valid = false;
                }
            }

            return valid;
        }

        public static bool IsValid(List<Object> records)
        {
            Type typeToExpect = null;
            for (var i = 0; i < records.Count; i++)
            {
                if (typeToExpect == null)
                {
                    typeToExpect = records[i].GetType();
                }
                else if (records[i].GetType() != typeToExpect)
                {
                    return false;
                }
            }

            return true;
        }
    }
}