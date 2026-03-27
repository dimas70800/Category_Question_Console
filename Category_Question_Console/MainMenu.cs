using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Category_Question_Console
{
    internal class MainMenu
    {
        private static List<Category> categories = new List<Category>();

        public static string AdminMenu(User currentUser)
        {
            var menu = new List<string>
            {
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

        public static string UserMenu(User currentUser)
        {
            var menu = new List<string>
            {
                "Выбрать категорию",
                "Загрузить категории",
                "Мой результат",
                "Logout",
                "Exit"
            };
            return RunMenu(menu, currentUser);
        }

        private static string RunMenu(List<string> menu, User currentUser)
        {
            LoadCategories();
            int position = 0;
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Меню | {currentUser.Login} | Очки: {currentUser.Score}\n");

                for (int i = 0; i < menu.Count; i++)
                    Console.WriteLine((i == position ? "> " : "  ") + menu[i]);

                var key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.UpArrow) position = (position == 0) ? menu.Count - 1 : position - 1;
                if (key == ConsoleKey.DownArrow) position = (position == menu.Count - 1) ? 0 : position + 1;
                if (key == ConsoleKey.Enter)
                {
                    string choice = menu[position];
                    if (choice == "Logout" || choice == "Exit")
                    {
                        SaveUsers();
                        SaveUserScores();
                        Console.Clear();
                        return choice;
                    }

                    HandleChoice(choice, currentUser);
                }
            }
        }

        private static void HandleChoice(string choice, User user)
        {
            Console.Clear();
            switch (choice)
            {
                case "Добавить категорию":
                    var cat = new Category();
                    Console.Write("Название категории: ");
                    cat.Name = Console.ReadLine();
                    cat.AddQuestion();
                    categories.Add(cat);
                    break;

                case "Добавить вопрос в категорию":
                    ShowCategories();
                    if (categories.Count > 0)
                    {
                        Console.Write("Выберите категорию (номер): ");
                        if (int.TryParse(Console.ReadLine(), out int catIndex) && catIndex > 0 && catIndex <= categories.Count)
                            categories[catIndex - 1].AddQuestion();
                    }
                    break;

                case "Редактировать вопрос":
                    EditQuestionFlow();
                    break;

                case "Удалить вопрос":
                    DeleteQuestionFlow();
                    break;

                case "Сохранить категории":
                    foreach (var c in categories)
                        c.SaveToFile();
                    break;

                case "Загрузить категории":
                    LoadCategories();
                    break;

                case "Выбрать категорию":
                    UserSelectCategory(user);
                    break;

                case "Мой результат":
                    Console.WriteLine($"Ваш текущий результат: {user.Score} баллов");
                    break;
            }
            Console.WriteLine("\nНажмите любую клавишу...");
            Console.ReadKey();
        }

        private static void EditQuestionFlow()
        {
            if (categories.Count == 0)
            {
                Console.WriteLine("Нет доступных категорий.");
                return;
            }

            ShowCategories();
            Console.Write("Выберите категорию (номер): ");
            if (!int.TryParse(Console.ReadLine(), out int catIndex) || catIndex < 1 || catIndex > categories.Count)
                return;

            var category = categories[catIndex - 1];
            if (category.IsEmpty())
            {
                Console.WriteLine("В этой категории нет вопросов.");
                return;
            }

            category.ShowQuestionsWithDetails();
            Console.Write("Выберите вопрос для редактирования (номер): ");
            if (!int.TryParse(Console.ReadLine(), out int qIndex) || qIndex < 1 || qIndex > category.Questions.Count)
                return;

            category.EditQuestion(qIndex - 1);
        }

        private static void DeleteQuestionFlow()
        {
            if (categories.Count == 0)
            {
                Console.WriteLine("Нет доступных категорий.");
                return;
            }

            ShowCategories();
            Console.Write("Выберите категорию (номер): ");
            if (!int.TryParse(Console.ReadLine(), out int catIndex) || catIndex < 1 || catIndex > categories.Count)
                return;

            var category = categories[catIndex - 1];
            if (category.IsEmpty())
            {
                Console.WriteLine("В этой категории нет вопросов.");
                return;
            }

            category.ShowQuestionsWithDetails();
            Console.Write("Выберите вопрос для удаления (номер): ");
            if (!int.TryParse(Console.ReadLine(), out int qIndex) || qIndex < 1 || qIndex > category.Questions.Count)
                return;

            category.DeleteQuestion(qIndex - 1);
        }

        private static void ShowCategories()
        {
            if (categories.Count == 0)
            {
                Console.WriteLine("Нет доступных категорий.");
                return;
            }
            for (int i = 0; i < categories.Count; i++)
                Console.WriteLine($"{i + 1}. {categories[i].Name} ({categories[i].Questions.Count} вопросов)");
        }

        private static void LoadCategories()
        {
            Category.CreateBaseCategories();

            if (!Directory.Exists("Categories"))
                Directory.CreateDirectory("Categories");

            var files = Directory.GetFiles("Categories", "*.json").ToArray();

            categories.Clear();
            foreach (var file in files)
            {
                try
                {
                    string json = File.ReadAllText(file);
                    var cat = JsonSerializer.Deserialize<Category>(json);
                    if (cat != null && !string.IsNullOrEmpty(cat.Name))
                        categories.Add(cat);
                }
                catch { }
            }
            Console.WriteLine($"Загружено {categories.Count} категорий.");
        }

        private static void SaveUsers()
        {
            var users = LoadTested();
            string json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("users.json", json);
        }

        private static void SaveUserScores()
        {
            var users = LoadTested();
            foreach (var category in categories)
            {
                category.SaveToFile();
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
                users.Add(new User { Login = "user", Password = "1234", Role = "Tested", Score = 0 });
                users.Add(new User { Login = "adm", Password = "1234", Role = "Admin", Score = 0 });
                return users;
            }
        }

        private static void UpdateUserScore(User currentUser)
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

        private static void UserSelectCategory(User user)
        {
            if (categories.Count == 0)
            {
                Console.WriteLine("Нет доступных категорий.");
                return;
            }

            ShowCategories();
            Console.Write("\nВыберите категорию: ");
            if (!int.TryParse(Console.ReadLine(), out int catNum) || catNum < 1 || catNum > categories.Count)
                return;

            var selectedCategory = categories[catNum - 1];

            if (selectedCategory.IsEmpty())
            {
                Console.WriteLine("В этой категории нет вопросов.");
                categories.Remove(selectedCategory);
                string filePath = Path.Combine("Categories", $"{selectedCategory.Name}.json");
                if (File.Exists(filePath))
                    File.Delete(filePath);
                return;
            }

            selectedCategory.ShowQuestions();
            Console.Write("\nВыберите вопрос: ");
            if (!int.TryParse(Console.ReadLine(), out int qNum) || qNum < 1 || qNum > selectedCategory.Questions.Count)
                return;

            var question = selectedCategory.GetQuestion(qNum - 1);

            Console.WriteLine("\n" + new string('-', 40));
            Console.WriteLine($"Вопрос: {question.Text}");
            Console.WriteLine(new string('-', 40));
            for (int i = 0; i < question.Options.Count; i++)
                Console.WriteLine($"{i + 1}. {question.Options[i]}");
            Console.WriteLine(new string('-', 40));
            Console.Write("Выберите ответ (введите номер варианта): ");

            if (int.TryParse(Console.ReadLine(), out int answer) && answer >= 1 && answer <= question.Options.Count && answer - 1 == question.CorrectOptionIndex)
            {
                user.Score += question.Points;
                Console.WriteLine($"\nПравильно! +{question.Points} баллов");
                Console.WriteLine($"Ваш текущий счет: {user.Score} баллов");
                UpdateUserScore(user);
                selectedCategory.RemoveQuestion(qNum - 1);

                if (selectedCategory.IsEmpty())
                {
                    Console.WriteLine($"\nКатегория '{selectedCategory.Name}' полностью пройдена и удалена.");
                    categories.Remove(selectedCategory);
                    string filePath = Path.Combine("Categories", $"{selectedCategory.Name}.json");
                    if (File.Exists(filePath))
                        File.Delete(filePath);
                }
                else
                {
                    selectedCategory.SaveToFile();
                }
            }
            else
            {
                Console.WriteLine($"\nНеверно");
            }
        }
    }
}