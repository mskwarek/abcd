﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPlane
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Control Plane";
            CNetworkCallController.Instance.CNetworkCallControllerStart();
        }
    }
}
