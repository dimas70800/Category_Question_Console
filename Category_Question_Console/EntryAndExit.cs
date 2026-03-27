// EntryAndExit.cs
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace Category_Question_Console
{
    internal class EntryAndExit
    {
        public static void RunApp()
        {
            while (true)
            {
                var users = LoadTested();
                User currentUser = LoginUser(users);
                if (currentUser == null)
                {
                    Console.WriteLine("Неправильный логин или пароль!");
                    Console.WriteLine();
                    Console.WriteLine("Для выхода нажмите 0");
                    var keyExit = Console.ReadKey();
                    if (keyExit.Key == ConsoleKey.D0)
                    {
                        Environment.Exit(0);
                    }
                    Console.Clear();
                    continue;
                }

                string result;

                if (currentUser.Role.Equals("admin", StringComparison.OrdinalIgnoreCase))
                {
                    result = MainMenu.AdminMenu(currentUser);
                }
                else
                {
                    result = MainMenu.UserMenu(currentUser);
                }

                SaveUserScore(currentUser);

                if (result.Equals("Logout", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (result.Equals("Exit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
            }
        }


        // В методе AdminMenu добавить:
        public static string AdminMenu(User currentUser)
        {
            var menu = new List<string>
            {
                "Управление задачами",        // НОВОЕ!
                "Добавить категорию",
                "Добавить вопрос в категорию",
                "Редактировать вопрос",
                "Удалить вопрос",
                "Сохранить категории",
                "Загрузить категории",
                "Logout",
                "Exit"
            };
            return RunMenu(menu, currentUser);
        }

        private static void SaveUserScore(User currentUser)
        {
            var users = LoadTested();
            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].Login == currentUser.Login)
                {
                    users[i].Score = currentUser.Score;
                    break;
                }
            }
            string json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("users.json", json);
        }

        private static User? LoginUser(List<User> users)
        {
            Console.WriteLine("Логин:");
            string login = Console.ReadLine();

            Console.WriteLine("Пароль:");
            string password = Console.ReadLine();

            foreach (var user in users)
            {
                if (user.Login == login.Trim() && user.Password == password.Trim())
                {
                    return user;
                }
            }
            return null;
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
                users.Add(new User { Login = "user", Password = "1234", Role = "Tested" });
                users.Add(new User { Login = "adm", Password = "1234", Role = "Admin" });
                string json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText("users.json", json);
                return users;
            }
        }
    }
}