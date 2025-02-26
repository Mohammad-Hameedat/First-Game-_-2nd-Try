using System;
using UnityEngine;
using UnityEngine.UI;

public class HungerArrow : MonoBehaviour
{
    public float colorMaxValue;

    public Gradient gradient;

    public Image imageFill;


    public void RevertToInitialColor()
    {
        imageFill.color = gradient.Evaluate(0f);
    }


    public void UpdateColorBasedOnHunger(float currentColorValue)
    {
        double currentColorPercentage = Math.Round(currentColorValue / colorMaxValue, 2);

        //Debug.Log("Health Percentage: " + healthPercentage);

        imageFill.color = gradient.Evaluate((float) currentColorPercentage);
    }
}

/*
using System;
using UnityEngine;
using UnityEngine.UI;

public class HungerArrow : MonoBehaviour
{
    private float elapsedTime;
    public float maxHealth;

    public Gradient gradient;

    public Image imageFill;



    private void OnEnable()
    {
        elapsedTime = maxHealth;
        SetInitialColor();
    }

    private void Update()
    {
        elapsedTime -= Time.deltaTime;

        SetColor(elapsedTime);
    }


    public void SetInitialColor()
    {
        imageFill.color = gradient.Evaluate(1f);
    }


    public void SetColor(float health)
    {
        double healthPercentage = Math.Round(health / maxHealth, 2);

        imageFill.color = gradient.Evaluate((float) healthPercentage);
    }

    private void OnDisable()
    {
        SetInitialColor();
    }
}
*/