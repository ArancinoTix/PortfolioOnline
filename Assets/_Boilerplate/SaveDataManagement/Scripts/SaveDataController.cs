using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using U9.Errors.Codes;
using U9.Encryption;
using VisualInspector;
using HeaderAttribute = VisualInspector.HeaderAttribute;


namespace U9.SaveDataManagement
{
    /// <summary>
    /// Implement this class for your specific project
    /// </summary>
    /// <typeparam name="TClass"></typeparam>
    public class SaveDataController : MonoSingleton<SaveDataController>
    {
        [Header("Service/Data Errors")]

        [Order(-70)] [SerializeField] private ResponseCodeOption _nullServiceDetails;

        [Order(-69)] [Separator][SerializeField] private ResponseCodeOption _nullDataDetails;

        [Header("Encryption")]
        [Separator] [Order(100)] [SerializeField] private BaseEncryptionService _encryptionService = null;

        [SerializeField] private BaseSaveDataService _dataService;

        /// <summary>
        /// Save data structures to be stored.
        /// Examples include:
        /// -- System (Settings)
        /// -- Login
        /// -- Gameplay
        /// 
        /// Each should be contained in a unique class
        /// </summary>
        private Dictionary<Type, BaseSaveDataStructure> _dataStructures = new Dictionary<Type, BaseSaveDataStructure>();

        public BaseEncryptionService EncryptionService { get => _encryptionService;  }

        //-------------------------------------------------------------------------------------------------------------------
        // INIT
        //-------------------------------------------------------------------------------------------------------------------

        protected virtual void Awake()
        {
            Instance = this;
        }

        //-------------------------------------------------------------------------------------------------------------------
        // GET/SET CURRENT DATA
        //-------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Retrieves the current data in memory for the given type if it exists
        /// </summary>
        /// <typeparam name="TDataStructure">The class type of the data to retrive, for example SystemDataStructure</typeparam>
        /// <returns>Returns the data instance if it exists, or default if it does not</returns>
        public TDataStructure GetData<TDataStructure>()
            where TDataStructure : BaseSaveDataStructure
        {
            var givenType = typeof(TDataStructure);

            if (_dataStructures.ContainsKey(givenType))
                return (TDataStructure)_dataStructures[givenType];
            else
                return default(TDataStructure);
        }

        /// <summary>
        /// Adds the give data type. It is only possible to have one of each.
        /// </summary>
        /// <param name="data">The data instance to add</param>
        public void AddData(BaseSaveDataStructure data, bool replace = false)
        {
            if (data == null)
                return;

            var givenType = data.GetType();

            if (_dataStructures.ContainsKey(givenType))
            {
                if (replace)
                    _dataStructures.Remove(givenType);
                else
                    return;
            }

            _dataStructures.Add(givenType, data);

        }

        //-------------------------------------------------------------------------------------------------------------------
        // ADD/REMOVE/GET DATA SERVICES
        //-------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Adds the given instance of the data service.
        /// </summary>
        /// <param name="serviceInstance">The instance of the service to add, for example PlayerPrefsDataService</param>
        public void RegisterService(BaseSaveDataService serviceInstance)
        {
            _dataService = serviceInstance;
        }

        //-------------------------------------------------------------------------------------------------------------------
        // ENCRYPTION FUNCTIONALITY
        //-------------------------------------------------------------------------------------------------------------------

        protected string Encrypt(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            if (_encryptionService != null)
                return _encryptionService.Encrypt(value);
            else
                return value;
        }

        protected string Decrypt(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            if (_encryptionService != null)
                return _encryptionService.Decrypt(value);
            else
                return value;
        }

        //-------------------------------------------------------------------------------------------------------------------
        // SAVE DATA - SERVICE FUNCTIONALITY
        //-------------------------------------------------------------------------------------------------------------------
       
        /// <summary>
        /// Attempt to save the given data type to the service
        /// </summary>
        /// <typeparam name="TDataStructure">The class type of the data to save, for example SystemDataStructure</typeparam>
        public async UniTask<SaveDataServiceResponse> Save<TDataStructure>()
            where TDataStructure : BaseSaveDataStructure
        {
            var dataStructure = GetData<TDataStructure>();
            return await SaveFrom(dataStructure, dataStructure.SaveSlot);
        }

