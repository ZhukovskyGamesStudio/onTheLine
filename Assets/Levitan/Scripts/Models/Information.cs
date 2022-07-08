using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Levitan {
    public class Information : IDraggable {
        public Action<Information> OnDestroyPressed;
        [SerializeField]
        private GameObject plusButton;

        [SerializeField]
        private TextMeshProUGUI lineText;

        public IConnectable ThoughtConnected;
        public string _line;

        public void Init(int lineIndex, string line) {
            _connections = new List<Connection>();
            _line = line;
            lineText.text = line;
            gameObject.SetActive(true);
        }

        public void Destroy() {
            DestroyDraggable();
        }

        public override void ChangeDialogName(string newName) {
        }

        public override bool CanAddConnection(IConnectable start) {
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
            if ((connection._endPoint as IDraggable)._data.Type == DraggableType.Thought) {
                ThoughtConnected = connection._endPoint;
            }

            plusButton.SetActive(false);
        }

        public override void RemoveConnection(Connection connection) {
            base.RemoveConnection(connection);
            ThoughtConnected = null;
            plusButton.SetActive(true);
        }

        public override void DestroyDraggable() {
            base.DestroyDraggable();
            OnDestroyPressed?.Invoke(this);
            OnDestroyPressed = null;
        }
    }
}