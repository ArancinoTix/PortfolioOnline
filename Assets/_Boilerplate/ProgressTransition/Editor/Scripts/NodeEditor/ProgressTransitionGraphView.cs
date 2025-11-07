using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace U9.ProgressTransition.Editor
{
    public class ProgressTransitionGraphView : GraphView
    {
        private List<ProgressTransitionNode> _nodes;
        private bool _needsRepositioning = true;
        private GameObjectTransitionsGroup _topMostGroup;

        private const float GROUP_COLUMN_OFFSET = 500f;
        private const float GROUP_ROW_OFFSET = 290f;
        private const float COMPONENT_ROW_OFFSET = 220f;

        public ProgressTransitionGraphView()
        {
            AddManipulators();
            AddGridBackground();

            AddStyles();
            AddMiniMap();

            _nodes = new List<ProgressTransitionNode>();

        }

        public void Reposition()
        {
            if (_nodes.Count == 0)
                return;
            if (_needsRepositioning)
            {
                Reposition(_topMostGroup, Vector2.zero, out Rect bounds);
            }
            
            _needsRepositioning = false;
        }

        private void Reposition(GameObjectTransitionsGroup group, Vector2 origin, out Rect nodeBounds)
        {
            var childOrigin = origin;
            var collectiveChildBounds = new Rect(origin, Vector2.zero);

            //Process children first
            for (int i = 0, ni = group.ChildGroups.Length; i<ni; i++)
            {
                if (i > 0)
                {
                    childOrigin.y += GROUP_ROW_OFFSET;
                    collectiveChildBounds.height += GROUP_ROW_OFFSET;
                }

                GameObjectTransitionsGroup child = group.ChildGroups[i];
                Reposition(child, childOrigin, out Rect childBounds);

                //Offset and resize bounds
                childOrigin.y += childBounds.height;
                collectiveChildBounds.height += childBounds.height;

                //Resize width
                if (collectiveChildBounds.width < childBounds.width)
                    collectiveChildBounds.width = childBounds.width;
            }

            //Move the origin by the width of the children and offset by the column
            origin.x = -group.NestingLayer *GROUP_COLUMN_OFFSET;
            nodeBounds = new Rect(origin, Vector2.zero);
            Vector2 localOffset = origin;

            for (int i = 0, ni = group.ComponentNodes.Length; i <ni; i++)
            {
                ProgressTransitionNode node = group.ComponentNodes[i];

                //Offset boy the row offset if we aren't the first one
                if (i > 0)
                {
                    localOffset.y += COMPONENT_ROW_OFFSET;
                    nodeBounds.height += COMPONENT_ROW_OFFSET;
                }

                //Get the current RECT properties and offset it.
                var currentPosition = node.worldBound;
                currentPosition.position += localOffset;
                node.SetPosition(currentPosition);

                //Update our offset var by the size of the rect
                localOffset.y += currentPosition.height;

                //Update our bounds to envelope it.
                nodeBounds.height += currentPosition.height;
                if (nodeBounds.width < currentPosition.width)
                    nodeBounds.width = currentPosition.width;
            }

            //If we had children, we need to expand our bounds to encompass them
            if(group.ChildGroups.Length >0)
            {
                if (collectiveChildBounds.height > nodeBounds.height)
                    nodeBounds.height = collectiveChildBounds.height;

                nodeBounds.width += GROUP_COLUMN_OFFSET + nodeBounds.width;
            }            
        }

        public void AddNodesFromGroup(GameObjectTransitionsGroup group)
        {
            _topMostGroup = group;
            AddNodes(group);
            ConnectNodes();
        }

        private void AddNodes(GameObjectTransitionsGroup group)
        {
            if(group.ComponentNodes.Length >0)
                AddElement(group.ComponentScope);

            foreach (var componentNode in group.ComponentNodes)
            {
                _nodes.Add(componentNode);
                AddElement(componentNode);
                componentNode.Draw(this);
            }

            foreach (var child in group.ChildGroups)
            {
                AddNodes(child);
            }

            _needsRepositioning = true;
        }

        private void ConnectNodes()
        {
            foreach(var node in _nodes)
            {
                if(node.IsSequence)
                {
                    node.ConnectNodes(_nodes);
                }
            }
        }

        private void AddManipulators()
        {
            
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            var dragger = new ContentDragger();
            dragger.activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
            this.AddManipulator(dragger);
            this.AddManipulator(new SelectionDragger());
        }

        private void AddStyles()
        {
            StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load("ProgressTransition/ProgressTransitionStyles.uss");
            styleSheets.Add(styleSheet);
        }

        private void AddGridBackground()
        {
            GridBackground gridBackground = new GridBackground();
            gridBackground.StretchToParentSize();

            Insert(0, gridBackground);
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();

            ports.ForEach(port =>
            {
                //Ignore the same node type / direction
                if(startPort == port || startPort.node == port.node || startPort.direction == port.direction)
                {
                    return;
                }

                //Also do not allow ports that would cause a loop, I.e Start Sequence inside Port sequence or one of it's children
                var originNode = (ProgressTransitionNode)(startPort.direction == Direction.Output ? startPort : port).node;
                var targetNode = (ProgressTransitionNode)(startPort.direction == Direction.Output ? port : startPort).node;

                //If we are connecting a sequence to a sequence, we need to validate if it would form a loop
                if (targetNode.IsSequence)
                {
                    var originTransition = (SequenceProgressTransition)originNode.TransitionComponent;
                    var targetTransition = (SequenceProgressTransition)targetNode.TransitionComponent;

                    //If it would form a loop, do not accept
                    if (WouldFormInfiniteLoop(originTransition, targetTransition))
                        return;
                }
                
                compatiblePorts.Add(port);
            });

            return compatiblePorts;
        }

        private bool WouldFormInfiniteLoop(SequenceProgressTransition origin, SequenceProgressTransition target)
        {
            //If either are null, then the cast failed. Thesefore it is impossible to form a loop
            if (origin == null || target == null)
                return false;

            //Cannot connect to self
            if (origin == target)
                return true;

            var sequenceType = typeof(SequenceProgressTransition);

            foreach (var subTarget in target.TransitionsToSequence)
            {
                if (subTarget != null)
                {
                    //This sequence is directly refering the origin
                    if (subTarget == origin)
                        return true;

                    //Would any nested form a loop?
                    if (subTarget.GetType() == sequenceType && WouldFormInfiniteLoop(origin, (SequenceProgressTransition)subTarget))
                        return true;
                }
            }

            //If we reach here, no loop was detected
            return false;
        }

        private void AddMiniMap()
        {
            var miniMap = new MiniMap()
            {
                anchored = true
            };

            miniMap.SetPosition(new Rect(10, 10, 200, 200));
            Add(miniMap);
        }
    }
}
