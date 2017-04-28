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