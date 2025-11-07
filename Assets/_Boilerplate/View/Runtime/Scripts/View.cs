using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using U9.ProgressTransition;
using U9.Motion;
using VisualInspector;
using HeaderAttribute = VisualInspector.HeaderAttribute;
using UnityEngine.UI;

namespace U9.View
{
	public class View : MonoBehaviour
	{
		public enum ViewState
		{
			Unset,
			Displayed,
			Hidden,
			Displaying,
			Hiding
		}

		//----------------------------------------------------------------------------------------------------------------------------------//
		// Motion Variables
		//----------------------------------------------------------------------------------------------------------------------------------//

		[MessageBox("Transitions to be used to trigger displays/hides. " +
			"\nMost views only need one assigned, and the same transition will be applied to hide and display." +
			"\n\nOnly assign multiple if you want different ways of displaying. " +
			"\nFor example, slide in from the right from title screen, but slide in from the left from gameplay")]
		[Header("Transitions")]
		[SerializeField] private BaseProgressTransition[] _displayTransitions;

		[Separator]		
		[SerializeField] private BaseProgressTransition[] _hideTransitions;
		
		[MessageBox("Alternative display/hide animation. If assigned, the matching display/hide Transitions will be ignored.")]
		[Header("UI Motion (Optional)")]
		[SerializeField] private BaseMotion _displayMotion;

		[Separator]
		[SerializeField] private BaseMotion _hideMotion;

		//----------------------------------------------------------------------------------------------------------------------------------//
		// View Setting Variables
		//----------------------------------------------------------------------------------------------------------------------------------//

		[Header("View Settings")]
		[SerializeField] private bool _logDebugs = false;

		[Tooltip("Should this view be hidden when the scene loads?")]
		[SerializeField] private bool _hideOnInit = true;

		[Tooltip("Should the gameobject be disabled when we hide?")]
		[SerializeField] private bool _disableGameobjectOnHide = true;

		[Tooltip("When the view hides/displays, the assigned canvas will be enabled/disabled accordingly")]
		[SerializeField] private Canvas _canvasToDisableOnHide;

		[Separator]
		[Tooltip("A canvas group to block any input when the view state is not == Displayed")]
		[SerializeField] private CanvasGroup _interactionGroup;


		private bool _inited;                                      // Has the view been initiated?
		private bool _interactionEnabled = true;                   // Is interaction enabled on this view?
		private ViewState _state = ViewState.Unset;     // The current display state of this view.
		private BaseProgressTransition _lastActiveTransition = null;
		private BaseMotion _lastActiveMotion = null;
		private GraphicRaycaster _graphicRaycaster;

		/// <summary>
		/// An event that is triggered when the display state of this viw changes.
		/// </summary>
		public event System.Action<ViewState> StateChanged;

		private const float DISPLAYED_PROGRESS = 1;
		private const float HIDDEN_PROGRESS = 0;

		//----------------------------------------------------------------------------------------------------------------------------------//
		// Propeties & Getters
		//----------------------------------------------------------------------------------------------------------------------------------//

		/// <summary>
		/// The current display state of this view.
		/// </summary>
		public ViewState State
		{
			get
			{
				return this._state;
			}
			private set
			{
				bool changed = _state != value;
				_state = value;
				if (changed)
					StateChanged?.Invoke(value);
			}
		}

		/// <summary>
		/// Is this view displayed, or in the process of being displayed?
		/// </summary>
		public bool IsDisplaying
		{
			get
			{
				return State == ViewState.Displayed || State == ViewState.Displaying;
			}
		}

		/// <summary>
		/// Is this view displayed, 
		/// </summary>
		public bool IsDisplayed
		{
			get
			{
				return State == ViewState.Displayed;
			}
		}


		/// <summary>
		/// Has this view been initiated?
		/// </summary>
		public bool IsInited
		{
			get
			{
				return _inited;
			}
		}

        public bool IsInteractionEnabled { get => _interactionEnabled; }
        public CanvasGroup InteractionGroup { get => _interactionGroup;  }
        public bool LogDebugs { get => _logDebugs; set => _logDebugs = value; }


        //----------------------------------------------------------------------------------------------------------------------------------//
        // Init
        //----------------------------------------------------------------------------------------------------------------------------------//

        protected virtual void Awake()
		{
			if (_logDebugs)
				Debug.Log(name + ": Awake");

			AttemptInitView();
		}

		/// <summary>
		/// Inits the view.
		/// </summary>
		protected virtual void InitView()
		{
			if (_logDebugs)
				Debug.Log(name + ": InitView");

			_inited = true;
			if(_canvasToDisableOnHide != null)
            {
				_graphicRaycaster = _canvasToDisableOnHide.gameObject.GetComponent<GraphicRaycaster>();
				if (_graphicRaycaster == null)
					_graphicRaycaster = _canvasToDisableOnHide.gameObject.AddComponent<GraphicRaycaster>();
            }

			foreach (var displayTransition in _displayTransitions)
				displayTransition.PrepCalculationParams();

			foreach (var hideTransition in _hideTransitions)
				hideTransition.PrepCalculationParams();

			AddDisplayListeners();
			AddHideListeners();

			if (_hideOnInit)
			{
				Hide(0,0,true);
			}
			else
			{
				Display(0,0,true);
			}

			if (_logDebugs)
				Debug.Log(name + ": View Initted");
		}

