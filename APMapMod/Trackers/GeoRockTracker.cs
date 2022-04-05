using Modding;
using Vasi;

namespace APMapMod.Trackers
{
    public static class GeoRockTracker
    {
        public static void Hook()
        {
            // AP INTEGRATION: Determine if current save is AP
            //if (RandomizerMod.RandomizerMod.RS.GenerationSettings.PoolSettings.GeoRocks) return;

            On.GeoRock.OnEnable += GeoRock_OnEnable;
            On.GeoRock.SetMyID += GeoRock_SetMyID;
        }

        public static void Unhook()
        {
            On.GeoRock.OnEnable -= GeoRock_OnEnable;
            On.GeoRock.SetMyID -= GeoRock_SetMyID;
        }

        private static void GeoRock_OnEnable(On.GeoRock.orig_OnEnable orig, GeoRock self)
        {
            orig(self);

            PlayMakerFSM geoRockFSM = self.gameObject.LocateMyFSM("Geo Rock");

            FsmUtil.AddAction(FsmUtil.GetState(geoRockFSM, "Destroy"), new TrackGeoRock(self.gameObject));
        }

        private static void GeoRock_SetMyID(On.GeoRock.orig_SetMyID orig, GeoRock self)
        {
            orig(self);

            // Rename duplicate ids
            if (self.gameObject.scene.name == "Crossroads_ShamanTemple" && self.gameObject.name == "Geo Rock 2")
            {
                if (self.transform.parent != null)
                {
                    self.geoRockData.id = "_Items/Geo Rock 2";
                }
            }

            if (self.gameObject.scene.name == "Abyss_06_Core" && self.gameObject.name == "Geo Rock Abyss")
            {
                if (self.transform.parent != null)
                {
                    self.geoRockData.id = "_Props/Geo Rock Abyss";
                }
            }
        }
    }
}