namespace JobManager.Application.Models.Entities.Mongo
{
    public class Customer : BaseMongoEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
    }
}
