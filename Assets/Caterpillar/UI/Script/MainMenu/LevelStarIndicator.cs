using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStarIndicator : MonoBehaviour
{
    public Transform CameraToLook;
    private MapButton Button;

    public SpriteRenderer Star1_Off;
    public SpriteRenderer Star1_On;
    public SpriteRenderer Star2_Off;
    public SpriteRenderer Star2_On;
    public SpriteRenderer Star3_Off;
    public SpriteRenderer Star3_On;

    private void Start()
    {
        Button = GetComponentInParent<MapButton>();
        UpdateStars();
    }

    // Update is called once per frame
    void Update()
    {
        transform.forward = -CameraToLook.forward;
    }

    void UpdateStars()
    {
        int Stars = SaveGame.GetStars(Button.LevelID);
        Star1_Off.enabled = (Stars < 1);
        Star2_Off.enabled = (Stars < 2);
        Star3_Off.enabled = (Stars < 3);
        Star1_On.enabled = (Stars >= 1);
        Star2_On.enabled = (Stars >= 2);
        Star3_On.enabled = (Stars == 3);
    }
}