		/// <summary>
		/// Inits the view.
		/// </summary>
		public void AttemptInitView()
		{
			if (!_inited)
			{
				InitView();
			}
		}

		//----------------------------------------------------------------------------------------------------------------------------------//
		// Listeners for Motions
		//----------------------------------------------------------------------------------------------------------------------------------//

		private void AddDisplayListeners()
		{
			if (_displayMotion != null)
			{
				_displayMotion.OnPlayMotion.AddListener(() => HandleDisplayTweenBegan());
				_displayMotion.OnFinishedMotion.AddListener(() => HandleDisplayTweenEnded());
			}
		}

		private void RemoveDisplayListeners()
		{
			if (_displayMotion != null)
			{
				_displayMotion.OnPlayMotion.RemoveListener(() => HandleDisplayTweenBegan());
				_displayMotion.OnFinishedMotion.RemoveListener(() => HandleDisplayTweenEnded());
			}
		}

		private void AddHideListeners()
		{
			if (_hideMotion != null)
			{
				_hideMotion.OnPlayMotion.AddListener(() => HandleHideTweenBegan());
				_hideMotion.OnFinishedMotion.AddListener(() => HandleHideTweenEnded());
			}
		}

		private void RemoveHideListeners()
		{
			if (_hideMotion != null)
			{
				_hideMotion.OnPlayMotion.RemoveListener(() => HandleHideTweenBegan());
				_hideMotion.OnFinishedMotion.RemoveListener(() => HandleHideTweenEnded());
			}
		}

		//----------------------------------------------------------------------------------------------------------------------------------//
		// Clean Up
		//----------------------------------------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Motions require manual removal from listeners
		/// </summary>
		protected virtual void OnDestroy()
		{
			StopLastTransition();
			RemoveDisplayListeners();
			RemoveHideListeners();
		}

		protected void StopLastTransition()
		{
			if (_lastActiveMotion != null)
			{
				_lastActiveMotion.Kill();
				_lastActiveMotion = null;
			}

			if (_lastActiveTransition != null)
			{
				_lastActiveTransition.Stop();
				_lastActiveTransition = null;
			}
		}

		//----------------------------------------------------------------------------------------------------------------------------------//
		// Display
		//----------------------------------------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Displays the view
		/// </summary>
		/// <param name="displayIndex">Most views only have one, but if you want to specify a specific animation, pass in the index.</param>
		/// <param name="delay">In seconds, how long do you want to wait before starting the animation</param>
		/// <param name="immediately">Do we to skip the animation and immediately display it?</param>
		/// <returns>The duration of the display animation</returns>
		public virtual float Display(int displayIndex = 0, float delay = 0, bool immediately = false)
		{
			if (_logDebugs)
				Debug.Log(name + ": Display, immediately? " + immediately);

			AttemptInitView();
			StopLastTransition();

			if (_state == ViewState.Displayed)
			{
				if (_logDebugs)
					Debug.Log(name + ": View is already displayed, do nothing");

				//Do nothing
				return 0;
			}
			else
			{
				DisableInteraction();
				State = ViewState.Displaying;

				if (_displayMotion != null)
				{
					if (_logDebugs)
						Debug.Log(name + ": Triggering Display Motion");

					//TODO if immediately
					_lastActiveMotion = _displayMotion;
				    _lastActiveMotion.Play(true);
					return _lastActiveMotion.Duration;
				}
				else
				{
					if (immediately || _displayTransitions.Length == 0)
					{
						if (_logDebugs)
							Debug.Log(name + ": Displaying immeditately");

						if (_displayTransitions.Length > 0)
						{
							_displayTransitions[0].SetProgress(DISPLAYED_PROGRESS,true);
						}

						BeginDisplay();
						EndDisplay();

						return 0;
					}
					else
					{
						if (_logDebugs)
							Debug.Log(name + ": Triggering display transition");

						if (displayIndex >= _displayTransitions.Length)
							displayIndex = 0;

						_lastActiveTransition = _displayTransitions[displayIndex];
						_lastActiveTransition.SetProgress(HIDDEN_PROGRESS);
						_lastActiveTransition.Play(true, delay, HandleDisplayTweenBegan, HandleDisplayTweenEnded);

						return _lastActiveTransition.Duration+delay;
					}
				}
			}
		}

		/// <summary>
		/// Handles when the display sequence has began.
		/// </summary>
		protected virtual void HandleDisplayTweenBegan()
		{
			BeginDisplay();
		}

