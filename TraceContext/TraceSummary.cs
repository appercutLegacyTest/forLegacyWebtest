using System.Collections.Generic;
using System.Text;

namespace TraceContext
{
    public class TraceSummary
    {
        public string PrintReport(TraceContext traceContext)
        {
            var result = new StringBuilder();
            result.AppendLine("Title \t\tTotal\tMax\tMin\tAvg\tCount");

            TraceItem[] traceItems;
            lock (traceContext.TraceItemsList)
            {
                traceItems = traceContext.TraceItemsList.ToArray();
            }

            var resultDictionary = CalculateStatistics(traceItems);

            foreach (KeyValuePair<string, StatisticsObject> entry in resultDictionary)
            {
                result.AppendFormat("{0} \t{1:0}\t{2:0}\t{3:0}\t{4:0}\t{5}",
                    entry.Key,
                    entry.Value.Total,
                    entry.Value.Max,
                    entry.Value.Min,
                    (entry.Value.Total / entry.Value.Count),
                    entry.Value.Count);
                result.AppendLine();
            }

            return result.ToString();
        }

        private SortedDictionary<string, StatisticsObject> CalculateStatistics(TraceItem[] traceItems)
        {
            var resultDictionary = new SortedDictionary<string, StatisticsObject>();
            foreach (var traceItem in traceItems)
            {
                if (traceItem.IsCancelled) continue;
                if (!resultDictionary.ContainsKey(traceItem.MethodName))
                {
                    resultDictionary[traceItem.MethodName] = new StatisticsObject
                    {
                        Total = 0,
                        Max = traceItem.Elapsed,
                        Min = traceItem.Elapsed,
                        Count = 0
                    };
                }

                resultDictionary[traceItem.MethodName].Total += traceItem.Elapsed;
                
                if (resultDictionary[traceItem.MethodName].Max < traceItem.Elapsed)
                {
                    resultDictionary[traceItem.MethodName].Max = traceItem.Elapsed;
                }

                if (resultDictionary[traceItem.MethodName].Min > traceItem.Elapsed)
                {
                    resultDictionary[traceItem.MethodName].Min = traceItem.Elapsed;
                }

                resultDictionary[traceItem.MethodName].Count++;
            }
            return resultDictionary;
        }

        internal class StatisticsObject
        {
            public double Total { get; set; }
            public double Max { get; set; }
            public double Min { get; set; }
            public int Count { get; set; }
        }
    }
}
