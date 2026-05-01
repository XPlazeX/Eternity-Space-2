using UnityEngine;

public class EBookOpener : MonoBehaviour
{
    [SerializeField] private EBook.EBookCard _book;
    [SerializeField] private int _bookMenuGroup;

    public void OpenEBook(){
        SceneStatics.SceneCore.GetComponent<MenuController>().OpenGroup(_bookMenuGroup);
        SceneStatics.SceneCore.GetComponent<EBook>().OpenBook(_book);
    }
}
