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

        public void ShowQuestions()
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
            string fileName = $"{Name}.json";
            string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(fileName, json);
            Console.WriteLine($"Категория сохранена в {fileName}");
        }
    }
}