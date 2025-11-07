using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;

namespace U9.ProgressTransition.Editor
{
    public class GameObjectTransitionsGroup
    {
        private Scope _componentScope;
        private GameObjectTransitionsGroup[] _childGroups; //The gameobject/parent grouping
        private int _nestingLayer; //0 = same parent, +1 is a child, -1 is higher (if we show higher
        private ProgressTransitionNode[] _componentNodes;
        private Transform _transform;
        private bool _containsRoot;
        private int _nestCount;

        public GameObjectTransitionsGroup(Transform transform, SequenceProgressTransition rootTransition, int nestingLayer, GameObjectTransitionsGroup[] childGroups, int nestCount, List<BaseProgressTransition> connectedTransitions, bool connectedOnly)
        {
            _transform = transform;
            _childGroups = childGroups;
            _nestCount = nestCount;
            _nestingLayer = nestingLayer;

            var components = transform.GetComponents<BaseProgressTransition>();
            int noOfComponents = components.Length;


            List<ProgressTransitionNode> componentNodes = new List<ProgressTransitionNode>();

            _componentNodes = new ProgressTransitionNode[noOfComponents];
            _containsRoot = false;

            for (int i = 0; i < noOfComponents; i++)
            {
                var component = components[i];

                bool include = !connectedOnly;
                if(connectedOnly)
                {
                    foreach(var connection in connectedTransitions)
                    {
                        if(connection == component)
                        {
                            include = true;
                            break;
                        }
                    }
                }

                if (include)
                {
                    bool isRoot = component == rootTransition;
                    var node = new ProgressTransitionNode(component, isRoot);

                    if (isRoot)
                    {
                        _containsRoot = true;
                    }

                    componentNodes.Add(node);
                }
            }

            _componentNodes = componentNodes.ToArray();

            if (noOfComponents > 0)
            {
                _componentScope = CreateBorderElement();
                _componentScope.headerContainer.Add(CreateComponentHeader(_transform.name));
                _componentScope.AddElements(_componentNodes);
            }
        }

        private Scope CreateBorderElement()
        {
            var element = new Scope()
            {
                autoUpdateGeometry = true
            };

            return element;
        }

