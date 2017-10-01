using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;

namespace FlightReservationBot.Dialogs
{
    [Serializable]
    public class GreetingDialog : IDialog
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Hi, I'm your Flight Booking assistant.");
            await Respond(context);

            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            var userName = string.Empty;
            var getName = false;

            context.UserData.TryGetValue<string>("Name", out userName);
            context.UserData.TryGetValue<bool>("GetName", out getName);

            if (getName)
            {
                userName = message.Text;

                context.UserData.SetValue<string>("Name", userName);
                context.UserData.SetValue<bool>("GetName", false);
            }

            await Respond(context);
            context.Done(message);
        }

        private static async Task Respond(IDialogContext context)
        {
            var userName = string.Empty;
            context.UserData.TryGetValue<string>("Name", out userName);

            if (string.IsNullOrEmpty(userName))
            {
                await context.PostAsync("What is your name?");
                context.UserData.SetValue<bool>("GetName", true);
            }
            else
            {
                await context.PostAsync($"Hi, {userName}. How can I help you with?");
            }
        }
    }
}