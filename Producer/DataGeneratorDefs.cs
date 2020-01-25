﻿using NumSharp;
using Newtonsoft.Json.Linq;

namespace Producer

{

    public interface IJSONDataGeneratorFactory
    {
        IJSONDataGenerator Make(Field field);
    }

    public class DoubleJSONDataGeneratorFactory : IJSONDataGeneratorFactory
    {
        public IJSONDataGenerator Make(Field field)
        {
            return new DoubleDataGenerator(field.name, field.param.mean, field.param.standard_deviation);

        }
    }

    public class IntegerJSONDataGeneratorFactory : IJSONDataGeneratorFactory
    {
        public IJSONDataGenerator Make(Field field)
        {
            return new IntegerDataGenerator(field.name, field.param.mean, field.param.standard_deviation);

        }
    }


    public class StringJSONDataGeneratorFactory : IJSONDataGeneratorFactory
    {
        public IJSONDataGenerator Make(Field field)
        {
            return new StringDataGenerator(field.name, field.param.max_len);

        }
    }


    public interface IJSONDataGenerator
    {
        JValue GenerateJsonData();
        string GetFieldName();

    }


    public interface IDataGenerator<T> : IJSONDataGenerator
    {

        T Generate();

        new JValue GenerateJsonData();
    }


    public class DoubleDataGenerator : IDataGenerator<double>
    {
        private static readonly double DEFAULT_MEAN = 0.0;
        private static readonly double DEFAULT_STANDARD_DEVIATION = 20.0;

        public DoubleDataGenerator(string name, double? mean, double? standardDeviation) : this(name, mean.GetValueOrDefault(DEFAULT_MEAN), standardDeviation.GetValueOrDefault(DEFAULT_STANDARD_DEVIATION)) { }

        public DoubleDataGenerator(string name, double mean, double standardDeviation)
        {
            this.Name = name;
            this.Mean = mean;
            this.StandardDeviation = standardDeviation;
        }

        public double Mean { get; private set; }

        public double StandardDeviation { get; private set; }

        public string Name { get; private set; }

        public double Generate()
        {
            var data = np.random.normal(this.Mean, this.StandardDeviation, 1);
            return data[0];
        }

        public JValue GenerateJsonData()
        {
            double data = this.Generate();
            return new JValue(data);
        }

        public string GetFieldName()
        {
            return Name;
        }
    }


    public class IntegerDataGenerator : IDataGenerator<int>
    {
        private static readonly double DEFAULT_MEAN = 10.0;
        private static readonly double DEFAULT_STANDARD_DEVIATION = 10.0;

        public IntegerDataGenerator(string name, double? mean, double? standardDeviation) : this(name, mean.GetValueOrDefault(DEFAULT_MEAN), standardDeviation.GetValueOrDefault(DEFAULT_STANDARD_DEVIATION)) { }

        public IntegerDataGenerator(string name, double mean, double standardDeviation)
        {
            this.Name = name; 
            this.Mean = mean;
            this.StandardDeviation = standardDeviation;
        }

        public string Name { get; private set; }

        public double Mean { get; private set; }

        public double StandardDeviation { get; private set; }

        public int Generate()
        {
            var data = np.random.normal(this.Mean, this.StandardDeviation, 1);
            return (int)data[0];
        }

        public JValue GenerateJsonData()
        {
            int data = this.Generate();
            return new JValue(data);
        }

        public string GetFieldName()
        {
            return Name;
        }
    }

    public class StringDataGenerator : IDataGenerator<string>
    {
        private static readonly int DEFAULT_MAXLEN = 10;

        public StringDataGenerator(string name, int? maxlen) : this(name, maxlen.GetValueOrDefault(DEFAULT_MAXLEN)) { }

        public StringDataGenerator(string name, int maxlen)
        {
            this.Name = name;
            this.MaxLen = maxlen;
        }
        public string Name { get; private set; }
        public double MaxLen { get; private set; }

        public string Generate()
        {
            string data = "Random data (hard-coded for now)";
            return data;
        }

        public JValue GenerateJsonData()
        {
            string data = this.Generate();
            return new JValue(data);
        }

        public string GetFieldName()
        {
            return Name;
        }
    }

    //public abstract class AErrorGenerator<T, U> : IDataGenerator<U> where T: struct
    //{
    //    private static readonly double DEFAULT_ERROR_RATE = 0.05;

    //    public double ErrorRate { get; private set; }

    //    public IDataGenerator<T> DataGenerator { get; private set; }

    //    protected AErrorGenerator(double errorRate, IDataGenerator<T> dataGenerator)
    //    {
    //        this.ErrorRate = errorRate;
    //        this.DataGenerator = dataGenerator;
    //    }

    //    protected AErrorGenerator(IDataGenerator<T> dataGenerator) : this(DEFAULT_ERROR_RATE, dataGenerator) { }

    //    public abstract U Generate();

    //    public abstract JValue GenerateJsonData();
    //}

    //public class NullErrorGenerator<T> : AErrorGenerator<T, T?> where T : struct
    //{
    //    public NullErrorGenerator(double errorRate, IDataGenerator<T> dataGenerator) : base(errorRate, dataGenerator) { }

    //    public NullErrorGenerator(IDataGenerator<T> dataGenerator) : base(dataGenerator) { }

    //    public override T? Generate()
    //    {
    //        // List<T?> data = this.DataGenerator.Generate(amount);

    //        // Object is not nullable so we have to change something in the definition of this method
    //        // It's supposed to be List<Object?> but it's not compiling I haven't figured out why yet
    //        // data[0] = null;


    //        return null;

    //    }

    //    public override JValue GenerateJsonData()
    //    {
    //        return new JValue();
    //    }
    //}

    //public abstract class AWrongTypeGenerator<T, U>: AErrorGenerator<T, U>
    //{
    //    public WrongTypeGenerator(double errorRate, IDataGenerator<T> dataGenerator): base(errorRate, dataGenerator) { }

    //    public WrongTypeGenerator(IDataGenerator<T> dataGenerator) : base(dataGenerator) { }

    //    public override List<U> Generate(int amount)
    //    {
    //        List<T> data = this.DataGenerator.Generate(amount);
    //        List<Object> finalData = new List<object>();
    //        foreach(T x in data)
    //        {
    //            finalData.Add((object)x);
    //        }
    //        finalData[0] = (object)"wrong data!";
    //        return finalData;
    //    }
    //}







}

