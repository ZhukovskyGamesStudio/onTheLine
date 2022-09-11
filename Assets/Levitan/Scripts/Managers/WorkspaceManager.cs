using System.Collections.Generic;
using UnityEngine;

namespace Levitan {
    public class WorkspaceManager : MonoBehaviour, IAppModule {
        
        [SerializeField]
        private Transform _draggableHolder;

        [SerializeField]
        private IDraggable DialogPrefab;

        [SerializeField]
        private IDraggable TagPrefab;

        [SerializeField]
        private IDraggable ThoughtPrefab;

        [SerializeField]
        private IDraggable TransitionPrefab;

        [SerializeField]
        private IDraggable InformationPrefab;

        [SerializeField]
        private Connection ConnectionPrefab;

        private CameraController _cameraController;

        private Dictionary<string, IDraggable> _draggables = new();

        public void Init(CameraController cameraController) {
            _cameraController = cameraController;
        }

        private IDraggable InstantiateDraggable(IDraggable prefab, DraggableData data = null) {
            IDraggable draggable = Instantiate(prefab, CameraController.GetDialogPosition(), Quaternion.identity,
                _draggableHolder);
            draggable.Init();
            if (data != null) {
                draggable.SetData(data);
            }

            _draggables.Add(draggable._data.ID, draggable);
            draggable.gameObject.SetActive(true);
            return draggable;
        }

        public void ClearField() {
            foreach (Transform child in _draggableHolder) {
                Destroy(child.gameObject);
            }

            _draggables.Clear();
        }

        public void LoadWorkspace(List<DraggableData> draggables) {
            ClearField();
            foreach (DraggableData draggableData in draggables) {
                try {
                    switch (draggableData.Type) {
                        case DraggableType.Dialog:
                            InstantiateDialog(draggableData);
                            break;

                        case DraggableType.Tag:
                            InstantiateTag(draggableData);
                            break;

                        case DraggableType.Thought:
                            InstantiateThought(draggableData);
                            break;

                        case DraggableType.Transition:
                            InstantiateTransition(draggableData);
                            break;

                        case DraggableType.Information:
                            InstantiateInformation(draggableData);
                            break;
                    }
                }
                catch {
                    AppManager.Instance._LogManager.Log(draggableData._dialogData.name +" couldnt spawn it.");
                }
            }

            foreach (IDraggable idraggable in _draggables.Values) {
                try {
                    idraggable.SpawnConnections();
                }
                catch {
                    AppManager.Instance._LogManager.Log(idraggable.Name + "has no connection");
                }
            }
        }

        public List<DraggableData> CollectWorkspace() {
            List<DraggableData> datas = new();
            foreach (var idraggable in _draggables.Values) {
                datas.Add(idraggable.CollectData());
            }

            return datas;
        }

        public void CollectExportData() {
            foreach (var idraggable in _draggables.Values) {
                (idraggable as DraggableDialog)?.CollectDialogData();
            }
        }

        public void InstantiateDialog(DraggableData data = null) {
            IDraggable draggable = InstantiateDraggable(DialogPrefab, data);
            draggable._data.Type = DraggableType.Dialog;
            if (data == null)
                draggable.ChangeDialogName("New Dialog");
        }

        public void InstantiateTag(DraggableData data = null) {
            IDraggable draggable = InstantiateDraggable(TagPrefab, data);
            draggable._data.Type = DraggableType.Tag;
            if (data == null)
                draggable.ChangeDialogName("New Tag");
        }

        public void InstantiateThought(DraggableData data = null) {
            IDraggable draggable = InstantiateDraggable(ThoughtPrefab, data);
            draggable._data.Type = DraggableType.Thought;
            if (data == null)
                draggable.ChangeDialogName("New Thought");
        }

        public void InstantiateTransition(DraggableData data) {
            DraggableDialog dialog = GetDraggableById(data._dialogData.ID) as DraggableDialog;
            IDraggable draggable = InstantiateDraggable(TransitionPrefab, data);
            draggable._data.Type = DraggableType.Transition;

            dialog.transitionsHolder.AddEmptyTransition(draggable);
        }

        public void InstantiateInformation(DraggableData data) {
            DraggableDialog dialog = GetDraggableById(data._dialogData.ID) as DraggableDialog;
            IDraggable draggable = InstantiateDraggable(InformationPrefab, data);
            draggable._data.Type = DraggableType.Information;

            dialog.informationsHolder.AddEmptyInformation(draggable);
        }

        public Connection InstantiateConnection(IDraggable start) {
            Connection connection = Instantiate(ConnectionPrefab, CameraController.GetDialogPosition(),
                Quaternion.identity, _draggableHolder);
            connection.gameObject.SetActive(true);
            connection.Init(start);
            return connection;
        }

        public IDraggable GetDraggableById(string id) {
            return _draggables[id];
        }

        public static IDraggable GetDraggableStatic(string id) {
            return AppManager.Instance._workspaceManager.GetDraggableById(id);
        }

        public void DeleteDraggable(IDraggable objectToDelete) {
            _draggables.Remove(objectToDelete._data.ID);
            Destroy(objectToDelete.gameObject);
        }
    }
}