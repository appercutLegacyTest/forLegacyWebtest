using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace TraceContext
{
    public class TraceContext
    {
        public List<TraceItem> TraceItemsList { get; private set; }
        public Stopwatch Watcher { get; private set; }

        public TraceContext()
        {
            TraceItemsList = new List<TraceItem>();
            Watcher = Stopwatch.StartNew();
        }

        public TraceItem StartTraceItem()
        {
            var traceItem = new TraceItem(Watcher);
            lock (TraceItemsList) {
                TraceItemsList.Add(traceItem);
            }            
            return traceItem;
        }

        public override string ToString()
        {
            var result = new StringBuilder();
            result.AppendFormat("Total time: {0:00} ms", Watcher.Elapsed.TotalMilliseconds);
            result.AppendLine();

            TraceItem[] traceItems;
            lock (TraceItemsList)
            {
                traceItems = TraceItemsList.ToArray();
            }

            var counter = 1;
            foreach (var traceItem in traceItems)
            {
                var message = traceItem.Message;

                result.AppendFormat(
                    "{0}. Started at {1}, duration {2}:", counter, traceItem.StartTime, traceItem.Elapsed);
                result.AppendLine();
                result.Append(message);
                result.AppendLine();

                counter++;
            }

            return result.ToString();
        }
    }


}
