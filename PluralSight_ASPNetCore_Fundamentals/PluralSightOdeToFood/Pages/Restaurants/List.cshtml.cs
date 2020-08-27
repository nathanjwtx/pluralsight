using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using OdeToFood.Core;
using OdeToFood.Data;

namespace PluralSightOdeToFood.Pages.Restaurants
{
    public class ListModel : PageModel
    {
        private readonly IConfiguration _config;
        private readonly IRestaurantData _restaurantData;
        public string Message { get; set; }
        
        // tells project to look for SearchTerm in the http request before calling OnGet
        // by default only supports POST but can modify as shown
        [BindProperty(SupportsGet = true)] public string SearchTerm { get; set; }
        
        public IEnumerable<Restaurant> Restaurants { get; set; }

        public ListModel(IConfiguration config, IRestaurantData restaurantData)
        {
            _config = config;
            _restaurantData = restaurantData;
        }
        
        public void OnGet(string searchTerm)
        {
            // Hard coded message
            // Message = "Hello, world!";
            // Message pulled through from appsettings.json
            Message = _config.GetValue<string>("Message");
            Restaurants = _restaurantData.GetRestaurantsByName(SearchTerm);
        }
    }
}