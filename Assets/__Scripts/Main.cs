using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    static public Main S; // A singleton for Main
    static Dictionary<eWeaponType, WeaponDefinition> WEAP_DICT;

    [Header("Inscribed")]
    public bool spawnEnemies = true;
    public GameObject[] prefabEnemies; // Array of Enemy prefabs
    public float enemySpawnPerSecond = 0.5f; // # Enemies/second
    public float enemyInsetDefault = 1.5f; // Padding for position
    public float gameRestartDelay = 2;

    public Text uiText;
    public Text scoreCheat;
    public static int score = 0;
    public static int scoreHolder = 0;

    public static Vector3 pos;


    public WeaponDefinition[] weaponDefinitions;
    public GameObject prefabPowerUp;
    public eWeaponType[] powerUpFrequency = new eWeaponType[]
    {
        eWeaponType.blaster,
        eWeaponType.blaster,
        eWeaponType.spread,
        eWeaponType.shield
    };

    // public Text score100;
    // public Text score200;
    // public Text score300;
    // public Text score400;
    // public Text score1000;

    
    private BoundsCheck bndCheck;

 

    void Update()
    {
        uiText.text = "Score: " + score.ToString( "#,0" );
        if (scoreHolder != 0){
        scoreCheat.text = scoreHolder.ToString();

        print(pos);
        scoreCheat.transform.position = new Vector3(pos.x,pos.y, 1);
        
        StartCoroutine(waiter());
        }
        
        
    }

IEnumerator waiter()
{
    yield return new WaitForSeconds(0.5f);
    scoreHolder = 0;
    scoreCheat.text = "";
}
    /// <summary>
    /// Called by an Enemy ship whenever it is destroyed. It sometimes
    /// creates a powerup in place of the ship destroyed
    /// </summary>
    /// <param name="e">The enemy that is destroyed</param>
    static public void SHIP_DESTROYED(Enemy e)
    {
        // Add to score total
        score += e.score;

        scoreHolder = e.score;

        pos = e.transform.position;
       // Text my_text = GameObject.Find("ScoreNumber").GetComponent<Text>();

        

        // Generate a score text at destroyed enemy location
        // if (e.score == 100) {
        // //       GameObject canvas = GameObject.Find("Canvas");
        // // Text tempTextBox = Instantiate(S.score100, e.transform.position, e.transform.rotation) as Text;
        // //     tempTextBox.transform.SetParent(canvas.transform, false);

        // // //Set the text box's text element font size and style:
        // //            tempTextBox.fontSize = 21;
        // //            //Set the text box's text element to the current textToDisplay:
        // //            tempTextBox.transform.position = e.transform.position;
        // //            tempTextBox.text = e.score.ToString();
    


        // }
        // else if (e.score == 200) {

        // }
        // else if (e.score == 400) {
            
        // }
        // else if (e.score == 1000) {
            
        // }

        // Potentially generate a PowerUp
        if (Random.value <= e.powerUpDropChance)
        {
            // Choose which PowerUp to pick
            // Pick one from the possibilities in powerUpFrequency
            int ndx = Random.Range(0, S.powerUpFrequency.Length);
            eWeaponType puType = S.powerUpFrequency[ndx];
            // Spawn a PowerUp
            GameObject go = Instantiate(S.prefabPowerUp) as GameObject;
            PowerUp pu = go.GetComponent<PowerUp>();
            // Set it to the proper WeaponType
            pu.SetType(puType);

            // Set it to the position of the destroyed ship
            pu.transform.position = e.transform.position;

            
        }

        


    }


    private void Awake()
    {
        S = this;
        // Set bndCheck to reference the BoundsCheck component on this GameObject
        bndCheck = GetComponent<BoundsCheck>();

        // Invoke SpawnEnemy() once (in 2 seconds, based on default values)
        Invoke(nameof(SpawnEnemy), 1f / enemySpawnPerSecond);

        // A generic Dictionary with WeaponType as the key
        WEAP_DICT = new Dictionary<eWeaponType, WeaponDefinition>();
        foreach (WeaponDefinition def in weaponDefinitions)
        {
            WEAP_DICT[def.type] = def;
        }
    }

    public void SpawnEnemy()
    {
        if (!spawnEnemies)
        {
            Invoke(nameof(SpawnEnemy), 1f / enemySpawnPerSecond);
            return;
        }

        // Pick a random Enemy prefab to instantiate
        int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate<GameObject>(prefabEnemies[ndx]);

        // Position the Enemy above the screen with a random x position
        float enemyInset = enemyInsetDefault;
        if (go.GetComponent<BoundsCheck>() != null)
        {
            enemyInset = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);
        }

        // Set the initial position for the spawned Enemy
        Vector3 pos = Vector3.zero;
        float xMin = -bndCheck.camWidth + enemyInset;
        float xMax = bndCheck.camWidth - enemyInset;
        pos.x = Random.Range(xMin, xMax);
        pos.y = bndCheck.camHeight + enemyInset;
        go.transform.position = pos;

        // Invoke SpawnEnemy() again
        Invoke(nameof(SpawnEnemy), 1f / enemySpawnPerSecond);
    }

    public void DelayedRestart()
    {
        // Invoke the Restart() method in delay seconds
        Invoke(nameof(Restart), gameRestartDelay);
    }

    public void Restart()
    {
        scoreHolder = 0;
        scoreCheat.text = "";
        score = 0;
        // Reload _Scene_0 to restart the game
        SceneManager.LoadScene("__Scene_0");
    }

    static public void HERO_DIED()
    {
        S.DelayedRestart();
    }

    ///<summary>
    ///Static function that gets a WeaponDefinition from the WEAP_DICT static
    ///protected field of the main class.
    /// </summary>
    /// <returns>The WeaponDefinition or, if there is no WeaponDefinition with
    /// the WeaponType passed in, returns a new WeaponDefinition with a
    /// WeaponType of none..</returns>
    /// <param name="wt">The WeaponType of the desired WeaponDefinition</param>
    static public WeaponDefinition GET_WEAPON_DEFINITION(eWeaponType wt)
    {
        // Check to make sure that the key exists in the Dictionary
        // Attempting to retrieve a key that didn't exist would throw an error,
        // so the following if statement is important.
        if (WEAP_DICT.ContainsKey(wt))
        {
            return (WEAP_DICT[wt]);
        }
        // This returns a new WeaponDefinition with a type of WeaponType.none,
        // which means it has failed to find the right WeaponDefinition
        return new WeaponDefinition();
    }
}
