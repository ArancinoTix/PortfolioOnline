using TMPro;
using UnityEngine;

namespace U9.View
{
    public class SubViewManager
    {
        private TextMeshProUGUI _pageLabel;
        private View[] _subViews;
        private int _currentSubPanelIndex = -1;
        private bool _canLoop = false;
        private float _nextViewDelay;

        private int _nextTweenIndexToUse = 0;
        private int _previousTweenIndexToUse = 1;

        public SubViewManager(TextMeshProUGUI pageLabel, View[] views, bool canLoop, float nextViewDelay = 0, int nextTweenIndex = 0, int previousTweenIndex = 1)
        {
            _pageLabel = pageLabel;
            _subViews = views;
            _canLoop = canLoop;
            _currentSubPanelIndex = 0;
            _nextViewDelay = nextViewDelay;
            _nextTweenIndexToUse = nextTweenIndex;
            _previousTweenIndexToUse = previousTweenIndex;

            Reset();
        }

        public void Reset()
        {
            int numberOfSubViews = _subViews.Length;
            //If we have no views, do nothing
            if (numberOfSubViews < 0)
                return;

            _currentSubPanelIndex = 0;

            if(_subViews.Length >0)
                _subViews[0].Display(0,0,true);

            for (int i = 1; i < numberOfSubViews; i++)
            {
                _subViews[i].Hide(0,0,true);
            }

            UpdatePageLabel();
        }

        private void UpdatePageLabel()
        {
            if (_pageLabel == null)
                return;

            int numberOfSubViews = _subViews.Length;
            //If we have no views, do nothing
            if (numberOfSubViews < 0)
            {
                _pageLabel.text = "0/0";
            }
            else
            {
                _pageLabel.text = (_currentSubPanelIndex + 1) + "/" + numberOfSubViews;
            }
        }

        public bool SwitchSubPanel(bool next)
        {
            int numberOfSubViews = _subViews.Length;

            //If we have no views, do nothing
            if (numberOfSubViews < 0)
                return false;

            //Increment/decrement as needed
            int newPanelIndex = next ? _currentSubPanelIndex + 1 : _currentSubPanelIndex - 1;

            //Did we increment or decrement out of range of the available views?
            bool hasExceededRange = false;
            if (newPanelIndex < 0)
            {
                //If we went below, set the index to the last one.
                hasExceededRange = true;
                newPanelIndex = numberOfSubViews - 1;
            }
            else if (newPanelIndex >= numberOfSubViews)
            {
                //If we went above, set the index to the first one.
                hasExceededRange = true;
                newPanelIndex = 0;
            }

            //If we haven't gone out of range, or if we are allowed to loop
            if (!hasExceededRange || _canLoop)
            {
                return SwitchTo(newPanelIndex, next);
            }
            else
                return false;
        }

        //Displays the specified index
        public bool SwitchTo(int viewIndex)
        {
            if (viewIndex < 0 || viewIndex >= _subViews.Length)
                return false;

            //Get the number of views before and after
            int noOfViews = _subViews.Length;
            int noOfViewBefore =
                   Mathf.FloorToInt(((float)noOfViews - 1f) * .5f);
            int noOfViewAfter = noOfViews - 1 - noOfViewBefore;

            if (noOfViewBefore < 0)
                noOfViewBefore = 0;
            if (noOfViewAfter < 0)
                noOfViewAfter = 0;

           
            //Calculate the local index
            int localIndex = viewIndex - _currentSubPanelIndex;

            if (_canLoop)
            {
                //7 => 0 = 0-7 =-7 (+8) = 1 (next)
                //0 => 7 = 7-0 =7 (-8) = -1 (previous)
                //0 => 3 = 3 (next)

                //If we are more than we expect to be before us. loop
                if (localIndex < -noOfViewBefore)
                    localIndex += noOfViews;
                else if (localIndex > noOfViewAfter)
                    localIndex -= noOfViews;
            }
          
            //If not equal, use the shortest direction
            bool next = localIndex >0;
            return SwitchTo(viewIndex, next);            
        }

        //Switch to a specific view
        public bool SwitchTo(View view)
        {
            for(int i =0, ni = _subViews.Length; i<ni; i++)
            {
                if(view == _subViews[i])
                {
                    return SwitchTo(i);
                }
            }
            //This occurs if we do not have the view
            return false;
        }

        private bool SwitchTo(int value, bool next)
        {
            //This is a safety check to only animate if the next view is not animating already.
            //Without this, you could spam the command and cause no view to be displayed.
            if (_subViews[value].State != View.ViewState.Hidden)
                return false;

            //Views can have multiple ways to show and hide.
            //For example it could slide in from the right, and then slide out to the left.
            //So we need to switch the index used based on the direction we are changing panels.
            int hideTweenIndex = next ? _previousTweenIndexToUse : _nextTweenIndexToUse;
            int displayTweenIndex = next ? _nextTweenIndexToUse : _previousTweenIndexToUse;

            //If a view is displayed, hide it
            if (_currentSubPanelIndex != -1)
                _subViews[_currentSubPanelIndex].Hide(hideTweenIndex);

            //Apply the new index
            _currentSubPanelIndex = value;

            _subViews[_currentSubPanelIndex].Display(displayTweenIndex,_nextViewDelay);

            UpdatePageLabel();
            return true;
        }
    }
}