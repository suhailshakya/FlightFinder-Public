using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using GoogleApi.Entities.Interfaces;
using System.Runtime.CompilerServices;
using Vacation.responseJSON;
using System.Linq.Expressions;

namespace Vacation.Flights
{
    class Itinerary
    {
        //store flight price, duration, destination city, layover codes
        private static List<string> allFightInfo = new List<string>(); 
        private DateTime todayDate = (DateTime.Today).Date;
        private string classFlight = "ECONOMY";
        private string nonStop = "false";
        private int maxItiDataSize = 0;
        private double flightPrice = 0;
        private int dataNum = 0;

        public Itinerary() { 
        }

        public async Task<List<string>> GetFlight(string arrivalCity, string departureCode, string departDate, string returnDate, string ninjaIata, string accessTk)
        {
            string bearer = "Bearer " + accessTk;
            if (allFightInfo.Count > 0)
            {
                allFightInfo.Clear();
            }

            Console.WriteLine("acces code for the itinerary received from auth: " + accessTk);

            string getURL = $"https://test.api.amadeus.com/v2/shopping/flight-offers?originLocationCode={departureCode}&destinationLocationCode={ninjaIata}&departureDate={departDate}&returnDate={returnDate}&adults=1&travelClass={classFlight}&nonStop={nonStop}&currencyCode=CAD&max=7";
            HttpResponseMessage response = new HttpResponseMessage();
            using (HttpClient getFlightTicker = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("GET"), getURL))
                {
                    request.Headers.TryAddWithoutValidation("Authorization", bearer);
                    response = await getFlightTicker.SendAsync(request);
                }
                try
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string parsedResponse = await response.Content.ReadAsStringAsync();
                        dynamic parsedJSON = JsonConvert.DeserializeObject(parsedResponse);
                        maxItiDataSize = parsedJSON.data.Count;
                        dynamic price;

                        if (maxItiDataSize != 0)
                        {
                            //create min heap if there are multiple prices and dequeue lowest price
                            if (maxItiDataSize > 1)
                            {
                                Console.WriteLine("There are multiple prices");
                                PriorityQueue<int, double> minHeap = new PriorityQueue<int, double>(Comparer<double>.Create((x, y) => x.CompareTo(y)));
                                for (int i = 0; i < maxItiDataSize; i++)
                                {
                                    //get price of flight, parse through json
                                    price = parsedJSON.data[i].price.grandTotal;
                                    flightPrice = Convert.ToDouble(price);

                                    //add to heap, lowest priority (price) set at the top
                                    Console.WriteLine("price " + i + " :  " + flightPrice);
                                    minHeap.Enqueue(i, flightPrice);
                                }

                                //once all prices have been added to heap, dequeue the lowest price from the top of the min heap
                                if (minHeap.TryDequeue(out var dequeuedValue, out var dequeuedPriority))
                                {
                                    // Dequeue was successful
                                    this.dataNum = dequeuedValue;
                                    this.flightPrice = dequeuedPriority;
                                }
                                else
                                {
                                    Console.WriteLine("The Min Heap is empty.");
                                }
                            }
                            else
                            {
                                //if there is only 1 price and element in the data section
                                //get price of flight, parse through json
                                Console.WriteLine("There is only 1 price");
                                this.dataNum = 0;
                                price = parsedJSON.data[dataNum].price.grandTotal;
                                this.flightPrice = Convert.ToDouble(price);
                            }

                            //check if the lowest or only price available is greater than this amount
                            if (this.flightPrice > 1300.00)
                            {
                                //adds price value to list string 
                                allFightInfo.Add(flightPrice.ToString());
                                return allFightInfo;
                            }

                            Console.WriteLine("this is the price: " + this.flightPrice.ToString());
                            allFightInfo.Add(this.flightPrice.ToString());

                            //departing information
                            if (departureCode == "YYZ")
                            {
                                allFightInfo.Add("Toronto - " + departureCode);
                            }
                            else
                            {
                                allFightInfo.Add("Kathmandu - " + departureCode);
                            }

                            //get duration of flight
                            dynamic dur1 = parsedJSON.data[this.dataNum].itineraries[0].duration;
                            Console.WriteLine("this is the duration of the flight otw: " + dur1.ToString());
                            allFightInfo.Add(dur1.ToString());

                            //time of depart flight
                            dynamic time1 = parsedJSON.data[this.dataNum].itineraries[0].segments[0].departure.at;
                            allFightInfo.Add(time1.ToString());

                            //check if there are any layovers by counting the segments in itineraries
                            var arrayValue = parsedJSON.data[this.dataNum].itineraries[0];

                            // Count the number of elements in the array
                            var numberOfElements = arrayValue.length;
                            if (numberOfElements > 1)
                            {
                                //get layover otw
                                dynamic departLayoverIata = parsedJSON.data[this.dataNum].itineraries[0].segments[1].departure.iataCode;
                                Console.WriteLine("this is the layover otw: " + departLayoverIata.ToString());
                                allFightInfo.Add(departLayoverIata.ToString());
                            }
                            else
                            {
                                allFightInfo.Add("No departure layovers");
                            }

                            //returning information
                            allFightInfo.Add(arrivalCity + " - " + ninjaIata);

                            dynamic dur2 = parsedJSON.data[this.dataNum].itineraries[1].duration;
                            Console.WriteLine("this is the duration otw BACK: " + dur2.ToString());
                            allFightInfo.Add(dur2.ToString());

                            //time of return flight
                            dynamic time2 = parsedJSON.data[this.dataNum].itineraries[1].segments[0].departure.at;
                            allFightInfo.Add(time2.ToString());

                            if (numberOfElements > 1)
                            {
                                //get layover otw back
                                dynamic returnLayoverIata = parsedJSON.data[this.dataNum].itineraries[1].segments[1].departure.iataCode;
                                Console.WriteLine("this is the layover on the way back: " + returnLayoverIata.ToString());
                                allFightInfo.Add(returnLayoverIata.ToString());
                            }
                            else
                            {
                                allFightInfo.Add("No return layovers");
                            }
                        }

                        else
                        {
                            //if there are no flights found for these parameters, there is no elements in the data []
                            string error = "NO TICKETS WITHIN PRICE OR DATE RANGE FOR THIS LOCATION.  Please select again";
                            allFightInfo.Add(error);
                        }
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        //most likely due to invalid Iata from Ninja (Madrid, ECV)
                        string error = "Server issue, please contact Suhail";
                        allFightInfo.Add(error);
                    }
                }
                catch (Exception ex)
                {
                    // Handle other exceptions
                    //most likely due to invalid Iata from Ninja (Madrid, ECV)
                    Console.WriteLine($"Exception caught: {ex.Message}");
                    string error = ex.Message + ". Server issue, please contact Suhail";
                    allFightInfo.Add(error);
                }
            }
            return allFightInfo;
        }
    }
}
