using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.Video;
using UnityEngine.InputSystem;
using Microsoft.MixedReality.Toolkit.UI;

// UnityWebRequest.Get example

// Access a website and use UnityWebRequest.Get to download a page.
// Also try to download a non-existing page. Display the error.

public class HapticTrigger : MonoBehaviour
{
    public string ipaddress;
    public float hapticWaittime = 5f, audioWaitTime = 2f;
    public InputAction stopPlayback, editSystem, lockSystem, testActuator, appQuit;

    private IEnumerator ht, at;

    void OnEnable()
    {
        stopPlayback.Enable();
        editSystem.Enable();
        lockSystem.Enable();
        testActuator.Enable();
        appQuit.Enable();
    }

    void OnDisable()
    {
        stopPlayback.Disable();
        editSystem.Disable();
        lockSystem.Disable();
        testActuator.Disable();
        appQuit.Disable();
    }

    void Awake()
    {
        stopPlayback.performed += context =>
        {
            this.GetComponent<VideoPlayer>().Stop();
            this.GetComponent<AudioSource>().enabled = false;
        };
        editSystem.performed += context =>
        {
            this.GetComponent<ObjectManipulator>().enabled = true;
        };
        lockSystem.performed += context =>
        {
            this.GetComponent<ObjectManipulator>().enabled = false;
        };
        testActuator.performed += context =>
        {
            StartCoroutine(GetRequest(ipaddress, 0f));
        };
        appQuit.performed += context =>
        {
            Application.Quit();
        };
    }


    void Start()
    {
        // A correct website page.
        //StartCoroutine(GetRequest("https://www.example.com"));

        // A non-existing page.
        //StartCoroutine(GetRequest(ipaddress));
        StartCoroutine(EnableAudio());
    }
    IEnumerator EnableAudio()
    {
        yield return new WaitForSeconds(audioWaitTime);
        this.GetComponent<AudioSource>().enabled = true;
        yield return null;
    }
    IEnumerator GetRequest(string uri)
    {
        yield return new WaitForSeconds(hapticWaittime);
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    break;
            }
        }
    }

    IEnumerator GetRequest(string uri, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    break;
            }
        }
    }

    public void Restart()
    {
        this.GetComponent<VideoPlayer>().Stop();
        this.GetComponent<AudioSource>().enabled = false;
        this.GetComponent<VideoPlayer>().Play();
        ht = GetRequest(ipaddress);
        StartCoroutine(ht);
        at = EnableAudio();
        StartCoroutine(at);
    }

    public void Stop()
    {
        StopCoroutine(ht);
        StopCoroutine(at);
        this.GetComponent<VideoPlayer>().Stop();
        this.GetComponent<AudioSource>().enabled = false;
    }
}
