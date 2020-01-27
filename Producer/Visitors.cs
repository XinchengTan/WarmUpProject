using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Producer
{

    public interface IVisitor<THost, TResult>
    {
        TResult CaseAt(string id, THost host);
    }

    public abstract class AVisitor<THost, TResult>
    {
        private readonly Dictionary<string, Func<THost, TResult>> funcs = new Dictionary<string, Func<THost, TResult>>();

        public TResult CaseAt(string id, THost host)
        {
            return funcs[id].Invoke(host);
        }

        public void AddCase(string id, Func<THost, TResult> func)
        {
            funcs.Add(id, func);
        }
    }

    public class FieldDataGeneratorFactory : AVisitor<FieldAttributes, IFieldDataGenerator>
    {
        public FieldDataGeneratorFactory()
        {
            this.AddCase(Type.Double, field => new DoubleDataGenerator(field.name, field.param.mean, field.param.standard_deviation));
            this.AddCase(Type.Integer, field => new IntegerDataGenerator(field.name, field.param.mean, field.param.standard_deviation));
            this.AddCase(Type.String, field => new StringDataGenerator(field.name, field.param.max_len));
        }
    }


    public class ConfigToFieldsTranslator : AVisitor<JObject, FieldAttributes>
    {

        public ConfigToFieldsTranslator()
        {
            this.AddCase(Type.Double, jObject => {
                try
                {
                    string name = (string)jObject["name"];
                    double mean = (double)jObject["distribution_params"]["mean"];
                    double std = (double)jObject["distribution_params"]["std"];
                    FieldParam param = new FieldParam
                    {
                        mean = mean,
                        standard_deviation = std
                    };
                    return new FieldAttributes(name, "double", param);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Type casting error in double distribution_params");
                    return new FieldAttributes();
                }
            });

            this.AddCase(Type.Integer, jObject => {
                try {
                    string name = (string)jObject["name"];
                    double mean = (double)jObject["distribution_params"]["mean"];
                    double std = (double)jObject["distribution_params"]["std"];
                    FieldParam param = new FieldParam
                    {
                        mean = mean,
                        standard_deviation = std
                    };
                    return new FieldAttributes(name, "int", param);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Type casting error in integer distribution_params");
                    return new FieldAttributes();
                }
            });

            this.AddCase(Type.String, jObject => {
                try {
                    string name = (string)jObject["name"];
                    int maxlen = (int)jObject["distribution_params"]["max_len"];
                    FieldParam param = new FieldParam
                    {
                        max_len = maxlen
                    };
                    return new FieldAttributes(name, "string", param);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Type casting error in string distribution_params");
                    return new FieldAttributes();
                }
            });
        }


    }
}
