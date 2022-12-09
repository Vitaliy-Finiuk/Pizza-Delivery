using System;
using CodeBase.LevelMechanics;

namespace CodeBase.Spline
{
    [Serializable]
    public class SplineBehaviour
    {
        public SplineType splineType;
        public ChunkType[] chunkType;
        public bool isLeftAndRightEnabled;
    }
}