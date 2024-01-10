using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReelIconPrefab : MonoBehaviour
{
    public int id = 0;
    public bool isWild = false;
    public float[] modifier = new float[] {0,0,5,25,100}; 
    public bool isAnimating = false;
    public float spawnRate = 25;
    public void SetId(int ID) {
        id = ID;
    }
    public virtual IEnumerator StartAnimation() {
        isAnimating = true;
        Vector3 startScale = transform.localScale;
        Vector3 startEulers = transform.localEulerAngles;
        float timer = 0;
        while(isAnimating) {
            timer += Time.deltaTime;
            float lerp =  (Mathf.Sin(timer * Main.instance.gameSettings.animationScalingSpeed) + 1) /2.0f;
            float scaleAmount = Mathf.Lerp(.8f,1.2f, lerp);
            transform.localScale = startScale * scaleAmount;
            transform.Rotate(Vector3.forward, 90 * Time.deltaTime);
            yield return null;
        }
        transform.localScale = startScale;
        transform.localEulerAngles = startEulers;
    }

    public virtual void StopAnimation() {
        isAnimating = false;
    }

    public virtual void RestartAnimation() {

    }
}
