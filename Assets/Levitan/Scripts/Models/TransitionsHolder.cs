using System.Collections.Generic;
using UnityEngine;

namespace Levitan {
    public class TransitionsHolder : MonoBehaviour {
        [SerializeField]
        public DraggableDialog _dialog;

        [SerializeField]
        private Transform PlusButton;

        private List<Transition> transitions = new List<Transition>();

        private const float transitionHeight = 0.45f;

        public void Init() {
            transitions = new List<Transition>();
        }

        public void InstantiateTransition() {
            DraggableData data = new() {
                _connectionsList = new List<ConnectionData>(),
                _dialogData = new DialogData() {
                    ID = _dialog.ID
                },
                ID = System.Guid.NewGuid().ToString()
            };
            AppManager.Instance._workspaceManager.InstantiateTransition(data);
        }

        public List<TransitionData> CollectTransitionsData() {
            List<TransitionData> datas = new();
            foreach (var transition in transitions) {
                try {
                    TransitionData data = new() {
                        dialog = transition.DialogConnected.ID,
                        thought = transition.ThoughtConnected.Name
                    };
                    datas.Add(data);
                }
                catch {
                    AppManager.Instance._LogManager.Log("Some transition of " + _dialog._data._dialogData.name +
                                                        " is incorrect.");
                }
            }

            return datas;
        }

        public void RedrawConnections() {
            foreach (var transition in transitions) {
                transition.RedrawConnections();
            }
        }

        public void AddEmptyTransition(IDraggable draggable) {
            draggable.transform.SetParent(transform);
            Transition transition = draggable as Transition;
            transition.transform.localPosition = new Vector3(0, transitions.Count * -1 * transitionHeight, 0);
            transition.OnDestroyPressed += RemoveTransition;
            transition.Init(transitions.Count);
            transitions.Add(transition);
            MovePlusButton();
        }

        private void RemoveTransition(int number) {
            transitions.RemoveAt(number);
            MovePlusButton();
            for (int i = 0; i < transitions.Count; i++) {
                transitions[i].transform.localPosition = new Vector3(0, i * -1 * transitionHeight, 0);
                transitions[i]._number = i;
            }

            RedrawConnections();
        }

        private void MovePlusButton() {
            PlusButton.localPosition = new Vector3(0,
                transitions.Count * -1 * transitionHeight - (transitions.Count > 0 ? 0.6f : 0.3f), 0);
        }

        public void DestroyAll() {
            List<Transition> trns = new List<Transition>(transitions);
            foreach (var transition in trns) {
                transition.DestroyDraggable();
            }
        }
    }
}