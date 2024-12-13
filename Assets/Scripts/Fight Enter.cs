using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // To reload the scene or quit the game

public class FightEnter : MonoBehaviour
{
    private GameObject currentEnemyObject;
        public Slider playerHealthBar; 
    public Slider enemyHealthBar;
    public bool enoughTime;
    public float playerhealth = 100f;
    public int currentplayerhealth;
    public string targetTag = "Enemy";
    public float movementSpeed = 15f;
    public float turnSpeed = 150f;
    public float jumpForce = 10f;

    public GameObject cameraController;
    private CameraController cameraScript;
    public Transform fightFieldCameraPosition;
    public GameObject fightTextObject;
    public GameObject enemyObject;
    public GameObject gameOverUI; // Reference to the Game Over UI
    public GameObject restartButton;
    public GameObject quitButton;

    private int currentEnemyHealth;
    private int maxEnemyHealth;
    private Vector3 initialPosition;
    private string enemyName;

    private Dictionary<string, int> enemyHealths = new Dictionary<string, int>
    {
        { "Slime", 50 },
        { "Zombie", 100 },
        { "Skeleton", 80 },
        { "Dragon", 200 }
    };

    private Dictionary<string, int[]> enemyDamageRanges = new Dictionary<string, int[]>
    {
        { "Slime", new int[] { 5, 10 } },
        { "Zombie", new int[] { 10, 15 } },
        { "Skeleton", new int[] { 10, 20 } },
        { "Dragon", new int[] { 15, 20 } }
    };

    void Start()
    {
        fightTextObject.SetActive(false);
        gameOverUI.SetActive(false); // Ensure Game Over screen is hidden at start
        restartButton.SetActive(false);
        quitButton.SetActive(false);
        currentplayerhealth = (int)playerhealth;
        enoughTime = true;
        enemyHealthBar.gameObject.SetActive(false);
        UpdatePlayerHealthBar();

        if (cameraController != null)
        {
            cameraScript = cameraController.GetComponent<CameraController>();
        }
    }

