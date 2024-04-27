using UnityEngine;
using Verse;

namespace StevesDoors
{
    public class StevesDoorsSettings : ModSettings
    {
        private static StevesDoorsSettings _instance;

        public static bool EnableLaserDoorRecoloring
        {
            get
            {
                return _instance._enableLaserDoorRecoloring;
            }
        }
        
        public static Color LaserDoorColor
        {
            get
            {
                return _instance._laserDoorColor;
            }
        }

        public bool _enableLaserDoorRecoloring = false;
        public Color _laserDoorColor = Color.white;
        
        public StevesDoorsSettings()
        {
            _instance = this;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref _enableLaserDoorRecoloring, "enableLaserDoorRecoloring", false);
            Scribe_Values.Look(ref _laserDoorColor, "laserDoorColor", Color.white);
        }
    }
}
