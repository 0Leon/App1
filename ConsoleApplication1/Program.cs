using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class MonitorSample
    {
        private ArrayList _canyBox = new ArrayList();
        private volatile bool _shouldStop = false;

        public void StopThread()
        {
            _shouldStop = true;
            Monitor.Enter(_canyBox);
            try
            {
                Monitor.PulseAll(_canyBox);
            }
            catch
            {
                Monitor.Exit(_canyBox);
            }

        }

        public void Produce()
        {
            while (!_shouldStop)
            {
                Monitor.Enter(_canyBox);
                try
                {
                    if (_canyBox.Count == 0)
                    {
                        _canyBox.Add("A candy");
                        Console.WriteLine("生产者：有糖吃啦。");
                        Monitor.Pulse(_canyBox);
                        Console.WriteLine("生产者：赶快来吃。");
                        Monitor.Wait(_canyBox);
                    }
                    else
                    {
                        Console.WriteLine("生产者：糖罐是满的。");
                        Monitor.Pulse(_canyBox);
                        Monitor.Wait(_canyBox);
                    }
                }
                finally
                {
                    Monitor.Exit(_canyBox);

                }
                Thread.Sleep(2000);
            }
            Console.WriteLine("生产者:下班啦。。");
        }


        public void Consume()
        {
            while (!_shouldStop || _canyBox.Count > 0)
            {
                Monitor.Enter(_canyBox);
                try
                {
                    if (_canyBox.Count == 1)
                    {
                        _canyBox.RemoveAt(0);
                        if (!_shouldStop)
                        {
                            Console.WriteLine("消费者：糖吃完了");
                        }
                        else
                        {
                            Console.WriteLine("消费者：还有糖没有吃完，马上就完");
                        }
                        Monitor.Pulse(_canyBox);
                        Console.WriteLine("消费者：赶快生产！");
                        Monitor.Wait(_canyBox);
                    }
                    else
                    {
                        Console.WriteLine("消费者：糖罐是空的！");
                        Monitor.Pulse(_canyBox);
                        Monitor.Wait(_canyBox);
                    }
                }
                finally
                {

                    Monitor.Exit(_canyBox);
                }

                Thread.Sleep(2000);
            }
            Console.WriteLine("消费者：都吃光啦，下次再吃！");
        }

    }
    class Program
    {
        static void Main(string[] args)
        {

            MonitorSample ss = new MonitorSample();
            Thread thProduce = new Thread(ss.Produce);
            Thread thConsume = new Thread(ss.Consume);
            Thread thConsume2 = new Thread(ss.Consume);
            Thread thConsume3 = new Thread(ss.Consume);

            thProduce.Start();
            Thread.Sleep(2000);
            thConsume.Start();
            thConsume2.Start();
            thConsume3.Start();
            Console.ReadLine();
            ss.StopThread();
            Thread.Sleep(1000);

            while (thProduce.ThreadState != ThreadState.Stopped)
            {
                ss.StopThread();
                thProduce.Join(1000);
            }

            while (thConsume.ThreadState != ThreadState.Stopped)
            {
                ss.StopThread();
                thConsume.Join(1000);
            }
            Console.WriteLine( "回车结束");
            Console.ReadLine();
        }
    }
}
