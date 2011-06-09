start ManagementLayer.exe.lnk 1
start ManagementLayer.exe.lnk 2
sleep -m 1000
start ControlPlane.exe.lnk 1
#start ControlPlane.exe.lnk 2
sleep -m 1000
start ClientNode.exe.lnk 1 1
start NetworkNode.exe.lnk 2 1
start NetworkNode.exe.lnk 3 1
start ClientNode.exe.lnk 4 1
start NetworkNode.exe.lnk 5 2
start NetworkNode.exe.lnk 6 2
start ClientNode.exe.lnk 7 2


