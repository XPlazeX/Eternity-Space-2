using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class DialogueOpener : MonoBehaviour
{
    //const string prefab_name = "Dialogue Window";
    [SerializeField] private AssetReference _dialogueAsset;
    
    private AsyncOperationHandle currentDialogueOperationHandle;
    private Transform _canvasTransform;

    private void Awake() {
        SceneTransition.SceneOpened += FindCanvas;
    }

    public void FindCanvas()
    {
        if (PlayerPrefs.GetFloat("DialogMod", 0) == 2)
            return;
            
        _canvasTransform = GameObject.FindWithTag("Canvas").transform;

        if (SceneTransition.ActiveSceneName == "Lobby" && !string.IsNullOrEmpty(GlobalSaveHandler.GetSave().LobbyDialogue))
        {
            GlobalSave gsave = GlobalSaveHandler.GetSave();
            TriggerDialogue(gsave.LobbyDialogue);
            gsave.LobbyDialogue = null;
            GlobalSaveHandler.RewriteSave(gsave);
        } else if (SceneTransition.ActiveSceneName == "MissionMenu")
        {
            GetComponent<MissionsDatabase>().LoadMenuDialogue(GameSessionInfoHandler.GetSessionSave().LocationID);
        }
    }

    private void OnDisable() {
        SceneTransition.SceneOpened -= FindCanvas;
    }

    public void TriggerDialogue(string name)
    {
        StartCoroutine(OpenDialogueInternal(name));
        print($"trigger dialogue: {name}");
    }

    public void TriggerDialogue(string name, float delay)
    {
        StartCoroutine(OpenDialogueInternal(name, delay));
        print($"trigger dialogue: {name}");
    }

    private IEnumerator OpenDialogueInternal(string name, float delay = 0f)
    {
        if (currentDialogueOperationHandle.IsValid())
        {
            Addressables.Release(currentDialogueOperationHandle);
        }

        var dialogueReference = _dialogueAsset;
        currentDialogueOperationHandle = dialogueReference.LoadAssetAsync<GameObject>();
        yield return currentDialogueOperationHandle;

        if (delay > 0)
            yield return new WaitForSeconds(delay);

        if (currentDialogueOperationHandle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject dialogObject = (GameObject)currentDialogueOperationHandle.Result;

            var sample = Instantiate(dialogObject);

            sample.transform.SetParent(_canvasTransform);
            sample.transform.SetSiblingIndex(sample.transform.GetSiblingIndex() - 1);
            sample.transform.localScale = Vector3.one;
            sample.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            sample.GetComponent<RectTransform>().sizeDelta = new Vector2(1f, 1f);

            try
            {
                sample.GetComponent<Dialogue>().InitializeDialog(name);
            }
            catch (System.Exception)
            {
                Debug.Log($"Ошибка инициализации диалога: {name}");
                throw;
            }
        }
    }
}
