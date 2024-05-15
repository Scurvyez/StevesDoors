﻿using System.Collections.Generic;
using Verse;
using UnityEngine;

namespace StevesDoors
{
    public class CompImprovedDoorsDouble : CompImprovedDoors
    {
        public CompProperties_ImprovedDoorsDouble Props => (CompProperties_ImprovedDoorsDouble)props;
        public Building_UnmirroredDoor Door;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            Door = parent as Building_UnmirroredDoor;
        }

        public override void PostDraw()
        {
            base.PostDraw();

            if (Props != null && Props.extraDoorGraphics != null)
            {
                foreach (var gD in Props.extraDoorGraphics)
                {
                    FadeMultiplier = 1f - (Door.OpenPct * gD.fadeFactor * AccentColor.a);
                    IsAccentGraphic = gD.isAccentGraphic;
                    Graphic graphic = gD.Graphic;
                    Material mat = graphic.MatSingle;

                    float archFactor = gD.xMoveAmount * gD.archFactor;

                    switch (Rotation.AsInt)
                    {
                        case 0: // door facing South
                            float xMoveS = gD.isLeftSideGraphic ? -gD.xMoveAmount : gD.xMoveAmount;
                            float zMoveS = gD.shouldArch && gD.isLeftSideGraphic ? Mathf.Lerp(-archFactor, archFactor, Door.OpenPct) :
                                          gD.shouldArch && !gD.isLeftSideGraphic ? Mathf.Lerp(archFactor, -archFactor, Door.OpenPct) : 0f;
                            MoveDir = new Vector3(xMoveS, 0f, zMoveS);
                            break;

                        case 1: // door facing West
                            float zMoveW = gD.isLeftSideGraphic ? gD.xMoveAmount : -gD.xMoveAmount;
                            float xMoveW = gD.shouldArch && gD.isLeftSideGraphic ? Mathf.Lerp(-archFactor, archFactor, Door.OpenPct) :
                                          gD.shouldArch && !gD.isLeftSideGraphic ? Mathf.Lerp(archFactor, -archFactor, Door.OpenPct) : 0f;
                            MoveDir = new Vector3(xMoveW, 0f, zMoveW);
                            break;

                        case 2: // door facing North
                            float xMoveN = gD.isLeftSideGraphic ? gD.xMoveAmount : -gD.xMoveAmount;
                            float zMoveN = gD.shouldArch && gD.isLeftSideGraphic ? Mathf.Lerp(archFactor, -archFactor, Door.OpenPct) :
                                          gD.shouldArch && !gD.isLeftSideGraphic ? Mathf.Lerp(-archFactor, archFactor, Door.OpenPct) : 0f;
                            MoveDir = new Vector3(xMoveN, 0f, zMoveN);
                            break;

                        case 3: // door facing East
                            float zMoveE = gD.isLeftSideGraphic ? -gD.xMoveAmount : gD.xMoveAmount;
                            float xMoveE = gD.shouldArch && gD.isLeftSideGraphic ? Mathf.Lerp(archFactor, -archFactor, Door.OpenPct) :
                                          gD.shouldArch && !gD.isLeftSideGraphic ? Mathf.Lerp(-archFactor, archFactor, Door.OpenPct) : 0f;
                            MoveDir = new Vector3(xMoveE, 0f, zMoveE);
                            break;

                        default:
                            MoveDir = Vector3.zero;
                            break;
                    }
                    DrawExtraDoorGraphics(MoveDir, gD.doorIrisMaxAngle, gD.spinFactor, gD.shouldFade, FadeMultiplier, Door.OpenPct, mat, gD.drawSize, IsAccentGraphic);
                }
            }
        }

        private void DrawExtraDoorGraphics(Vector3 xMoveAmount, float irisMaxAngle, float spinFactor, bool shouldFade, float opacity, float openPct, Material mat, Vector3 drawSize, bool isAccent)
        {
            // Adjust draw size based on door def and open percentage (double laser doors only)
            if (Ext != null && Ext.isLaserDoor)
            {
                if (openPct > 0f)
                {
                    // Calculate the new width based on the open percentage
                    float newWidth = drawSize.x * (1f - openPct);
                    drawSize.x = Mathf.Max(newWidth, 0.5f);
                }
            }

            DrawPos = parent.DrawPos + xMoveAmount * openPct;
            RotationAngle = irisMaxAngle * Door.OpenPct;
            Matrix = Matrix4x4.TRS(DrawPos, Rotation.AsQuat * Quaternion.Euler(0f, RotationAngle * spinFactor, 0f), new Vector3(drawSize.x, 1f, drawSize.y));
            FinalMat = shouldFade ? FadedMaterialPool.FadedVersionOf(mat, opacity) : mat;

            MPB.Clear();
            if (Ext != null && Ext.isLaserDoor)
            {
                MPB.SetColor("_Color", new Color(DoorColor.r, DoorColor.g, DoorColor.b, opacity));
            }
            else if (isAccent)
            {
                MPB.SetColor("_Color", new Color(AccentColor.r, AccentColor.g, AccentColor.b, (ShowAccentGraphics ? 1f : 0f) * opacity));
            }
            else
            {
                MPB.SetColor("_Color", new Color(mat.color.r, mat.color.g, mat.color.b, opacity));
            }
            Graphics.DrawMesh(MeshPool.plane10, Matrix, FinalMat, 0, null, 0, MPB);
        }

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref DoorColor, "_doorColor", Color.white);
            Scribe_Values.Look(ref AccentColor, "_accentColor", Color.white);
            Scribe_Values.Look(ref ShowAccentGraphics, "_showAccentGraphics", true);
            Scribe_Values.Look(ref FadeMultiplier, "_fadeMultiplier");
            Scribe_Values.Look(ref MoveDir, "_moveDir");
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
                yield return $"{SDLog.ErrorMsgCol} [CompProperties_ImprovedDoorsDouble] No data found for <extraDoorGraphics>, please provide some.";
            }
        }
    }
}
