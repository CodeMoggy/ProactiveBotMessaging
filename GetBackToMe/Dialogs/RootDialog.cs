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

using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace GetBackToMe.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>, ILastDialogMessageSentToUser
    {
        private string _dialogName = @"[RootDialog] ";

        public string LastDialogMessageSentToUser { get; set; }

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;

            if (message.Text.Equals("help", StringComparison.CurrentCultureIgnoreCase))
            {
                var reply = $@"{_dialogName}Howdy, my name is Wyatt, Wyatt ERP! :)


I can help you discover when orders are despatched, and let you perform actions such as viewing the order or allowing you to tell your customer when the order is expected to arrive. 

To continue and to be notified when an order is desptched, please specify the order number in the foramt #XXXX.

If you don't enter an order number in this format, I'll assume you just want to chat!";

                await context.PostAsync(reply);

                context.Wait(MessageReceivedAsync);
            }
            else
            {
                if (message.Text.Contains("#") == false)
                {
                    LastDialogMessageSentToUser = "Tell me your nickname.";

                    await context.PostAsync($@"{_dialogName}No order number? Let's chat instead...tell me your nickname.");

                    context.Wait(this.MessageReceivedAsyncStartConversationAsync);
                }
                else
                {
                    var orderNumber = message.Text.Split('#')[1].Split(' ')[0];

                    var conversationReference = new ConversationReference(message.Id, message.From, message.Recipient, message.Conversation, message.ChannelId, message.ServiceUrl);

                    var queueMessage = new QueueMessage
                    {
                        ConversationReference = conversationReference,
                        SubscriptionId = orderNumber
                    };

                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

                    // Create the queue client.
                    CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

                    // Retrieve a reference to a container.
                    CloudQueue queue = queueClient.GetQueueReference("subscription-items");

                    // Create the queue if it doesn't already exist
                    queue.CreateIfNotExists();

                    CloudQueueMessage messageNew = new CloudQueueMessage(JsonConvert.SerializeObject(queueMessage));
                    queue.AddMessage(messageNew);

                    LastDialogMessageSentToUser = string.Empty;

                    await context.PostAsync($@"{_dialogName}When the order {orderNumber} has been despatched you will be notified here.");

                    context.Wait(MessageReceivedAsync);
                }

            }

        }

        public async Task MessageReceivedAsyncStartConversationAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var reply = $@"{_dialogName}Hello {(await argument).Text}...how old are you?";

            await context.PostAsync(reply);
            LastDialogMessageSentToUser = reply;
            context.Wait(this.MessageReceivedAgeConfirmedAsync);
        }

        public async Task MessageReceivedAgeConfirmedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var age = (await argument).Text;
            var reply = $@"{_dialogName} {age}! I don't believe you. ";

            // dialog finished
            LastDialogMessageSentToUser = string.Empty;

            await context.PostAsync(reply);

            context.Wait(MessageReceivedAsync);
        }

    }

    [Serializable]
    public class QueueMessage
    {
        public ConversationReference ConversationReference;
        public string SubscriptionId;
    }
}