using System;
using Verse;
using UnityEngine;

namespace StevesDoors
{
    [StaticConstructorOnStartup]
    public static class AnimationFunctionsUtility
    {
        public static readonly Func<float, float> Linear = delegate (float x)
        {
            return x;
        };

        public static readonly Func<float, float> FadeOutLinear = delegate (float x)
        {
            return 1 - x;
        };

        public static readonly Func<float, float> FadeOutQuad = delegate (float x)
        {
            return 1 - x * x;
        };

        public static readonly Func<float, float> FadeOutCubic = delegate (float x)
        {
            return 1 - x * x * x;
        };

        public static readonly Func<float, float> Sine = delegate (float x)
        {
            return Mathf.Sin(x * Mathf.PI);
        };

        public static readonly Func<float, float> Cosine = delegate (float x)
        {
            return Mathf.Cos(x * Mathf.PI);
        };

        public static readonly Func<float, float> Tangent = delegate (float x)
        {
            return Mathf.Tan(x * Mathf.PI);
        };

        public static readonly Func<float, float> InverseSine = delegate (float x)
        {
            return 1f - Sine(x);
        };

        public static readonly Func<float, float> UnsignedSine = delegate (float x)
        {
            return Mathf.Sin(2f * x * Mathf.PI);
        };

        public static readonly Func<float, float> EaseInQuad = delegate (float x)
        {
            return x * x;
        };

        public static readonly Func<float, float> EaseOutQuad = delegate (float x)
        {
            return 1 - (1 - x) * (1 - x);
        };

        public static readonly Func<float, float> EaseInOutQuad = delegate (float x)
        {
            return x < 0.5 ? 2 * x * x : 1 - Mathf.Pow(-2 * x + 2, 2) / 2;
        };

        public static readonly Func<float, float> EaseOutInQuad = delegate (float x)
        {
            return x < 0.5 ? (0.5f * EaseOutQuad(2f * x)) : (0.5f + 0.5f * EaseInQuad(2f * (x - 0.5f)));
        };

        public static readonly Func<float, float> EaseInCubic = delegate (float x)
        {
            return x * x * x;
        };

        public static readonly Func<float, float> EaseOutCubic = delegate (float x)
        {
            return 1 - Mathf.Pow(1 - x, 3);
        };

        public static readonly Func<float, float> EaseInOutCubic = delegate (float x)
        {
            return x < 0.5 ? 4 * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 3) / 2;
        };

        public static readonly Func<float, float> EaseOutInCubic = delegate (float x)
        {
            return x < 0.5 ? (0.5f * EaseOutCubic(2f * x)) : (0.5f + 0.5f * EaseInCubic(2f * (x - 0.5f)));
        };

        public static readonly Func<float, float> Burst = delegate (float x)
        {
            return Sine(EaseOutCubic(x));
        };

        public static readonly Func<float, float> ReverseBurst = delegate (float x)
        {
            return Sine(EaseInCubic(x));
        };
    }
}
