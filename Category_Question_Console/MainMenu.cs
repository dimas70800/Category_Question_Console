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
                "Мой результат",
                "Logout",
                "Exit"
            };
            return RunMenu(menu, currentUser);
        }

        private static string RunMenu(List<string> menu, User currentUser)
        {
            LoadCategories();
            LoadUserProgress(currentUser);
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
                        SaveUserProgress(currentUser);
                        SaveUsers();
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
                    LoadUserProgress(user);
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

        private static void LoadUserProgress(User user)
        {
            string progressFile = $"progress_{user.Login}.json";

            if (File.Exists(progressFile))
            {
                try
                {
                    string json = File.ReadAllText(progressFile);
                    var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

                    if (data != null && data.ContainsKey("Score"))
                    {
                        user.Score = JsonSerializer.Deserialize<int>(data["Score"].ToString());
                    }

                    if (data != null && data.ContainsKey("ResolvedQuestions"))
                    {
                        var resolvedQuestions = JsonSerializer.Deserialize<Dictionary<string, List<int>>>(data["ResolvedQuestions"].ToString());

                        foreach (var category in categories)
                        {
                            if (resolvedQuestions.ContainsKey(category.Name))
                            {
                                foreach (var index in resolvedQuestions[category.Name])
                                {
                                    if (index >= 0 && index < category.Questions.Count)
                                    {
                                        category.Questions[index].IsResolved = true;
                                    }
                                }
                            }
                        }
                    }
                }
                catch { }
            }
        }

        private static void SaveUserProgress(User user)
        {
            string progressFile = $"progress_{user.Login}.json";

            var resolvedQuestions = new Dictionary<string, List<int>>();

            foreach (var category in categories)
            {
                var resolvedIndices = new List<int>();
                for (int i = 0; i < category.Questions.Count; i++)
                {
                    if (category.Questions[i].IsResolved)
                    {
                        resolvedIndices.Add(i);
                    }
                }
                if (resolvedIndices.Count > 0)
                {
                    resolvedQuestions[category.Name] = resolvedIndices;
                }
            }

            var data = new Dictionary<string, object>
            {
                { "Score", user.Score },
                { "ResolvedQuestions", resolvedQuestions }
            };

            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(progressFile, json);
        }

        private static void SaveUsers()
        {
            var users = LoadTested();
            string json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("users.json", json);
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
                users.Add(new User { Login = "user2", Password = "1234", Role = "Tested", Score = 0 });
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

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Выбор категории | {user.Login} | Очки: {user.Score}\n");

                // Показываем только категории с нерешенными вопросами
                var availableCategories = new List<Category>();
                for (int i = 0; i < categories.Count; i++)
                {
                    if (categories[i].Questions.Any(q => !q.IsResolved))
                    {
                        availableCategories.Add(categories[i]);
                        int unresolvedCount = categories[i].Questions.Count(q => !q.IsResolved);
                        Console.WriteLine($"{availableCategories.Count}. {categories[i].Name} ({unresolvedCount} доступных вопросов)");
                    }
                }

                if (availableCategories.Count == 0)
                {
                    Console.WriteLine("Все вопросы пройдены! Поздравляем!");
                    Console.WriteLine("\n0. Вернуться в главное меню");
                    Console.Write("\nВыберите действие: ");
                    if (Console.ReadLine() == "0")
                        break;
                    continue;
                }

                Console.WriteLine("\n0. Вернуться в главное меню");
                Console.Write("\nВыберите категорию: ");

                if (!int.TryParse(Console.ReadLine(), out int catNum))
                    continue;

                if (catNum == 0)
                    break;

                if (catNum < 1 || catNum > availableCategories.Count)
                {
                    Console.WriteLine("Неверный номер категории!");
                    Console.ReadKey();
                    continue;
                }

                var selectedCategory = availableCategories[catNum - 1];
                UserSelectQuestion(user, selectedCategory);
            }
        }

        private static void UserSelectQuestion(User user, Category category)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Категория: {category.Name} | {user.Login} | Очки: {user.Score}\n");

                // Показываем только нерешенные вопросы
                var availableQuestions = new List<Question>();
                for (int i = 0; i < category.Questions.Count; i++)
                {
                    if (!category.Questions[i].IsResolved)
                    {
                        availableQuestions.Add(category.Questions[i]);
                        Console.WriteLine($"{availableQuestions.Count}. {category.Questions[i].Text} ({category.Questions[i].Points} баллов)");
                    }
                }

                if (availableQuestions.Count == 0)
                {
                    Console.WriteLine("В этой категории больше нет доступных вопросов!");
                    Console.WriteLine("\nНажмите любую клавишу для возврата...");
                    Console.ReadKey();
                    break;
                }

                Console.WriteLine("\n0. Вернуться к выбору категорий");
                Console.Write("\nВыберите вопрос: ");

                if (!int.TryParse(Console.ReadLine(), out int qNum))
                    continue;

                if (qNum == 0)
                    break;

                if (qNum < 1 || qNum > availableQuestions.Count)
                {
                    Console.WriteLine("Неверный номер вопроса!");
                    Console.ReadKey();
                    continue;
                }

                // Находим оригинальный индекс вопроса
                int originalIndex = -1;
                for (int i = 0, count = 0; i < category.Questions.Count; i++)
                {
                    if (!category.Questions[i].IsResolved)
                    {
                        count++;
                        if (count == qNum)
                        {
                            originalIndex = i;
                            break;
                        }
                    }
                }

                if (originalIndex != -1)
                {
                    AskQuestion(user, category, originalIndex);
                    SaveUserProgress(user);
                }
            }
        }

        private static void AskQuestion(User user, Category category, int questionIndex)
        {
            var question = category.Questions[questionIndex];

            Console.Clear();
            Console.WriteLine($"Категория: {category.Name} | {user.Login} | Очки: {user.Score}\n");
            Console.WriteLine(new string('-', 40));
            Console.WriteLine($"Вопрос: {question.Text}");
            Console.WriteLine(new string('-', 40));
            for (int i = 0; i < question.Options.Count; i++)
                Console.WriteLine($"{i + 1}. {question.Options[i]}");
            Console.WriteLine(new string('-', 40));
            Console.Write("Выберите ответ (введите номер варианта): ");

            if (int.TryParse(Console.ReadLine(), out int answer) && answer >= 1 && answer <= question.Options.Count)
            {
                if (answer - 1 == question.CorrectOptionIndex)
                {
                    user.Score += question.Points;
                    Console.WriteLine($"\n✓ Правильно! +{question.Points} баллов");
                    Console.WriteLine($"Ваш текущий счет: {user.Score} баллов");
                    question.IsResolved = true;
                    UpdateUserScore(user);
                }
                else
                {
                    Console.WriteLine($"\n✗ Неверно! Правильный ответ: {question.Options[question.CorrectOptionIndex]}");
                }
            }
            else
            {
                Console.WriteLine("\nНеверный ввод!");
            }

            Console.WriteLine("\nНажмите любую клавишу, чтобы продолжить...");
            Console.ReadKey();
        }
    }
}