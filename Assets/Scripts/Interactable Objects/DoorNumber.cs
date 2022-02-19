using UnityEngine;

public class DoorNumber : MonoBehaviour
{
    public bool isOpen;
    public Transform Door;
    public float speed;
    AudioSource audioSource;

    Quaternion targetRot;
    Quaternion startRot;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        startRot = Door.rotation;
    }

    public void Open()
    {
        audioSource.Play();
        isOpen = true;
        targetRot = startRot * Quaternion.Euler(-90, 0, 0);
    }

    public void Close()
    {
        audioSource.Play();
        isOpen = false;
        targetRot = startRot;
    }

    void FixedUpdate()
    {
        targetRot = isOpen ? (startRot * Quaternion.Euler(90, 0, 0)) : startRot;
        if (Quaternion.Angle(Door.rotation, targetRot) > 1f)
        {
            Door.rotation = Quaternion.Slerp(Door.rotation, targetRot, speed);
        }
    }

}
