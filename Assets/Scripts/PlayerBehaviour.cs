using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PlayerBehaviour : MonoBehaviour
{
    public SharedControls Controls;

    public Transform explosionPrefab;
    private new Camera camera;

    private void Awake()
    {
        camera = GetComponentInChildren<Camera>();
    }

    private void Start()
    {
        Controls.Value.Gameplay.Shoot.performed += (ctx) => Fire();
    }

    private void OnEnable()
    {
        Controls.Value.Gameplay.Shoot.Enable();
    }

    private void OnDisable()
    {
        Controls.Value.Gameplay.Shoot.Disable();
    }

    void Fire()
    {
        Physics.Raycast(camera.transform.position, camera.transform.forward, out RaycastHit hit, 100);
        var explosion = Instantiate(explosionPrefab, hit.point, Quaternion.identity);
        const float explosionDespawnDelay = 5;
        Destroy(explosion.gameObject, explosionDespawnDelay);
    }
}
