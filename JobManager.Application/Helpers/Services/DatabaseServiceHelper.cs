namespace JobManager.Application.Helpers.Services
{
    public static class DatabaseServiceHelper
    {
        public static bool IsWriteQuery(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return false;
            var lowerCaseQuery = query.ToLower();
            return (lowerCaseQuery.Contains("INSERT ") ||
                    lowerCaseQuery.Contains("INTO ") ||
                    lowerCaseQuery.Contains("EXEC ") ||
                    lowerCaseQuery.Contains("UPDATE ") ||
                    lowerCaseQuery.Contains("DELETE "));
        }
    }
}
