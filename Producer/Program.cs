using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Producer
{
    class MainClass
    {
        //private static string LOCAL_FILEPATH = "/Users/caratan/Desktop/Spring 2020/producerConfig.json";

        public static void Main(string[] args)
        {

            Console.WriteLine("Please enter an absolute path to config file:\n");
            // Start parsing
            string? filePath = Console.ReadLine();
            FullConfig parsed_config = ParseConfig(filePath);
            Producer producer = new Producer(parsed_config.records_count, parsed_config.fields);


            // TODO: Connect with consumer and confirm



            // Make and send Records
            Console.WriteLine("Generated Data Records: ");
            try
            {
                producer.SendAllRecords(new ProducerToDefaultConsumerAddpt());
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
