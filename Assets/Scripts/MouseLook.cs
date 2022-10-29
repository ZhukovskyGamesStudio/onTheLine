using UnityEngine;

public class MouseLook : MonoBehaviour {
    [SerializeField]
    private Transform headTransform;

    public enum RotationAxes {
        MouseXAndY = 0,
        MouseX = 1,
        MouseY = 2
    }

    public RotationAxes axes = RotationAxes.MouseXAndY;
    public float sensitivityX = 15F;
    public float sensitivityY = 15F;

    public float minimumX = -360F;
    public float maximumX = 360F;

    public float minimumY = -60F;
    public float maximumY = 60F;

    [HideInInspector]
    public float SensivityMultiplier = 1;

    float rotationY = 0F;
    bool isFixed;

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isFixed = false;
    }

    void Update() {
        if (isFixed)
            return;
        if (Input.GetKeyDown(KeyCode.F)) {
            isFixed = !isFixed;
        }

        if (axes == RotationAxes.MouseXAndY) {
            float rotationX = headTransform.localEulerAngles.y +
                              Input.GetAxis("Mouse X") * sensitivityX * SensivityMultiplier;
            rotationX = 180 + Mathf.Clamp(rotationX - 180, minimumX, maximumX);

            rotationY += Input.GetAxis("Mouse Y") * sensitivityY * SensivityMultiplier;
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

            headTransform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
        } else if (axes == RotationAxes.MouseX) {
            headTransform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX * SensivityMultiplier, 0);
        } else {
            rotationY += Input.GetAxis("Mouse Y") * sensitivityY * SensivityMultiplier;
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

            headTransform.localEulerAngles = new Vector3(-rotationY, headTransform.localEulerAngles.y, 0);
        }

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit)) {
            if (hit.transform.gameObject.GetComponent<InteractableObject>()) {
                if (Input.GetMouseButtonDown(0))
                    hit.transform.gameObject.GetComponent<InteractableObject>().OnmouseDown();
                //Debug.Log("Hitted Interactable");
            }
        }
    }

    public void FixCamera(bool isOn) {
        isFixed = isOn;
    }
}