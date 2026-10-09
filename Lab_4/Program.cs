using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
public struct LogEntry
{
    public DateTime Timestamp;
    public string Level;
    public string Category;
    public string Message;
}

public class Program
{
    static void Main()
    {
        string[] lines = File.ReadAllLines("event_server.log");
        LogEntry[] entries = ParseLog(lines);

        Console.WriteLine("Всего записей: " + entries.Length);
        Console.WriteLine();

        Console.WriteLine("Info: " + CountByLevel(entries, "Info"));
        Console.WriteLine("Warning: " + CountByLevel(entries, "Warning"));
        Console.WriteLine("Error: " + CountByLevel(entries, "Error"));
        Console.WriteLine("Fatal: " + CountByLevel(entries, "Fatal"));
        Console.WriteLine();

        DateTime date = new DateTime(2026, 9, 2);
        LogEntry[] byDate = FilterByDate(entries, date);
        Console.WriteLine("Записей за " + date.ToString("dd.MM.yyyy") + ": " + byDate.Length);

        LogEntry[] combat = FilterByCategory(entries, "Combat");
        Console.WriteLine("Записей категории Combat: " + combat.Length);

        LogEntry[] found = Search(entries, "база данных");
        Console.WriteLine("Записей со словами \"база данных\": " + found.Length);
        Console.WriteLine();

        Console.WriteLine("Ошибки:");
        PrintEntries(FilterByLevel(entries, "Error"));
        PrintEntries(FilterByLevel(entries, "Fatal"));
        Console.WriteLine();

        Console.WriteLine("Статус сервера: " + GetServerStatus(entries));

        ExportFiltered("errors.log", FilterByLevel(entries, "Error"));
        Console.WriteLine("Ошибки сохранены в errors.log");
    }

    public static LogEntry[] ParseLog(string[] lines)
    {
        List<LogEntry> result = new List<LogEntry>();

        for (int i = 0; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
            {
                continue;
            }

            result.Add(ParseLine(lines[i]));
        }

        return result.ToArray();
    }
    static LogEntry ParseLine(string line)
    {
        LogEntry entry = new LogEntry();

        entry.Timestamp = DateTime.ParseExact(line.Substring(0, 23),
            "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);

        int firstOpen = line.IndexOf('[');
        int firstClose = line.IndexOf(']', firstOpen);
        int secondOpen = line.IndexOf('[', firstClose);
        int secondClose = line.IndexOf(']', secondOpen);

        entry.Level = line.Substring(firstOpen + 1, firstClose - firstOpen - 1);
        entry.Category = line.Substring(secondOpen + 1, secondClose - secondOpen - 1);
        entry.Message = line.Substring(secondClose + 2);

        return entry;
    }

    public static LogEntry[] FilterByDate(LogEntry[] entries, DateTime date)
    {
        List<LogEntry> result = new List<LogEntry>();

        for (int i = 0; i < entries.Length; i++)
        {
            if (entries[i].Timestamp.Date == date.Date)
            {
                result.Add(entries[i]);
            }
        }

        return result.ToArray();
    }

    public static LogEntry[] FilterByLevel(LogEntry[] entries, string level)
    {
        List<LogEntry> result = new List<LogEntry>();

        for (int i = 0; i < entries.Length; i++)
        {
            if (entries[i].Level == level)
            {
                result.Add(entries[i]);
            }
        }

        return result.ToArray();
    }

    public static LogEntry[] FilterByCategory(LogEntry[] entries, string category)
    {
        List<LogEntry> result = new List<LogEntry>();

        for (int i = 0; i < entries.Length; i++)
        {
            if (entries[i].Category == category)
            {
                result.Add(entries[i]);
            }
        }

        return result.ToArray();
    }

    public static LogEntry[] Search(LogEntry[] entries, string text)
    {
        List<LogEntry> result = new List<LogEntry>();

        for (int i = 0; i < entries.Length; i++)
        {
            if (entries[i].Message.Contains(text, StringComparison.OrdinalIgnoreCase))
            {
                result.Add(entries[i]);
            }
        }

        return result.ToArray();
    }

    public static int CountByLevel(LogEntry[] entries, string level)
    {
        return FilterByLevel(entries, level).Length;
    }

    public static string GetServerStatus(LogEntry[] entries)
    {
        bool hasErrors = false;

        for (int i = 0; i < entries.Length; i++)
        {
            LogEntry entry = entries[i];

            if (entry.Level == "Fatal" && entry.Category == "Server")
            {
                return "КРИТИЧЕСКАЯ ОШИБКА: сервер остановлен";
            }

            if (entry.Level == "Error" || entry.Level == "Fatal")
            {
                hasErrors = true;
            }
        }

        if (hasErrors)
        {
            return "Есть ошибки: требуется проверка";
        }

        return "Сервер работает штатно";
    }

    public static void ExportFiltered(string path, LogEntry[] entries)
    {
        string[] lines = new string[entries.Length];

        for (int i = 0; i < entries.Length; i++)
        {
            LogEntry entry = entries[i];
            lines[i] = entry.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff")
                + " [" + entry.Level + "][" + entry.Category + "] " + entry.Message;
        }

        File.WriteAllLines(path, lines);
    }

    static void PrintEntries(LogEntry[] entries)
    {
        for (int i = 0; i < entries.Length; i++)
        {
            Console.WriteLine("  " + entries[i].Timestamp.ToString("dd.MM HH:mm") + " ["
                + entries[i].Category + "] " + entries[i].Message);
        }
    }
}