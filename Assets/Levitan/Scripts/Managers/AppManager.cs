using UnityEngine;

namespace Levitan {
    public class AppManager : MonoBehaviour {
        public static AppManager Instance;

        [SerializeField]
        public CameraController _cameraController;

        [SerializeField]
        public UIManager _uiManager;

        [SerializeField]
        public WorkspaceManager _workspaceManager;

        [SerializeField]
        public SaveManager _saveManager;

        [SerializeField]
        public ColorManager _ColorManager;

        [SerializeField]
        public LogManager _LogManager;

        private void Awake() {
            Instance = this;
            _cameraController.Init(_uiManager, _workspaceManager);
            _uiManager.Init(_saveManager);
            _workspaceManager.Init(_cameraController);
            _saveManager.Init(_workspaceManager);
        }
    }
}