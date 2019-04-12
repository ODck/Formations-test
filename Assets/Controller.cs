using UnityEngine;
using System.Collections;

// The GameObject is made to bounce using the space key.
// Also the GameOject can be moved forward/backward and left/right.
// Add a Quad to the scene so this GameObject can collider with a floor.

public partial class Controller : MonoBehaviour
{
    public float speed = 6.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    //public int followersCount;
    private Vector3 moveDirection = Vector3.zero;
    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // let the gameObject fall down
        gameObject.transform.position = new Vector3(0, 5, 0);
    }

    private void Update()
    {
        //followersCount = followers.transform.childCount;
        if (controller.isGrounded)
        {
            // We are grounded, so recalculate
            // move direction directly from axes

            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection = moveDirection * speed;

            if (Input.GetKey(KeyCode.Q))
            {
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + (Vector3.up * 5));
            }
            if (Input.GetKey(KeyCode.E))
            {
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + (Vector3.down * 5));
            }
        }
        moveDirection.y = moveDirection.y - (gravity * Time.deltaTime);
        // Move the controller
        controller.Move(moveDirection * Time.deltaTime);
        var realPos = new Vector3(transform.position.x,transform.position.y,transform.position.z);
        realPos.x = Mathf.Clamp(realPos.x, -5, 5);
        realPos.z = Mathf.Clamp(realPos.z, -10, 10);
        transform.position = realPos;
    }
}