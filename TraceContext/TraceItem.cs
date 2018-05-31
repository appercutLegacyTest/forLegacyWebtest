using System.Diagnostics;

namespace TraceContext
{
    public class TraceItem
    {
        public string Message { get; private set; }
        public string MethodName { get; private set; }
        public bool IsStopped { get; private set; }
        public bool IsCancelled { get; private set; }
        public double StartTime { get; private set; }
        public double StopTime { get; private set; }
        public double Elapsed
        {
            get => IsStopped ? StopTime - StartTime : 0;
        }

        private Stopwatch _watcher;

        public TraceItem(Stopwatch watcher)
        {
            _watcher = watcher;
            StartTime = _watcher.Elapsed.TotalMilliseconds;
            IsStopped = false;
            IsCancelled = false;

            StackFrame frame = new StackFrame(2);
            var method = frame.GetMethod();
            MethodName = method.Name;
        }

        public void Complete(string text)
        {
            if (!IsStopped)
            {
                StopTime = _watcher.Elapsed.TotalMilliseconds;
                Message = text;
                IsStopped = true;
            }
        }

        public void Cancel()
        {
            if (!IsStopped)
            {
                Message = "cancelled";
                IsCancelled = true;
                StopTime = _watcher.Elapsed.TotalMilliseconds;
                IsStopped = true;
            }
        }

    }
}