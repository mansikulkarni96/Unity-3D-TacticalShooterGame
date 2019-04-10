using System.Collections;
using System.Collections.Generic;
//using System.Drawing.Color;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private Animator enemyAnimator;
    [SerializeField]
    float speed = 100f;
    protected bool PlayerInRange;

    public Transform player;
    public Transform enemy;

    private Vector3 distance;
    private float range;
    private PlayerMovement playerMovement;
    public GameObject playerObject;
    public GameObject explosion;

    void Start()
    {

    }

    void Awake()
    {
        enemyAnimator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (GameObject.Find("Player") != null)
        {

            distance = enemy.position - player.position;
            distance.y = 0;
            range = distance.magnitude;
            distance /= range;
            if (range <= 2)
            {
                PlayerInRange = true;
            }
            else
            {
                PlayerInRange = false;
            }
            if (PlayerInRange)
            {
                Attack();
            }
        }
    }

    private void Attack()
    {
        gameObject.transform.LookAt(player);
        playerMovement = playerObject.GetComponent<PlayerMovement>();
        playerMovement.DestroyPlayer();
        GameObject inst = Instantiate(explosion, this.transform.position, this.transform.rotation) as GameObject;
        Destroy(this.gameObject);
    }
}