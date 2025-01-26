namespace JobManager.Application.Models.Entities.MsSql
{
    public class Customer : BaseMsSqlEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
    }
}
