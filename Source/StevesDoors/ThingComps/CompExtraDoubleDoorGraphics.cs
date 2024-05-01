using System.Collections.Generic;
using Verse;
using UnityEngine;

namespace StevesDoors
{
    public class CompExtraDoubleDoorGraphics : ThingComp
    {
        public CompProperties_ExtraDoubleDoorGraphics Props => (CompProperties_ExtraDoubleDoorGraphics)props;
        private CompProperties_EnhancedDoorGraphics CompEnhancedDoor;
        public Building_UnmirroredMultiTileDoor Door;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            CompEnhancedDoor = parent.def.GetCompProperties<CompProperties_EnhancedDoorGraphics>();
            Door = parent as Building_UnmirroredMultiTileDoor;
        }

        public override void PostDraw()
        {
            base.PostDraw();

            if (Props != null && Props.extraDoorGraphics != null)
            {
                Rot4 rotation = parent.Rotation;

                foreach (var gD in Props.extraDoorGraphics)
                {
                    float fadeMultiplier = 1f - (Door.OpenPct * gD.fadeFactor);
                    Graphic graphic = gD.Graphic;
                    Material mat = graphic.MatSingle;

                    Vector3 moveDir;
                    float archFactor = gD.xMoveAmount * gD.archFactor;

                    switch (rotation.AsInt)
                    {
                        case 0: // door facing South
                            float xMoveS = gD.isLeftSideGraphic ? -gD.xMoveAmount : gD.xMoveAmount;
                            float zMoveS = gD.shouldArch && gD.isLeftSideGraphic ? Mathf.Lerp(-archFactor, archFactor, Door.OpenPct) :
                                          gD.shouldArch && !gD.isLeftSideGraphic ? Mathf.Lerp(archFactor, -archFactor, Door.OpenPct) :
                                          0f;
                            moveDir = new Vector3(xMoveS, 0f, zMoveS);
                            break;

                        case 1: // door facing West
                            float zMoveW = gD.isLeftSideGraphic ? gD.xMoveAmount : -gD.xMoveAmount;
                            float xMoveW = gD.shouldArch && gD.isLeftSideGraphic ? Mathf.Lerp(-archFactor, archFactor, Door.OpenPct) :
                                          gD.shouldArch && !gD.isLeftSideGraphic ? Mathf.Lerp(archFactor, -archFactor, Door.OpenPct) :
                                          0f;
                            moveDir = new Vector3(xMoveW, 0f, zMoveW);
                            break;

                        case 2: // door facing North
                            float xMoveN = gD.isLeftSideGraphic ? gD.xMoveAmount : -gD.xMoveAmount;
                            float zMoveN = gD.shouldArch && gD.isLeftSideGraphic ? Mathf.Lerp(archFactor, -archFactor, Door.OpenPct) :
                                          gD.shouldArch && !gD.isLeftSideGraphic ? Mathf.Lerp(-archFactor, archFactor, Door.OpenPct) :
                                          0f;
                            moveDir = new Vector3(xMoveN, 0f, zMoveN);
                            break;

                        case 3: // door facing East
                            float zMoveE = gD.isLeftSideGraphic ? -gD.xMoveAmount : gD.xMoveAmount;
                            float xMoveE = gD.shouldArch && gD.isLeftSideGraphic ? Mathf.Lerp(archFactor, -archFactor, Door.OpenPct) :
                                          gD.shouldArch && !gD.isLeftSideGraphic ? Mathf.Lerp(-archFactor, archFactor, Door.OpenPct) :
                                          0f;
                            moveDir = new Vector3(xMoveE, 0f, zMoveE);
                            break;

                        default:
                            moveDir = Vector3.zero;
                            break;
                    }
                    DrawExtraDoorGraphics(moveDir, gD.spinFactor, gD.shouldFade, fadeMultiplier, Door.OpenPct, mat, gD.drawSize);
                }
            }
        }


        private void DrawExtraDoorGraphics(Vector3 xMoveAmount, float spinFactor, bool shouldFade, float opacity, float openPct, Material mat, Vector3 drawSize)
        {
            float curOpenPct = Door.OpenPct;
            Rot4 rotation = parent.Rotation;
            Quaternion rotationQuat = rotation.AsQuat;
            Vector3 drawPos = parent.DrawPos + xMoveAmount * openPct;

            // Adjust draw size based on door def and open percentage (laser doors only)
            if (parent.def == SDDefOf.SD_LaserDoorDouble && openPct > 0f)
            {
                // Calculate the new width based on the open percentage
                float newWidth = drawSize.x * (1f - openPct);
                drawSize.x = Mathf.Max(newWidth, 0.5f);
            }

            float maxRotation = CompEnhancedDoor?.doorIrisMaxAngle ?? 0f;
            float rotationAngle = maxRotation * curOpenPct;
            Matrix4x4 matrix = Matrix4x4.TRS(drawPos, rotationQuat * Quaternion.Euler(0f, rotationAngle * spinFactor, 0f), new Vector3(drawSize.x, 1f, drawSize.y));
            Material finalMat = shouldFade ? FadedMaterialPool.FadedVersionOf(mat, opacity) : mat;

            /*
            if (StevesDoorsSettings.EnableLaserDoorRecoloring)
            {
                if (parent.def == SDDefOf.SD_LaserDoorDefault)
                {
                    Color laserDoorColor = new();
                    laserDoorColor = StevesDoorsSettings.LaserDoorColor;
                    finalMat.color = laserDoorColor;
                }
                else if (parent.def == SDDefOf.SD_LaserDoorDouble)
                {
                    Color laserDoubleDoorColor = new();
                    laserDoubleDoorColor = StevesDoorsSettings.LaserDoorDoubleColor;
                    finalMat.color = laserDoubleDoorColor;
                }
            }
            */

            Graphics.DrawMesh(MeshPool.plane10, matrix, finalMat, 0);
        }
    }

    public class CompProperties_ExtraDoubleDoorGraphics : CompProperties
    {
        public List<GraphicDataEnhancedDoors> extraDoorGraphics = null;

        public CompProperties_ExtraDoubleDoorGraphics() => compClass = typeof(CompExtraDoubleDoorGraphics);

        public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
        {
            foreach (string error in base.ConfigErrors(parentDef))
            {
                yield return error;
            }
            if (extraDoorGraphics == null)
            {
                yield return "[<color=#4494E3FF>Steve's Doors</color>] <color=#e36c45FF>[CompProperties_ExtraDoubleDoorGraphics] No data found for <extraDoorGraphics>, please provide some.</color>";
            }
        }
    }
}
