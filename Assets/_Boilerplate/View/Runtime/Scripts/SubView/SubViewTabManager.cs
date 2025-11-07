using System;
using System.Collections;
using U9.Ease;
using U9.Utils;
using UnityEngine;
using UnityEngine.Localization;

namespace U9.View
{
    [System.Serializable]
    public class SubViewTabManager
    {
        private ButtonWithLabel[] _tabs;
        private int[] _tabVisualIndexes;
        private Coroutine[] _tabPositionCoroutines;
        private bool _canLoop = false;
        private IBaseEase _easeFormula;
        private float _transitionDuration;
        private RectTransform _tabParent;
        private Vector2 _tabSpacing;
        private int _numberOfTabs;
        private int _currentTabIndex = -1;
        private int _tabCountBeforeCurrent = 0;
        private int _tabCountAfterCurrent = 0;

        public Action<int> onTabClicked;



        /// <summary>
        /// Initialises the base properities
        /// </summary>
        /// <param name="numberOfTabs"></param>
        /// <param name="tabButtonPrefab"></param>
        /// <param name="tabParent"></param>
        /// <param name="canLoop"></param>
        /// <param name="tabSpacing"></param>
        /// <param name="transitionDuration"></param>
        /// <param name="transitionEaseType"></param>
        private SubViewTabManager(int numberOfTabs,ButtonWithLabel tabButtonPrefab, RectTransform tabParent, bool canLoop,
            Vector2 tabSpacing, float transitionDuration = .35f, EaseType transitionEaseType = EaseType.InOutSine)
        {
            _numberOfTabs = numberOfTabs;
            _tabs = new ButtonWithLabel[numberOfTabs];
            _tabVisualIndexes = new int[numberOfTabs];
            _tabPositionCoroutines = new Coroutine[numberOfTabs];

            _easeFormula = EaseFormula.GetEaseFunction(transitionEaseType);
            _transitionDuration = transitionDuration;
            _canLoop = canLoop;
            _tabParent = tabParent;
            _tabSpacing = tabSpacing;
            _currentTabIndex = 0;

            if (!canLoop)
            {
                _tabCountBeforeCurrent = 0;
                _tabCountAfterCurrent = _numberOfTabs - 1;
            }
            else
            {
                _tabCountBeforeCurrent = 
                    Mathf.FloorToInt(((float)_numberOfTabs-1f) * .5f); //If we have 7 buttons, we need 3 to the left of the active tab.
                _tabCountAfterCurrent = _numberOfTabs - 1 - _tabCountBeforeCurrent; //If we have 7 buttons, then we have 6 others to positon. 3 to the left and 3 to the right

                if (_tabCountBeforeCurrent < 0)
                    _tabCountBeforeCurrent = 0;
                if (_tabCountAfterCurrent < 0)
                    _tabCountAfterCurrent = 0;
            }
        }
     
        /// <summary>
        /// Create a tab manager based on a list of localised labels
        /// </summary>
        /// <param name="labels">Labels to assign to the tabs</param>
        /// <param name="tabButtonPrefab">The prefab to use for each button</param>
        /// <param name="tabParent">The parent to contain the buttons. This will be used for transition animations</param>
        /// <param name="canLoop">Can we loop/cycle the tabs</param>
        /// <param name="tabSpacing">How much spacing between each tab</param>
        /// <param name="transitionDuration">When we transition, how long should it take?</param>
        /// <param name="transitionEaseType">When we transition, what easing should we use</param>
        public SubViewTabManager(LocalizedString[] labels,
          ButtonWithLabel tabButtonPrefab, RectTransform tabParent, bool canLoop,
          Vector2 tabSpacing, float transitionDuration = .35f, EaseType transitionEaseType = EaseType.InOutSine)
          : this(labels.Length, tabButtonPrefab, tabParent, canLoop, tabSpacing, transitionDuration, transitionEaseType)
        {
            for (int i = 0; i < _numberOfTabs; i++)
            {
                _tabs[i] = CreateTab(tabButtonPrefab, labels[i], i);
            }
        }

