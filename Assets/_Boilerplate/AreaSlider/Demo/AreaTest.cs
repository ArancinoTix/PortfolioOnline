using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace U9.AreaSlider.Demo
{
    public class AreaTest : MonoBehaviour
    {
        [SerializeField] private Transform character;
        [SerializeField] private AreaSlider area;

        [SerializeField] private float[] weights;

        private float xIncrease = 1;
        private float yIncrease = 1;
        // Start is called before the first frame update
        void Start()
        {
            area.OnValuesChanged += OnWeightChanged;
        }

        // Update is called once per frame
        void Update()
        {
        }

        void OnWeightChanged(float[] w)
        {
            weights = w;

            xIncrease = 1 + w[0] - w[2];

            yIncrease = 1 + w[3];
            yIncrease = 1 - w[1];

            character.localScale = new Vector3(xIncrease, yIncrease, 1);
        }
    }
}
