using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPlane
{
    class CConnectionControl
    {
        //metoda do wymiany pomiedzy CC
        // parametry: para SNP, SNP i SNPP, para SNPP
        // zwraca: potwierdzenie
        public bool PeerCoordinationOut(int SNP_s, int SNP_d)
        {
            bool confirmation=false;
            return confirmation;
        }
        //metoda zadajaca zestawienia polaczenia. uzywana w trybie hierarchicznym
        //parametry: para SNP
        //zwraca: polaczenie podsieciowe
        public void ConnectionRequestOut(int SNP_s, int SNP_d)
        {}
        

        //metoda kierowana do RC by uzyskac sciezke pomiedzy dwoma punktami 
        //parametry: 'unresolved route fragment'
        //zwraca: zbior SNPP
        public void RouteTableQuery()
        {}

        //metoda do zestawienia polaczenia? kierowana do LRM
        //parametry:brak
        //zwraca: link connection ( pare SNP)
        public void LinkConnectionRequest()
        {
        }

        public void LinkConnectionDeallocation(int SNP)
        { }
    }
}
