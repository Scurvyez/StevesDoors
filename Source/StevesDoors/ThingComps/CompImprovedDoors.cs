using System.Collections.Generic;
using StevesDoors.Utils;
using UnityEngine;
using Verse;

namespace StevesDoors
{
    public class CompImprovedDoors : ThingComp
    {
        public bool _isAccentGraphic;
        public bool _showAccentGraphics = true;
        public Color _doorColor = Color.white;
        public Color _accentColor = Color.white;
        public readonly MaterialPropertyBlock _mPB = new();
        public Rot4 _rotation;
        public float _fadeMultiplier;
        public Vector3 _moveDir;
        public ImprovedDoorExtension _ext;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            _ext = parent.def.GetModExtension<ImprovedDoorExtension>();
            _rotation = parent.Rotation;
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo gizmo in base.CompGetGizmosExtra())
            {
                yield return gizmo;
            }
            if (_ext != null)
            {
                if (_ext.isLaserDoor)
                {
                    yield return new Command_Action
                    {
                        defaultLabel = "Door Color",
                        defaultDesc = "Change the color of the door.",
                        icon = ContentFinder<Texture2D>.Get("StevesDoors/UI/Icons/LaserDoorColorPicker_GizmoIcon"),
                        action = () =>
                        {
                            Find.WindowStack.Add(new Dialogue_DoorColorPicker(_doorColor, newColor => _doorColor = newColor));
                        }
                    };
                }
                if (_ext.hasAccentColors)
                {
                    yield return new Command_Action
                    {
                        defaultLabel = "Accent Color",
                        defaultDesc = "Change the color of the doors' accent pieces.",
                        icon = ContentFinder<Texture2D>.Get("StevesDoors/UI/Icons/DoorAccentColorPicker_GizmoIcon"),
                        action = () =>
                        {
                            Find.WindowStack.Add(new Dialogue_DoorAccentsColorPicker(_accentColor, newColor => _accentColor = newColor));
                        }
                    };
                    yield return new Command_Toggle
                    {
                        defaultLabel = "Show Accents",
                        defaultDesc = "Toggle whether or not accent pieces are rendered.",
                        icon = ContentFinder<Texture2D>.Get("StevesDoors/UI/Icons/DoorShowAccents_GizmoIcon"),
                        isActive = () => _showAccentGraphics,
                        toggleAction = delegate
                        {
                            _showAccentGraphics = !_showAccentGraphics;
                        }
                    };
                }
            }
        }
    }
}
