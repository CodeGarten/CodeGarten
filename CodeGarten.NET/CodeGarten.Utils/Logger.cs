using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace CodeGarten.Utils
{
    public class Logger: IDisposable     
    {
        private readonly LinkedList<string> _msgQueue;
        private readonly TextWriter _textWriter;
        private Thread _owner;

        public Logger() : this(Console.Out)
        {}

        public Logger(TextWriter writer)
        {
            _textWriter = writer;
            _msgQueue = new LinkedList<string>();
        }

        public void Start()
        {
            var thread = new Thread(Run);
            Interlocked.CompareExchange(ref _owner, thread, null);

            if (thread != _owner)
                throw new InvalidOperationException("The Logger is already start");

            thread.Priority = ThreadPriority.Lowest;

            thread.Start();
        }

        private void PrintAll()
        {
            while (_msgQueue.Count > 0)
            {
                var msg = _msgQueue.First.Value;
                _textWriter.WriteLine(String.Format("{0}: {1}", DateTime.Now, msg));
                _msgQueue.RemoveFirst();
            }
        }

        private void Run()
        {

            lock (this)
            {
                do
                {
                    PrintAll();
                    try
                    {
                        Monitor.Wait(this);

                    }
                    catch (ThreadInterruptedException)
                    {
                        _owner = null;
                        PrintAll();
                        return;
                    }

                } while (true);
            }
        }

        public void Stop()
        {
            lock(this)
            {
                if (_owner == null)
                    throw new InvalidOperationException("The logger is not started");
                _owner.Interrupt();
            }
            
        }

        public void Log(string msg)
        {
            lock (this)
            {
                //TODO
                _msgQueue.AddLast(msg);

                Monitor.Pulse(this);
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
