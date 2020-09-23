using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{

    public Scene target;

    void OnTriggerEnter(Collider other) {
        SceneManager.LoadScene(target.name);
    }
}
