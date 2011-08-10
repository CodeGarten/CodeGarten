using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using NUnit.Framework;

namespace CodeGarten.Logger.Tests
{
    [TestFixture]
    public class LoggerTests
    {
        [Test]
        public void BasicTest()
        {
            var path = @"C:\CodeGarten\loggerTest.txt";
            var writer = File.CreateText(path);
            var logger = new Logger(writer);
            var list = new LinkedList<Thread>();
            var numbers = new bool[1000];
            logger.Start();
            Thread thread;
            for (var i = 0; i < numbers.Length; i++)
            {
                var iContext = i;
                thread = new Thread(() => logger.Log(iContext.ToString()));
                thread.Start();
                list.AddLast(thread);
            }

            foreach (var threadList in list)
                threadList.Join();

            writer.Close();
            var reader = File.ReadAllLines(path);
            
            foreach (var s in reader)
            {
                int i;
                var split = s.Split(new[] {':', ' '}, StringSplitOptions.RemoveEmptyEntries);
                Int32.TryParse(split[4], out i);
                numbers[i] = true;
            }


            foreach (var number in numbers)
                if(!number)
                    throw new Exception();

            logger.Stop();
        }
        
    }
}
