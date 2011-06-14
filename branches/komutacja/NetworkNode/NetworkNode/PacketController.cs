using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Data;
using LinkResourceManager;

namespace NetworkNode
{
    public sealed class PacketController
    {
        private static PacketController cPacketController = new PacketController();
        private Logger.CLogger logger = Logger.CLogger.Instance;
        static readonly object _locker = new object();

        private PacketController()
        {

            logger.print("PacketController",null,(int)Logger.CLogger.Modes.constructor);
            
        }

        public static PacketController Instance
        {
            get
            {
                return cPacketController;
            }
        }

        public CLinkInfo processReceivedData(Dictionary<String, Object> d)
        {
            lock (_locker)
            {
                if (d.ContainsKey("SNPLinkConnectionRequest"))
                {
                    return CLinkResourceManager.Instance.SNPLinkConnectionRequest((CLinkInfo)d["SNP"]);
                }
                else if (d.ContainsKey("SNPLinkConnectionDeallocation"))
                {
                    return CLinkResourceManager.Instance.SNPLinkConnectionDeallocation((CLinkInfo)d["SNP"]);
                }
                else
                    return null;
            }
        }
    }
}
