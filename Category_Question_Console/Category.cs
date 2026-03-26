using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Category_Question_Console
{
    internal class Category
    {
        public string Name { get; set; }
        public List<Question> Questions { get; set; } = new List<Question>();

        public bool IsEmpty() => Questions.Count == 0;

        public void AddQuestion()
        {
            var q = new Question();
            Console.Write("Введите текст вопроса: ");
            q.Text = Console.ReadLine();

            Console.Write("Количество вариантов ответа: ");
            int count = int.Parse(Console.ReadLine() ?? "2");

            for (int i = 0; i < count; i++)
            {
                Console.Write($"Вариант {i + 1}: ");
                q.Options.Add(Console.ReadLine());
            }

            Console.Write("Номер правильного ответа (1-based): ");
            q.CorrectOptionIndex = int.Parse(Console.ReadLine() ?? "1") - 1;

            Console.Write("Стоимость вопроса (баллы): ");
            q.Points = int.Parse(Console.ReadLine() ?? "10");

            Questions.Add(q);
            Console.WriteLine("Вопрос добавлен!");
        }

        public void EditQuestion(int index)
        {
            if (index < 0 || index >= Questions.Count)
            {
                Console.WriteLine("Вопрос не найден!");
                return;
            }

            var q = Questions[index];
            Console.WriteLine($"\nРедактирование вопроса #{index + 1}:");
            Console.WriteLine($"Текущий текст: {q.Text}");
            Console.Write("Новый текст вопроса (оставьте пустым, чтобы не менять): ");
            string newText = Console.ReadLine();
            if (!string.IsNullOrEmpty(newText)) q.Text = newText;

            Console.WriteLine($"Текущая стоимость: {q.Points}");
            Console.Write("Новая стоимость (число, оставьте пустым для пропуска): ");
            string pointsInput = Console.ReadLine();
            if (int.TryParse(pointsInput, out int newPoints)) q.Points = newPoints;

            Console.WriteLine($"Текущий правильный ответ (1-based): {q.CorrectOptionIndex + 1}");
            Console.Write("Номер правильного ответа (оставьте пустым для пропуска): ");
            string correctInput = Console.ReadLine();
            if (int.TryParse(correctInput, out int newCorrect) && newCorrect >= 1 && newCorrect <= q.Options.Count)
                q.CorrectOptionIndex = newCorrect - 1;

            Console.WriteLine("\nВарианты ответов:");
            for (int i = 0; i < q.Options.Count; i++)
                Console.WriteLine($"{i + 1}. {q.Options[i]}");

            Console.Write("Редактировать варианты? (да/нет): ");
            if (Console.ReadLine()?.ToLower() == "да")
            {
                q.Options.Clear();
                Console.Write("Количество вариантов ответа: ");
                int count = int.Parse(Console.ReadLine() ?? "2");
                for (int i = 0; i < count; i++)
                {
                    Console.Write($"Вариант {i + 1}: ");
                    q.Options.Add(Console.ReadLine());
                }
                Console.Write("Номер правильного ответа (1-based): ");
                q.CorrectOptionIndex = int.Parse(Console.ReadLine() ?? "1") - 1;
            }

            Console.WriteLine("Вопрос отредактирован!");
        }

        public void DeleteQuestion(int index)
        {
            if (index < 0 || index >= Questions.Count)
            {
                Console.WriteLine("Вопрос не найден!");
                return;
            }

            Console.WriteLine($"Вы уверены, что хотите удалить вопрос: \"{Questions[index].Text}\"? (да/нет)");
            if (Console.ReadLine()?.ToLower() == "да")
            {
                Questions.RemoveAt(index);
                Console.WriteLine("Вопрос удален!");
            }
        }

        public void ShowQuestionsWithDetails()
        {
            if (IsEmpty())
            {
                Console.WriteLine("В категории нет вопросов.");
                return;
            }

            Console.WriteLine($"\nКатегория: {Name}");
            for (int i = 0; i < Questions.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {Questions[i].Text} ({Questions[i].Points} баллов)");
            }
        }

        public void ShowQuestions()
        {
            ShowQuestionsWithDetails();
        }

        public Question GetQuestion(int index)
        {
            if (index < 0 || index >= Questions.Count) return null;
            return Questions[index];
        }

        public void RemoveQuestion(int index)
        {
            if (index >= 0 && index < Questions.Count)
                Questions.RemoveAt(index);
        }

        public void SaveToFile()
        {
            if (!Directory.Exists("Categories"))
                Directory.CreateDirectory("Categories");

            string fileName = Path.Combine("Categories", $"{Name}.json");
            string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(fileName, json);
            Console.WriteLine($"Категория сохранена в {fileName}");
        }

        // Category.cs - исправленный метод CreateBaseCategories()
        public static void CreateBaseCategories()
        {
            if (!Directory.Exists("Categories"))
                Directory.CreateDirectory("Categories");

            var categoriesToCreate = new List<Category>();
            var existingFiles = Directory.GetFiles("Categories", "*.json").Select(Path.GetFileNameWithoutExtension).ToList();

            var mathCategory = new Category { Name = "Математика" };
            if (!existingFiles.Contains("Математика"))
            {
                for (int i = 1; i <= 10; i++)
                {
                    mathCategory.Questions.Add(new Question
                    {
                        Text = $"Сколько будет 2 + {i}?",
                        Options = new List<string> { $"{2 + i}", $"{2 + i + 1}", $"{2 + i - 1}", $"{i}" },
                        CorrectOptionIndex = 0,
                        Points = 10
                    });
                }
                categoriesToCreate.Add(mathCategory);
            }

            var geographyCategory = new Category { Name = "География" };
            if (!existingFiles.Contains("География"))
            {
                geographyCategory.Questions.Add(new Question { Text = "Столица России?", Options = new List<string> { "Москва", "Санкт-Петербург", "Новосибирск", "Казань" }, CorrectOptionIndex = 0, Points = 10 });
                geographyCategory.Questions.Add(new Question { Text = "Самая длинная река в мире?", Options = new List<string> { "Амазонка", "Нил", "Янцзы", "Миссисипи" }, CorrectOptionIndex = 0, Points = 10 });
                geographyCategory.Questions.Add(new Question { Text = "Самая высокая гора?", Options = new List<string> { "Эверест", "К2", "Канченджанга", "Лхоцзе" }, CorrectOptionIndex = 0, Points = 10 });
                geographyCategory.Questions.Add(new Question { Text = "Самое большое озеро?", Options = new List<string> { "Каспийское море", "Верхнее", "Виктория", "Гурон" }, CorrectOptionIndex = 0, Points = 10 });
                geographyCategory.Questions.Add(new Question { Text = "Самая маленькая страна?", Options = new List<string> { "Ватикан", "Монако", "Сан-Марино", "Лихтенштейн" }, CorrectOptionIndex = 0, Points = 10 });
                geographyCategory.Questions.Add(new Question { Text = "Самый густонаселенный город?", Options = new List<string> { "Токио", "Дели", "Шанхай", "Сан-Паулу" }, CorrectOptionIndex = 0, Points = 10 });
                geographyCategory.Questions.Add(new Question { Text = "Самая большая пустыня?", Options = new List<string> { "Сахара", "Антарктическая", "Аравийская", "Гоби" }, CorrectOptionIndex = 0, Points = 10 });
                geographyCategory.Questions.Add(new Question { Text = "Самый большой океан?", Options = new List<string> { "Тихий", "Атлантический", "Индийский", "Северный Ледовитый" }, CorrectOptionIndex = 0, Points = 10 });
                geographyCategory.Questions.Add(new Question { Text = "Самая высокая точка Европы?", Options = new List<string> { "Эльбрус", "Монблан", "Дыхтау", "Казбек" }, CorrectOptionIndex = 0, Points = 10 });
                geographyCategory.Questions.Add(new Question { Text = "Самый большой материк?", Options = new List<string> { "Евразия", "Африка", "Северная Америка", "Южная Америка" }, CorrectOptionIndex = 0, Points = 10 });
                categoriesToCreate.Add(geographyCategory);
            }

            var historyCategory = new Category { Name = "История" };
            if (!existingFiles.Contains("История"))
            {
                historyCategory.Questions.Add(new Question { Text = "В каком году была Вторая мировая война?", Options = new List<string> { "1939-1945", "1914-1918", "1941-1945", "1939-1944" }, CorrectOptionIndex = 0, Points = 10 });
                historyCategory.Questions.Add(new Question { Text = "Кто открыл Америку?", Options = new List<string> { "Колумб", "Магеллан", "Васко да Гама", "Кук" }, CorrectOptionIndex = 0, Points = 10 });
                historyCategory.Questions.Add(new Question { Text = "Первый полет человека в космос?", Options = new List<string> { "1961", "1957", "1965", "1969" }, CorrectOptionIndex = 0, Points = 10 });
                historyCategory.Questions.Add(new Question { Text = "Кто написал 'Войну и мир'?", Options = new List<string> { "Толстой", "Достоевский", "Пушкин", "Чехов" }, CorrectOptionIndex = 0, Points = 10 });
                historyCategory.Questions.Add(new Question { Text = "Где произошла Куликовская битва?", Options = new List<string> { "1380", "1240", "1480", "1612" }, CorrectOptionIndex = 0, Points = 10 });
                historyCategory.Questions.Add(new Question { Text = "Кто был первым президентом США?", Options = new List<string> { "Вашингтон", "Джефферсон", "Линкольн", "Адамс" }, CorrectOptionIndex = 0, Points = 10 });
                historyCategory.Questions.Add(new Question { Text = "Год основания Санкт-Петербурга?", Options = new List<string> { "1703", "1712", "1721", "1682" }, CorrectOptionIndex = 0, Points = 10 });
                historyCategory.Questions.Add(new Question { Text = "Кто изобрел телефон?", Options = new List<string> { "Белл", "Эдисон", "Тесла", "Маркони" }, CorrectOptionIndex = 0, Points = 10 });
                historyCategory.Questions.Add(new Question { Text = "Столица Византийской империи?", Options = new List<string> { "Константинополь", "Рим", "Афины", "Александрия" }, CorrectOptionIndex = 0, Points = 10 });
                historyCategory.Questions.Add(new Question { Text = "Год крещения Руси?", Options = new List<string> { "988", "862", "945", "980" }, CorrectOptionIndex = 0, Points = 10 });
                categoriesToCreate.Add(historyCategory);
            }

            foreach (var cat in categoriesToCreate)
            {
                string fileName = Path.Combine("Categories", $"{cat.Name}.json");
                string json = JsonSerializer.Serialize(cat, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(fileName, json);
                Console.WriteLine($"Создана новая категория: {cat.Name}");
            }
        }
    }
}