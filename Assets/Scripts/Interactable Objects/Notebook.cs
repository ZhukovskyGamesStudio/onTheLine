using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Notebook : InteractableObject {
    public static Notebook instance;

    [SerializeField]
    private List<string> _alreadyLines;

    [SerializeField]
    private Button _linePrefab;

    [SerializeField]
    private Commutator _commutator;

    [SerializeField]
    private VerticalLayoutGroup _linesContainer;

    private List<Button> _sayLines = new();
    private List<string> _phrazes = new();
    private bool _isTaken;
    private MovingBetweenTwoPointObject _mbtp;

    private void Awake() {
        instance = this;
        _mbtp = GetComponent<MovingBetweenTwoPointObject>();
    }

    private void Start() {
        AddLine("Добрый день, ТС Белокаменного района", PersonBehindHole.OPERATOR_HELLO);
        AddLine("Связь установлена", PersonBehindHole.OPERATOR_CONNECTION_OK);
        AddLine("Повторите, пожалуйста.", "/repeat/");
        foreach (var VARIABLE in _alreadyLines) {
            AddLine(VARIABLE);
        }
    }

    public override void OnmouseDown() {
        _isTaken = !_isTaken;
        if (_isTaken)
            _mbtp.Take();
        else
            _mbtp.Put();
    }

    protected override void Update() {
        base.Update();
        if (Input.GetKeyDown(KeyCode.T)) {
            _isTaken = !_isTaken;
            if (_isTaken)
                _mbtp.Take();
            else
                _mbtp.Put();
        }
    }

    private void SetInteractable(bool isOn) {
        foreach (Button lineButton in _sayLines) {
            lineButton.transform.GetComponentInChildren<MeshCollider>().enabled = isOn;
            lineButton.interactable = isOn;
        }
    }

    public void AddLine(string line, string thought = "") {
        if (thought == "") {
            thought = line;
        }

        if (_phrazes.Contains(thought)) {
            return;
        }

        _phrazes.Add(thought);
        Button newButton = Instantiate(_linePrefab.gameObject, _linesContainer.transform).GetComponent<Button>();
        newButton.GetComponent<Text>().text = line;
        newButton.gameObject.SetActive(true);
        newButton.name = (_phrazes.Count - 1).ToString();
        newButton.onClick.AddListener(delegate {
            int number = _phrazes.Count - 1;
            SelectPhraze(newButton.name);
        });
        newButton.gameObject.SetActive(true);
        newButton.interactable = _isTaken;
        _sayLines.Add(newButton);
        _linesContainer.CalculateLayoutInputVertical();
    }

    private void SelectPhraze(string buttonNameIndex) {
        _commutator.PassSoundFromOperator(_phrazes[int.Parse(buttonNameIndex)]);
        GameObject myEventSystem = GameObject.Find("EventSystem");
        myEventSystem .GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
        SetInteractable(false);
        _isTaken = false;
        _mbtp.Put();
    }
}