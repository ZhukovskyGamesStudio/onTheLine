using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SimpleFileBrowser;

namespace Levitan {
    public class SaveManager : MonoBehaviour {
        public bool IsRememberCameraPosition; //MoveToSettings

        public string ProjectSavePath;

        private ProjectData _projectData = new();

        private WorkspaceManager _workspaceManager;

        private string _savePath;

        private const string LEVITAN_FILE_TYPE = ".lvtn";

        public void Init(WorkspaceManager workspaceManager) {
            _workspaceManager = workspaceManager;
        }

        public void ChangeSaveFolder(string newPath) {
            ProjectSavePath = newPath;
        }

        public void CreateNewProject() {
            _projectData = new ProjectData();
            _workspaceManager.ClearField();
        }

        public IEnumerator LoadProject() {
            AppManager.Instance._cameraController.IsEditing = true;
            yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, false, null, null,
                "Select file to load", "Load");
            AppManager.Instance._cameraController.IsEditing = false;
            if (FileBrowser.Success) {
                string json = File.ReadAllText(FileBrowser.Result[0]);
                _projectData = JsonUtility.FromJson<ProjectData>(json);
                _workspaceManager.LoadWorkspace(_projectData._draggableDatas);
                PlaceCamera();
                _savePath = FileBrowser.Result[0];
            } else {
                Debug.Log("You dont select file");
            }
        }

        private void PlaceCamera() {
            if (IsRememberCameraPosition) {
                Camera.main.transform.position = _projectData.cameraPosition;
                Camera.main.orthographicSize = _projectData.cameraSize;
            } else {
                if (_projectData._draggableDatas.Count == 0) {
                    return;
                }

                float low = 10000, top = -100000, right = -100000, left = 100000;
                foreach (var VARIABLE in _projectData._draggableDatas) {
                    Vector3 dPos = VARIABLE.position;
                    if (dPos.y < low) {
                        low = dPos.y;
                    }

                    if (dPos.y > top) {
                        top = dPos.y;
                    }

                    if (dPos.x < left) {
                        left = dPos.x;
                    }

                    if (dPos.x > right) {
                        right = dPos.x;
                    }
                }

                Vector3 position = new Vector3((right + left) / 2, (low + top) / 2);
                float maxDelta = Mathf.Max(Mathf.Abs(right - left), Mathf.Abs(top - low));
                float coefficient = 1 / 3f;
                AppManager.Instance._cameraController.SetSize(maxDelta * coefficient);
                Camera.main.transform.position = new Vector3(position.x, position.y, Camera.main.transform.position.z);
            }
        }

        public IEnumerator SaveProject() {
            AppManager.Instance._cameraController.IsEditing = true;
            yield return FileBrowser.WaitForSaveDialog(FileBrowser.PickMode.Files, false, null, null,
                "Select place to save project", "Save");
            AppManager.Instance._cameraController.IsEditing = false;
            if (FileBrowser.Success) {
                string projectName =
                    FileBrowser.Result[0][(FileBrowser.Result[0].LastIndexOf("\\", StringComparison.Ordinal) + 1)..]
                        .Replace(LEVITAN_FILE_TYPE, "");
                _projectData.projectName = projectName;
                Save(FileBrowser.Result[0] + LEVITAN_FILE_TYPE);
            }
        }

        private void Save(string path) {
            _projectData._draggableDatas = _workspaceManager.CollectWorkspace();
            _projectData.cameraPosition = Camera.main.transform.position;
            _projectData.cameraSize = Camera.main.orthographicSize;
            string json = JsonUtility.ToJson(_projectData);
            File.WriteAllTextAsync(path, json);
        }

        public void QuickSave() {
            if (string.IsNullOrEmpty(_savePath) || !File.Exists(_savePath)) {
                StartCoroutine(SaveProject());
            } else {
                Save(_savePath);
                AppManager.Instance._LogManager.Log("Project saved!");
            }
        }

        public IEnumerator ExportProject() {
            AppManager.Instance._cameraController.IsEditing = true;
            yield return FileBrowser.WaitForSaveDialog(FileBrowser.PickMode.Folders, false, null, null,
                "Select folder to export", "Export");
            AppManager.Instance._cameraController.IsEditing = false;
            if (FileBrowser.Success) {
                _workspaceManager.CollectExportData();
                _projectData._draggableDatas = _workspaceManager.CollectWorkspace();
                foreach (var draggable in _projectData._draggableDatas) {
                    if (draggable.Type == DraggableType.Dialog) {
                        Dialog asset = RuntimeParser.ParseDialogData(draggable._dialogData);
                        string json = JsonUtility.ToJson(asset);
                        File.WriteAllTextAsync(
                            FileBrowser.Result[0] + Path.DirectorySeparatorChar +
                            (string.IsNullOrEmpty(_projectData.projectName) ? "" : (_projectData.projectName + "_")) +
                            draggable._dialogData.name + ".json",
                            json);
                    }
                }
            } else {
                Debug.Log("You dont select folder");
            }
        }

        /* public void ExportDialog(IDraggable draggable) {
             string path = SimpleFileBrowser.FileBrowser. SaveFilePanel("Export dialog as scriptableObject",
                 "",
                 draggable._data._dialogData.name + ".asset",
                 "asset");
         }*/
    }

    [System.Serializable]
    public class ProjectData {
        public string projectName;
        public Vector3 cameraPosition;
        public float cameraSize;
        public List<DraggableData> _draggableDatas;
    }

    [System.Serializable]
    public class DraggableData {
        public string ID;
        public DraggableType Type;
        public DialogData _dialogData;
        public Vector3 position = Vector3.zero;
        public List<ConnectionData> _connectionsList = new();
    }

    [System.Serializable]
    public class ConnectionData {
        public ConnectionTypes type;
        public string start;
        public string end;
    }

    public enum ConnectionTypes {
        Require,
        RequireFalse
    }

    public enum DraggableType {
        Dialog,
        Tag,
        Thought,
        Transition,
        Information
    }
}