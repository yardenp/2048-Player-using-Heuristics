using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Game2048
{
    class Program
    {
        #region Type Definitions

        public static Boolean SholdPrint = false;
        public static Boolean OnlyGame = false;
        public static Boolean SholdStopEveryMove = false;
        public static Boolean ExpectAddedTile = false;
        public static Boolean BestPlayer = false;
        public static int numOfGames = 1000;

        public struct State
        {
            public Tile[] Tiles;
            public bool Win;
            public bool Lose;
            public int Score;


            public override bool Equals(object ob)
            {
                if (ob is State)
                {
                    State other = (State)ob;
                    for (var row = 0; row < 4; row++)
                    {
                        for (var column = 0; column < 4; column++)
                        {
                            if (Tiles[column + row * 4].value != other.Tiles[column + row * 4].value)
                                return false;
                        }
                    }
                    return Win == other.Win && Lose == other.Lose && Score == other.Score;
                }
                else
                {
                    return false;
                }
            }

            public override int GetHashCode() { return 0; }
        }

        public enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }

        internal class Tile
        {
            internal int value;

            public Tile()
                : this(0)
            {
            }

            public Tile(int num)
            {
                value = num;
            }

            public virtual bool Empty
            {
                get
                {
                    return value == 0;
                }
            }
        }

        #endregion

        public class Game2048
        {

            #region Properties

            public System.Random Rnd = new System.Random();
            public State GameState = new State() { Win = false, Lose = false, Score = 0 };
            public int ourTarget = 2048;

            #endregion

            #region Game State

            public State CopyState()
            {
                return new State { Tiles = CopyTiles(GameState.Tiles), Win = GameState.Win, Lose = GameState.Lose, Score = GameState.Score };
            }

            public Tile CopyTile(Tile tile)
            {
                return new Tile { value = tile.value };
            }

            public Tile[] CopyTiles(Tile[] tiles)
            {
                return tiles.ToList().Select(tile => CopyTile(tile)).ToArray();

            }

            public void RestoreState(State backupState)
            {
                GameState.Tiles = CopyTiles(backupState.Tiles);
                GameState.Win = backupState.Win;
                GameState.Lose = backupState.Lose;
                GameState.Score = backupState.Score;

            }

            public virtual void resetGame()
            {
                GameState.Score = 0;
                GameState.Win = false;
                GameState.Lose = false;
                GameState.Tiles = new Tile[4 * 4];
                for (int i = 0; i < GameState.Tiles.Length; i++)
                {
                    GameState.Tiles[i] = new Tile(0);
                }
                addTile();
                addTile();
            }

            #endregion

            #region Moves

            public virtual void move(Direction direction)
            {
                if (!canMove())
                {
                    GameState.Lose = true;
                }

                if (!GameState.Win && !GameState.Lose)
                {
                    switch (direction)
                    {
                        case Direction.Left:
                            left();
                            break;
                        case Direction.Right:
                            right();
                            break;
                        case Direction.Down:
                            down();
                            break;
                        case Direction.Up:
                            up();
                            break;
                    }
                }

                if (!GameState.Win && !canMove())
                {
                    GameState.Lose = true;
                }

            }

            public virtual void left()
            {
                //    bool needAddTile = false;
                for (int i = 0; i < 4; i++)
                {
                    Tile[] line = getLine(i);
                    Tile[] merged = mergeLine(moveLine(line));

                    setLine(i, merged);
                    //    if (!needAddTile && !compare(line, merged) && changed(line, merged))
                    //    {
                    //        needAddTile = true;
                    //    }
                    //}

                    //if (needAddTile)
                    //{
                    //   // addTile();
                }
            }

            public virtual void right()
            {
                GameState.Tiles = rotate(180);
                left();
                GameState.Tiles = rotate(180);
            }

            public virtual void up()
            {
                GameState.Tiles = rotate(270);
                left();
                GameState.Tiles = rotate(90);
            }

            public virtual void down()
            {
                GameState.Tiles = rotate(90);
                left();
                GameState.Tiles = rotate(270);
            }

            #endregion

            #region Tiles (Board)

            public Tile tileAt(int x, int y)
            {
                return GameState.Tiles[x + y * 4];
            }

            public void addTile()
            {
                IList<Tile> list = availableSpace();
                if (availableSpace().Count > 0)
                {
                    int index = (int)(Rnd.NextDouble() * list.Count) % list.Count;
                    Tile emptyTile = list[index];
                    emptyTile.value = Rnd.NextDouble() < 0.9 ? 2 : 4;
                }
            }

            public IList<Tile> availableSpace()
            {
                IList<Tile> list = new List<Tile>(16);
                foreach (Tile t in GameState.Tiles)
                {
                    if (t.Empty)
                    {
                        list.Add(t);
                    }
                }
                return list;
            }
            
            private bool Full
            {
                get
                {
                    return availableSpace().Count == 0;
                }
            }

            private Tile[] rotate(int angle)
            {
                Tile[] newTiles = new Tile[4 * 4];
                int offsetX = 3, offsetY = 3;
                if (angle == 90)
                {
                    offsetY = 0;
                }
                else if (angle == 270)
                {
                    offsetX = 0;
                }

                double rad = ConvertToRadians(angle);
                int cos = (int)Math.Cos(rad);
                int sin = (int)Math.Sin(rad);
                for (int x = 0; x < 4; x++)
                {
                    for (int y = 0; y < 4; y++)
                    {
                        int newX = (x * cos) - (y * sin) + offsetX;
                        int newY = (x * sin) + (y * cos) + offsetY;
                        newTiles[(newX) + (newY) * 4] = tileAt(x, y);
                    }
                }
                return newTiles;
            }

            public void PrintBoard()
            {
                if (Program.SholdPrint)
                {
                    for (var row = 0; row < 4; row++)
                    {
                        for (var column = 0; column < 4; column++)
                        {
                            Console.Write(tileAt(column, row).value);
                            Console.Write("    ");
                        }
                        Console.WriteLine();
                    }
                    Console.WriteLine();
                    Console.WriteLine();
                }

            }

            internal virtual bool canMove()
            {
                if (!Full)
                {
                    return true;
                }
                for (int x = 0; x < 4; x++)
                {
                    for (int y = 0; y < 4; y++)
                    {
                        Tile t = tileAt(x, y);
                        if ((x < 3 && t.value == tileAt(x + 1, y).value) || ((y < 3) && t.value == tileAt(x, y + 1).value))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }

            #endregion

            #region Tile Lines

            private bool compare(Tile[] line1, Tile[] line2)
            {
                if (line1 == line2)
                {
                    return true;
                }
                else if (line1.Length != line2.Length)
                {
                    return false;
                }

                for (int i = 0; i < line1.Length; i++)
                {
                    if (!line1[i].Equals(line2[i]))
                    {
                        return false;
                    }
                }
                return true;
            }

            public virtual bool changed(Tile[] oldLine, Tile[] newLine)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (oldLine[i].value != newLine[i].value)
                    {
                        return true;
                    }
                }
                return false;
            }

            private Tile[] moveLine(Tile[] oldLine)
            {
                LinkedList<Tile> l = new LinkedList<Tile>();
                for (int i = 0; i < 4; i++)
                {
                    if (!oldLine[i].Empty)
                    {
                        l.AddLast(oldLine[i]);
                    }
                }
                if (l.Count == 0)
                {
                    return oldLine;
                }
                else
                {
                    ensureSize(l, 4);

                    var newLine = l.ToList().ToArray();
                    return newLine;
                }
            }

            private Tile[] mergeLine(Tile[] oldLine)
            {
                LinkedList<Tile> list = new LinkedList<Tile>();
                for (int i = 0; i < 4 && !oldLine[i].Empty; i++)
                {
                    int num = oldLine[i].value;
                    if (i < 3 && oldLine[i].value == oldLine[i + 1].value)
                    {
                        num *= 2;
                        GameState.Score += num;

                        if (num == ourTarget)
                        {
                            GameState.Win = true;
                        }
                        i++;
                    }
                    list.AddLast(new Tile(num));
                }
                if (list.Count == 0)
                {
                    return oldLine;
                }
                else
                {
                    ensureSize(list, 4);
                    return list.ToList().ToArray();
                }
            }

            private static void ensureSize(LinkedList<Tile> l, int s)
            {
                while (l.Count != s)
                {
                    l.AddLast(new Tile());
                }
            }

            private Tile[] getLine(int index)
            {
                Tile[] result = new Tile[4];
                for (int i = 0; i < 4; i++)
                {
                    result[i] = tileAt(i, index);
                }
                return result;
            }

            private void setLine(int index, Tile[] re)
            {
                Array.Copy(re, 0, GameState.Tiles, index * 4, 4);
            }

            #endregion

            #region Helpers

            public double ConvertToRadians(double angle)
            {
                return (Math.PI / 180) * angle;
            }
            #endregion

        }






        public static class Evaluator
        {
            #region Properties
            public static Game2048 Game;
            public static System.Random Rnd = new System.Random();
            public static int NumberOfHeuristics = 10;
            public static int StopAt = 4096;
            public static int HueristicsTileJump = 512;
            #endregion

            #region Evaluation Function
            
            public static double[] getRandomWeights()
            {
              //generate random weights
              var weights = new double[NumberOfHeuristics];
              var sum = 1000.0;
              for (var i = 0; i < NumberOfHeuristics - 1; i++)
              {
                  weights[i] = Rnd.Next(0, (int)sum);
                  sum -= weights[i];
              }
              weights[NumberOfHeuristics - 1] = sum;
              weights = weights.ToList().OrderBy(item => Rnd.Next()).ToArray();
                /*          if (Program.SholdPrint)
                          {
                              for (int i = 0; i < 6; i++)
                              {
                                  Console.Write(weights[i] + " |");
                              }
                              Console.WriteLine();
                              Console.Read();
                          }
                  */
                if (BestPlayer)
                {
                    // best player we got
                    weights  = new double[10] { 71 /*NumOfEmptyCells*/, 696/*AddedScore*/, 51/*NumberOfIdenticalTiles*/ 
                                            , 807/*DistancesAndValues*/, 966/*ApproximateSameTiles*/, 5/*Proximate1PowerDiffTiles*/ 
                                            , 476/*LocationCorner*/ , 771/*LocationSides*/ ,211/*LocationMiddle*/
                                            , 14/*TrapedTile*/};
                }
              return weights;
          }

        
           

          public static void PrintState(State stateToPrint)
            {
                if (Program.SholdPrint)
                {
                    Console.WriteLine("Score:" + stateToPrint.Score);
                    Console.WriteLine("Win:" + stateToPrint.Win);
                    Console.WriteLine("Lose:" + stateToPrint.Lose);
                    for (var row = 0; row < 4; row++)
                    {
                        for (var column = 0; column < 4; column++)
                        {
                            Console.Write(stateToPrint.Tiles[column + row * 4].value);
                            Console.Write("    ");
                        }
                        Console.WriteLine();
                    }
                    Console.WriteLine();
                    Console.WriteLine();
                }
            }
            public static double evaluate(double[] weights, int numOfGames)
            {
                Game = new Game2048();
                Game.resetGame();
                Game.PrintBoard();
                double leftScore, rightScore, upScore, downScore;
                State left_afterState;// = Game.GameState; // just so it will be initialized
                State right_afterState;// = Game.GameState;
                State up_afterState;// = Game.GameState;
                State down_afterState;// = Game.GameState;
                State stateBeforeChange;
                Dictionary<Direction, State> directionDictionary = new Dictionary<Direction, State>();

                Direction nextDirection;
                int count1024 = 0, count2048=0, count4096=0;
                bool got1024 = false, got2048 = false, got4096= false;
                double[] Qualities = new double[numOfGames];
                
                for (int i = 0; i < numOfGames; i++)
                {
                    StopAt = 4096;
                    got1024 = false; got2048 = false; got4096 = false;
                    Game.resetGame();
                    Game.PrintBoard();
                    int count = 0;

                    while (!Game.GameState.Win && !Game.GameState.Lose)
                    {
                        stateBeforeChange = Game.CopyState();

                        
                        left_afterState = simulateMove(Direction.Left, stateBeforeChange, weights, out leftScore);// PrintState(left_afterState);
                        right_afterState = simulateMove(Direction.Right, stateBeforeChange, weights, out rightScore);// PrintState(right_afterState);
                        up_afterState = simulateMove(Direction.Up, stateBeforeChange, weights, out upScore);// PrintState(up_afterState);
                        down_afterState = simulateMove(Direction.Down, stateBeforeChange, weights, out downScore);// PrintState(down_afterState);

                        directionDictionary[Direction.Left] = left_afterState;
                        directionDictionary[Direction.Right] = right_afterState;
                        directionDictionary[Direction.Up] = up_afterState;
                        directionDictionary[Direction.Down] = down_afterState;

                        nextDirection = ChooseDirectionByScore(leftScore, rightScore, upScore, downScore);

                        Game.RestoreState(directionDictionary[nextDirection]);
                        if (Program.SholdPrint)
                        {
                            Console.Write("Direction is:" + nextDirection);
                            Console.WriteLine();
                        }
                        Game.addTile();
                        count++;
                        PrintState(Game.GameState);
                        if(HasTile(1024) && !got1024){ count1024++; got1024 = true;}
                        if(HasTile(2048) && !got2048){ count2048++; got2048 = true; }
                        if(HasTile(4096) && !got4096) { count4096++; got4096 = true; }

                        if (HasTile(HueristicsTileJump)) HueristicsTileJump *= 2;
                        if(HasTile(StopAt))
                        {
                            Program.SholdPrint = true;
                            PrintState(Game.GameState);
                            Program.SholdPrint = false;
                //            Console.Read();
               //             Console.Read();
                            StopAt = StopAt * 2;
                            
                        }
                        if (SholdStopEveryMove)
                        {

                            Console.Read();
                            Console.Read();
                        }
                    }

                    

                    if (Program.SholdPrint)
                    {
                        if (Game.GameState.Win) Console.Write("WON GAME");
                        else if (Game.GameState.Lose) Console.Write("LOST GAME");
                        Console.WriteLine();
                        Console.WriteLine();
                        Console.Read();
                        Console.Read();
                    }
       //            Console.WriteLine("number of steps:" + count);
      //             Console.WriteLine("Score: " + Game.GameState.Score);
                    //calculate board grade (fitness)
                    Qualities[i] = EvaluateFitness();
                }

                Console.WriteLine("1024 " + count1024 +" times");
                Console.WriteLine("2048 " + count2048 + " times");
                Console.WriteLine("4096 " + count4096 + " times");
                return getMid(Qualities);
            }

            public static Boolean HasTile(int target){
                for(int i=0; i<16;i++){
                    if(Game.GameState.Tiles[i].value == target){
                        return true;
                    }
                }
                return false;
            }

            public static double getMid(double[] Qualities)
            {
                int length = Qualities.Length;
                
                Array.Sort<double>(Qualities);
                if (length % 2 == 1) return Qualities[length / 2]; // the middle value
                else return (Qualities[(length / 2)-1] + Qualities[length / 2])/2; // the avrage of the two middles


            }

            public static double EvaluateFitness()
            {
                var fitness = Game.GameState.Score;
         /*       if (Game.GameState.Win)
                {
                    fitness += 10000;
                }
         */       return fitness;
            }

            public static Direction ChooseDirectionByScore(double leftScore, double rightScore, double upScore, double downScore)
            {
                double maxValue = Math.Max(leftScore, Math.Max(rightScore, Math.Max(upScore, downScore)));
                if (maxValue == leftScore) return Direction.Left;
                if (maxValue == rightScore) return Direction.Right;
                if (maxValue == upScore) return Direction.Up;
                if (maxValue == downScore) return Direction.Down;
                else
                {
                    throw new Exception("somthing wrong with ChooseDirectionByScore");
                }
            }

            #endregion

            #region Movement Simulation


            private static double getAVGStateGradeForRandomTileAdditions
                                (int randomTileValue, State stateBeforeRandomAddition, State stateBeforeMove, double[] weights)
            {
                IList<Tile> availableSpace = Game.availableSpace();
                List<double> stateGrades = new List<double>();

                foreach (var emptyTile in availableSpace)
                {
                    emptyTile.value = randomTileValue;
                    stateGrades.Add(CalculateHeuristicsAndWeights(stateBeforeMove, weights));
                    Game.RestoreState(stateBeforeRandomAddition);
                }
                return availableSpace.Count() == 0 ? CalculateHeuristicsAndWeights(stateBeforeMove, weights) : stateGrades.Average();
            }

       
            private static State simulateMove(Direction direction, State stateBeforeChange, double[] weights, out double moveHeuristicScore)
            {
                //run with one level of the game tree
                if (Program.SholdPrint && !OnlyGame) Console.WriteLine("****Heuristics Direction:" + direction + "****");

                Game.move(direction);

                var afterMoveBeforeRandomState = Game.CopyState();
                if (ExpectAddedTile)
                {
                    var avgGradeForRandomTileOfValue2 = getAVGStateGradeForRandomTileAdditions(2, afterMoveBeforeRandomState,
                                                                                                  stateBeforeChange, weights);
                    var avgGradeForRandomTileOfValue4 = getAVGStateGradeForRandomTileAdditions(4, afterMoveBeforeRandomState,
                                                                                              stateBeforeChange, weights);


                    moveHeuristicScore = 0.1 * avgGradeForRandomTileOfValue4 + 0.9 * avgGradeForRandomTileOfValue2;
                }
                else
                {
                    moveHeuristicScore = CalculateHeuristicsAndWeights(stateBeforeChange, weights);
                }
                Game.RestoreState(stateBeforeChange);
                return afterMoveBeforeRandomState;


                //run with three or two levels of the game tree
/*
                double leftScore, rightScore, upScore, downScore;
                Dictionary<Direction, double> directionDictionary = new Dictionary<Direction, double>();
                Direction nextDirection;

                if (Program.SholdPrint) Console.WriteLine("****Heuristics Direction:" + direction + "****");
                Game.move(direction); //first move to the direction then calculate all other directions
                State FirstMoveState = Game.CopyState();
                PrintState(Game.GameState);
                if (stateBeforeChange.Equals(Game.GameState))
                {
                    moveHeuristicScore = - 1;     // diractions that the board won't change
                    Game.RestoreState(stateBeforeChange); // going back to original move
                    return Game.GameState; // just to return something
                }
                
                simulate2Move(Direction.Left, FirstMoveState, stateBeforeChange, weights, out leftScore);
                simulate2Move(Direction.Right, FirstMoveState, stateBeforeChange, weights, out rightScore);
                simulate2Move(Direction.Up, FirstMoveState, stateBeforeChange, weights, out upScore);
                simulate2Move(Direction.Down, FirstMoveState, stateBeforeChange, weights, out downScore);
               
                directionDictionary[Direction.Left] = leftScore;
                directionDictionary[Direction.Right] = rightScore;
                directionDictionary[Direction.Up] = upScore;
                directionDictionary[Direction.Down] = downScore;

                nextDirection = ChooseDirectionByScore(leftScore, rightScore, upScore, downScore);

                Console.WriteLine("leftScore:" + leftScore);
                Console.WriteLine("rightScore:" + rightScore);
                Console.WriteLine("upScore:" + upScore);
                Console.WriteLine("downScore:" + downScore);
                moveHeuristicScore = directionDictionary[nextDirection]; // the best score for all directions
      
                moveHeuristicScore = CalculateHeuristicsAndWeights(stateBeforeChange, weights);  
                Game.RestoreState(stateBeforeChange); // going back to original move
                return FirstMoveState;
  */          }

            private static void simulate2Move(Direction direction,State FirstMovment, State stateBeforeChange, double[] weights, out double moveHeuristicScore)
            {
                double leftScore, rightScore, upScore, downScore;
                Game.move(direction);
          //      Console.WriteLine(direction + ":");
        //        PrintState(Game.GameState);
                State SecondMovment = Game.CopyState();
                simulate3Move(Direction.Left, SecondMovment, stateBeforeChange, weights, out leftScore);
                simulate3Move(Direction.Right, SecondMovment, stateBeforeChange, weights, out rightScore);
                simulate3Move(Direction.Up, SecondMovment, stateBeforeChange, weights, out upScore);
                simulate3Move(Direction.Down, SecondMovment, stateBeforeChange, weights, out downScore);

                moveHeuristicScore = CalculateHeuristicsAndWeights(stateBeforeChange, weights);
                Game.RestoreState(FirstMovment); // going back to First move
            }



            private static void simulate3Move(Direction direction, State SecondMovment, State stateBeforeChange, double[] weights, out double moveHeuristicScore)
            {
                Game.move(direction);
                //      Console.WriteLine(direction + ":");
                //        PrintState(Game.GameState);

                moveHeuristicScore = CalculateHeuristicsAndWeights(stateBeforeChange, weights);
                Game.RestoreState(SecondMovment); // going back to First move
            }
            #endregion

            #region Heuristics

            public static double CalculateHeuristicsAndWeights(State stateBeforeChange, double[] weights)
            {
                if (stateBeforeChange.Equals(Game.GameState)) return Double.MinValue;     // diractions that the board won't change 
                double[] heuristics = new double[NumberOfHeuristics];

                double heuristicNumOfEmptyCells = Heuritic_NumberOfEmptyCells();
                heuristics[0] = heuristicNumOfEmptyCells;

                double heuristicAddedScore = Heuristic_AddedScore(stateBeforeChange);
                heuristics[1] = heuristicAddedScore;

                double heuristicNumberOfIdenticalTiles = Heuristic_NumberOfIdenticalTiles();
                heuristics[2] = heuristicNumberOfIdenticalTiles;

                double heuristicDistancesAndValues = Heuritic_DistancesAndValues();
                heuristics[3] = heuristicDistancesAndValues;

                double HeuriticApproximateSameTiles = Heuritic_ProximateSameTiles();
                heuristics[4] = HeuriticApproximateSameTiles;

                double HeuriticProximate1PowerDiffTiles = Heuritic_Proximate1PowerDiffTiles();
                heuristics[5] = HeuriticProximate1PowerDiffTiles;

                double HeuristicLocationCorner = Heuristic_LocationCorner();
                heuristics[6] = HeuristicLocationCorner;

                double HeuristicLocationSides = Heuristic_LocationSides();
                heuristics[7] = HeuristicLocationSides;

                double HeuristicLocationMiddle = Heuristic_LocationMiddle();
                heuristics[8] = HeuristicLocationMiddle;

                double HeuriticTrapedTile = Heuritic_TrapedTile();
                heuristics[9] = HeuriticTrapedTile;

                if (Program.SholdPrint && !OnlyGame)
                {
                    Console.WriteLine("heuristicNumOfEmptyCells:" + heuristicNumOfEmptyCells);
                    Console.WriteLine("heuristicAddedScore: " + heuristicAddedScore);
                    Console.WriteLine("heuristicNumberOfIdenticalTiles:" + heuristicNumberOfIdenticalTiles);
                    Console.WriteLine("heuristicDistancesAndValues:" + heuristicDistancesAndValues);
                    Console.WriteLine("HeuriticProximateSameTiles:" + HeuriticApproximateSameTiles);
                    Console.WriteLine("HeuritiProximate1PowerDiffTiles:" + HeuriticProximate1PowerDiffTiles);
                    Console.WriteLine("HeuristicLocationCorner:" + HeuristicLocationCorner);
                    Console.WriteLine("HeuristicLocationSides:" + HeuristicLocationSides);
                    Console.WriteLine("HeuristicLocationMiddle:" + HeuristicLocationMiddle);
                    Console.WriteLine("HeuriticTrapedTile:" + HeuriticTrapedTile);
                }

                var sigma = 0.0;
                for (var i = 0; i < weights.Count(); i++)
                {
                    sigma += weights[i] * heuristics[i];
                }
                return sigma;
                //return /*Rnd.NextDouble()*0.00001*/ +heuristicNumOfEmptyCells + heuristicAddedScore + heuristicNumberOfIdenticalTiles + heuristicDistancesAndValues + HeuriticApproximateSameTiles + HeuriticProximate1PowerDiffTiles; // change here to see magic :)
            }

            private static double normalize(double returnValue, double maxValue)
            {
                return returnValue / maxValue;
            }

            private static double normalize(double returnValue, double minValue, double maxValue)
            {
                return (returnValue - minValue) / (maxValue - minValue);
            }



            //This Functions are written under the assumption that the current game state is AFTER the move

            public static double Heuritic_TrapedTile()
            {
                double counter = 0;
                double tmp = (Math.Log(StopAt, 2) / 2);
                for (int index = 0; index < 16; index++)
                {
                    var neighbors = getValuesOf4NeighborTiles(index);
                    var myValue = neighbors.Item1;
                    var upValue = neighbors.Item2;
                    var downValue = neighbors.Item3;
                    var leftValue = neighbors.Item4;
                    var rightValue = neighbors.Item5;
                    if (myValue != -1 && myValue != 0)
                    {
                        if ((upValue == -1 || upValue > myValue) && (downValue == -1 || downValue > myValue)
                            && (leftValue == -1 || leftValue > myValue) && (rightValue == -1 || rightValue > myValue))
                        {
                            counter -= (tmp - Math.Log(myValue, 2)) + tmp;
                        }
                    }
                }
                return normalize(counter, 0, 6 * (tmp - Math.Log(2, 2)) + tmp);
            }

            public static double Heuristic_LocationCorner()
            {
                double upLeft = 0, upRight = 0, downLeft = 0, downRight = 0;
                // calculate the rank of each Cornered tile
                if (Game.tileAt(0, 0).value != 0) upLeft = Math.Log(Game.tileAt(0, 0).value, 2);
                if (Game.tileAt(3, 0).value != 0) upRight = Math.Log(Game.tileAt(3, 0).value, 2);
                if (Game.tileAt(0, 3).value != 0) downLeft = Math.Log(Game.tileAt(0, 3).value, 2);
                if (Game.tileAt(3, 3).value != 0) downRight = Math.Log(Game.tileAt(3, 3).value, 2);
                return normalize(upLeft + upRight + downLeft + downRight, 0, 4 * Math.Log(StopAt, 2));
            }

            public static double Heuristic_LocationSides()
            {
                double upWallLeft = 0, upWallRight = 0, leftWallUp = 0, leftWallDown = 0, rightWallUp = 0, rightWallDown = 0, downWallLeft = 0, downWallRight = 0;
                // calculate the rank of each side tile
                if (Game.tileAt(1, 0).value != 0) upWallLeft = Math.Log(Game.tileAt(1, 0).value, 2);
                if (Game.tileAt(2, 0).value != 0) upWallRight = Math.Log(Game.tileAt(2, 0).value, 2);

                if (Game.tileAt(0, 1).value != 0) leftWallUp = Math.Log(Game.tileAt(0, 1).value, 2);
                if (Game.tileAt(0, 2).value != 0) leftWallDown = Math.Log(Game.tileAt(0, 2).value, 2);

                if (Game.tileAt(3, 1).value != 0) rightWallUp = Math.Log(Game.tileAt(3, 1).value, 2);
                if (Game.tileAt(3, 2).value != 0) rightWallDown = Math.Log(Game.tileAt(3, 2).value, 2);

                if (Game.tileAt(1, 3).value != 0) downWallLeft = Math.Log(Game.tileAt(1, 3).value, 2);
                if (Game.tileAt(2, 3).value != 0) downWallRight = Math.Log(Game.tileAt(2, 3).value, 2);
                return normalize(upWallLeft + upWallRight + leftWallUp + leftWallDown + rightWallUp + rightWallDown + downWallLeft + downWallRight, 0, 8 * Math.Log(StopAt, 2));
            }

            public static double Heuristic_LocationMiddle()
            {
                double upLeft = 0, upRight = 0, downLeft = 0, downRight = 0;
                // calculate the rank of each Cornered tile
                if (Game.tileAt(1, 1).value != 0) upLeft = Math.Log(Game.tileAt(1, 1).value, 2);
                if (Game.tileAt(2, 1).value != 0) upRight = Math.Log(Game.tileAt(2, 1).value, 2);
                if (Game.tileAt(1, 2).value != 0) downLeft = Math.Log(Game.tileAt(1, 2).value, 2);
                if (Game.tileAt(2, 2).value != 0) downRight = Math.Log(Game.tileAt(2, 2).value, 2);
                return normalize(upLeft + upRight + downLeft + downRight, 0, 4 * Math.Log(StopAt, 2));
            }





            // without the random tile 
            public static double Heuristic_NumberOfIdenticalTiles()
            {
                double sumOfNumbersAndRanks = 0;
                int count = 0;
                for (int value = 2; value != Game.ourTarget; value = value * 2)
                {
                    for (int index = 0; index < 16; index++)
                    {
                        if (Game.GameState.Tiles[index].value == value)
                            count++;
                    }
                    if (count > 1)
                        sumOfNumbersAndRanks += count * Math.Log(value, 2);
                    count = 0;
                }
                return normalize(sumOfNumbersAndRanks, 5 * (Math.Log(StopAt/*Game.ourTarget*/, 2) - 1));
            }


            public static double Heuristic_AddedScore(State stateBeforeChange)
            {
                double differenceInScore = Game.GameState.Score - stateBeforeChange.Score;
                if (differenceInScore == 0) return 0;
                // Console.WriteLine("long line:"+normalize(Math.Log(Game.GameState.Score - stateBeforeChange.Score,2), Math.Log(Game.ourTarget, 2)));
                // Console.WriteLine("shortline:" + Math.Log(Game.GameState.Score - stateBeforeChange.Score, Game.ourTarget)); // they are equal
                return Math.Log(differenceInScore, StopAt/* Game.ourTarget*/);
            }


            public static double Heuritic_DistancesAndValues()
            {
                double valueAccumulator = 0;
                for (int index = 0; index < 16; index++)
                {
                    for (int anotherIndex = 0; anotherIndex < 16; anotherIndex++)
                    {
                        if (index != anotherIndex)
                        {
                            var myValue = Game.GameState.Tiles[index].value == 0 ? 0 :
                                            Math.Log(Game.GameState.Tiles[index].value, 2);
                            var otherValue = Game.GameState.Tiles[anotherIndex].value == 0 ? 0 :
                                             Math.Log(Game.GameState.Tiles[anotherIndex].value, 2);
                            var diff = Math.Abs(myValue - otherValue);

                            var myi = Math.Floor((double)(index / 4));
                            var myj = index % 4;

                            var otheri = Math.Floor((double)(anotherIndex / 4));
                            var otherj = anotherIndex % 4;

                            var distance = Math.Abs(myi - otheri)
                                           + Math.Abs(myj - otherj);

                            valueAccumulator += 1 / (distance + diff + 0.0001);
                        }
                    }
                }

                var maxValue = 15 * 16 * (1 / (1 + 0 + 0.0001));
                var minValue = 15 * 16 * (1 / (6 + 12 + 0.0001)); //assuming max value 2^12
                return normalize(valueAccumulator, minValue, maxValue);
            }

            private static Tuple<double, double, double, double, double> getValuesOf4NeighborTiles(int index)
            {
                var myi = Math.Floor((double)(index / 4));
                var myj = index % 4;

                var tileUp = myi == 0 ? -1 : (int)((myi - 1) * 4 + myj);
                var tileDown = myi == 3 ? -1 : (int)((myi + 1) * 4 + myj);
                var tileLeft = myj == 0 ? -1 : (int)(myi * 4 + myj - 1);
                var tileRight = myj == 3 ? -1 : (int)(myi * 4 + myj + 1);

                var myValue = index == -1 ? -1 :
                              Game.GameState.Tiles[index].value == 0 ? 0 :
                              Math.Log(Game.GameState.Tiles[index].value, 2);
                var upValue = tileUp == -1 ? -1 :
                              Game.GameState.Tiles[tileUp].value == 0 ? 0 :
                              Math.Log(Game.GameState.Tiles[tileUp].value, 2);
                var downValue = tileDown == -1 ? -1 :
                                Game.GameState.Tiles[tileDown].value == 0 ? 0 :
                                Math.Log(Game.GameState.Tiles[tileDown].value, 2);
                var leftValue = tileLeft == -1 ? -1 :
                                Game.GameState.Tiles[tileLeft].value == 0 ? 0 :
                                Math.Log(Game.GameState.Tiles[tileLeft].value, 2);
                var rightValue = tileRight == -1 ? -1 :
                                 Game.GameState.Tiles[tileRight].value == 0 ? 0 :
                                 Math.Log(Game.GameState.Tiles[tileRight].value, 2);

                return new Tuple<double, double, double, double, double>
                                    (myValue, upValue, downValue, leftValue, rightValue);
            }

            public static double Heuritic_ProximateSameTiles()
            {
                double counter = 0;
                for (int index = 0; index < 16; index++)
                {
                    var neighbors = getValuesOf4NeighborTiles(index);
                    var myValue = neighbors.Item1;
                    var upValue = neighbors.Item2;
                    var downValue = neighbors.Item3;
                    var leftValue = neighbors.Item4;
                    var rightValue = neighbors.Item5;

                    if (myValue != -1 && myValue == upValue)
                        counter = counter + myValue;
                    if (myValue != -1 && myValue == downValue)
                        counter = counter + myValue;
                    if (myValue != -1 && myValue == leftValue)
                        counter = counter + myValue;
                    if (myValue != -1 && myValue == rightValue)
                        counter = counter + myValue;
                }
                /* all tiles different */
                var minValue = 0;
                /* all tiles the same 2^14 */
                var maxValue = 4 * 4 * 14 /* 4 center tiles with 4 directinos */
                               + 4 * 2 * 14/* 4 edge tiles with 2 directinos */
                               + 8 * 3 * 14 /* 8  rest of tiles with 3 directinos */;
                return normalize(counter, minValue, maxValue);
            }


            public static double Heuritic_Proximate1PowerDiffTiles()
            {
                double counter = 0;
                for (int index = 0; index < 16; index++)
                {
                    var neighbors = getValuesOf4NeighborTiles(index);
                    var myValue = neighbors.Item1;
                    var upValue = neighbors.Item2;
                    var downValue = neighbors.Item3;
                    var leftValue = neighbors.Item4;
                    var rightValue = neighbors.Item5;

                    if (myValue != -1 && upValue != -1 && Math.Abs(myValue - upValue) == 1)
                        counter = counter + Math.Max(myValue, upValue);
                    if (myValue != -1 && downValue != -1 && Math.Abs(downValue - upValue) == 1)
                        counter = counter + +Math.Max(myValue, downValue); ;
                    if (myValue != -1 && leftValue != -1 && Math.Abs(myValue - leftValue) == 1)
                        counter = counter + +Math.Max(myValue, leftValue); ;
                    if (myValue != -1 && rightValue != -1 && Math.Abs(myValue - rightValue) == 1)
                        counter = counter + +Math.Max(myValue, rightValue); ;
                }
                /* all tiles aren't 1 power different */
                var minValue = 0;
                /* all tiles are 1 power difference with maximum 2^14 */
                var maxValue = 4 * 4 * 14 /* 4 center tiles with 4 directinos */
                               + 4 * 2 * 14/* 4 edge tiles with 2 directinos */
                               + 8 * 3 * 14 /* 8  rest of tiles with 3 directinos */;
                return normalize(counter, minValue, maxValue);
            }


            public static double Heuritic_NumberOfEmptyCells()
            {
                var available = Game.availableSpace();
                var numOfAvailableSpaces = available.Count();
                return normalize(numOfAvailableSpaces, 14.0);
            }

            #endregion


        }    

        public static void Main(string[] args)
        {
           
           SholdPrint = true;
           OnlyGame = false;
           SholdStopEveryMove = true;
           BestPlayer = true;
           ExpectAddedTile = false;
           numOfGames = 1;

            /*
            List<double> Qualities = new List<double>();
 
            for (int i = 0; i < 100; i++)
            {
                Qualities.Add(Evaluator.evaluate(Evaluator.getRandomWeights(), numOfGames));
            }

            double MaxValue = Qualities.Max();
            double MinValue = Qualities.Min();

            Console.WriteLine("numOfGames:" + numOfGames + " ,greatest diff is:" + (MaxValue - MinValue));
            */
            Console.WriteLine("quality is:" + Evaluator.evaluate(Evaluator.getRandomWeights(), numOfGames));
            Console.Read();
            Console.Read();
   
        }


    }
}
    

