using UnityEngine;
using UnityEngine.UI;

public class LabUI : MonoBehaviour
{
    [SerializeField] private Lab _lab;
    [SerializeField] private string _labLocalizationFilename;
    [SerializeField] private int _labLocalizationCategoriesRow;
    [SerializeField] private Text _terminalLabel;
    [SerializeField] private ScrollRect _scrollingRect;
    [Space()]
    [SerializeField] private LabPerfocard[] _labCards;

    private void Start() {
        _terminalLabel.text = SceneLocalizator.GetLocalizedString(_labLocalizationFilename, 1, 1);
    }

    public void ScrollValueChanged(Vector2 value)
    {
        CheckScrollConditions(_scrollingRect.horizontalNormalizedPosition);
    }

    public void CheckScrollConditions(float x)
    {
        for (int i = 0; i < _labCards.Length; i++)
        {
            if (x > _labCards[i].minXrectNormalized && x < _labCards[i].maxXrectNormalized)
            {
                LoadCategory(_labCards[i].labCategory);
                return;
            }
        }

        _lab.BlockControls();
        _terminalLabel.text = SceneLocalizator.GetLocalizedString(_labLocalizationFilename, 1, 1);
    }

    public void LoadCategory(int id)
    {
        _terminalLabel.text = SceneLocalizator.GetLocalizedString(_labLocalizationFilename, _labLocalizationCategoriesRow, id);
        _lab.LoadCategory(id);
    }

    [System.Serializable]
    private struct LabPerfocard
    {
        public int labCategory;
        public float minXrectNormalized;
        public float maxXrectNormalized;
    }
}
