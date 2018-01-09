namespace SenStaySync.PageProcesser.AirBnb
{
    public class LoginResult
    {
        public LoginResult()
        {
            Status = AirBnbLoginStatus.Failed;
        }

        public AirBnbLoginStatus Status { get; set; }
        public string Message { get; set; } 
    }
}