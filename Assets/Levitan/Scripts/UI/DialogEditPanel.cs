using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace Levitan {
    public class DialogEditPanel : MonoBehaviour {

        public Color first, second;
        
        
        [SerializeField]
        private TMP_InputField _allText, _sayToOperatorText, _fromInput, _toInput;

        [SerializeField]
        private TMP_InputField _addInfoInput, _removeInfoInput;

        [SerializeField]
        private TMP_InputField _nameText;

        private DialogData curData;
        private DraggableDialog curDraggableDialog;

        private string _currentSelectedText;
        private void Awake() {
            _allText.onTextSelection.AddListener(ChangeSelectionText);
            _sayToOperatorText.onTextSelection.AddListener(ChangeSelectionText);
        }

        public void Open(DraggableDialog draggableDialog) {
            AppManager.Instance._cameraController.IsEditing = true;
            curDraggableDialog = draggableDialog;
            curData = curDraggableDialog._data._dialogData;
            gameObject.SetActive(true);
            _sayToOperatorText.SetTextWithoutNotify(curData.SayToOperator);
            _allText.SetTextWithoutNotify(curData.allText);
            _nameText.SetTextWithoutNotify(curData.name);
            _fromInput.SetTextWithoutNotify(curData.from.ToString());
            _toInput.SetTextWithoutNotify(curData.to.ToString());
            _currentSelectedText = string.Empty;
            RedrawText();
            RedrawOperatorText();
        }

        public void ChangeSelectionText(string text, int start, int end) {
            if (end < start) {
                (end, start) = (start, end);
            }
              
            _currentSelectedText = text.Substring(start, end - start);
            _currentSelectedText = ClearText(_currentSelectedText);
        }

        public void ChangeName(string text) {
            curData.name = text;
            curDraggableDialog.ChangeDialogName(text);
        }

        public void ChangeSayToOperator(string text) {
            text = ClearText(text);
            curData.SayToOperator = text;
            RedrawOperatorText();
        }

        public void OnStartEdit() {
            _allText.SetTextWithoutNotify(ClearText(_allText.text));
        }
        
        public void OnStartEditToOperator() {
            _sayToOperatorText.SetTextWithoutNotify(ClearText(_sayToOperatorText.text));
        }

        public void ChangeText(string text) {
            text = ClearText(text);
            curData.allText = text;
            curDraggableDialog.ChangeDialogText(text);
            RedrawText();
        }

        private string ClearText(string dirtyText) {
            dirtyText = Regex.Replace(dirtyText, @"<i><color=grey>\S* </color></i>", "");
            dirtyText = Regex.Replace(dirtyText, @"(<b>|</b>|</color>)", "");
            dirtyText = Regex.Replace(dirtyText, $@"<color=(grey|#{ColorUtility.ToHtmlStringRGBA(first)}|#{ColorUtility.ToHtmlStringRGBA(second)})>", "");
            dirtyText = Regex.Replace(dirtyText, "\r", "");
            return dirtyText;
        }

        private void RedrawOperatorText() {
            string dirtyText = ClearText(_sayToOperatorText.text);
            string[] lines = dirtyText.Split("\n");
            bool isFirstColor = true;
            if (lines.Length > 0) {
                foreach (Information informationData in curDraggableDialog.informationsHolder.informations) {
                    dirtyText = dirtyText.Replace(informationData._line, "<b>" + informationData._line + "</b>");
                }
            }

            _sayToOperatorText.SetTextWithoutNotify(dirtyText);
        }

        private void RedrawText() {
            string dirtyText = ClearText(_allText.text);
            string[] lines = dirtyText.Split("\n");
            bool isFirstColor = true;
            if (lines.Length > 0) {
                dirtyText = $@"<color=#{ColorUtility.ToHtmlStringRGBA(first)}>" +
                            lines[0].Insert(0, "<i><color=grey>" + 0 + ". </color></i>") + "</color>";
                if (lines.Length > 1) {
                    for (int i = 1; i < lines.Length; i++) {
                        if (lines[i].Length != 0) {
                            if (lines[i][0] != '-') {
                                isFirstColor = !isFirstColor;
                            }
                        }

                        dirtyText +=
                            "\n" + $@"<color=#{ColorUtility.ToHtmlStringRGBA(isFirstColor ? first : second)}>" +
                            lines[i].Insert(0, "<i><color=grey>" + i + ". </color></i>") + "</color>";
                    }
                }

                foreach (Information informationData in curDraggableDialog.informationsHolder.informations) {
                    dirtyText = dirtyText.Replace(informationData._line, "<b>" + informationData._line + "</b>");
                }
            }

            _allText.SetTextWithoutNotify(dirtyText);
        }

        public void ChangeFrom(string text) {
            curData.from = Convert.ToInt32(text);
        }

        public void ChangeTo(string text) {
            curData.to = Convert.ToInt32(text);
        }

        public void AddInformation() {
            if (String.IsNullOrEmpty(_currentSelectedText)) {
                Debug.Log("Selected line is Empty!");
                return;
            }

            foreach (var information in curDraggableDialog.informationsHolder.informations) {
                if (information._line == _currentSelectedText) {
                    Debug.Log("Already added that line!");
                    return;
                }
            }

            if (curData.informations == null) {
                curData.informations = new List<NewInformationData>();
            }
            curData.informations.Add(new NewInformationData() {
                line = _currentSelectedText
            });
            curDraggableDialog.InstantiateInformation(_currentSelectedText);
            RedrawText();
            RedrawOperatorText();
        }

        public void Close() {
            gameObject.SetActive(false);
            curDraggableDialog.ChangeFromToText(curData.from,curData.to);
            AppManager.Instance._cameraController.IsEditing = false;
        }
    }
}