using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum PlayerBombState { Fall, Idle,Explode }
public class PlayerBombSpawner : MonoBehaviour
{
    public static PlayerBombSpawner instance;

    [Title("Main")]
    [SerializeField] FloatingJoystick joystick;
    [SerializeField] bool spawnActive;
    [SerializeField] float spawnDuration;
    [SerializeField] float spawnCounter;

    [Space(6)]

    [Title("Pool")]
    [SerializeField] GameObject bombPrefab;
    [SerializeField] int poolCount;
    [SerializeField] List<PlayerBomb> bombs;
    [SerializeField] List<PlayerBomb> activeBombs;


    private Transform bombsParent;
    private Vector2 joyPos;
    private void Awake()
    {
        instance = (!instance) ? this : instance;

        bombsParent = new GameObject("Bombs Parent").transform;
        Pool();
    }

    private void FixedUpdate()
    {
        joyPos = new Vector2(joystick.Horizontal, joystick.Vertical);
        SpawnerController();
    }


    private void SpawnerController()
    {
        if (!spawnActive || GameManager.FreezeGame || joyPos.magnitude == 0 || GameManager.GameFinish)
            return;

        spawnCounter = Mathf.MoveTowards(spawnCounter, spawnDuration, Time.fixedDeltaTime);

        if (spawnCounter == spawnDuration)
        {
            SpawnBomb();
            spawnCounter = 0;
        }
    }

    private void SpawnBomb()
    {
        if (!GetBomb())
            return;

        PlayerBomb bomb = GetBomb();
        bomb.gameObject.SetActive(true);
        bomb.transform.position = transform.position;
        bomb.PlaySpawnAnim().Forget();
        AddActiveBomb(bomb: bomb);
    }


    public static void AddActiveBomb(PlayerBomb bomb)
    {
        if (instance.activeBombs.Contains(bomb))
            return;

        instance.activeBombs.Add(bomb);
    }

    public static void RemoveActiveBomb(PlayerBomb bomb)
    {
        if (!instance.activeBombs.Contains(bomb))
            return;

        instance.activeBombs.Remove(bomb);
    }

    public static void AllExplosionActiveBombs()
    {
        instance.activeBombs.ForEach(bomb => bomb.Explosion(delay: 0.1f).Forget());
    }

    private void Pool()
    {
        while (poolCount > 0)
        {
            poolCount--;

            PlayerBomb bomb = Instantiate(bombPrefab, bombsParent).GetComponent<PlayerBomb>();
            bomb.gameObject.SetActive(false);
            bombs.Add(bomb);
        }
    }


    public static PlayerBomb GetBomb()
    {
        return instance.bombs.Find(bomb => !bomb.gameObject.activeInHierarchy);
    }


    public void SetSpawnActive(bool active)
    {
        spawnActive = active;
    }


}
