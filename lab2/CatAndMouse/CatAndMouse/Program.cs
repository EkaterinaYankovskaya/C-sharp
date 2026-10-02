using System;
using System.IO;

namespace CatAndMouseGame
{
    public enum State
    {
        Winner,
        Looser,
        Playing,
        NotInGame
    }

    public class Player
    {
        public string name;              
        public int location;             
        public State state = State.NotInGame; 
        public int distanceTraveled = 0; 

        public Player(string name)
        {
            this.name = name;
            this.location = -1; // не в игре
        }

        public void Move(int steps, int boardSize)
        {
            if (state == State.NotInGame)
            {
                location = ((steps - 1) % boardSize + boardSize) % boardSize + 1;
                state = State.Playing;
            }
            else if (state == State.Playing && steps != 0)
            {
                distanceTraveled += Math.Abs(steps);
                location = ((location - 1 + steps) % boardSize + boardSize) % boardSize + 1;
            }
        }
    }

    // Состояния игры
    public enum GameState
    {
        Start,
        End
    }

    // Класс Game
    public class Game
    {
        public static string InputFile = "1.ChaseData.txt";
        public static string OutFile = "1.PursuitLog.txt";

        public int size; // размер игрового поля
        public Player cat;
        public Player mouse;
        public GameState state;

        public Game(int size)
        {
            this.size = size;
            cat = new Player("Cat");
            mouse = new Player("Mouse");
            state = GameState.Start;
        }

        public void Run()
        {
            if (!File.Exists(InputFile)) return;

            string[] lines = File.ReadAllLines(InputFile);
            if (lines.Length == 0) return;

            // Если в первой строке указан размер поля N, переопределяем его
            if (int.TryParse(lines[0].Trim(), out int parsedSize))
                this.size = parsedSize;

            // Подготовка шапки файла лога
            File.WriteAllText(OutFile, "Cat and Mouse\n\nCat   Mouse Distance\n-------------------\n");

            // Читаем все команды из файла по очереди
            foreach (string line in lines)
            {
                if (state == GameState.End) break;

                string[] parts = line.Trim().Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0) continue;

                char command = parts[0].ToUpper()[0];

                if ((command == 'M' || command == 'C') && parts.Length > 1 && int.TryParse(parts[1], out int steps))
                {
                    DoMoveCommand(command, steps);

                    // Проверка поимки мыши
                    if (cat.state == State.Playing && mouse.state == State.Playing && cat.location == mouse.location)
                    {
                        cat.state = State.Winner;
                        mouse.state = State.Looser;
                        state = GameState.End;
                    }
                }
                else if (command == 'P')
                {
                    DoPrintCommand();
                }
            }

            state = GameState.End;

            // Дописываем подвал и итоги в файл
            string footer = "-------------------\n\n\nDistance traveled: Mouse Cat\n" +
                           $" {mouse.distanceTraveled,-14} {cat.distanceTraveled}\n\n" +
                           (mouse.state == State.Looser ? $"Mouse caught at: {cat.location}\n" : "Mouse evaded Cat\n");

            File.AppendAllText(OutFile, footer);
        }

        private void DoMoveCommand(char command, int steps)
        {
            switch (command)
            {
                case 'M': mouse.Move(steps, size); break;
                case 'C': cat.Move(steps, size); break;
            }
        }

        private void DoPrintCommand()
        {
            string catStr = (cat.state == State.NotInGame) ? "??" : cat.location.ToString();
            string mouseStr = (mouse.state == State.NotInGame) ? "??" : mouse.location.ToString();

            int dist = GetDistance();
            string distStr = (dist == -1) ? "??" : dist.ToString();

            // Сразу дописываем новую строчку состояния в файл
            File.AppendAllText(OutFile, $" {catStr,-5} {mouseStr,-5} {distStr}\n");
        }

        private int GetDistance()
        {
            if (cat.state == State.NotInGame || mouse.state == State.NotInGame)
                return -1;

            return Math.Abs(cat.location - mouse.location);
        }
    }

    // Клиентский класс
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Введите номер теста (1, 2 или 3): ");
            string choice = Console.ReadLine()?.Trim();

            if (choice != "1" && choice != "2" && choice != "3") choice = "1";

            Game.InputFile = $"{choice}.ChaseData.txt";
            Game.OutFile = $"{choice}.PursuitLog.txt";

            Game game = new Game(16);
            game.Run();

            Console.WriteLine($"Игра завершена. Результат сохранен в {Game.OutFile}");
        }
    }
}