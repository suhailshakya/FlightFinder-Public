using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Caching;
using Vacation.Flights;
using Google.Protobuf.WellKnownTypes;
using Microsoft.VisualBasic;

namespace Vacation
{
    public class DatabaseCache
    {
        private readonly MemoryCache iataCache;
        private readonly MemoryCache regionsCache;

        public DatabaseCache()
        {
            iataCache = MemoryCache.Default;
            regionsCache = MemoryCache.Default;
        }

        public void AddIataToCache(string cityKey, List<string> iataValues)
        {
            //print all current iata cache here
            iataCache.Set(cityKey, iataValues, DateTimeOffset.Now.AddMinutes(20));
            foreach (var cacheItem in iataCache)
            {
                Console.WriteLine($" cityKey: {cacheItem.Key}");
                var iataVals = cacheItem.Value as List<string>;

                if (iataVals != null)
                {
                    foreach (var iv in iataVals)
                    {
                        Console.WriteLine();
                        Console.WriteLine("START HERE");
                        Console.WriteLine($" IataVal: {iv}");
                        Console.WriteLine("END HERE");
                        Console.WriteLine();
                    }
                }
            }
            Console.WriteLine("Iata code from Ninjas has been added to cache");
        }

        public void AddRegionsToCache(string cityCodeKey, List<string> regionTableValues)
        {
            regionsCache.Set(cityCodeKey, regionTableValues, DateTimeOffset.Now.AddMinutes(20));
            Console.WriteLine("regions data added to cache");
        }

        public List<string> GetIatasBaby(string cityKey)
        {
            if (iataCache.Contains(cityKey)) {
                //print all iataCache here
                foreach (var cacheItem in iataCache)
                {
                    Console.WriteLine($"iata cache key : {cacheItem.Key}");
                    /*for (int i=0; i < (cacheItem.Value).Count; i++)
                    {
                        Console.WriteLine($"iata cache value: {cacheItem.Value}");
                    }*/
                }
                Console.WriteLine("GOT IATAS FROM CACHE YERRR");
                return (List<string>)iataCache[cityKey];
            }
            return null;
        }

        public List<string> getRegionsBaby(string cityCodeKey)
        {
            if (regionsCache.Contains(cityCodeKey))
            {
                Console.WriteLine("GOT REGIONS FROM CACHE YERRR");
                return (List<string>)regionsCache[cityCodeKey];
            }
            return null;
        }

        public int CountCachedRegions()
        {
            int totalCityRows = 0;

            // Print all keys currently stored in the cache
            Console.WriteLine("Keys currently stored in the cache:");
            foreach (var cacheItem in regionsCache)
            {
                if (cacheItem.Key.Length < 4) {
                    Console.WriteLine($"   Key: {cacheItem.Key}");
                    totalCityRows++;
                }
            }
            Console.WriteLine("");

            // Print the total number of rows
            Console.WriteLine($"Total number of rows in the cache with cities: {totalCityRows}");
            Console.WriteLine("");
            return totalCityRows;
        }
    }


}





/* 
 Required functions
1. Constructor to set up the structure
2. AddToCache function --> start expiration time
3. GetFromCache function --> check expiration time hasn't been exceeded
*/
/*public class DatabaseCache
{
    private readonly MemoryCache cache;
    private readonly List<string> cacheKeys;

    public DatabaseCache()
    {
        cache = MemoryCache.Default;
        cacheKeys = new List<string>();
    }

    public void AddToCache(string key, List<string> value)
    {
        cache.Set(key, value, DateTimeOffset.Now.AddMinutes(5));
        Console.WriteLine("you've reached the Add function in the cache and data has been successfully added");
        cacheKeys.Add(key);
        PrintCacheContents();
    }

    public void PrintCacheContents()
    {
        // Iterate over the keys in the list
        foreach (var cacheKey in cacheKeys)
        {
            Console.WriteLine("Cache Key: " + cacheKey);

            // Retrieve the value for each key
            var cachedValue = (List<string>)cache.Get(cacheKey);

            if (cachedValue != null)
            {
                foreach (var item in cachedValue)
                {
                    Console.WriteLine("Cache Value: " + item);
                }
            }
        }
    }

    public List<string> GetFromCache(string key)
    {
        if (cache.Contains(key))
        {
            Console.WriteLine("you've reached the Get function in the cache and the key exists in the cache already");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine(" -------------------------- CACHE -----------------------------------");
            Console.WriteLine("");
            PrintCacheContents();
            Console.WriteLine("");
            Console.WriteLine(" -------------------------- CACHE -----------------------------------");
            Console.WriteLine("");
            Console.WriteLine("");
            return (List<string>)cache.Get(key);
        }
        Console.WriteLine("you've reached the Get function in the cache and the key DOES NOT exists");
        return null;
    }

    public List<string> GetCityFromCache(int id)
    {
        //getting a random city from the Cache
        //start with the id number and loop
        //if the key at the given iteration is a cityCode
        //add the key to the end of the list which already contains (city, country, countryCode) as a list
        List<string> cityInfo = new List<string>();
        //new list with only cityCodes
        var twoLetterKeys = cacheKeys.Where(key => key.Length == 2).ToList();

        foreach (string key in cacheKeys)
        {
            if (id == 0)
            {
                cityInfo = cache.Get(key) as List<string>;
                Console.WriteLine(" this is random cityCode from cache: " + key);
                break;
            }
            id--;
        }
        Console.WriteLine(" This is the other info relating to the city code: " + cityInfo[0] + " " + cityInfo[1] + " " + cityInfo[2]);
        return cityInfo;

    }

    public int cityCodeTotals()
    {
        int total = 0;
        foreach (string key in cacheKeys)
        {
            if (key.Length == 2)
            {
                total++;
            }
        }
        return total;
    }

}*/