namespace JobManager.Application.Models.Entities.Mongo
{
    public class BaseMongoEntity
    {
        public int _id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
