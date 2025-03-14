namespace TimerTrackerApp.Model
{
    public class TaskItem
    {
        public string? TaskName { get; set; }
        public string? TaskDescription { get; set; }
        public TaskState TaskStatus { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime ResumeTime { get; set; }
        public TimeSpan Duration { get; set; }
        public TimeSpan ThresholdTime { get; set; }
        public DateTime LastSavedTime { get; set; }  // New field for tracking ongoing duration
    }

    public enum TaskState
    {
        NotStarted,
        Ongoing,
        Paused,
        Completed
    }
}
