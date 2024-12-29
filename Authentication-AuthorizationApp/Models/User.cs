using System.Collections.Generic;

namespace TaskAuthenticationAuthorization.Models
{
    public enum BuyerType
    {
        None,
        Regular,
        Golden,
        Wholesale
    }
    public enum Discount
    {
        O, R, V
    }
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; } 
        public string Address { get; set; }
        public Discount? Discount { get; set; }
        public string Password { get; set; }

        public int? RoleId { get; set; }
        public Role Role { get; set; }
        public BuyerType? BuyerType { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