        /// <summary>
        /// Create a tab manager based on a list of sub views
        /// </summary>
        /// <param name="views">Views to extract labels from</param>
        /// <param name="tabButtonPrefab">The prefab to use for each button</param>
        /// <param name="tabParent">The parent to contain the buttons. This will be used for transition animations</param>
        /// <param name="canLoop">Can we loop/cycle the tabs</param>
        /// <param name="tabSpacing">How much spacing between each tab</param>
        /// <param name="transitionDuration">When we transition, how long should it take?</param>
        /// <param name="transitionEaseType">When we transition, what easing should we use</param>
        public SubViewTabManager(SubView[] views, 
            ButtonWithLabel tabButtonPrefab, RectTransform tabParent, bool canLoop,
            Vector2 tabSpacing, float transitionDuration =.35f, EaseType transitionEaseType = EaseType.InOutSine)
            : this (views.Length, tabButtonPrefab,tabParent,canLoop,tabSpacing,transitionDuration, transitionEaseType)
        {           
            for(int i =0; i< _numberOfTabs; i++)
            {
                _tabs[i] = CreateTab(tabButtonPrefab, views[i].LocalisedDisplayName, i);
            }
        }

        /// <summary>
        /// Creates the tab instance with the defined label reference and positions it based on the index
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="labelValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private ButtonWithLabel CreateTab(ButtonWithLabel prefab, LocalizedString labelValue, int index)
        {
            //Create it
            var instance = GameObject.Instantiate<ButtonWithLabel>(prefab, _tabParent);
            var instanceTransform = GetTransformFor(instance);

            //Set the position of the tab
            GetTabPositionForIndex(index, out Vector2 position, out int visualIndex);
            instanceTransform.anchoredPosition = position;

            //We assign the starting visual index
            _tabVisualIndexes[index] = visualIndex;

            //Assign it's localised label references
            instance.Label.StringReference = labelValue;

            //When clicked, this should send it's index value to who ever listens.
            //Clicking a tab will not trigger a transition directly.
            instance.onClick.AddListener(() => { onTabClicked?.Invoke(index); });

            //The first (active) tab should not be interactable
            instance.interactable = index != 0;

            return instance;
        }

        /// <summary>
        /// Sets the active tab to the given index
        /// </summary>
        /// <param name="tabIndex">Which tab should be set active?</param>
        /// <param name="instantly">Should we animate?</param>
        public void SetActiveTabTo(int tabIndex, bool instantly = false)
        {
            if (tabIndex != _currentTabIndex)
            {
                int offset = tabIndex - _currentTabIndex;
                if (_canLoop)
                {
                    //7 => 0 = 0-7 =-7 (+8) = 1 (next)
                    //0 => 7 = 7-0 =7 (-8) = -1 (previous)
                    //0 => 3 = 3 (next)

                    //If we are more than we expect to be before us. loop
                    if (offset < -_tabCountBeforeCurrent)
                        offset += _numberOfTabs;
                    else if (offset > _tabCountAfterCurrent)
                        offset -= _numberOfTabs;
                }

                _currentTabIndex = tabIndex;

                for (int i = 0; i < _numberOfTabs; i++)
                {
                    UpdateTabPosition(i, offset, instantly);
                }
            }
        }

