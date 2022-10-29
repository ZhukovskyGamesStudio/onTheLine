using UnityEngine;
using UnityEngine.Events;

public class PhrazeLine : MonoBehaviour {

   public UnityEvent onMouseDownAction;
   
   public void OnMouseDown() {
     onMouseDownAction?.Invoke();
   }
}
