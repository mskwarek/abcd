using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkNode
{
    public sealed class CCommutationField
    {
        static readonly CCommutationField instance = new CCommutationField();

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

        public int passOnData(Data.CCharacteristicData data, CPort inputPort)
        {
            //pobieram obecne VPI/VCI z komorki
            int VCI = data.getCAdministrationData().getVCI();
            int VPI = data.getCAdministrationData().getVPI();
            int portnumber = 0;
            
                //na ich podstawie + port wejsciowy, pobieram dane o porcie wyjsciowym
                Data.PortInfo outputPortInfo = CCommutationTable.Instance.getOutputPortInfo(new Data.PortInfo(inputPort.ID, VPI, VCI));
                if (outputPortInfo != null)
                    // pobieram obiekt portu

                    if (CPortManager.Instance.getOutputPort(outputPortInfo.getPortID()).GetType() == typeof(CNetworkPortOut))
                    {
                        CNetworkPortOut outputPort;
                        outputPort = (CNetworkPortOut)CPortManager.Instance.getOutputPort(outputPortInfo.getPortID());
                        //testowe
                        //outputPort.startPort(50101);
                        //---------------
                        portnumber = outputPort.send(data, outputPortInfo, false);
                    }
                    else
                    {
                        CClientPortOut outputPort;
                        outputPort = (CClientPortOut)CPortManager.Instance.getOutputPort(outputPortInfo.getPortID());
                        portnumber = outputPort.send(data);
                    }
                else
                {
                   Logger.CLogger.Instance.print("passOnData","Blad doboru portu wyjsciowego",(int)Logger.CLogger.Modes.error);
                }
            return portnumber;
        }

    }
}
