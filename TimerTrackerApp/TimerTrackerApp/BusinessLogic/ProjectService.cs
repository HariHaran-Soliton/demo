using TimerTrackerApp.DataAccess;
using TimerTrackerApp.Model;

namespace TimerTrackerApp.BusinessLogic
{
    public class ProjectService
    {
        private List<Project>? _projects;
        private UserRepository _userRepository;

        public ProjectService(UserRepository userRepository)
        {
            _userRepository = userRepository;
            _projects = new List<Project>();
        }

        public void SetUserProjects(UserData userData)
        {
            _projects = userData.Projects ?? new List<Project>();
        }

        public List<Project>? GetUserProjects()
        {
            return _projects;
        }

        public void AddNewProject(Project project, UserData userData)
        {
            if (_projects == null)
            {
                _projects = new List<Project>();
            }
            _projects.Add(project);
            SaveUserData(userData);
        }

        public void DeleteProject(Project project, UserData userData)
        {
            _projects.Remove(project);
            SaveUserData(userData);
        }
        public bool IsProjectNameExist(string name)
        {
            if (_projects == null)
            {
                return false;
            }
            else
            {
                return _projects.Any(p => p.Name.Equals(name));
            }
        }

        public void SaveUserData(UserData userData)
        {
            userData.Projects = _projects ?? new List<Project>();
            _userRepository.SaveUser(userData);
        }
    }
}