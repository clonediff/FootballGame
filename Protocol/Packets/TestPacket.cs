using Protocol.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Packets
{
    public class TestPacket
    {
        [Field(0)]
        public int TestNumber;

        [Field(1)]
        public double TestDouble;

        [Field(2)]
        public bool TestBoolean;

        public override string ToString()
        {
            return $"TestNumber: {TestNumber}\tTestDouble: {TestDouble}\tTestBoolean: {TestBoolean}";
        }
    }
}
