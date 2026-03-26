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
                    if (choice == "Logout" || choice == "Exit"){
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
                    Console.Write("Выберите категорию (номер): ");
                    if (int.TryParse(Console.ReadLine(), out int catIndex) && catIndex > 0 && catIndex <= categories.Count)
                        categories[catIndex - 1].AddQuestion();
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
            var files = Directory.GetFiles(".", "*.json")
                                 .Where(f => !f.Contains("users.json"))
                                 .ToArray();

            foreach (var file in files)
            {
                try
                {
                    string json = File.ReadAllText(file);
                    var cat = JsonSerializer.Deserialize<Category>(json);
                    if (cat != null && categories.All(c => c.Name != cat.Name))
                        categories.Add(cat);
                }
                catch { }
            }
            Console.WriteLine("Категории загружены.");
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
                return;
            }

            selectedCategory.ShowQuestions();
            Console.Write("\nВыберите вопрос: ");
            if (!int.TryParse(Console.ReadLine(), out int qNum) || qNum < 1 || qNum > selectedCategory.Questions.Count)
                return;

            var question = selectedCategory.GetQuestion(qNum - 1);

            for (int i = 0; i < question.Options.Count; i++)
                Console.WriteLine($"{i + 1}. {question.Options[i]}");

            Console.Write("\nВаш ответ: ");
            if (int.TryParse(Console.ReadLine(), out int answer) && answer - 1 == question.CorrectOptionIndex)
            {
                user.Score += question.Points;
                Console.WriteLine($"Правильно! +{question.Points} баллов");
                selectedCategory.RemoveQuestion(qNum - 1);

                if (selectedCategory.IsEmpty())
                {
                    Console.WriteLine($"Категория '{selectedCategory.Name}' полностью пройдена и удалена.");
                    categories.Remove(selectedCategory);
                }
            }
            else
            {
                Console.WriteLine("Неверно.");
            }
        }
    }
}