import { AuthClient } from "@dfinity/auth-client";
import { HttpAgent } from "@dfinity/agent";
import { MyStorage } from "./MyStorage";

var identity = null;
var storage = new MyStorage ();

document.getElementById("click").addEventListener("click", async (e) => {
  e.preventDefault();
  document.getElementById("click").setAttribute("disabled", true);
  document.getElementById("click").setAttribute("hidden", "hidden");

  await GetIdentity ();

  return false;
});

const webSocket = new WebSocket('ws://localhost:8080/data');

webSocket.addEventListener('open', () => {
    console.log('WebSocket connection established.');
});

webSocket.addEventListener('message', event => {
  if (event.data == "Done")
  {
    webSocket.close ();
    window.close ();
  }
});

function sendMessage(message) {
  webSocket.send(message);
}

export async function GetIdentity() {
  try {
    const authClient = await AuthClient.create({
      storage: storage,
      keyType: 'Ed25519',
    });

    const isAuth =  await authClient.isAuthenticated();
    if (isAuth) {
      console.log("setLoggedIn "+isAuth);
      const identity = await authClient.getIdentity();
      sendMessage (JSON.stringify (identity));
    }
    else
    {
      // NFID
      const APPLICATION_NAME = "Dragginz";
      const APPLICATION_LOGO_URL = "https://cdn.glitch.global/bcd5284d-ad10-482d-bcea-f96a80f01aa7/avatar.png?v=1675867411047";
      const AUTH_PATH = "/authenticate/?applicationName=" + APPLICATION_NAME + "&applicationLogo=" + APPLICATION_LOGO_URL + "#authorize";
      const NFID_AUTH_URL = "https://nfid.one" + AUTH_PATH;

      await new Promise((resolve, reject) => {
          authClient.login({
              identityProvider: NFID_AUTH_URL,
              windowOpenerFeatures:
                  `left=${window.screen.width / 2 - 525 / 2}, ` +
                  `top=${window.screen.height / 2 - 705 / 2},` +
                  `toolbar=0,location=0,menubar=0,width=525,height=705`,
              onSuccess: resolve,
              onError: reject,
          });
      });
    
      identity = authClient.getIdentity();
      sendMessage (JSON.stringify (identity));
      // end of NFID

      // II
/*        
      const daysToAdd = 3;
      const expiry = Date.now() + (daysToAdd * 86400000);
      
      await authClient.login({
        onSuccess: async () => {
          identity = await authClient.getIdentity();
          sendMessage (JSON.stringify (identity));
        },
        onError: (error) => {
          console.log (error);
        },
        maxTimeToLive: BigInt(expiry * 1000000),
//        identityProvider: "https://identity.ic0.app/#authorize"
        identityProvider: "http://localhost:4943?canisterId=qaa6y-5yaaa-aaaaa-aaafa-cai#authorize"
      });
      // end of II
*/
    }
  } catch (e) {
    console.error(e);
  }
}