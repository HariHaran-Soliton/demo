namespace TimerTrackerApp.Model
{
    public class User
    {
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public string UserId {get; set; }
        public User(string userName, string password)
        {
            UserName = userName;
            PassWord = password;
        }
    }
}
