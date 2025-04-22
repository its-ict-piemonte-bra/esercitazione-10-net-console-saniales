using TicTacToe;

namespace lesson
{
    public class Program
    {
        /// <summary>
        /// The main entrypoint of your application.
        /// </summary>
        /// <param name="args">The arguments passed to the program</param>
        public static void Main(string[] args)
        {
            Console.WriteLine("Attenzione, questo gioco ha una funzione di salvataggio automatico");

            Game game;
            Console.WriteLine("Vuoi caricare la partita salvata?");
            string line = Console.ReadLine()!;

            if (line == "S")
            {
                try
                {
                    StreamReader reader = new StreamReader("./salvataggio.txt");
                    game = Game.Deserialize(reader);
                    reader.Close();
                }
                catch
                {
                    Console.WriteLine("File di salvataggio non trovato, inizializzo nuova partita");
                    game = InitializeGame();
                }
            }
            else
            {

                game = InitializeGame();
            }

            Console.WriteLine("Partita iniziata");
            Console.WriteLine("-----------------");

            StreamWriter writer;
            while (!game.IsGameOver())
            {
                Console.WriteLine(game.ToString());

                writer = new StreamWriter("./salvataggio.txt");
                game.Serialize(writer);
                writer.Close();

                try
                {
                    Console.Write("Scegli la coordinata X:");
                    int x = Convert.ToInt32(Console.ReadLine());
                    Console.Write("Scegli la coordinata Y:");
                    int y = Convert.ToInt32(Console.ReadLine());

                    // facciamo la mossa
                    game.CheckCell(x, y);
                }
                catch
                {
                    Console.WriteLine("Mossa non valida");
                }
            }

            // controlliamo l'esito
            if (game.IsDraw())
            {
                Console.WriteLine("Pareggio");
            }
            else
            {
                Console.WriteLine($"Vincitore: {game.GetWinner()}");
            }

            writer = new StreamWriter("./salvataggio.txt");
            game.Serialize(writer);
            writer.Close();
        }

        private static Game InitializeGame()
        {
            Game game = null;
            bool valid;

            do
            {
                valid = true;

                try
                {
                    Console.Write("Inserisci il nome del giocatore 1:");
                    string playerOneName = Console.ReadLine()!;
                    Console.Write("Inserisci il nome del giocatore 2:");
                    string playerTwoName = Console.ReadLine()!;

                    game = new Game(playerOneName, playerTwoName);
                }
                catch
                {
                    Console.WriteLine("Nomi giocatori non validi");
                    valid = false;
                }
            } while (!valid);

            return game!;
        }
    }
}
