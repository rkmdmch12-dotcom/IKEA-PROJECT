using System;
using System.Collections.Generic;
using System.Text;

namespace IKEA_PROJECT
{
    public class Edge
    {
        public string Destination { get; set; }
        public double Distance { get; set; }

        public Edge(string destination, double distance)
        {
            Destination = destination;
            Distance = distance;
        }
    }

}