        /// <summary>
        /// Attempt to save the given data type to the service
        /// </summary>
        /// <typeparam name="TDataStructure">The class type of the data to save, for example SystemDataStructure</typeparam>
        public async UniTask<SaveDataServiceResponse> Save<TDataStructure>(int slotIndex)
            where TDataStructure : BaseSaveDataStructure
        { 
            var dataStructure = GetData<TDataStructure>();
            return await SaveFrom(dataStructure, slotIndex);
        }

        /// <summary>
        /// Attempt to save the given data type from the provided object
        /// </summary>
        /// <param name="dataStructure"></param>
        /// <returns></returns>
        public async UniTask<SaveDataServiceResponse> SaveFrom(BaseSaveDataStructure dataStructure, int slotIndex = 0)
        { 
            if (dataStructure == null)
            {
                await UniTask.DelayFrame(1);
                if (_nullDataDetails != null)
                    return new SaveDataServiceResponse(_nullDataDetails.Code, "Data is null", _nullDataDetails);
                else
                    return new SaveDataServiceResponse(-2, "Data is null", null);
            }
            else if (_dataService == null)
            {
                await UniTask.DelayFrame(1);
                if (_nullServiceDetails != null)
                    return new SaveDataServiceResponse(_nullServiceDetails.Code, "Service is null", _nullServiceDetails);
                else
                    return new SaveDataServiceResponse(-1, "Service is null", null);
            }
            else
            {
                var encryptedData = Encrypt(dataStructure.Serialize());
                dataStructure.SaveSlot = slotIndex;

                var response = await _dataService.Save(dataStructure.GetId(), encryptedData, slotIndex);
                return response;
            }
        }

        //-------------------------------------------------------------------------------------------------------------------
        // LOAD DATA - SERVICE FUNCTIONALITY
        //-------------------------------------------------------------------------------------------------------------------
  
        
        /// <summary>
        /// Loads to the first data service
        /// </summary>
        /// <typeparam name="TDataStructure">The class type of the data to load, for example SystemDataStructure</typeparam>
        public async UniTask<SaveDataServiceResponse> Load<TDataStructure>(int slotIndex = 0)
            where TDataStructure : BaseSaveDataStructure
        {
            var dataStructure = GetData<TDataStructure>();
            return await LoadInto(dataStructure, slotIndex);
        }

        /// <summary>
        /// Attempt to load the given data type in to the provided data container.
        /// </summary>
        /// <typeparam name="TDataStructure">The class type of the data to load, for example SystemDataStructure</typeparam>
        public async UniTask<SaveDataServiceResponse> LoadInto(BaseSaveDataStructure dataStructure, int slotIndex = 0)
        {
            if (dataStructure == null)
            {
                await UniTask.DelayFrame(1);
                if (_nullDataDetails != null)
                    return new SaveDataServiceResponse(_nullDataDetails.Code, "Data structure is null", _nullDataDetails);
                else
                    return new SaveDataServiceResponse(-2, "Data structure is null", null);
            }
            else if (_dataService == null)
            {
                await UniTask.DelayFrame(1);
                if (_nullServiceDetails != null)
                    return new SaveDataServiceResponse(_nullServiceDetails.Code, "Service is null", _nullServiceDetails);
                else
                    return new SaveDataServiceResponse(-1, "Service is null", null);                
            }
            else
            {
                var response = await _dataService.Load(dataStructure.GetId(), slotIndex);

                if(response.isSuccessful)
                {
                    dataStructure.Deserialize(Decrypt(response.value));

                    //We create a new response as we do not want to expose the raw data
                    response = new SaveDataServiceResponse(response.code, "", response.details);
                }

                return response;
            }
        }

        //-------------------------------------------------------------------------------------------------------------------
        // DELETE DATA - SERVICE FUNCTIONALITY
        //-------------------------------------------------------------------------------------------------------------------
       
        /// <summary>
        /// Loads to the first data service
        /// </summary>
        /// <typeparam name="TDataStructure">The class type of the data to load, for example SystemDataStructure</typeparam>
        public async UniTask<SaveDataServiceResponse> Delete<TDataStructure>()
            where TDataStructure : BaseSaveDataStructure
        {
            var dataStructure = GetData<TDataStructure>();
            return await Delete(dataStructure, dataStructure.SaveSlot);
        }

