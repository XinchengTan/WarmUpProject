using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Producer
{
    class MainClass
    {
        private static string LOCAL_FILEPATH = "/Users/caratan/Desktop/Spring 2020/producerConfig.json";

        public static void Main(string[] args)
        {

            Console.WriteLine("Please enter an absolute path to config file:\n");
            // Start parsing
            string? filePath = Console.ReadLine();
            FullConfig parsed_config = ParseConfig(filePath);
            RecordMaker recordMaker = new RecordMaker(parsed_config.fields);


            // TODO: Connect with consumer and confirm


            // Make record
            Console.WriteLine("Generated Data Records: ");
            for (int counter = parsed_config.records_count; counter > 0; counter--)
            {
                JArray record = recordMaker.MakeRecord();
                Console.WriteLine(record.ToString());
            };

            // TODO: Send records



            // Exit
            Console.WriteLine("Console exiting...");
        }


        private static FullConfig ParseConfig(string? filePath)
        {
            Console.WriteLine($"Echoing input file path: {filePath}");

            if (String.IsNullOrEmpty(filePath))
            {
                // TODO: Throw exception
                Console.WriteLine("Got empty file path! Start trying default path...");
                filePath = LOCAL_FILEPATH;
            }

            using (StreamReader stream = File.OpenText(filePath))
            using (JsonTextReader reader = new JsonTextReader(stream))
            {
                JObject jConfig = (JObject) JToken.ReadFrom(reader);
                ConfigToFieldsTranslator parser = new ConfigToFieldsTranslator();
                FullConfig cfg = parser.Translate(jConfig);

                return cfg;
            }
        }
    }
}
