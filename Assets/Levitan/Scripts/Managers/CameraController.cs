using UnityEngine;

namespace Levitan {
    public class CameraController : MonoBehaviour, IAppModule {
        public static bool IsDrawingLine;
        public float moveMultiplier;
        public float minZoom, maxZoom;
        public float lerpSpeed = 0.3f;
        public bool IsEditing;

        [SerializeField]
        private Vector2 _workspaceBounds;

        [SerializeField]
        private float minMoveDelta = 0.3f;

        private Vector3 _cameraStartPos;
        private Vector3 _dialogOffset;

        private bool _isDragging;
        private bool _isFocused;

        private Transform _mainCamera;
        private Vector3 _mouseStartPos;

        private UIManager _uiManager;
        private WorkspaceManager _workspaceManager;

        public Vector3 MousePosition => (Input.mousePosition - new Vector3(Screen.width, Screen.height) / 2) / 100;
        private float ZoomPercent => Camera.main.orthographicSize * 1f / minZoom;

        private void Update() {
            if (_isDragging || IsEditing || !_isFocused) return;
            Collider2D info = Physics2D.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition).origin,
                Vector2.zero).collider;

            if (Input.GetKeyDown(KeyCode.S) && Input.GetKey(KeyCode.LeftControl)) {
                AppManager.Instance._saveManager.QuickSave();
                return;
            }

            if (Input.GetKeyDown(KeyCode.Delete) && !IsDrawingLine)
                if (info != null)
                    if (info.transform.TryGetComponent(out IDraggable draggable))
                        draggable.DestroyDraggable();

            if (Input.GetMouseButtonDown(1) && !IsDrawingLine) {
                _uiManager.ShowCursorMenu(MousePosition * 100);
            } else if (Input.mouseScrollDelta.y != 0) {
                float zoomDelta = Input.mouseScrollDelta.y * -1;
                Vector3 delta = MousePosition;

                if (Input.mouseScrollDelta.y > 0) {
                    Vector3 target = _mainCamera.position + delta;
                    ZoomCameraToTarget(target, zoomDelta);
                } else {
                    ZoomBack(zoomDelta);
                }
            }
        }

        private void LateUpdate() {
            _isDragging = false;
        }

        private void OnApplicationFocus(bool hasFocus) {
            _isFocused = hasFocus;
        }

        public void Init(UIManager uiManager, WorkspaceManager workspaceManager) {
            _mainCamera = Camera.main.transform;
            _uiManager = uiManager;
            _workspaceManager = workspaceManager;
            _isFocused = true;
        }

        public static Vector3 GetDialogPosition() {
            Vector3 screenPos = Input.mousePosition;
            screenPos.z = Camera.main.nearClipPlane + 1;
            Vector3 final = Camera.main.ScreenToWorldPoint(screenPos);
            final.z = 0;
            return final;
        }

        public void DragCamera() {
            if (IsEditing)
                return;
            Vector3 deltaMouse = Input.mousePosition - _mouseStartPos;
            deltaMouse *= moveMultiplier * ZoomPercent;
            if (deltaMouse.magnitude > minMoveDelta) {
                Vector3 target = _cameraStartPos - deltaMouse;
                MoveCameraToTarget(target);
                _isDragging = true;
            }
        }

        public void StartDrag() {
            _cameraStartPos = _mainCamera.position;
            _mouseStartPos = Input.mousePosition;
        }

        public void EndDrag() {
            _isDragging = false;
        }

        public void BeginDragDialog(Transform dialogTransform) {
            _dialogOffset = dialogTransform.position - GetDialogPosition();
        }

        public void DragDialog(RectTransform dialogTransform) {
            dialogTransform.position = _dialogOffset + GetDialogPosition();
        }

        private void MoveCameraToTarget(Vector3 target) {
            target = new Vector3(Mathf.Clamp(target.x, _workspaceBounds.x * -1, _workspaceBounds.x),
                Mathf.Clamp(target.y, _workspaceBounds.y * -1, _workspaceBounds.y), _mainCamera.position.z);
            _mainCamera.position = Vector3.Lerp(_mainCamera.position, target, lerpSpeed);
        }

        private void ZoomCameraToTarget(Vector3 target, float zoomDelta) {
            target = new Vector3(Mathf.Clamp(target.x, _workspaceBounds.x * -1, _workspaceBounds.x),
                Mathf.Clamp(target.y, _workspaceBounds.y * -1, _workspaceBounds.y), _mainCamera.position.z);

            float newSize = Camera.main.orthographicSize + zoomDelta;
            if (newSize < minZoom || newSize > maxZoom)
                return;
            Camera.main.orthographicSize = newSize;
            _mainCamera.position = Vector3.Lerp(_mainCamera.position, target, lerpSpeed);
        }

        public void SetSize(float newSize) {
            newSize = Mathf.Clamp(newSize, minZoom, maxZoom);
            Camera.main.orthographicSize = newSize;
        }

        private void ZoomBack(float zoomDelta) {
            Vector3 target = _mainCamera.position;

            float newSize = Camera.main.orthographicSize + zoomDelta;
            newSize = Mathf.Clamp(newSize, minZoom, maxZoom);
            Camera.main.orthographicSize = newSize;
            _mainCamera.position = Vector3.Lerp(_mainCamera.position, target, lerpSpeed);
        }
    }
}