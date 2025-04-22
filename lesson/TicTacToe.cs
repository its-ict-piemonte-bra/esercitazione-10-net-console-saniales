namespace TicTacToe
{
    /// <summary>
    /// Creates a new tic tac toe game.
    /// </summary>
    public class Game
    {
        /// <summary>
        /// Represents the players in game.
        /// </summary>
        private Player[] players;

        /// <summary>
        /// Represents the game grid.
        /// </summary>
        private Grid gameGrid;

        /// <summary>
        /// Represents the player who is going to make the move.
        /// </summary>
        private Player playerInTurn;

        private bool gameOver;
        private bool draw;
        
        private Player? winner;

        /// <summary>
        /// Creates a new Tic Tac Toe game.
        /// </summary>
        /// <param name="playerOneName">The name for player one.</param>
        /// <param name="playerTwoName">The name for player two.</param>
        public Game(
            string playerOneName,
            string playerTwoName
        )
        {
            if (string.IsNullOrWhiteSpace(playerOneName))
            {
                throw new ArgumentException("Player one name is invalid");
            }
            else if (string.IsNullOrWhiteSpace(playerTwoName))
            {
                throw new ArgumentException("Player two name is invalid");
            }
            else if (playerOneName == playerTwoName)
            {
                throw new ArgumentException("Player one and two must have a different name");
            }

            this.gameOver = false;
            this.draw = false;
            this.winner = null;

            this.players = [
                new Player(playerOneName, 'X'),
                new Player(playerTwoName, 'O')
            ];

            this.gameGrid = new Grid();

            Random rnd = new Random();
            int randomIndex = rnd.Next(0, 2);

            playerInTurn = players[randomIndex];
        }

        /// <summary>
        /// Makes the player in turn check a cell. Then moves the turn to the
        /// next player.
        /// </summary>
        /// <param name="x">The X coordinate of the cell</param>
        /// <param name="y">The Y coordinate of the cell</param>
        public void CheckCell(int x, int y)
        {
            char winnerSymbol = this.gameGrid.CheckCell(this.playerInTurn, x, y);
            if (winnerSymbol == '-')
            {
                this.nextTurn();
            }
            else
            {
                this.gameOver = true;

                if (winnerSymbol == '\0')
                {
                    this.draw = true;
                }
                else 
                {
                    this.winner = playerInTurn;
                }
            }
        }

        /// <summary>
        /// Returns if the game is over
        /// </summary>
        /// <returns>True if the game is over, false otherwise</returns>
        public bool IsGameOver()
        {
            return this.gameOver;
        }

        /// <summary>
        /// Returns if the game is a draw
        /// </summary>
        /// <returns>True if the game is a draw, false otherwise</returns>
        public bool IsDraw()
        {
            return this.draw;
        }

        /// <summary>
        /// Returns the winner, if available.
        /// </summary>
        /// <returns>The player object representing the winner, or null if not available</returns>
        public Player? GetWinner()
        {
            return this.winner;
        }

        /// <summary>
        /// Moves the turn to the next player.
        /// </summary>
        private void nextTurn()
        {
            if (this.playerInTurn == this.players[0])
            {
                this.playerInTurn = this.players[1];
            }
            else
            {
                this.playerInTurn = this.players[0];
            }
        }

        /// <summary>
        /// Returns a string representation of a tic tac toe game.
        /// </summary>
        /// <returns>A string representation of a tic tac toe game.</returns>
        public override string ToString()
        {
            return $"Griglia di gioco:\n{this.gameGrid}\nTocca a {playerInTurn}";
        }

        /// <summary>
        /// Serializes a game into a stream
        /// </summary>
        /// <param name="writer">The stream to write into</param>
        public void Serialize(StreamWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer is null");
            }

            foreach(Player player in this.players)
            {
                player.Serialize(writer);
            }

            writer.WriteLine("----------------");

            this.gameGrid.Serialize(writer);

            writer.WriteLine("----------------");

            this.playerInTurn.Serialize(writer);

            writer.WriteLine("----------------");

            writer.WriteLine(this.gameOver);
            writer.WriteLine(this.draw);
        }

        /// <summary>
        /// Deserializes a game from a stream
        /// </summary>
        /// <param name="reader">The stream reader from which read from</param>
        /// <returns>The initialized player</returns>
        public static Game Deserialize(StreamReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader is null");
            }
            else if (reader.EndOfStream)
            {
                throw new InvalidOperationException("stream is ended");
            }

            Game game = new Game("loading", "loading 2");

            game.players = [
                Player.Deserialize(reader),
                Player.Deserialize(reader)
            ];

            reader.ReadLine();

            game.gameGrid = Grid.Deserialize(reader);

            reader.ReadLine();

            game.playerInTurn = Player.Deserialize(reader);

            reader.ReadLine();

            game.gameOver = Convert.ToBoolean(reader.ReadLine());
            game.draw = Convert.ToBoolean(reader.ReadLine());
            
            if (game.gameOver && !game.draw)
            {
                game.winner = game.playerInTurn;
            }

            return game;
        }
    }

    /// <summary>
    /// Represents a single player in a tic-tac-toe game.
    /// </summary>
    public class Player
    {
        /// <summary>
        /// The valid symbols for a player.
        /// </summary>
        private static readonly char[] playerSymbols;

        /// <summary>
        /// Initializes the static properties of the Player class.
        /// </summary>
        static Player()
        {
            playerSymbols = ['X', 'O'];
        }

        /// <summary>
        /// The user display name
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// The user grid symbol
        /// </summary>
        public readonly char Symbol;

        /// <summary>
        /// Creates a new player instance, given the 
        /// specified name and symbol.
        /// </summary>
        /// <param name="name">The user display name</param>
        /// <param name="symbol">The user grid symbol</param>
        public Player(string name, char symbol)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name is invalid");
            }
            else if (!playerSymbols.Contains(symbol))
            {
                throw new ArgumentException($"Symbol is not valid, must be in ({string.Join(',', playerSymbols)})");
            }

            this.Name = name;
            this.Symbol = symbol;
        }

        /// <summary>
        /// Returns a string representation of the Player.
        /// </summary>
        /// <returns>A string representation of the Player.</returns>
        public override string ToString()
        {
            return $"{this.Name}: {this.Symbol}";
        }

        /// <summary>
        /// Serializes a player into a stream
        /// </summary>
        /// <param name="writer">The stream to write into</param>
        public void Serialize(StreamWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer is null");
            }

            writer.WriteLine($"{this.Name}:{this.Symbol}");
        }

        /// <summary>
        /// Deserializes a player from a stream
        /// </summary>
        /// <param name="reader">The stream reader from which read from</param>
        /// <returns>The initialized player</returns>
        public static Player Deserialize(StreamReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader is null");
            }
            else if (reader.EndOfStream)
            {
                throw new InvalidOperationException("stream is ended");
            }

            string line = reader.ReadLine()!;
            string[] values = line.Split(":");

            return new Player(values[0], values[1][0]);
        }
    }

    public class Grid
    {
        /// <summary>
        /// The symbols matrix.
        /// </summary>
        private char[,] symbols;

        /// <summary>
        /// The moves from the start of the game.
        /// </summary>
        private int moves;

        /// <summary>
        /// Creates a new empty game grid.
        /// </summary>
        public Grid()
        {
            this.moves = 0;
            this.symbols = new char[,]{
                { ' ', ' ', ' ' },
                { ' ', ' ', ' ' },
                { ' ', ' ', ' ' },
            };
        }

        /// <summary>
        /// Makes a player check the specified cell on the grid, then checks for
        /// victory.
        /// </summary>
        /// <param name="player">The player moving</param>
        /// <param name="x">The x coordinate on the grid</param>
        /// <param name="y">The y coordinate on the grid</param>
        /// <returns>The symbol of the winning player, '\0' if draw, '-' if the game is not over</returns>
        public char CheckCell(Player player, int x, int y)
        {
            if (player == null)
            {
                throw new ArgumentNullException("Player is null");
            }
            else if (x < 0 || x > 2)
            {
                throw new ArgumentOutOfRangeException("X is out of the grid, must be between 0 and 2");
            }
            else if (y < 0 || y > 2)
            {
                throw new ArgumentOutOfRangeException("Y is out of the grid, must be between 0 and 2");
            }
            else if (this.symbols[x, y] != ' ')
            {
                throw new InvalidOperationException("This cell is not empty");
            }

            this.symbols[x, y] = player.Symbol;
            this.moves++;

            return this.checkVictory(player.Symbol);
        }

        /// <summary>
        /// Checks for victory of one player.
        /// </summary>
        /// <param name="symbol">The symbol to check</param>
        /// <returns>The symbol of the winning player, '\0' if draw, '-' if the game is not over</returns>
        private char checkVictory(char symbol)
        {
            if (this.moves < 5)
            {
                return '-';
            }

            for (int i = 0; i < 3; i++)
            {
                // rows
                if (
                    this.symbols[i, 0] == symbol &&
                    this.symbols[i, 1] == symbol &&
                    this.symbols[i, 2] == symbol
                )
                {
                    return symbol;
                }

                // columns
                if (
                    this.symbols[0, i] == symbol &&
                    this.symbols[1, i] == symbol &&
                    this.symbols[2, i] == symbol
                )
                {
                    return symbol;
                }
            }

            // main diagonal
            if (
                this.symbols[0, 0] == symbol &&
                this.symbols[1, 1] == symbol &&
                this.symbols[2, 2] == symbol
            )
            {
                return symbol;
            }

            // secondary diagonal
            if (
                this.symbols[0, 2] == symbol &&
                this.symbols[1, 1] == symbol &&
                this.symbols[2, 0] == symbol
            )
            {
                return symbol;
            }

            if (this.moves == 9)
            {
                return '\0';
            }

            return '-';
        }

        /// <summary>
        /// Returns a string representation of the tic tac toe grid.
        /// </summary>
        /// <returns>A string representation of the tic tac toe grid.</returns>
        public override string ToString()
        {
            return
            $"   |   |   \n" +
            $" {this.symbols[0, 0]} | {this.symbols[0, 1]} | {this.symbols[0, 2]} \n" +
            $"   |   |   \n" +
            $"---+---+---\n" +
            $"   |   |   \n" +
            $" {this.symbols[1, 0]} | {this.symbols[1, 1]} | {this.symbols[1, 2]} \n" +
            $"   |   |   \n" +
            $"---+---+---\n" +
            $"   |   |   \n" +
            $" {this.symbols[2, 0]} | {this.symbols[2, 1]} | {this.symbols[2, 2]} \n" +
            $"   |   |   ";
        }

        /// <summary>
        /// Serializes a grid into a stream
        /// </summary>
        /// <param name="writer">The stream to write into</param>
        public void Serialize(StreamWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer is null");
            }

            writer.WriteLine($"{this.symbols[0,0]}:{this.symbols[0,1]}:{this.symbols[0,2]}");
            writer.WriteLine($"{this.symbols[1,0]}:{this.symbols[1,1]}:{this.symbols[1,2]}");
            writer.WriteLine($"{this.symbols[2,0]}:{this.symbols[2,1]}:{this.symbols[2,2]}");
            writer.WriteLine(this.moves);
        }

        /// <summary>
        /// Deserializes a grid from a stream
        /// </summary>
        /// <param name="reader">The stream reader from which read from</param>
        /// <returns>The initialized player</returns>
        public static Grid Deserialize(StreamReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader is null");
            }
            else if (reader.EndOfStream)
            {
                throw new InvalidOperationException("stream is ended");
            }

            Grid grid = new Grid();
            
            for (int i = 0; i < 3; i++)
            {
                string line = reader.ReadLine()!;
                string[] values = line.Split(":");
                grid.symbols[0, i] = values[0][0];
                grid.symbols[1, i] = values[1][0];
                grid.symbols[2, i] = values[2][0];
            }

            grid.moves = Convert.ToInt32(reader.ReadLine());

            return grid;
        }
    }

}