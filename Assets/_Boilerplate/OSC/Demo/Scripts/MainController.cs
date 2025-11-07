using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace U9.OSC.Demo
{
    public class MainController : MonoBehaviour
    {
        [SerializeField] private OSCController osc;
        [SerializeField] private Button testButton;
        [SerializeField] private TextMeshProUGUI recevieText;

        private void Start()
        {
            testButton.onClick.AddListener(OnTestClick);
            osc.OnTest += OnTestReceived;
        }

        void OnTestClick()
        {
            osc.SendMessageToClient(OSCCommands.k_TestCommand, "this is a test", true);
        }

        void OnTestReceived(string txt)
        {
            recevieText.text = txt;
        }
    }
}
