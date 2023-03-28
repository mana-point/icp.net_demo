ICP.net Demo


There are 3 projects here.

1. The Azle Backend example. See demo_backend directory. To install run "npm install". Make sure you are running "dfx start". Once you are you can build the backend, using the create.sh or update.sh scripts.

2. A relay connector site. This generates a simple front end web page to connect to the II and be able to connect to your canister.

If you are running locally, you will need to install the internet-identity repo, and run it locally to connect. You will also need to change line 94 of the index.js to point to your II canister.

          identityProvider: "http://localhost:4943?canisterId=qaa6y-5yaaa-aaaaa-aaafa-cai#authorize"

To build this simple page, run "npm run build". The result will be built to the dist directory. Please host this somewhere online, so unity can call it, or run the webserver locally.

3. The Unity Project. This connects to the II initially, while doing so it opens up a web browser and a local websocket. The webbrowser can be set in the Unity project, by selecting the MainGame object, and setting the URL of the DeepLiker. You will also need to set the Main Canister ID of the azle project in the Main Game script so that the unity client knows what client to connect to. Also set the useLocalhost bool to true if you are running local host or false if connecting to a live server.

4. To allow unity to connect to the canister, you need to generate some c# and copy them from the demo_backend directory, to the unity/asset/scripts/Candid/Demo directory. To create these files run

"rm -rf ./csharp"

and

"dotnet ./clientGenerator/EdjCase.ICP.ClientGenerator.dll ."

Then copy the ./csharp directory to "ICP_Connection_demo/asset/scripts/Candid/Demo". Remove exsisting files first.




email me at phil@manapoint.io if you have any questions.





