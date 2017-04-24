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