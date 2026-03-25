using System;
using System.Collections.Generic;
using System.Text;

namespace Category_Question_Console
{
    internal class MainMenu
    {
        public static string RunMenu(List<string> menu)
        {
            int position = 0;
            ConsoleKey key;

            while (true)
            {
                Console.Clear();

                for (int i = 0; i < menu.Count; i++)
                {
                    if (i == position)
                        Console.WriteLine("> " + menu[i]);
                    else
                        Console.WriteLine("  " + menu[i]);
                }

                key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        position = (position == 0) ? menu.Count - 1 : position - 1;
                        break;

                    case ConsoleKey.DownArrow:
                        position = (position == menu.Count - 1) ? 0 : position + 1;
                        break;

                    case ConsoleKey.Enter:
                        string selected = menu[position];

                        if (selected.Equals("Logout", StringComparison.OrdinalIgnoreCase) ||
                            selected.Equals("Exit", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.Clear();
                            Console.WriteLine($"Подтвердить {selected}? (Y/N)");
                            var confirm = Console.ReadKey(true).Key;

                            if (confirm == ConsoleKey.Y || confirm == ConsoleKey.Enter)
                            {
                                Console.Clear();
                                return selected;
                            }
                        }

                        else if (selected.Equals("Добавить Категорию", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine("Добавить");
                            Console.ReadKey();
                            //AddCategory
                        }
                        else if (selected.Equals("Изменить категорию", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine("Изменить");
                            Console.ReadKey();
                            //ChangeCategory
                        }
                        else if (selected.Equals("Удалить категорию", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine("Удалить");
                            Console.ReadKey();
                            //DeleteCategory
                        }
                        else if (selected.Equals("Сохранить категорию", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine("Сохранить");
                            Console.ReadKey();
                            //SaveCategory
                        }
                        else if (selected.Equals("Загрузить категорию", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine("Загрузить");
                            Console.ReadKey();
                            //LoadCategory
                        }
                        else if (selected.Equals("Выбрать категорию", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine("Выбрать");
                            Console.ReadKey();
                            //ChooseCategory
                        }

                        break;
                }
            }
        }

        public static string UserMenu()
        {
            List<string> menu = new List<string>()
            {
                "Выбрать категорию",
                "Загрузить категорию",
                "Logout",
                "Exit"
            };

            return RunMenu(menu);
        }

        public static string AdminMenu()
        {
            List<string> menu = new List<string>()
            {
                "Добавить категорию",
                "Изменить категорию",
                "Удалить категорию",
                "Сохранить категорию",
                "Загрузить категорию",
                "Logout",
                "Exit"
            };

            return RunMenu(menu);
        }
    }
}
