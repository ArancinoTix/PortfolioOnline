using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace U9.Avatar
{


    [System.Serializable]
    public class AssetReferenceMesh : AssetReferenceT<Mesh>
    {
        public AssetReferenceMesh(string guid) : base(guid) { }
    }

    [System.Serializable]
    public class AssetReferenceMaterial : AssetReferenceT<Material> 
    {
        public AssetReferenceMaterial(string guid) : base(guid) { }
    }

    [CreateAssetMenu(fileName = "PartConfig", menuName = "UNIT9/Avatar/Configs/Part Config", order = 0)]
    public class PartConfig : ScriptableObject
    {
        [Header("UI")]
        [SerializeField] Sprite m_Icon;

        [Header("Mesh")]
        [SerializeField] PartSlot m_Slot;

        [SerializeField] AssetReferenceMesh m_LowQualityCombination;
        [SerializeField] AssetReferenceMesh m_HighQualityCombination;
        [SerializeField] AssetReferenceMaterial m_MaterialReference;

        public AssetReferenceMesh GetCombination(bool useLowQuality)
        {
            return useLowQuality ? m_LowQualityCombination: m_HighQualityCombination;
        }

        public AssetReferenceMesh GetValidCombination()
        {
            if (m_HighQualityCombination.IsValid())
                return m_HighQualityCombination;
            else
                return m_LowQualityCombination;
        }

        public AssetReferenceMaterial GetMaterial
        {
            get => m_MaterialReference;
        }

        public Sprite Icon { get => m_Icon; }
        public PartSlot Slot { get => m_Slot; }
        public string ID {
            get
            {
                string slotName = m_Slot != null ? m_Slot.name : string.Empty;
                return string.Format("{0}_{1}", slotName, name);
            }
        }

    }

    [System.Serializable]
    public class MeshMaterialCombination
    {
        [SerializeField] Mesh m_Mesh;
        [SerializeField] Material[] m_Materials;

        public Mesh Mesh { get => m_Mesh; }
        public Material[] Materials { get => m_Materials; }

    }
}
