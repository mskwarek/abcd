using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data
{[Serializable]
    public class CPNNITable : IEquatable<CPNNITable>
    {
        // typ node'a
        private String _NodeType;

            public String NodeType
            {
             get { return _NodeType; }
             set { _NodeType = value; }
            }

        // numer node'a
        private int _NodeNumber;

            public int NodeNumber
            {
             get { return _NodeNumber; }
             set { _NodeNumber = value; }
            }

            // numer portu node'a
        private int _NodePortNumber;

            public int NodePortNumberSender
            {
                get { return _NodePortNumber; }
                set { _NodePortNumber = value; }
            }
            // numer node'a sasiada
        private int _NeighbourNodeNumber;

        public int NeighbourNodeNumber
        {
            get { return _NeighbourNodeNumber; }
            set { _NeighbourNodeNumber = value; }
        }
        // numer portu node'a sasiada
        private int _NeighbourPortNumber;

            public int NeighbourPortNumberReciever
            {
                get { return _NeighbourPortNumber; }
                set { _NeighbourPortNumber = value; }
            }


        // typ node'a sasiada
        private String _NeighbourNodeType;

        public String NeighbourNodeType
        {
            get { return _NeighbourNodeType; }
            set { _NeighbourNodeType = value; }
        }

        // stan node'a sasiada
        private bool _isNeighbourActive;

        public bool IsNeighbourActive
        {
            get { return _isNeighbourActive; }
            set { _isNeighbourActive = value; }
        }

        private string domainName;

        public string DomainName
        {
            get { return domainName; }
            set { domainName = value; }
        }

        public CPNNITable(int NodeNumber, String NodeType, int NodePortNumber, int NeighbourNodeNumber, String NeighbourNodeType,int NeighbourPortNumber ,string domainName,bool isActive) 
        {
            this.NodeNumber = NodeNumber;
            this.NodeType = NodeType;
            this.NodePortNumberSender = NodePortNumber;
            this.NeighbourNodeNumber = NeighbourNodeNumber;
            this.NeighbourNodeType = NeighbourNodeType;
            this.NeighbourPortNumberReciever = NeighbourPortNumber;
            this.domainName = domainName;
            this.IsNeighbourActive = isActive;
        }

        public bool Equals(CPNNITable other)
        {

            if (this.NodePortNumberSender == other.NodePortNumberSender && this.NodeType == other.NodeType && this.NodeNumber == other.NodeNumber)
            {
                return true;
            }
            else
            {
                return false;
            }

        }


        public override int GetHashCode()
        {
            return this.NodePortNumberSender.GetHashCode() + this.NodeNumber.GetHashCode() + this.NodeType.GetHashCode();
        }

    
    }
}
