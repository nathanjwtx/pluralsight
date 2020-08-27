using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using OdeToFood.Core;

namespace OdeToFood.Data
{
    public class SqlRestaurantData : IRestaurantData
    {
        private readonly OdeToFoodDbContext _dbContext;

        public SqlRestaurantData(OdeToFoodDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Restaurant> GetRestaurantsByName(string name)
        {
            var query = from r in _dbContext.Restaurants
                where r.Name.StartsWith(name) || string.IsNullOrEmpty(name)
                orderby r.Name
                select r;

            return query;
        }

        public Restaurant GetById(int id)
        {
            return _dbContext.Find<Restaurant>(id);
        }

        public Restaurant Update(Restaurant updatedRestaurant)
        {
            var entity = _dbContext.Restaurants.Attach(updatedRestaurant);
            entity.State = EntityState.Modified;
            return updatedRestaurant;
        }

        public Restaurant Add(Restaurant newRestaurant)
        {
            _dbContext.Restaurants.AddAsync(newRestaurant);
            return newRestaurant;
        }

        public Restaurant Delete(int id)
        {
            var restaurant = GetById(id);
            _dbContext.Restaurants.Remove(restaurant);

            return restaurant;
        }

        public int GetCountOfRestaurants()
        {
            // by adding this to a view component we are adding a db query to every page. Might not offer the best performance. Consider caching
            return _dbContext.Restaurants.Count();
        }

        public int Commit()
        {
            return _dbContext.SaveChanges();
        }
    }
}