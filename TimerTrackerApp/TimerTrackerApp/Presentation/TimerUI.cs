using Spectre.Console;
using TimerTrackerApp.BusinessLogic;
using TimerTrackerApp.Model;

namespace TimerTrackerApp.Presentation
{
    public class TimerUI
    {
        private readonly TimerService _timerService;

        public TimerUI(TimerService timerService)
        {
            _timerService = timerService;
        }

        public void ShowTimerMenu(UserData userData)
        {
            while (true)
            {
                Console.Clear();
                var projectChoices = userData.Projects.Select(p => p.Name).Append("Exit").ToList();

                var projectName = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[grey100]Select a Project (or Exit):[/]")
                        .AddChoices(projectChoices)
                        .HighlightStyle("green")
                );

                if (projectName == "Exit")
                {
                    AnsiConsole.MarkupLine("[bold red]Exiting...[/]");
                    AnsiConsole.MarkupLine("[yellow]Press any key to close...[/]");
                    return;
                }

                if (userData.Projects is null || userData.Projects.Count == 0)
                {
                    AnsiConsole.MarkupLine("[bold red]No projects available[/]");
                    AnsiConsole.MarkupLine("[yellow]Press any key to close...[/]");
                    return;
                }

                var project = userData.Projects.FirstOrDefault(p => p.Name == projectName);

                if (project.TaskItems is null || project.TaskItems.Count == 0)
                {
                    AnsiConsole.MarkupLine("[bold red]No Task available[/]");
                    AnsiConsole.MarkupLine("[yellow]Press any key to close...[/]");
                    return;
                }
                List<string> taskChoices = project.TaskItems.Select(t => t.TaskName).Append("Back").ToList();

                string taskName = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[grey100]Select a Task (or Back):[/]")
                        .AddChoices(taskChoices)
                        .HighlightStyle("green")
                );

                if (taskName == "Back")
                {
                    continue;
                }

                ShowTaskActions(userData, projectName, taskName);
            }
        }



        private void ShowTaskActions(UserData userData, string projectName, string taskName)
        {
            while (true)
            {
                Console.Clear();
                AnsiConsole.MarkupLine($"[bold]Task: {taskName} (Project: {projectName})[/]");

                var action = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[grey100]Select an action:[/]")
                        .AddChoices("Start", "Stop", "Pause", "Back")
                        .HighlightStyle("green")
                );

                switch (action)
                {
                    case "Start":
                        _timerService.StartTask(userData, projectName, taskName);
                        break;
                    case "Stop":
                        _timerService.StopTask(userData, projectName, taskName);
                        break;
                    case "Pause":
                        _timerService.PauseTask(userData, projectName, taskName);
                        break;
                    case "Back":
                        return;
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }
    }
}
