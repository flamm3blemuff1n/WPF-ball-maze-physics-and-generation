using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class Cell
    {
        public double X { get; }
        public double Z { get; }
        public bool Visited { get; set; }

        public Cell(double x, double z)
        {
            this.X = x;
            this.Z = z;
            Visited = false;
        }
    }
}
