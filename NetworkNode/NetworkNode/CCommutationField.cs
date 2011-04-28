using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkNode
{
    public sealed class CCommutationField
    {
        static readonly CCommutationField instance = new CCommutationField();

        static CCommutationField()
        {
        }

        CCommutationField()
        {
        }

        public static CCommutationField Instance
        {
            get
            {
                return instance;
            }
        }
    }
}
