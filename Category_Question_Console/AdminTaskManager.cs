using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Category_Question_Console
{
    internal static class AdminTaskManager
    {
        private static List<TaskItem> tasks = new List<TaskItem>();
        private static readonly string TasksFile = "admin_tasks.json";

        public static void Initialize()
        {
            LoadTasks();
            if (tasks.Count == 0)
            {
                CreateDefaultTasks();
            }
        }

        private static void CreateDefaultTasks()
        {
            tasks = new List<TaskItem>
            {
                new TaskItem
                {
                    Id = 1,
                    Title = "Добавление темы",
                    Category = "theme",
                    Description = "Создание новой категории вопросов",
                    SubTasks = new List<SubTask>
                    {
                        new SubTask { Id = 1, Title = "Ввести название темы" },
                        new SubTask { Id = 2, Title = "Добавить описание" },
                        new SubTask { Id = 3, Title = "Сохранить категорию" }
                    }
                },
                new TaskItem
                {
                    Id = 2,
                    Title = "Редактирование темы",
                    Category = "theme",
                    Description = "Изменение существующей категории"
                },
                new TaskItem
                {
                    Id = 3,
                    Title = "Редактирование вопроса",
                    Category = "question",
                    Description = "Изменение текста, вариантов ответа или стоимости вопроса"
                },
                new TaskItem
                {
                    Id = 4,
                    Title = "Удаление вопроса",
                    Category = "question",
                    Description = "Удаление вопроса из категории"
                },
                new TaskItem
                {
                    Id = 5,
                    Title = "Добавление вопроса",
                    Category = "question",
                    Description = "Создание нового вопроса в категории"
                },
                new TaskItem
                {
                    Id = 6,
                    Title = "Удаление темы",
                    Category = "theme",
                    Description = "Удаление категории и всех её вопросов"
                }
            };
            SaveTasks();
        }

        public static void ShowAllTasks()
        {
            Console.Clear();
            Console.WriteLine("=== РЕЖИМ АДМИНИСТРАТОРА ===\n");

            int position = 0;
            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("=== ЗАДАЧИ АДМИНИСТРАТОРА ===\n");

                for (int i = 0; i < tasks.Count; i++)
                {
                    string prefix = (i == position) ? "> " : "  ";
                    string status = tasks[i].IsCompleted ? "✓" : "○";
                    string badge = tasks[i].Category == "theme" ? "[ТЕМА]" : "[ВОПРОС]";

                    Console.WriteLine($"{prefix}{status} {tasks[i].Title} {badge}");

                    if (i == position && tasks[i].SubTasks.Count > 0)
                    {
                        Console.WriteLine($"    Подзадачи ({tasks[i].SubTasks.Count}/3):");
                        foreach (var sub in tasks[i].SubTasks)
                        {
                            string subStatus = sub.IsCompleted ? "✓" : "○";
                            Console.WriteLine($"      {subStatus} {sub.Title}");
                        }
                        Console.WriteLine("    [+ Создать подзадачу]");
                    }
                }

                Console.WriteLine("\n↑↓ - навигация | Enter - выбрать | A - добавить | D - удалить | E - выйти");

                var key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        position = (position == 0) ? tasks.Count - 1 : position - 1;
                        break;
                    case ConsoleKey.DownArrow:
                        position = (position == tasks.Count - 1) ? 0 : position + 1;
                        break;
                    case ConsoleKey.Enter:
                        ExecuteTask(tasks[position]);
                        break;
                    case ConsoleKey.A:
                        AddNewTask();
                        break;
                    case ConsoleKey.D:
                        DeleteTask(position);
                        break;
                    case ConsoleKey.E:
                        exit = true;
                        break;
                }
            }
        }

        private static void ExecuteTask(TaskItem task)
        {
            Console.Clear();
            Console.WriteLine($"=== {task.Title.ToUpper()} ===\n");
            Console.WriteLine($"Описание: {task.Description}\n");

            switch (task.Title)
            {
                case "Добавление темы":
                    AddThemeTask();
                    break;
                case "Редактирование темы":
                    EditThemeTask();
                    break;
                case "Редактирование вопроса":
                    EditQuestionTask();
                    break;
                case "Удаление вопроса":
                    DeleteQuestionTask();
                    break;
                case "Добавление вопроса":
                    AddQuestionTask();
                    break;
                case "Удаление темы":
                    DeleteThemeTask();
                    break;
            }

            task.IsCompleted = true;
            SaveTasks();

            Console.WriteLine("\n✓ Задача выполнена!");
            Console.WriteLine("Нажмите любую клавишу...");
            Console.ReadKey();
        }

        // 1. ДОБАВЛЕНИЕ ТЕМЫ
        private static void AddThemeTask()
        {
            Console.Write("Название новой темы: ");
            string themeName = Console.ReadLine();

            Console.Write("Описание темы: ");
            string description = Console.ReadLine();

            var newCategory = new Category
            {
                Name = themeName,
                Questions = new List<Question>()
            };

            Console.Write("Добавить первый вопрос сейчас? (да/нет): ");
            if (Console.ReadLine().ToLower() == "да")
            {
                newCategory.AddQuestion();
            }

            newCategory.SaveToFile();
            Console.WriteLine($"\n✓ Тема '{themeName}' успешно создана!");
        }

        // 2. РЕДАКТИРОВАНИЕ ТЕМЫ
        private static void EditThemeTask()
        {
            var categories = LoadCategories();
            if (categories.Count == 0)
            {
                Console.WriteLine("Нет доступных тем!");
                return;
            }

            Console.WriteLine("Выберите тему для редактирования:");
            for (int i = 0; i < categories.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {categories[i].Name} ({categories[i].Questions.Count} вопросов)");
            }

            Console.Write("\nНомер темы: ");
            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= categories.Count)
            {
                var category = categories[index - 1];

                Console.Write($"\nНовое название (текущее: {category.Name}): ");
                string newName = Console.ReadLine();
                if (!string.IsNullOrEmpty(newName))
                {
                    string oldFile = Path.Combine("Categories", $"{category.Name}.json");
                    category.Name = newName;
                    category.SaveToFile();
                    if (File.Exists(oldFile)) File.Delete(oldFile);
                }

                Console.WriteLine("✓ Тема обновлена!");
            }
        }

        // 3. РЕДАКТИРОВАНИЕ ВОПРОСА
        private static void EditQuestionTask()
        {
            var categories = LoadCategories();
            if (categories.Count == 0)
            {
                Console.WriteLine("Нет доступных тем!");
                return;
            }

            Console.WriteLine("Выберите тему:");
            for (int i = 0; i < categories.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {categories[i].Name}");
            }

            Console.Write("\nНомер темы: ");
            if (int.TryParse(Console.ReadLine(), out int catIndex) && catIndex > 0 && catIndex <= categories.Count)
            {
                var category = categories[catIndex - 1];

                if (category.IsEmpty())
                {
                    Console.WriteLine("В этой теме нет вопросов!");
                    return;
                }

                category.ShowQuestionsWithDetails();
                Console.Write("\nНомер вопроса для редактирования: ");

                if (int.TryParse(Console.ReadLine(), out int qIndex) && qIndex > 0 && qIndex <= category.Questions.Count)
                {
                    category.EditQuestion(qIndex - 1);
                    category.SaveToFile();
                }
            }
        }

        // 4. УДАЛЕНИЕ ВОПРОСА
        private static void DeleteQuestionTask()
        {
            var categories = LoadCategories();
            if (categories.Count == 0)
            {
                Console.WriteLine("Нет доступных тем!");
                return;
            }

            Console.WriteLine("Выберите тему:");
            for (int i = 0; i < categories.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {categories[i].Name}");
            }

            Console.Write("\nНомер темы: ");
            if (int.TryParse(Console.ReadLine(), out int catIndex) && catIndex > 0 && catIndex <= categories.Count)
            {
                var category = categories[catIndex - 1];

                if (category.IsEmpty())
                {
                    Console.WriteLine("В этой теме нет вопросов!");
                    return;
                }

                category.ShowQuestionsWithDetails();
                Console.Write("\nНомер вопроса для удаления: ");

                if (int.TryParse(Console.ReadLine(), out int qIndex) && qIndex > 0 && qIndex <= category.Questions.Count)
                {
                    category.DeleteQuestion(qIndex - 1);
                    category.SaveToFile();
                }
            }
        }

        // 5. ДОБАВЛЕНИЕ ВОПРОСА
        private static void AddQuestionTask()
        {
            var categories = LoadCategories();
            if (categories.Count == 0)
            {
                Console.WriteLine("Нет доступных тем!");
                return;
            }

            Console.WriteLine("Выберите тему:");
            for (int i = 0; i < categories.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {categories[i].Name}");
            }

            Console.Write("\nНомер темы: ");
            if (int.TryParse(Console.ReadLine(), out int catIndex) && catIndex > 0 && catIndex <= categories.Count)
            {
                var category = categories[catIndex - 1];
                category.AddQuestion();
                category.SaveToFile();
            }
        }

        // 6. УДАЛЕНИЕ ТЕМЫ
        private static void DeleteThemeTask()
        {
            var categories = LoadCategories();
            if (categories.Count == 0)
            {
                Console.WriteLine("Нет доступных тем!");
                return;
            }

            Console.WriteLine("Выберите тему для удаления:");
            for (int i = 0; i < categories.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {categories[i].Name} ({categories[i].Questions.Count} вопросов)");
            }

            Console.Write("\nНомер темы: ");
            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= categories.Count)
            {
                var category = categories[index - 1];

                Console.Write($"\n⚠️ Вы уверены, что хотите удалить тему '{category.Name}' со всеми вопросами? (да/нет): ");
                if (Console.ReadLine().ToLower() == "да")
                {
                    string filePath = Path.Combine("Categories", $"{category.Name}.json");
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                        Console.WriteLine($"✓ Тема '{category.Name}' успешно удалена!");
                    }
                }
            }
        }

        private static List<Category> LoadCategories()
        {
            var categories = new List<Category>();
            if (!Directory.Exists("Categories")) return categories;

            var files = Directory.GetFiles("Categories", "*.json");
            foreach (var file in files)
            {
                try
                {
                    string json = File.ReadAllText(file);
                    var cat = JsonSerializer.Deserialize<Category>(json);
                    if (cat != null) categories.Add(cat);
                }
                catch { }
            }
            return categories;
        }

        private static void AddNewTask()
        {
            Console.Write("Название новой задачи: ");
            string title = Console.ReadLine();

            Console.Write("Описание: ");
            string desc = Console.ReadLine();

            Console.Write("Категория (theme/question): ");
            string category = Console.ReadLine();

            tasks.Add(new TaskItem
            {
                Id = tasks.Count + 1,
                Title = title,
                Description = desc,
                Category = category
            });

            SaveTasks();
            Console.WriteLine("✓ Задача добавлена!");
        }

        private static void DeleteTask(int index)
        {
            if (index >= 0 && index < tasks.Count)
            {
                tasks.RemoveAt(index);
                SaveTasks();
                Console.WriteLine("✓ Задача удалена!");
            }
        }

        private static void LoadTasks()
        {
            if (File.Exists(TasksFile))
            {
                try
                {
                    string json = File.ReadAllText(TasksFile);
                    tasks = JsonSerializer.Deserialize<List<TaskItem>>(json) ?? new List<TaskItem>();
                }
                catch { tasks = new List<TaskItem>(); }
            }
        }

        private static void SaveTasks()
        {
            string json = JsonSerializer.Serialize(tasks, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(TasksFile, json);
        }
    }
}