using UnityEngine;

namespace Demo.Enemy
{
    public class EnemySightSensor : MonoBehaviour
    {
        private GameObject enemyMutant; // Reference to the EnemyMutant GameObject
        public GameObject playerCharacter; // Reference to the PlayerCharacter GameObject

        public EnemySightSensor(GameObject enemyMutant, GameObject playerCharacter)
        {
            this.enemyMutant = enemyMutant;
            this.playerCharacter = playerCharacter;
        }

        // Check if the PlayerCharacter is within 10 units of the EnemyMutant
        public bool Ping()
        {
            if (enemyMutant != null && playerCharacter != null)
            {
                float distance = Vector3.Distance(enemyMutant.transform.position, playerCharacter.transform.position);
                return distance <= 10f;
            }
            else
            {
                Debug.LogWarning("EnemyMutant or PlayerCharacter GameObject is not assigned.");
                return false;
            }
        }
    }
}
