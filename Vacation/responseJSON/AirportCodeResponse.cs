using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vacation.responseJSON
{
    //JsonConvert.DeserializeObject<List<resonsesJSON.AirportCodeResponse>>
    public class AirportCodeResponse
    {
        public string icao { get; set; }
        public string iata { get; set; }
        public string name { get; set; }
        public string city { get; set; }
        public string region { get; set; }
        public string country { get; set; }
        public string elevation_ft { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string timezone { get; set; }
    }
}

