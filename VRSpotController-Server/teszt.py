from spot import *

def GetAndSaveImages():
    spot = Spot()
    #sources = [ImgSource.BACK, ImgSource.LEFT, ImgSource.FRONT_LEFT, ImgSource.FRONT_RIGHT, ImgSource.RIGHT]
    spot.StandUp()
    time.sleep(3)
    sources = spot.ListCameras()
    imgs = spot.GetImages(sources)
    for i in range(0,len(imgs)):
        Spot.SaveImage(imgs[i], f"{sources[i]}.jpg")

    time.sleep(3)
    spot.SitDown()

def GetState():
    spot = Spot()
    state = spot.GetState()
    battery = Spot.GetBattery(state)
    print(f"Battery:{battery}")
    rotation = Spot.GetRotation(state)
    print(f"Rotation:{rotation}")
    position = Spot.GetPosition(state)
    print(f"Position:{position}")

#GetState()
GetAndSaveImages()