		/// <summary>
		/// Handles when the display sequence has ended.
		/// </summary>
		protected virtual void HandleDisplayTweenEnded()
		{
			EndDisplay();
		}

		/// <summary>
		/// Display has begun, trigger related events and properties.
		/// </summary>
		protected virtual void BeginDisplay()
		{
			DisableInteraction();

			if (_logDebugs)
				Debug.Log(name + ": Display began");

			if (_disableGameobjectOnHide)
				gameObject.SetActive(true);

			if (_canvasToDisableOnHide != null)
			{
				_canvasToDisableOnHide.enabled = true;
				_graphicRaycaster.enabled = true;
			}

			State = ViewState.Displaying;
		}

		/// <summary>
		/// The display has finished.
		/// </summary>
		protected virtual void EndDisplay()
		{
			if (_logDebugs)
				Debug.Log(name + ": Display Ended");

			State = ViewState.Displayed;
			EnableInteraction();
		}


		//----------------------------------------------------------------------------------------------------------------------------------//
		// Variables
		//----------------------------------------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Hides the view
		/// </summary>
		/// <param name="hideIndex">Most views only have one, but if you want to specify a specific animation, pass in the index.</param>
		/// <param name="delay">In seconds, how long do you want to wait before starting the animation</param>
		/// <param name="immediately">Do we to skip the animation and immediately display it?</param>
		/// <returns>The duration of the hide animation</returns>
		public virtual float Hide(int hideIndex = 0, float delay = 0, bool immediately = false)
		{
			if (_logDebugs)
				Debug.Log(name + ": Hide, immediately? " + immediately);

			AttemptInitView();
			StopLastTransition();

			if (_state == ViewState.Hidden)
			{
				if (_logDebugs)
					Debug.Log(name + ": View is already hidden, do nothing.");

				//Do nothing
				return 0;
			}
			else
			{
				DisableInteraction();
				State = ViewState.Hiding;

				if (_hideMotion != null)
				{
					if (_logDebugs)
						Debug.Log(name + ": Triggering Hide Motion");

					//TODO if immediately
					_lastActiveMotion = _hideMotion;
					_lastActiveMotion.Play();
					return _lastActiveMotion.Duration;
				}
				else
				{
					if (immediately || _hideTransitions.Length == 0)
					{
						if (_logDebugs)
							Debug.Log(name + ": Hiding immediately");

						if (_hideTransitions.Length > 0)
						{
							_hideTransitions[0].SetProgress(HIDDEN_PROGRESS,true);
						}

						BeginHide();
						EndHide();

						return 0;
					}
					else
					{
						if (_logDebugs)
							Debug.Log(name + ": Triggering hide transition");

						if (hideIndex >= _hideTransitions.Length)
							hideIndex = 0;

						_lastActiveTransition = _hideTransitions[hideIndex];
						_lastActiveTransition.SetProgress(DISPLAYED_PROGRESS);
						_lastActiveTransition.Play(false, delay, HandleHideTweenBegan, HandleHideTweenEnded);

						return _lastActiveTransition.Duration+delay;
					}
				}
			}
		}

		/// <summary>
		/// Handles when the hide sequence has began.
		/// </summary>
		protected virtual void HandleHideTweenBegan()
		{
			BeginHide();
		}


		/// <summary>
		/// Handles when the hide sequence has ended.
		/// </summary>
		protected virtual void HandleHideTweenEnded()
		{
			EndHide();
		}

		/// <summary>
		/// Hide has begun, trigger related events and properties.
		/// </summary>
		protected virtual void BeginHide()
		{
			if (_logDebugs)
				Debug.Log(name + ": Hide began");

			DisableInteraction();

			State = ViewState.Hiding;
		}


		/// <summary>
		/// The hide has finished.
		/// </summary>
		protected virtual void EndHide()
		{
			if (_logDebugs)
				Debug.Log(name + ": Triggering hide ended");

			State = ViewState.Hidden;

			if (_disableGameobjectOnHide)
				gameObject.SetActive(false);

			if (_canvasToDisableOnHide != null)
			{
				_canvasToDisableOnHide.enabled = false;
				_graphicRaycaster.enabled = false;
			}
		}

		//----------------------------------------------------------------------------------------------------------------------------------//
		// Interaction Status
		//----------------------------------------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Disables the interaction on this view.
		/// </summary>
		public void DisableInteraction()
		{
			_interactionEnabled = false;
			if (_interactionGroup)
				_interactionGroup.blocksRaycasts = false;
			DisableInteractionInternal();
		}

		/// <summary>
		/// Enables the interaction on this view.
		/// </summary>
		public void EnableInteraction()
		{
			_interactionEnabled = true;
			if (_interactionGroup)
				_interactionGroup.blocksRaycasts = true;
			EnableInteractionInternal();
		}

		protected virtual void EnableInteractionInternal() { }
		protected virtual void DisableInteractionInternal() { }
	}
}
