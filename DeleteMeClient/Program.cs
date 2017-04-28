//MIT License

//Copyright(c) 2017 Richard Custance

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Configuration;
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
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);

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
