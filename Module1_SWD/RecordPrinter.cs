using System;
using System.Collections.Generic;

namespace Module1_SWD
{
    public class RecordPrinter
    {
        public static void PrintFileFormat(Dictionary<string, List<object>> attributesToRecords)
        {
        }

        public static void PrintEachAttribute(Dictionary<string, List<object>> attributesToRecords)
        {
            foreach (var attributesToRecord in attributesToRecords)
            {
                PrintAttribute(attributesToRecord);
            }
        }

        public static void PrintAttribute(KeyValuePair<string, List<object>> attributesToRecords)
        {
            Console.WriteLine(attributesToRecords.Key);
            foreach (Object value in attributesToRecords.Value)
            {
                Console.WriteLine(value);
            }

            Console.WriteLine();
        }
    }
}