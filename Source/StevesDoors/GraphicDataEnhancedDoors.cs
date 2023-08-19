using Verse;

namespace StevesDoors
{
    public class GraphicDataEnhancedDoors : GraphicData
    {
        public bool isLeftSideGraphic = false;
        public bool useGlowerColor = false;
        public bool shouldFade = false;
        public bool shouldArch = false;

        public float xMoveAmount = 1.0f;
        public float spinFactor = 1.0f;
        public float fadeFactor = 1.0f;
        public float archFactor = 1.0f;

        public GraphicDataEnhancedDoors() : base()
        {

        }
    }
}
