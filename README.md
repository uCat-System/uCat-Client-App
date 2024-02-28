Welcome to uCat!

Installation instructions:

1. Clone the uCat repo.
2. Install Unity Hub
3. Within Unity Hub, install Unity version 2022.1.4.
4. Open the cloned repo (uCat root folder) in Unity.
5. If you have a quest 2 or 3 headset, you can plug it into your PC in Oculus Link mode and it should be enabled on hitting Play.
5. Otherwise, simply ensure your microphone is enabled and hit play!

Note: If you want to use the ConvoMode (AI agent) feature, you will need a valid openAI api key. Click here to do that.

Once you have your key, you need to create a file in the root of the `/Assets` folder, called `secrets.json`. Its contents should be:

 `{"OPENAI_API_KEY":"<your-api-key>"}`

Please make an issue on this repo if you have any problems running the app.