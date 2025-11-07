using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Based on https://easings.net/

namespace U9.Ease
{
    public enum EaseType
    {
        Linear = 1,
        InSine = 2,
        OutSine = 3,
        InOutSine = 4,
        InQuad = 5,
        OutQuad = 6,
        InOutQuad = 7,
        InCubic = 8,
        OutCubic = 9,
        InOutCubic = 10,
        InQuart = 11,
        OutQuart = 12,
        InOutQuart = 13,
        InQuint = 14,
        OutQuint = 15,
        InOutQuint = 16,
        InExpo = 17,
        OutExpo = 18,
        InOutExpo = 19,
        InCirc = 20,
        OutCirc = 21,
        InOutCirc = 22,
        InElastic = 23,
        OutElastic = 24,
        InOutElastic = 25,
        InBack = 26,
        OutBack = 27,
        InOutBack = 28,
        InBounce = 29,
        OutBounce = 30,
        InOutBounce = 31
    }

    public class EaseFormula
    {
        public static float GetEasedValue(EaseType easeType, float x)
        {
            switch (easeType)
            {
                case EaseType.InSine:
                    return InSine(x);
                case EaseType.OutSine:
                    return OutSine(x);
                case EaseType.InOutSine:
                    return InOutSine(x);
                case EaseType.InQuad:
                    return InQuad(x);
                case EaseType.OutQuad:
                    return OutQuad(x);
                case EaseType.InOutQuad:
                    return InOutQuad(x);
                case EaseType.InCubic:
                    return InCubic(x);
                case EaseType.OutCubic:
                    return OutCubic(x);
                case EaseType.InOutCubic:
                    return InOutCubic(x);
                case EaseType.InQuart:
                    return InQuart(x);
                case EaseType.OutQuart:
                    return OutQuart(x);
                case EaseType.InOutQuart:
                    return InOutQuart(x);
                case EaseType.InQuint:
                    return InQuint(x);
                case EaseType.OutQuint:
                    return OutQuint(x);
                case EaseType.InOutQuint:
                    return InOutQuint(x);
                case EaseType.InExpo:
                    return InExpo(x);
                case EaseType.OutExpo:
                    return OutExpo(x);
                case EaseType.InOutExpo:
                    return InOutExpo(x);
                case EaseType.InCirc:
                    return InCirc(x);
                case EaseType.OutCirc:
                    return OutCirc(x);
                case EaseType.InOutCirc:
                    return InOutCirc(x);
                case EaseType.InElastic:
                    return InElastic(x);
                case EaseType.OutElastic:
                    return OutElastic(x);
                case EaseType.InOutElastic:
                    return InOutElastic(x);
                case EaseType.InBack:
                    return InBack(x);
                case EaseType.OutBack:
                    return OutBack(x);
                case EaseType.InOutBack:
                    return InOutBack(x);
                case EaseType.InBounce:
                    return InBounce(x);
                case EaseType.OutBounce:
                    return OutBounce(x);
                case EaseType.InOutBounce:
                    return InOutBounce(x);
                case EaseType.Linear:
                default:
                    return Linear(x);
            }
        }

        public static IBaseEase GetEaseFunction(EaseType easeType)
        {
            switch (easeType)
            {
                case EaseType.InSine:
                    return new InSine();
                case EaseType.OutSine:
                    return new OutSine();
                case EaseType.InOutSine:
                    return new InOutSine();
                case EaseType.InQuad:
                    return new InQuad();
                case EaseType.OutQuad:
                    return new OutQuad();
                case EaseType.InOutQuad:
                    return new InOutQuad();
                case EaseType.InCubic:
                    return new InCubic();
                case EaseType.OutCubic:
                    return new OutCubic();
                case EaseType.InOutCubic:
                    return new InOutCubic();
                case EaseType.InQuart:
                    return new InQuart();
                case EaseType.OutQuart:
                    return new OutQuart();
                case EaseType.InOutQuart:
                    return new InOutQuart();
                case EaseType.InQuint:
                    return new InQuint();
                case EaseType.OutQuint:
                    return new OutQuint();
                case EaseType.InOutQuint:
                    return new InOutQuint();
                case EaseType.InExpo:
                    return new InExpo();
                case EaseType.OutExpo:
                    return new OutExpo();
                case EaseType.InOutExpo:
                    return new InOutExpo();
                case EaseType.InCirc:
                    return new InCirc();
                case EaseType.OutCirc:
                    return new OutCirc();
                case EaseType.InOutCirc:
                    return new InOutCirc();
                case EaseType.InElastic:
                    return new InElastic();
                case EaseType.OutElastic:
                    return new OutElastic();
                case EaseType.InOutElastic:
                    return new InOutElastic();
                case EaseType.InBack:
                    return new InBack();
                case EaseType.OutBack:
                    return new OutBack();
                case EaseType.InOutBack:
                    return new InOutBack();
                case EaseType.InBounce:
                    return new InBounce();
                case EaseType.OutBounce:
                    return new OutBounce();
                case EaseType.InOutBounce:
                    return new InOutBounce();
                case EaseType.Linear:
                default:
                    return new Linear();
            }
        }

        // LINEAR --------------------------------------------
        public static float Linear(float x)
        {
            return x;
        }

        public static float InSine(float x)
        {
            return 1f - Mathf.Cos((x * Mathf.PI) / 2);
        }

        public static float OutSine(float x)
        {
            return Mathf.Sin((x * Mathf.PI) / 2f);
        }

        public static float InOutSine(float x)
        {
            return -(Mathf.Cos(Mathf.PI * x) - 1f) / 2f;
        }

        // QUAD --------------------------------------------
        public static float InQuad(float x)
        {
            return x *x;
        }

        public static float OutQuad(float x)
        {
            return 1 - (1 - x) * (1 - x);
        }

