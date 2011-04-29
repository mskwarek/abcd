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

        public void passOnData(Data.CCharacteristicData data, CPort inputPort)
        {
            int VCI = data.getCAdministrationData().getVCI();
            int VPI = data.getCAdministrationData().getVPI();

            CCommutationTable.Instance.getOutputPortInfo(new Data.PortInfo(inputPort.ID, VPI, VCI));
        }
    }
}
