FSM is a state machine frame work that works around the concept of "states", "behaviours" and "transitions".
You create a machine, and give it a set of states. You then give those states behaviours, and then inform the machine how the states can be transitioned to.

An example would be a pause state.
States:
1. Hidden
2. Paused 
3. Settings
4. Help

Transitions:
1. Hidden can only go to Paused
2. Paused can go to hidden, Settings and Help
3. Settings and Help can only go to Paused.

---------------

The OnEnter will be informed on the previous state, in case it need to have a special reaction to it.
The OnExit will be informed on the next state, in case it need to have a special reaction to it.

And example of a state needing to reach is a Title Screen.
If Title exits to Settings, it might use a less "flashy" hide transition.
But if Title exits to NewGame, we might want to trigger a more grand hide animation + an audio effect.