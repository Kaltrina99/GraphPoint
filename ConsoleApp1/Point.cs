using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Point
    {
        public string Name { get; }
        public int X { get; }
        public int Y { get; }

        public Point(string name, int x, int y)
        {
            Name = name;
            X = x;
            Y = y;
        }
    }
}
