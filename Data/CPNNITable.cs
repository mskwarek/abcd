using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data
{[Serializable]
    public class CPNNITable : IEquatable<CPNNITable>
    {
        private String _NodeType;

            public String NodeType
            {
             get { return _NodeType; }
             set { _NodeType = value; }
            }
        
    
        private int _NodeNumber;

            public int NodeNumber
            {
             get { return _NodeNumber; }
             set { _NodeNumber = value; }
            }
        
        private int _NodePortNumber;

            public int NodePortNumberSender
            {
                get { return _NodePortNumber; }
                set { _NodePortNumber = value; }
            }
        private int _NeighbourNodeNumber;

        public int NeighbourNodeNumber
        {
            get { return _NeighbourNodeNumber; }
            set { _NeighbourNodeNumber = value; }
        }

        private int _NeighbourPortNumber;

            public int NeighbourPortNumberReciever
            {
                get { return _NeighbourPortNumber; }
                set { _NeighbourPortNumber = value; }
            }

            

        private String _NeighbourNodeType;

        public String NeighbourNodeType
        {
            get { return _NeighbourNodeType; }
            set { _NeighbourNodeType = value; }
        }
        private bool _isNeighbourActive;

        public bool IsNeighbourActive
        {
            get { return _isNeighbourActive; }
            set { _isNeighbourActive = value; }
        }


        public CPNNITable(int NodeNumber, String NodeType, int NodePortNumber, int NeighbourNodeNumber, String NeighbourNodeType,int NeighbourPortNumber ,bool isActive) 
        {
            this.NodeNumber = NodeNumber;
            this.NodeType = NodeType;
            this.NodePortNumberSender = NodePortNumber;
            this.NeighbourNodeNumber = NeighbourNodeNumber;
            this.NeighbourNodeType = NeighbourNodeType;
            this.NeighbourPortNumberReciever = NeighbourPortNumber;
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
