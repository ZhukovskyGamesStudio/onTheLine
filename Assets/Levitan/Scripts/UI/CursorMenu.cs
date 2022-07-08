using UnityEngine;

namespace Levitan {
    public class CursorMenu : MonoBehaviour {
        public void CreateTag() {
            AppManager.Instance._workspaceManager.InstantiateTag();
            gameObject.SetActive(false);
        }

        public void CreateDialog() {
            AppManager.Instance._workspaceManager.InstantiateDialog();
            gameObject.SetActive(false);
        }

        public void CreateThought() {
            AppManager.Instance._workspaceManager.InstantiateThought();
            gameObject.SetActive(false);
        }
    }
}