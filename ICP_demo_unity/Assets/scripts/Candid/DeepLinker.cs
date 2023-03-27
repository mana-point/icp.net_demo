using UnityEngine;
using System;
using WebSocketSharp.Server;
using WebSocketSharp;

namespace Candid
{
	public class Chat : WebSocketBehavior
	{
		protected override void OnMessage(MessageEventArgs e)
		{
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

			Application.OpenURL(url);
		}

		WebSocketServer wssv;

		void StartSocket()
		{
			wssv = new WebSocketServer("ws://127.0.0.1:8080");
			wssv.AddWebSocketService<Chat>("/data");
			wssv.Start();
		}

		public void CloseSocket(string identity)
		{
			Debug.Log("CloseSocket");

			wssv.Stop();
			wssv = null;

			if (callback != null)
				callback(identity);

			callback = null;
		}
	}


}