using System.Collections;
using UnityEngine;

public class creditsManager : MonoBehaviour
{
    // Start is called before the first frame updat
    private LoadScreen _loadScreen;

    void Start()
    {
        _loadScreen = FindObjectOfType<LoadScreen>();
        StartCoroutine(CreditsCoroutine());
    }



    IEnumerator CreditsCoroutine()
    {
        yield return new WaitForSeconds(6);
        _loadScreen.LoadLevels();
    }
}