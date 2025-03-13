using TimerTrackerApp.BusinessLogic;
using TimerTrackerApp.DataAccess;
using TimerTrackerApp.Presentation;

namespace TimerTrackerApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            UserRepository userRepository = new UserRepository();
            AuthenticationService authenticationService = new AuthenticationService(userRepository);
            ProjectService taskService = new ProjectService(userRepository);
            ApplicationUI taskUI = new ApplicationUI(taskService);
            AuthenticationUI authenticationUI = new AuthenticationUI(authenticationService, taskUI);
            authenticationUI.ShowAuthenticationMenu();
            Console.ReadKey();
        }
    }
}
