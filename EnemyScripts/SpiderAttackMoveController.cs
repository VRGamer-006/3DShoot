using UnityEngine;
using System.Collections;

public class SpiderAttackMoveController : MonoBehaviour {
    public GameObject AIMoveSizeobj;
    public Transform player;    
    private EnemyMove enemyMove;

    void Awake()
    {
        enemyMove = AIMoveSizeobj.GetComponent<EnemyMove>();
    }
	void FixedUpdate () {
        enemyMove.movementDirection = player.position - transform.position;
    }
}
