using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Options : MonoBehaviour 
{
    public AudioMixer audioMixter;
    public Slider slider;
    public TMP_Dropdown dropbox;
    public Toggle toggle;

    public float volumevalue;
    public int graphicsvalue;
    public bool fullscreenvalue;

    private void Start()
    {
        slider.value = PlayerPrefs.GetFloat("volume");
        dropbox.value = PlayerPrefs.GetInt("graphics");
        toggle.enabled = PlayerPrefs.GetInt("fullscreen") == 1 ? true : false ;

    }
    private void Update()
    {
        audioMixter.SetFloat("volume", volumevalue);
        QualitySettings.SetQualityLevel(graphicsvalue);
        Screen.fullScreen = fullscreenvalue;
        PlayerPrefs.SetFloat("volume", volumevalue);
        PlayerPrefs.SetInt("graphics", graphicsvalue);
        PlayerPrefs.SetInt("fullscreen", fullscreenvalue ? 1 : 0);
    }
    public void setvolumen(float volumen)
    {
        volumevalue = volumen;

    }

    public void setquality(int qualityindex)
    {
        graphicsvalue = qualityindex;

    }

    public void setfullscreen(bool fullscreen)
    {

        fullscreenvalue = fullscreen;

    }
    public void back()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
