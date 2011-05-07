using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data
{
    [Serializable]
    public class CCommutationTable : Dictionary<PortInfo, PortInfo>
    {
        public CCommutationTable(IEqualityComparer<PortInfo> equalityComparer)
            : base(equalityComparer)
        { }
    }
}
