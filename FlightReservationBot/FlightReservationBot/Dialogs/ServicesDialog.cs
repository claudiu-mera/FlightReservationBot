﻿using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;

namespace FlightReservationBot.Dialogs
{
    [Serializable]
    public class ServicesDialog : IDialog
    {
        public async Task StartAsync(IDialogContext context)
        {
            await CreateHeroReply(context);
        }

        private async Task CreateHeroReply(IDialogContext context)
        {
            var replyToConversation = context.MakeMessage();

            replyToConversation.AttachmentLayout =  AttachmentLayoutTypes.Carousel;
            replyToConversation.Attachments = new List<Attachment>();

            Dictionary<string, string> cardContentList = new Dictionary<string, string>();
            cardContentList.Add("Large Cabin Bag", "https://wizzair.com/static/images/default-source/information-services-images/card-images/large-cabin-bag-new-v2_402cf88c.png");
            cardContentList.Add("Priority Boarding", "https://f9prodcdn.azureedge.net/media/3281/airport_countdown_clock_icon.png");
            cardContentList.Add("Extra Legroom", "https://img.static-af.com/images/media/347F169E-DD8E-4851-A229E0331DFDC3F6/source/seat-plus-300x300/?aspect_ratio=1:1");
            cardContentList.Add("Sports Equipment", "http://investdailynews.com/wp-content/uploads/2017/02/Global-Sports-Equipment-Market.jpg");

            foreach (KeyValuePair<string, string> cardContent in cardContentList)
            {
                List<CardImage> cardImages = new List<CardImage>();
                cardImages.Add(new CardImage(url: cardContent.Value));

                List<CardAction> cardButtons = new List<CardAction>();

                CardAction plButton = new CardAction()
                {
                    Value = $"https://en.wikipedia.org/wiki/{cardContent.Key}",
                    Type = "openUrl",
                    Title = "More info"
                };

                cardButtons.Add(plButton);

                HeroCard plCard = new HeroCard()
                {
                    Title = $"{cardContent.Key}",
                    Subtitle = $"{cardContent.Key} Wikipedia Page",
                    Images = cardImages,
                    Buttons = cardButtons
                };

                Attachment plAttachment = plCard.ToAttachment();
                replyToConversation.Attachments.Add(plAttachment);
            }

            await context.PostAsync(replyToConversation);

            context.Done(replyToConversation);
        }
    }
}