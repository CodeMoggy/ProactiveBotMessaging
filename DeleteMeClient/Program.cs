using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DeleteMeClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var p = new Program();

            p.Run();

            Console.Write("Thank you, good-bye!!!");

            Thread.Sleep(3000);
        }


        void Run()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=rcwyattrawstorage;AccountKey=MF9hDBS2Tx5k4bLgnqstcZ1lfBpkGNb6NL3SrkWReuf8cnTfreA9OcXWSIObaLK/gflPOm5PVRzEZxEOtfsLwg==");

            // Create the queue client.
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a container.
            CloudQueue queue = queueClient.GetQueueReference("event-items");

            // Create the queue if it doesn't already exist
            queue.CreateIfNotExists();

            Console.Write("What order has been despatched? ");

            // this is the id that will be used to identify the order
            var subscriptionId = Console.ReadLine();

            // create a new queus message
            CloudQueueMessage messageNew = new CloudQueueMessage(subscriptionId);

            // add the message to the queue
            queue.AddMessage(messageNew);

            // by default assume another order will be despatched
            var reply = "Y";

            // keep asking until the user says N (actually anything other than 'Y' or 'y')
            while (reply.Equals("Y", StringComparison.CurrentCultureIgnoreCase))
            {
                Console.Write("Do you have another order that has been despatched, Y or N? ");

                reply = Console.ReadLine();

                if (reply.Equals("Y", StringComparison.InvariantCultureIgnoreCase))
                {
                    Console.Write("Okay, please tell me the next order that has been despatched? ");

                    subscriptionId = Console.ReadLine();

                    // create a new queus message
                    messageNew = new CloudQueueMessage(subscriptionId);

                    // add the message to the queue
                    queue.AddMessage(messageNew);
                }
            }
        }
    }
}
