namespace JobManager.Application.Configurations
{
    public static class AppSettings
    {
        public static readonly string JobRecurringSuffix = "Recurring";
        public static readonly string JobHandlerSuffix = "Handler";
        public static readonly string JobMethodName = "Handle";
        public static readonly string JobRecurringMethodName = "Start";
        public static readonly string JobParametersSuffix = "Parameters";

        public static readonly string JobDeactivationCronTime = "0 5 31 2 *";
        public static readonly string ProductionEnvironment = "Production";
        public static readonly string DevelopmentEnvironment = "Development";

        public static string? AppEnvironment = default;
        public static string? ProjectName = default;

        public static bool IsDebug = default;
        public static bool IsProdDb = default;

        public static bool AllowHangfireRegistration()
        {
            return !(string.IsNullOrEmpty(AppEnvironment) ||
                     string.Equals(DevelopmentEnvironment, AppEnvironment, StringComparison.OrdinalIgnoreCase) ||
                     IsDebug) && (!string.IsNullOrEmpty(ProjectName));
        }

        public static void Init()
        {
            AppEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            ProjectName = Environment.GetEnvironmentVariable("PROJECT");

#if DEBUG
            IsDebug = true;
            AppEnvironment = DevelopmentEnvironment;
            ProjectName = null;
#endif
#if !DEBUG
            if (string.IsNullOrEmpty(ProjectName))
                throw new Exception("Live service must have a ProjectName.");
#endif
        }

        public static object GetAppSettings()
        {
            return new
            {
                JobRecurringSuffix,
                JobHandlerSuffix,
                JobMethodName,
                JobRecurringMethodName,
                JobDeactivationCronTime,
                ProductionEnvironment,
                DevelopmentEnvironment,
                AppEnvironment,
                ProjectName,
                IsDebug,
                IsProdDb,
                AllowHangfireRegistration = AllowHangfireRegistration()
            };
        }
    }
}
