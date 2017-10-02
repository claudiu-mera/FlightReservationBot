using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightReservationBot.Helpers;
using FlightReservationBot.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;

namespace FlightReservationBot.Dialogs
{
    [LuisModel("b51f9208-0348-4731-8d0d-eb4f70c12162", "bcac5e3e55b4456c933432d89938e388")]
    [Serializable]
    public class LUISDialog : LuisDialog<FlightReservation>
    {
        private readonly BuildFormDelegate<FlightReservation> ReserveFlight;

        [field: NonSerialized()]
        protected Activity _message;

        public LUISDialog(BuildFormDelegate<FlightReservation> reserveFlight)
        {
            this.ReserveFlight = reserveFlight;
        }

        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("I'm sorry. I don't understand what you are meaning.");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Greeting")]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {
            context.Call(new GreetingDialog(), Callback);
        }

        [LuisIntent("BookFlight")]
        public async Task FlightReservation(IDialogContext context, LuisResult result)
        {
            var enrollmentForm = new FormDialog<FlightReservation>(new FlightReservation(), this.ReserveFlight, FormOptions.PromptInStart);
            context.Call(enrollmentForm, Callback);
        }

        [LuisIntent("QueryExtraServices")]
        public async Task QueryExtraServices(IDialogContext context, LuisResult result)
        {
            foreach (var entity in result.Entities.Where(e => e.Type == "ExtraService"))
            {
                var entityValue = entity.Entity.Replace(" ", string.Empty).ToLower();

                if (entityValue == "largecabinbag" || entityValue == "priorityboarding" || entityValue == "extralegroom" || entityValue == "sportsequipment")
                {
                    await context.PostAsync("We have that.");
                    context.Wait(MessageReceived);
                    return;
                }
                else
                {
                    await context.PostAsync("I'm sorry. We don't have that.");
                    context.Wait(MessageReceived);
                    return;
                }
            }

            await context.PostAsync("I'm sorry. We don't have that.");
            context.Wait(MessageReceived);
            return;
        }

        [LuisIntent("QueryRoutes")]
        public async Task QueryRoutes(IDialogContext context, LuisResult result)
        {
            foreach (var entity in result.Entities.Where(e => e.Type == "Place"))
            {
                var entityValue = entity.Entity.ToLower();

                if (entityValue == "cluj" || entityValue == "madrid" || entityValue == "paris" || entityValue == "london")
                {
                    await CreateHeroCardReply(context, _message, entityValue.Capitalize());
                    context.Wait(MessageReceived);
                    return;
                }
                else
                {
                    await context.PostAsync("I'm sorry. We don't provide routes to that place.");
                    context.Wait(MessageReceived);
                    return;
                }
            }

            await context.PostAsync("I'm sorry. We don't provide routes to that place.");
            context.Wait(MessageReceived);
            return;
        }

        private async Task CreateHeroCardReply(IDialogContext context, Activity message, string place)
        {
            Activity replyToConversation = message.CreateReply($"Yes, we provide routes to/from {place}.");
            replyToConversation.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            replyToConversation.Attachments = new List<Attachment>();

            var selectedPlace = Models.Place.AvailablePlaces.FirstOrDefault(p => p.Name.Equals(place, StringComparison.OrdinalIgnoreCase));


            List<CardImage> cardImages = new List<CardImage>();
            cardImages.Add(new CardImage(url: selectedPlace.ImageUrl));
            List<CardAction> cardButtons = new List<CardAction>();

            CardAction plButton = new CardAction()
            {
                Value = selectedPlace.WikiUrl,
                Type = "openUrl",
                Title = "WikiPedia Page"
            };

            cardButtons.Add(plButton);

            HeroCard plCard = new HeroCard()
            {
                Title = $"More info about {place}",
                Subtitle = $"{place} Wikipedia Page",
                Images = cardImages,
                Buttons = cardButtons
            };
            Attachment plAttachment = plCard.ToAttachment();
            replyToConversation.Attachments.Add(plAttachment);
            
            await context.PostAsync(replyToConversation);
        }

        protected override async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            _message = (Activity)await item;
            await base.MessageReceived(context, item);
        }

        private async Task Callback(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(MessageReceived);
        }
    }
}