using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace U9.Motion.Demo
{
    public class CustomClass : MonoBehaviour
    {
        // Private variables
        // These can be serialized if you wish (These are not editable by Custom Motion)
        private int m_IntValue = 0;
        private float m_FloatValue = 0.0f;
        private Vector2 m_Vec2Value = Vector2.zero;
        private Vector3 m_Vec3Value = Vector3.zero;
        private Vector4 m_Vec4Value = Vector4.zero;
        private Color m_ColorValue = Color.white;
        private Sprite m_SpriteValue = null;

        // Public accessors
        // These are accessable and editable by Custom Motion, even if setter is private
        public int IntegerProperty
        {
            get { return m_IntValue; }
            private set 
            { 
                m_IntValue = value;
                m_IntValText.text = m_IntValue.ToString();
            }
        }
        public float SingleProperty
        {
            get { return m_FloatValue; }
            private set
            { 
                m_FloatValue = value;
                m_FloatValText.text = m_FloatValue.ToString();
            }
        }
        public Vector2 Vector2Property
        {
            get { return m_Vec2Value; }
            private set 
            { 
                m_Vec2Value = value;
                m_Vec2ValText.text = m_Vec2Value.ToString();
            }
        }
        public Vector3 Vector3Property
        {
            get { return m_Vec3Value; }
            private set
            { 
                m_Vec3Value = value;
                m_Vec3ValText.text = m_Vec3Value.ToString();
            }
        }
        public Vector4 Vector4Property
        {
            get { return m_Vec4Value; }
            private set 
            { 
                m_Vec4Value = value;
                m_Vec4ValText.text = m_Vec4Value.ToString();
            }
        }
        public Color ColorProperty
        {
            get { return m_ColorValue; }
            private set 
            {
                m_ColorValue = value;
                m_ColorVal.color = m_ColorValue;
            }
        }
        public Sprite SpriteProperty
        {
            get { return m_SpriteValue; }
            private set 
            { 
                m_SpriteValue = value;
                m_SpriteVal.sprite = m_SpriteValue;
            }
        }

        [Header("Value Displayers")]
        [SerializeField] private TMP_Text m_IntValText;
        [SerializeField] private TMP_Text m_FloatValText;
        [SerializeField] private TMP_Text m_Vec2ValText;
        [SerializeField] private TMP_Text m_Vec3ValText;
        [SerializeField] private TMP_Text m_Vec4ValText;
        [SerializeField] private Image m_ColorVal;
        [SerializeField] private Image m_SpriteVal;
    }
}
