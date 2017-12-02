using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlightReservationBot.Models
{
    public enum FlightTypeOptions
    {
        OneWay = 1,
        TwoWay = 2
    }

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
        [Prompt("Please enter the Origin you are flying from:")]
        public string Origin;

        [Prompt("Please enter the Destination you are flying to:")]
        public string Destination;

        public FlightTypeOptions? FlightType;

        [Prompt("Please enter a Departure date:")]
        public DateTime DepartureDate;

        [Optional]
        [Prompt("Please enter a Return date:")]
        public DateTime? ReturnDate;

        public string PassengerName;

        public CheckInOptions? CheckIn;

        [Prompt("What kind of Extra Options would you like? {||}")]
        [Optional]
        public List<ExtraServiceOptions> ExtraService;

        public static IForm<FlightReservation> BuildForm()
        {
            return new FormBuilder<FlightReservation>()
                .Message("Welcome to Flight Booking assistant!")
                .Field(nameof(Origin),
                validate: async (state, value) => {
                    var result = new ValidateResult();
                    string origin = (string)value;

                    result.IsValid = Place.AvailablePlaces.Exists(p => p.Name.Equals(origin, StringComparison.OrdinalIgnoreCase));
                    result.Feedback = result.IsValid ? null : "Place not available";
                    result.Value = origin;

                    return result;
                })
                .Field(nameof(Destination),
                validate: async (state, value) => {
                    var result = new ValidateResult();
                    string destination = (string)value;

                    var isAvailable = Place.AvailablePlaces.Exists(p => p.Name.Equals(destination, StringComparison.OrdinalIgnoreCase));
                    var isDifferent = !destination.Equals(state.Origin, StringComparison.OrdinalIgnoreCase); 

                    result.IsValid = isAvailable && isDifferent;

                    if(!isAvailable)
                    {
                        result.Feedback = "Place not available";
                    }
                    else if(!isDifferent)
                    {
                        result.Feedback = "Destination cannot be same as Origin";
                    }
                    else
                    {
                        result.Feedback = null;
                    }

                    result.Value = destination;

                    return result;
                })
                .Field(nameof(FlightType))
                .Field(nameof(DepartureDate), 
                validate: async(state, value) => {
                    var result = new ValidateResult();
                    result.IsValid = ((DateTime)value - DateTime.Now).Days > 0;
                    result.Feedback = result.IsValid ? null : "Departure date must be later than current date";
                    result.Value = (DateTime)value;
                    return result;
                })
                .Field(nameof(ReturnDate),
                active: ((state) => state.FlightType == FlightTypeOptions.TwoWay),
                validate: async (state, value) =>
                {
                    var result = new ValidateResult();
                    result.IsValid = (DateTime)value > state.DepartureDate;
                    result.Feedback = result.IsValid ? null : "Departure date cannot be later that return date";
                    result.Value = (DateTime)value;
                    return result;
                })
                .AddRemainingFields()
                .Confirm("Flight details: \r\r \r\r" +
                "Origin:{Origin} \r\r" +
                "Destination:{Destination} \r\r " +
                "Departure date:{DepartureDate:dd MMMM yyyy} \r\r" +
                "Return date:{ReturnDate:dd MMMM yyyy} \r\r" +
                "Passenger name: {PassengerName} \r\r" +
                "Check in method: {CheckIn} \r\r" +
                "Extra services: {ExtraService} \r\r \r\r" +
                "Total cost: €200 \r\r \r\r" +
                "Are you sure you want to proceed?")
                .Build();
        }
    }
}