﻿using Assets;
using System.Collections;
using UnityEngine;

public class PowerUpComponent : MonoBehaviour
{
	public static readonly float SHOTGUN_ANGLE = 50f;
	public static readonly float SHOTGUN_REACH = 3f;

	public PowerUpType type;
    // Use this for initialization
    private bool hasTimedPowerUp;
	private int lastShoot = -1;

	public GameObject particleShooter;
	public GameObject bazoubulle;

	// Use this for initialization
	void Start () {
        type = PowerUpType.NONE;
        hasTimedPowerUp = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (type == PowerUpType.NONE)
            return;

        switch(type)
        {
            case PowerUpType.GIANT_BUBBLE :
                if (!hasTimedPowerUp)
                    StartCoroutine(PowerUp());

                    gameObject.transform.localScale = new Vector3(2, 2, 2);

                break;

			case PowerUpType.SHOTBULLE:
		        if(Input.GetButton("Fire1"))
		        {
			        ShootABubble();
			        type = PowerUpType.NONE;
		        }
		        break;

			case PowerUpType.BAZOUBULLE:
				if(Input.GetButton("Fire1"))
				{
					Bazoubulle();
					type = PowerUpType.NONE;
				}
				break;

			case PowerUpType.NONE:
                gameObject.transform.localScale = Vector3.one;
                break;
        }
	}

	private void ShootABubble()
	{
		if(lastShoot != -1 && lastShoot + 20 > Time.frameCount)
			return;
		lastShoot = Time.frameCount;

		Rigidbody body = GetComponent<Rigidbody>();

		Vector2 shootDir = new Vector2(transform.forward.x, transform.forward.z);
		shootDir.Normalize();

		foreach(GameObject player in GameObject.FindWithTag("WorldController").GetComponent<CheckAlives>().GetPlayers())
		{
			Vector2 playerDir = new Vector2(player.transform.position.x - transform.position.x, player.transform.position.z - transform.position.z);

			if(playerDir.magnitude > SHOTGUN_REACH)
				continue;

			if(Vector2.Angle(shootDir, playerDir) < SHOTGUN_ANGLE)
			{
				player.GetComponent<Rigidbody>().AddForce(shootDir.x * 16, 0, shootDir.y * 16, ForceMode.Impulse);
			}
		}

		body.velocity = Vector3.zero;
		body.AddForce(shootDir.x * -8, 0, shootDir.y * -8, ForceMode.Impulse);      
        Shake.ShakeEffect();

		Destroy(Instantiate(particleShooter, transform.position, transform.rotation), 0.7f);
	}


	void Bazoubulle()
	{
		if(lastShoot != -1 && lastShoot + 20 > Time.frameCount)
			return;
		lastShoot = Time.frameCount;

		GameObject clone = Instantiate(bazoubulle, transform.position - new Vector3(0, 0.25f, 0) + transform.forward, transform.rotation);

		clone.GetComponent<Bazoubulle>().baseVel = new Vector3(transform.forward.x * 20, 0, transform.forward.z * 20);
		clone.GetComponent<Bazoubulle>().holder = gameObject;


		Destroy(clone, 30f);
	}

	IEnumerator PowerUp()
    {
        hasTimedPowerUp = true;
        yield return new WaitForSeconds(10);

        type = PowerUpType.NONE;
        hasTimedPowerUp = false;
    }
}
