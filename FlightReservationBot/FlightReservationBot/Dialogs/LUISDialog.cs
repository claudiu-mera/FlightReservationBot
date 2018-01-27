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
        private const string BookFlightOption = "Book a flight";

        private const string GetServicesOption = "Get the available services";

        private static readonly List<string> availableRoutes = new List<string> { "cluj", "madrid", "paris", "london" };

        private static readonly List<string> availableOptions = new List<string> { "largecabinbag", "priorityboarding", "extralegroom", "sportsequipment" };

        private readonly BuildFormDelegate<FlightReservation> ReserveFlight;

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
            this.CreateFlightReservation(context);
        }

        [LuisIntent("QueryExtraServices")]
        public async Task QueryExtraServices(IDialogContext context, LuisResult result)
        {
            foreach (var entity in result.Entities.Where(e => e.Type == "ExtraService"))
            {
                var entityValue = entity.Entity.Replace(" ", string.Empty).ToLower();

                if (availableOptions.Exists(o => o.Equals(entityValue)))
                {
                    await context.PostAsync("Yes, we have that.");
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

                if (availableRoutes.Exists(p => p.Equals(entityValue)))
                {
                    var recommendedDestination = entityValue.Capitalize();
                    context.UserData.SetValue<string>("RecommendedDestination", recommendedDestination);

                    await CreateHeroCardReply(context, recommendedDestination);
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

        [LuisIntent("ProvideHelp")]
        public async Task GetHelp(IDialogContext context, LuisResult result)
        {
            PromptDialog.Choice(context, this.OnOptionSelected, new List<string> { BookFlightOption, GetServicesOption }, "Here is what I can do for you:", "Not a valid option", 3);
        }

        private async Task OnOptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string optionSelected = await result;

                switch (optionSelected)
                {
                    case BookFlightOption:
                        this.CreateFlightReservation(context);
                        break;

                    case GetServicesOption:
                        context.Call(new ServicesDialog(), this.Callback);
                        break;
                }
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync($"Too many attemps. But don't worry, I'm handling that exception and you can try again!");

                context.Wait(this.MessageReceived);
            }
        }

        private void CreateFlightReservation(IDialogContext context)
        {
            var reservationForm = new FormDialog<FlightReservation>(new FlightReservation(context), this.ReserveFlight, FormOptions.PromptInStart)
                .Do(async (currentContext, reservation) =>
                {
                    try
                    {
                        var completed = await reservation;

                        // Actually process the reservation...
                        await currentContext.PostAsync("You are done! Your reservation was successfully processed!");

                        var reply = currentContext.MakeMessage();
                        reply.Attachments.Add(
                            new Attachment()
                            {
                                ContentUrl = "https://blog.trabber.com/wp-content/uploads/2015/01/keep-calm-flight.png",
                                ContentType = "image/png",
                                Name = "safe_flight.png"
                            });

                        await currentContext.PostAsync(reply);
                    }
                    catch (FormCanceledException<FlightReservation> e)
                    {
                        string reply;
                        if (e.InnerException == null)
                        {
                            reply = $"You cancelled before entering {e.Last} -- maybe you can finish next time!";
                        }
                        else
                        {
                            reply = "Sorry, I've had a short circuit. Please try again.";
                        }
                        await currentContext.PostAsync(reply);
                    }
                });

            context.Call(reservationForm, Callback);
        }

        private async Task CreateHeroCardReply(IDialogContext context, string place)
        {
            var replyToConversation = context.MakeMessage();
            replyToConversation.Text = $"Yes, we provide routes to/from {place}.";
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
                Title = "Wikipedia Page"
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

        private async Task Callback(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(MessageReceived);
        }
    }
}