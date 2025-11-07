using UnityEditor;

namespace U9.OSC
{
    [CustomEditor(typeof(OSCSettings))]
    public class OSCSettingsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var myScript = target as OSCSettings;

            if (!myScript.ResendHeartbeatOnReceive)
                myScript.SendHeartbeatTime = EditorGUILayout.FloatField("Send Heartbeat Time", myScript.SendHeartbeatTime);

        }
    }
}