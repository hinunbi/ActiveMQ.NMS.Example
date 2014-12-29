using Common.Logging;
using Spring.Context;
using Spring.Context.Support;
using Spring.Messaging.Nms.Listener;
using System;
using System.Threading;


namespace Com.Brm.Messaging
{
    public class MessageApplication
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(MessageApplication));

        static void Main(string[] args)
        {
            //Spring.NET 변수들
            IApplicationContext ctx = null;
            MessageProducer producer = null;
            Thread producerThread = null;
            SimpleMessageListenerContainer consumer = null;

            // 종료키 등록 및 종료 로직
            Console.CancelKeyPress += delegate(object sender, ConsoleCancelEventArgs e)
            {
                e.Cancel = false;
                if (producer != null)
                {
                    producer.RequestStop();
                    producerThread.Join();
                    producer = null;
                    producerThread = null;
                }
                if (ctx != null)
                {
                    ctx.Dispose();
                }
                Console.WriteLine("Done.");
            };

            while (true)
            {
                try
                {
                    ctx = ContextRegistry.GetContext();

                    // 메시지 발신자 시작
                    LOG.Info("Main thread: Starting MessageProducer...");
                    producer = (MessageProducer)ctx.GetObject("MessageProducer");
                    producerThread = new Thread(producer.SendMessage);
                    producerThread.Start();

                    // 메시지 수신자 시작
                    LOG.Info("Main thread: Starting MessageConsumer...");
                    consumer = (SimpleMessageListenerContainer)ctx.GetObject("MessageListenerContainer");
                    consumer.Start();

                    // 메인 스레드 중지(종료키대기)
                    Thread.Sleep(Timeout.Infinite);
                }
                catch (Exception e)
                {
                    LOG.Warn(e.Message);
                    // Request that the worker thread stop itself:
                    if (producer != null)
                    {
                        producer.RequestStop();
                        producerThread.Join();
                        producer = null;
                        producerThread = null;
                    }
                    if (ctx != null)
                    {
                        ctx.Dispose();
                        ctx = null;
                        Thread.Sleep(1000);
                    }
                }
            }
        }
    }
}

