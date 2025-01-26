using System.ComponentModel;

namespace JobManager.Application.Models.Enums
{
    public enum OrderStatusType
    {
        [Description("New")]
        New = 1,
        [Description("Invoiced")]
        Invoiced = 2,
        [Description("Packed")]
        Packed = 3,
        [Description("Shipped")]
        Shipped = 4
    }
}
