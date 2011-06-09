using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManagementLayer
{
    public sealed class IdentifiersManager
    {
        static readonly IdentifiersManager instance = new IdentifiersManager();

        public static IdentifiersManager Instance
        {
            get
            {
                return instance;
            }
        }

        Queue<int> VCIPole = new Queue<int>();
        Queue<int> VPIPoleNNI = new Queue<int>();
        Queue<int> VPIPoleUNI = new Queue<int>();


        IdentifiersManager()
        {
            
            for (int i = 0; i <= Data.CAdministrationData.VCI_MAX; i++)
            {
                VCIPole.Enqueue(i);
            }

            for (int i = 0; i <= Data.CAdministrationData.VPI_NNI_MAX; i++)
            {
                VPIPoleNNI.Enqueue(i);
            }

            for (int i = 0; i <= Data.CAdministrationData.VPI_UNI_MAX; i++)
            {
                VPIPoleUNI.Enqueue(i);
            }
        }

        //NNI VPI
        public int getFreeNNIVPI()
        {
            int VPI;

            VPI = VPIPoleNNI.Dequeue();

            return VPI;
        }

        public void releaseNNIVPI(int VPI)
        {
            VPIPoleNNI.Enqueue(VPI);
        }

        //UNI VPI
        public int getFreeUNIVPI()
        {
            int VPI;

            VPI = VPIPoleUNI.Dequeue();

            return VPI;
        }

        public void releaseUNIVPI(int VPI)
        {
            VPIPoleUNI.Enqueue(VPI);
        }

        //VCI
        public int getFreeVCI()
        {
            int VCI;

            VCI = VCIPole.Dequeue();

            return VCI;
        }

        public void releaseVCI(int VCI)
        {
            VCIPole.Enqueue(VCI);
        }
    }
}
