using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlightReservationBot.Models
{
    public class Place
    {
        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public string WikiUrl { get; set; }

        public static readonly List<Place> AvailablePlaces = new List<Place>
        {
            new Place
            {
                Name = "cluj",
                ImageUrl = "http://www.albatross-travel.ro/wp-content/uploads/2017/02/Day-2-Cuj-Napoca-Turda-Alba-Iulia-Sibiu-2-300x300.gif",
                WikiUrl = "https://en.wikipedia.org/wiki/Cluj-Napoca"
            },
            new Place
            {
                Name = "madrid",
                ImageUrl = "https://i0.wp.com/www.casino-review.co/wp-content/uploads/2017/04/Casino-Review-Live-Resort-Madrid.jpg?resize=300%2C300&ssl=1",
                WikiUrl = "https://en.wikipedia.org/wiki/Madrid"
            },
            new Place
            {
                Name = "paris",
                ImageUrl = "http://images.globusfamily.com/photos/WPP-T4.jpg",
                WikiUrl = "https://en.wikipedia.org/wiki/Paris"},
            new Place
            {
                Name = "london",
                ImageUrl = "http://www.speakuplondon.com/wp-content/uploads/2015/12/73295-640x360-london-skyline-ns-300x300.jpg",
                WikiUrl = "https://en.wikipedia.org/wiki/London"
            },
        };
    }
}