        public static float InOutQuad(float x)
        {
            return x < 0.5f ? 2f * x * x : 1f - Mathf.Pow(-2f * x + 2f, 2f) / 2f;
        }

        // CUBIC --------------------------------------------
        public static float InCubic(float x)
        {
            return x * x * x;
        }

        public static float OutCubic(float x)
        {
            return 1f - Mathf.Pow(1f - x, 3f);
        }

        public static float InOutCubic(float x)
        {
            return x < 0.5f ? 4f * x * x * x : 1f - Mathf.Pow(-2f * x + 2f, 3f) / 2f;
        }

        // QUART --------------------------------------------
        public static float InQuart(float x)
        {
            return x * x * x * x;
        }

        public static float OutQuart(float x)
        {
            return 1f - Mathf.Pow(1f - x, 4f);
        }

        public static float InOutQuart(float x)
        {
            return x < 0.5f ? 8f * x * x * x * x : 1 - Mathf.Pow(-2f * x + 2f, 4f) / 2f;
        }

        // QUINT --------------------------------------------
        public static float InQuint(float x)
        {
            return x * x * x * x * x;
        }

        public static float OutQuint(float x)
        {
            return 1f - Mathf.Pow(1f - x, 5f);
        }
        public static float InOutQuint(float x)
        {
            return x < 0.5f ? 16f * x * x * x * x * x : 1f - Mathf.Pow(-2f * x + 2f, 5f) / 2f;
        }

        // EXPO --------------------------------------------
        public static float InExpo(float x)
        {
            return x == 0f ? 0 : Mathf.Pow(2f, 10f * x - 10f);
        }
        public static float OutExpo(float x)
        {
            return x == 1f ? 1f : 1f - Mathf.Pow(2f, -10f * x);
        }
        public static float InOutExpo(float x)
        {
            return x == 0f
                      ? 0f
                      : x == 1f
                      ? 1f
                      : x < 0.5f ? Mathf.Pow(2f, 20f * x - 10f) / 2f
                      : (2f - Mathf.Pow(2f, -20f * x + 10f)) / 2f;
        }

        // CIRC --------------------------------------------
        public static float InCirc(float x)
        {
            return 1f - Mathf.Sqrt(1f - Mathf.Pow(x, 2f));
        }
        public static float OutCirc(float x)
        {
            return Mathf.Sqrt(1f - Mathf.Pow(x - 1f, 2f));
        }
        public static float InOutCirc(float x)
        {
            return x < 0.5f
                      ? (1f - Mathf.Sqrt(1f - Mathf.Pow(2f * x, 2f))) / 2f
                      : (Mathf.Sqrt(1f - Mathf.Pow(-2f * x + 2f, 2f)) + 1f) / 2f;
        }

        // BACK --------------------------------------------
        public static float InBack(float x)
        {
            float c1 = 1.70158f;
            float c3 = c1 + 1f;

            return c3 * x * x * x - c1 * x * x;
        }
        public static float OutBack(float x)
        {
            float c1 = 1.70158f;
            float c3 = c1 + 1f;

            return 1f + c3 * Mathf.Pow(x - 1f, 3f) + c1 * Mathf.Pow(x - 1f, 2f);
        }
        public static float InOutBack(float x)
        {
            float c1 = 1.70158f;
            float c2 = c1 * 1.525f;

            return x < 0.5f
                      ? (Mathf.Pow(2f * x, 2f) * ((c2 + 1f) * 2f * x - c2)) / 2f
                      : (Mathf.Pow(2f * x - 2f, 2f) * ((c2 + 1f) * (x * 2f - 2f) + c2) + 2f) / 2f;
        }

        // ELASTIC --------------------------------------------
        public static float InElastic(float x)
        {
            float c4 = (2f * Mathf.PI) / 3f;

            return x == 0f
              ? 0f
              : x == 1f
              ? 1f
              : -Mathf.Pow(2f, 10f * x - 10f) * Mathf.Sin((x * 10f - 10.75f) * c4);
        }
        public static float OutElastic(float x)
        {
            float c4 = (2f * Mathf.PI) / 3f;

            return x == 0
              ? 0
              : x == 1
              ? 1
              : Mathf.Pow(2f, -10f * x) * Mathf.Sin((x * 10f - 0.75f) * c4) + 1f;
        }
        public static float InOutElastic(float x)
        {
            float c5 = (2f * Mathf.PI) / 4.5f;

            return x == 0
              ? 0
              : x == 1
              ? 1
              : x < 0.5f
              ? -(Mathf.Pow(2f, 20f * x - 10f) * Mathf.Sin((20f * x - 11.125f) * c5)) / 2f
              : (Mathf.Pow(2f, -20f * x + 10f) * Mathf.Sin((20f * x - 11.125f) * c5)) / 2f + 1f;
        }

        // BOUNCE --------------------------------------------
        public static float InBounce(float x)
        {
            return 1f - OutBounce(1 - x);
        }
        public static float OutBounce(float x)
        {
            float n1 = 7.5625f;
            float d1 = 2.75f;

            if (x < 1f / d1)
            {
                return n1 * x * x;
            }
            else if (x < 2f / d1)
            {
                return n1 * (x -= 1.5f / d1) * x + 0.75f;
            }
            else if (x < 2.5f / d1)
            {
                return n1 * (x -= 2.25f / d1) * x + 0.9375f;
            }
            else
            {
                return n1 * (x -= 2.625f / d1) * x + 0.984375f;
            }
        }
        public static float InOutBounce(float x)
        {
            return x < 0.5f
                  ? (1f - OutBounce(1f - 2f * x)) / 2f
                  : (1f + OutBounce(2f * x - 1f)) / 2f;
        }
    }
}