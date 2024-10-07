using System.Collections;
using UnityEngine;

public class ScalePlane : MonoBehaviour
{
    float gameScreenWidht;
    float gameScreenHeight;


    private void Start()
    {
        StartCoroutine(BackgroundScale());
    }



    IEnumerator BackgroundScale()
    {
        while (true)
        {
            float screenWidthInPixels = Screen.width;
            float screenHeightInPixels = Screen.height;


            if (screenWidthInPixels != gameScreenWidht || screenHeightInPixels != gameScreenHeight)
            {
                gameScreenWidht = screenWidthInPixels;
                gameScreenHeight = screenHeightInPixels;

                // Manually calculate the aspect ratio
                float aspectRatio = screenWidthInPixels / screenHeightInPixels;

                // Calculate the screen height in world units
                float screenHeight = Camera.main.orthographicSize * 2f / 10f;

                // Calculate the screen width in world units using the manually calculated aspect ratio
                float screenWidth = screenHeight * aspectRatio;

                yield return new WaitForSeconds(.1f); // wait for screen size to be updated avoiding error ocurration in the <<<<<EDITOR ONLY>>>>>

                transform.localScale = new Vector3(screenWidth, 1f, screenHeight);
            }
            yield return null;
        }
    }

}
