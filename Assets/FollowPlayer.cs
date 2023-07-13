using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    // player
    public Transform player;

    // boss
    public Transform boss;

    // Camera Speed
    public float cameraRotSpeed = 10f;

    // Camera position Offset
    public Vector3 offset;

    // Camera LockOn
    private bool isLockedOn;

    // LockOn Distance
    public float lockOnDistance = 5f;

    // Ground Layer
    public LayerMask groundLayer;

    // collisionOffset
    public float collisionOffset = 0.2f;

    void Update()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            isLockedOn = !isLockedOn;
        }
    }

    void LateUpdate()
    {
        if (!isLockedOn)
        {
            // Calculate Camera Pos by player pos
            Vector3 desiredPosition = player.position + offset;

            RaycastHit hit;

            if (Physics.Linecast(player.position, desiredPosition, out hit, groundLayer))
            {
                Vector3 normalPos = hit.point + hit.normal * collisionOffset;

                transform.position = Vector3.Lerp(transform.position, normalPos, cameraRotSpeed * Time.deltaTime);
            }
            else
            {
                // Camera Lerp 
                transform.position = Vector3.Lerp(transform.position, desiredPosition, cameraRotSpeed * Time.deltaTime);
            }

            // Get mouse movement
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            // Rotate camera based on mouse movement
            transform.RotateAround(player.position, Vector3.up, mouseX * cameraRotSpeed * Time.deltaTime);
            transform.RotateAround(player.position, transform.right, -mouseY * cameraRotSpeed * Time.deltaTime);
        }
        else
        {
            // Calculate midpoint between player and boss
            Vector3 midpoint = (player.position + boss.position) / 2f;

            // Calculate distance between player and boss
            float playerBossDistance = Vector3.Distance(player.position, boss.position);

            // Calculate camera position
            Vector3 desiredPosition = midpoint + offset;

            // Adjust camera position based on player-boss distance
            desiredPosition -= transform.forward * playerBossDistance;

            // Camera Lerp 
            transform.position = Vector3.Lerp(transform.position, desiredPosition, cameraRotSpeed * Time.deltaTime);

            // Look at midpoint between player and boss
            transform.LookAt(midpoint);
        }
    }
}
