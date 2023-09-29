using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Main_menu.New_game_screen
{
    public class NewGameManager : MonoBehaviour
    {
        //components
        [SerializeField] private List<GameObject> models;
        [SerializeField] private CanvasGroup uiObject,playButtonCanvas;
        [SerializeField] private Button playButton;
        private MyMenuInputManager _myMenuInputManager;
        [SerializeField] private TMP_InputField _inputField;
        private LoadScreen _loadScreen;
        private JSONsaving _jsoNsaving;

        //variables
        private int _modelIndex;
        private string _name;

        //lists
        private List<List<Renderer>> _renderers;

        private MainMenuSounds _sounds;

        private void Awake()
        {
            _sounds = FindObjectOfType<MainMenuSounds>();
            _jsoNsaving = FindObjectOfType<JSONsaving>();
            _myMenuInputManager = FindObjectOfType<MyMenuInputManager>();
            _loadScreen = FindObjectOfType<LoadScreen>();
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
            playButton.onClick.AddListener(PlayButtonAction);
            HidePlayButton();
        }


        /// <summary>
        /// Show next model and update current model index
        /// </summary>
        public void ShowNext()
        {
            _sounds.SelectOptionSound();
            DeactivateModel(_modelIndex);
            _modelIndex = (_modelIndex + 1) % models.Count;
            ActivateModel(_modelIndex);
        }

        /// <summary>
        /// Show previous model and update current model index
        /// </summary>
        public void ShowPrev()
        {
            _sounds.SelectOptionSound();
            DeactivateModel(_modelIndex);
            _modelIndex = _modelIndex - 1 < 0 ? models.Count - 1 : _modelIndex - 1;
            ActivateModel(_modelIndex);
        }

        public void PlayButtonAction()
        {
            _sounds.SelectOptionSound();
            _jsoNsaving._saveData.StartNewGame(GetModelIndex(), GetName());
            _jsoNsaving.SaveTheData();
            _loadScreen.LoadLevels();
            playButton.interactable = false;
        }

        public void ShowUI()
        {
            uiObject.alpha = 1;
            uiObject.blocksRaycasts = true;
            uiObject.interactable = true;
        }

        public void HideUI()
        {
            uiObject.alpha = 0;
            uiObject.interactable = false;
            uiObject.blocksRaycasts = false;
        }
        public void ShowPlayButton()
        {
            playButtonCanvas.alpha = 1;
            playButtonCanvas.interactable = true;
            playButtonCanvas.blocksRaycasts = true;
        }
        public void HidePlayButton()
        {
            playButtonCanvas.alpha = 0;
            playButtonCanvas.interactable = false;
            playButtonCanvas.blocksRaycasts = false;
        }
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
        /// avoids unwanted changes of model
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