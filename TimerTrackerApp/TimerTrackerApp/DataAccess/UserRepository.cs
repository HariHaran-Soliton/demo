using System.Text.Json;
using TimerTrackerApp.Model;

namespace TimerTrackerApp.DataAccess
{
    public class UserRepository
    {
        private static string GetSpecialFolderPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "TimeTrackerApp");
        }

        public bool UserExists(string userName)
        {
            string folderPath = GetSpecialFolderPath();
            string filePath = Path.Combine(folderPath, $"{userName}.json");
            return File.Exists(filePath);
        }

        public void SaveUser(UserData userData)
        {
            string folderPath = GetSpecialFolderPath();
            Directory.CreateDirectory(folderPath);
            string filePath = Path.Combine(folderPath, $"{userData.User.UserName}.json");

            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonData = JsonSerializer.Serialize(userData, options);
            File.WriteAllText(filePath, jsonData);
        }

        public UserData? GetUser(string userName)
        {
            string folderPath = GetSpecialFolderPath();
            string filePath = Path.Combine(folderPath, $"{userName}.json");

            if (!File.Exists(filePath))
                return null;

            string jsonData = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<UserData>(jsonData);
        }
    }
}

