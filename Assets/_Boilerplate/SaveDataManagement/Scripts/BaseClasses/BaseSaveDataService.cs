using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using U9.Errors.Codes;

namespace U9.SaveDataManagement
{
    public abstract class BaseSaveDataService : MonoBehaviour
    {
        [SerializeField] private ResponseCodeOption[] _responseCodeDetailOptions = new ResponseCodeOption[0];
        public const int SUCCESS_CODE = 200;
        public const int NO_DATA_CODE = 2001;

        protected SaveDataServiceResponse CreateResponse(int responseCode, string responseValue)
        {
            ResponseCodeOption responseDetails = null;

            foreach(var option in _responseCodeDetailOptions)
            {
                if(option.Code == responseCode)
                {
                    responseDetails = option;
                    break;
                }
            }

            return new SaveDataServiceResponse(responseCode, responseValue, responseDetails);
        }

        protected SaveDataServiceResponse CreateSuccessfulResponse(string responseValue)
        {
            return CreateResponse(SUCCESS_CODE, responseValue);
        }

        protected BoolSaveDataServiceResponse CreateResponse(int responseCode, string responseValue, bool containsId)
        {
            ResponseCodeOption responseDetails = null;

            foreach (var option in _responseCodeDetailOptions)
            {
                if (option.Code == responseCode)
                {
                    responseDetails = option;
                    break;
                }
            }

            return new BoolSaveDataServiceResponse(responseCode, responseValue, containsId, responseDetails);
        }
        protected SlotSaveDataServiceResponse CreateResponse(int responseCode, string responseValue, int[] slotIndexes)
        {
            ResponseCodeOption responseDetails = null;

            foreach (var option in _responseCodeDetailOptions)
            {
                if (option.Code == responseCode)
                {
                    responseDetails = option;
                    break;
                }
            }

            return new SlotSaveDataServiceResponse(responseCode, responseValue, slotIndexes, responseDetails);
        }

        protected BoolSaveDataServiceResponse CreateSuccessfulResponse(bool containsId)
        {
            return CreateResponse(SUCCESS_CODE, string.Empty, containsId);
        }

        protected SlotSaveDataServiceResponse CreateSuccessfulResponse(int[] slotIndexes)
        {
            return CreateResponse(SUCCESS_CODE, string.Empty, slotIndexes);
        }

        protected SaveDataServiceResponse CreateNoDataResponse()
        {
            return CreateResponse(NO_DATA_CODE, string.Empty);
        }


        /// <summary>
        /// Saves the given data
        /// </summary>
        /// <param name="dataId">Id of the data to save, for example "systemData"</param>
        /// <param name="dataValue">value of the data to save.</param>
        /// <param name="slotIndex">It's possible to have multiple versions of a save data, which slot are we saving to?.</param>
        public virtual async UniTask<SaveDataServiceResponse> Save(string dataId, string dataValue, int slotIndex)
        {
            await UniTask.DelayFrame(1);
            return CreateSuccessfulResponse("");
        }

        /// <summary>
        /// Loads the given data
        /// </summary>
        /// <param name="dataId">Id of the data to load, for example "system data"</param>
        /// <param name="slotIndex">It's possible to have multiple versions of a save data, which slot are we loading from?.</param>
        public virtual async UniTask<SaveDataServiceResponse> Load(string dataId, int slotIndex)
        {
            await UniTask.DelayFrame(1);
            return CreateSuccessfulResponse("");
        }

        /// <summary>
        /// Deletes the given data
        /// </summary>
        /// <param name="dataId">Id of the data to delete, for example "system data"</param>
        /// <param name="slotIndex">It's possible to have multiple versions of a save data, which slot are we deleting?.</param>
        /// <returns></returns>
        public virtual async UniTask<SaveDataServiceResponse> Delete(string dataId, int slotIndex)
        {
            await UniTask.DelayFrame(1);
            return CreateSuccessfulResponse("");
        }

        /// <summary>
        /// Checks if the data service has any data for the given ID
        /// </summary>
        /// <param name="dataId">Id of the data to check for, for example "system data"</param>
        public virtual async UniTask<BoolSaveDataServiceResponse> Contains(string dataId)
        {
            await UniTask.DelayFrame(1);
            return CreateSuccessfulResponse(true);
        }

        /// <summary>
        /// Checks if the data service has any data for the given ID
        /// </summary>
        /// <param name="dataId">Id of the data to check for, for example "system data"</param>
        public virtual async UniTask<SlotSaveDataServiceResponse> GetSlotIndexes(string dataId)
        {
            await UniTask.DelayFrame(1);
            return CreateSuccessfulResponse(new int[0]);
        }
    }

    [System.Serializable]
    public class SaveDataServiceResponse
    {
        public bool isSuccessful { get => code == BaseSaveDataService.SUCCESS_CODE; }
        public int code { get; }
        public ResponseCodeOption details { get; }
        public string value { get; }

        public SaveDataServiceResponse(int code, string value, ResponseCodeOption details)
        {
            this.code = code;
            this.value = value;
            this.details = details;
        }
    }

    [System.Serializable]
    public class BoolSaveDataServiceResponse : SaveDataServiceResponse
    {
        public bool boolValue { get; }

        public BoolSaveDataServiceResponse(int code, string value, bool boolValue, ResponseCodeOption details) :base (code, value,details)
        {
            this.boolValue = boolValue;
        }
    }

    [System.Serializable]
    public class SlotSaveDataServiceResponse : SaveDataServiceResponse
    {
        public int[] slotValues { get; }

        public SlotSaveDataServiceResponse(int code, string value, int[] slotValues, ResponseCodeOption details) : base(code, value, details)
        {
            this.slotValues = slotValues;
        }
    }
}