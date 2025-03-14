namespace TimerTrackerApp.Model
{
    public class UserData
    {
        public User User { get; set; }
        public List<Project> Projects { get; set; }
        public RunningTask? ActiveTask { get; set; }  

        public UserData()
        {
            Projects = new List<Project>();
        }

        public UserData(User user, List<Project> projects)
        {
            User = user;
            Projects = projects;
        }
    }

    public class RunningTask
    {
        public string ProjectName { get; set; }
        public string TaskName { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        public bool IsPaused { get; set; }

        public RunningTask(string projectName, string taskName, TimeSpan elapsedTime)
        {
            ProjectName = projectName;
            TaskName = taskName;
            ElapsedTime = elapsedTime;
        }
    }
}
