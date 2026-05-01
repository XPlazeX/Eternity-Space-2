using UnityEngine;

public class BeaconBGLoader : MonoBehaviour
{
    [SerializeField] private BackgroundObject[] _backgroundObjects;

    public void Initialize() {
        if (GameSessionInfoHandler.GetSessionSave().BeaconBG == -1)
        {
            GameSessionSave save = GameSessionInfoHandler.GetSessionSave();
            save.BeaconBG = Random.Range(0, _backgroundObjects.Length);
            GameSessionInfoHandler.RewriteSessionSave(save);
        }

        InitializeObject(_backgroundObjects[GameSessionInfoHandler.GetSessionSave().BeaconBG]);
    }

    private void InitializeObject(BackgroundObject backgroundObject)
    {
        if (backgroundObject.SpawnObject == null)
            return;

        var obj = Instantiate(backgroundObject.SpawnObject, backgroundObject.Position, Quaternion.Euler(0, 0, backgroundObject.Rotation));

        if (!obj.GetComponent<CameraObjects>())
            obj.transform.SetParent(SceneStatics.CharacterCore.transform);

        obj.GetComponent<SpriteRenderer>().color = backgroundObject.SpawnColor;
    }

    #if UNITY_EDITOR
    private void Update() {
        if (Input.GetKeyDown(KeyCode.R))
        {
            GameSessionSave save = GameSessionInfoHandler.GetSessionSave();
            save.BeaconBG = Random.Range(0, _backgroundObjects.Length);
            GameSessionInfoHandler.RewriteSessionSave(save);
            print($"change beaconBG to {GameSessionInfoHandler.GetSessionSave().BeaconBG}");

            SceneTransition.ReloadScene();
        }
    }
    #endif
}
