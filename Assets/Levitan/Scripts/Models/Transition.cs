using System;
using System.Collections.Generic;
using UnityEngine;

namespace Levitan {
    public class Transition : IDraggable {
        public Action<int> OnDestroyPressed;
        public int _number;

        [SerializeField]
        private GameObject plusButton;

        public IConnectable ThoughtConnected, DialogConnected;

        public void Init(int number) {
            _number = number;
            _connections = new List<Connection>();
            gameObject.SetActive(true);
        }

        public void Destroy() {
            DestroyDraggable();
        }

        public override void ChangeDialogName(string newName) {
        }

        public override bool CanAddConnection(IConnectable start) {
            IDraggable draggable = start as IDraggable;
            if (draggable != null) {
                return draggable._data.Type == DraggableType.Thought && ThoughtConnected == null;
            }

            return false;
        }

        public override Vector3 GetRectEdgeForPosition(Vector3 position) {
            Vector3 res = transform.position + Vector3.right * (2.5f * Mathf.Sign(position.x - transform.position.x));
            res.z = 0;
            return res;
        }

        protected override void OnMouseDrag() {
        }

        public override void AddConnection(Connection connection) {
            base.AddConnection(connection);
            if (connection._endPoint == this) {
                ThoughtConnected = connection._startPoint;
            } else {
                DialogConnected = connection._endPoint;
            }

            plusButton.SetActive(DialogConnected == null);
        }

        public override void RemoveConnection(Connection connection) {
            base.RemoveConnection(connection);
            if (connection._startPoint == this) {
                DialogConnected = null;
            } else {
                ThoughtConnected = null;
            }

            plusButton.SetActive(DialogConnected == null);
        }

        public override void DestroyDraggable() {
            base.DestroyDraggable();
            OnDestroyPressed?.Invoke(_number);
            OnDestroyPressed = null;
        }
    }
}