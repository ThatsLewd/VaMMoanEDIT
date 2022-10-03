using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System.Linq;
using System.IO;
using MeshVR;
using Request = MeshVR.AssetLoader.AssetBundleFromFileRequest;
using MVR;

// VAMMoan
//
// Partial : voices

namespace VAMMoanPlugin
{
    public partial class VAMMoan : MVRScript
    {
		public class Voices
		{
			public static string VOICES_PATH;
			public string VoicesInformations = "";

			public List<string> voicesNames;
			Dictionary<string, Voice> nameToVoice = new Dictionary<string, Voice>();
			
			public Request voicesBundleRequest = null;
			public Request voicesSharedBundleRequest = null;

			public bool isLoading = false;
			
			public Voices()
			{
				
				try
				{
					// Will be updated with the callback of the last assetbundle
					isLoading = true;
					
					VOICES_PATH = ASSETS_PATH;
					voicesNames = new List<string>();
					
					// Creating the voice list
					SuperController.singleton.GetDirectoriesAtPath(VOICES_PATH).ToList().ForEach((string path)=>
					{
						path = SuperController.singleton.NormalizePath(path);
						string name = PathExt.GetFileName(path);
						voicesNames.Add(name);
					});
					
					// Loading the bundle for the current selected voice
					Request request = new AssetLoader.AssetBundleFromFileRequest {path = VOICES_PATH + "/voices.voicebundle", callback = OnVoicesBundleLoaded};
					AssetLoader.QueueLoadAssetBundleFromFile(request);
					
					// Loading the shared bundle used by all voices
					Request requestShared = new AssetLoader.AssetBundleFromFileRequest {path = VOICES_PATH + "/voices-shared.voicebundle", callback = OnVoicesBundleSharedLoaded};
					AssetLoader.QueueLoadAssetBundleFromFile(requestShared);			
				}
				catch(Exception e)
				{
					Debug.LogWarning(e);
				}
				
			}

			public Voice GetVoice(string name)
			{
				if( isLoading == true ) return null;
				
				if (nameToVoice.ContainsKey(name))
				{
					return nameToVoice[name];
				}
				else
				{
					return null;
				}
			}

			public Voice GetRandomVoice()
			{
				int randomIndex = Mathf.Clamp(UnityEngine.Random.Range(0, voices.Count), 0, voices.Count-1);
				return voices[randomIndex];
			}

			private void OnVoicesBundleLoaded(Request aRequest) {
				voicesBundleRequest = aRequest;				
			}

			private void OnVoicesBundleSharedLoaded(Request aRequest) {
				voicesSharedBundleRequest = aRequest;				
				
				// When this one is load, I can initialize all voices
				SuperController.singleton.GetDirectoriesAtPath(VOICES_PATH).ToList().ForEach((string path)=>
				{
					path = SuperController.singleton.NormalizePath(path);
					string name = PathExt.GetFileName(path);
					nameToVoice[name] = new Voice(VOICES_PATH, path, name, this);
				});
				
				isLoading = false;
			}
			
			public void UnloadAudio() {
				AssetLoader.DoneWithAssetBundleFromFile(VOICES_PATH + "/voices.voicebundle");
				AssetLoader.DoneWithAssetBundleFromFile(VOICES_PATH + "/voices-shared.voicebundle");
			}

			public List<Voice> voices
			{
				get
				{
					return nameToVoice.Values.ToList();
				}
			}
		}
	}
}