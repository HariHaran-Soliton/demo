namespace TimerTrackerApp.Model
{
    public class UserData
    {
        public User User { get; set; }
        public List<Project> Projects { get; set; }
        public UserData()
        {
        }
        public UserData(User user, List<Project> projects)
        {
            User = user;
            Projects = projects;
        }
    }
}
