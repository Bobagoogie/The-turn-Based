using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyList : MonoBehaviour
{
    public GameObject fightTextObject; // To display fight information
    public Enemies enemies = Enemies.Slime;
    public enum Enemies { Slime, Zombie, Skeleton, Dragon }

    // Dictionary to store max health values for each enemy type
    private Dictionary<Enemies, int> maxHealths;

    // The actual health of the enemy
    public int currentHealth;
    public int maxHealth;

    // Start is called before the first frame update
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log($"{gameObject.name} ({enemies}) collided with Player. Current Health: {currentHealth}");

            // Example action: Move the enemy
            transform.position = new Vector3(-2004f, 0f, 4f);
              transform.rotation = Quaternion.Euler(0f, 135f, 0f);
            // Return health value for possible further use
            Debug.Log($"Returning health on collision: {currentHealth}");
        }
    }
}