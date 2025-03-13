using TimerTrackerApp.DataAccess;
using TimerTrackerApp.Model;

namespace TimerTrackerApp.BusinessLogic
{
    public class TaskService
    {
        private readonly UserData _userData;
        private readonly Project _project;
        private readonly UserRepository _userRepository;

        public TaskService(UserData userData, Project project, UserRepository userRepository)
        {
            _userData = userData;
            _project = project;
            _userRepository = userRepository;

            if (_project.TaskItems == null)
            {
                _project.TaskItems = new List<TaskItem>();
            }
        }

        public bool IsTaskNameUnique(string taskName)
        {
            return !_project.TaskItems.Any(t => t.TaskName.Equals(taskName, StringComparison.OrdinalIgnoreCase));
        }

        public void AddTask(TaskItem task)
        {
            _project.TaskItems.Add(task);
            SaveUserData();
        }

        public void DeleteTask(string taskName)
        {
            var task = _project.TaskItems.FirstOrDefault(t => t.TaskName.Equals(taskName, StringComparison.OrdinalIgnoreCase));
            if (task != null)
            {
                _project.TaskItems.Remove(task);
                SaveUserData();
            }
        }

        public void UpdateTask(TaskItem updatedTask)
        {
            var taskIndex = _project.TaskItems.FindIndex(t => t.TaskName == updatedTask.TaskName);
            if (taskIndex != -1)
            {
                _project.TaskItems[taskIndex] = updatedTask;  
                SaveUserData(); 
            }
        }

        public void EditTask(string taskName, string newDescription)
        {
            var task = _project.TaskItems.FirstOrDefault(t => t.TaskName.Equals(taskName));
            if (task != null)
            {
                task.TaskDescription = newDescription;
                SaveUserData();
            }
        }

        public List<TaskItem> GetTasks() => _project.TaskItems;

        private void SaveUserData()
        {
            _userData.Projects = _userData.Projects ?? new List<Project>();
            _userRepository.SaveUser(_userData);
        }
    }
}
