using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace U9.SaveDataManagement
{
    public class PlayerPrefsDataService : BaseSaveDataService
    {
        const char SLOT_LIST_PREPEND = '[';
        const char SLOT_LIST_APPEND = ']';

        private void Awake()
        {
            //PlayerPrefs.DeleteAll();
        }

        public override async UniTask<SaveDataServiceResponse> Save(string dataId, string dataValue, int slotIndex)
        {
            await UniTask.DelayFrame(1);


            //As we can have multiple slots, we need to keep track of them.
            string slotListEntry = SLOT_LIST_PREPEND + slotIndex.ToString() + SLOT_LIST_APPEND; // [0]
            string slotListTrackingName = SlotTrackingName(dataId);

            string slotIndexTrackingValue = PlayerPrefs.GetString(slotListTrackingName);

            if (!slotIndexTrackingValue.Contains(slotListEntry))
                slotIndexTrackingValue += slotListEntry;

#if UNITY_EDITOR
            Debug.Log($"### PlayerPrefsDataService - Saving [{SlotName(dataId, slotIndex)}] - [{dataValue}]");
            Debug.Log($"### PlayerPrefsDataService - Saving [{slotListTrackingName}] - [{slotIndexTrackingValue}]");
#endif

            PlayerPrefs.SetString(SlotName(dataId,slotIndex), dataValue);
            PlayerPrefs.SetString(slotListTrackingName, slotIndexTrackingValue);

            return CreateSuccessfulResponse("");
        }

        public override async UniTask<SaveDataServiceResponse> Load(string dataId, int slotIndex)
        {
            await UniTask.DelayFrame(1);

#if UNITY_EDITOR
            Debug.Log($"### PlayerPrefsDataService - Loading [{SlotName(dataId, slotIndex)}]");
#endif
            var key = SlotName(dataId, slotIndex);

            if (PlayerPrefs.HasKey(key))
                return CreateSuccessfulResponse(PlayerPrefs.GetString(key, ""));
            else
                return CreateNoDataResponse();
        }

        public override async UniTask<BoolSaveDataServiceResponse> Contains(string dataId)
        {
            await UniTask.DelayFrame(1);

#if UNITY_EDITOR
            Debug.Log($"### PlayerPrefsDataService - Checking for [{dataId}]");
#endif
            string slotListTrackingName = SlotTrackingName(dataId);
            string slotIndexTrackingValue = PlayerPrefs.GetString(slotListTrackingName);

            return CreateSuccessfulResponse(slotIndexTrackingValue.Length>0); //If the list is not empty, we have saves
        }

        public override async UniTask<SaveDataServiceResponse> Delete(string dataId, int slotIndex)
        {
            await UniTask.DelayFrame(1);

            //As we can have multiple slots, we need to remove knowledge of this slot
            string slotListEntry = SLOT_LIST_PREPEND + slotIndex.ToString() + SLOT_LIST_APPEND; // [0]
            string slotListTrackingName = SlotTrackingName(dataId);

            string slotIndexTrackingValue = PlayerPrefs.GetString(slotListTrackingName);

            if (slotIndexTrackingValue.Contains(slotListEntry))
                slotIndexTrackingValue = slotIndexTrackingValue.Replace(slotListEntry, "");

#if UNITY_EDITOR
            Debug.Log($"### PlayerPrefsDataService - Deleting [{SlotName(dataId, slotIndex)}]");
            Debug.Log($"### PlayerPrefsDataService - Saving [{slotListTrackingName}] - [{slotIndexTrackingValue}]");
#endif

            PlayerPrefs.DeleteKey(SlotName(dataId, slotIndex));
            PlayerPrefs.SetString(slotListTrackingName, slotIndexTrackingValue); //Save the new list of indexes

            return CreateSuccessfulResponse("");
        }

        public override async UniTask<SlotSaveDataServiceResponse> GetSlotIndexes(string dataId)
        {
            await UniTask.DelayFrame(1);

            //As we can have multiple slots, we need to remove knowledge of this slot
            string slotListTrackingName = SlotTrackingName(dataId);
            string slotIndexTrackingValue = PlayerPrefs.GetString(slotListTrackingName);
            slotIndexTrackingValue = slotIndexTrackingValue.Replace(SLOT_LIST_PREPEND.ToString(), "");

            string[] splits = slotIndexTrackingValue.Split(SLOT_LIST_APPEND,StringSplitOptions.RemoveEmptyEntries);
            var indexes = new int[splits.Length];

            for(int i =0, ni = splits.Length; i<ni;i++)
            {
                indexes[i] = int.Parse(splits[i]);
            }

            return CreateSuccessfulResponse(indexes);
        }

        private string SlotName(string id, int slot)
        {
            return string.Format("{0}-{1}", id, slot);
        }

        private string SlotTrackingName(string id)
        {
            return string.Format("{0}-List", id);
        }
    }
}
