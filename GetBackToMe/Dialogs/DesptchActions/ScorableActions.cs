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

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Scorables.Internals;
using Microsoft.Bot.Connector;
using System;

namespace GetBackToMe.Dialogs.DesptchActions
{
    public class ScorableActions : ScorableBase<IActivity, string, double>
    {
        private readonly IDialogStack _stack;

        public ScorableActions(IDialogStack stack)
        {
            SetField.NotNull(out _stack, nameof(stack), stack);
        }

        protected override Task DoneAsync(IActivity item, string state, CancellationToken token)
        {
            return Task.CompletedTask;
        }

        protected override double GetScore(IActivity item, string state)
        {
            return state != null && state == "actions-triggered" ? 1 : 0;
        }

        protected override bool HasScore(IActivity item, string state)
        {
            return state != null && state == "actions-triggered";
        }

        protected override Task PostAsync(IActivity item, string state, CancellationToken token)
        {
            var message = item as IMessageActivity;

            var dialog = new ScorableActionsDialog();

            var interruption = dialog.Void(_stack);

            _stack.Call(interruption, null);

            return Task.CompletedTask;
        }

        protected override async Task<string> PrepareAsync(IActivity item, CancellationToken token)
        {
            var message = item.AsMessageActivity();

            if (message == null)
                return null;

            var messageText = message.Text;

            if (message.Text.ToLower().Contains("actions"))
                return "actions-triggered";
            // this value is passed to GetScore/HasScore/PostAsync and can be anything meaningful to the scoring
            return null;
        }
    }
}