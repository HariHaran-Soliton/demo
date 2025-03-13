namespace TimerTrackerApp.Model
{
    public class Project
    {
        public string Name { get; set; }
        public List<TaskItem> TaskItems { get; set; }
        public ProjectCategory Category { get; set; }
        public Project(string name, ProjectCategory category)
        {
            Name = name;
            Category = category;
        }
    }

    public enum ProjectCategory
    {
        work,
        personal
    }
}
