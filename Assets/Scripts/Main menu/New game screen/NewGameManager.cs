using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Main_menu.New_game_screen
{
    public class NewGameManager : MonoBehaviour
    {
        //components
        [SerializeField] private List<GameObject> models;
        [SerializeField] private GameObject uiObject;
        private MyMenuInputManager _myMenuInputManager;
        private TMP_InputField _inputField;

        //variables
        private int _modelIndex;
        private string _name;

        //lists
        private List<List<Renderer>> _renderers;

        private void Awake()
        {
            _myMenuInputManager = FindObjectOfType<MyMenuInputManager>();
            _inputField = uiObject.GetComponent<TMP_InputField>();
            _renderers = new List<List<Renderer>>();
        }

        private void Start()
        {
            _modelIndex = 1;
            GetRenderers();
            HideModels();
            HideUI();
            ActivateModel(_modelIndex);
            _inputField.onValueChanged.AddListener(delegate(string arg0) { OnValueChanged(arg0); });
            _inputField.onSelect.AddListener(delegate { OnSelect(); });
            _inputField.onDeselect.AddListener(delegate { OnDeselect(); });
        }


        /// <summary>
        /// Show next model and update current model index
        /// </summary>
        public void ShowNext()
        {
            DeactivateModel(_modelIndex);
            _modelIndex = (_modelIndex + 1) % models.Count;
            ActivateModel(_modelIndex);
        }

        /// <summary>
        /// Show previous model and update current model index
        /// </summary>
        public void ShowPrev()
        {
            DeactivateModel(_modelIndex);
            _modelIndex = _modelIndex - 1 < 0 ? models.Count - 1 : _modelIndex - 1;
            ActivateModel(_modelIndex);
        }

        public void ShowUI() => uiObject.SetActive(true);

        public void HideUI() => uiObject.SetActive(false);

        private void HideModels()
        {
            for (int i = 0; i < models.Count; i++)
                DeactivateModel(i);
        }

        private void GetRenderers()
        {
            foreach (var model in models)
            {
                List<Renderer> auxRenderers = new List<Renderer>(model.GetComponentsInChildren<Renderer>());
                _renderers.Add(auxRenderers);
            }
        }

        private void OnValueChanged(string value) => _name = value;

        /// <summary>
        /// avoids un wanted changes of model
        /// </summary>
        private void OnSelect() => _myMenuInputManager.SetInputsEnabled(false);

        /// <summary>
        /// avoids un wanted changes of model
        /// </summary>
        private void OnDeselect() => _myMenuInputManager.SetInputsEnabled(true);


        private void DeactivateModel(int index)
        {
            foreach (var renderer in _renderers[index])
                renderer.enabled = false;
        }

        private void ActivateModel(int index)
        {
            foreach (var renderer in _renderers[index])
                renderer.enabled = true;
        }

        public int GetModelIndex()
        {
            return _modelIndex;
        }

        public string GetName()
        {
            return _name;
        }
    }
}