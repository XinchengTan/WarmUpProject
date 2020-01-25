using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Producer
{
    // This was our original definition of Producer
    public class RecordMaker
    {
        private IProducerToConsumerAdpt adapter = new DefaultAdpt();
        private List<IJSONDataGenerator> generators;
        private static readonly Dictionary<string, IJSONDataGeneratorFactory> typeToFacMap = new Dictionary<string, IJSONDataGeneratorFactory>
        {
            {"double", new DoubleJSONDataGeneratorFactory() },
            {"int", new IntegerJSONDataGeneratorFactory() },
            {"string", new StringJSONDataGeneratorFactory() }
        };

        public RecordMaker(List<Field> fields, IProducerToConsumerAdpt adpt)
        {
            this.adapter = adpt;
            this.generators = this.MakeGenerators(fields);

        }

        // FOR TESTING
        public RecordMaker(List<Field> fields)
        {
            this.generators = this.MakeGenerators(fields);
        }


        private List<IJSONDataGenerator> MakeGenerators(List<Field> fields)
        {
            List<IJSONDataGenerator> generators = new List<IJSONDataGenerator>();
            fields.ForEach(field => generators.Add(this.MakeGenerator(field)));
            return generators;
        }

        private IJSONDataGenerator MakeGenerator(Field field)
        {
            return typeToFacMap[field.typeID].Make(field.param);
        }

        public JArray MakeRecord()
        {
            JArray record = new JArray();
            generators.ForEach(generator => record.Add(generator.GenerateJsonData()));
            return record;
        }

        private JToken ApplyError(JToken record)
        {
            //TODO: do something here
            return record;
        }
        public void SendRecord()
        {
            
            adapter.Send(this.ApplyError(MakeRecord()));
        }

        
    }
}
