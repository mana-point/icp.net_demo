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
	[SerializeField]
	GameObject playerPrefab;

	[SerializeField]
	GameObject otherPlayerPrefab;

	[SerializeField]
	Transform worldObj;

	[SerializeField]
	public string mainCanisterId = "";

	[SerializeField]
	public bool useLocalhost = true;

	GameObject playerObj = null;
	Dictionary<string, GameObject> otherPlayerObjs = new Dictionary<string, GameObject>();

	// canvas objects
	[SerializeField]
	GameObject mainMenu;

	[SerializeField]
	Button connectBtn;

	[SerializeField]
	TextMeshProUGUI moveTimer;

	ulong playerTimer = 0;
	float updateTimer = 0;

	bool isCalling = false;

	// Start is called before the first frame update
	void Start()
	{
		moveTimer.text = "";
	}

	// Update is called once per frame
	void Update()
	{
		if (BackendConnector.IsConnected && playerObj != null)
		{
			if (!isCalling)
			{
				updateTimer += Time.deltaTime;

				// update world map every 3 seconds
				if (updateTimer > 5)
				{
					updateCall();
					updateTimer = 0;
				}
			}

			DateTime time = Helpers.JavaTimeStampToDateTime(playerTimer);
			int timeRemaining = Helpers.timeRemaining(time);

			if (timeRemaining > 0)
			{
				moveTimer.text = "Move In: " + Helpers.getTimeString(timeRemaining);
			}
			else
			{
				moveTimer.text = "Good to Move";

				if (isCalling == false)
				{
					if (Input.GetKeyDown(KeyCode.W))
						onMoveUp();
					else if (Input.GetKeyDown(KeyCode.S))
						onMoveDown();
					else if (Input.GetKeyDown(KeyCode.A))
						onMoveLeft();
					else if (Input.GetKeyDown(KeyCode.D))
						onMoveRight();
				}
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
				await BackendConnector.createIdentityByJsonAndConnect(mainCanisterId, identity_json, useLocalhost);
			else
				await BackendConnector.ConnectICPNET(mainCanisterId, false, useLocalhost);
		}

		if (!BackendConnector.IsConnected)
			return;

		// login
		try
		{
			isCalling = true;

			LoginResult result = await BackendConnector.mainClient.Login();

			await UniTask.SwitchToMainThread();

			if (result.Ok.GetValueOrDefault() != null)
			{
				Login login = result.Ok.GetValueOrDefault ();

				UniTask.SwitchToMainThread();

				updateWorldMap(login.Map, login.Player);

				mainMenu.gameObject.SetActive(false);
			}
			else
			{
				// show error
			}

			isCalling = false;
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
			isCalling = true;

			UpdateMapResult result = await BackendConnector.mainClient.UpdateWorld();

			await UniTask.SwitchToMainThread();

			if (result.Ok.GetValueOrDefault() != null)
			{
				Map map = result.Ok.GetValueOrDefault();
				UniTask.SwitchToMainThread();

				updateWorldMap(map);

				mainMenu.gameObject.SetActive(false);
			}
			else
			{
				// show error
			}

			isCalling = false;
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
			isCalling = true;

			MoveMsg moveMsg = new MoveMsg(direction);
			LoginResult result = await BackendConnector.mainClient.MovePlayer(moveMsg);

			await UniTask.SwitchToMainThread();

			if (result.Ok.GetValueOrDefault() != null)
			{
				Login login= result.Ok.GetValueOrDefault();

				UniTask.SwitchToMainThread();

				updateWorldMap(login.Map, login.Player);

				mainMenu.gameObject.SetActive(false);
			}
			else
			{
				// show error
			}

			isCalling = false;
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
			playerTimer = player.LastTime;

			if (playerObj != null)
			{
				// update the player
				playerObj.transform.position = new Vector3(player.Position.X, 0, player.Position.Y);
			}
			else
			{
				// create the player
				playerObj = Instantiate<GameObject>(playerPrefab);

				playerObj.transform.SetParent(worldObj);
				playerObj.transform.localScale = Vector3.one;
				playerObj.transform.rotation = Quaternion.identity;
				playerObj.transform.position = new Vector3(player.Position.X, 0, player.Position.Y);
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
					GameObject obj = otherPlayerObjs[playerInfo.Owner];

					obj.transform.position = new Vector3(player.Position.X, 0, player.Position.Y);
				}
				else
				{
					// create a new Object
					GameObject obj = Instantiate<GameObject>(otherPlayerPrefab);

					obj.transform.SetParent(worldObj);
					obj.transform.localScale = Vector3.one;
					obj.transform.rotation = Quaternion.identity;
					obj.transform.position = new Vector3(player.Position.X, 0, player.Position.Y);

					otherPlayerObjs.Add (playerInfo.Owner, obj);
				}
			}
		}
	}
}
