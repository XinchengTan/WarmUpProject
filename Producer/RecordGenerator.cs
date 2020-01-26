using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Producer
{
    public interface IRecordGenerator
    {
        JObject GenerateRecord();
    }

    public class RecordGeneratorWithError: IRecordGenerator
    {
        private readonly IRecordGenerator gen;

        public RecordGeneratorWithError(List<IFieldDataGenerator> fieldDataGens)
        {
            this.gen = new AdditionalFieldErrorGenerator(new MissingFieldErrorGenerator(new WrongTypeErrorGenerator(new RecordGenerator(fieldDataGens))));
        }

        public JObject GenerateRecord()
        {
            return gen.GenerateRecord();
        }
    }


    public class RecordGenerator: IRecordGenerator
    {
        private readonly List<IFieldDataGenerator> fieldDataGens;

        public RecordGenerator(List<IFieldDataGenerator> fieldDataGens)
        {
            this.fieldDataGens = fieldDataGens;
        }


        public JObject GenerateRecord()
        {
            JObject record = new JObject();
            fieldDataGens.ForEach(generator => record[generator.GetFieldName()] = generator.GenerateFieldData());
            return record;
        }
        
    }

    public abstract class AErrorGenerator : IRecordGenerator
    {
        private static readonly double DEFULT_ERROR_RATE = 0.1;

        public IRecordGenerator Generator;
        public double ErrorRate { get; private set; }

        private readonly Random random = new Random();

        public AErrorGenerator(IRecordGenerator generator, double errorRate)
        {
            this.Generator = generator;
            this.ErrorRate = errorRate;
        }

        public AErrorGenerator(IRecordGenerator generator): this(generator, DEFULT_ERROR_RATE) { }

        public JObject GenerateRecord()
        {
            if (random.NextDouble() < this.ErrorRate)
            {
                return ApplyError(Generator.GenerateRecord());
            } else
            {
                return Generator.GenerateRecord();
            }
        }

        protected abstract JObject ApplyError(JObject record);
    }

    public class ErrorGenerator : AErrorGenerator
    {
        public ErrorGenerator(IRecordGenerator gen) :
            base(new AdditionalFieldErrorGenerator(new MissingFieldErrorGenerator(new WrongTypeErrorGenerator(gen)))) { }

        protected override JObject ApplyError(JObject record)
        {
            return record;
        }
    }

    public class WrongTypeErrorGenerator: AErrorGenerator
    {
        public WrongTypeErrorGenerator(IRecordGenerator gen): base(gen) { }

        protected override JObject ApplyError(JObject record)
        {
            JProperty property = record.Properties().GetEnumerator().Current;
            string name = property.Name;
            JTokenType valueType = property.Value.Type;
            if (valueType == JTokenType.String)
            {
                record.Add(name, new JValue(0));
            } else
            {
                record.Add(name, new JValue("wrong type data"));
            }
            return record;

        }
    }

    public class AdditionalFieldErrorGenerator: AErrorGenerator
    {
        public AdditionalFieldErrorGenerator(IRecordGenerator gen) : base(gen) { }

        protected override JObject ApplyError(JObject record)
        {
            record.Add("error", "This is an additional field");
            return record;
        }
    }

    public class MissingFieldErrorGenerator : AErrorGenerator
    {

        public MissingFieldErrorGenerator(IRecordGenerator gen) : base(gen) { }

        protected override JObject ApplyError(JObject record)
        {
            JProperty property = record.Properties().GetEnumerator().Current;
            record.Remove(property.Name);
            return record;
        }
    }
}
