using Bogus;
using TVMenukaart.Models;

namespace TVMenukaart.Data
{
    public class RestaurantFaker : Faker<Restaurant>
    {
        public RestaurantFaker()
        {
            RuleFor(x => x.Name, f => f.Company.CompanyName());
        }
    }
}
