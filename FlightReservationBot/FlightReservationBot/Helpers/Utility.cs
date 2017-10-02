using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlightReservationBot.Helpers
{
    public static class Utility
    {
        public static string Capitalize(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentException("Cannot capitalize null or empty string.");
            }
            return input.First().ToString().ToUpper() + input.Substring(1);
        }
    }
}