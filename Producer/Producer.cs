using System;
using System.Collections.Generic;
using System.Linq;

namespace Producer
{
    public class Producer
    {

        public int Amount { get; private set; }

        private RecordGenerator recordGenerator;

        public Producer(int amount, List<FieldAttributes> fields)
        {
            this.Amount = amount;
            this.recordGenerator = new RecordGenerator(this.MakeFieldDataGenerators(fields));

        }


        private static readonly Dictionary<string, IFieldDataGeneratorFactory> typeToFacMap = new Dictionary<string, IFieldDataGeneratorFactory>
        {
            {"double", new DoubleDataGeneratorFactory() },
            {"int", new IntegerDataGeneratorFactory() },
            {"string", new StringDataGeneratorFactory() }
        };


        private List<IFieldDataGenerator> MakeFieldDataGenerators(List<FieldAttributes> fields)
        {
            List<IFieldDataGenerator> generators = new List<IFieldDataGenerator>();
            fields.ForEach(field => generators.Add(typeToFacMap[field.typeID].Make(field)));
            return generators;
        }

        // Sends one data record
        public void SendRecord(IProducerToConsumerAdpt adpt)
        {
            adpt.Send(recordGenerator.MakeRecord());
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
