using LibCompute;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Day_15
{
    internal enum DIRECTIONS
    {
        NORTH = 1,
        SOUTH = 2,
        WEST = 3,
        EAST = 4
    }

    internal enum BLOCK_TYPE
    {
        WALL = 0,
        CLEAR = 1,
        OXYGEN = 2
    }

    internal class Node
    {
        public Point Location { get; }
        public BLOCK_TYPE Type { get; set; }

        public List<Node> Neighbours { get; } = new List<Node>();

        public Node(Point location, BLOCK_TYPE type)
        {
            Location = location;
            Type = type;
        }
    }

    internal class Program
    {
        private static Dictionary<Point, Node> _nodes = new Dictionary<Point, Node>();

        private static Node GetNode(Point pt)
        {
            if (!_nodes.ContainsKey(pt))
                _nodes.Add(pt, new Node(pt, BLOCK_TYPE.CLEAR));
            return _nodes[pt];
        }

        private static Node GetNode(int x, int y)
        {
            return GetNode(new Point(x, y));
        }

        private static List<Node> FindBestPath(Node start, Node goal)
        {
            var path = new List<Node>();

            var discovered = new Dictionary<Node, Node>
        {
            { start, null }
        };

            var q = new Queue<Node>();

            q.Enqueue(start);

            while (q.Count > 0)
            {
                var v = q.Dequeue();

                if (v == goal)
                {
                    path.Add(goal);

                    var previous = discovered[goal];
                    do
                    {
                        path.Add(previous);
                        previous = discovered[previous];
                    } while (previous != null);

                    return path;
                }
                foreach (var edge in v.Neighbours)
                {
                    if (!discovered.ContainsKey(edge))
                    {
                        discovered.Add(edge, v);
                        q.Enqueue(edge);
                    }
                }
            }

            return null;
        }

        private static int _bestPathLength = int.MaxValue;
        private static int _lastNodeCount = 0;

        private static int _exploratedThreshold = 200;
        private static int _sameNodeCount = 0;

        private static void PropagateOxygen(Node source)
        {
            var oxygenatedNodes = new List<Node>();
            var notWallNodes = _nodes.Where(x => x.Value.Type != BLOCK_TYPE.WALL).ToList();
            oxygenatedNodes.Add(source);
            int t = 0;

            while (notWallNodes.Count > oxygenatedNodes.Count)
            {
                var toOxygenate = new List<Node>();
                foreach (var oxygenatedNode in oxygenatedNodes)
                {
                    foreach (var neighbour in oxygenatedNode.Neighbours)
                    {
                        if (!oxygenatedNodes.Contains(neighbour))
                        {
                            toOxygenate.Add(neighbour);
                        }
                    }
                }

                foreach (var nd in toOxygenate.Distinct())
                {
                    oxygenatedNodes.Add(nd);
                }

                t++;
            }

            Console.WriteLine($"Time to propagate: {t}");
        }

        private static void Main()
        {
            var io = new IOPipe();
            var computer = new IntcodeComputer("Repair robot", "input", io);

            io.FireEveryNbOutput = 1;

            int x = 0, y = 0;
            int nextX = 0, nextY = 0;

            var start = GetNode(0, 0);

            var previousNode = start;

            var random = new Random();

            io.IntOuputted += (s, e) =>
              {
                  var type = (BLOCK_TYPE)io.ReadOutputInt();
                  var node = GetNode(nextX, nextY);
                  node.Type = type;

                  if (type != BLOCK_TYPE.WALL)
                  {
                      x = nextX;
                      y = nextY;

                      if (!node.Neighbours.Contains(previousNode))
                      {
                          node.Neighbours.Add(previousNode);
                          previousNode.Neighbours.Add(node);
                      }

                      previousNode = node;
                  }

                  if (type == BLOCK_TYPE.OXYGEN)
                  {
                      if (_lastNodeCount < _nodes.Count)
                      {
                          _sameNodeCount = 0;

                          _lastNodeCount = _nodes.Count;
                          var goal = node;
                          var path = FindBestPath(start, node);

                          if (path.Count - 1 < _bestPathLength) // looks like there is only one path to oxygen but i assumed there could be many
                          {
                              _bestPathLength = path.Count - 1;
                              Console.WriteLine($"Path to oxgen: {path.Count - 1}");
                          }
                      }
                      else if (_lastNodeCount == _nodes.Count)
                      {
                          _sameNodeCount++;

                          if (_sameNodeCount >= _exploratedThreshold) //assume the maze is explored when we don't discover any new node for a while
                          {
                              Console.WriteLine("Maze is explored completely");
                              PropagateOxygen(node);
                              computer.Abort();
                          }
                      }
                  }
              };

            io.ReadingInt += (s, e) =>
              {
                  DIRECTIONS direction;
                  do
                  {
                      direction = (DIRECTIONS)random.Next(1, 5);

                      switch (direction)
                      {
                          case DIRECTIONS.EAST:
                              nextX = x + 1;
                              nextY = y;
                              break;

                          case DIRECTIONS.WEST:
                              nextX = x - 1;
                              nextY = y;
                              break;

                          case DIRECTIONS.NORTH:
                              nextY = y - 1;
                              nextX = x;
                              break;

                          case DIRECTIONS.SOUTH:
                              nextY = y + 1;
                              nextX = x;
                              break;
                      }
                  } while (_nodes.ContainsKey(new Point(nextX, nextY)) && GetNode(nextX, nextY).Type == BLOCK_TYPE.WALL); //check in memory instead of using the slower computer

                  io.InputInt((int)direction);
              };

            computer.Run();
        }
    }
}