using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.Localization.Settings;
using U9.Utils;

public class QAView : MonoSingleton<QAView> {

	[SerializeField] GameObject m_Container;
	[SerializeField] TextMeshProUGUI m_BuildLabel;
	[SerializeField] TextMeshProUGUI m_UnityLabel;
	[SerializeField] TextMeshProUGUI m_LocaleLabel;
	[SerializeField] TextMeshProUGUI m_SceneLabel;
	[SerializeField] TextMeshProUGUI m_PlatformLabel;
	[SerializeField] TextMeshProUGUI m_DeviceLabel;
	[SerializeField] TextMeshProUGUI m_CurrentTimeLabel;
	[SerializeField] TextMeshProUGUI m_RuntimeLabel;

	void Awake()
	{
		Instance = this;
	}

	// Use this for initialization
	void Start () {
		
		m_BuildLabel.text = "Build: " + Application.version;
		m_UnityLabel.text = "Unity: " + Application.unityVersion;
		m_PlatformLabel.text = "Platform: " + Application.platform.ToString ();
		m_DeviceLabel.text = "Device: " + SystemInfo.deviceModel;

        LocalizationSettings.SelectedLocaleChanged += SetLocaleLabel;
		SetLocaleLabel(LocalizationSettings.SelectedLocale);
	}


	private void SetLocaleLabel(UnityEngine.Localization.Locale locale)
	{
		string n = locale.LocaleName.ToString ().Replace ('_', ' ');
		string c = locale.Identifier.Code.ToString ().ToUpper ().Replace('_','-');

		m_LocaleLabel.text = "Locale: " + n + " / " + c;
	}

	public void SetSceneLabel(string scene)
	{
		m_SceneLabel.text = "Scene: " + scene;
	}

	// Update is called once per frame
	void Update () {
		if (m_Container.activeSelf) {
			m_CurrentTimeLabel.text = "Current Time: " + DateTime.Now.ToString ("dd/MM/yyyy HH:mm:ss");

			TimeMaths.SecondsToHMSMS(Time.time, out int hours, out int minutes, out int seconds, out int milliseconds);
			
			m_RuntimeLabel.text = string.Format ("Runtime: {0}:{1}:{2}.{3}", hours.ToString ("00"), minutes.ToString ("00"), seconds.ToString ("00"), milliseconds.ToString ("0000"));
		}
		if (Input.GetKeyDown (KeyCode.Tab))
			m_Container.SetActive (!m_Container.activeSelf);
	}
}
