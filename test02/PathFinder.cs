using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Reflection;
//using System.Drawing.Imaging;
using System.Threading;
using System.Diagnostics;
using System.Timers;
using System.Collections.ObjectModel;

namespace test02
{
    class PathFinder
    {
        public class Cell
        {
            public Objects.Vector2D position = new Objects.Vector2D();
            //public int idx;
            //public List<Cell> neighbors = new List<Cell> { };
            public int g;
            public int h;
            public int F { get { return this.g + this.h; } }
            public Cell cameFrom;
        }

        internal static List<Objects.Vector2D> GetPath(int[,] map, Objects.Vector2D start, Objects.Vector2D end)
        {
            var openSet = new Collection<Cell>();
            var closedSet = new Collection<Cell>();

            Cell startCell = new Cell()
            {
                position = start,
                cameFrom = null,
                g = 0,
                h = GetHeuristicPathLength(start, end)
            };

            openSet.Add(startCell);

            while (openSet.Count > 0)
            {
                var currentCell = openSet.OrderBy(node => node.F).First();

                if (currentCell.position.Equals(end))
                {
                    return GetPathForCell(currentCell);
                }
                openSet.Remove(currentCell);
                closedSet.Add(currentCell);

                foreach (var neighbourCell in GetNeighbours(Generate.map, currentCell, end))
                {
                    if (closedSet.Count(node => node.position.Equals(neighbourCell.position)) > 0) continue;

                    var openCell = openSet.FirstOrDefault(node => node.position.Equals(neighbourCell.position));

                    if (openCell == null) openSet.Add(currentCell);
                    else if (openCell.g > neighbourCell.g)
                    {
                        openCell.cameFrom = currentCell;
                        openCell.g = currentCell.g;
                    }
                }
            }
            return null;
        }

        private static Collection<Cell> GetNeighbours(int[,] map, Cell cell, Objects.Vector2D end)
        {
            var result = new Collection<Cell>();

            Objects.Vector2D[] Vectors = new Objects.Vector2D[4];
            Vectors[0] = new Objects.Vector2D(cell.position.x + 1, cell.position.y);
            Vectors[1] = new Objects.Vector2D(cell.position.x - 1, cell.position.y);
            Vectors[2] = new Objects.Vector2D(cell.position.x, cell.position.y + 1);
            Vectors[3] = new Objects.Vector2D(cell.position.x, cell.position.y - 1);

            foreach (var vector in Vectors)
            {
                if (vector.x < 0 || vector.x >= map.GetLength(0)) continue;
                if (vector.y < 0 || vector.y >= map.GetLength(1)) continue;
                if (map[Convert.ToInt32(vector.x), Convert.ToInt32(vector.y)] != 0) continue;

                var neighbourCell = new Cell()
                {
                    position = vector,
                    cameFrom = cell,
                    g = cell.g + GetDistanceBetweenNeighbours(),
                    h = GetHeuristicPathLength(vector, end)
                };
                result.Add(neighbourCell);
            }

            return result;
        }

        private static List<Objects.Vector2D> GetPathForCell(Cell cell)
        {
            var path = new List<Objects.Vector2D>();
            var currentCell = cell;
            
            while (currentCell != null)
            {
                path.Add(currentCell.position);
                currentCell = currentCell.cameFrom;
            }
            path.Reverse();
            return path;
        }

        private static int GetDistanceBetweenNeighbours()
        {
            return 1;
        }

        private static int GetHeuristicPathLength(Objects.Vector2D from, Objects.Vector2D to)
        {
            return Convert.ToInt32(Math.Abs(from.x - to.x) + Math.Abs(from.y - to.y));
        }
    }
}
