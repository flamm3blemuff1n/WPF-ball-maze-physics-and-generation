using Opdracht1;
using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace Library
{
    public class Maze
    {
        public List<Cube> Walls { get; private set; }

        private ContainerUIElement3D wallContainer;
        private Dictionary<String, Cube> wallsWithId = new Dictionary<string, Cube>();

        private enum DirectionType { up, down, left, right, stay };

        private int CELLS;
        private double cellWidth;
        private Dictionary<string, Cell> cells;

        private Random RandomNumber = new Random();

        public Maze(int cells, ContainerUIElement3D container)
        {
            this.CELLS = cells;
            this.wallContainer = container;
            this.wallContainer.Children.Clear();
            CreateWalls();
            CreateCells();

            Console.WriteLine("NEEEEEEEEEEEEEEEEEW");
        }

        private void CreateWalls()
        {
            Walls = new List<Cube>();
            cellWidth = (101 / (double)CELLS) - 1;

            int[,] wallCoords = new int[,] {
                { -51, 0, -51, 102, 2, 1 }, { 50, 0, -51, 102, 2, 1 }, { -50, 0, -51, 1, 2, 100 }, { -50, 0, 50, 1, 2, 100 } //4 sides
            };

            for (int i = 0; i < wallCoords.GetLength(0); i++)
            {
                Cube wall = new Cube(wallCoords[i, 0], wallCoords[i, 1], wallCoords[i, 2], wallCoords[i, 3], wallCoords[i, 4], wallCoords[i, 5]);
                Walls.Add(wall);
                wallContainer.Children.Add(wall.Model);
            }

            for (int i = 0; i < CELLS - 1; i++)
            {
                for (int j = 0; j < CELLS; j++)
                {
                    double z = -50 + (j * (cellWidth + 1));
                    double x = -50 + ((i + 1) * cellWidth) + (i * 1);

                    Console.WriteLine(x + "," + z);

                    Cube wall = new Cube(x, 0, z, cellWidth+1, 2, 1);
                    wallsWithId.Add(x + "," + z, wall);
                    Walls.Add(wall);
                    wallContainer.Children.Add(wall.Model);
                }
            }

            for (int i = 0; i < CELLS - 1; i++)
            {
                for (int j = 0; j < CELLS; j++)
                {
                    double z = -50 + ((i + 1) * cellWidth) + (i * 1);
                    double x = -50 + (j * (cellWidth + 1));

                    Console.WriteLine(x + "," + z);

                    Cube wall = new Cube(x, 0, z, 1, 2, cellWidth+1);
                    wallsWithId.Add(x + "," + z, wall);
                    Walls.Add(wall);
                    wallContainer.Children.Add(wall.Model);
                }
            }
        }

        private void CreateCells()
        {
            cells = new Dictionary<string, Cell>();

            for (int i = -1; i <= CELLS; i++)
            {
                for (int j = -1; j <= CELLS; j++)
                {
                    Cell cell = new Cell(i, j);
                    if (i == -1 || j == -1 || i == CELLS || j == CELLS) cell.Visited = true;
                    cells.Add(i + "," + j, cell);
                }
            }
        }

        public void GenerateRecursiveBacktrack()
        {
            Cell currentCell = cells["0,0"];
            int CellsToDo = CELLS * CELLS;
            Random Direction = new Random();
            Queue<Cell> queue = new Queue<Cell>();

            while (CellsToDo > 0)
            {
                currentCell.Visited = true;

                switch (GetRandomDirection(currentCell))
                {
                    case DirectionType.stay:
                        if (queue.Count > 0) currentCell = queue.Dequeue();
                        else CellsToDo -= 1;
                        break;
                    case DirectionType.up: //go up, remove wall from current
                        queue.Enqueue(currentCell);
                        RemoveWall(currentCell, DirectionType.up);
                        currentCell = cells[currentCell.X + "," + (currentCell.Z + 1)];
                        CellsToDo -= 1;
                        break;
                    case DirectionType.down: //go down, remove wall from next
                        queue.Enqueue(currentCell);
                        currentCell = cells[currentCell.X + "," + (currentCell.Z - 1)];
                        RemoveWall(currentCell, DirectionType.up);
                        CellsToDo -= 1;
                        break;
                    case DirectionType.left: //go left, remove wall from current
                        queue.Enqueue(currentCell);
                        RemoveWall(currentCell, DirectionType.left);
                        currentCell = cells[(currentCell.X + 1) + "," + currentCell.Z];
                        CellsToDo -= 1;
                        break;
                    case DirectionType.right: //go right, remove wall from next
                        queue.Enqueue(currentCell);
                        currentCell = cells[(currentCell.X - 1) + "," + currentCell.Z];
                        RemoveWall(currentCell, DirectionType.left);
                        CellsToDo -= 1;
                        break;
                }
            }
        }

        private DirectionType GetRandomDirection(Cell cell)
        {
            DirectionType direction = DirectionType.stay;
            if (HasUnvisitedNeigbour(cell))
            {
                while (direction == DirectionType.stay)
                {
                    switch (RandomNumber.Next(1, 5))
                    {

                        case 1: //go up z+1
                            if (cell.Z != CELLS && cells[cell.X + "," + (cell.Z + 1)].Visited == false)
                            {
                                direction = DirectionType.up;
                            }
                            break;
                        case 2: //go down z-1 
                            if (cell.Z != 0 && cells[cell.X + "," + (cell.Z - 1)].Visited == false)
                            {
                                direction = DirectionType.down;
                            }
                            break;
                        case 3: //go left x+1
                            if (cell.X != CELLS && cells[(cell.X + 1) + "," + cell.Z].Visited == false)
                            {
                                direction = DirectionType.left;
                            }
                            break;
                        case 4: //go right x-1
                            if (cell.X != 0 && cells[(cell.X - 1) + "," + cell.Z].Visited == false)
                            {
                                direction = DirectionType.right;
                            }
                            break;
                    }
                }
            }
            return direction;
        }

        private bool HasUnvisitedNeigbour(Cell cell)
        {
            if (cells[cell.X + "," + (cell.Z + 1)].Visited == false
                || cells[cell.X + "," + (cell.Z - 1)].Visited == false
                || cells[(cell.X + 1) + "," + cell.Z].Visited == false
                || cells[(cell.X - 1) + "," + cell.Z].Visited == false) return true;

            return false;
        }

        private void RemoveWall(Cell cell, DirectionType direction)
        {
            if (direction == DirectionType.up)
            {
                double z = -50 + ((cell.Z + 1) * cellWidth) + (cell.Z * 1);
                double x = -50 + (cell.X * (cellWidth + 1));

                Console.WriteLine("REMOVE; " + x + "," + z);

                wallContainer.Children.Remove(wallsWithId[x + "," + z].Model);
                Walls.Remove(wallsWithId[x + "," + z]);
            }
            else if (direction == DirectionType.left)
            {
                double z = -50 + (cell.Z * (cellWidth + 1));
                double x = -50 + ((cell.X + 1) * cellWidth) + (cell.X * 1);

                Console.WriteLine("REMOVE; " + x + "," + z);

                wallContainer.Children.Remove(wallsWithId[x + "," + z].Model);
                Walls.Remove(wallsWithId[x + "," + z]);
            }
        }
    }
}
