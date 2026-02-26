from spot import *

spot = Spot()
spot.GetState()
spot.PrintFaults()
spot.PowerOn()

time.sleep(4)
spot.StandUp()
time.sleep(3)
spot.Move(vector2(1,0),1)
time.sleep(3)
spot.Move(vector2(-1,0),1)
spot.SitDown()
time.sleep(3)
#spot.GetImages()

#spot.DropLease()