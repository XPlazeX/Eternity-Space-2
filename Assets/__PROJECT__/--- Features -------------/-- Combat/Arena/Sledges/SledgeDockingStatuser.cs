using UnityEngine;

public class SledgeDockingStatuser : MonoBehaviour
{
    [SerializeField] private GameObject readyDockingPointer;
    [SerializeField] private GameObject notReadyDroneDockingPointer;
    [Header("Drone Pointer")]
    [SerializeField] private Transform dronePointer;
    [SerializeField] private Transform pivotTransform;
    [SerializeField] private float dronePointerOffset = 4f;
    [SerializeField] private float dronePointerHideDistance = 6f;

    public SledgeDockingStatus UpdateDockingStatus()
    {
        PowerupFabricDrone drone = FindAnyObjectByType<PowerupFabricDrone>();

        bool droneReady = drone == null || drone.Mode == PowerupFabricDrone.DroneMode.Parked;

        if (droneReady)
        {
            dronePointer.gameObject.SetActive(false);
        } 
        else
        {
            Vector3 fromPlayerToDrone = drone.transform.position - pivotTransform.position;

            dronePointer.gameObject.SetActive(fromPlayerToDrone.magnitude >= dronePointerHideDistance);

            Vector3 directionFromPlayer = fromPlayerToDrone.normalized;
            dronePointer.position = pivotTransform.position + directionFromPlayer * dronePointerOffset;
            dronePointer.up = directionFromPlayer;
            
        }

        readyDockingPointer.SetActive(droneReady);
        notReadyDroneDockingPointer.SetActive(!droneReady);
        return (droneReady) ? SledgeDockingStatus.Ready : SledgeDockingStatus.NotReady;
    }
}

public enum SledgeDockingStatus
{
    NotReady,
    Ready
}
