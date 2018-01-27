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
                ContentUrl = "https://flightbot.blob.core.windows.net/container/Robot-Icon.png",
                ContentType = "image/png",
                Name = "robot_icon.png"
            });

            await context.PostAsync(reply);

            await context.PostAsync("Hi, I'm your Flight Booking assistant. Glad to see you!");

            context.Done(reply);
        }
    }
}