using System;
using System.Collections.Generic;

namespace Category_Question_Console
{
    internal class Question
    {
        public string Text { get; set; } // Сам вопрос
        public List<string> Options { get; set; } = new List<string>(); // Варианты ответов
        public int CorrectOptionIndex { get; set; } // Индекс верного ответа
        public int Points { get; set; } // "Стоимость" вопроса
        public bool IsResolved { get; set; } // Решен ли верно (для итогов)

        public void Edit()
        {
            Console.WriteLine($"Редактирование вопроса: {Text}");
            Console.Write("Новый текст вопроса (оставьте пустым, чтобы не менять): ");
            string input = Console.ReadLine();
            if (!string.IsNullOrEmpty(input)) Text = input;

            Console.Write("Новая стоимость (число): ");
            if (int.TryParse(Console.ReadLine(), out int p)) Points = p;
        }
    }
}