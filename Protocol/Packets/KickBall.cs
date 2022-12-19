using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Packets
{
    public class KickBall
    {
        public string UserId { get; set; }
        public Point Direction { get; set; }
        //TODO: public double Power { get; set; }
    }
}
