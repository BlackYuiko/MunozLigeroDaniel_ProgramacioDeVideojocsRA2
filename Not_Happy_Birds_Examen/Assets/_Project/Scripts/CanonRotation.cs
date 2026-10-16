using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CanonRotation : MonoBehaviour, InputSystem_Actions.IAngryBirdsActions
{
    public Vector3 _maxRotation;
    public Vector3 _minRotation;
    private float _offset = -51.6f;
    public GameObject ShootPoint;
    public GameObject Bullet;
    public float ProjectileSpeed = 0;
    public float MaxSpeed;
    public float MinSpeed;
    public GameObject PotencyBar;
    private float _initialScaleX;
    private Vector2 _distanceBetweenMouseAndPlayer;
    private bool isRaising = false;
    [SerializeField] private float _multiplier = 10f;

    private InputSystem_Actions inputActions;
    public Vector2 mousePos;


    private void Awake()
    {
        _initialScaleX = PotencyBar.transform.localScale.x;
        inputActions = new InputSystem_Actions();
        inputActions.AngryBirds.SetCallbacks(this);
    }

    public void OnEnable() => inputActions.Enable();
	public void OnDisable() => inputActions.Disable();

	void Update()
    {
        if (isRaising)
        {
			ProjectileSpeed = Time.deltaTime * _multiplier + ProjectileSpeed; //acotar entre dos valors (mirar variables)
			CalculateBarScale();
		}
    }
    public void CalculateBarScale()
    {
        PotencyBar.transform.localScale = new Vector3(Mathf.Lerp(0, _initialScaleX, ProjectileSpeed / MaxSpeed),
            transform.localScale.y,
            transform.localScale.z);
    }

    public void OnMousePosition(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
			mousePos = context.ReadValue<Vector2>();

			Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePos);
			Vector2 direction = mouseWorldPosition - transform.position;
			transform.up = direction;

			var ang = (Mathf.Atan2(direction.y, direction.x) * 180f / Mathf.PI + _offset);

			transform.rotation = Quaternion.Euler(0, 0, ang);

            _distanceBetweenMouseAndPlayer = direction;


		}
	}

    public void OnBulletShot(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isRaising = true;
		}
        if (context.canceled)
        {
            var projectile = Instantiate(Bullet, ShootPoint.transform.position, Quaternion.identity); //canviar la posició on s'instancia
            projectile.GetComponent<Rigidbody2D>().linearVelocity = _distanceBetweenMouseAndPlayer * ProjectileSpeed / MaxSpeed;

			ProjectileSpeed = 0f;
            isRaising = false;

		}
    }
}
