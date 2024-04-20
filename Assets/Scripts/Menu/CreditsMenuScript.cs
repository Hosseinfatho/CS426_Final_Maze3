using UnityEngine;
using UnityEngine.UI;

public class CreditsMenuScript : MonoBehaviour
{
    [SerializeField] float scrollSpeedDown = 0.1f;
    [SerializeField] float scrollSpeedBackToTop = 1f;
    [SerializeField] float waitAtTheTop = 1;
    [SerializeField] float waitAtTheBottom = 3;

    float timeToWait; //keep time from which to wait before starting to scroll
    ScrollRect scrollRect;
    bool direction;

    void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
        resetCreditsMenu();
    }

    // also call this when opening the credits menu
    public void resetCreditsMenu()
    {
        // this function is called when credits button is pressed, but start wasn't called yet b/c this was disabled previously
        // so to prevent null exception we got this line
        if (scrollRect == null) return;

        scrollRect.verticalNormalizedPosition = 1; //scroll all the way to the top
        timeToWait = Time.fixedTime;
        direction = true;
    }

    void Update()
    {
        if ((direction && Time.fixedTime - timeToWait > waitAtTheTop) || (!direction && Time.fixedTime - timeToWait > waitAtTheBottom))
        {
            if (direction)
                scrollRect.verticalNormalizedPosition -= scrollSpeedDown * Time.deltaTime;
            else
                scrollRect.verticalNormalizedPosition += scrollSpeedBackToTop * Time.deltaTime;

            //if reached end, reverse direction and note the time
            if (scrollRect.verticalNormalizedPosition <= 0 || scrollRect.verticalNormalizedPosition >= 1)
            {
                direction = !direction;
                timeToWait = Time.fixedTime;

                // without this we might get stuck when value is over 1 (and direction is changed each update)
                if (scrollRect.verticalNormalizedPosition < 0)
                    scrollRect.verticalNormalizedPosition = 0;
                if (scrollRect.verticalNormalizedPosition > 1)
                    scrollRect.verticalNormalizedPosition = 1;
            }
        }
    }
}
