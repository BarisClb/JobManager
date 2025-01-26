namespace JobManager.Application.Models.Entities.MsSql
{
    public class Order : BaseMsSqlEntity
    {
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
