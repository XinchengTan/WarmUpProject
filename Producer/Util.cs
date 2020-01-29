using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Producer
{
    class Util {
        private static readonly string LOCAL_FILEPATH = "/Users/shenhongyu/Desktop/producerConfig.json";

        private static readonly ConfigToFieldsTranslator configToFieldsTranslator = new ConfigToFieldsTranslator();
        private static readonly FieldDataGeneratorFactory generatorFactory = new FieldDataGeneratorFactory();



        private static JObject LoadConfig(string? filePath)
        {
            Console.WriteLine($"Echoing input file path: {filePath}");

            if (String.IsNullOrEmpty(filePath))
            {
                // TODO: Throw exception
                Console.WriteLine("Got empty file path! Start trying default path...");
                filePath = LOCAL_FILEPATH;
            }

            using StreamReader stream = File.OpenText(filePath);
            using JsonTextReader reader = new JsonTextReader(stream);
            JObject jConfig = (JObject)JToken.ReadFrom(reader);
            return jConfig;
        }

        public static FullConfig? ParseConfig(string? filePath)
        {
            JObject jConfig = LoadConfig(filePath);

            List<FieldAttributes> fields = new List<FieldAttributes>();
            foreach (JObject fieldConfig in (JArray)jConfig["dimension_attributes"])
            {
                string typeID = (string)fieldConfig["type"];
                fields.Add(configToFieldsTranslator.CaseAt(typeID, fieldConfig));
            }


            // TESTING:
            try
            {
                int threads_count = (int)jConfig["threads_count"];
                int records_count = (int)jConfig["records_count"] < 2147483647 ? (int)jConfig["records_count"] : 2147483647;

                // Console.WriteLine((double)jConfig["error_rate"]["missing_field"]);

                ErrorRateConfig errorRate = new ErrorRateConfig
                {
                    missingField = (double)jConfig["error_rate"]["missing_field"],
                    badValue = (double)jConfig["error_rate"]["bad_value"],
                    additionalField = (double)jConfig["error_rate"]["additional_field"]

                };
                Console.WriteLine(errorRate.missingField);

                FullConfig fullConfig = new FullConfig(
                    threads_count,
                    records_count,
                    errorRate,
                    fields
                );
                return fullConfig;
            }
            catch (Exception e)
            {
                Console.WriteLine("Type casting error in thread or record or error_rate.");
                return null;
            }

        }


        public static IFieldDataGenerator MakeFieldDataGenerator(FieldAttributes f)
        {
            return generatorFactory.CaseAt(f.typeID, f);

        }


        //public static T? GetValueOrNull<T>(Object obj) where T : struct
        //{
        //    Console.WriteLine(obj);
        //    Console.WriteLine(typeof(T));

        //    //try
        //    //{
        //    //    return (T) obj;
        //    //}
        //    //catch(Exception e)
        //    //{
                
        //    //    Console.WriteLine(e.StackTrace);
        //    //    return null;
        //    //}

        //    return (T)obj;
        //}

        public static IRecordGenerator ApplyError(IRecordGenerator gen, ErrorRateConfig config)
        {
            IRecordGenerator gen1 = ApplyErrorHelper(config.badValue, "Bad value", gen);
            IRecordGenerator gen2 = ApplyErrorHelper(config.missingField, "Missing field", gen1);
            IRecordGenerator gen3 = ApplyErrorHelper(config.additionalField, "Additional field", gen2);
            
            return gen3;
        }

        private static IRecordGenerator ApplyErrorHelper(double? unprocessedRate, string errorType, IRecordGenerator gen)
        {

            double rate = unprocessedRate.GetValueOrDefault(0.0);
            if (rate == 0.0)
            {
                return gen;
            }
            switch(errorType)
            {
                case "Bad value":
                    return new WrongTypeErrorGenerator(gen, rate);
                case "Additional field":
                    return new AdditionalFieldErrorGenerator(gen, rate);
                case "Missing field":
                    return new MissingFieldErrorGenerator(gen, rate);
                default:
                    return gen;
            }

        }
    }
}
