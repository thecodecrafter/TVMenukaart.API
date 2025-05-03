using Bogus;
using RemoteMenu.Models;

namespace RemoteMenu.Data
{
    public class RestaurantFaker : Faker<Restaurant>
    {
        public RestaurantFaker()
        {
            RuleFor(x => x.Name, f => f.Company.CompanyName());
        }
    }
}
