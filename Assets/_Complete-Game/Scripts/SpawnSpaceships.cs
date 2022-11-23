using UnityEngine;
using System.Collections.Generic;
using Unity.Burst;
using Random = UnityEngine.Random;
using UnityEngine.Jobs;
using Unity.Mathematics;

[BurstCompile]
public class SpawnSpaceships : MonoBehaviour
{
    public GameObject enemyshipPrefab;
    public int spaceshipsAmount = 20;
    public Transform PlayerShip;
    public List<Transform> enemyShips;
    public float shipSpeed = 5f;
    private Bounds _bounds;
    public bool UseJobs;
    public float JobsInterpolator = 0.01f;
    
    // Transforms onde o Job irá atuar
    [SerializeField]public Transform[] SpaceshipsArray;
    private TransformAccessArray transformAccessArray;    
    private void Awake()
    {
        _bounds = GetComponent<BoxCollider>().bounds;
    }

    private void Start()
    {
        for (var i = 0; i < spaceshipsAmount; i++)
        {
            var xPos = Random.Range(_bounds.min.x, _bounds.max.x);
            var enemyShipPosition = new Vector3(xPos, transform.position.y, transform.position.z);
            var enemy = Instantiate(enemyshipPrefab, enemyShipPosition, Quaternion.identity);
            enemyShips.Add(enemy.transform);
        }
        SpaceshipsArray = enemyShips.ToArray();
        //transformAccessArray = new TransformAccessArray(SpaceshipsArray, 4);        
        transformAccessArray = new TransformAccessArray(SpaceshipsArray);
    }
    
    public struct JobsMover : IJobParallelForTransform
    {
        // public float3 shipTransformPos;
        //public float speed;
        public float3 playerTransformPos;
        public float deltaTime;
        public float t; // 0 e 1
        
        //public float currentTime;
        public void Execute(int index, TransformAccess transform)
        {
            // Para cada membro do array de TransformAccess, um transform é processado aqui em paralelo 
            transform.position = math.lerp(transform.position, playerTransformPos, t);
            //Vector3.MoveTowards(transform.position, 
            //playerTransform.position, speed * Time.deltaTime);
        }
    }

    void Update()
    {
        if (UseJobs)
        {
            // foreach (var ship in enemyShips)
            // {
                // if (ship == null)
                //     continue;
                // var spaceshipJobs = new JobsMover()
                // {
                //     playerTransformPos = PlayerShip.position,
                //     //speed = shipSpeed,
                //     shipTransformPos = ship.position,
                //     t = 0.1f
                // };
            // }
            var job = new JobsMover()
            {
                deltaTime = Time.deltaTime,
                t = JobsInterpolator,
                playerTransformPos = PlayerShip.position,
            };
            var jobHandle = job.Schedule(transformAccessArray);
            jobHandle.Complete();

            //transformAccessArray.Dispose();
            //Dar dispose nos NativeArrays aqui
        }
        else
        {
            foreach (var ship in enemyShips)
            {
                if (ship == null)
                    continue;
                ship.LookAt(PlayerShip);
                ship.position = Vector3.MoveTowards(ship.position, PlayerShip.position, shipSpeed * Time.deltaTime);
            }
        }
    }

    void OnDestroy()
    {
        transformAccessArray.Dispose();
    }
    
}
