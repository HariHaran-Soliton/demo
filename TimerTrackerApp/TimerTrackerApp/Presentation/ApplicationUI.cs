using Spectre.Console;
using TimerTrackerApp.BusinessLogic;
using TimerTrackerApp.Model;
using ConsoleTables;
using TimerTrackerApp.DataAccess;
<<<<<<< HEAD

=======
>>>>>>> b3b76a3436b33e93b526882a19c21ad0ba046dbd
namespace TimerTrackerApp.Presentation
{
    public class ApplicationUI
    {
        private readonly ProjectService _projectService;

        public ApplicationUI(ProjectService projectService)
        {
            _projectService = projectService;
        }

<<<<<<< HEAD
        public void ApplicationMenu(UserData userData)
=======

        public void TimerMenu(UserData userData)
>>>>>>> b3b76a3436b33e93b526882a19c21ad0ba046dbd
        {
            _projectService.SetUserProjects(userData);
            while (true)
            {
                Console.Clear();
                DisplayDashboard(userData);
                AnsiConsole.MarkupLine($"[bold cyan]=== Task Menu of {userData.User.UserName} ===[/]");

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[grey100]Select an option:[/]")
                        .AddChoices("Manage Project", "Manage Task", "Control Timer", "Generate Report", "Export CSV", "Logout")
                        .HighlightStyle("green")
                );
                switch (choice)
                {
                    case "Manage Project":
                        ManageProject(userData);
                        break;
                    case "Manage Task":
                        ManageTaskMenu(userData);
                        break;
                    case "Control Timer":
                        UserRepository userRepository = new UserRepository();
<<<<<<< HEAD
                        var taskTimerUI = new TimerUI(new TimerService(userRepository));
                        taskTimerUI.ShowTimerMenu(userData);
                        Console.ReadKey();
=======
                        var taskTimerUI = new TaskTimerUI(new TimerService(userRepository));
                        taskTimerUI.ShowTimerMenu(userData);
>>>>>>> b3b76a3436b33e93b526882a19c21ad0ba046dbd
                        break;
                    case "Generate Report":
                        GenerateReport(userData);
                        Console.ReadKey();
                        break;
                    case "Export CSV":
                        DataExporter.ExportData(userData);
                        Console.ReadKey();
                        break;
                    case "Logout":
                        AnsiConsole.MarkupLine("[bold red]Logging Out...[/]");
                        Thread.Sleep(1000);
                        return;
                }
            }
        }

        private void DisplayDashboard(UserData userData)
        {
            var recentTasks = userData.Projects
                .SelectMany(p => p.TaskItems)
                .OrderByDescending(t => t.EndTime)
                .Take(2)
                .ToList();

            var runningTask = userData.Projects
                .SelectMany(p => p.TaskItems)
                .FirstOrDefault(t => t.TaskStatus == TaskState.Ongoing);

            AnsiConsole.MarkupLine("[bold yellow]--- Dashboard ---[/]");

            var recentTasksTable = new Table();
            recentTasksTable.AddColumn("Task");
            recentTasksTable.AddColumn("Description");
            recentTasksTable.AddColumn("End Time");

            AnsiConsole.MarkupLine("[bold green]Recent Activities:[/]");
            foreach (var task in recentTasks)
            {
                recentTasksTable.AddRow(task.TaskName ?? "N/A", task.TaskDescription ?? "N/A", task.EndTime.ToString());
            }
            if (recentTasksTable.Rows.Count > 0)
            {
                AnsiConsole.Write(recentTasksTable);
            }
            else
            {
                AnsiConsole.MarkupLine("[bold red]No recent task.[/]");
            }

            if (runningTask != null)
            {
                var runningTaskTable = new Table();
                runningTaskTable.AddColumn("Task");
                runningTaskTable.AddColumn("Description");
                runningTaskTable.AddColumn("Start Time");

                runningTaskTable.AddRow(runningTask.TaskName ?? "N/A", runningTask.TaskDescription ?? "N/A", runningTask.StartTime.ToString());

                AnsiConsole.MarkupLine("[bold green]Currently Running Task:[/]");
                AnsiConsole.Write(runningTaskTable);
            }
            else
            {
                AnsiConsole.MarkupLine("[bold red]No task is currently running.[/]");
            }
        }

