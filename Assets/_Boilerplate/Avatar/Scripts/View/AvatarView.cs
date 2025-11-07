using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace U9.Avatar
{
    public class AvatarView : MonoBehaviour
    {
        [Header("General")]

        [SerializeField] Animator m_Animator;
        [SerializeField] bool m_UseLowPolyAssets = false;

        [Header("Renderers")]
        [SerializeField] List<ConfigurableMeshRenderer> m_Renderers = new List<ConfigurableMeshRenderer>();

        Dictionary<ColorSlot, ColorConfig> m_CurrentColorConfigs = new Dictionary<ColorSlot, ColorConfig>();

        bool m_Initted = false;

        public Animator Animator
        {
            get
            {
                if (m_Animator == null)
                    m_Animator = GetComponent<Animator>();
                return m_Animator;
            }
            set => m_Animator = value;
        }

        public ConfigurableMeshRenderer GetRenderer(PartSlot slot)
        {
            if (slot != null)
            {
                foreach (ConfigurableMeshRenderer renderer in m_Renderers)
                {
                    if (renderer.Slot != null && renderer.Slot.name == slot.name)
                        return renderer;
                }
            }

            //If we got here, we don't have a slot for this yet. add it.
            ConfigurableMeshRenderer newSlotRenderer = new ConfigurableMeshRenderer(slot);
            InitRenderer(newSlotRenderer);

            return newSlotRenderer;
        }

        public ConfigurableMeshRenderer GetRenderer(ColorSlot slot)
        {
            if (slot != null)
            {
                foreach (ConfigurableMeshRenderer renderer in m_Renderers)
                {
                    if (renderer.ColorSlot != null && renderer.ColorSlot.name == slot.name)
                        return renderer;
                }
            }

            //If we got here, we don't have a slot for this yet. add it.
            ConfigurableMeshRenderer newSlotRenderer = new ConfigurableMeshRenderer(slot);
            InitRenderer(newSlotRenderer);

            return newSlotRenderer;
        }

        void Init()
        {
            if (!m_Initted)
            {
                m_Initted = true;

                if (m_Animator == null)
                    m_Animator = GetComponent<Animator>();



                //Init the renderers (Preps the default values)
                foreach (ConfigurableMeshRenderer renderer in m_Renderers)
                {
                    InitRenderer(renderer);
                }
            }
        }

        /// <summary>
        /// Inits the given renderer
        /// </summary>
        /// <param name="renderer"></param>
        void InitRenderer(ConfigurableMeshRenderer renderer)
        {
            renderer.Init(transform);
            renderer.OnLoaded += OnRendererLoaded;
        }

        /// <summary>
        /// Called when a part successfully loads
        /// </summary>
        /// <param name="renderer"></param>
        void OnRendererLoaded(ConfigurableMeshRenderer renderer)
        {
            if (renderer != null)
            {
                foreach (var colorConfig in m_CurrentColorConfigs)
                {
                    if (colorConfig.Value != null)
                        renderer.ApplyColor(colorConfig.Value);
                }

            }
        }

        /// <summary>
        /// Prepare the view
        /// </summary>
        void Awake()
        {
            Init();
        }


        /// <summary>
        /// Apply the config to one of the renderers.
        /// </summary>
        /// <param name="config"></param>
        public void Apply(PartConfig config)
        {

            if (config != null && config.Slot != null)
            {
                var renderer = GetRenderer(config.Slot);
                Apply(renderer, config);
            }
        }

        /// <summary>
        /// Applies the given config and reapplies colors.
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="config"></param>
        void Apply(ConfigurableMeshRenderer renderer, PartConfig config)
        {
            renderer.Apply(config, m_UseLowPolyAssets);

            foreach (var colorConfig in m_CurrentColorConfigs)
            {
                if (colorConfig.Value != null)
                    renderer.ApplyColor(colorConfig.Value);
            }
        }

        public void Apply(ColorConfig config)
        {
            if (config.Slot != null)
            {

                if (m_CurrentColorConfigs.ContainsKey(config.Slot))
                    m_CurrentColorConfigs[config.Slot] = config;
                else
                    m_CurrentColorConfigs.Add(config.Slot, config);
                //var renderer = GetRenderer(config.Slot);
                //Debug.Log(renderer.ToString());
                foreach (ConfigurableMeshRenderer renderer in m_Renderers)
                    renderer.ApplyColor(config);

            }
        }

        /// <summary>
        /// Reset the given renderer to its default value, thus unapplying a custom config.
        /// </summary>
        /// <param name="slot"></param>
        public void ResetToDefault(PartSlot slot)
        {
            if (slot != null)
            {
                var renderer = GetRenderer(slot);
                ResetToDefault(renderer);
            }
        }

        /// <summary>
        /// Resets the given renderer and reapplies colors.
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="config"></param>
        void ResetToDefault(ConfigurableMeshRenderer renderer)
        {
            renderer.ResetToDefault();

            foreach (var colorConfig in m_CurrentColorConfigs)
            {
                if (colorConfig.Value != null)
                    renderer.ApplyColor(colorConfig.Value);
            }
        }

        public static void CleanUpLooseReferences()
        {
            //There is a potential bug where references can fail to unload. Call this every so often to clear out the lost references.
            Resources.UnloadUnusedAssets();
        }
    }

    /// <summary>
    /// A sub class to handle mesh renderer specific tasks
    /// </summary>
    [System.Serializable]
    public class ConfigurableMeshRenderer
    {
        [SerializeField] PartSlot m_Slot;
        [SerializeField] ColorSlot m_ColorSlot;
        [SerializeField] SkinnedMeshRenderer m_Renderer;

        //The default values before a config is applied
        SkinnedMeshRenderer m_DefaultRenderer;
        Transform m_Parent;

        //The current loaded config
        PartConfig m_CurrentConfig = null;

        //The load handles. Used to unload the assets
        AsyncOperationHandle<Mesh> m_MeshComboHandle;
        AsyncOperationHandle<Mesh> m_PrevMeshComboHandle;

        AsyncOperationHandle<Material> m_MaterialComboHandle;
        AsyncOperationHandle<Material> m_PrevMaterialComboHandle;

        //Called when the mesh and material have loaded
        public Action<ConfigurableMeshRenderer> OnLoaded;

        public ConfigurableMeshRenderer(PartSlot slot)
        {
            m_Slot = slot;
        }

        public ConfigurableMeshRenderer(ColorSlot slot)
        {
            m_ColorSlot = slot;
        }

        public SkinnedMeshRenderer Renderer { get => m_Renderer == null ? m_DefaultRenderer : m_Renderer; }
        public PartSlot Slot { get => m_Slot; }
        public ColorSlot ColorSlot { get => m_ColorSlot; }

        /// <summary>
        /// Returns the name of the config for saving purposes
        /// </summary>
        /// <returns></returns>
        public string GetCurrentPartID()
        {
            if (m_CurrentConfig != null)
            {

                return m_CurrentConfig.ID;
            }
            else
                return string.Empty;
        }

        /// <summary>
        /// Preps the default values so we can revert later on
        /// </summary>
        public void Init(Transform parent)
        {
            m_Parent = parent;

            if (m_Renderer != null)
            {
                //Capture the defaults if required
                m_DefaultRenderer = m_Renderer;
            }
        }

        /// <summary>
        /// Removes a custom config and returns to the default renderer setup
        /// </summary>
        public void ResetToDefault()
        {
            UnapplyLastConfig();

            if (m_DefaultRenderer != null)
                m_DefaultRenderer.enabled = true;
        }

        /// <summary>
        /// Applies the specified config. If it is null, it will reset to default instead.
        /// </summary>
        /// <param name="config"></param>
        public void Apply(PartConfig config, bool useLowPoly)
        {
            //Only continue if we are using a new config
            if (config == m_CurrentConfig)
                return;

            //If valid, load it
            if (config != null)
            {
                AssetReferenceMesh combo = config.GetCombination(useLowPoly);
                AssetReferenceMaterial mat = config.GetMaterial;

                if (combo.RuntimeKeyIsValid())
                {

                    m_CurrentConfig = config;

                    m_PrevMeshComboHandle = m_MeshComboHandle;

                    m_MeshComboHandle = Addressables.LoadAssetAsync<Mesh>(combo);//Addressables.InstantiateAsync(combo,m_Parent); //check laliga
                    m_MeshComboHandle.Completed += OnMeshComboLoaded;

                    m_PrevMaterialComboHandle = m_MaterialComboHandle;

                    m_MaterialComboHandle = Addressables.LoadAssetAsync<Material>(mat);
                    m_MaterialComboHandle.Completed += OnMaterialLoaded;

                    return;
                }
            }

            //If invalid, reset
            ResetToDefault();
        }

        /// <summary>
        /// Triggered once the mesh handles resolves
        /// </summary>
        /// <param name="obj"></param>
        private void OnMeshComboLoaded(AsyncOperationHandle<Mesh> obj) //ref laliga
        {
            if (m_MeshComboHandle.IsValid() && m_MeshComboHandle.IsDone)
            {
                if(m_DefaultRenderer != null)
                    m_DefaultRenderer.enabled = false;

                m_Renderer.sharedMesh = obj.Result;//.GetComponent<SkinnedMeshRenderer>(); 
                m_Renderer.enabled = true;

                if (OnLoaded != null)
                    OnLoaded(this);

                ReleasePreviousHandles();
            }
        }

        private void OnMaterialLoaded(AsyncOperationHandle<Material> obj)
        {
            if (m_MaterialComboHandle.IsValid() && m_MaterialComboHandle.IsDone)
            {
                m_Renderer.sharedMaterial = obj.Result;
            }
        }

        void ReleasePreviousHandles()
        {
            if (m_PrevMeshComboHandle.IsValid())
                Addressables.ReleaseInstance(m_PrevMeshComboHandle);

            if (m_PrevMaterialComboHandle.IsValid())
                Addressables.ReleaseInstance(m_PrevMaterialComboHandle);
        }

        /// <summary>
        /// Releases the last config
        /// </summary>
        public void UnapplyLastConfig()
        {
            if (m_CurrentConfig != null)
            {
                if (m_MeshComboHandle.IsValid())
                    Addressables.ReleaseInstance(m_MeshComboHandle);
                m_CurrentConfig = null;
            }
        }

        /// <summary>
        /// Applies the specified color to a shader property.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="colorId"></param>
        public void ApplyColor(ColorConfig config)
        {
            if (m_Renderer != null)
            {
                foreach (Material m in m_Renderer.materials)
                {
                    if (m != null)
                    {
                        m.SetColor(config.Slot.ShaderColorName, config.Color);
                    }
                }
            }
        }
    }
}
