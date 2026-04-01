using System;
using System.Collections.Generic;

namespace Category_Question_Console
{
    internal class Question
    {
        public string Text { get; set; }
        public List<string> Options { get; set; } = new List<string>();
        public int CorrectOptionIndex { get; set; }
        public int Points { get; set; }
        public bool IsResolved { get; set; } = false;

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