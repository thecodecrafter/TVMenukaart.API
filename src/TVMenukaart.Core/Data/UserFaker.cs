using Bogus;
using TVMenukaart.Models;

namespace TVMenukaart.Data
{
    public class UserFaker : Faker<AppUser>
    {
        public UserFaker()
        {
            RuleFor(a => a.UserName, f => f.Person.UserName);
            RuleFor(a => a.PhoneNumber, f => f.Phone.PhoneNumber());
            RuleFor(a => a.Email, f => f.Person.Email);
        }
    }
}
