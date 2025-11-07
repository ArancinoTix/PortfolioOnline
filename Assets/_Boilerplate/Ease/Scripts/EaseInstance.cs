using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Based on https://easings.net/

namespace U9.Ease
{    
    public interface IBaseEase
    {
        public float Calculate(float x);
        public EaseType EaseType();
    }

    // LINEAR --------------------------------------------
    public class Linear : IBaseEase
    {
        public float Calculate(float x)
        {
            return EaseFormula.Linear(x);
        }

        public EaseType EaseType()
        {
            return Ease.EaseType.Linear;
        }
    }

    // SINE --------------------------------------------
    public class InSine : IBaseEase
    {
        public float Calculate(float x)
        {
            return EaseFormula.InSine(x);
        }
        public EaseType EaseType()
        {
            return Ease.EaseType.InSine;
        }
    }

    public class OutSine : IBaseEase
    {
        public float Calculate(float x)
        {
            return EaseFormula.OutSine(x);
        }
        public EaseType EaseType()
        {
            return Ease.EaseType.OutSine;
        }
    }

    public class InOutSine : IBaseEase
    {
        public float Calculate(float x)
        {
            return EaseFormula.InOutSine(x);
        }
        public EaseType EaseType()
        {
            return Ease.EaseType.InOutSine;
        }
    }

    // QUAD --------------------------------------------
    public class InQuad : IBaseEase
    {
        public float Calculate(float x)
        {
            return EaseFormula.InQuad(x);
        }
        public EaseType EaseType()
        {
            return Ease.EaseType.InQuad;
        }
    }

    public class OutQuad : IBaseEase
    {
        public float Calculate(float x)
        {
            return EaseFormula.OutQuad(x);
        }
        public EaseType EaseType()
        {
            return Ease.EaseType.OutQuad;
        }
    }

    public class InOutQuad : IBaseEase
    {
        public float Calculate(float x)
        {
            return EaseFormula.InOutQuad(x);
        }
        public EaseType EaseType()
        {
            return Ease.EaseType.InOutQuad;
        }
    }

    // CUBIC --------------------------------------------
    public class InCubic : IBaseEase
    {
        public float Calculate(float x)
        {
            return EaseFormula.InCubic(x);
        }
        public EaseType EaseType()
        {
            return Ease.EaseType.InCubic;
        }
    }
    public class OutCubic : IBaseEase
    {
        public float Calculate(float x)
        {
            return EaseFormula.OutCubic(x);
        }
        public EaseType EaseType()
        {
            return Ease.EaseType.OutCubic;
        }
    }

    public class InOutCubic : IBaseEase
    {
        public float Calculate(float x)
        {
            return EaseFormula.InOutCubic(x);
        }
        public EaseType EaseType()
        {
            return Ease.EaseType.InOutCubic;
        }
    }

    // QUART --------------------------------------------
    public class InQuart : IBaseEase
    {
        public float Calculate(float x)
        {
            return EaseFormula.InQuart(x);
        }
        public EaseType EaseType()
        {
            return Ease.EaseType.InQuart;
        }
    }
    public class OutQuart : IBaseEase
    {
        public float Calculate(float x)
        {
            return EaseFormula.OutQuart(x);
        }
        public EaseType EaseType()
        {
            return Ease.EaseType.OutQuart;
        }
    }
    public class InOutQuart : IBaseEase
    {
        public float Calculate(float x)
        {
            return EaseFormula.InOutQuart(x);
        }
        public EaseType EaseType()
        {
            return Ease.EaseType.InOutQuart;
        }
    }

    // QUINT --------------------------------------------
    public class InQuint : IBaseEase
    {
        public float Calculate(float x)
        {
            return EaseFormula.InQuint(x);
        }
        public EaseType EaseType()
        {
            return Ease.EaseType.InQuint;
        }
    }
    public class OutQuint : IBaseEase
    {
        public float Calculate(float x)
        {
            return EaseFormula.OutQuint(x);
        }
        public EaseType EaseType()
        {
            return Ease.EaseType.OutQuint;
        }
    }
    public class InOutQuint : IBaseEase
    {
        public float Calculate(float x)
        {
            return EaseFormula.InOutQuint(x);
        }
        public EaseType EaseType()
        {
            return Ease.EaseType.InOutQuint;
        }
    }

