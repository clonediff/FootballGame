using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Wrappers
{
    public struct StringWrapper
    {
        public string Value;

        public static implicit operator string(StringWrapper value)
            => value.Value;

        public static implicit operator StringWrapper(string value)
            => new StringWrapper { Value = value };
    }
}
