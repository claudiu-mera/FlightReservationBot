using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlightReservationBot.Models
{
    public enum CheckInOptions
    {
        Online = 1,
        Airport = 2
    }

    public enum ExtraServiceOptions
    {
        LargeCabinBag = 1,
        PriorityBoarding = 2,
        ExtraLegroom = 3,
        SportsEquipment = 4
    }

    [Serializable]
    public class FlightReservation
    {
        [Prompt("Please enter the Origin where you are flying from:")]
        public string Origin;

        [Prompt("Please enter the Destination where you are flying to:")]
        public string Destination;

        [Prompt("Please enter a Departure date:")]
        public DateTime DepartureDate;

        [Prompt("Please enter an Arrival date:")]
        public DateTime? ArrivalDate;

        public string PassengerName;

        public CheckInOptions? CheckIn;

        [Prompt("What kind of Extra Options would you like? {||}")]
        public List<ExtraServiceOptions> ExtraService;

        public static IForm<FlightReservation> BuildForm()
        {
            return new FormBuilder<FlightReservation>()
                .Message("Welcome to Flight Booking assistant!")
                .Build();
        }
    }
}