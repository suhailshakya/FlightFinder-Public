using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using SendGrid;
using System.Configuration;

namespace Vacation
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            //server side blazor
            services.AddServerSideBlazor();
            services.AddRazorPages(); 
            services.AddSingleton<Vacation.Auth.Authorization>();
            services.AddSingleton<Services.SharedDataService>();
            services.AddSingleton<Vacation.DBaccess.DatabaseConnection>();
            services.AddSingleton<Vacation.Flights.AirportCodes>();
            services.AddSingleton<Vacation.Flights.Itinerary>();
            services.AddSingleton<Vacation.DatabaseCache>();
            services.AddSingleton<Vacation.Photos.CityPhotos>();
            services.AddSingleton<Vacation.responseJSON.AirportCodeResponse>();
            services.AddSingleton<Vacation.responseJSON.ItineraryResponse>();
            services.AddSingleton<Vacation.responseJSON.GoogleCityPhotosResponse>();
            services.AddSingleton<System.Net.Http.HttpClient>();
            services.AddSingleton<Vacation.Email>();
            services.AddSingleton<Vacation.ReadFile>();
        }

        public void Configure(IApplicationBuilder app)
        {
            //for css and js files
            app.UseStaticFiles();
            app.UseRouting();
            app.UseEndpoints(endpoints => 
            { 
                //uses SignalR to interact between DOM and blazor server
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
