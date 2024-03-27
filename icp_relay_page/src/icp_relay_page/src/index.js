import { AuthClient } from "@dfinity/auth-client";
import { HttpAgent } from "@dfinity/agent";
import { MyStorage } from "./MyStorage";

console.log ("Starting Call");

var isChromium = window.chrome;
var winNav = window.navigator;
var vendorName = winNav.vendor;
var isOpera = typeof window.opr !== "undefined";
var isIEedge = winNav.userAgent.indexOf("Edg") > -1;
var isIOSChrome = winNav.userAgent.match("CriOS");

var identity = null;
var url = new URL(window.location.href);
var localhost = url.searchParams.get("localhost");
var storeName = "LiveStorage";
var identityProvider = "https://identity.ic0.app/#authorize";
if (localhost)
{
  identityProvider = "http://rdmx6-jaaaa-aaaaa-aaadq-cai.localhost:4943/#authorize";
  storeName = "LocalhostStorage";
}

console.log (identityProvider);

var storage = new _MyStorage__WEBPACK_IMPORTED_MODULE_2__.MyStorage (storeName);
var isChrome = false;
  
if (isIOSChrome) {
   // is Google Chrome on IOS
} else if(
  isChromium !== null &&
  typeof isChromium !== "undefined" &&
  vendorName === "Google Inc." &&
  isOpera === false &&
  isIEedge === false
) 
{
  document.getElementById("click").addEventListener("click", async (e) => {
    e.preventDefault();
    document.getElementById("click").setAttribute("disabled", true);
    document.getElementById("click").setAttribute("hidden", "hidden");

    await GetIdentity ();

    return false;
  });
  
  isChrome = true;
} 
else 
{ 
    document.getElementById("click").setAttribute("disabled", true);
    document.getElementById("click").setAttribute("hidden", "hidden");
}
  
// open web socket
console.log ("Open Websockets");
  
const webSocket = new WebSocket('ws://localhost:8080/data');
var connected = false;
  
webSocket.addEventListener('open', () => {
  console.log('WebSocket connection established.');
  connected = true;
});

webSocket.addEventListener('close', (e) => {
  console.log("Websocket Closed");
});

webSocket.addEventListener('error', (e) => {
  console.log(e);
});

webSocket.addEventListener('message', event => {
  if (event.data == "Done")
  {
    console.log ("Closing websocket");
    webSocket.close ();
    window.close ();
  }
});

function sendMessage(message) {
  webSocket.send(message);
}

function sleep(ms) {
  return new Promise(resolve => setTimeout(resolve, ms));
}

function checkConnection ()
{
  console.log ("checkConnection");
  if (!connected)
  {
    setTimeout (checkConnection, 1000);
  }
  else
  {
    GetIdentity ();
  }
}

if (!isChrome)
  checkConnection ();

async function GetIdentity() {
  try {
    console.log ("GetIdentity");

    var authClient = await _dfinity_auth_client__WEBPACK_IMPORTED_MODULE_0__.AuthClient.create({
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
/*      const APPLICATION_NAME = "ICP.net Demo";
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
*/
      // II      
      const daysToAdd = 3;
      const expiry = Date.now() + (daysToAdd * 86400000);

      await authClient.login({
        onSuccess: async () => {
          console.log ("Success");

          identity = await authClient.getIdentity();
          sendMessage (JSON.stringify (identity));
        },
        onError: (error) => {
          console.log (error);
        },
        maxTimeToLive: BigInt(expiry * 1000000),
//        identityProvider: "https://identity.ic0.app/#authorize"
//        identityProvider: "http://localhost:4943?canisterId=qjdve-lqaaa-aaaaa-aaaeq-cai#authorize"
        identityProvider: identityProvider
      });
      // end of II
    }
  } catch (e) {
    console.error(e);
  }
}
