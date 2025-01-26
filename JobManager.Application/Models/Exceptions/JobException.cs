namespace JobManager.Application.Models.Exceptions
{
    public class JobException : Exception
    {
        public JobException()
        {
        }

        public JobException(string message) : base(message)
        {
        }
    }
}
