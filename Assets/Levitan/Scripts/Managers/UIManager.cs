using UnityEngine;

namespace Levitan {
    public class UIManager : MonoBehaviour, IAppModule {
        [SerializeField]
        private RectTransform _cursorMenu;

        [SerializeField]
        private DialogEditPanel _dialogEditPanel;

        private SaveManager _saveManager;

        public void Init(SaveManager saveManager) {
            _saveManager = saveManager;
        }

        public void ShowCursorMenu(Vector3 position) {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                transform as RectTransform,
                Input.mousePosition, Camera.main,
                out Vector2 movePos);

            //transform.position = transform.TransformPoint(movePos);

            _cursorMenu.gameObject.SetActive(true);
            //position.z =   _cursorMenu.localPosition.z;
            _cursorMenu.position = transform.TransformPoint(movePos);
        }

        public void OpenDialogEditPanel(DraggableDialog dialogData) {
            _dialogEditPanel.Open(dialogData);
        }

        public void NewProject() {
            _saveManager.CreateNewProject();
        }

        public void LoadProject() {
            StartCoroutine(_saveManager.LoadProject());
        }

        public void SaveProject() {
            StartCoroutine(_saveManager.SaveProject());
        }

        public void ExportProject() {
            StartCoroutine(_saveManager.ExportProject());
        }
    }
}