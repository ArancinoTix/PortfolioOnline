using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace U9.ProgressTransition.Editor
{
    public class ProgressTransitionNode : Node
    {
        public ProgressTransitionNode(BaseProgressTransition transitionComponent, bool isRoot)
        {
            IsSequence = transitionComponent.GetType() == typeof(SequenceProgressTransition);
            TransitionComponent = transitionComponent;
            IsRoot = isRoot;

            this.title = transitionComponent.GetType().Name;
            this.style.width = 300;
            this.SerializedObject = new SerializedObject(transitionComponent);

            if (IsRoot)
            {
                this.style.backgroundColor = new Color(0.15f,.15f,.25f);
            }
        }

        public Port InputPort { get; private set; }
        public bool IsRoot { get; }
        public bool IsSequence { get;  }
        public BaseProgressTransition TransitionComponent { get;  }
        public SerializedObject SerializedObject { get; }

        private SerializedProperty _connectionsProperty;
        private List<ConnectionRowElement> _connectionRows = new List<ConnectionRowElement>();
        private GraphView _graphView;

        public void Draw(GraphView graphView)
        {
            _graphView = graphView;
            InputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
            InputPort.portName = "";

            inputContainer.Add(InputPort);

            var customDataContainer = new VisualElement();
            
            customDataContainer.Add(CreateFieldHeader("Description"));
            var descField = new PropertyField();
            descField.BindProperty(SerializedObject.FindProperty("_description"));
            descField.label = "";
            customDataContainer.Add(descField);

            if(IsSequence)
            {
                customDataContainer.Add(CreateFieldHeader("Sequence Type"));
                var sequenceField = new PropertyField();
                sequenceField.BindProperty(SerializedObject.FindProperty("_sequenceType"));
                sequenceField.label = "";
                customDataContainer.Add(sequenceField);

                customDataContainer.Add(CreateFieldHeader("Stagger Amount"));
                var staggerField = new PropertyField();
                staggerField.BindProperty(SerializedObject.FindProperty("_staggerAmount"));
                staggerField.label = "";
                customDataContainer.Add(staggerField);
            }

            customDataContainer.Add(CreateFieldHeader("Progress"));
            var progressField = new PropertyField();
            progressField.BindProperty(SerializedObject.FindProperty("_progress"));
            progressField.label = "";
            customDataContainer.Add(progressField);

            customDataContainer.Add(CreateFieldHeader("Duration"));
            var durationField = new PropertyField();
            durationField.BindProperty(SerializedObject.FindProperty("_duration"));
            durationField.label = "";
            durationField.SetEnabled(!IsSequence);
            customDataContainer.Add(durationField);

            customDataContainer.Add(CreateFieldHeader("Ease Type"));
            var easeField = new PropertyField();
            easeField.BindProperty(SerializedObject.FindProperty("_easeType"));
            easeField.label = "";
            customDataContainer.Add(easeField);

            extensionContainer.Add(customDataContainer);

            if(IsSequence)
            {
                _connectionsProperty = SerializedObject.FindProperty("_transitionsToSequence");
                int numberOfExistingConnections = _connectionsProperty.arraySize;

                for(int i = 0; i<numberOfExistingConnections; i++)
                {
                    var connectionProperty = _connectionsProperty.GetArrayElementAtIndex(i);

                    DrawConnectionField(connectionProperty);
                }

                //Add new entry
                Button addButton = null;
                addButton = new Button(() => 
                {
                    _connectionsProperty.InsertArrayElementAtIndex(_connectionsProperty.arraySize);
                    var newConnection = _connectionsProperty.GetArrayElementAtIndex(_connectionsProperty.arraySize - 1);
                    newConnection.objectReferenceValue = null;

                    //Draw it
                    DrawConnectionField(newConnection);

                    //Re add the button to move it to the end.
                    outputContainer.Add(addButton);

                    //Apply
                    SerializedObject.ApplyModifiedProperties();
                })
                {
                    style = {
                            fontSize = 18,
                            paddingTop = -2,
                            marginLeft = 90,
                            marginRight = 90
                        },
                    text = "+"
                };
                outputContainer.Add(addButton);
            }
            else
            {
                //Do nothing, this cannot manipulate another
            }

            RefreshExpandedState();
        }

        private void DrawConnectionField(SerializedProperty connectionProperty)
        {
            //connection port. This needs to be a "callback" port as we nwws exposed callbacks
            CallbackPort connectionPort = CallbackPort.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));

            //Container
            ConnectionRowElement connectionRow = new ConnectionRowElement(connectionPort, connectionProperty,Remove,MoveUp,MoveDown, ConnectionUpdated);

            //Add
            _connectionRows.Add(connectionRow);
            outputContainer.Add(connectionRow);
        }

        private void Remove(ConnectionRowElement row)
        {
            for (int i = 0, ni = _connectionsProperty.arraySize; i < ni; i++)
            {
                if (SerializedProperty.EqualContents(_connectionsProperty.GetArrayElementAtIndex(i), row.Property))
                {
                    //Delete it
                    _connectionsProperty.DeleteArrayElementAtIndex(i);
                    SerializedObject.ApplyModifiedProperties();

                    var rowToRemove = _connectionRows[i];
                    rowToRemove.RemoveEvents(); //Need to remove events to prevent nulls

                    //Remove stray edges
                    if (rowToRemove.Port.connected)
                    {
                        _graphView.DeleteElements(rowToRemove.Port.connections);

                        //Disconnect
                        rowToRemove.Port.DisconnectAll();
                    }

                    //Remove the specific row as we only want to update one connection
                    outputContainer.Remove(rowToRemove);
                    _connectionRows.RemoveAt(i);
                    break;
                }
            }

            //Update all bindings as the array as changed
            for (int i = 0, ni = _connectionsProperty.arraySize; i < ni; i++)
            {
                _connectionRows[i].Rebind(_connectionsProperty.GetArrayElementAtIndex(i));
            }
        }

        private void ConnectionUpdated(ConnectionRowElement rowt)
        {
            SerializedObject.ApplyModifiedProperties();
        }

        private void MoveUp(ConnectionRowElement row)
        {
            for (int i = 0, ni = _connectionsProperty.arraySize; i < ni; i++)
            {
                if (SerializedProperty.EqualContents(_connectionsProperty.GetArrayElementAtIndex(i), row.Property))
                {
                    //we can only move up if not the first
                    if (i > 0)
                    {
                        _connectionsProperty.MoveArrayElement(i, i-1);
                        SerializedObject.ApplyModifiedProperties();
                    }
                    break;
                }
            }
        }

        private void MoveDown(ConnectionRowElement row)
        {
            for (int i = 0, ni = _connectionsProperty.arraySize; i < ni; i++)
            {
                if (SerializedProperty.EqualContents(_connectionsProperty.GetArrayElementAtIndex(i), row.Property))
                {
                    //we can only move down if not the last one
                    if (i < ni-1)
                    {
                        _connectionsProperty.MoveArrayElement(i, i + 1);
                        SerializedObject.ApplyModifiedProperties();
                    }
                    break;
                }
            }
        }

        private Label CreateFieldHeader(string text)
        {
            if (text.Length > 25)
                text = text.Substring(0, 25) + "...";

            return new Label
            {
                style =
                {
                    paddingTop = 10f,
                    paddingLeft = 3f,
                    paddingRight = 3f,
                },
                text = text
            };
        }

        private string Truncate(string value, int limit)
        {
            if (value.Length > limit)
                return value.Substring(0, limit-3) + "...";
            else
                return value;
        }

        public void ConnectNodes(List<ProgressTransitionNode> allNodes)
        {
            //For each connection we have
            foreach(var row in _connectionRows)
            {
                //If we have a value set
                if(row.Property.objectReferenceValue != null)
                {
                    //Check all nodes
                    foreach(var node in allNodes)
                    {
                        //If node is the value we expect
                        if(node != this && node.TransitionComponent == row.Property.objectReferenceValue)
                        {
                            //connect it
                            var edge = row.Port.ConnectTo(node.InputPort);
                            _graphView.Add(edge);
                            break;
                        }
                    }
                }
            }
        }
    }

    public class ConnectionRowElement : VisualElement
    {
        private SerializedProperty _property;
        private CallbackPort _port;

        private Action<ConnectionRowElement> _onRemove;
        private Action<ConnectionRowElement> _onMoveUp;
        private Action<ConnectionRowElement> _onMoveDown;
        private Action<ConnectionRowElement> _onConnectionUpdated;

        public SerializedProperty Property { get => _property; }
        public CallbackPort Port { get => _port; }

        private PropertyField _connectionField;

        public ConnectionRowElement(CallbackPort connectionPort, SerializedProperty property, Action<ConnectionRowElement> onRemove, Action<ConnectionRowElement> onMoveUp, Action<ConnectionRowElement> onMoveDown, Action<ConnectionRowElement> onConnectionUpdated)
        {
            _property = property;
            _port = connectionPort;
            _onRemove = onRemove;
            _onMoveUp = onMoveUp;
            _onMoveDown = onMoveDown;
            _onConnectionUpdated = onConnectionUpdated;

            connectionPort.OnConnect += ConnectionUpdated;
            connectionPort.OnDisconnect += ConnectionUpdated;

            style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
            style.alignContent = new StyleEnum<Align>(Align.Stretch);

            //Remove this entry if clicked
            var removeButton = new Button(() =>
            {
                onRemove?.Invoke(this);
            })
            {
                style = {
                            width = 20
                        },
                text = "X"
            };

            //connection port
            connectionPort.portName = "";

            //Field representing the connecting component
            _connectionField = new PropertyField();
            _connectionField.BindProperty(property);
            _connectionField.label = "";
            _connectionField.style.width = 150;
            _connectionField.style.paddingTop = 2;

            //Move up or down
            var moveupButton = new Button(() => {
                onMoveUp?.Invoke(this);
            })
            {
                style = {
                            width = 20,
                            marginRight = 1,
                            fontSize = 8
                        },
                text = "Å£"
            };

            var moveDownButton = new Button(() => {
                onMoveDown?.Invoke(this);
            })
            {
                style = {
                            width = 20,
                            marginLeft = 1,
                            marginRight = -5,
                            fontSize = 8
                        },
                text = "Å•"
            };

            //Add the fields
            Add(removeButton);
            Add(_connectionField);
            Add(moveupButton);
            Add(moveDownButton);
            Add(connectionPort);
        }

        public void Rebind(SerializedProperty property)
        {
            _property = property;
            _connectionField.BindProperty(property);
        }

        private void ConnectionUpdated(Port port)
        {
            if (_property != null)
            {
                BaseProgressTransition connectingComponent = null;

                foreach (var edge in Port.connections)
                {
                    if (edge.input != null)
                    {
                        connectingComponent = ((ProgressTransitionNode)edge.input.node).TransitionComponent;
                        break;
                    }
                }

                if (connectingComponent != _property.objectReferenceValue)
                {
                    _property.objectReferenceValue = connectingComponent;
                    _onConnectionUpdated?.Invoke(this);
                }
            }
        }

        public void RemoveEvents()
        {
            _port.OnConnect = null;
            _port.OnDisconnect = null;
        }
    }

    public class CallbackPort : Port
    {
        private class DefaultEdgeConnectorListener : IEdgeConnectorListener
        {
            private GraphViewChange m_GraphViewChange;
            private List<Edge> m_EdgesToCreate;
            private List<GraphElement> m_EdgesToDelete;

            public DefaultEdgeConnectorListener()
            {
                this.m_EdgesToCreate = new List<Edge>();
                this.m_EdgesToDelete = new List<GraphElement>();
                this.m_GraphViewChange.edgesToCreate = this.m_EdgesToCreate;
            }

            public void OnDropOutsidePort(Edge edge, Vector2 position)
            {
            }

            public void OnDrop(UnityEditor.Experimental.GraphView.GraphView graphView, Edge edge)
            {
                this.m_EdgesToCreate.Clear();
                this.m_EdgesToCreate.Add(edge);
                this.m_EdgesToDelete.Clear();
                if (edge.input.capacity == Port.Capacity.Single)
                {
                    foreach (Edge connection in edge.input.connections)
                    {
                        if (connection != edge)
                            this.m_EdgesToDelete.Add((GraphElement)connection);
                    }
                }
                if (edge.output.capacity == Port.Capacity.Single)
                {
                    foreach (Edge connection in edge.output.connections)
                    {
                        if (connection != edge)
                            this.m_EdgesToDelete.Add((GraphElement)connection);
                    }
                }
                if (this.m_EdgesToDelete.Count > 0)
                    graphView.DeleteElements((IEnumerable<GraphElement>)this.m_EdgesToDelete);
                List<Edge> edgesToCreate = this.m_EdgesToCreate;
                if (graphView.graphViewChanged != null)
                    edgesToCreate = graphView.graphViewChanged(this.m_GraphViewChange).edgesToCreate;
                foreach (Edge edge1 in edgesToCreate)
                {
                    graphView.AddElement((GraphElement)edge1);
                    edge.input.Connect(edge1);
                    edge.output.Connect(edge1);
                }
            }
        }

        public Action<Port> OnConnect;
        public Action<Port> OnDisconnect;

        protected CallbackPort(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type) : base(portOrientation, portDirection, portCapacity, type)
        {
        }

        public override void Connect(Edge edge)
        {
            base.Connect(edge);

            OnConnect?.Invoke(this);
        }

        public override void Disconnect(Edge edge)
        {
            base.Disconnect(edge);
            OnDisconnect?.Invoke(this);
        }

        public override void DisconnectAll()
        {
            base.DisconnectAll();
            OnDisconnect?.Invoke(this);
        }

        public new static CallbackPort Create<TEdge>(
            Orientation orientation,
            Direction direction,
            Port.Capacity capacity,
            System.Type type)
            where TEdge : Edge, new()
        {
            var listener = new CallbackPort.DefaultEdgeConnectorListener();
            var ele = new CallbackPort(orientation, direction, capacity, type)
            {
                m_EdgeConnector = (EdgeConnector)new EdgeConnector<TEdge>((IEdgeConnectorListener)listener)
            };
            ele.AddManipulator((IManipulator)ele.m_EdgeConnector);
            return ele;
        }
    }
}
