using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Producer
{
    // This is a visitor
    public class ConfigToFieldsTranslator
    {

        private readonly Dictionary<string, Func<JObject, FieldAttributes>> translator = new Dictionary<string, Func<JObject, FieldAttributes>>();
        private void AddCase(string typeID, Func<JObject, FieldAttributes> translatorCase)
        {
            translator.Add(typeID, translatorCase);
        }

        public FieldAttributes CaseAt(string typeID, JObject value)
        {
            return translator[typeID].Invoke(value);
        }

        public ConfigToFieldsTranslator()
        {
            this.AddCase("double", jObject => {
                string name = (string)jObject["name"];
                double mean = (double)jObject["distribution_params"]["mean"];
                double std = (double)jObject["distribution_params"]["std"];
                FieldParam param = new FieldParam
                {
                    mean = mean,
                    standard_deviation = std
                };
                return new FieldAttributes(name, "double", param);
            });

            this.AddCase("int", jObject => {
                string name = (string)jObject["name"];
                double mean = (double)jObject["distribution_params"]["mean"];
                double std = (double)jObject["distribution_params"]["std"];
                FieldParam param = new FieldParam
                {
                    mean = mean,
                    standard_deviation = std
                };
                return new FieldAttributes(name, "int", param);
            });

            this.AddCase("string", jObject => {
                string name = (string)jObject["name"];
                int maxlen = (int)jObject["distribution_params"]["max_len"];
                FieldParam param = new FieldParam
                {
                    max_len = maxlen
                };
                return new FieldAttributes(name, "string", param);
            });
        }

        
    }
}
