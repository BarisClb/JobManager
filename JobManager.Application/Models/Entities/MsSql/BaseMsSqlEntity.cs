namespace JobManager.Application.Models.Entities.MsSql
{
    public class BaseMsSqlEntity
    {
        public Guid Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
