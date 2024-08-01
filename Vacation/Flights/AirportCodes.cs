using GoogleApi.Entities.Search.Video.Common;
using K4os.Compression.LZ4.Streams.Adapters;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Cmp;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace Vacation.Flights
{
    class AirportCodes
    {
        private string table = "regions";
        private string database = "locations";
        private DBaccess.DatabaseConnection db;
        private string cityRegions = "";
        private string countryCodeRegions = "";
        private string cityCodeRegions = "";
        private string countryRegions = "";
        int upperBound = 0;
        List<string> iataParams = new List<string>();
        bool flag = false;
        private List<string> globalCityAccess = new List<string>();
        List<string> iataCodes = new List<string>();
        ReadFile rf = new ReadFile();



        public AirportCodes() {
            db = new DBaccess.DatabaseConnection();
        }
        //after database is successfully connected
        //open the connection and run queries


        
        //selects a random row from regions table and returns those values as a list
        public async Task<List<string>> getRecordsDB()
        {
            DatabaseCache cacheAccess = new DatabaseCache();

            //the following condition is basically only ran once to get the length of the table
            if (upperBound == 0)
            {
                db.OpenConnection();
                flag = true;
                //query database to retrieve length of rows
                string query1 = $"use {database}";
                using (MySqlCommand c1 = new MySqlCommand(query1, db.GetConnection()))
                {
                    c1.ExecuteNonQuery();
                }
                string query2 = $"select count(*) from {table}";
                using (MySqlCommand c2 = new MySqlCommand(query2, db.GetConnection()))
                {
                    upperBound = Convert.ToInt32(c2.ExecuteScalar())+1;
                }
                //prints all values from regions table
                string q3 = $"select * from {table}";
                using (MySqlCommand command = new MySqlCommand(q3, db.GetConnection()))
                {
                    // Create a DataTable to store the result set
                    DataTable dataTable = new DataTable();
                    // Use MySqlDataAdapter to fill the DataTable
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                    // Display the DataTable in the console
                    DisplayDataTable(dataTable);
                }
                Console.WriteLine("upperbound: " + upperBound);
            }
            //generate random number between 1 and upperBound
            Random r = new Random();
            int rand = r.Next(1, upperBound);
            Console.WriteLine("generated random number : " + rand);
            rand = rand - 1;
            Console.WriteLine("amount of rows to offset: " + rand);

            //count the amount of rows the cache has with unique city Codes as the key
            //if the cache does not have all the city Codes from the DB, continue on
            int totalCities = cacheAccess.CountCachedRegions();
            if (totalCities != upperBound-1)
            {
                //check if db connection is already set
                if (flag == false)
                {
                    db.OpenConnection();
                }

                string queryIATAparams = $"select city, country, cityCode, countryCode from {table} limit 1 offset {rand}";
                //use data reader to parse queried data and store into a variety of variables
                try
                {   // Execute the query and get a data reader
                    using (MySqlDataReader reader = this.db.ExecuteQuery(queryIATAparams))
                    {
                        if (reader != null)
                        {
                            List<string> regionData = new List<string>();
                            // Process the data here
                            while (reader.Read())
                            {
                                // Extract values from the reader and store locally
                                cityRegions = reader.GetString(0);
                                countryRegions = reader.GetString(1);
                                cityCodeRegions = reader.GetString(2);
                                countryCodeRegions = reader.GetString(3);

                                // Store values locally or perform other processing
                                iataParams.Add(cityRegions);
                                iataParams.Add(countryRegions);
                                iataParams.Add(cityCodeRegions);
                                iataParams.Add(countryCodeRegions);
                                Console.WriteLine("");
                                Console.WriteLine(" Following is extracted from Airports Table ");
                                Console.WriteLine("");
                                Console.WriteLine($" City From Regions : {cityRegions} , Country From Regions : {countryRegions} , CityCode From Regions : {cityCodeRegions} , CountryCode From Regions : {countryCodeRegions}");
                                Console.WriteLine("");
                            }
                            //insert values into cache
                            regionData.Add(cityRegions);
                            regionData.Add(countryRegions);
                            regionData.Add(countryCodeRegions);
                            Console.WriteLine("Adding the regions table data to cache");
                            cacheAccess.AddRegionsToCache(cityCodeRegions, regionData);
                            globalCityAccess.Add(cityCodeRegions);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
                db.CloseConnection();
                flag = false;
            }
            //if cache has the same length of rows from Regions table : then read from cache instead of db, since all values are available on cache
            else
            {
                string randCityCode = globalCityAccess[rand];
                Console.WriteLine("All cities from regions table are cached, here is the random selected city Code: " + randCityCode);
                List<string> temp = new List<string>();
                temp = cacheAccess.getRegionsBaby(randCityCode);
                iataParams.Add(temp[0]); //city
                iataParams.Add(temp[1]); //country
                iataParams.Add(randCityCode); //cityCode
                iataParams.Add(temp[2]); //countryCode
            }
            return iataParams;
        }



        static void DisplayDataTable(DataTable dataTable)
        {
            Console.WriteLine("Following is display from the Airports table: ");
            // Display column headers
            foreach (DataColumn column in dataTable.Columns)
            {
                Console.Write($"{column.ColumnName,-15}");
            }
            Console.WriteLine("");

            // Display data rows
            foreach (DataRow row in dataTable.Rows)
            {
                foreach (var item in row.ItemArray)
                {
                    Console.Write($"{item,-15}");
                }
                Console.WriteLine("");
            }
        }



        //after a random record is retrieved from database regions table, use that value to generate a pic
        public async Task<List<string>> GetCityPhoto(string city)
        {
            Console.WriteLine("city: " + city);
            List<string> googleCityInfo = new List<string>();
            string googlePhotoRef = "";
            string googleFormAdd = "";
            string apiKey = rf.ReadFileCreds("googlePhotoKey");
            //run this API first to get the reference value from Google Photos
            string googleURL = $"https://maps.googleapis.com/maps/api/place/textsearch/json?query={city}&key={apiKey}";
            using (HttpClient getGoogleCity = new HttpClient())
            {
                HttpResponseMessage response = await getGoogleCity.GetAsync(googleURL);
                try
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string parseCity = await response.Content.ReadAsStringAsync();
                        dynamic parseCity2 = JsonConvert.DeserializeObject(parseCity);
                        googlePhotoRef = parseCity2.results[0].photos[0].photo_reference;
                        googleFormAdd = parseCity2.results[0].formatted_address;
                        googleCityInfo.Add(googlePhotoRef);
                        googleCityInfo.Add(googleFormAdd);
                    }
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine("Google Places API did not work, no referenece generated");
                    Console.WriteLine($"An HTTP request exception occurred: {ex.Message}");
                }
            }
            return googleCityInfo;
        }



        //function that runs Ninjas API to retrieve corresponding Iata codes
        public async Task<List<string>> GetIata(string city, string countryCode, string cityCode)
        {
            if (iataCodes.Count > 0)
            {
                iataCodes.Clear();
            }
            Console.WriteLine("city: " + city);

            //check if city exists in cache, if it does: then return iata codes and city without having to run the api request again
            DatabaseCache cacheAccess = new DatabaseCache();
            List<string> cacheIataValues = cacheAccess.GetIatasBaby(city);
            if (cacheIataValues != null)
            {
                int i = 0;
                while (i < cacheIataValues.Count)
                {
                    iataCodes.Add(cacheIataValues[i]);
                    i++;
                }
            }
            else
            {
                string apiKey = rf.ReadFileCreds("ninjaKey");

                string ninjaURL = $"https://api.api-ninjas.com/v1/airports?country={countryCode}&city={city}";
                using (HttpClient getNinjaData = new HttpClient())
                {
                    getNinjaData.DefaultRequestHeaders.Add("x-api-key", apiKey);
                    try
                    {
                        Thread.Sleep(2000);
                        HttpResponseMessage response = await getNinjaData.GetAsync(ninjaURL);
                        Console.WriteLine("");
                        Console.WriteLine("Ninjas status code: " + response.StatusCode);
                        if (response.IsSuccessStatusCode)
                        {
                            string parsedResponse = await response.Content.ReadAsStringAsync();
                            // Deserialize JSON into a list of Airport objects
                            List<responseJSON.AirportCodeResponse> airports = JsonConvert.DeserializeObject<List<responseJSON.AirportCodeResponse>>(parsedResponse);
                            Console.WriteLine("");
                            Console.WriteLine("");

                            //print out values retrieved from Ninjas API
                            for (int i = 0; i < airports.Count; i++)
                            {
                                string actualIATA = airports[i].iata;
                                if (actualIATA != "")
                                {
                                    Console.WriteLine($" IATA from ninjas: {actualIATA}");
                                    iataCodes.Add(actualIATA);
                                }
                            }

                            Console.WriteLine("");
                            Console.WriteLine("");

                            //add Ninja response values to Cache here
                            //add city name as key and list of all iata codes associated with city as value
                            Console.WriteLine("Adding the NINJAs data to cache");
                            cacheAccess.AddIataToCache(city, iataCodes);

                            //call function to check if iata codes need to be inserted into db
                            AirportCodes ac = new AirportCodes();
                            await ac.updateAirportsRecords(city, iataCodes, cityCode);
                        }
                        else
                        {
                            Console.WriteLine("Error with API here at the NINJAs API");
                        }
                    }
                    catch (HttpRequestException ex)
                    {
                        Console.WriteLine($"An HTTP request exception occurred: {ex.Message}");
                    }
                }
            }
            return iataCodes;
        }



        //function to check if iata codes returned from Ninjas api need to be inserted in airports table
        public async Task updateAirportsRecords(string city, List<string> iataC, string cityCode)
        {
            List<string> iataCopy = new List<string>(iataC);
            db.OpenConnection();
            // Check if airports table already has a value (checking for just the cityCode is enough) returned from Ninjas API existing in its table
            string existQuery = "SELECT COUNT(*) FROM airports WHERE cityCode = @cityCode";

            using (MySqlCommand comm = new MySqlCommand(existQuery, db.GetConnection()))
            {
                comm.Parameters.AddWithValue("@cityCode", cityCode);
                int rowCount = Convert.ToInt32(await comm.ExecuteScalarAsync());

                if (rowCount < 1)
                {
                    Console.WriteLine($"The value '{cityCode}' DOES NOT exist in the ");
                    // If the values do not exist, use an insert query
                    while (iataCopy.Count > 0)
                    {
                        Console.WriteLine("Program should come here if the city does not exist in the Airports table ");
                        string query2 = "INSERT INTO airports (city, iata, cityCode) VALUES (@city, @iata, @cityCode)";
                        using (MySqlCommand c1 = new MySqlCommand(query2, db.GetConnection()))
                        {
                            c1.Parameters.AddWithValue("@city", city);
                            c1.Parameters.AddWithValue("@iata", iataCopy[0]);
                            c1.Parameters.AddWithValue("@cityCode", cityCode);

                            await c1.ExecuteNonQueryAsync();
                            Console.WriteLine("The city has been added to the Airports table");
                        }
                        //remove the iata code that was added to the Airports table
                        iataCopy.RemoveAt(0);
                        if (iataCopy.Count > 0) Console.WriteLine("This is the new first in the list: " + iataCopy[0]);
                    }

                    // Print out the contents of the 'airports' table after insert queries
                    Console.WriteLine("Airports table after insert queries:");
                    string selectQuery = "SELECT * FROM airports";
                    using (MySqlCommand selectCommand = new MySqlCommand(selectQuery, db.GetConnection()))
                    {
                        using (MySqlDataReader reader = (MySqlDataReader)await selectCommand.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                string resultCity = reader.GetString("city");
                                string resultIata = reader.GetString("iata");
                                string resultCityCode = reader.GetString("cityCode");

                                Console.WriteLine($"City: {resultCity}, IATA: {resultIata}, City Code: {resultCityCode}");
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"The value '{cityCode}' ALREADY DOES exist in the column.");
                    //update cache here
                }
            }
            db.CloseConnection();
        }


    }
}