    void Update()
    {
        // Basic movement
        if (Input.GetKey(KeyCode.W)) transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.S)) transform.Translate(-Vector3.forward * movementSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.D)) transform.Rotate(Vector3.up * turnSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.A)) transform.Rotate(-Vector3.up * turnSpeed * Time.deltaTime);

        // Player actions
        if (enoughTime)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) { RiskyAttack(); }
            if (Input.GetKeyDown(KeyCode.Alpha2)) { SafeAttack(); }
            if (Input.GetKeyDown(KeyCode.Alpha3)) { RiskyHeal(); }
            if (Input.GetKeyDown(KeyCode.Alpha4)) { SafeHeal(); }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(targetTag))
        {
            initialPosition = transform.position;
            currentEnemyObject = collision.gameObject;
            enemyName = collision.gameObject.name;

            if (enemyHealths.TryGetValue(enemyName, out maxEnemyHealth))
            {
                currentEnemyHealth = maxEnemyHealth;
                Debug.Log($"{enemyName} encountered! Max Health: {maxEnemyHealth}");
            }
            else
            {
                Debug.LogError($"Health for {enemyName} not found.");
                return;
            }
            enemyHealthBar.gameObject.SetActive(true);
            fightTextObject.SetActive(true);
            movementSpeed = 0;
            Debug.Log("Fight mode initiated!");

            StartCoroutine(DelayedMoveToFightField());
            
        }
    }

    private IEnumerator DelayedMoveToFightField()
    {
        UpdateEnemyHealthBar();
        yield return new WaitForSeconds(3f);

        EndMovementPlusMoveToFightField();
        UpdatePlayerHealthBar();
        
        if (cameraScript != null)
        {
            cameraScript.followPlayer = false;
            if (fightFieldCameraPosition != null)
            {
                Camera.main.transform.position = fightFieldCameraPosition.position;
                Camera.main.transform.rotation = fightFieldCameraPosition.rotation;
            }
        }
    }

    void EndMovementPlusMoveToFightField()
    {
        transform.position = new Vector3(-2000f, 0.5f, 0f);
        turnSpeed = 0f;
        fightTextObject.SetActive(false);
    }

    public void RiskyAttack()
    {
        if (currentEnemyHealth > 0)
        {
            if (enoughTime == true){
            if (Random.value > 0.5f)
            {
                int damage = Random.Range(15, 30);
                currentEnemyHealth -= damage;
                Debug.Log($"RiskyAttack succeeded! Dealt {damage} damage.");
            }
            else
            {
                Debug.Log("RiskyAttack failed!");
            }
            UpdateEnemyHealthBar();
            CheckEnemyHealth();
        }
        }
    }

    public void SafeAttack()
    {
        if (enoughTime == true){
        if (currentEnemyHealth > 0)
        {
            int damage = Random.Range(5, 10);
            currentEnemyHealth -= damage;
            Debug.Log($"SafeAttack succeeded! Dealt {damage} damage.");
            enoughTime = false;
            UpdateEnemyHealthBar();
            CheckEnemyHealth();
        }
        }
    }

    public void RiskyHeal()
    {
        if (enoughTime == true){
        if (currentEnemyHealth > 0)
        {
            if (Random.value > 0.5f)
            {
                int heal = Random.Range(30, 50);
                currentplayerhealth += heal;
                currentplayerhealth = Mathf.Min(currentplayerhealth, (int)playerhealth);
                Debug.Log($"RiskyHeal succeeded! Restored {heal} health.");
            }
            else
            {
                Debug.Log("RiskyHeal failed!");
            }
            enoughTime = false;
            UpdatePlayerHealthBar();
            CheckEnemyHealth();
        }
        }
    }

    public void SafeHeal()
    {
        if (enoughTime == true){
        if (currentEnemyHealth > 0)
        {
            int heal = Random.Range(15, 25);
            currentplayerhealth += heal;
            currentplayerhealth = Mathf.Min(currentplayerhealth, (int)playerhealth);
            Debug.Log($"SafeHeal succeeded! Restored {heal} health.");
            enoughTime = false;
            UpdatePlayerHealthBar();
            CheckEnemyHealth();
        }
        }
    }

    private void CheckEnemyHealth()
    {
        if (currentEnemyHealth <= 0)
        {
            
            enemyHealthBar.gameObject.SetActive(false);
            Debug.Log($"Enemy {enemyName} defeated!");
            currentEnemyHealth = 0;
            fightTextObject.SetActive(false);
            movementSpeed = 15f;
            ReturnToInitialPosition();
            if (currentEnemyObject != null)
            {
                Destroy(currentEnemyObject); // Destroy the enemy GameObject
            }
        }
        else
        {
            StartCoroutine(EnemyAttackWithDelay());
            Debug.Log($"Enemy health: {currentEnemyHealth}");
        }
    }

    private void ReturnToInitialPosition()
    {
        transform.position = initialPosition;
        turnSpeed = 150f;

        if (cameraScript != null)
        {
            cameraScript.ResetCamera();
            cameraScript.followPlayer = true;
        }

        enemyName = null;
        currentEnemyHealth = 0;
        enoughTime = true;

        Debug.Log("Returned to the initial position. Ready for the next fight.");
    }

    public void EnemyAttack(string enemyName)
    {
        if (enemyDamageRanges.TryGetValue(enemyName, out int[] damageRange))
        {
            int damage = Random.Range(damageRange[0], damageRange[1]);
            currentplayerhealth -= damage;
            Debug.Log($"{enemyName} attacked and dealt {damage} damage to the player.");
            UpdatePlayerHealthBar();
            CheckPlayerHealth();
        }
        else
        {
            Debug.LogError($"No damage range found for {enemyName}.");
        }
    }

    

    private void CheckPlayerHealth()
    {
        if (currentplayerhealth <= 0)
        {
            Debug.Log("Player has been defeated!");
            currentplayerhealth = 0;

            // Display Game Over screen
            if (gameOverUI != null)
            {
                gameOverUI.SetActive(true);
                restartButton.SetActive(true);
                quitButton.SetActive(true);
                Time.timeScale = 0f; // Pause the game
            }
        }
        else
        {
            Debug.Log($"Player health: {currentplayerhealth}");
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Resume the game
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload current scene
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit(); // Quit the application
    }

    private void UpdatePlayerHealthBar()
    {
        playerHealthBar.value = currentplayerhealth;
    }

    private void UpdateEnemyHealthBar()
    {
        enemyHealthBar.maxValue = maxEnemyHealth;
        enemyHealthBar.value = currentEnemyHealth;
    }
    private IEnumerator EnemyAttackWithDelay()
    {
        yield return new WaitForSeconds(1f);
        if (enemyName != null)
        {
            EnemyAttack(enemyName);
            enoughTime = true;
        }
    }
}
