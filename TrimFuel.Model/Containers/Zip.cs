using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Containers
{
    public class Zip
    {
        //TODO: implement different variants by regular expressions
        public Zip(string zip)
        {
            if (!string.IsNullOrEmpty(zip))
            {
                string[] zipParts = zip.Split('-');
                Part1 = zipParts[0];
                Part2 = (zipParts.Length > 1) ? zipParts[1] : null;
            }
        }

        public string Part1 { get; set; }
        public string Part2 { get; set; }
    }
}
