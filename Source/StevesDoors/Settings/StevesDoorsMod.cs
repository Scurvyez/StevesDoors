using UnityEngine;
using Verse;

namespace StevesDoors
{
    public class StevesDoorsMod : Mod
    {
        StevesDoorsSettings settings;
        public static StevesDoorsMod mod;

        private Color tempLaserDoorColor;
        private bool laserDoorColorDragging = false;

        public StevesDoorsMod(ModContentPack content) : base(content)
        {
            mod = this;
            settings = GetSettings<StevesDoorsSettings>();

            tempLaserDoorColor = settings._laserDoorColor;
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            Listing_Standard list = new();
            Rect viewRect = new Rect(inRect.x, inRect.y, inRect.width, inRect.height);
            list.Begin(viewRect);

            list.Label("<color=#4494E3FF>Door Properties</color>");
            list.Gap(3f);

            list.CheckboxLabeled("SD_EnableLaserDoorRecoloring".Translate(), ref settings._enableLaserDoorRecoloring, "SD_EnableLaserDoorRecoloringDesc".Translate());

            if (settings._enableLaserDoorRecoloring)
            {
                Listing_Standard list2 = new();
                list2.Begin(viewRect);
                list2.Gap(100f);

                float initialVertOffset = 0f;
                float initialHorzOffset = 40f;

                Rect laserDoorRect = new Rect(viewRect.x + initialHorzOffset, viewRect.y + initialVertOffset, 1f, 1f);
                DrawSettingWithTextures(laserDoorRect, "Laser Doors", ref tempLaserDoorColor, ref laserDoorColorDragging);

                list2.End();
            }
            else
            {
                tempLaserDoorColor = settings._laserDoorColor;
            }

            list.End();
        }

        private static void DrawSettingWithTextures(Rect refRect, string label, ref Color value, ref bool currentlyDragging)
        {
            float colorWheelScale = 70f;
            float laserDoorTexScale = 70f;
            float padding = 5f; // padding between the textures and the label

            Widgets.DrawLineHorizontal(refRect.x, refRect.y, colorWheelScale + laserDoorTexScale); // text underscore
            Widgets.Label(new Rect(refRect.x, refRect.y - 25f, 200f, 25f), label); // our text
            Rect laserDoorTexRect = new Rect(refRect.x, (refRect.y + padding), laserDoorTexScale, laserDoorTexScale);

            GUI.DrawTexture(laserDoorTexRect, Assets.LaserDoorTex, ScaleMode.ScaleToFit, true, 1f, value, 0f, 0f);

            refRect.x += laserDoorTexScale + padding;
            refRect.width -= laserDoorTexScale + padding;
            Rect colorWheelRect = new Rect(refRect.x, refRect.y + padding, colorWheelScale, colorWheelScale);
            Widgets.HSVColorWheel(colorWheelRect, ref value, ref currentlyDragging);
        }

        public override void WriteSettings()
        {
            settings._laserDoorColor = tempLaserDoorColor;

            base.WriteSettings();
        }

        public override string SettingsCategory()
        {
            return "SD_ModName".Translate();
        }
    }
}
