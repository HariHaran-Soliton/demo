using Spectre.Console;
using TimerTrackerApp.DataAccess;
using TimerTrackerApp.Model;

public class TimerService
{
    private readonly UserRepository _userRepository;

    public TimerService(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public void StartTask(UserData userData, string projectName, string taskName)
    {
        var project = userData.Projects.FirstOrDefault(p => p.Name == projectName);
        if (project == null) return;

        var runningTask = project.TaskItems.FirstOrDefault(t => t.TaskStatus == TaskState.Ongoing);
        if (runningTask != null)
        {
            AnsiConsole.MarkupLine("[red]Another task is already running. Please stop or pause it first.[/]");
            return;
        }

        var task = project.TaskItems.FirstOrDefault(t => t.TaskName == taskName);
        if (task == null) return;

        if (task.TaskStatus == TaskState.Completed)
        {
            AnsiConsole.MarkupLine("[red]This task is already completed.[/]");
            return;
        }

        if (task.TaskStatus == TaskState.NotStarted)
        {
            task.StartTime = DateTime.Now; // Set only the first time
        }

        task.TaskStatus = TaskState.Ongoing;
        _userRepository.SaveUser(userData);

        AnsiConsole.MarkupLine($"[green]Started task: {taskName} at {task.StartTime}[/]");
    }


    public void StopTask(UserData userData, string projectName, string taskName)
    {
        var project = userData.Projects.FirstOrDefault(p => p.Name == projectName);
        if (project == null) return;

        var task = project.TaskItems.FirstOrDefault(t => t.TaskName == taskName);
        if (task == null) return;

        if (task.TaskStatus != TaskState.Ongoing)
        {
            AnsiConsole.MarkupLine("[red]Task is not running. Start the task first.[/]");
            return;
        }

        task.EndTime = DateTime.Now;
        task.Duration += task.EndTime - task.StartTime;
        task.TaskStatus = TaskState.Completed;

        _userRepository.SaveUser(userData);

        AnsiConsole.MarkupLine($"[yellow]Stopped task: {taskName} at {task.EndTime}[/]");
        AnsiConsole.MarkupLine($"[blue]Total duration: {task.Duration}[/]");
    }

    public void PauseTask(UserData userData, string projectName, string taskName)
    {
        var project = userData.Projects.FirstOrDefault(p => p.Name == projectName);
        if (project == null) return;

        var task = project.TaskItems.FirstOrDefault(t => t.TaskName == taskName);
        if (task == null) return;

        if (task.TaskStatus != TaskState.Ongoing)
        {
            AnsiConsole.MarkupLine("[red]Task is not running. Start the task first.[/]");
            return;
        }

        task.EndTime = DateTime.Now;
        task.Duration += task.EndTime - task.StartTime;
        task.TaskStatus = TaskState.Paused;

        _userRepository.SaveUser(userData);

        AnsiConsole.MarkupLine($"[yellow]Paused task: {taskName} at {task.EndTime}[/]");
        AnsiConsole.MarkupLine($"[blue]Total duration: {task.Duration}[/]");
    }
}