        /// <summary>
        /// Loads to the first data service
        /// </summary>
        /// <typeparam name="TDataStructure">The class type of the data to load, for example SystemDataStructure</typeparam>
        public async UniTask<SaveDataServiceResponse> Delete<TDataStructure>(int slotIndex)
            where TDataStructure : BaseSaveDataStructure
        {
            var dataStructure = GetData<TDataStructure>();
            return await Delete(dataStructure, slotIndex);
        }

        /// <summary>
        /// Attempt to load the given data type in to the provided data container.
        /// </summary>
        /// <typeparam name="TDataStructure">The class type of the data to load, for example SystemDataStructure</typeparam>
        private async UniTask<SaveDataServiceResponse> Delete(BaseSaveDataStructure dataStructure, int slotIndex = 0)
        {
            if (dataStructure == null)
            {
                await UniTask.DelayFrame(1);
                if (_nullDataDetails != null)
                    return new SaveDataServiceResponse(_nullDataDetails.Code, "Data structure is null", _nullDataDetails);
                else
                    return new SaveDataServiceResponse(-2, "Data structure is null", null);
            }
            else if (_dataService == null)
            {
                await UniTask.DelayFrame(1);
                if (_nullServiceDetails != null)
                    return new SaveDataServiceResponse(_nullServiceDetails.Code, "Service is null", _nullServiceDetails);
                else
                    return new SaveDataServiceResponse(-1, "Service is null", null);
            }
            else
            {
                var response = await _dataService.Delete(dataStructure.GetId(), slotIndex);

                if (response.isSuccessful)
                {
                    response = new SaveDataServiceResponse(response.code, "", response.details);
                }

                return response;
            }
        }

        //-------------------------------------------------------------------------------------------------------------------
        // CHECK DATA EXISTS - SERVICE FUNCTIONALITY
        //-------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Attempt to check if we have any data for the given type for the specific service
        /// </summary>
        /// <typeparam name="TDataService">>The class type of the service to check, for example PlayerPrefsDataService</typeparam>
        /// <typeparam name="TDataStructure">The class type of the data to check, for example SystemDataStructure</typeparam>
        public async UniTask<SaveDataServiceResponse> CheckIfDataServiceHasData<TDataStructure>()
            where TDataStructure : BaseSaveDataStructure
        {
            var dataStructure = GetData<TDataStructure>();

            if (dataStructure == null)
            {
                await UniTask.DelayFrame(1);
                if (_nullDataDetails != null)
                    return new SaveDataServiceResponse(_nullDataDetails.Code, "Data structure is null", _nullDataDetails);
                else
                    return new SaveDataServiceResponse(-2, "Data structure is null", null);
            }
            else if (_dataService == null)
            {
                await UniTask.DelayFrame(1);
                if (_nullServiceDetails != null)
                    return new SaveDataServiceResponse(_nullServiceDetails.Code, "Service is null", _nullServiceDetails);
                else
                    return new SaveDataServiceResponse(-1, "Service is null", null);
            }
            else
            {
                return await _dataService.Contains(dataStructure.GetId());
            }
        }

        public async UniTask<SlotSaveDataServiceResponse> GetSaveSlotIndexes<TDataStructure>()
           where TDataStructure : BaseSaveDataStructure
        {
            var dataStructure = GetData<TDataStructure>();

            if (dataStructure == null)
            {
                await UniTask.DelayFrame(1);
                if (_nullDataDetails != null)
                    return new SlotSaveDataServiceResponse(_nullDataDetails.Code, "Data structure is null", new int[0], _nullDataDetails);
                else
                    return new SlotSaveDataServiceResponse(-2, "Data structure is null", new int[0], null);
            }
            else if (_dataService == null)
            {
                await UniTask.DelayFrame(1);
                if (_nullServiceDetails != null)
                    return new SlotSaveDataServiceResponse(_nullServiceDetails.Code, "Service is null", new int[0], _nullServiceDetails);
                else
                    return new SlotSaveDataServiceResponse(-1, "Service is null", new int[0], null);
            }
            else
            {
                return await _dataService.GetSlotIndexes(dataStructure.GetId());
            }
        }
    }
}
