using UnityEngine;
using System.Collections;

public class DestroyTimer : MonoBehaviour {
    
    //Object Pooling 
    public bool UseObjectPooling = false;
    private GameObject ActualDestroyFX;

    //Settings
	public float DestroyTime = 1.0f;
    public GameObject DestroyFX;
    public Transform DestroyFXPos;
	private float Timer = 0.0f;

    public bool Fade = false;
    public Renderer FadeMaterial;
    public AnimationCurve FadeCurve;

	
    private void Update () {
		Timer += Time.deltaTime;
        if (Fade && FadeMaterial)
        {
            float FadeFloat = FadeCurve.Evaluate(Timer / DestroyTime);
            FadeMaterial.material.SetColor("_Color", new Vector4(1.0f, 1.0f, 1.0f, FadeFloat));
        }

        if (Timer > DestroyTime) 
            DestroyThis();

    }

    private void DestroyThis()
    {
        if (!UseObjectPooling)
        {
            if (DestroyFX && DestroyFXPos)
                Instantiate(DestroyFX, DestroyFXPos.position, Quaternion.identity);

            DestroyImmediate(this.gameObject);
        }
        else
        {
            if (DestroyFX && DestroyFXPos)
            {
                ActualDestroyFX.SetActive(true);
                ActualDestroyFX.transform.position = DestroyFXPos.position;
            }

            this.gameObject.SetActive(false); 
        }
    }
    
           
}
