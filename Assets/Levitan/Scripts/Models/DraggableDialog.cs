using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Levitan {
    public class DraggableDialog : IDraggable {
        [SerializeField]
        public InformationsHolder informationsHolder;

        [SerializeField]
        public TransitionsHolder transitionsHolder;

        [SerializeField]
        private TextMeshProUGUI DialogAllText;

        [SerializeField]
        private TMP_Text _FromToText;

        public void OpenDialogEditPanel() {
            AppManager.Instance._uiManager.OpenDialogEditPanel(this);
        }

        public override void SetData(DraggableData data) {
            base.SetData(data);
            transitionsHolder.Init();
            informationsHolder.Init();
            ChangeDialogText(data._dialogData.allText);
            ChangeFromToText(data._dialogData.from, data._dialogData.to);
            RedrawConnections();
        }

        public void ChangeDialogText(string newText) {
            _data._dialogData.allText = newText;
            DialogAllText.text = newText;
        }

        public void ChangeFromToText(int from, int to) {
            _FromToText.text = from + " -> " + to;
        }

        public void ChangeSayToOperator(string newText) {
            _data._dialogData.SayToOperator = newText;
        }

        public void InstantiateInformation(string text) {
            DraggableData data = new() {
                _connectionsList = new List<ConnectionData>(),
                _dialogData = new DialogData {
                    ID = _data.ID,
                    allText = text
                },
                ID = Guid.NewGuid().ToString()
            };
            AppManager.Instance._workspaceManager.InstantiateInformation(data);
        }

        public override void RedrawConnections() {
            base.RedrawConnections();
            transitionsHolder.RedrawConnections();
            informationsHolder.RedrawConnections();
        }

        public DialogData CollectDialogData() {
            DialogData data = _data._dialogData;
            data.listenKeys = new List<string>();
            data.listenValues = new List<string>();
            data.requireTags = new List<string>();
            data.forbiddenTags = new List<string>();
            data.produceTags = new List<string>();
            foreach (ConnectionData connectionData in _data._connectionsList) {
                if (connectionData.start == _data.ID) {
                    IDraggable draggable = WorkspaceManager.GetDraggableStatic(connectionData.end);
                    if (draggable._data.Type == DraggableType.Tag)
                        data.produceTags.Add(draggable._data._dialogData.name);
                }

                if (connectionData.end == _data.ID) {
                    IDraggable draggable = WorkspaceManager.GetDraggableStatic(connectionData.start);
                    if (draggable._data.Type == DraggableType.Tag) {
                        if (connectionData.type == ConnectionTypes.RequireFalse)
                            data.forbiddenTags.Add(draggable._data._dialogData.name);
                        else
                            data.requireTags.Add(draggable._data._dialogData.name);
                    }
                }
            }

            data.transitions = transitionsHolder.CollectTransitionsData();
            data.informations = informationsHolder.CollectInformationsData();
            data.ID = _data.ID;
            return _data._dialogData;
        }

        public override void DestroyDraggable() {
            transitionsHolder.DestroyAll();
            informationsHolder.DestroyAll();
            base.DestroyDraggable();
        }

        public override bool CanAddConnection(IConnectable start) {
            IDraggable draggable = start as IDraggable;
            if (draggable != null) return draggable._data.Type is DraggableType.Tag or DraggableType.Transition;

            return false;
        }

        protected void DoubleClick() {
            OpenDialogEditPanel();
        }
    }

    [Serializable]
    public class DialogData {
        public string ID;
        public string SayToOperator;
        public int from, to;
        public string allText;
        public string name;
        public List<string> requireTags;
        public List<string> forbiddenTags;
        public List<string> produceTags;
        public List<string> listenKeys;
        public List<string> listenValues;
        public List<NewInformationData> informations;
        public List<TransitionData> transitions;

        public static DialogData Default => new() {
            name = "NewDialog",
            allText = "PutDialogTextHere..."
        };
    }

    [Serializable]
    public class NewInformationData {
        public string line;
        public string thought;
    }

    [Serializable]
    public class TransitionData {
        public string thought;
        public string dialog;
    }
}