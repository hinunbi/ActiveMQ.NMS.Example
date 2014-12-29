using System;
using Spring.Messaging.Nms.Core;
using System.Threading;
using Common.Logging;

namespace Com.Brm.Messaging
{
    public class MessageProducer : NmsGatewaySupport
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(MessageProducer));

        private TimeSpan delayTime = new TimeSpan(0, 0, 0, 0, 1000);
        public TimeSpan DelayTime
        {
            set
            {
                delayTime = value;
            }
        }

        private int messageCount = 1;

        private volatile bool shouldStop;

        public void process()
        {
        }
        public void SendMessage()
        {
            LOG.Info("MessageProducer thread: working...");
            while (!shouldStop)
            {
                String message = String.Format("\"[{0:0000}> {1}\"", messageCount++, "Hello");
                LOG.Info(String.Format("MessageProducer : " + message));
                bool sent = false;
                while (!sent && !shouldStop)
                {
                    try
                    {
                        NmsTemplate.ConvertAndSend(message);
                        sent = true;
                    }
                    catch (Exception e)
                    {
                        LOG.Warn("NmsTemplate Send Fail: " + e.Message);
                        LOG.Info("NmsTemplate Send retry... ");
                    }
                    Thread.Sleep(delayTime);
                }

            }
            LOG.Info("MessageProducer thread: terminating gracefully.");
        }
        public void RequestStop()
        {
            shouldStop = true;
        }
    }
}
