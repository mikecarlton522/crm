using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Containers
{
    public class Phone
    {
        //TODO: implement "111-111-1111", (111)111-1111 ... as well (regular expressions)
        public Phone(string phone)
        {
            if (!string.IsNullOrEmpty(phone) && phone.Length == 10)
            {
                Code = phone.Substring(0, 3);
                Part1 = phone.Substring(3, 3);
                Part2 = phone.Substring(6, 4);
            }
        }

        public Phone(string code, string part1, string part2)
        {
            Code = code;
            Part1 = part1;
            Part2 = part2;
        }

        public string Code { get; set; }
        public string Part1 { get; set; }
        public string Part2 { get; set; }

        public override string ToString()
        {
            return string.Format("{0}{1}{2}", Code, Part1, Part2);
        }
    }
}
