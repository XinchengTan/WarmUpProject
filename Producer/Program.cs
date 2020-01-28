using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Producer
{
    class Controller
    {
        private static string LOCAL_FILEPATH = "/Users/caratan/Desktop/Spring 2020/producerConfig.json";
        private static string RECEIVER_ADDR = "http://localhost:7071/api/ProducerAzure";

        public static void Main(string[] args)
        {

            Console.WriteLine("Please enter an absolute path to config file:\n");
            // Start parsing
            string? filePath = Console.ReadLine() ?? LOCAL_FILEPATH;
            string hostAddr = RECEIVER_ADDR;           
            FullConfig? parsed_config = Util.ParseConfig(filePath);

            if (parsed_config.Equals(null))
            {
                // Type casting error
            }
            else {
                FullConfig new_parsed_config = ((FullConfig)parsed_config);
                //foreach (FieldAttributes field in new_parsed_config.fields) {
                //    if (field.Equals(default(FieldAttributes))) {
                //        // Type casting error in dimension_attributes
                //        Console.WriteLine("Testing....");
                //        return;
                //    }
                //}

                Producer producer = new Producer(new_parsed_config.records_count, new_parsed_config.fields);
                
                // TODO: Connect with consumer and confirm


                // Make and send Records
                Console.WriteLine("Generated Data Records: ");
                Console.WriteLine(producer.MakeRecord().ToString());
                try
                {
                    producer.SendAllRecords(new ProducerToDefaultConsumerAddpt(), hostAddr);
                    // Console.WriteLine(record.ToString());
                }
                catch (WebException webExcp)
                {

                }
                // Exit
                Console.WriteLine("Console exiting...");
            }
        }
    }
}
