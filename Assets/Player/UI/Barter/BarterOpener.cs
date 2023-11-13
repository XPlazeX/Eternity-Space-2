// using UnityEngine;
// using UnityEngine.UI;

// public class BarterOpener : MonoBehaviour
// {
//     const float barterWindowXPosition = 1300f;
//     const float bordWidth = 100f;

//     [SerializeField] private GameObject _buttonSender;
//     [SerializeField] private Transform _panelParent;
//     [SerializeField] private GameObject _inventory_barterPanel;
//     //[SerializeField] private ScrollRect _scrollComponent;

//     private GearBarterHandler _gearBarterHandler;
//     //private ModuleChoice _moduleChoice;

//     public void UnlockBarter()
//     {
//         _buttonSender.SetActive(true);
//     }

//     public void SpawnBarter(GearBarterHandler gearBarterHandler)
//     {
//         _gearBarterHandler = Instantiate(gearBarterHandler);

//         SceneStatics.UICore.GetComponent<InventoryScaler>().AddRightRect(_gearBarterHandler.GetComponent<RectTransform>());
//     }

//     public void OpenBarter()
//     {
//         //_scrollComponent.enabled = true;
//         if (_inventory_barterPanel.activeInHierarchy)
//             return;
            
//         _inventory_barterPanel.SetActive(true);
//         TimeHandler.Pause();
//     }
// }
