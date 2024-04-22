using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIShader : MonoBehaviour
{
    [SerializeField] EnemyAI enemyAIScript;
    [SerializeField] Material glowMaterial;
    [SerializeField] GameObject objectToChangeMaterial;

    Renderer objectToChangeMaterialRenderer;
    float enemyBlinkTime = 0;
    Material standardMaterial;

    void Start()
    {
        objectToChangeMaterialRenderer = objectToChangeMaterial.GetComponent<Renderer>();
        standardMaterial = objectToChangeMaterialRenderer.material;
    }

    void Update()
    {
        if (enemyAIScript.playerInSight())
        {
            if (Time.fixedTime - enemyBlinkTime > 0.5)
            {
                if (objectToChangeMaterialRenderer.material != standardMaterial)
                    objectToChangeMaterialRenderer.material = standardMaterial;
                else
                    objectToChangeMaterialRenderer.material = glowMaterial;

                enemyBlinkTime = Time.fixedTime;
            }
        }
        else if (objectToChangeMaterialRenderer.material != standardMaterial)
            objectToChangeMaterialRenderer.material = standardMaterial;

    }
}
