using System.Threading;

namespace TraceContext
{
    public class TestMethods
    {
        public void Test1000(TraceContext traceContext, int additionalDelay)
        {
            TraceItem traceItem = traceContext.StartTraceItem();
            Thread.Sleep(1000 + additionalDelay);
            traceItem.Complete("Test1 " + additionalDelay);
        }

        public void Test2000(TraceContext traceContext, int additionalDelay)
        {
            TraceItem traceItem = traceContext.StartTraceItem();
            Thread.Sleep(2000 + additionalDelay);
            traceItem.Complete("Test2 " + additionalDelay);
        }

        public void Test3000(TraceContext traceContext, int additionalDelay)
        {
            TraceItem traceItem = traceContext.StartTraceItem();
            Thread.Sleep(3000 + additionalDelay);
            traceItem.Complete("Test3 " + additionalDelay);
        }
    }
}
