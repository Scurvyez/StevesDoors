using RimWorld;
using Verse;
using UnityEngine;

namespace StevesDoors
{
    [StaticConstructorOnStartup]
    public class Building_UnmirroredDoor : Building_Door
    {
        public new float OpenPct => Mathf.Clamp01((float)ticksSinceOpen / (float)TicksToOpenNow);
        private CompProperties_EnhancedDoorGraphics CompEnhancedDoor;
        private Rot4 UpdatedRotation;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            CompEnhancedDoor = def.GetCompProperties<CompProperties_EnhancedDoorGraphics>();
            UpdatedRotation = DoorUtility.DoorRotationAt(Position, Map, false);
        }

        protected override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            base.DrawAt(drawLoc, flip);
            Rot4 rotation = UpdatedRotation;
            float curOpenPct = OpenPct;

            if (CompEnhancedDoor != null)
            {
                GraphicDataEnhancedDoors gDL = CompEnhancedDoor.defaultDoorLeftGraphic;
                GraphicDataEnhancedDoors gDR = CompEnhancedDoor.defaultDoorRightGraphic;
                float xMoveAmountL = gDL.xMoveAmount;
                float xMoveAmountR = gDR.xMoveAmount;
                Material doorLMat = gDL.Graphic.MatSingle;
                Material doorRMat = gDR.Graphic.MatSingle;

                if (doorLMat != null && doorRMat != null)
                {
                    Vector3 doorRightMoveDir;
                    Vector3 doorLeftMoveDir;

                    switch (rotation.AsInt)
                    {
                        case 0: // door facing South
                            doorLeftMoveDir = new Vector3(-xMoveAmountL, 0f, 0f); // -1 means to the left one tile
                            doorRightMoveDir = new Vector3(xMoveAmountR, 0f, 0f); // 1, to the right one tile
                            DrawDoor(doorLeftMoveDir, doorRightMoveDir, curOpenPct, doorLMat, doorRMat);
                            break;
                        case 1: // door facing West
                            doorLeftMoveDir = new Vector3(0f, 0f, xMoveAmountL);
                            doorRightMoveDir = new Vector3(0f, 0f, -xMoveAmountR);
                            DrawDoor(doorLeftMoveDir, doorRightMoveDir, curOpenPct, doorLMat, doorRMat);
                            break;
                    }
                }
            }

            Comps_PostDraw();
        }

        private void DrawDoor(Vector3 vector1, Vector3 vector2, float num, Material leftMat, Material rightMat)
        {
            float curOpenPct = OpenPct;
            Rot4 rotation = UpdatedRotation;
            Quaternion rotationQuat = rotation.AsQuat;

            Vector3 dDLS = CompEnhancedDoor.defaultDoorLeftGraphic.drawSize;
            Vector3 dDRS = CompEnhancedDoor.defaultDoorRightGraphic.drawSize;

            Vector3 doorLeftDrawPos = DrawPos + vector1 * num;
            Vector3 doorRightDrawPos = DrawPos + vector2 * num;

            if (CompEnhancedDoor != null && CompEnhancedDoor.isIrisDoor) // rotating doors (iris-style)
            {
                float maxRotation = CompEnhancedDoor.doorIrisMaxAngle; // Maximum rotation angle for the "iris" effect
                float rotationAngle = maxRotation * curOpenPct;
                Quaternion rotatedQuat = rotationQuat * Quaternion.Euler(0f, -rotationAngle, 0f);

                DrawMeshWithTransform(leftMat, doorLeftDrawPos, rotatedQuat, new Vector3(dDLS.x, 1f, dDLS.y));
                DrawMeshWithTransform(rightMat, doorRightDrawPos, rotatedQuat, new Vector3(dDRS.x, 1f, dDRS.y));
            }
            else
            {
                DrawMeshWithTransform(leftMat, doorLeftDrawPos, rotationQuat, new Vector3(dDLS.x, 1f, dDLS.y));
                DrawMeshWithTransform(rightMat, doorRightDrawPos, rotationQuat, new Vector3(dDRS.x, 1f, dDRS.y));
            }
        }

        private void DrawMeshWithTransform(Material material, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            Matrix4x4 matrix = Matrix4x4.TRS(position, rotation, scale);
            Graphics.DrawMesh(MeshPool.plane10, matrix, material, 0);
        }
    }
}
