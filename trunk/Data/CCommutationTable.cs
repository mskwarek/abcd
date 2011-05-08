using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Data
{
    [Serializable]
    public class CCommutationTable : Dictionary<PortInfo, PortInfo>
    {
        public CCommutationTable(IEqualityComparer<PortInfo> equalityComparer)
            : base(equalityComparer)
        { }

        protected CCommutationTable(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