        public void GenerateReport(UserData userData)
        {
            Console.Clear();
            while (true)
            {
                string reportType = SelectReportType();
                if (reportType == "Exit")
                {
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    break;
                }

                var tasksWithProjects = GetTasksForReportType(userData, reportType);
                if (tasksWithProjects.Count == 0)
                {
                    AnsiConsole.MarkupLine("[red]No tasks found for this period![/]");
                    Console.WriteLine("Press any key to continue...");
                    return;
                }

                string choice = SelectFilterSortMenu();
                if (choice == "Back")
                {
                    continue;
                }

                Console.Clear();

                if (choice == "Filter")
                {
                    tasksWithProjects = FilterTasks(tasksWithProjects);
                }
                else if (choice == "Sort")
                {
                    tasksWithProjects = SortTasks(tasksWithProjects);
                }

                DisplayReport(tasksWithProjects, reportType);

                if (PromptExit())
                {
                    Console.WriteLine("Press any key to continue...");
                    break;
                }
                else
                {
                    Console.Clear();
                }
            }
        }

        private string SelectReportType()
        {
            return AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[blue]Select report type:[/]")
                    .AddChoices("Daily", "Weekly", "Monthly", "Exit"));
        }

        private List<(TaskItem Task, Project Project)> GetTasksForReportType(UserData userData, string reportType)
        {
            DateTime startDate = DateTime.Today;
            DateTime endDate = startDate.AddDays(1);

            if (reportType == "Weekly")
            {
                startDate = GetStartOfWeek();
                endDate = startDate.AddDays(7);
            }
            else if (reportType == "Monthly")
            {
                startDate = GetStartOfMonth();
                endDate = startDate.AddMonths(1);
            }

            var taskList = new List<(TaskItem, Project)>();
            foreach (var project in userData.Projects)
            {
                if (project.TaskItems == null) continue;
                foreach (var task in project.TaskItems)
                {
                    if (task.StartTime >= startDate && task.StartTime < endDate)
                    {
                        taskList.Add((task, project));
                    }
                }
            }
            return taskList;
        }

        private DateTime GetStartOfWeek()
        {
            int diff = (7 + (DateTime.Now.DayOfWeek - DayOfWeek.Monday)) % 7;
            return DateTime.Now.AddDays(-diff).Date;
        }

        private DateTime GetStartOfMonth()
        {
            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        }

        private string SelectFilterSortMenu()
        {
            return AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]Filter, Sort, or Show All:[/]")
                    .AddChoices("Filter", "Sort", "Show All", "Back"));
        }

        private List<(TaskItem Task, Project Project)> FilterTasks(List<(TaskItem Task, Project Project)> tasksWithProjects)
        {
            string filterChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Filter by:[/]")
                    .AddChoices("Project Name", "Task Name", "Task Status", "Back"));

            if (filterChoice == "Back") return tasksWithProjects;

            var filteredTasks = new List<(TaskItem, Project)>();

            if (filterChoice == "Project Name")
            {
                var projectNames = tasksWithProjects
                    .Select(t => t.Project.Name)
                    .Distinct()
                    .ToList();
                string selectedProject = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[yellow]Select a project:[/]")
                        .AddChoices(projectNames));
                filteredTasks = tasksWithProjects
                    .Where(t => t.Project.Name == selectedProject)
                    .ToList();
            }
            else if (filterChoice == "Task Name")
            {
                var taskNames = tasksWithProjects
                    .Select(t => t.Task.TaskName)
                    .Distinct()
                    .ToList();
                string selectedTask = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[yellow]Select a task:[/]")
                        .AddChoices(taskNames));
                filteredTasks = tasksWithProjects
                    .Where(t => t.Task.TaskName == selectedTask)
                    .ToList();
            }
            else if (filterChoice == "Task Status")
            {
                var statuses = Enum.GetNames(typeof(TaskState)).ToList();
                string selectedStatus = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[yellow]Select a task status:[/]")
                        .AddChoices(statuses));
                filteredTasks = tasksWithProjects
                    .Where(t => t.Task.TaskStatus.ToString() == selectedStatus)
                    .ToList();
            }

            return filteredTasks;
        }

        private List<(TaskItem Task, Project Project)> SortTasks(List<(TaskItem Task, Project Project)> tasksWithProjects)
        {
            string sortChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]Sort tasks by:[/]")
                    .AddChoices("Start Time", "End Time", "Duration", "Task Status", "Back"));

            if (sortChoice == "Back") return tasksWithProjects;

            var sortedTasks = new List<(TaskItem, Project)>(tasksWithProjects);

            if (sortChoice == "Start Time")
            {
                sortedTasks.Sort((a, b) => a.Item1.StartTime.CompareTo(b.Item1.StartTime));
            }
            else if (sortChoice == "End Time")
            {
                sortedTasks.Sort((a, b) => a.Item1.EndTime.CompareTo(b.Item1.EndTime));
            }
            else if (sortChoice == "Duration")
            {
                sortedTasks.Sort((a, b) => a.Item1.Duration.CompareTo(b.Item1.Duration));
            }
            else if (sortChoice == "Task Status")
            {
                sortedTasks.Sort((a, b) => string.Compare(a.Item1.TaskStatus.ToString(), b.Item1.TaskStatus.ToString(), StringComparison.Ordinal));
            }

            return sortedTasks;
        }

        private void DisplayReport(List<(TaskItem Task, Project Project)> tasksWithProjects, string reportTitle)
        {
            Console.Clear();

            var table = new ConsoleTable("Project Name", "Task Name", "Description", "Start Time", "End Time", "Duration", "Threshold Time", "Task Status");

            foreach (var (task, project) in tasksWithProjects)
            {
                table.AddRow(
                    project.Name ?? "N/A",
                    task.TaskName ?? "N/A",
                    task.TaskDescription ?? "N/A",
                    task.StartTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    task.EndTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    task.Duration.ToString(),
                    task.ThresholdTime.ToString(),
                    task.TaskStatus.ToString());
            }

            AnsiConsole.MarkupLine($"[bold yellow]{reportTitle} Report[/]");
            AnsiConsole.WriteLine(table.ToString());
        }

        private bool PromptExit()
        {
            string choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[blue]What would you like to do next?[/]")
                    .AddChoices("Exit to Main Menu", "Perform Another Action"));

            return choice == "Exit to Main Menu";
        }

        public void ManageProject(UserData userData)
        {
            while (true)
            {
                Console.Clear();
                AnsiConsole.MarkupLine($"[bold cyan]=== Manage Project Menu ===[/]");
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[grey100]Select an option:[/]")
                        .AddChoices("Create Project", "Delete Project", "Edit Project", "View Projects", "Go To Previous Menu")
                        .HighlightStyle("green")
                );

                switch (choice)
                {
                    case "Create Project":
                        CreateNewProject(userData);
                        break;
                    case "Delete Project":
                        DeleteProject(userData);
                        break;
                    case "Edit Project":
                        EditProject(userData);
                        break;
                    case "View Projects":
                        ViewProject();
                        break;
                    case "Go To Previous Menu":
                        AnsiConsole.MarkupLine("[bold red]Exiting to previous menu...[/]");
                        Thread.Sleep(1000);
                        return;
                }
            }
        }

        public void CreateNewProject(UserData userData)
        {
            Console.Clear();
            AnsiConsole.MarkupLine($"[bold cyan]=== Create Project Menu ===[/]");
            Console.Write("Enter New Project Name:");
            string? projectName = Console.ReadLine();
            bool IsvalidProjectName = false;
            while (!IsvalidProjectName)
            {
                if (string.IsNullOrEmpty(projectName) || string.IsNullOrWhiteSpace(projectName))
                {
                    Console.WriteLine("Input cannot be null, empty, or just spaces. Please try again.");
                    Console.Write("Enter New Project Name:");
                    projectName = Console.ReadLine();
                }
                else if (_projectService.IsProjectNameExist(projectName))
                {
                    Console.WriteLine("Project name already exist. Please try again.");
                    Console.Write("Enter New Project Name:");
                    projectName = Console.ReadLine();
                }
                else if (!projectName.All(char.IsLetter))
                {
                    AnsiConsole.MarkupLine("[bold red]Project should not have only alphabets.[/]");
                    AnsiConsole.MarkupLine("[yellow]Press Enter to continue...[/]");
                    Console.ReadKey();
                    return;
                }
                else
                {
                    IsvalidProjectName = true;
                }
            }
            ProjectCategory projectCategory = AnsiConsole.Prompt(new SelectionPrompt<ProjectCategory>()
                                                           .Title("[grey100]Select task category:[/]")
                                                           .AddChoices(Enum.GetValues<ProjectCategory>())
                                                           .HighlightStyle("green"));
            Project project = new Project(projectName, projectCategory);
            _projectService.AddNewProject(project, userData);
            AnsiConsole.MarkupLine("[bold green]Project added successfully![/]");
            Thread.Sleep(1000);
        }

        public void DeleteProject(UserData userData)
        {
            List<Project>? projects = _projectService.GetUserProjects();

            if (projects == null || projects.Count == 0)
            {
                AnsiConsole.MarkupLine("[bold red]No Projects Added![/]");
                Console.ReadKey();
                return;
            }

            string? projectName = null;
            bool isValidProjectName = false;

            while (!isValidProjectName)
            {
                Console.Clear();
                AnsiConsole.MarkupLine("[bold cyan]=== Delete Project Menu ===[/]");
                AnsiConsole.Markup("[bold yellow]Enter Project Name:[/] ");
                projectName = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(projectName))
                {
                    AnsiConsole.MarkupLine("[bold red]Project Name is not valid![/]");
                    AnsiConsole.MarkupLine("[yellow]Press any key to try again...[/]");
                    Console.ReadKey();
                }
                else
                {
                    isValidProjectName = true;
                }
            }
            Project? filteredProject = null;
            foreach (var project in projects)
            {
                if (!string.IsNullOrEmpty(project.Name) && project.Name.Equals(projectName))
                {
                    filteredProject = project;
                    break;
                }
            }

            if (filteredProject != null)
            {
                Console.Clear();
                AnsiConsole.MarkupLine("[bold cyan]=== Confirm Deletion ===[/]");
                AnsiConsole.MarkupLine($"[bold white]Project Name:[/] [green]{filteredProject.Name}[/]");
                AnsiConsole.MarkupLine($"[bold white]Category:[/] [blue]{filteredProject.Category}[/]");
                AnsiConsole.MarkupLine("\n[yellow]Press [bold]Enter[/] to delete, or [bold]Esc[/] to cancel...[/]");

                while (true)
                {
                    var key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.Enter)
                    {
                        _projectService.DeleteProject(filteredProject, userData);
                        AnsiConsole.MarkupLine("[bold green]Project deleted successfully![/]");
                        break;
                    }
                    else if (key == ConsoleKey.Escape)
                    {
                        AnsiConsole.MarkupLine("[bold yellow]Deletion cancelled![/]");
                        break;
                    }
                }
            }
            else
            {
                AnsiConsole.MarkupLine("[bold red]Project not found![/]");
            }

            AnsiConsole.MarkupLine("\n[yellow]Press any key to continue...[/]");
            Console.ReadKey();
        }
        public void EditProject(UserData userData)
        {
            List<Project>? projects = _projectService.GetUserProjects();

            if (projects == null || projects.Count == 0)
            {
                AnsiConsole.MarkupLine("[bold red]No Projects Added![/]");
                Console.ReadKey();
                return;
            }

            string? projectName = null;
            bool isValidProjectName = false;
            int tries = 3;
            while (!isValidProjectName)
            {
                if (tries <= 0)
                {
                    AnsiConsole.MarkupLine("[bold red]No tries left[/]");
                    AnsiConsole.MarkupLine("[yellow]Press any key to close...[/]");
                    Console.ReadKey();
                    return;
                }
                Console.Clear();
                AnsiConsole.MarkupLine("[bold cyan]=== Edit Project Menu ===[/]");
                AnsiConsole.Markup("[bold yellow]Enter Project Name:[/] ");
                projectName = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(projectName))
                {
                    AnsiConsole.MarkupLine("[bold red]Project Name is not valid![/]");
                    AnsiConsole.MarkupLine("[yellow]Press any key to try again...[/]");
                    Console.ReadKey();
                }
                else
                {
                    isValidProjectName = true;
                }
                tries--;
            }

            Project? filteredProject = null;
            foreach (var project in projects)
            {
                if (!string.IsNullOrEmpty(project.Name) && project.Name.Equals(projectName))
                {
                    filteredProject = project;
                    break;
                }
            }

            if (filteredProject != null)
            {
                Console.Clear();
                AnsiConsole.MarkupLine("[bold cyan]=== Confirm Edit ===[/]");
                AnsiConsole.MarkupLine($"[bold white]Project Name:[/] [green]{filteredProject.Name}[/]");
                AnsiConsole.MarkupLine($"[bold white]Category:[/] [blue]{filteredProject.Category}[/]");
                AnsiConsole.MarkupLine("\n[yellow]Press [bold]Enter[/] to edit, or [bold]Esc[/] to cancel...[/]");

                while (true)
                {
                    var key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.Enter)
                    {
                        Console.Clear();
                        AnsiConsole.MarkupLine("[bold cyan]=== Edit Project ===[/]");
                        AnsiConsole.MarkupLine("[bold white]1. Edit Project Name[/]");
                        AnsiConsole.MarkupLine("[bold white]2. Edit Project Category[/]");
                        string? option = null;
                        bool isValidOption = false;
                        int triesLeft = 3;
                        while (!isValidOption)
                        {
                            if (triesLeft <= 0)
                            {
                                AnsiConsole.MarkupLine("[bold red]No tries left[/]");
                                AnsiConsole.MarkupLine("[yellow]Press any key to close..[/]");
                                Console.ReadKey();
                                return;
                            }
                            AnsiConsole.MarkupLine("[grey100]Select an option (1 or 2):[/]");
                            option = Console.ReadLine();
                            if (string.IsNullOrWhiteSpace(option) || !(option == "1" || option == "2"))
                            {
                                AnsiConsole.MarkupLine("[bold red]Invalid option! Please enter 1 or 2.[/]");
                                AnsiConsole.MarkupLine("[yellow]Press any key to try again...[/]");
                                Console.ReadKey();
                            }
                            else
                            {
                                isValidOption = true;
                            }
                            triesLeft--;
                        }

                        switch (option)
                        {
                            case "1":
                                string? newProjectName = null;
                                bool isValidName = false;
                                int tryLeft = 3;
                                while (!isValidName)
                                {
                                    if (tryLeft <= 0)
                                    {
                                        AnsiConsole.MarkupLine("[bold red]No tries left![/]");
                                        AnsiConsole.MarkupLine("[yellow]Press any key to exit...[/]");
                                        Console.ReadKey();
                                        return;
                                    }
                                    AnsiConsole.Markup("[bold yellow]Enter new project name:[/] ");
                                    newProjectName = Console.ReadLine();
                                    if (_projectService.IsProjectNameExist(newProjectName))
                                    {
                                        AnsiConsole.MarkupLine("[bold red]Project Name already exist![/]");
                                        AnsiConsole.MarkupLine("[yellow]Press any key to exit...[/]");
                                        Console.ReadKey();
                                        return;
                                    }
                                    else if (string.IsNullOrWhiteSpace(newProjectName))
                                    {
                                        AnsiConsole.MarkupLine("[bold red]Project Name is not valid![/]");
                                        AnsiConsole.MarkupLine("[yellow]Press any key to try again...[/]");
                                        Console.ReadKey();
                                    }
                                    else
                                    {
                                        isValidName = true;
                                    }
                                    tryLeft--;
                                }

                                filteredProject.Name = newProjectName;
                                break;

                            case "2":
                                ProjectCategory projectCategory = AnsiConsole.Prompt(new SelectionPrompt<ProjectCategory>()
                                                             .Title("[grey100]Select new task category:[/]")
                                                             .AddChoices(Enum.GetValues<ProjectCategory>())
                                                             .HighlightStyle("green"));
                                filteredProject.Category = projectCategory;
                                break;
                        }
                        _projectService.SaveUserData(userData);
                        AnsiConsole.MarkupLine("[bold green]Project edited successfully![/]");
                        break;
                    }
                    else if (key == ConsoleKey.Escape)
                    {
                        AnsiConsole.MarkupLine("[bold yellow]Edit cancelled![/]");
                        break;
                    }
                }
            }
            else
            {
                AnsiConsole.MarkupLine("[bold red]Project not found![/]");
            }

            AnsiConsole.MarkupLine("\n[yellow]Press any key to continue...[/]");
            Console.ReadKey();
        }

        public void ViewProject()
        {
            var projects = _projectService.GetUserProjects();
            if (projects == null || projects.Count == 0)
            {
                AnsiConsole.MarkupLine("[bold red] No Projects Added ! .[/]");
                Console.ReadKey();
                return;
            }
            else
            {
                ConsoleTable table = new ConsoleTable("Project", "Category");
                foreach (var project in projects)
                {
                    table.AddRow(project.Name, project.Category);
                }
                table.Write();
            }
            AnsiConsole.MarkupLine("[yellow]Press Enter to continue...[/]");
            Console.ReadKey();
        }

        public void ManageTaskMenu(UserData userData)
        {
            Console.Clear();
            AnsiConsole.MarkupLine($"[bold cyan]=== Manage Task Menu ===[/]");
            var project = SelectProject(userData);
            if (project == null)
            {
                AnsiConsole.MarkupLine("[bold red]No project selected. Returning to previous menu...[/]");
                Thread.Sleep(1000);
                return;
            }
            TaskService _taskService = new TaskService(userData, project, new UserRepository());
            while (true)
            {
                Console.Clear();
                AnsiConsole.MarkupLine($"[bold cyan]=== Manage Task Menu ===[/]");
                var choice = AnsiConsole.Prompt(
                  new SelectionPrompt<string>()
                      .Title("[grey100]Select an option:[/]")
                      .AddChoices("Create Task", "Delete Task", "Edit Task", "View Tasks", "Go To Previous Menu")
                      .HighlightStyle("green"));
                switch (choice)
                {
                    case "Create Task":
                        CreateNewTask(_taskService);
                        break;
                    case "Delete Task":
                        DeleteTask(_taskService);
                        break;
                    case "Edit Task":
                        EditTask(_taskService);
                        break;
                    case "View Tasks":
                        ViewTasks(_taskService);
                        break;
                    case "Go To Previous Menu":
                        AnsiConsole.MarkupLine("[bold red]Exiting to previous menu...[/]");
                        Thread.Sleep(1000);
                        return;
                }
            }
        }
        private Project? SelectProject(UserData userData)
        {
            if (userData.Projects == null || userData.Projects.Count == 0)
            {
                AnsiConsole.MarkupLine("[bold red]No projects available! Please create a project first.[/]");
                Thread.Sleep(1000);
                return null;
            }

            var projectName = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[grey100]Select a project:[/]")
                    .AddChoices(userData.Projects.Select(p => p.Name ?? "Unnamed Project"))
                    .HighlightStyle("green")
            );

            return userData.Projects.FirstOrDefault(p => p.Name == projectName);
        }

        public void CreateNewTask(TaskService taskService)
        {
            Console.Clear();
            string taskName;
            int tries = 2;
            while (true)
            {
                Console.Clear();
                AnsiConsole.MarkupLine("[bold cyan]=== Create New Task ===[/]");
                taskName = AnsiConsole.Ask<string>("[green]Enter task name:[/] ");
                if (tries <= 0)
                {
                    AnsiConsole.MarkupLine("[bold red]No tries Left !.[/]");
                    AnsiConsole.MarkupLine("[yellow]Press Enter to continue...[/]");
                    Console.ReadKey();
                    return;
                }
                if (string.IsNullOrEmpty(taskName))
                {
                    AnsiConsole.MarkupLine("[bold red]Please enter a vaild name.[/]");
                }
                else if (!taskName.All(char.IsLetter))
                {
                    AnsiConsole.MarkupLine("[bold red]Taskname should not have only alphabets.[/]");
                    AnsiConsole.MarkupLine("[yellow]Press Enter to continue...[/]");
                    Console.ReadKey();
                }
                else if (!taskService.IsTaskNameUnique(taskName))
                {
                    AnsiConsole.MarkupLine("[bold red]Task name already exists! Please enter a unique name.[/]");
                    AnsiConsole.MarkupLine("[yellow]Press Enter to continue...[/]");
                    Console.ReadKey();
                }
                else
                {
                    break;
                }
                tries--;
            }
            string description = AnsiConsole.Ask<string>("[green]Enter task description:[/] ");
            TimeSpan thresholdTime = default;
            bool flag = false;
            int maxTries = 3;
            for (int attempts = 0; attempts < maxTries; attempts++)
            {
                Console.Write("Enter threshold time (hh:mm:ss) or Enter Zero to not set threshold: ");
                string? userInput = Console.ReadLine();

                if (TimeSpan.TryParse(userInput, out thresholdTime))
                {
                    flag = true;
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid input!");
                }

                if (attempts == maxTries - 1)
                {
                    Console.WriteLine("Maximum number of attempts reached. Exiting...");
                    Thread.Sleep(1000);
                    return;
                }
            }

            if (!flag)
            {
                Console.WriteLine("Exiting as input is invalid!");
                Thread.Sleep(1000);
                return;
            }
            taskService.AddTask(new TaskItem
            {
                TaskName = taskName,
                TaskDescription = description,
                TaskStatus = TaskState.NotStarted,
                ThresholdTime = thresholdTime
            });
            AnsiConsole.MarkupLine("[bold green]Task created successfully![/]");
            Thread.Sleep(1000);
        }

        public void DeleteTask(TaskService taskService)
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold cyan]=== Delete Task ===[/]");
            var tasks = taskService.GetTasks();
            if (tasks == null || tasks.Count == 0)
            {
                AnsiConsole.MarkupLine("[bold red]No tasks available![/]");
                Thread.Sleep(1000);
                return;
            }
            string taskName = AnsiConsole.Ask<string>("[green]Enter the task name to delete:[/] ");
            var task = tasks.FirstOrDefault(t => t.TaskName.Equals(taskName, StringComparison.OrdinalIgnoreCase));

            if (task == null)
            {
                AnsiConsole.MarkupLine("[bold red]Task not found![/]");
                Thread.Sleep(1000);
                return;
            }
            Console.Clear();
            AnsiConsole.MarkupLine("[bold cyan]=== Task Details ===[/]");
            AnsiConsole.MarkupLine($"[bold white]Task Name:[/] [green]{task.TaskName}[/]");
            AnsiConsole.MarkupLine($"[bold white]Description:[/] [yellow]{task.TaskDescription}[/]");
            AnsiConsole.MarkupLine($"[bold white]Status:[/] [blue]{task.TaskStatus}[/]");
            AnsiConsole.MarkupLine($"[bold white]Start Time:[/] [magenta]{task.StartTime}[/]");
            AnsiConsole.MarkupLine($"[bold white]End Time:[/] [magenta]{task.EndTime}[/]");
            AnsiConsole.MarkupLine($"[bold white]Duration:[/] [cyan]{task.Duration}[/]");
            AnsiConsole.MarkupLine($"[bold white]Threshold Time:[/] [magenta]{task.ThresholdTime}[/]");

            AnsiConsole.MarkupLine("\n[yellow]Press [bold]Enter[/] to delete, or [bold]Esc[/] to cancel...[/]");

            while (true)
            {
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Enter)
                {
                    taskService.DeleteTask(taskName);
                    AnsiConsole.MarkupLine("[bold green]Task deleted successfully![/]");
                    break;
                }
                else if (key == ConsoleKey.Escape)
                {
                    AnsiConsole.MarkupLine("[bold yellow]Deletion cancelled![/]");
                    break;
                }
            }

            AnsiConsole.MarkupLine("\n[yellow]Press any key to continue...[/]");
            Console.ReadKey();
        }


        public void EditTask(TaskService taskService)
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold cyan]=== Edit Task ===[/]");

            var tasks = taskService.GetTasks();
            if (tasks == null || tasks.Count == 0)
            {
                AnsiConsole.MarkupLine("[bold red]No tasks available![/]");
                Thread.Sleep(1000);
                return;
            }
            string taskName = AnsiConsole.Ask<string>("[green]Enter the task name to edit:[/] ");
            var selectedTask = tasks.FirstOrDefault(t => t.TaskName == taskName);
            if (selectedTask == null)
            {
                AnsiConsole.MarkupLine("[bold red]Task not found![/]");
                Thread.Sleep(1000);
                return;
            }
            Console.Clear();
            AnsiConsole.MarkupLine("[bold cyan]=== Task Details ===[/]");

            var table = new ConsoleTable("Property", "Value");
            table.AddRow("Task Name", selectedTask.TaskName)
                 .AddRow("Description", selectedTask.TaskDescription ?? "N/A")
                 .AddRow("Status", selectedTask.TaskStatus)
                 .AddRow("Start Time", selectedTask.StartTime.ToString("yyyy-MM-dd HH:mm:ss"))
                 .AddRow("End Time", selectedTask.EndTime.ToString("yyyy-MM-dd HH:mm:ss"))
                 .AddRow("Duration", selectedTask.Duration.ToString(@"hh\:mm\:ss"))
                 .AddRow("Threshold Time", selectedTask.ThresholdTime.ToString(@"hh\:mm\:ss"));
            table.Write();

            AnsiConsole.MarkupLine("\n[yellow]Press [bold]Enter[/] to edit, or [bold]Esc[/] to cancel...[/]");
            while (true)
            {
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Enter) break;
                if (key == ConsoleKey.Escape)
                {
                    AnsiConsole.MarkupLine("[bold yellow]Editing cancelled![/]");
                    return;
                }
            }
            while (true)
            {
                Console.Clear();
                AnsiConsole.MarkupLine("[bold cyan]=== Select Property to Edit ===[/]");

                var propertyChoice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[grey100]Which property do you want to edit?[/]")
                        .AddChoices("Task Name", "Description", "Status", "Start Time", "End Time", "Threshold Time", "Done Editing")
                        .HighlightStyle("green")
                );

                if (propertyChoice == "Done Editing") break;

                switch (propertyChoice)
                {
                    case "Task Name":
                        string newName = null;
                        newName = AnsiConsole.Ask<string>("[green]Enter new task name:[/] ", selectedTask.TaskName);
                        int tries = 3;
                        while (!taskService.IsTaskNameUnique(newName) && tries > 0)
                        {
                            Console.WriteLine("Project Name already exist!");
                            newName = AnsiConsole.Ask<string>("[green]Enter new task name:[/] ", selectedTask.TaskName);
                            tries--;
                        }
                        if (tries <= 0)
                        {
                            AnsiConsole.MarkupLine("[bold red]No tries left![/]");

                        }
                        else
                        {
                            selectedTask.TaskName = newName;
                            taskService.UpdateTask(selectedTask);
                            AnsiConsole.MarkupLine("[bold green]Task updated successfully![/]");
                        }
                        break;
                    case "Description":
                        selectedTask.TaskDescription = AnsiConsole.Ask<string>("[green]Enter new description:[/] ", selectedTask.TaskDescription);
                        taskService.UpdateTask(selectedTask);
                        AnsiConsole.MarkupLine("[bold green]Task updated successfully![/]");
                        break;
                    case "Status":
                        selectedTask.TaskStatus = AnsiConsole.Prompt(new SelectionPrompt<TaskState>()
                                                            .Title("[grey100]Select new task status:[/]")
                                                            .AddChoices(Enum.GetValues<TaskState>())
                                                            .HighlightStyle("green"));
                        taskService.UpdateTask(selectedTask);
                        AnsiConsole.MarkupLine("[bold green]Task updated successfully![/]");
                        break;
                    case "Start Time":
                        selectedTask.StartTime = AnsiConsole.Ask<DateTime>("[green]Enter new start time (yyyy-MM-dd HH:mm:ss):[/] ", selectedTask.StartTime);
                        taskService.UpdateTask(selectedTask);
                        AnsiConsole.MarkupLine("[bold green]Task updated successfully![/]");
                        break;
                    case "End Time":
                        selectedTask.EndTime = AnsiConsole.Ask<DateTime>("[green]Enter new end time (yyyy-MM-dd HH:mm:ss):[/] ", selectedTask.EndTime);
                        selectedTask.Duration = selectedTask.EndTime - selectedTask.StartTime;
                        taskService.UpdateTask(selectedTask);
                        AnsiConsole.MarkupLine("[bold green]Task updated successfully![/]");
                        break;
                    case "Threshold Time":
                        selectedTask.ThresholdTime = AnsiConsole.Ask<TimeSpan>("[green]Enter new threshold time (hh:mm:ss):[/] ", selectedTask.ThresholdTime);
                        taskService.UpdateTask(selectedTask);
                        AnsiConsole.MarkupLine("[bold green]Task updated successfully![/]");
                        break;
                }
                Thread.Sleep(800);
            }
            Thread.Sleep(1000);
        }


        public void ViewTasks(TaskService taskService)
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold cyan]=== View Tasks ===[/]");

            var tasks = taskService.GetTasks();
            if (tasks == null || tasks.Count == 0)
            {
                AnsiConsole.MarkupLine("[bold red]No tasks available![/]");
                Thread.Sleep(1000);
                return;
            }
            var table = new ConsoleTable("Task Name", "Description", "Status", "Start Time", "End Time", "Duration", "Threshold");
            foreach (var task in tasks)
            {
                table.AddRow(
                    task.TaskName,
                    task.TaskDescription ?? "N/A",
                    task.TaskStatus,
                    task.StartTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    task.EndTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    task.Duration.ToString(@"hh\:mm\:ss"),
                    task.ThresholdTime.ToString(@"hh\:mm\:ss")
                );
            }
            table.Write();
            AnsiConsole.MarkupLine("\n[yellow]Press any key to return...[/]");
            Console.ReadKey();
        }

    }
}
