using System.Collections.Generic;
using Verse;
using UnityEngine;

namespace StevesDoors
{
    public class CompExtraDoorGraphics : ThingComp
    {
        public CompProperties_ExtraDoorGraphics Props => (CompProperties_ExtraDoorGraphics)props;
        private CompProperties_EnhancedDoorGraphics CompEnhancedDoor;
        public Building_UnmirroredDoor Door;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            CompEnhancedDoor = parent.def.GetCompProperties<CompProperties_EnhancedDoorGraphics>();
            Door = parent as Building_UnmirroredDoor;
        }

        public override void PostDraw()
        {
            base.PostDraw();

            if (Props != null && Props.extraDoorGraphics != null)
            {
                Rot4 rotation = parent.Rotation;

                for (int i = 0; i < Props.extraDoorGraphics.Count; i++)
                {
                    GraphicDataEnhancedDoors gD = Props.extraDoorGraphics[i];
                    float fadeMultiplier = 1f - (Door.OpenPct * gD.fadeFactor);
                    Graphic graphic = gD.Graphic;
                    Material mat = graphic.MatSingle;
                    //Material mat = graphic.GetColoredVersion(graphic.Shader, parent.DrawColor, parent.DrawColorTwo).MatSingle;

                    Vector3 moveDir;
                    float archFactor = gD.xMoveAmount * gD.archFactor;

                    switch (rotation.AsInt)
                    {
                        case 0: // door facing South
                            float xMoveS = gD.isLeftSideGraphic ? -gD.xMoveAmount : gD.xMoveAmount;
                            
                            float zMoveS;
                            if (gD.shouldArch && gD.isLeftSideGraphic)
                            {
                                zMoveS = Mathf.Lerp(-archFactor, archFactor, Door.OpenPct);
                            }
                            else if (gD.shouldArch && !gD.isLeftSideGraphic)
                            {
                                zMoveS = Mathf.Lerp(archFactor, -archFactor, Door.OpenPct);
                            }
                            else
                            {
                                zMoveS = 0f;
                            }
                            
                            moveDir = new Vector3(xMoveS, 0f, zMoveS);
                            break;
                            
                        case 1: // door facing West
                            float zMoveW = gD.isLeftSideGraphic ? gD.xMoveAmount : -gD.xMoveAmount;

                            float xMoveW;
                            if (gD.shouldArch && gD.isLeftSideGraphic)
                            {
                                xMoveW = Mathf.Lerp(-archFactor, archFactor, Door.OpenPct);
                            }
                            else if (gD.shouldArch && !gD.isLeftSideGraphic)
                            {
                                xMoveW = Mathf.Lerp(archFactor, -archFactor, Door.OpenPct);
                            }
                            else
                            {
                                xMoveW = 0f;
                            }

                            moveDir = new Vector3(xMoveW, 0f, zMoveW);
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

            float maxRotation = CompEnhancedDoor?.doorIrisMaxAngle ?? 0f;
            float rotationAngle = maxRotation * curOpenPct;
            Matrix4x4 matrix = Matrix4x4.TRS(drawPos, rotationQuat * Quaternion.Euler(0f, rotationAngle * spinFactor, 0f), new Vector3(drawSize.x, 1f, drawSize.y));

            Material finalMat = shouldFade ? FadedMaterialPool.FadedVersionOf(mat, opacity) : mat;
            Graphics.DrawMesh(MeshPool.plane10, matrix, finalMat, 0);
        }
    }

    public class CompProperties_ExtraDoorGraphics : CompProperties
    {
        public List<GraphicDataEnhancedDoors> extraDoorGraphics = null;

        public CompProperties_ExtraDoorGraphics() => compClass = typeof(CompExtraDoorGraphics);
    }
}
