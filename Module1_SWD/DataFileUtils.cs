using System;
using System.Collections.Generic;

namespace Module1_SWD
{
    public class DataFileUtils
    {
        public static Dictionary<string, List<Object>> ReadFile(string filePath, bool hasHeaders)
        {
            Dictionary<string, List<Object>> recordsByAttribute = new Dictionary<string, List<Object>>();

            string line;
            bool isHeaderInitialize = false;
            string[] splitHeaders = null;

            System.IO.StreamReader file = new System.IO.StreamReader(filePath);
            while ((line = file.ReadLine()) != null)
            {
                if (!String.IsNullOrEmpty(line))
                {
                    if (isHeaderInitialize == false && !line.Contains("#"))
                    {
                        if (!hasHeaders)
                        {
                            string tmp = line.Replace(';', ' ');
                            string[] splitFirstRow = tmp.Split(null);
                            splitHeaders = new string[splitFirstRow.Length];
                            for (var i = 0; i < splitFirstRow.Length; i++)
                            {
                                string customAttributeName = "Attribute_" + i;
                                recordsByAttribute.Add(customAttributeName, new List<object>());
                                splitHeaders[i] = customAttributeName;
                            }
                            SplitRecordsLine(line, recordsByAttribute, splitHeaders);
                        }
                        else
                        {
                            InitializeHeaders(line, recordsByAttribute, out splitHeaders, hasHeaders);
                        }
                        isHeaderInitialize = true;
                    }
                    else if (!line.Contains("#"))
                    {
                        SplitRecordsLine(line, recordsByAttribute, splitHeaders);
                    }
                }

            }

            file.Close();
            return recordsByAttribute;
        }

        public static void SplitRecordsLine(string line, Dictionary<string, List<object>> attributesToRecords,
            string[] splitHeaders)
        {
            string[] split = line.Split(null);
            for (var i = 0; i < split.Length; i++)
            {
                List<Object> tmpRecords;
                attributesToRecords.TryGetValue(splitHeaders[i], out tmpRecords);
                if (TypeComparator.IsFractionalNumeric(split[i]))
                {
                    tmpRecords.Add(Convert.ToDecimal(split[i]));
                }
                else if (TypeComparator.IsNumeric(split[i]))
                {
                    tmpRecords.Add(Convert.ToInt32(split[i]));
                }
                else
                {
                    tmpRecords.Add(split[i]);
                }

            }
        }

        private static void InitializeHeaders(string line, Dictionary<string, List<object>> attributesToRecords,
            out string[] splitHeaders, bool hasHeaders)
        {
            string headers;
            headers = line.Replace(';', ' ');
            splitHeaders = headers.Split(null);
            for (var i = 0; i < splitHeaders.Length; i++)
            {
                attributesToRecords.Add(splitHeaders[i], new List<object>());
            }

        }
    }
}