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

			WebSocketServer.instance.CloseSocket(e.Data);
		}
	}

	public class WebSocketServer : MonoBehaviour
	{
		public static WebSocketServer instance;

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

		public void HandleConnection(Action<string> _callback = null, bool useLocalhost = false, string localHost = "")
		{
			if (callback != null)
				return;

			callback = _callback;
			StartSocket();

			Debug.Log("Open Url: " + url);

			if (useLocalhost)
				Application.OpenURL("https://cautious-leather-textbook.glitch.me?localhost=" + localHost);
			else
				Application.OpenURL("https://cautious-leather-textbook.glitch.me");
		}

		WebSocketSharp.Server.WebSocketServer wssv;
		Data dataConnection;

		void StartSocket()
		{
			Debug.Log("Web Socket starting");

			wssv = new WebSocketSharp.Server.WebSocketServer("ws://127.0.0.1:8080");
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

			Debug.Log("Close Web Socket");

			wssv.RemoveWebSocketService("/data");
			wssv.Stop();
			wssv = null;

			callback = null;
		}
	}
}