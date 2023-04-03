using UnityEngine;
using System;
using WebSocketSharp.Server;
using WebSocketSharp;

namespace Candid
{
	public class Data : WebSocketBehavior
	{
		protected override void OnOpen()
		{
			base.OnOpen();

			Debug.Log("Connect Opened");
		}

		protected override void OnMessage(MessageEventArgs e)
		{
			Debug.Log("Message Recieved: " + e.Data);

			Send("Done");

			DeepLinker.instance.CloseSocket(e.Data);
		}
	}

	public class DeepLinker : MonoBehaviour
	{
		public static DeepLinker instance;

		[SerializeField]
		string url = "https://cautious-leather-textbook.glitch.me";

		// Start is called before the first frame update5
		void Start()
		{
			instance = this;
		}

		void OnDestroy()
		{
		}

		bool opened = false;
		Action<string> callback = null;

		public void HandleConnection(Action<string> _callback = null)
		{
			callback = _callback;
			StartSocket();

			Debug.Log("Open Url: " + url);

			Application.OpenURL(url);
		}

		WebSocketServer wssv;
		Data dataConnection;

		void StartSocket()
		{
			Debug.Log("Web Socket starting");

			wssv = new WebSocketServer("ws://127.0.0.1:8080");
			wssv.AddWebSocketService<Data>("/data");
			wssv.Start();

			Debug.Log("Web Socket Started");
		}

		public void CloseSocket(string identity)
		{
			Debug.Log("Send to connector");

			if (callback != null)
				callback(identity);

			callback = null;

			Debug.Log("CloseSocket");
			wssv.Stop();
			wssv = null;
		}
	}
}