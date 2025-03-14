using Spectre.Console;
using TimerTrackerApp.BusinessLogic;
using TimerTrackerApp.Model;

namespace TimerTrackerApp.Presentation
{
    public class AuthenticationUI
    {
        private readonly AuthenticationService _authenticationService;
        private ApplicationUI _taskUI;
        public AuthenticationUI(AuthenticationService authenticationService, ApplicationUI taskUI)
        {
            _authenticationService = authenticationService;
            _taskUI = taskUI;
        }

        public void ShowAuthenticationMenu()
        {
            while (true)
            {
                Console.Clear();
                AnsiConsole.MarkupLine("[bold cyan]=== Authentication Menu ===[/]");
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[grey100]Select an option:[/]")
                        .AddChoices("Sign Up", "Login", "Exit")
                        .HighlightStyle("green")
                );

                switch (choice)
                {
                    case "Sign Up":
                        SignUp();
                        break;
                    case "Login":
                        UserData? userData = Login();
                        if (userData != null)
                        {
                            _taskUI.ApplicationMenu(userData);
                        }
                        break;
                    case "Exit":
                        AnsiConsole.MarkupLine("[bold red]Exiting application...[/]");
                        Environment.Exit(0);
                        break;
                }
            }
        }

        private void SignUp()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold cyan]=== Sign Up ===[/]");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write("Enter a username:");
            Console.ResetColor();
            string? username = Console.ReadLine();

            if (string.IsNullOrEmpty(username) || string.IsNullOrWhiteSpace(username) || username.Contains(" "))
            {
                AnsiConsole.MarkupLine("[bold red]Username should not have spaces or be empty.[/]");
                AnsiConsole.MarkupLine("[yellow]Press Enter to continue...[/]");
                Console.ReadKey();
                return;
            }
            else if (!username.All(char.IsLetter))
            {
                AnsiConsole.MarkupLine("[bold red]Username should not have only alphabets.[/]");
                AnsiConsole.MarkupLine("[yellow]Press Enter to continue...[/]");
                Console.ReadKey();
                return;
            }
            else if (!_authenticationService.CheckUserName(username))
            {
                AnsiConsole.MarkupLine("[bold red]Sign-up failed. Username may already exist.[/]");
                AnsiConsole.MarkupLine("[yellow]Press Enter to continue...[/]");
                Console.ReadKey();
                return;
            }

            string password = AnsiConsole.Prompt(new TextPrompt<string>("[green]Enter Strong password:[/] ").Secret());
            int passWordTries = 2;

            while (!_authenticationService.IsPasswordValid(password))
            {
                if (passWordTries > 0)
                {
                    Console.WriteLine("Enter Strong Password");
                    Console.WriteLine("Must be 6 Character long");
                    Console.WriteLine("Contain Upper case, Lower case, Number, Special Character");
                    password = AnsiConsole.Prompt(new TextPrompt<string>("[green]Enter String password:[/] ").Secret());
                    passWordTries--;
                }

                else
                {
                    AnsiConsole.MarkupLine("[bold red]Sign-up failed. As No tires left to set password.[/]");
                    AnsiConsole.MarkupLine("[yellow]Press Enter to continue...[/]");
                    Console.ReadLine();
                    return;
                }
            }

            if (_authenticationService.SignUp(username, password))
            {
                AnsiConsole.MarkupLine("[yellow]Press Enter to continue...[/]");
                Console.ReadLine();
            }

            else
            {
                AnsiConsole.MarkupLine("[bold red]Sign-up failed. Username may already exist.[/]");
            }
        }

        private UserData? Login()
        {
            UserData? userData = null;
            Console.Clear();
            AnsiConsole.MarkupLine("[bold cyan]=== Login ===[/]");
            string username = AnsiConsole.Ask<string>("[green]Enter username:[/] ");

            if (!_authenticationService.CheckLoginUserName(username))
            {
                AnsiConsole.MarkupLine("[yellow]Press Enter to continue...[/]");
                Console.ReadKey();
                return null;
            }

            else
            {
                int passWordTries = 3;

                while (passWordTries > 0)
                {
                    string password = AnsiConsole.Prompt(new TextPrompt<string>("[green]Enter password:[/] ").Secret());
                    userData = _authenticationService.Login(username, password);
                    if (userData == null)
                    {
                        AnsiConsole.MarkupLine("[yellow]Press Enter to continue...[/]");
                        Console.ReadKey();
                    }

                    else
                    {
                        return userData;
                    }
                    passWordTries--;
                }
            }

            AnsiConsole.MarkupLine("[bold red]Login failed. Invalid credentials.[/]");
            AnsiConsole.MarkupLine("[yellow]Press Enter to continue...[/]");
            Console.ReadKey();
            return userData;
        }
    }
}
