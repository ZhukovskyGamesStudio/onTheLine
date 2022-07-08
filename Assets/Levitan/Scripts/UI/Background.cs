using UnityEngine;
using UnityEngine.Events;

namespace Levitan {
    public class Background : MonoBehaviour {
        [SerializeField]
        private UnityEvent OnDragAction;

        [SerializeField]
        private UnityEvent OnStartDragAction;

        private bool _isDragging;

        private void OnMouseDrag() {
            if (!_isDragging) {
                OnStartDragAction?.Invoke();
            }

            _isDragging = true;
            OnDragAction?.Invoke();
        }

        private void OnMouseUp() {
            _isDragging = false;
        }
    }
}