using JobManager.Application.Configurations;

namespace JobManager.Application.Helpers.Services
{
    public static class JobServiceHelper
    {
        public static string? ProdRequestCode;

        public static List<Type> GetAllJobs()
        {
            var type = typeof(IJobRecurring);
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && p.IsClass)
                .ToList();
        }

        public static string? CheckProdRequestCode(string? prodRequestCode)
        {
            if (!string.IsNullOrEmpty(prodRequestCode) && !string.IsNullOrEmpty(ProdRequestCode) && string.Equals(prodRequestCode, ProdRequestCode, StringComparison.OrdinalIgnoreCase))
                return null;

            Random random = new();
            var randomNumber = random.Next(0, 100000).ToString().PadLeft(8, '0');
            ProdRequestCode = randomNumber;
            return randomNumber;
        }
    }
}
