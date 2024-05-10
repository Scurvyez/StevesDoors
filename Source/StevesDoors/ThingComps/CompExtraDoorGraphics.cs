using System.Collections.Generic;
using Verse;
using UnityEngine;

namespace StevesDoors
{
    public class CompExtraDoorGraphics : ThingComp
    {
        public CompProperties_ExtraDoorGraphics Props => (CompProperties_ExtraDoorGraphics)props;
        public Building_UnmirroredDoor Door;

        private bool _isLaserDoor;
        private Color _doorColor = Color.white;
        private CompProperties_EnhancedDoorGraphics _compEnhancedDoor;
        private readonly MaterialPropertyBlock _mPB = new();
        private Rot4 _rotation;
        private float _fadeMultiplier;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            Door = parent as Building_UnmirroredDoor;
            _rotation = parent.Rotation;
            _compEnhancedDoor = parent.def.GetCompProperties<CompProperties_EnhancedDoorGraphics>();
            if (Door != null && Door.def == SDDefOf.SD_LaserDoorDefault)
            {
                _isLaserDoor = true;
            }
        }

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref _doorColor, "_doorColor");
        }

        public override void PostDraw()
        {
            base.PostDraw();

            if (Props != null && Props.extraDoorGraphics != null)
            {
                foreach (var gD in Props.extraDoorGraphics)
                {
                    _fadeMultiplier = 1f - (Door.OpenPct * gD.fadeFactor);
                    Graphic graphic = gD.Graphic;
                    Material mat = graphic.MatSingle;

                    Vector3 moveDir;
                    float archFactor = gD.xMoveAmount * gD.archFactor;

                    switch (_rotation.AsInt)
                    {
                        case 0: // door facing South
                            float xMoveS = gD.isLeftSideGraphic ? -gD.xMoveAmount : gD.xMoveAmount;
                            float zMoveS = gD.shouldArch && gD.isLeftSideGraphic ? Mathf.Lerp(-archFactor, archFactor, Door.OpenPct) :
                                          gD.shouldArch && !gD.isLeftSideGraphic ? Mathf.Lerp(archFactor, -archFactor, Door.OpenPct) : 0f;
                            moveDir = new Vector3(xMoveS, 0f, zMoveS);
                            break;

                        case 1: // door facing West
                            float zMoveW = gD.isLeftSideGraphic ? gD.xMoveAmount : -gD.xMoveAmount;
                            float xMoveW = gD.shouldArch && gD.isLeftSideGraphic ? Mathf.Lerp(-archFactor, archFactor, Door.OpenPct) :
                                          gD.shouldArch && !gD.isLeftSideGraphic ? Mathf.Lerp(archFactor, -archFactor, Door.OpenPct) : 0f;
                            moveDir = new Vector3(xMoveW, 0f, zMoveW);
                            break;

                        case 2: // door facing North
                            float xMoveN = gD.isLeftSideGraphic ? gD.xMoveAmount : -gD.xMoveAmount;
                            float zMoveN = gD.shouldArch && gD.isLeftSideGraphic ? Mathf.Lerp(archFactor, -archFactor, Door.OpenPct) :
                                          gD.shouldArch && !gD.isLeftSideGraphic ? Mathf.Lerp(-archFactor, archFactor, Door.OpenPct) : 0f;
                            moveDir = new Vector3(xMoveN, 0f, zMoveN);
                            break;

                        case 3: // door facing East
                            float zMoveE = gD.isLeftSideGraphic ? -gD.xMoveAmount : gD.xMoveAmount;
                            float xMoveE = gD.shouldArch && gD.isLeftSideGraphic ? Mathf.Lerp(archFactor, -archFactor, Door.OpenPct) :
                                          gD.shouldArch && !gD.isLeftSideGraphic ? Mathf.Lerp(-archFactor, archFactor, Door.OpenPct) : 0f;
                            moveDir = new Vector3(xMoveE, 0f, zMoveE);
                            break;

                        default:
                            moveDir = Vector3.zero;
                            break;
                    }
                    DrawExtraDoorGraphics(moveDir, gD.spinFactor, gD.shouldFade, _fadeMultiplier, Door.OpenPct, mat, gD.drawSize);
                }
            }
        }

        private void DrawExtraDoorGraphics(Vector3 xMoveAmount, float spinFactor, bool shouldFade, float opacity, float openPct, Material mat, Vector3 drawSize)
        {
            float curOpenPct = Door.OpenPct;
            Rot4 rotation = parent.Rotation;
            Quaternion rotationQuat = rotation.AsQuat;
            Vector3 drawPos = parent.DrawPos + xMoveAmount * openPct;

            float maxRotation = _compEnhancedDoor?.doorIrisMaxAngle ?? 0f;
            float rotationAngle = maxRotation * curOpenPct;
            Matrix4x4 matrix = Matrix4x4.TRS(drawPos, rotationQuat * Quaternion.Euler(0f, rotationAngle * spinFactor, 0f), new Vector3(drawSize.x, 1f, drawSize.y));
            Material finalMat = shouldFade ? FadedMaterialPool.FadedVersionOf(mat, opacity) : mat;

            if (_isLaserDoor)
            {
                _mPB.Clear();
                _mPB.SetColor("_Color", new Color(_doorColor.r, _doorColor.g, _doorColor.b, opacity));
            }
            Graphics.DrawMesh(MeshPool.plane10, matrix, finalMat, 0, null, 0, _mPB);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo gizmo in base.CompGetGizmosExtra())
            {
                yield return gizmo;
            }
            if (parent.def == SDDefOf.SD_LaserDoorDefault)
            {
                yield return new Command_Action
                {
                    defaultLabel = "Change Door Color",
                    defaultDesc = "Change the color of the door.",
                    icon = ContentFinder<Texture2D>.Get("UI/Commands/ChangeColor"),
                    action = () =>
                    {
                        Find.WindowStack.Add(new Dialogue_DoorColorPicker(_doorColor, newColor => _doorColor = newColor));
                    }
                };
            }
        }
    }

    public class CompProperties_ExtraDoorGraphics : CompProperties
    {
        public List<GraphicDataEnhancedDoors> extraDoorGraphics = null;

        public CompProperties_ExtraDoorGraphics() => compClass = typeof(CompExtraDoorGraphics);

        public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
        {
            foreach (string error in base.ConfigErrors(parentDef))
            {
                yield return error;
            }
            if (extraDoorGraphics == null)
            {
                yield return $"<color={SDLog.ErrorMsgCol}>[Steve's Doors]</color> [CompProperties_ExtraDoorGraphics] No data found for <extraDoorGraphics>, please provide some.";
            }
        }
    }
}
