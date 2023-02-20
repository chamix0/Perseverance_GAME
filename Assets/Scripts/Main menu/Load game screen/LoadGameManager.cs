using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Main_menu.Load_game_screen
{
    [DefaultExecutionOrder(1)]
    public class LoadGameManager : MonoBehaviour
    {
        #region DATA

        //components
        private MyMenuInputManager _myInputManager;
        private JSONsaving _jsoNsaving;
        private SaveData _saveData;
        private MainMenuManager _menuManager;
        [SerializeField] private GameObject uiGameObject;
        [SerializeField] private Toggle loadFileTogle;
        [SerializeField] private Toggle eraseFileTogle;

        //variables
        private bool _load = true; //to check wether it has to load a file or delete 
        private int _slotIndex;
        private Color _orColor;

        //lists
        [SerializeField] private List<Button> _buttons;
        private List<List<TMP_Text>> _texts;
        private List<Image> _backgrounds;

        #endregion

        private void Awake()
        {
            _jsoNsaving = FindObjectOfType<JSONsaving>();
            _menuManager = FindObjectOfType<MainMenuManager>();
            _saveData = _jsoNsaving._saveData;
            _texts = new List<List<TMP_Text>>();
            _backgrounds = new List<Image>();
        }

        void Start()
        {
            _slotIndex = 0;
            SetText();
            HideUI();
            SetButtonListeners();
            loadFileTogle.isOn = true;
            eraseFileTogle.isOn = false;
            loadFileTogle.interactable = false;
        }

        /// <summary>
        /// If load is set, it sets the save data file index of the current game to the index of the button played and starts the game.
        /// but if erase is set (load=false) it will clear that game  file. 
        /// </summary>
        /// <param name="index">index of the button and the slot</param>
        private void Click(int index)
        {
            if (_load)
            {
                if (_saveData.GetGameData(index).GetGameStarted())
                {
                    _saveData.SetLastSessionSlotIndex(index);
                    //cambiar de escena
                    print("cambio de escena");
                }
            }
            else
            {
                _saveData.EraseSlot(index);
                _jsoNsaving.SaveTheData();
                _menuManager.CheckForContinueAndNewGame();
            }

            UpdateText();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        /// <summary>
        /// If load is set, it sets the save data file index of the current game to the index of the button played and starts the game.
        /// but if erase is set (load=false) it will clear that game  file. 
        /// </summary>
        public void PressEnter()
        {
            if (_load)
            {
                if (_saveData.GetGameData(_slotIndex).GetGameStarted())
                {
                    _saveData.SetLastSessionSlotIndex(_slotIndex);
                    //cambiar de escena
                    print("cambio de escena");
                }
            }
            else
            {
                _saveData.EraseSlot(_slotIndex);
                _jsoNsaving.SaveTheData();
                _menuManager.CheckForContinueAndNewGame();
            }

            UpdateText();
        }

        /// <summary>
        ///  Enable load mode and update the toggles.
        /// </summary>
        public void EnableLoad()
        {
            _load = true;
            eraseFileTogle.isOn = false;
            loadFileTogle.isOn = true;
            loadFileTogle.interactable = false;
            eraseFileTogle.interactable = true;
        }

        /// <summary>
        ///  Enable erase mode and update the toggles.
        /// </summary>
        public void EnableErase()
        {
            _load = false;
            eraseFileTogle.isOn = true;
            loadFileTogle.isOn = false;
            loadFileTogle.interactable = true;
            eraseFileTogle.interactable = false;
        }

        /// <summary>
        /// Sets the texts corresponding to each slot.
        /// </summary>
        private void SetText()
        {
            foreach (var button in _buttons)
            {
                _backgrounds.Add(button.gameObject.transform.Find("Image").GetComponent<Image>());
                List<TMP_Text> textsList = new List<TMP_Text>
                {
                    button.gameObject.transform.Find("run name").GetComponent<TMP_Text>(),
                    button.gameObject.transform.Find("last time played text").GetComponent<TMP_Text>(),
                    button.gameObject.transform.Find("total time played").GetComponent<TMP_Text>(),
                    button.gameObject.transform.Find("slot text").GetComponent<TMP_Text>()
                };
                _texts.Add(textsList);
            }

            _orColor = _texts[0][0].color;
        }

        /// <summary>
        /// Updates the slot info with the save data info of that slot.
        /// </summary>
        public void UpdateText()
        {
            for (int i = 0; i < _buttons.Count; i++)
            {
                GameData gameData = _saveData.GetGameData(i);
                _texts[i][0].SetText(gameData.GetName());
                _texts[i][1].SetText(gameData.GetLastTimePlayed());
                _texts[i][2].SetText(gameData.getTotalTime());
            }
        }

        public void HideUI() => uiGameObject.SetActive(false);

        public void ShowUI() => uiGameObject.SetActive(true);


        public void SelectNextButton()
        {
            int aux = _slotIndex;
            aux = (aux + 1) % _buttons.Count;
            _slotIndex = aux;
            UpdateColors();
        }

        public void SelectPrevButton()
        {
            int aux = _slotIndex;
            aux = aux - 1 < 0 ? _buttons.Count - 1 : aux - 1;
            _slotIndex = aux;
            UpdateColors();
        }

        /// <summary>
        /// Highlight the selected slot.
        /// </summary>
        public void UpdateColors()
        {
            for (int i = 0; i < _buttons.Count; i++)
            {
                foreach (var text in _texts[i])
                    text.color = Color.black;

                if (i != _slotIndex)
                {
                    foreach (var text in _texts[i])
                        text.color = _orColor;
                    _backgrounds[i].enabled = false;
                }
                else
                {
                    foreach (var text in _texts[i])
                        text.color = Color.white;
                    _backgrounds[i].enabled = true;
                }
            }
        }

        private void SetButtonListeners()
        {
            for (int i = 0; i < _buttons.Count; i++)
            {
                int aux = i;
                _buttons[i].onClick.AddListener(delegate { Click(aux); });
            }

            loadFileTogle.onValueChanged.AddListener(delegate(bool arg0)
            {
                if (arg0) EnableLoad();
            });
            eraseFileTogle.onValueChanged.AddListener(delegate(bool arg0)
            {
                if (arg0)
                {
                    EnableErase();
                }
            });
        }
    }
}