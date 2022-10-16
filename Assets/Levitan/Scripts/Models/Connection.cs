using System;
using System.Linq;
using UnityEngine;

namespace Levitan {
    public class Connection : MonoBehaviour {
        public bool isForbidden;

        [SerializeField]
        private SpriteRenderer _typeIcon;

        [SerializeField]
        private SpriteRenderer _lineEnd, _lineEndIcon;

        [SerializeField]
        private SpriteRenderer _lineStart;

        [SerializeField]
        private LineRenderer _line;

        public float EndCollisionRadius;
        public Color FalseColor;
        public IConnectable _endPoint;
        private bool _isDragging;
        public IConnectable _startPoint;
        private IDraggable _tempTarget;

        private void Update() {
            if (!_isDragging)
                return;

            if (Input.GetMouseButtonDown(0)) {
                StopDrawing();
                return;
            }

            if (Input.GetMouseButtonDown(1)) {
                CancelConnection();
                return;
            }

            Move();
            CheckOtherDraggable();
        }

        private void OnCollisionEnter(Collision collision) {
            Debug.Log("Collide to draggable");
        }

        public void Init(IDraggable start) {
            _startPoint = start;
            _line.positionCount = 2;
            _lineStart.color = AppManager.Instance._ColorManager.GetColorByDraggable(start._data.Type);
            DraggableType endType = GetEndForStart(start._data.Type);
            _lineEndIcon.color = AppManager.Instance._ColorManager.GetColorByDraggable(endType);

            // A simple 2 color gradient with a fixed alpha of 1.0f.
            float alpha = 1.0f;
            Gradient gradient = new();
            gradient.SetKeys(
                new[] {new GradientColorKey(_lineStart.color, 0.0f), new GradientColorKey(_lineEndIcon.color, 1.0f)},
                new[] {new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f)}
            );
            if (start._data.Type == DraggableType.Tag || start._data.Type == DraggableType.Dialog) {
                _typeIcon.gameObject.SetActive(true);
                _typeIcon.color = _lineEndIcon.color;
            }

            _line.colorGradient = gradient;
        }

        private DraggableType GetEndForStart(DraggableType startType) {
            return startType switch {
                DraggableType.Dialog => DraggableType.Tag,
                DraggableType.Tag => DraggableType.Dialog,
                DraggableType.Thought => DraggableType.Transition,
                DraggableType.Transition => DraggableType.Dialog,
                DraggableType.Information => DraggableType.Thought,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public void Init(IConnectable start) {
            _startPoint = start;
            _line.positionCount = 2;
        }

        public void StartDrag() {
            if (AppManager.Instance._cameraController.IsEditing)
                return;
            _isDragging = true;
        }

        public void SetData(ConnectionData data) {
            WorkspaceManager workspaceManager = AppManager.Instance._workspaceManager;
            _startPoint = workspaceManager.GetDraggableById(data.start);
            _endPoint = workspaceManager.GetDraggableById(data.end);
            _startPoint.AddConnection(this);
            _endPoint.AddConnection(this);
            isForbidden = data.type == ConnectionTypes.RequireFalse;
            Redraw();
        }

        public ConnectionData CollectData() {
            return new() {
                start = _startPoint.ID,
                end = _endPoint.ID,
                type = isForbidden ? ConnectionTypes.RequireFalse : ConnectionTypes.Require
            };
        }

        public void StopDrawing() {
            if (_tempTarget != null) {
                _endPoint = _tempTarget;

                if (!_endPoint.CanAddConnection(_startPoint)) {
                    CancelConnection();
                    return;
                }

                if (_startPoint.HasSameConnection(_endPoint.ID)) {
                    Debug.Log("You already draw this connection.");
                    CancelConnection();
                    return;
                }

                _isDragging = false;
                //SendThemMessages
                _startPoint.AddConnection(this);
                _endPoint.AddConnection(this);
                CameraController.IsDrawingLine = false;
            } else {
                CancelConnection();
            }
        }

        private void CancelConnection() {
            _startPoint.RemoveConnection(this);
            CameraController.IsDrawingLine = false;
            _isDragging = false;
            Destroy(gameObject);
        }

        public void Redraw() {
            _line.SetPosition(0, _startPoint.GetRectEdgeForPosition(_endPoint.Position));
            _line.SetPosition(1, _endPoint.GetRectEdgeForPosition(_line.GetPosition(0)));
            _lineStart.transform.position = _line.GetPosition(0);
            _lineEnd.transform.position = _line.GetPosition(1);
            _lineEnd.transform.right =
                _line.GetPosition(_line.positionCount - 1) - _line.GetPosition(_line.positionCount - 2);
            _typeIcon.color = isForbidden ? FalseColor : _lineEndIcon.color;
        }

        private void Move() {
            _line.SetPosition(0, _startPoint.GetRectEdgeForPosition(CameraController.GetDialogPosition()));

            if (_tempTarget != null)
                _line.SetPosition(1, _tempTarget.GetRectEdgeForPosition(_line.GetPosition(0)));
            else
                _line.SetPosition(1, CameraController.GetDialogPosition());

            _lineStart.transform.position = _line.GetPosition(0);
            _lineEnd.transform.position = _line.GetPosition(1);
            _lineEnd.transform.right =
                _line.GetPosition(_line.positionCount - 1) - _line.GetPosition(_line.positionCount - 2);
        }

        private void CheckOtherDraggable() {
            Collider2D[] colliders =
                Physics2D.OverlapCircleAll(CameraController.GetDialogPosition(), EndCollisionRadius);
            IDraggable collidedDraggable = colliders
                .FirstOrDefault(collider1 => collider1.TryGetComponent(out IDraggable co) && co != _startPoint)
                ?.GetComponent<IDraggable>();
            _tempTarget = collidedDraggable;
            if (collidedDraggable != null) {
                //AttachArrowToSideOfIt
            } else {
                _tempTarget = null;
            }
        }

        public void DisconnectEnd() {
            if (AppManager.Instance._cameraController.IsEditing)
                return;
            _startPoint.RemoveConnection(this);
            _endPoint.RemoveConnection(this);
            _isDragging = true;
            _tempTarget = null;
            _endPoint = null;
        }

        public void ChangeType() {
            isForbidden = !isForbidden;
            _typeIcon.color = isForbidden ? FalseColor : _lineEndIcon.color;
        }

        public void DisconnectAndDelete() {
            _startPoint.RemoveConnection(this);
            _endPoint.RemoveConnection(this);
            Destroy(gameObject);
        }
    }
}