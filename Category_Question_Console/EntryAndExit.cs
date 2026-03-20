using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Category_Question_Console
{
    internal class EntryAndExit
    {
        public static void RunApp()
        {
            while (true)
            {
                var tested = LoadTested();
            }
        }

        private static List<User> LoadTested()
        {
            List<User> users = new List<User>();
            if (File.Exists("users.json"))
            {
                var json = File.ReadAllText("users.json");
                users = JsonSerializer.Deserialize<List<User>>(json);
                return users;
            }
            else
            {
                users.Add(new User { Login = "user", Password = "1234", Role = "Tested"});
                users.Add(new User { Login = "adm", Password = "1234", Role = "Admin"});
                string json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText("users.json", json);
                return users;
            }
        }
    }
}
