using System;
using System.Collections.Generic;
using System.Linq;

namespace Module1_SWD
{
    public class MathUtils
    {
        public static List<Decimal> Normalization(List<Decimal> values, Int32 decimals = 7)
        {
            decimal avg = (decimal)Avg(values);
            decimal standardDeviation = (decimal)StandardDeviation(values);
            return values.Select(i =>
            {
                decimal result = (avg - (decimal)i) / standardDeviation;
                return Math.Round(result, decimals);
            }).ToList();
        }

        public static Decimal StandardDeviation(List<Decimal> values)
        {
            Decimal standardDeviation = 0;
            int count = values.Count;
            if (count > 1)
            {
                Decimal avg = Avg(values);
                Decimal sum = values.Sum(d => (d - avg) * (d - avg));
                standardDeviation = (decimal)Math.Sqrt((double)(sum / count));
            }

            return standardDeviation;
        }

        public static Decimal Avg(List<Decimal> records)
        {
            return records.Average();
        }

        public static List<Decimal> Discretization(List<Decimal> records, Int32 range, int decimals = 7)
        {
            Decimal min = records.Min();
            Decimal max = records.Max();

            Decimal difference = max - min;
            Decimal step = Math.Round((difference / range), decimals);

            Decimal currentNewValue = min;
            List<Decimal> ranges = new List<Decimal>();
            while (currentNewValue < max)
            {
                ranges.Add(currentNewValue);
                currentNewValue += step;
            }
            ranges.Add(max);

            List<Decimal> resultRecords = new List<Decimal>();
            for (var i = 0; i < records.Count; i++)
            {
                for (var l = 0; l < ranges.Count; l++)
                {
                    if (l <= ranges.Count - 2)
                    {
                        if (l == 0 && (records[i] <= ranges[l + 1]  || records[i] <= ranges[0] ))
                        {
                            resultRecords.Add(ranges[l]);
                            break;
                        }
                        else if (records[i] > ranges[l] && records[i] <= ranges[l + 1])
                        {
                            resultRecords.Add(ranges[l + 1]);
                            break;
                        }
                        else if (records[i] == ranges[ranges.Count - 1])
                        {
                            resultRecords.Add(ranges[ranges.Count - 1]);
                            break;
                        }
                        // else
                        // {
                        //     resultRecords.Add(ranges[ranges.Count - 1]);
                        // }
                    }
                }
            }

            return resultRecords;
        }
    }
}