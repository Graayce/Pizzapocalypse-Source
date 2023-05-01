using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    // References
    new private Rigidbody rigidbody;
    private Transform cameraPivot;
    private Transform mainCamera;
    private Transform frontLeft;
    private Transform frontRight;
    private Transform backLeft;
    private Transform backRight;

    // Attributes
    [Header("Driving")]
    [SerializeField] private float speed;
    [SerializeField] private float acceleration;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float wheelSpinSpeed;
    [SerializeField] private float maxWheelRotation;
    [SerializeField] private float wheelTurnSpeed;

    [Header("Camera")]
    [SerializeField] private float sensitivity;
    [SerializeField] private float distance;
    [SerializeField] private float zoomSpeed;
    [SerializeField] private float minZoom;
    [SerializeField] private float maxZoom;

    // States
    private float currentSpeed;
    private Vector3 cameraDirection;


    void Awake()
    {
        mainCamera = Camera.main.transform;
        cameraPivot = mainCamera.parent;
        rigidbody = GetComponent<Rigidbody>();

        Transform car = transform.Find("PizzaCar");

        frontLeft = car.Find("FrontLeft");
        frontRight = car.Find("FrontRight");
        backLeft = car.Find("BackLeft");
        backRight = car.Find("BackRight");

        cameraDirection = mainCamera.position - cameraPivot.position;
        cameraDirection.Normalize();

        distance = Vector3.Distance(cameraPivot.position, mainCamera.position);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!Menu.instance.Paused && !DeliveryManager.instance.Failed)
        {
            MoveCar();
            MoveCamera();
        }
        else
            rigidbody.velocity = Vector3.zero;

        if (!DeliveryManager.instance.Failed)
            CameraSanityCheck();
    }

    private void MoveCar()
    {
        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal") * turnSpeed;

        if (vertical != 0)
        {
            if (vertical > 0)
                currentSpeed = Mathf.Lerp(currentSpeed, speed, acceleration * Time.deltaTime);
            else
                currentSpeed = Mathf.Lerp(currentSpeed, -speed, acceleration * Time.deltaTime);
        }
        else
            currentSpeed = Mathf.Lerp(currentSpeed, 0, acceleration * Time.deltaTime);

        float y = rigidbody.velocity.y;
        Vector3 directionVec = transform.forward * currentSpeed;
        Vector3 velocity = new Vector3(directionVec.x, y, directionVec.z);
        rigidbody.velocity = velocity;


        Quaternion target = Quaternion.Euler(0, 0, 0);

        if (horizontal != 0)
        {
            if (rigidbody.velocity.magnitude > 0.5f)
            {
                if (currentSpeed < 0)
                    horizontal *= -1;
                
                transform.Rotate(Vector3.up * horizontal * Time.deltaTime);
            }

            int turnDirection = (int)Mathf.Sign(horizontal);

            target = Quaternion.Euler(0, 0, frontLeft.rotation.z + (maxWheelRotation * turnDirection));
        }

        float speedProportion = currentSpeed / speed;
        int moveDirection = (int)Mathf.Sign(vertical);

        RotateWheels(target, speedProportion, moveDirection);
        ScrollView();

        cameraPivot.position = transform.position;
    }

    private void RotateWheels(Quaternion target, float speedProportion, int moveDirection)
    {
        // I don't wanna hear a word about this :(
        frontLeft.Rotate(Vector3.right * wheelSpinSpeed * speedProportion * moveDirection);
        frontRight.Rotate(Vector3.right * wheelSpinSpeed * speedProportion * moveDirection);
        backLeft.Rotate(Vector3.right * wheelSpinSpeed * speedProportion * moveDirection);
        backRight.Rotate(Vector3.right * wheelSpinSpeed * speedProportion * moveDirection);

        frontLeft.localRotation = Quaternion.Lerp(frontLeft.localRotation, target, wheelTurnSpeed * Time.deltaTime);
        frontRight.localRotation = Quaternion.Lerp(frontRight.localRotation, target, wheelTurnSpeed * Time.deltaTime);
        //backLeft.localRotation = Quaternion.Lerp(backLeft.localRotation, target, wheelTurnSpeed * Time.deltaTime);
        //backRight.localRotation = Quaternion.Lerp(backRight.localRotation, target, wheelTurnSpeed * Time.deltaTime);
    }

    private void ScrollView()
    {
        float scroll = Input.GetAxisRaw("Mouse ScrollWheel");
        
        if (scroll != 0)
            distance -= zoomSpeed * Mathf.Sign(scroll);

        distance = Mathf.Clamp(distance, minZoom, maxZoom);
    }

    private void MoveCamera()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * sensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensitivity;

        cameraPivot.Rotate(Vector3.up * mouseX);

        cameraPivot.Rotate(Vector3.left * mouseY);

        cameraPivot.rotation = Quaternion.Euler(cameraPivot.eulerAngles.x, cameraPivot.eulerAngles.y, 0);
    }

    private void CameraSanityCheck()
    {
        Vector3 direction = mainCamera.position - cameraPivot.position;

        RaycastHit hit;

        if (Physics.Raycast(cameraPivot.position, direction, out hit, distance))
            mainCamera.position = hit.point;
        else
            mainCamera.localPosition = cameraDirection * distance;
    }
}
