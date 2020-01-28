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

        public static FullConfig? ParseConfig(string? filePath)
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
            ConfigToFieldsTranslator parser = new ConfigToFieldsTranslator();
            FullConfig? cfg = Translate(jConfig);

            return cfg;
        }

        // Takes in the whole config JSON file as a JObject and returns a list of Fields
        private static FullConfig? Translate(JObject config)
        {
            ConfigToFieldsTranslator translator = new ConfigToFieldsTranslator();
            List<FieldAttributes> fields = new List<FieldAttributes>();
            foreach (JObject fieldConfig in (JArray)config["dimension_attributes"])
            {
                string typeID = (string)fieldConfig["type"];
                fields.Add(translator.CaseAt(typeID, fieldConfig));
            }


            // TESTING:
            try
            {
                int threads_count = (int)config["threads_count"];
                int records_count = (int)config["records_count"] < 2147483647 ? (int)config["records_count"] : 2147483647;
                double error_rate = (double)config["error_rate"];

                FullConfig fullConfig = new FullConfig(
                    threads_count,
                    records_count,
                    error_rate,
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


        public static Nullable<T> GetValueOrNull<T>(Object obj) where T : struct
        {
            try
            {
                return (T)obj;
            }
            catch
            {
                //Console.WriteLine("Type casting error in Config file.");
                return null;
            }
        }
    }
}
