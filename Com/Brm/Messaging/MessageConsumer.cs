using Apache.NMS;
using Common.Logging;
using System;

namespace Com.Brm.Messaging
{


    public class MessageConsumer
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(MessageConsumer));

        private int messageCount = 1;


        void ReceiveMessage(String message)
        {
            LOG.Info(String.Format("MessageConsumer: <{0:0000}] {1}", messageCount++, message));
        }

    }
}
