using System;
using System.Globalization;
using System.IO;

class Program
{
    static void Main()
    {
        string[] lines = File.ReadAllLines("event_server.log");
        Console.WriteLine(BuildReport(lines));
    }

    public static string BuildReport(string[] allLines)
    {
        string[] lines = GetEventLines(allLines);
        if (lines.Length == 0)
            return "Событие не найдено";

        string eventName = Between(lines[0], "Событие началось: ", null);
        DateTime date = DateTime.ParseExact(lines[0].Substring(0, 23),
            "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);

        string winner = Between(FindLine(lines, " объявлены победителями события"),
            "] ", " объявлены победителями");
        if (winner == null)
            winner = "Не найден";

        string pointsText = Between(FindLine(lines, winner + " получили ", " очков события"),
            winner + " получили ", " очков");
        int points = pointsText == null ? 0 : int.Parse(pointsText);

        string itemPhrase = winner + " получили ивентовый предмет:";
        string item = Between(FindLine(lines, itemPhrase), itemPhrase, null);
        if (item == null)
            item = "Не найден";

        string consolationLine = FindLine(lines, "утешительную награду:");
        string consolation = consolationLine == null
            ? "Утешительная награда: не найдена"
            : "Утешительная награда "
              + Between(consolationLine, "] ", " получили утешительную награду:") + ": "
              + Between(consolationLine, "утешительную награду:", " очков") + " очков";

        return "# Итоги события: " + eventName + "\n\n"
             + "Дата: " + date.ToString("dd.MM.yyyy") + "\n"
             + "Победитель: " + winner + "\n"
             + "Очки победителя: " + points + "\n"
             + "Ивентовый предмет: " + item + "\n"
             + consolation + "\n"
             + "Предупреждений во время события: " + CountLines(lines, "[Warning]") + "\n"
             + "Ошибок во время события: " + CountLines(lines, "[Error]");
    }
    static string[] GetEventLines(string[] lines)
    {
        int start = -1;
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Contains("Событие началось:"))
            {
                start = i;
                break;
            }
        }

        if (start == -1)
            return new string[0];

        string eventName = Between(lines[start], "Событие началось: ", null);
        string closeText = "Событие \"" + eventName + "\" закрыто";

        int end = lines.Length - 1;
        for (int i = start; i < lines.Length; i++)
        {
            if (lines[i].Contains(closeText))
            {
                end = i;
                break;
            }
        }

        string[] result = new string[end - start + 1];
        for (int i = 0; i < result.Length; i++)
            result[i] = lines[start + i];

        return result;
    }
    static string FindLine(string[] lines, params string[] parts)
    {
        foreach (string line in lines)
        {
            bool hasAll = true;
            foreach (string part in parts)
            {
                if (!line.Contains(part))
                    hasAll = false;
            }

            if (hasAll)
                return line;
        }

        return null;
    }

    static string Between(string line, string start, string end)
    {
        if (line == null)
            return null;

        int from = line.IndexOf(start) + start.Length;
        int to = end == null ? line.Length : line.IndexOf(end, from);
        return line.Substring(from, to - from).Trim();
    }
    static int CountLines(string[] lines, string text)
    {
        int count = 0;
        foreach (string line in lines)
        {
            if (line.Contains(text))
                count++;
        }
        return count;
    }
}