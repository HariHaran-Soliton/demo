using System.Security.Cryptography;
using System.Text;
using TimerTrackerApp.DataAccess;
using TimerTrackerApp.Model;

namespace TimerTrackerApp.BusinessLogic
{
    public class AuthenticationService
    {
        private readonly UserRepository _userRepository;

        public AuthenticationService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }

        public bool CheckUserName(string userName)
        {
            if (_userRepository.UserExists(userName))
            {
                Console.WriteLine("Username already exists. Please choose another.");
                Thread.Sleep(1000);
                return false;
            }

            else
            {
                return true;
            }
        }

        public bool CheckLoginUserName(string userName)
        {
            if (_userRepository.UserExists(userName))
            {
                return true;
            }

            else
            {
                Console.WriteLine("UserName Not Found.");
                return false;
            }
        }

        public bool IsPasswordValid(string inputString)
        {
            bool isConditionSatisfied = false;

            if (inputString.Length >= 6)
            {
                isConditionSatisfied = true;
            }

            if (isConditionSatisfied)
            {
                isConditionSatisfied = false;
                char[] specialCharacters = { '!', '@', '#', '$', '%', '^', '&', '+', '=', '*', '(', ')', '_', '-', '?', '/', '>', '<', ':', ';', '{', '}', '[', ']', '\\', '|' };
                if (inputString.IndexOfAny(specialCharacters) != -1)
                {
                    isConditionSatisfied = true;
                }
            }

            if (isConditionSatisfied)
            {
                isConditionSatisfied = false;
                if (inputString.Any(c => c >= 'a' && c <= 'z'))
                {
                    isConditionSatisfied = true;
                }
            }

            if (isConditionSatisfied)
            {
                isConditionSatisfied = false;
                if (inputString.Any(c => c >= '0' && c <= '9'))
                {
                    isConditionSatisfied = true;
                }
            }

            if (isConditionSatisfied)
            {
                isConditionSatisfied = false;
                if (inputString.Any(c => c >= 'A' && c <= 'Z'))
                {
                    isConditionSatisfied = true;
                }
            }

            return isConditionSatisfied;
        }

        public bool SignUp(string userName, string password)
        {
            string hashedPassword = HashPassword(password);
            User newUser = new User(userName, hashedPassword);
            newUser.UserId = GenerateUserId();
            UserData userData = new UserData(newUser, new List<Project>());
            _userRepository.SaveUser(userData);
            Console.WriteLine("Sign-up successful! Please log in.");
            Thread.Sleep(1000);
            return true;
        }

        public UserData? Login(string username, string password)
        {
            UserData? userData = _userRepository.GetUser(username);

            if (userData == null)
            {
                Console.WriteLine("User does not exist.");
                Thread.Sleep(1000);
                return null;
            }
            string hashedInputPassword = HashPassword(password);

            if (userData.User.PassWord != hashedInputPassword)
            {
                Console.WriteLine("Incorrect password.");
                Thread.Sleep(1000);
                return null;
            }
            Console.WriteLine($"Login successful! Welcome, {username}.");
            Thread.Sleep(1000);
            return userData;
        }

        public static string GenerateUserId()
        {
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            Random random = new Random();
            int randomNumber = random.Next(1000, 9999);
            long userId = timestamp * 10000 + randomNumber;
            string formattedUserId = string.Format("{0:0000-0000-0000-0000}", userId);
            return formattedUserId;
        }
    }
}
