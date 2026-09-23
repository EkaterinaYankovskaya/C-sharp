using System;
using System.Collections.Generic;
using System.IO;

namespace CatAndMouseGame
{
    // Состояния игрока согласно ТЗ
    public enum PlayerState
    {
        NotInGame, // Не в игре (начальная позиция еще не задана)
        Playing,   // Играет
        Winner,    // Победитель
        Loser      // Проигравший
    }

    // Класс Игрока (Кот / Мышь)
    public class Player
    {
        public string Name { get; private set; }
        public int Position { get; private set; }
        public int DistanceTraveled { get; private set; }
        public PlayerState State { get; set; }

        public Player(string name)
        {
            Name = name;
            Position = -1;
            DistanceTraveled = 0;
            State = PlayerState.NotInGame;
        }

        // Перемещение игрока по зацикленному полю из boardSize клеток (от 1 до N)
        public void Move(int step, int boardSize)
        {
            if (State == PlayerState.NotInGame)
            {
                // Если первая команда — задание начальной позиции (1..N)
                int startPos = ((step - 1) % boardSize + boardSize) % boardSize + 1;
                Position = startPos;
                State = PlayerState.Playing;
            }
            else if (State == PlayerState.Playing)
            {
                if (step == 0) return;

                // Увеличение пройденной дистанции на абсолютное значение
                DistanceTraveled += Math.Abs(step);

                // Зацикленный пересчет позиции (1..boardSize)
                int newPos = ((Position - 1 + step) % boardSize + boardSize) % boardSize + 1;
                Position = newPos;
            }
        }
    }

    // Класс игры "Кошки-мышки"
    public class Game
    {
        public int BoardSize { get; private set; }
        public Player Cat { get; private set; }
        public Player Mouse { get; private set; }
        public bool IsGameOver { get; private set; }

        private List<string> logLines;

        public Game(int boardSize)
        {
            BoardSize = boardSize;
            Cat = new Player("Cat");
            Mouse = new Player("Mouse");
            IsGameOver = false;
            logLines = new List<string>();
        }

        // Вычисление текущего расстояния между Котом и Мышью
        public int GetDistance()
        {
            if (Cat.State == PlayerState.NotInGame || Mouse.State == PlayerState.NotInGame)
                return -1;

            return Math.Abs(Cat.Position - Mouse.Position);
        }

        // Обработка одной строки/команды из файла
        public void ProcessCommand(string line)
        {
            if (IsGameOver || string.IsNullOrWhiteSpace(line))
                return;

            string[] parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return;

            string cmd = parts[0].ToUpper();

            if (cmd == "M" && parts.Length > 1)
            {
                int val = int.Parse(parts[1]);
                Mouse.Move(val, BoardSize);
                CheckCatchCondition();
            }
            else if (cmd == "C" && parts.Length > 1)
            {
                int val = int.Parse(parts[1]);
                Cat.Move(val, BoardSize);
                CheckCatchCondition();
            }
            else if (cmd == "P")
            {
                PrintState();
            }
        }

        // Проверка условия поимки мыши
        private void CheckCatchCondition()
        {
            if (Cat.State == PlayerState.Playing && Mouse.State == PlayerState.Playing)
            {
                if (Cat.Position == Mouse.Position)
                {
                    Cat.State = PlayerState.Winner;
                    Mouse.State = PlayerState.Loser;
                    IsGameOver = true;
                }
            }
        }

        // Формирование строки таблицы при команде P
        private void PrintState()
        {
            string catStr = (Cat.State == PlayerState.NotInGame) ? "??" : Cat.Position.ToString();
            string mouseStr = (Mouse.State == PlayerState.NotInGame) ? "??" : Mouse.Position.ToString();
            string distStr;

            if (Cat.State == PlayerState.NotInGame || Mouse.State == PlayerState.NotInGame)
            {
                distStr = "??";
            }
            else
            {
                distStr = GetDistance().ToString();
            }

            // Выравнивание по столбцам: Cat, Mouse, Distance
            logLines.Add($" {catStr,-5} {mouseStr,-5} {distStr}");
        }

        // Сохранение результатов в файл точно по ТЗ
        public void SaveLog(string outputPath)
        {
            using (StreamWriter sw = new StreamWriter(outputPath))
            {
                sw.WriteLine("Cat and Mouse");
                sw.WriteLine();
                sw.WriteLine("Cat   Mouse Distance");
                sw.WriteLine("-------------------");

                foreach (var line in logLines)
                {
                    sw.WriteLine(line);
                }

                sw.WriteLine("-------------------");
                sw.WriteLine();
                sw.WriteLine();
                sw.WriteLine("Distance traveled: Mouse Cat");
                sw.WriteLine($" {Mouse.DistanceTraveled,-14} {Cat.DistanceTraveled}");
                sw.WriteLine();

                if (Mouse.State == PlayerState.Loser)
                {
                    sw.WriteLine($"Mouse caught at: {Cat.Position}");
                }
                else
                {
                    sw.WriteLine("Mouse evaded Cat");
                }
            }
        }
    }

    // Клиентский класс
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Введите номер теста (1, 2 или 3): ");
            string choice = Console.ReadLine()?.Trim();

            if (choice != "1" && choice != "2" && choice != "3")
            {
                Console.WriteLine("Неверный номер! Пожалуйста, введите 1, 2 или 3.");
                return;
            }

            string inputFileName = $"{choice}.ChaseData.txt";
            string outputFileName = $"{choice}.PursuitLog.txt";

            if (!File.Exists(inputFileName))
            {
                Console.WriteLine($"Файл {inputFileName} не найден!");
                return;
            }

            string[] lines = File.ReadAllLines(inputFileName);
            if (lines.Length == 0) return;

            // Первая строка — размер поля N
            int boardSize = int.Parse(lines[0].Trim());

            Game game = new Game(boardSize);

            // Чтение и выполнение команд
            for (int i = 1; i < lines.Length; i++)
            {
                game.ProcessCommand(lines[i]);
                if (game.IsGameOver)
                    break; // Прекращаем чтение при поимке мыши
            }

            // Сохранение выходных данных
            game.SaveLog(outputFileName);

            Console.WriteLine($"\nТест #{choice} успешно обработан!");
            Console.WriteLine($"Результат записан в файл: {outputFileName}");
        }
    }
}