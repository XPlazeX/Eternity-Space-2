using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EBook : MonoBehaviour
{
    public const string ebook_path = "Books";
    public const string line_separator = "</br>";
    private const string card_switch_trigger = "CardSwitch";

    [SerializeField] private Text _head;
    [SerializeField] private Text _containment;
    [SerializeField] private Text _pageLabel;
    [SerializeField] private Image _screenImage;
    [SerializeField] private Color _normalScreenColor;
    [SerializeField] private Color _highlitedScreenColor;
    [Space()]
    [SerializeField] private Animator _animator;
    [SerializeField] private float _animationTime = 2f;
    [SerializeField] private Image _cardImage;
    [Space()]
    [SerializeField] private EBookCard _defaultBook;
    [SerializeField] private bool _autoStart = true;

    private TextLoader _textLoader;
    private Sprite _nextCardSprite;
    private int _page = 0;
    private int _pageCount;
    private bool _cardInserted = true;
    private bool _light = false;
    
    private void Start() {
        if (_autoStart)
            Initialize();
    }

    private void Initialize()
    {
        _textLoader = new TextLoader(ebook_path, _defaultBook.bookName);
        _light = false;
        _screenImage.color = _light ? _highlitedScreenColor : _normalScreenColor;

        OpenBook(_defaultBook, true);
    }

    public string GetReplacedString(string input)
    {
        return input.Replace(line_separator, "\n");
    }

    public void OpenBook(EBookCard book, bool fast = false)
    {
        _textLoader = new TextLoader(ebook_path, book.bookName);
        _head.text = GetReplacedString(_textLoader.GetCell(0,0));
        _pageCount = _textLoader.TextMatrixLength - 1;

        ClearText();
        _cardInserted = false;
        _nextCardSprite = book.cardSprite;
        _page = 0;

        if (fast)
        {
            _cardImage.sprite = _nextCardSprite;
            _cardInserted = true;
            OpenPage(0);
        }
        _animator.SetTrigger(card_switch_trigger);
    }

    public void OpenPage(int id)
    {
        if (_textLoader == null)
            throw new System.ArgumentNullException("Книга не существует/не была инициализирована!");

        _head.text = GetReplacedString(_textLoader.GetCell(0, 0));
        _containment.text = GetReplacedString(_textLoader.GetCell(id + 1, 0));
        _pageLabel.text = $"{_page + 1}/{_pageCount}";
    }

    public void NextPage()
    {
        if (!_cardInserted)
            return;
        _page ++;

        if (_page == _pageCount)
            _page = 0;

        OpenPage(_page);
    }

    public void PreviousPage()
    {
        if (!_cardInserted)
            return;
        _page --;

        if (_page == -1)
            _page = _pageCount - 1;

        OpenPage(_page);
    }

    public void ToggleLight()
    {
        _light = !_light;
        _screenImage.color = _light ? _highlitedScreenColor : _normalScreenColor;
    }

    public void ClearText()
    {
        _head.text = "";
        _containment.text = "";
        _pageLabel.text = "";
    }

    public void CardInserted()
    {
        _cardInserted = true;
        OpenPage(0);
    }

    public void CardSpriteSwitch()
    {
        _cardImage.sprite = _nextCardSprite;
    }

    [System.Serializable]
    public struct EBookCard
    {
        public string bookName;
        public Sprite cardSprite;
    }
}
