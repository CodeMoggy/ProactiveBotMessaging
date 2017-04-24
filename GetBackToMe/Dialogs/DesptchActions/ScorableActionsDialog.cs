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
                await context.PostAsync($@"{_dialogName}I didn't get that :( Which action? 'View Order' or 'Notify Customer'?");

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
                await context.PostAsync("** LAST MESSAGE: " + lastDialogMessageSentToUser.LastDialogMessageSentToUser);
            }

            // State transition - complete this Dialog and remove it from the stack
            context.Done<object>(new object());
        }
    }
}