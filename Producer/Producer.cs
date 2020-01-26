using System;
using System.Collections.Generic;
using System.Linq;

namespace Producer
{
    public class Producer
    {

        public int Amount { get; private set; }

        private readonly IRecordGenerator recordGenerator;

        public Producer(int amount, List<FieldAttributes> fields)
        {
            this.Amount = amount;
            this.recordGenerator = new RecordGeneratorWithError(fields);

        }

        // Sends one data record
        public void SendRecord(IProducerToConsumerAdpt adpt)
        {
            adpt.Send(recordGenerator.GenerateRecord());
            this.Amount --;
        }

        // Sends all data records required
        public void SendAllRecords(IProducerToConsumerAdpt adpt)
        {
            while(this.Amount != 0)
            {
                this.SendRecord(adpt);
            }
        }
    }
}
