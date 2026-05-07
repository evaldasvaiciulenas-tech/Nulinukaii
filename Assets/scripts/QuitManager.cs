using UnityEngine;

public class QuitManager : MonoBehaviour
{
    public GameObject quitDialog;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            quitDialog.SetActive(true);
        }
    }

    public void ShowQuitDialog()
{
    quitDialog.SetActive(true);
}

    public void Confirm()
    {
        Application.Quit();
    }

    public void Cancel()
    {
        quitDialog.SetActive(false);
    }


}