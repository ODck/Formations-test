using UnityEngine;

// The GameObject is made to bounce using the space key.
// Also the GameObject can be moved forward/backward and left/right.
// Add a Quad to the scene so this GameObject can collider with a floor.

public class Controller : MonoBehaviour
{
    private CharacterController controller;
    public float gravity = 20.0f;
    private Vector3 moveDirection = Vector3.zero;
    public float speed = 6.0f;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {

        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection = moveDirection * speed;

        if (Input.GetKey(KeyCode.Q))
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + Vector3.up * 5);

        if (Input.GetKey(KeyCode.E))
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + Vector3.down * 5);

        moveDirection.y = moveDirection.y - gravity * Time.deltaTime;
        // Move the controller
        controller.Move(moveDirection * Time.deltaTime);
        
        var position = transform.position;
        var realPos = new Vector3(position.x, position.y, position.z);
        realPos.x = Mathf.Clamp(realPos.x, -5, 5);
        realPos.z = Mathf.Clamp(realPos.z, -10, 10);
        transform.position = realPos;
    }
}