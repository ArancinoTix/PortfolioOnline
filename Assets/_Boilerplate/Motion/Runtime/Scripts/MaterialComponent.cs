using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace U9.Motion
{
    /// <summary>
    /// Special component to add ability to change Material shader properties via Motion
    /// </summary>
    public class MaterialComponent : MonoBehaviour
    {
        public Material material;

        private void Awake()
        {
            material = Instantiate(material);

            // Mesh Render found? Apply copy of the material
            if (TryGetComponent<MeshRenderer>(out var meshRenderer))
            {
                meshRenderer.material = material;
                return;
            }
            // Skinned mesh mrenderer found
            else if (TryGetComponent<SkinnedMeshRenderer>(out var skinnedMeshRenderer))
            {
                skinnedMeshRenderer.material = material;
                return;
            }
            // Raw Image found
            else if (TryGetComponent<RawImage>(out var rawImage))
            {
                rawImage.material = material;
                return;
            }
            // Image found
            else if (TryGetComponent<Image>(out var image))
            {
                image.material = material;
                return;
            }
        }
    }
}