    // EXPO --------------------------------------------
    public class InExpo : IBaseEase
    {
        public float Calculate(float x)
        {
            return EaseFormula.InExpo(x);
        }
        public EaseType EaseType()
        {
            return Ease.EaseType.InExpo;
        }
    }
    public class OutExpo : IBaseEase
    {
        public float Calculate(float x)
        {
            return EaseFormula.OutExpo(x);
        }
        public EaseType EaseType()
        {
            return Ease.EaseType.OutExpo;
        }
    }
    public class InOutExpo : IBaseEase
    {
        public float Calculate(float x)
        {
            return EaseFormula.InOutExpo(x);
        }
        public EaseType EaseType()
        {
            return Ease.EaseType.InOutExpo;
        }
    }

    // CIRC --------------------------------------------
    public class InCirc : IBaseEase
    {
        public float Calculate(float x)
        {
            return EaseFormula.InCirc(x);
        }
        public EaseType EaseType()
        {
            return Ease.EaseType.InCirc;
        }
    }
    public class OutCirc : IBaseEase
    {
        public float Calculate(float x)
        {
            return EaseFormula.OutCirc(x);
        }
        public EaseType EaseType()
        {
            return Ease.EaseType.OutCirc;
        }
    }
    public class InOutCirc : IBaseEase
    {
        public float Calculate(float x)
        {
            return EaseFormula.InOutCirc(x);
        }
        public EaseType EaseType()
        {
            return Ease.EaseType.InOutCirc;
        }
    }

    // BACK --------------------------------------------
    public class InBack : IBaseEase
    {
        public float Calculate(float x)
        {
            return EaseFormula.InBack(x);
        }
        public EaseType EaseType()
        {
            return Ease.EaseType.InBack;
        }
    }
    public class OutBack : IBaseEase
    {
        public float Calculate(float x)
        {
            return EaseFormula.OutBack(x);
        }
        public EaseType EaseType()
        {
            return Ease.EaseType.OutBack;
        }
    }
    public class InOutBack : IBaseEase
    {
        public float Calculate(float x)
        {
            return EaseFormula.InOutBack(x);
        }
        public EaseType EaseType()
        {
            return Ease.EaseType.InOutBack;
        }
    }

    // ELASTIC --------------------------------------------
    public class InElastic : IBaseEase
    {
        public float Calculate(float x)
        {
            return EaseFormula.InElastic(x);
        }
        public EaseType EaseType()
        {
            return Ease.EaseType.InElastic;
        }
    }
    public class OutElastic : IBaseEase
    {
        public float Calculate(float x)
        {
            return EaseFormula.OutElastic(x);
        }
        public EaseType EaseType()
        {
            return Ease.EaseType.OutElastic;
        }
    }
    public class InOutElastic : IBaseEase
    {
        public float Calculate(float x)
        {
            return EaseFormula.InOutElastic(x);
        }
        public EaseType EaseType()
        {
            return Ease.EaseType.InOutElastic;
        }
    }

    // BOUNCE --------------------------------------------
    public class InBounce : IBaseEase
    {
        public float Calculate(float x)
        {
            return EaseFormula.InBounce(x);
        }
        public EaseType EaseType()
        {
            return Ease.EaseType.InBounce;
        }
    }
    public class OutBounce : IBaseEase
    {
        public float Calculate(float x)
        {
            return EaseFormula.OutBounce(x);
        }
        public EaseType EaseType()
        {
            return Ease.EaseType.OutBounce;
        }
    }
    public class InOutBounce : IBaseEase
    {
        public float Calculate(float x)
        {
            return EaseFormula.InOutBounce(x);
        }
        public EaseType EaseType()
        {
            return Ease.EaseType.InOutBounce;
        }
    }
}