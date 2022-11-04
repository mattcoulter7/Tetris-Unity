using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisShadow : MonoBehaviour
{
    private TetrisPiece tetrisPiece;
    private Renderer[] shadowRenderers;
    
    // Start is called before the first frame update
    void Start()
    {
        tetrisPiece = GetComponentInParent<TetrisPiece>();
        shadowRenderers = GetComponentsInChildren<Renderer>();
    }

    private void ToggleShadows(bool visible)
    {
        foreach (Renderer r in shadowRenderers)
        {
            r.enabled = visible;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Rect rect = tetrisPiece.GetRect();

        int shadowDistance = tetrisPiece.GetShadowDistance();
        //ToggleShadows(shadowDistance > rect.height);
        transform.position = tetrisPiece.transform.position - (Vector3.up * shadowDistance);
    }

    public void OnPlant()
    {
        Destroy(gameObject);
    }
}
