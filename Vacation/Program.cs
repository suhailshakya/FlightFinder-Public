using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;


/*
 * Main class that starts of the program
 * should first call Authorization.cs and run authentication API,
 * based on success or failure it should continue accordingly
 * 
 * Access DB to set params and call AirportCodes.cs,
 * within AirportCodes.cs --> use those params to call Ninjas API
 * 
 * Validate AirportCodes return values:
 * If successful and valid --> set params needed for CityPhotos,
 * by accessing DB for needed data (IATA, city, country),
 * then call CityPhotos.cs, which runs Google Places Photo API
 * 
 */

namespace Vacation
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        //implmenting blazor
        public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });

        /*static async Task Main()
        {
            List <string> iatasList = new List<string>();
            string departureCity = "Toronto";
            string departureCode = "YYZ";
            DateTime todayDate = (DateTime.Today).Date;
            string formattedDate = "";
            string formattedReturnDate = "";
            string classFlight = "ECONOMY";
            int maxPrice = 1200;
            string nonStop = "false";
            List<string> ninjaIatas = new List<string>();
            string googleCity;

            //call auth
            Auth.Authorization callAuth = new Auth.Authorization();
            //blazor call 1 start
            await callAuth.SetAuth();
            //blazor call 1 end
            string amadeusAuthCode = callAuth.GetAccessToken();

            Console.WriteLine(" Token: " + amadeusAuthCode);
            
            if (amadeusAuthCode.Length > 0)
            {
                //create AirportCodes object, which makes call to open DB and run queries
                Flights.AirportCodes iata = new Flights.AirportCodes();

                //add option to generate another iata code to add to the db or hash 
                while (true)
                {
                    Console.WriteLine("----------------- NEW ------------------- ");
                    Console.WriteLine("enter q to quit or anything else to continue : ");
                    string val = Console.ReadLine();
                    if (val == "q")
                    {
                        break;
                    }
                    //get values from regions table then store same values in iatasList
                    //blazor call 2 start (repeated with button)
                    iatasList = iata.getRecordsDB();

                    //blazor call 2 end
                    //google api to generate picture of random city
                    //iata object cannot be used because that makes connection to database, which already is connected
                    //                                                      city 
                    googleCity = await Flights.AirportCodes.GetCityPhoto(iatasList[0]);
                    Console.WriteLine("City from Google : " + googleCity);

                    //blazor call 3 start
                    //use all iatasList values to run GetIata method, which runs Ninjas API
                    //                                                city       countryCode     cityCode
                    ninjaIatas = await Flights.AirportCodes.GetIata(iatasList[0], iatasList[3], iatasList[2]);
                    iatasList.Clear();
                    
                    //do you want to get a flight ticket or nah?
                    Console.WriteLine("enter y to get flight ticket : ");
                    string val2 = Console.ReadLine();
                    
                    //link this with UI button for Kathmandu or Toronto
                    string deVal = Console.ReadLine();

                    if (val2 == "y")
                    {
                        while (formattedDate.Equals(""))
                        {
                            Console.WriteLine(" Enter departure date yyyy-mm-dd : ");
                            string val3 = Console.ReadLine();
                            if (DateTime.TryParse(val3, out DateTime userDate))
                            {
                                if (userDate <= todayDate)
                                {
                                    Console.WriteLine("Invalid, date cannot be in the past");
                                }
                                else
                                {
                                    formattedDate = userDate.ToString("yyyy-MM-dd");
                                    Console.WriteLine("Date: " + formattedDate);
                                    Console.WriteLine();
                                    while (formattedReturnDate.Equals(""))
                                    {
                                        Console.WriteLine(" Enter return date yyyy-mm-dd : ");
                                        string val4 = Console.ReadLine();
                                        if (DateTime.TryParse(val4, out DateTime userReturnDate))
                                        {
                                            if (userReturnDate <= userDate)
                                            {
                                                Console.WriteLine("Return date cannot be before depart date");
                                            }
                                            else if (userReturnDate <= todayDate)
                                            {
                                                Console.WriteLine("Invalid, return date cannot be in the past");
                                            }
                                            else
                                            {
                                                formattedReturnDate = userReturnDate.ToString("yyyy-MM-dd");
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        //Blazor call 3 end
                        //after authentication is set and iatasCodes are retrieved
                        List<string> ticket = await Flights.Itinerary.GetFlight(amadeusAuthCode, departureCode, ninjaIatas[0], formattedDate, formattedReturnDate, classFlight, maxPrice, nonStop);
                        // try to use a tuple (int ticketPrice, string arrivalCity, string arrivalCode, List<string> layovers) flightInfo

                        Console.WriteLine(" -------------------------------------     Ticket info         ------------------------------- ");
                        Console.WriteLine(" |                                                                                            |");
                        Console.WriteLine("        Leaving from : " + departureCity);
                        for (int i = 0; i < ticket.Count; i++)
                        {
                            Console.WriteLine("                                           " + ticket[i]);
                        }
                        Console.WriteLine("        Coming back from : " + ninjaIatas[1]);
                        Console.WriteLine(" |                                                                                            |");
                        Console.WriteLine(" -------------------------------------     Ticket info         ------------------------------- ");
                        formattedDate = "";
                        formattedReturnDate = "";
                    }
                }
            }
            else
            {
                Console.WriteLine("No database connection");
            }
        }*/


    }
}