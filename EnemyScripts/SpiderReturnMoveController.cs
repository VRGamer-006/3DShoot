using UnityEngine;
using System.Collections;

public class SpiderReturnMoveController : MonoBehaviour {

    public GameObject spiderObject;
    public GameObject AIMoveSizeobj;
    private SpiderAnimation spiderAnimation; 
    private Vector3 targetPosition;      
    private EnemyMove enemyMove;  
    Quaternion rotation;
    private int randomx;
    private int randomz;
    private Vector3 dir;
    private int flashcount;
    void Awake()
    {
        rotation = transform.parent.rotation;
        spiderAnimation = spiderObject.GetComponent("SpiderAnimation") as SpiderAnimation;
        enemyMove = AIMoveSizeobj.GetComponent<EnemyMove>();
        targetPosition = transform.position;
        flashcount = 0;
    }
    void Update () {
        flashcount++;
        if (flashcount == 10)
        {
            flashcount = 0;
            randomx = Random.Range(-10, 10);
            randomz = Random.Range(-10, 10);
        }
        enemyMove.movementDirection = new Vector3(randomx, 0, randomz);
        spiderAnimation.enemyRun();
        dir = Vector3.Slerp(dir, enemyMove.movementDirection, Time.deltaTime * 100);
        rotation = Quaternion.LookRotation(dir);
    }
}
