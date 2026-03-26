using System.Text.Json.Serialization;

namespace Category_Question_Console
{
    internal class User
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public int Score { get; set; } = 0;

        [JsonIgnore]
        public string DisplayInfo => $"{Login} (Score: {Score})";
    }
}