        private void UpdateTabPosition(int tabIndex, int offset, bool instantly)
        {
            var lastVisualIndex = _tabVisualIndexes[tabIndex];
            GetNewTabPositionForIndex(lastVisualIndex,offset, out Vector2 targetPosition, out int newVisualIndex);

            _tabVisualIndexes[tabIndex] = newVisualIndex;

            var tab = _tabs[tabIndex];
            tab.interactable = tabIndex != _currentTabIndex;

            var lastRoutine = _tabPositionCoroutines[tabIndex];
            if(lastRoutine != null)
            {
                CoroutineHandler.GetInstance(false)?.StopCoroutine(lastRoutine);
            }

            if (instantly)
            {
                GetTransformFor(tab).anchoredPosition = targetPosition;
                return;
            }
            
            if(_canLoop 
                && 
                (
                    (lastVisualIndex <0 && newVisualIndex >0)
                    ||
                    (lastVisualIndex>0 && newVisualIndex<0)
                ))
            {
                //Did the index cycle to the other side?
                //If so, we need it to instantly move over
                var jumpedVisualIndex = newVisualIndex > 0 ? newVisualIndex + 1 : newVisualIndex - 1;

                GetTransformFor(tab).anchoredPosition = _tabSpacing * jumpedVisualIndex;
            }

            //Otherwise animate it
            _tabPositionCoroutines[tabIndex] = 
                CoroutineHandler.GetInstance(true).StartCoroutine(
                    TabPositionRoutine(GetTransformFor(tab), targetPosition)
                    );
            
        }

        IEnumerator TabPositionRoutine(RectTransform transform, Vector2 toPosition)
        {
            Vector2 fromPosition = transform.anchoredPosition;

            float l = 0;
            float speed = 1 / _transitionDuration;

            while (l<1)
            {
                l += Time.unscaledDeltaTime * speed;

                var easedL = _easeFormula.Calculate(l);
                transform.anchoredPosition = Vector2.Lerp(fromPosition, toPosition, easedL);
                yield return null;
            }

            transform.anchoredPosition = toPosition;
        }

        private RectTransform GetTransformFor(ButtonWithLabel button)
        {
            return (RectTransform)button.transform;
        }

        /// <summary>
        /// Gets the position of a tab with the defined index. This takes into account the active tab index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private void GetTabPositionForIndex(int index, out Vector2 position, out int visualIndex)
        {
            if(!_canLoop)
            {
                //If we can't loop, the tabs are lined up one after the other
                position = _tabSpacing * index;
                visualIndex = index;
            }
            else
            {
                //If we can loop, we need to cycle the tabs so that they appear left and right of the currentIndex

                var positionalIndex = index - _currentTabIndex; //If Current is 6 and we the tab to position 1, this is -5

                //If we the max to the left is 3, and we are -5, we need to cycle the button to the right
                if(positionalIndex<-_tabCountBeforeCurrent)
                {
                    positionalIndex += _numberOfTabs;
                }
                //Or if the max to the right is 3, and we are higher than this, cycle us to the left
                else if(positionalIndex > _tabCountAfterCurrent)
                {
                    positionalIndex -= _numberOfTabs;
                }

                position = _tabSpacing * positionalIndex;
                visualIndex = positionalIndex;
            }
        }

        /// <summary>
        /// Gets the position of a tab with the defined index. This takes into account the active tab index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private void GetNewTabPositionForIndex(int currentVisualIndex, int offset, out Vector2 position, out int newVisualIndex)
        { 
            //We were at 0, but move to 5 (offset 5). So we are now -5
            //Or we were -5, but move to 0, we are now 0
            newVisualIndex = currentVisualIndex - offset;

            if (_canLoop)           
            {
                //If we can loop, we need to cycle the tabs so that they appear left and right of the currentIndex
                int maxOffset = Mathf.Max(_tabCountBeforeCurrent, _tabCountAfterCurrent);

                //If we the max to the left is 3, and we are -5, we need to cycle the button to the right
                if (newVisualIndex < -maxOffset)
                {
                    newVisualIndex += _numberOfTabs;
                }
                //Or if the max to the right is 3, and we are higher than this, cycle us to the left
                else if (newVisualIndex > maxOffset)
                {
                    newVisualIndex -= _numberOfTabs;
                }
            }

            //If we can't loop, the tabs are lined up one after the other
            position = _tabSpacing * newVisualIndex;
        }
    }
}
