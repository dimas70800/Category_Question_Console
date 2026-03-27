using Category_Question_Console;
using System.IO;

class Program
{
    static void Main()
    {
        Directory.SetCurrentDirectory("..\\..\\..");
        if (!Directory.Exists("Categories"))
            Directory.CreateDirectory("Categories");

        // Инициализация задач администратора
        AdminTaskManager.Initialize();

        EntryAndExit.RunApp();
    }
}