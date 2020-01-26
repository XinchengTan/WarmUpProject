using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Producer
{
    // This was our original definition of Producer
    public class RecordMaker
    {
        private IProducerToConsumerAdpt adapter = new DefaultAdpt();
        private List<IFieldDataGenerator> generators;
        private static readonly Dictionary<string, IFieldDataGeneratorFactory> typeToFacMap = new Dictionary<string, IFieldDataGeneratorFactory>
        {
            {"double", new DoubleDataGeneratorFactory() },
            {"int", new IntegerDataGeneratorFactory() },
            {"string", new StringDataGeneratorFactory() }
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


        private List<IFieldDataGenerator> MakeGenerators(List<Field> fields)
        {
            List<IFieldDataGenerator> generators = new List<IFieldDataGenerator>();
            fields.ForEach(field => generators.Add(this.MakeGenerator(field)));
            return generators;
        }

        private IFieldDataGenerator MakeGenerator(Field field)
        {
            return typeToFacMap[field.typeID].Make(field);
        }

        public JObject MakeRecord()
        {
            //JArray record = new JArray();
            //generators.ForEach(generator => record.Add(generator.GenerateJsonData()));

            JObject record = new JObject();
            generators.ForEach(generator => record[generator.GetFieldName()] = generator.GenerateFieldData());
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
