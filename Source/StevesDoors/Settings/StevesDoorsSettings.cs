using UnityEngine;
using Verse;

namespace StevesDoors
{
    /*
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

        public static Color LaserDoorDoubleColor
        {
            get
            {
                return _instance._laserDoorDoubleColor;
            }
        }

        public bool _enableLaserDoorRecoloring = false;
        public Color _laserDoorColor = Color.white;
        public Color _laserDoorDoubleColor = Color.white;

        public StevesDoorsSettings()
        {
            _instance = this;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref _enableLaserDoorRecoloring, "enableLaserDoorRecoloring", false);
            Scribe_Values.Look(ref _laserDoorColor, "laserDoorColor", Color.white);
            Scribe_Values.Look(ref _laserDoorDoubleColor, "laserDoorDoubleColor", Color.white);
        }
    }
    */
}
