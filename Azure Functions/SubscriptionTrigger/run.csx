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

// listens to a queue to determine who wants to subscribe to what
// adds subscriptions to table storage

#r "Microsoft.WindowsAzure.Storage"
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System;

public async static Task Run(SubscriptionMessage myQueueItem, CloudTable subscriptionTable, TraceWriter log)
{
    // create a new table item based on the message details in the subscription request
    // when considering multitenant it would be worth rethinking how the table is partitioned
    SubscriptionTableItem myItem = new SubscriptionTableItem
    {
        PartitionKey = "key",
        RowKey = Guid.NewGuid().ToString(),
        Time = DateTime.Now.ToString("hh.mm.ss.ffffff"),
        ConversationReference = JsonConvert.SerializeObject(myQueueItem.ConversationReference),
        SubscriptionId = myQueueItem.SubscriptionId   
    };

    // add the tableentity to the subscriptiontable
    var operation = TableOperation.Insert(myItem);
    await subscriptionTable.ExecuteAsync(operation);

    // logging purposes only
    log.Info($"C# Queue trigger function processed: {myQueueItem}");
}

public class SubscriptionTableItem : TableEntity
{
    public string Time { get; set; }
    public string ConversationReference { get; set; }
    public string SubscriptionId { get; set; }
}

public class SubscriptionMessage
{
    public ConversationReference ConversationReference { get; set; }
    public string SubscriptionId { get; set; }
}