        private Label CreateComponentHeader(string text)
        {
            if (text.Length > 25)
                text = text.Substring(0, 25) + "...";

            return new Label
            {
                style =
                {
                    paddingTop = 3f,
                    //marginTop = 7,
                    paddingLeft = 25f,
                    paddingRight = 25,
                    fontSize = 12,
                    unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.MiddleCenter),
                    unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Bold)
                },
                text = text
            };
        }

        public GameObjectTransitionsGroup[] ChildGroups { get => _childGroups; }
        public int NestingLayer { get => _nestingLayer; set => _nestingLayer = value; }
        public ProgressTransitionNode[] ComponentNodes { get => _componentNodes;  }
        public Transform Transform { get => _transform; }
        public bool ContainsRoot { get => _containsRoot;  }
        public int NestCount { get => _nestCount;  }
        public Scope ComponentScope { get => _componentScope; }

        public bool HasComponentsInNest()
        {
            if (ComponentNodes.Length > 0)
                return true;

            if(ChildGroups.Length >0)
            {
                foreach(var child in ChildGroups)
                {
                    if (child.HasComponentsInNest())
                        return true;
                }
            }

            return false;
        }
    }


    public class ProgressTransitionNodeEditor : EditorWindow
    {
        private VisualElement _rootElement;
        ProgressTransitionGraphView _graphView;
        private SequenceProgressTransition _rootProgressNode;
        private GameObjectTransitionsGroup _topMostGroup;
        private List<GameObjectTransitionsGroup> _transitionGroups;
        private List<BaseProgressTransition> _connectedTransitions;
        private bool _showConnectedOnly = false;

        private const int MAX_PARENT_LAYER = 3; //We can check 3 layers above us
        private const int MAX_CHILD_LAYER_COUNT = 6; //We can check 6 layers lower

        private const float GRID_LARGE_LINE_SPACING = 100f;
        private const float GRID_SMALL_LINE_SPACING = 25;

        private const float COLUMN_WIDTH = 300f;
        private const float BOX_PADDING = 2;
        private const float COLUMN_OFFSET = 120f;
        private const float ROW_OFFSET = 100f;

        private const float COMPONENT_HEIGHT = 300f;

        /// <summary>
        /// Open Node Editor window
        /// </summary> 
        public static void OpenWindow(SequenceProgressTransition rootNode, bool connectedOnly)
        {
            ProgressTransitionNodeEditor window = (ProgressTransitionNodeEditor)GetWindow(typeof(ProgressTransitionNodeEditor));

            window.Init(rootNode, connectedOnly);
        }      

        public void Init(SequenceProgressTransition rootNode, bool connectedOnly)
        {
            _rootProgressNode = rootNode;
            _showConnectedOnly = connectedOnly;

            _transitionGroups = new List<GameObjectTransitionsGroup>();

            //First we find the maximum parent that isn't null
            int layer = 0;
            Transform transform = rootNode.transform;
            while(true)
            {
                if (transform.GetComponent<U9.View.View>() != null)
                {
                    //If our current transform has a view, we should not go higher, as we will possible get non relevant transitions.
                    break;
                }
                else
                {
                    if (layer < MAX_PARENT_LAYER && transform.parent != null)
                    {
                        transform = transform.parent;
                        layer++;
                    }
                    else
                        break;
                }
            }

            //Get the connected nodes
            _connectedTransitions = new List<BaseProgressTransition>();
            _connectedTransitions.Add(rootNode);
            AddConnectedTransitions(rootNode, ref _connectedTransitions);

            //Gather the groups
            _topMostGroup = GatherTransitions(transform, layer);

            int count = 0;
            foreach(var group in _transitionGroups)
            {
                if(group.ComponentNodes.Length >0)
                    count++;
            }

            DrawGraphView();
        }

        private void AddConnectedTransitions(BaseProgressTransition transition, ref List<BaseProgressTransition> connectedTransitions)
        {
            if(transition.GetType() == typeof(SequenceProgressTransition))
            {
                SequenceProgressTransition sequence = (SequenceProgressTransition)transition;

                foreach(var connection in sequence.TransitionsToSequence)
                {
                    if(!connectedTransitions.Contains(connection))
                    {
                        connectedTransitions.Add(connection);
                        AddConnectedTransitions(connection, ref connectedTransitions);
                    }
                }
            }
        }

        private void DrawGraphView()
        {
            _graphView = new ProgressTransitionGraphView();
            _graphView.StretchToParentSize();

            rootVisualElement.Add(_graphView);

            _graphView.AddNodesFromGroup(_topMostGroup);
        }

        private void Update()
        {
            if (_graphView != null)
            {
                _graphView.Reposition();
            }
            else
                Close();
        }

        private GameObjectTransitionsGroup GatherTransitions(Transform transform, int layer)
        {
            List<GameObjectTransitionsGroup> childGroups = new List<GameObjectTransitionsGroup>();
                        
            int nestCount = 0;

            //Check children if our layer allows it
            int childLayer = layer - 1;
            if (childLayer >= -MAX_CHILD_LAYER_COUNT)
            {
                foreach (Transform child in transform)
                {
                    //We don't want to gather transitons under a nested sub view.
                    if (child.GetComponent<U9.View.View>() == null)
                    {
                        var childGrouping = GatherTransitions(child, childLayer);

                        //Does this child have more groups beneath it?
                        if (childGrouping.NestCount > nestCount)
                            nestCount = childGrouping.NestCount;

                        childGroups.Add(childGrouping);
                    }
                }
            }

            if (childGroups.Count > 0)
            {
                //If we had any children, increment
                nestCount++;
            }

            var group = new GameObjectTransitionsGroup(transform, _rootProgressNode, layer, childGroups.ToArray(), nestCount, _connectedTransitions,_showConnectedOnly);

            //Return the grouping
            _transitionGroups.Add(group);
            return group;
        }
    }
}
