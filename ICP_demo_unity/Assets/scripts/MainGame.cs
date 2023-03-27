using Candid;
using Candid.Demo.Models;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainGame : MonoBehaviour
{
	public GameObject playerPrefab;
	public GameObject otherPlayerPrefab;

	public Transform worldObj;

	GameObject playerObj = null;
	Dictionary<string, GameObject> otherPlayerObjs = new Dictionary<string, GameObject>();

	// canvas objects
	[SerializeField]
	GameObject mainMenu;

	[SerializeField]
	Button connectBtn;

	[SerializeField]
	TextMeshProUGUI moveTimer;

	float updateTimer = 0;

	// Start is called before the first frame update
	void Start()
	{
		moveTimer.text = "";
	}

	// Update is called once per frame
	void Update()
	{
		if (BackendConnector.IsConnected)
		{
			updateTimer += Time.deltaTime;

			// update world map every 3 seconds
			if (updateTimer > 3)
			{
				updateCall();
			}
		}
	}

	// press button
	public void OnConnectedToServer()
	{
		connectBtn.interactable = false;

		DeepLinker.instance.HandleConnection(onLoginCompleted);
	}

	void onLoginCompleted(string json)
	{
		LoginCompleted(json);
	}

	public void onMoveLeft()
	{
		movePlayer(0);
	}

	public void onMoveRight()
	{
		movePlayer(1);
	}

	public void onMoveUp()
	{
		movePlayer(2);
	}

	public void onMoveDown()
	{
		movePlayer(3);
	}

	public async UniTask LoginCompleted(string identity_json = null)
	{
		Debug.Log("LoadGameData");

		// connect to backend
		if (!BackendConnector.IsConnected)
		{
			if (identity_json != null)
				await BackendConnector.createIdentityByJsonAndConnect(identity_json, true);
			else
				await BackendConnector.ConnectICPNET(false, true);
		}

		if (!BackendConnector.IsConnected)
			return;

		// login
		try
		{
			LoginResult result = await BackendConnector.mainClient.Login();

			await UniTask.SwitchToMainThread();

			if (result.Ok.GetValueOrDefault() != null)
			{
				Login login = result.Ok.GetValueOrDefault ();
				updateWorldMap(login.Map, login.Player);

				mainMenu.gameObject.SetActive(false);
			}
			else
			{
				// show error
			}
		}
		catch (Exception e)
		{
			Debug.Log(e);
		}
	}

	public async UniTask updateCall ()
	{
		try
		{
			UpdateMapResult result = await BackendConnector.mainClient.UpdateWorld();

			await UniTask.SwitchToMainThread();

			if (result.Ok.GetValueOrDefault() != null)
			{
				Map map = result.Ok.GetValueOrDefault();
				updateWorldMap(map);

				mainMenu.gameObject.SetActive(false);
			}
			else
			{
				// show error
			}
		}
		catch (Exception e)
		{
			Debug.Log(e);
		}
	}


	public async UniTask movePlayer(sbyte direction)
	{
		try
		{
			MoveMsg moveMsg = new MoveMsg(direction);
			LoginResult result = await BackendConnector.mainClient.MovePlayer(moveMsg);

			await UniTask.SwitchToMainThread();

			if (result.Ok.GetValueOrDefault() != null)
			{
				Login login= result.Ok.GetValueOrDefault();
				updateWorldMap(login.Map, login.Player);

				mainMenu.gameObject.SetActive(false);
			}
			else
			{
				// show error
			}
		}
		catch (Exception e)
		{
			Debug.Log(e);
		}
	}

	public void updateWorldMap(Map map, Player player = null)
	{
		// update player Obj
		if (player != null)
		{
			if (playerObj != null)
			{
				// update the player
			}
			else
			{
				// create the player
			}
		}

		// update Other objs
		if (map != null)
		{
			for (var o = 0; o < map.Players.Count; o++)
			{
				Player playerInfo = map.Players[o];

				if (otherPlayerObjs.ContainsKey(playerInfo.Owner))
				{
					// update object
				}
				else
				{
					// create a new Object
				}
			}
		}
	}
}
