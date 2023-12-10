using System;
using System.IO;
using Serilog;
using Serilog.Core;

namespace dotNet_module_16_practice
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Добро пожаловать! Для начала работы укажите путь к отслеживаемой директории:");

            string pathToWatch = Console.ReadLine();
            string logFilePath = "changes_log.txt";

            ConfigureLogger(logFilePath);

            try
            {
                using (var watcher = new FileSystemWatcher())
                {
                    watcher.Path = pathToWatch;

                    watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName;

                    watcher.Created += OnChanged;
                    watcher.Deleted += OnChanged;
                    watcher.Renamed += OnRenamed;

                    watcher.EnableRaisingEvents = true;

                    Console.WriteLine($"Начато отслеживание изменений в директории: {pathToWatch}");
                    Console.WriteLine("Нажмите любую клавишу для остановки отслеживания.");
                    Console.ReadKey();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                Log.Error(ex, "Ошибка при отслеживании директории: {Path}", pathToWatch);
            }
        }

        static void ConfigureLogger(string logFilePath)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        static void OnChanged(object sender, FileSystemEventArgs e)
        {
            Log.Information($"Изменение типа {e.ChangeType}: {e.FullPath}");
        }

        static void OnRenamed(object sender, RenamedEventArgs e)
        {
            Log.Information($"Переименование файла/директории с {e.OldFullPath} на {e.FullPath}");
        }
    }
}
