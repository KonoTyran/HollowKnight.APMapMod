using HutongGames.PlayMaker;
using APMapMod.Data;
using UnityEngine;

namespace APMapMod.Trackers
{
    public class TrackGeoRock : FsmStateAction
    {
        private readonly GameObject _go;
        private readonly GeoRockData _grd;

        public TrackGeoRock(GameObject go)
        {
            _go = go;
            _grd = _go.GetComponent<GeoRock>().geoRockData;
        }

        public override void OnEnter()
        {
            if (!_grd.id.Contains("-"))
            {
                APMapMod.LS.ObtainedVanillaItems[_grd.id + _grd.sceneName] = true;
            }

            APMapMod.LS.GeoRockCounter ++;

            //APMapMod.Instance.Log("Geo Rock broken");
            //APMapMod.Instance.Log(" ID: " + _grd.id);
            //APMapMod.Instance.Log(" Scene: " + _grd.sceneName);

            Finish();
        }
    }

    public class TrackItem : FsmStateAction
    {
        private readonly string _oName;

        public TrackItem(string oName)
        {
            _oName = oName;
        }

        public override void OnEnter()
        {
            string scene = StringUtils.CurrentNormalScene()??"";

            if (!_oName.Contains("-"))
            {
                APMapMod.LS.ObtainedVanillaItems[_oName + scene] = true;
            }

            //APMapMod.Instance.Log("Item picked up");
            //APMapMod.Instance.Log(" Name: " + _oName);
            //APMapMod.Instance.Log(" Scene: " + scene);

            Finish();
        }
    }
}