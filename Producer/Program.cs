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
        // private static string LOCAL_FILEPATH = "/Users/caratan/Desktop/Spring 2020/producerConfig.json";
        private static string LOCAL_FILEPATH = "/Users/shenhongyu/Desktop/producerConfig.json";
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

                // Initialize producer
                Producer producer = new Producer(new_parsed_config.threads_count, new_parsed_config.records_count, new_parsed_config.fields);

                // Initialize adapter
                // TODO: Decide which consumer to connect with i.e. which adapter to use


                // TODO: Connect with consumer and confirm




                // Make and send Records
                try
                {
                    //producer.SendAllRecords(new ProducerToDefaultConsumerAdpt(), hostAddr);
                    producer.ThreadSendAllRecords(new_parsed_config.threads_count, new ProducerToDefaultConsumerAdpt(), hostAddr);
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
