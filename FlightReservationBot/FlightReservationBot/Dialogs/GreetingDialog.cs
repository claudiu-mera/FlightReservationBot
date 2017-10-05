using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace FlightReservationBot.Dialogs
{
    [Serializable]
    public class GreetingDialog : IDialog
    {
        public async Task StartAsync(IDialogContext context)
        {
            var reply = context.MakeMessage();

            reply.Attachments.Add(new Attachment()
            {
                ContentUrl = "https://lh5.ggpht.com/TZV-dVGu3IQ6vUFDik_lWcIsFegOt8NlhhfJ4a7f7jtqeG-1ZQ8_xGvSxXdfPbW5mrv4=w300",
                ContentType = "image/png",
                Name = "plane.jpg"
            });

            await context.PostAsync(reply);

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
                await context.PostAsync($"Hi, {userName}. Glad to see you!");
            }
        }
    }
}