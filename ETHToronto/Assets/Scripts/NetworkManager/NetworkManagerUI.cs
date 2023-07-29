using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button serverBtn;
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button clientBtn;

    [SerializeField] private GameObject mainCam;


    private void Awake() {
        serverBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartServer();
            mainCam.SetActive(false);

        });
        hostBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
            mainCam.SetActive(false);

        });
        clientBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
            mainCam.SetActive(false);

        });
    }
}
