using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Dialogs.Internals;

namespace GetBackToMe.Dialogs.DesptchActions.ViewOrder
{
    [Serializable]
    public class ViewOrderDialog : IDialog<object>
    {
        private string _dialogName = @"[ViewOrderDialog] ";

        // Entry point to the Dialog
        private string _orderNumber { get; set; }

        // Entry point to the Dialog
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync($"{_dialogName}Please confirm the Order Number in the format #XXXX");

            context.Wait(MessageReceivedOrderNumberConfirmed);
        }

        public async Task MessageReceivedOrderNumberConfirmed(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var text = (await argument).Text;

            if (text.Contains('#') == false)
            {
                await context.PostAsync($"{_dialogName}Order Number is in the wrong format!");

                context.Wait(MessageReceivedOrderNumberConfirmed);
            }
            else
            {

                this._orderNumber = text.Split('#')[1].Split(' ')[0];

                await context.PostAsync($@"{_dialogName}Here are the following details for order {_orderNumber}:
* ""Order: {_orderNumber}""
* ""Customer: Contoso Flights""
* ""Line1: 10 Widgets @ £20 each""
* ""Line2: 2 Wings @ £400 each""
* ""Total: £1000"" ");

                

                // State transition - complete this Dialog and remove it from the stack
                context.Done<object>(new object());
            }
        }
    }
}