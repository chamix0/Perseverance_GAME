using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeeFight : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Slider slider;
    private DoorManager doorManager;
    private PlayerValues playerValues;
    private EnemyShooterZone enemyShooterZone;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
}