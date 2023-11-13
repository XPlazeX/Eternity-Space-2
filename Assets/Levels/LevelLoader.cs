// using UnityEngine;
// using System.Collections;

// public class LevelLoader : MonoBehaviour
// {
//     //[SerializeField] private WeightSelectorSelector[] _wsSelectors;
//     [Space()]
//     [Header("Testing")]
//     [SerializeField] private bool _testMode;
//     [SerializeField] private int _locationCode;
//     [SerializeField] private int _levelType = 0;

//     //[SerializeField] private int[] _nebulaeID;

//     public bool Initialize()
//     {
//         if (_testMode)
//         {
//             LoadLocation(_locationCode);
//             //LoadType(_levelType);

//             return true;
//         }

//         //int selectedType = 0;


//         GameSessionSave save = GameSessionInfoHandler.GetSessionSave();

//         // if (save.EnterNewLevel && save.CurrentLevel != 0)
//         // {
//         //     float select = 0f;
//         //     for (int i = 0; i < _wsSelectors.Length; i++)
//         //     {
//         //         select = Random.value;

//         //         if (select <= _wsSelectors[i].Chance)
//         //         {
//         //             selectedType = i;
//         //             break;
//         //         }
//         //     }
            
//         //     print("New WS selected!");
//         //     save.WeightSelectorID = selectedType;
//         //     GameSessionInfoHandler.RewriteSessionSave(save);
//         // }
//         // else {
//         //     selectedType = GameSessionInfoHandler.GetSessionSave().WeightSelectorID;
//         // }

//         LoadLocation(GameSessionInfoHandler.GetSessionSave().LocationID);
//         //LoadType(selectedType);

//         return true;
//     }

//     // private IEnumerator LoadingLocation(int id)
//     // {

//     // }

//     private void LoadLocation(int locCode)
//     {
//         GameObject.FindWithTag("BetweenScenes").GetComponent<MissionsDatabase>().EnforceLevelCore(locCode);
//     }

//     // public void LoadType(int type = 0)
//     // {
//     //     WeightSelector weightSelector = Instantiate(_wsSelectors[type].WS);
//     //     weightSelector.Initialize();

//     //     TypeModifier[] typeModifiers = weightSelector.GetComponents<TypeModifier>();
//     //     for (int i = 0; i < typeModifiers.Length; i++)
//     //     {
//     //         typeModifiers[i].Enforce();
//     //     }
//     // }

//     #if UNITY_EDITOR
//     private void Update() {
//         if (Input.GetKeyDown(KeyCode.R))
//         {
//             //PrepareBeaconColorCheme();
//             //print("change beaconCS");
//             SceneTransition.SwitchToScene("Game");
//         }
//     }
//     #endif

//     [System.Serializable]
//     private class WeightSelectorSelector
//     {
//         [SerializeField] private WeightSelector _weightSelector;
//         [SerializeField][Range(0, 1f)] private float _partChance;

//         public WeightSelector WS => _weightSelector;
//         public float Chance => _partChance;
//     }
// }
