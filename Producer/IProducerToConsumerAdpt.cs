using System;
using Newtonsoft.Json.Linq;

namespace Producer
{
    public interface IProducerToConsumerAdpt
    {
        void Send(JToken record);

    }

    public class DefaultAdpt : IProducerToConsumerAdpt
    {
        public void Send(JToken record)
        {
            // DO NOTHING
        }
    }
}
