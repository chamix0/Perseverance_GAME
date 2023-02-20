using UnityEngine;
using UnityEngine.SceneManagement;

namespace General_Inputs
{
    public class SceneController : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void LoadTestScene()
        {
            SceneManager.LoadScene(1);
        }
    }
}
