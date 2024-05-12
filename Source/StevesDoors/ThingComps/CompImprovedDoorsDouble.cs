using System.Collections.Generic;
using Verse;
using UnityEngine;

namespace StevesDoors
{
    public class CompImprovedDoorsDouble : CompImprovedDoors
    {
        public CompProperties_ImprovedDoorsDouble Props => (CompProperties_ImprovedDoorsDouble)props;
        public Building_UnmirroredMultiTileDoor Door;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            Door = parent as Building_UnmirroredMultiTileDoor;
        }

        public override void PostDraw()
        {
            base.PostDraw();

            if (Props != null && Props.extraDoorGraphics != null)
            {
                foreach (var gD in Props.extraDoorGraphics)
                {
                    _fadeMultiplier = 1f - (Door.OpenPct * gD.fadeFactor * _accentColor.a);
                    _isAccentGraphic = gD.isAccentGraphic;
                    Graphic graphic = gD.Graphic;
                    Material mat = graphic.MatSingle;

                    float archFactor = gD.xMoveAmount * gD.archFactor;

                    switch (_rotation.AsInt)
                    {
                        case 0: // door facing South
                            float xMoveS = gD.isLeftSideGraphic ? -gD.xMoveAmount : gD.xMoveAmount;
                            float zMoveS = gD.shouldArch && gD.isLeftSideGraphic ? Mathf.Lerp(-archFactor, archFactor, Door.OpenPct) :
                                          gD.shouldArch && !gD.isLeftSideGraphic ? Mathf.Lerp(archFactor, -archFactor, Door.OpenPct) : 0f;
                            _moveDir = new Vector3(xMoveS, 0f, zMoveS);
                            break;

                        case 1: // door facing West
                            float zMoveW = gD.isLeftSideGraphic ? gD.xMoveAmount : -gD.xMoveAmount;
                            float xMoveW = gD.shouldArch && gD.isLeftSideGraphic ? Mathf.Lerp(-archFactor, archFactor, Door.OpenPct) :
                                          gD.shouldArch && !gD.isLeftSideGraphic ? Mathf.Lerp(archFactor, -archFactor, Door.OpenPct) : 0f;
                            _moveDir = new Vector3(xMoveW, 0f, zMoveW);
                            break;

                        case 2: // door facing North
                            float xMoveN = gD.isLeftSideGraphic ? gD.xMoveAmount : -gD.xMoveAmount;
                            float zMoveN = gD.shouldArch && gD.isLeftSideGraphic ? Mathf.Lerp(archFactor, -archFactor, Door.OpenPct) :
                                          gD.shouldArch && !gD.isLeftSideGraphic ? Mathf.Lerp(-archFactor, archFactor, Door.OpenPct) : 0f;
                            _moveDir = new Vector3(xMoveN, 0f, zMoveN);
                            break;

                        case 3: // door facing East
                            float zMoveE = gD.isLeftSideGraphic ? -gD.xMoveAmount : gD.xMoveAmount;
                            float xMoveE = gD.shouldArch && gD.isLeftSideGraphic ? Mathf.Lerp(archFactor, -archFactor, Door.OpenPct) :
                                          gD.shouldArch && !gD.isLeftSideGraphic ? Mathf.Lerp(-archFactor, archFactor, Door.OpenPct) : 0f;
                            _moveDir = new Vector3(xMoveE, 0f, zMoveE);
                            break;

                        default:
                            _moveDir = Vector3.zero;
                            break;
                    }
                    DrawExtraDoorGraphics(_moveDir, gD.doorIrisMaxAngle, gD.spinFactor, gD.shouldFade, _fadeMultiplier, Door.OpenPct, mat, gD.drawSize, _isAccentGraphic);
                }
            }
        }

        private void DrawExtraDoorGraphics(Vector3 xMoveAmount, float irisMaxAngle, float spinFactor, bool shouldFade, float opacity, float openPct, Material mat, Vector3 drawSize, bool isAccent)
        {
            Vector3 drawPos = parent.DrawPos + xMoveAmount * openPct;

            // Adjust draw size based on door def and open percentage (double laser doors only)
            if (_ext != null && _ext.isLaserDoor)
            {
                if (openPct > 0f)
                {
                    // Calculate the new width based on the open percentage
                    float newWidth = drawSize.x * (1f - openPct);
                    drawSize.x = Mathf.Max(newWidth, 0.5f);
                }
            }

            float rotationAngle = irisMaxAngle * Door.OpenPct;

            Matrix4x4 matrix = Matrix4x4.TRS(drawPos, _rotation.AsQuat * Quaternion.Euler(0f, rotationAngle * spinFactor, 0f), new Vector3(drawSize.x, 1f, drawSize.y));
            Material finalMat = shouldFade ? FadedMaterialPool.FadedVersionOf(mat, opacity) : mat;

            _mPB.Clear();
            if (_ext != null && _ext.isLaserDoor)
            {
                _mPB.SetColor("_Color", new Color(_doorColor.r, _doorColor.g, _doorColor.b, opacity));
            }
            else if (isAccent)
            {
                _mPB.SetColor("_Color", new Color(_accentColor.r, _accentColor.g, _accentColor.b, (_showAccentGraphics ? 1f : 0f) * opacity));
            }
            else
            {
                _mPB.SetColor("_Color", new Color(mat.color.r, mat.color.g, mat.color.b, opacity));
            }
            Graphics.DrawMesh(MeshPool.plane10, matrix, finalMat, 0, null, 0, _mPB);
        }

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref _doorColor, "_doorColor", Color.white);
            Scribe_Values.Look(ref _accentColor, "_accentColor", Color.white);
            Scribe_Values.Look(ref _showAccentGraphics, "_showAccentGraphics", true);
            Scribe_Values.Look(ref _fadeMultiplier, "_fadeMultiplier");
            Scribe_Values.Look(ref _moveDir, "_moveDir");
        }
    }

    public class CompProperties_ImprovedDoorsDouble : CompProperties
    {
        public List<GraphicDataEnhancedDoors> extraDoorGraphics = null;

        public CompProperties_ImprovedDoorsDouble() => compClass = typeof(CompImprovedDoorsDouble);

        public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
        {
            foreach (string error in base.ConfigErrors(parentDef))
            {
                yield return error;
            }
            if (extraDoorGraphics == null)
            {
                yield return $"<color={SDLog.ErrorMsgCol}>[Steve's Doors]</color> [CompProperties_ImprovedDoorsDouble] No data found for <extraDoorGraphics>, please provide some.";
            }
        }
    }
}
