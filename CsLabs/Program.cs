using System;

namespace CsLabs
{
    class Program
    {
        public static void Main()
        {
            Console.Write("Количество игроков: ");
            int players = int.Parse(Console.ReadLine());

            Console.Write("Оперативная память (ГБ): ");
            int memory = int.Parse(Console.ReadLine());

            Console.Write("Сервер публичный? (true/false): ");
            bool isPublic = bool.Parse(Console.ReadLine());

            Console.Write("Установлен пароль? (true/false): ");
                bool hasPassword = bool.Parse(Console.ReadLine());

            if (players <= 0)
            {
                Console.WriteLine("Запуск невозможен: количество игроков должно быть больше нуля.");
            }
            else if (memory < 2)
            {
                Console.WriteLine(
                    "Запуск невозможен: серверу недостаточно оперативной памяти.");
            }
            else if (isPublic && hasPassword)
            {
                Console.WriteLine(
                    "Запуск возможен с предупреждением: публичный сервер защищён паролем.");
            }
            else if (players > 100 && memory < 8)
            {
                Console.WriteLine(
                    "Запуск возможен с предупреждением: для такого количества игроков рекомендуется больше оперативной памяти.");
            }
            else
            {
                Console.WriteLine("Сервер готов к запуску.");
            }
        }
    }
}