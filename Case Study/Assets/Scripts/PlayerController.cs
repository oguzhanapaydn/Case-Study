using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Current;
    public float limitX;

    public float xSpeed;
    public float RunningSpeed;
    private float _currentRunningSpeed;

    public GameObject RidingCubePrefab;
    public List<RidingCube> cubes;

    private bool _jumperBridge;
    public GameObject jumperPiecePrefab;
    private JumpSpawner _jumpSpawner;
    private float _creatingJumpTimer;
    // Start is called before the first frame update
    void Start()
    {
        Current = this;
        _currentRunningSpeed = RunningSpeed;

    }

    // Update is called once per frame
    void Update()
    {
        float newX = 0;
        float touchXDelta = 0;
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            touchXDelta = Input.GetTouch(0).deltaPosition.x / Screen.width;
        }
        else if( Input.GetMouseButton(0))
        {
            touchXDelta = Input.GetAxis("Mouse X");
        }

        newX = transform.position.x + xSpeed * touchXDelta * Time.deltaTime;
        newX = Mathf.Clamp(newX, -limitX, limitX);


        Vector3 newPosition = new Vector3(newX, transform.position.y, transform.position.z + _currentRunningSpeed * Time.deltaTime);
        transform.position = newPosition;

        if (_jumpSpawner)
        {
            _creatingJumpTimer -= Time.deltaTime;
            if (_creatingJumpTimer < 0)
            {
                _creatingJumpTimer = 0.01f;
                IncrementCubeVolume(-0.01f);
                GameObject createJumpPiece = Instantiate(jumperPiecePrefab);

                Vector3 direction = _jumpSpawner.endReference.transform.position - _jumpSpawner.startReference.transform.position;
                float distance = direction.magnitude;
                direction = direction.normalized;
                createJumpPiece.transform.forward = direction;
                float characterDistance = transform.position.z - _jumpSpawner.startReference.transform.position.z;
                characterDistance = Mathf.Clamp(characterDistance, 0, distance);
                Vector3 newPiecePosition = _jumpSpawner.startReference.transform.position + direction * characterDistance;
                newPiecePosition.x = transform.position.x;
                createJumpPiece.transform.position = newPiecePosition;




            }
        }
    }


    public void IncrementCubeVolume(float value)
    {
        if(cubes.Count == 0)
        {
            if (value > 0)
            {
                CreateCube(value);
            }

            else
            {
                //GAmeover
            }
        }
        else
        {
            cubes[cubes.Count - 1].IncrementCubeVolume(value);        }


    }

    public void CreateCube(float value)
    {
        RidingCube createCube = Instantiate(RidingCubePrefab, transform).GetComponent<RidingCube>();
        cubes.Add(createCube);
        createCube.IncrementCubeVolume(value);


    }

    public void DestroyCube(RidingCube cube)
    {
        cubes.Remove(cube);
        Destroy(cube.gameObject);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "AddCube")
        {
            IncrementCubeVolume(0.1f);
            Destroy(other.gameObject);
        }else if (other.tag == "StartSpawningJump")
        {   
            StartSpawningJump(other.transform.parent.GetComponent<JumpSpawner>());
        }else if(other.tag == "StopSpawningJump")
        {
            StopSpawningJump();
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Trap")
        {
            IncrementCubeVolume(-Time.fixedDeltaTime);
            
        }


    }

    public void StartSpawningJump(JumpSpawner spawner)
    {
        _jumpSpawner = spawner;
        _jumperBridge = true;


    }
    
    public void StopSpawningJump()
    {
        _jumperBridge = false;
    }

}


