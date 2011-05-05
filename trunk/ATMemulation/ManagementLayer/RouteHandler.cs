using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManagementLayer
{
    public sealed class RouteHandler
    {
        static readonly RouteHandler instance = new RouteHandler();

        public static RouteHandler Instance
        {
            get
            {
                return instance;
            }
        }

        List<RouteEngine.Route> routes = new List<RouteEngine.Route>();
        Queue<int> VCIPole;
        Queue<int> VPIPole;


        RouteHandler()
        {
            for (int i = 0; i <= Data.CAdministrationData.VCI_MAX; i++)
            {
                VCIPole.Enqueue(i);
            }

            for (int i = 0; i <= Data.CAdministrationData.VPI_NNI_MAX; i++)
            {
                VPIPole.Enqueue(i);
            }

        }
        //zarzadca ustanowionych sciezek
        //dobiera VPI VCI
        //zwalnia zasoby
        public void setNewRouteParams(RouteEngine.Route route)
        {
            // klasa ktora ma PortInfo + NodeNumber

            //rezerwacja zasobow
            foreach (CLink cLink in route.Connections)
            {
                CShortestPathCalculatorWrapper.Instance.reserveCLink(cLink);

                //stworz wpis do tablicy zawierajacy nodenumber portnumber vpi vci

            }

            //Stworz i wyslij tablice komutacji do poszczegolnych wezlow

        }


        public void unsetNewRouteParams(RouteEngine.Route route)
        {
            //przywrocenie zasobow do puli
            foreach (CLink cLink in route.Connections)
            {
                CShortestPathCalculatorWrapper.Instance.releaseCLink(cLink);
            }
        }
    }
}
