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
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using GetBackToMe.Dialogs.DesptchActions.ViewOrder;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;

namespace GetBackToMe.Dialogs.DesptchActions
{
    [Serializable]
    public class ScorableActionsDialog : IDialog<object>
    {
        private string _dialogName = @"[ScorableActionsDialog] ";

        // Entry point to the Dialog
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync($@"{_dialogName}Which action? 'View Order' or 'Notify Customer'?");

            context.Wait(MessageReceivedOperationChoice);
        }

        public async Task MessageReceivedOperationChoice(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;

            if (message.Text.ToLower().Contains("order"))
            {
                // State transition - add 'view order' Dialog to the stack, when done call AfterChildDialogIsDone callback
                context.Call<object>(new ViewOrderDialog(), AfterChildDialogIsDone);
            }
            else if (message.Text.ToLower().Contains("customer"))
            {
                // State transition - add 'notify customer' Dialog to the stack, when done call AfterChildDialogIsDone callback
                context.Call<object>(new NotifyCustomerDialog(), AfterChildDialogIsDone);
            }
            else
            {
                await context.PostAsync($@"{_dialogName} Not a recognised action. Which action, 'View Order' or 'Notify Customer'?");

                // State transition - wait for 'operation choice' message from user (loop back)
                context.Wait(MessageReceivedOperationChoice);
            }
        }

        private async Task AfterChildDialogIsDone(IDialogContext context, IAwaitable<object> result)
        {
            ILastDialogMessageSentToUser lastDialogMessageSentToUser = null;

            foreach (var frame in context.Frames)
            {
                if (frame.Target.GetType() != this.GetType() && frame.Target is ILastDialogMessageSentToUser)
                {
                    lastDialogMessageSentToUser = frame.Target as ILastDialogMessageSentToUser;
                    break;
                }
            }

            if (lastDialogMessageSentToUser != null)
            {
                if (!string.IsNullOrEmpty(lastDialogMessageSentToUser.LastDialogMessageSentToUser))
                    await context.PostAsync($"** LAST MESSAGE: {lastDialogMessageSentToUser.LastDialogMessageSentToUser}");
            }

            // State transition - complete this Dialog and remove it from the stack
            context.Done<object>(new object());
        }
    }
}