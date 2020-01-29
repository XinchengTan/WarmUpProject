using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Producer
{
    public class Producer
    {

        public int Amount { get; private set; }

        private readonly IRecordGenerator recordGenerator;

        public Producer(int amount, List<FieldAttributes> fields, ErrorRateConfig config)
        {
            this.Amount = amount;
            this.recordGenerator = Util.ApplyError(new RecordGenerator(fields), config);
            //this.recordGenerator = new RecordGeneratorWithError(fields);

        }


        // Sends one data record
        public void SendRecord(IProducerToConsumerAdpt adpt, string receiver_addr)
        {
            JObject record = recordGenerator.GenerateRecord();
            Console.WriteLine($"Generated Data Record: {record}");
            //adpt.Send(record, receiver_addr); //TODO: Uncomment me to test connection with consumer!
            this.Amount--;
        }

        // Sends all data records required
        public void SendAllRecords(IProducerToConsumerAdpt adpt, string receiver_addr)
        {
            while (this.Amount != 0)
            {
                this.SendRecord(adpt, receiver_addr);
            }
        }
    }
}
