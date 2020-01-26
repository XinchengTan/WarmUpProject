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

    // This is a visitor
    public class ConfigToFieldsTranslator: AVisitor<JObject, FieldAttributes>
    {

        public ConfigToFieldsTranslator()
        {
            this.AddCase("double", jObject => {
                string name = (string) jObject["name"];
                double mean = (double) jObject["distribution_params"]["mean"];
                double std = (double) jObject["distribution_params"]["std"];
                FieldParam param = new FieldParam
                {
                    mean = mean,
                    standard_deviation = std
                };
                return new FieldAttributes(name, "double", param);
            });

            this.AddCase("int", jObject => {
                string name = (string) jObject["name"];
                double mean = (double) jObject["distribution_params"]["mean"];
                double std = (double) jObject["distribution_params"]["std"];
                FieldParam param = new FieldParam
                {
                    mean = mean,
                    standard_deviation = std
                };
                return new FieldAttributes(name, "int", param);
            });

            this.AddCase("string", jObject => {
                string name = (string) jObject["name"];
                int maxlen = (int) jObject["distribution_params"]["max_len"];
                FieldParam param = new FieldParam
                {
                    max_len = maxlen
                };
                return new FieldAttributes(name, "string", param);
            });
        }

        
    }
}
