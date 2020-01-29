using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Diagnostics;

namespace Producer
{
    public class Producer
    {

        public int records_per_thread { get; private set; }
        public int records_last_thread { get; private set; }
        public IProducerToConsumerAdpt adpt { get; private set; }

        private readonly IRecordGenerator recordGenerator;

        public Producer(int threads, int amount, List<FieldAttributes> fields)
        {
            this.records_per_thread = amount / threads;
            this.records_last_thread = amount % threads;
            Console.WriteLine("records_per_thread: " + this.records_per_thread);
            Console.WriteLine("records_last_thread: " + this.records_per_thread);
            this.recordGenerator = new RecordGeneratorWithError(fields);

        }


        // Sends one data record
        public void SendRecord(IProducerToConsumerAdpt adpt, string receiver_addr)
        {
            JObject record = recordGenerator.GenerateRecord();
            Console.WriteLine($"Generated Data Record: {record}");
            //adpt.Send(record, receiver_addr); //TODO: Uncomment me to test connection with consumer!
            this.records_per_thread--;
        }

        // Sends all data records required
        public void SendAllRecords(IProducerToConsumerAdpt adpt, string receiver_addr)
        {
            Stopwatch sw = Stopwatch.StartNew();
            while (this.records_per_thread != 0)
            {
                this.SendRecord(adpt, receiver_addr);
            }
            sw.Stop();
            Console.WriteLine("Time taken: {0}ms", sw.Elapsed.TotalMilliseconds);
        }


        // Sending data records using Thread:
        public void ThreadSendRecord(object args)
        {

            int amount = (int)((Array)args).GetValue(0);
            string receiver_addr = (string)((Array)args).GetValue(1);

            while ((int)amount != 0)
            {
                JObject record = recordGenerator.GenerateRecord();
                Console.WriteLine($"Generated Data Record: {record}");
                //this.adpt.Send(record, receiver_addr); //TODO: Uncomment me to test connection with consumer!
                amount--;
            }
        }


        public void ThreadSendAllRecords(int threads, IProducerToConsumerAdpt adpt, string receiver_addr)
        {
            // Set the adapter for ThreadSendRecord to call.
            this.adpt = adpt;

            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < threads; i++) {
                Console.WriteLine("Thread " + i);
                if (i == threads - 1)
                {
                    Thread thr = new Thread(new ParameterizedThreadStart(ThreadSendRecord));
                    thr.Start(new object[2] { this.records_last_thread + this.records_per_thread, receiver_addr });
                }
                else {
                    Thread thr = new Thread(new ParameterizedThreadStart(ThreadSendRecord));
                    thr.Start(new object[2] { this.records_per_thread, receiver_addr });
                }
            }
            sw.Stop();
            Console.WriteLine("Time taken: {0}ms", sw.Elapsed.TotalMilliseconds);
        }
    }
}
