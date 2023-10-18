using RimWorld;
using Verse;
using UnityEngine;
using System.Collections.Generic;
using Verse.AI;
using System.Text;

namespace StevesDoors
{
    [StaticConstructorOnStartup]
    public class Building_UnmirroredDoor : Building_Door
    {
        public float OpenPct => Mathf.Clamp01((float)ticksSinceOpen / (float)TicksToOpenNow);
        private CompProperties_EnhancedDoorGraphics CompEnhancedDoor;
        public bool IsAccessDoor = false;
        public List<Faction> AllowedFactions = new List<Faction>();

        public Building_UnmirroredDoor()
        {
            // Add the player faction to the allowed factions by default
            AllowedFactions.Add(Faction.OfPlayer);
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            CompEnhancedDoor = def.GetCompProperties<CompProperties_EnhancedDoorGraphics>();
        }

        public bool AllowedForFaction(Faction faction)
        {
            return AllowedFactions.Contains(faction);
        }

        public override bool PawnCanOpen(Pawn p)
        {
            if (IsAccessDoor)
            {
                if (!AllowedForFaction(p.Faction))
                {
                    p.stances.stunner.StunFor(60, p, addBattleLog: false, showMote: false);
                    p.jobs.EndCurrentJob(JobCondition.InterruptForced, startNewJob: false, canReturnToPool: true);
                    //p.jobs.ClearQueuedJobs();
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return base.PawnCanOpen(p);
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo gizmo in base.GetGizmos())
            {
                yield return gizmo;
            }
            
            if (Faction == Faction.OfPlayer)
            {
                Command_Toggle command = new Command_Toggle();
                command.defaultLabel = "Set Restrictions"; // make key
                command.defaultDesc = "Set whether or not this door will act as a secure access point."; // make key
                command.isActive = () => IsAccessDoor;
                command.toggleAction = delegate
                {
                    IsAccessDoor = !IsAccessDoor;
                };
                command.icon = IsAccessDoor ? TexCommands.RestrictedAccess : TexCommands.UnrestrictedAccess;
                yield return command;

                if (IsAccessDoor)
                {
                    Command_Action manageGroupsCommand = new Command_Action();
                    manageGroupsCommand.defaultLabel = "Allowed Factions"; // make key
                    manageGroupsCommand.defaultDesc = "Set which factions are allowed to use this door."; // make key
                    manageGroupsCommand.action = OpenManageFactionsDialog;
                    manageGroupsCommand.icon = TexCommands.AllowedAccess;
                    yield return manageGroupsCommand;
                }
            }
        }

        private void OpenManageFactionsDialog()
        {
            Find.WindowStack.Add(new Dialog_ManageAllowedFactions(this));
        }

        public override void Draw()
        {
            Rotation = DoorRotationAt(Position, Map);
            Rot4 rotation = Rotation;
            float curOpenPct = OpenPct;

            if (CompEnhancedDoor != null)
            {
                GraphicDataEnhancedDoors gDL = CompEnhancedDoor.defaultDoorLeftGraphic;
                GraphicDataEnhancedDoors gDR = CompEnhancedDoor.defaultDoorRightGraphic;
                Material doorLMat = gDL.Graphic.MatSingle;
                Material doorRMat = gDR.Graphic.MatSingle;
                float xMoveAmountL = gDL.xMoveAmount;
                float xMoveAmountR = gDR.xMoveAmount;

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
            Rot4 rotation = Rotation;
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

        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(base.GetInspectString());
            if (IsAccessDoor && AllowedFactions != null && AllowedFactions.Count > 0)
            {
                stringBuilder.AppendLine();
                stringBuilder.Append("Allowed Factions: ");

                bool isFirst = true;
                foreach (Faction faction in AllowedFactions) 
                {
                    if (!isFirst)
                    {
                        stringBuilder.Append(", ");
                    }

                    stringBuilder.Append(faction.Name);
                    isFirst = false;
                }
            }
            return stringBuilder.ToString();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref IsAccessDoor, "IsAccessDoor", defaultValue: false);
            Scribe_Collections.Look(ref AllowedFactions, "AllowedFactions", LookMode.Reference);
        }
    }

    public class Dialog_ManageAllowedFactions : Window
    {
        private Building_UnmirroredDoor Door;
        private Vector2 ScrollPosition = Vector2.zero;

        public Dialog_ManageAllowedFactions(Building_UnmirroredDoor door)
        {
            this.Door = door;
            doCloseButton = true;
            forcePause = true;
            absorbInputAroundWindow = true;
        }

        public override Vector2 InitialSize => new Vector2(350f, 550f);

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Small;

            Widgets.Label(new Rect(0f, 0f, inRect.width, 30f), "Allowed Factions:");

            float yOffset = 12f;
            float lineHeight = 30f;
            float contentHeight = DefDatabase<FactionDef>.AllDefsListForReading.Count * lineHeight;

            Rect viewRect = new Rect(0f, 0f, inRect.width - 16f, contentHeight + 12f);

            Widgets.BeginScrollView(new Rect(0f, 30f, inRect.width, inRect.height - 80f), ref ScrollPosition, viewRect);

            foreach (Faction faction in Find.FactionManager.GetFactions(allowHidden: true, allowDefeated: false, allowNonHumanlike: true, minTechLevel: TechLevel.Undefined))
            {
                Rect factionRect = new Rect(0f, yOffset, inRect.width - 30f, lineHeight);
                bool isFactionAllowed = Door.AllowedFactions.Contains(faction);

                Widgets.CheckboxLabeled(factionRect, faction.Name, ref isFactionAllowed);

                if (isFactionAllowed)
                {
                    if (!Door.AllowedFactions.Contains(faction))
                    {
                        Door.AllowedFactions.Add(faction);
                    }
                }
                else
                {
                    Door.AllowedFactions.Remove(faction);
                }

                yOffset += lineHeight;
            }

            Widgets.EndScrollView();
        }
    }
}
