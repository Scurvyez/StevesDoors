using System.Collections.Generic;
using Verse;
using UnityEngine;

namespace StevesDoors
{
    [StaticConstructorOnStartup]
    public class CompEnhancedDoorGraphics : ThingComp
    {
        public CompProperties_EnhancedDoorGraphics Props => (CompProperties_EnhancedDoorGraphics)props;
        private CompGlower glowerComp;
        private Color glowerColor;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            glowerComp = parent.GetComp<CompGlower>();
        }

        public override void CompTick()
        {
            base.CompTick();

            if (glowerComp != null && glowerComp.Glows)
            {
                glowerColor = glowerComp.GlowColor.ToColor;
            }
        }

        public override void PostDraw()
        {
            base.PostDraw();

            if (Props.extraStaticDoorGraphics != null)
            {
                for (int i = 0; i < Props.extraStaticDoorGraphics.Count; i++)
                {
                    if (parent != null)
                    {
                        Rot4 rotation = parent.Rotation;
                        Vector3 drawPos = parent.DrawPos;
                        drawPos.y = parent.DrawPos.y + 1.0f;

                        if (Props.extraStaticDoorGraphics[i].useGlowerColor)
                        {
                            Props.extraStaticDoorGraphics[i].color = glowerColor;
                        }
                        Props.extraStaticDoorGraphics[i].Graphic.Draw((drawPos + Props.extraStaticDoorGraphics[i].drawOffset), rotation, parent);
                    }
                }
            }
        }
    }
    
    public class CompProperties_EnhancedDoorGraphics : CompProperties
    {
        public bool isIrisDoor = false;
        public float doorIrisMaxAngle = 0f;
        public GraphicDataEnhancedDoors defaultDoorLeftGraphic;
        public GraphicDataEnhancedDoors defaultDoorRightGraphic;
        public List<GraphicDataEnhancedDoors> extraStaticDoorGraphics = null;

        public CompProperties_EnhancedDoorGraphics() => compClass = typeof(CompEnhancedDoorGraphics);

        public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
        {
            foreach (string error in base.ConfigErrors(parentDef))
            {
                yield return error;
            }
            if (defaultDoorLeftGraphic == null && defaultDoorRightGraphic == null)
            {
                yield return $"<color={SDLog.ErrorMsgCol}>[Steve's Doors]</color> [CompProperties_EnhancedDoorGraphics] No textures found for <defaultDoorLeftGraphic> and <defaultDoorRightGraphic>, please provide both.";
            }
            else if (defaultDoorLeftGraphic == null)
            {
                yield return $"<color={SDLog.ErrorMsgCol}>[Steve's Doors]</color> [CompProperties_EnhancedDoorGraphics] No texture found for <defaultDoorLeftGraphic>, please provide one.";
            }
            else if (defaultDoorRightGraphic == null)
            {
                yield return $"<color={SDLog.ErrorMsgCol}>[Steve's Doors]</color> [CompProperties_EnhancedDoorGraphics] No texture found for <defaultDoorRightGraphic>, please provide one.";
            }
        }
    }
}
