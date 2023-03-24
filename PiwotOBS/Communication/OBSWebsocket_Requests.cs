using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace PiwotOBS
{
    /// <summary>
    /// Instance of a connection with an OBS-websocket server
    /// </summary>
    public partial class OBSWebsocket
    {
        #region Private Constants

        private const string REQUEST_FIELD_VOLUME_DB = "inputVolumeDb";
        private const string REQUEST_FIELD_VOLUME_MUL = "inputVolumeMul";
        private const string RESPONSE_FIELD_IMAGE_DATA = "imageData";

        #endregion

        /// <summary>
        /// Get the name of the currently active scene. 
        /// </summary>
        /// <returns>Name of the current scene</returns>
        public string? GetCurrentProgramScene()
        {
            JsonObject response = SendRequest(nameof(GetCurrentProgramScene));
            return (string?)response["currentProgramSceneName"];
        }

        /// <summary>
        /// Set the current scene to the specified one
        /// </summary>
        /// <param name="sceneName">The desired scene name</param>
        public void SetCurrentProgramScene(string sceneName)
        {
            var request = new JsonObject
            {
                { nameof(sceneName), sceneName }
            };

            SendRequest(nameof(SetCurrentProgramScene), request);
        }


        /// <summary>
        /// Get a list of scenes in the currently active profile
        /// </summary>
        public JsonObject GetSceneList()
        {
            JsonObject response = SendRequest(nameof(GetSceneList));
            return response;
        }

        /// <summary>
        /// Toggles the status of the stream output.
        /// </summary>
        /// <returns>New state of the stream output</returns>
        public bool ToggleStream()
        {
            var response = SendRequest(nameof(ToggleStream));
            return (bool)(response?["outputActive"]??false);
        }

        /// <summary>
        /// Toggles the status of the record output.
        /// </summary>
        public void ToggleRecord()
        {
            SendRequest(nameof(ToggleRecord));
        }

        /// <summary>
        /// Gets the status of the stream output
        /// </summary>
        /// <returns>An <see cref="OutputStatus"/> object describing the current outputs states</returns>
        public JsonObject GetStreamStatus()
        {
            var response = SendRequest(nameof(GetStreamStatus));
            return response;
        }


        /// <summary>
        /// Change the volume of the specified source
        /// </summary>
        /// <param name="inputName">Name of the source which volume will be changed</param>
        /// <param name="inputVolume">Desired volume. Must be between `0.0` and `1.0` for amplitude/mul (useDecibel is false), and under 0.0 for dB (useDecibel is true). Note: OBS will interpret dB values under -100.0 as Inf.</param>
        /// <param name="inputVolumeDb">Interperet `volume` data as decibels instead of amplitude/mul.</param>
        public void SetInputVolume(string inputName, float inputVolume, bool inputVolumeDb = false)
        {
            var requestFields = new JsonObject
            {
                { nameof(inputName), inputName }
            };

            if (inputVolumeDb)
            {
                requestFields.Add(REQUEST_FIELD_VOLUME_DB, inputVolume);
            }
            else
            {
                requestFields.Add(REQUEST_FIELD_VOLUME_MUL, inputVolume);
            }

            SendRequest(nameof(SetInputVolume), requestFields);
        }

        /// <summary>
        /// Get the volume of the specified source
        /// Volume is between `0.0` and `1.0` if using amplitude/mul (useDecibel is false), under `0.0` if using dB (useDecibel is true).
        /// </summary>
        /// <param name="inputName">Source name</param>
        /// <returns>An <see cref="VolumeInfo"/>Object containing the volume and mute state of the specified source.</returns>
        public JsonObject GetInputVolume(string inputName)
        {
            var request = new JsonObject
            {
                { nameof(inputName), inputName }
            };

            var response = SendRequest(nameof(GetInputVolume), request);
            return response;
        }

        /// <summary>
        /// Gets the audio mute state of an input.
        /// </summary>
        /// <param name="inputName">Name of input to get the mute state of</param>
        /// <returns>Whether the input is muted</returns>
        public bool GetInputMute(string inputName)
        {
            var requestFields = new JsonObject
            {
                { nameof(inputName), inputName }
            };

            var response = SendRequest(nameof(GetInputMute), requestFields);
            return (bool)response["inputMuted"];
        }

        /// <summary>
        /// Set the mute state of the specified source
        /// </summary>
        /// <param name="inputName">Name of the source which mute state will be changed</param>
        /// <param name="inputMuted">Desired mute state</param>
        public void SetInputMute(string inputName, bool inputMuted)
        {
            var requestFields = new JsonObject
            {
                { nameof(inputName), inputName },
                { nameof(inputMuted), inputMuted }
            };

            SendRequest(nameof(SetInputMute), requestFields);
        }

        /// <summary>
        /// Toggle the mute state of the specified source
        /// </summary>
        /// <param name="inputName">Name of the source which mute state will be toggled</param>
        public void ToggleInputMute(string inputName)
        {
            var requestFields = new JsonObject
            {
                { nameof(inputName), inputName }
            };

            SendRequest(nameof(ToggleInputMute), requestFields);
        }

        /// <summary>
        /// Sets the transform and crop info of a scene item
        /// </summary>
        /// <param name="sceneName">Name of the scene that has the SceneItem</param>
        /// <param name="sceneItemId">Id of the Scene Item</param>
        /// <param name="sceneItemTransform">JsonObject holding transform settings</param>
        public void SetSceneItemTransform(string sceneName, int sceneItemId, JsonObject sceneItemTransform)
        {
            var requestFields = new JsonObject
            {
                { nameof(sceneName), sceneName },
                { nameof(sceneItemId), sceneItemId },
                { nameof(sceneItemTransform), sceneItemTransform }
            };

            SendRequest(nameof(SetSceneItemTransform), requestFields);
        }


        /// <summary>
        /// Start streaming. Will trigger an error if streaming is already active
        /// </summary>
        public void StartStream()
        {
            SendRequest(nameof(StartStream));
        }

        /// <summary>
        /// Stop streaming. Will trigger an error if streaming is not active.
        /// </summary>
        public void StopStream()
        {
            SendRequest(nameof(StopStream));
        }

        /// <summary>
        /// Start recording. Will trigger an error if recording is already active.
        /// </summary>
        public void StartRecord()
        {
            SendRequest(nameof(StartRecord));
        }

        /// <summary>
        /// Stop recording. Will trigger an error if recording is not active.
        /// <returns>File name for the saved recording</returns>
        /// </summary>
        public string StopRecord()
        {
            var response = SendRequest(nameof(StopRecord));
            return (string)response["outputPath"];
        }

        /// <summary>
        /// Pause the current recording. Returns an error if recording is not active or already paused.
        /// </summary>
        public void PauseRecord()
        {
            SendRequest(nameof(PauseRecord));
        }

        /// <summary>
        /// Resume/unpause the current recording (if paused). Returns an error if recording is not active or not paused.
        /// </summary>
        public void ResumeRecord()
        {
            SendRequest(nameof(ResumeRecord));
        }


        /// <summary>
        /// Get current recording status.
        /// </summary>
        /// <returns></returns>
        public JsonObject GetRecordStatus()
        {
            var response = SendRequest(nameof(GetRecordStatus));
            return response;
        }


        /// <summary>
        /// Removes a scene item from a scene.
        /// Scenes only.
        /// </summary>
        /// <param name="sceneItemId">Scene item id</param>
        /// <param name="sceneName">Scene name from which to delete item</param>
        public void RemoveSceneItem(string sceneName, int sceneItemId)
        {
            var requestFields = new JsonObject
            {
                { nameof(sceneName), sceneName },
                { nameof(sceneItemId), sceneItemId }
            };

            SendRequest(nameof(RemoveSceneItem), requestFields);
        }


        /// <summary>
        /// Gets a list of all scene items in a scene.
        /// Scenes only
        /// </summary>
        /// <param name="sceneName">Name of the scene to get the items of</param>
        /// <returns>Array of scene items in the scene</returns>
        public List<JsonObject> GetSceneItemList(string sceneName)
        {
            JsonObject request = null;
            if (!string.IsNullOrEmpty(sceneName))
            {
                request = new JsonObject
                {
                    { nameof(sceneName), sceneName }
                };
            }

            var response = SendRequest(nameof(GetSceneItemList), request);
            return response["sceneItems"]?.AsArray().Select(m => m.Deserialize<JsonNode>().AsObject()).ToList()??new List<JsonObject>();
        }


        /// <summary>
        /// Gets data about the current plugin and RPC version.
        /// </summary>
        /// <returns>Version info in an <see cref="ObsVersion"/> object</returns>
        public JsonObject GetVersion()
        {
            JsonObject response = SendRequest(nameof(GetVersion));
            return response;
        }

        
        /// <summary>
        /// Gets an array of all inputs in OBS.
        /// </summary>
        /// <param name="inputKind">Restrict the array to only inputs of the specified kind</param>
        /// <returns>List of Inputs in OBS</returns>
        public List<JsonObject> GetInputList(string inputKind = null)
        {
            var request = new JsonObject
            {
                { nameof(inputKind), inputKind }
            };

            var response = inputKind is null
                ? SendRequest(nameof(GetInputList))
                : SendRequest(nameof(GetInputList), request);

            var returnList = new List<JsonObject>();
            foreach (var input in response["inputs"].AsArray())
            {
                returnList.Add(input.Deserialize<JsonNode>().AsObject());
            }

            return returnList;
        }

        /// <summary>
        /// Gets an array of all available input kinds in OBS.
        /// </summary>
        /// <param name="unversioned">True == Return all kinds as unversioned, False == Return with version suffixes (if available)</param>
        /// <returns>Array of input kinds</returns>
        public List<string> GetInputKindList(bool unversioned = false)
        {
            var request = new JsonObject
            {
                { nameof(unversioned), unversioned }
            };

            var response = unversioned is false
                ? SendRequest(nameof(GetInputKindList))
                : SendRequest(nameof(GetInputKindList), request);

            return JsonSerializer.Deserialize<List<string>>(response["inputKinds"].ToJsonString());
        }

        /// <summary>
        /// Removes an existing input.
        /// Note: Will immediately remove all associated scene items.
        /// </summary>
        /// <param name="inputName">Name of the input to remove</param>
        public void RemoveInput(string inputName)
        {
            var request = new JsonObject
            {
                { nameof(inputName), inputName }
            };

            SendRequest(nameof(RemoveInput), request);
        }

        /// <summary>
        /// Sets the name of an input (rename).
        /// </summary>
        /// <param name="inputName">Current input name</param>
        /// <param name="newInputName">New name for the input</param>
        public void SetInputName(string inputName, string newInputName)
        {
            var request = new JsonObject
            {
                { nameof(inputName), inputName },
                { nameof(newInputName), newInputName }
            };

            SendRequest(nameof(SetInputName), request);
        }

        /// <summary>
        /// Gets the settings of an input.
        /// Note: Does not include defaults. To create the entire settings object, overlay `inputSettings` over the `defaultInputSettings` provided by `GetInputDefaultSettings`.
        /// </summary>
        /// <param name="inputName">Name of the input to get the settings of</param>
        /// <returns>New populated InputSettings object</returns>
        public JsonObject GetInputSettings(string inputName)
        {
            var request = new JsonObject
            {
                { nameof(inputName), inputName }
            };

            var response = SendRequest(nameof(GetInputSettings), request);
            return response;
        }


        /// <summary>
        /// Sets the settings of an input.
        /// </summary>
        /// <param name="inputName">Name of the input to set the settings of</param>
        /// <param name="inputSettings">Object of settings to apply</param>
        /// <param name="overlay">True == apply the settings on top of existing ones, False == reset the input to its defaults, then apply settings.</param>
        public void SetInputSettings(string inputName, JsonObject inputSettings, bool overlay = true)
        {
            var request = new JsonObject
            {
                { nameof(inputName), inputName },
                { nameof(inputSettings), inputSettings },
                { nameof(overlay), overlay }
            };

            SendRequest(nameof(SetInputSettings), request);
        }


        /// <summary>
        /// Gets the status of a media input.\n\nMedia States:
        /// - `OBS_MEDIA_STATE_NONE`
        /// - `OBS_MEDIA_STATE_PLAYING`
        /// - `OBS_MEDIA_STATE_OPENING`
        /// - `OBS_MEDIA_STATE_BUFFERING`
        /// - `OBS_MEDIA_STATE_PAUSED`
        /// - `OBS_MEDIA_STATE_STOPPED`
        /// - `OBS_MEDIA_STATE_ENDED`
        /// - `OBS_MEDIA_STATE_ERROR`
        /// </summary>
        /// <param name="inputName">Name of the media input</param>
        /// <returns>Object containing string mediaState, int mediaDuration, int mediaCursor properties</returns>
        public JsonObject GetMediaInputStatus(string inputName)
        {
            var request = new JsonObject
            {
                { nameof(inputName), inputName }
            };

            return SendRequest(nameof(GetMediaInputStatus), request);
        }

        /// <summary>
        /// Currently BROKEN in OBS-websocket/OBS-studio
        /// Basically GetSceneItemList, but for groups.
        /// Using groups at all in OBS is discouraged, as they are very broken under the hood.
        /// Groups only
        /// </summary>
        /// <param name="sceneName">Name of the group to get the items of</param>
        /// <returns>Array of scene items in the group</returns>
        public List<JsonObject> GetGroupSceneItemList(string sceneName)
        {
            var request = new JsonObject
            {
                { nameof(sceneName), sceneName }
            };

            var response = SendRequest(nameof(GetGroupSceneItemList), request);
            return response["sceneItems"]?.AsArray().Select(m =>m.Deserialize<JsonNode>().AsObject()).ToList()??new List<JsonObject>();
        }

        /// <summary>
        /// Searches a scene for a source, and returns its id.\n\nScenes and Groups
        /// </summary>
        /// <param name="sceneName">Name of the scene or group to search in</param>
        /// <param name="sourceName">Name of the source to find</param>
        /// <param name="searchOffset">Number of matches to skip during search. >= 0 means first forward. -1 means last (top) item</param>
        /// <returns>Numeric ID of the scene item</returns>
        public int GetSceneItemId(string sceneName, string sourceName, int searchOffset)
        {
            var request = new JsonObject
            {
                { nameof(sceneName), sceneName },
                { nameof(sourceName), sourceName },
                { nameof(searchOffset), searchOffset }
            };

            var response = SendRequest(nameof(GetSceneItemId), request);
            return (int)response["sceneItemId"];
        }

        /// <summary>
        /// Gets the transform and crop info of a scene item.
        /// Scenes and Groups
        /// </summary>
        /// <param name="sceneName">Name of the scene the item is in</param>
        /// <param name="sceneItemId">Numeric ID of the scene item</param>
        /// <returns>Object containing scene item transform info</returns>
        public JsonObject GetSceneItemTransform(string sceneName, int sceneItemId)
        {
            var response = GetSceneItemTransformRaw(sceneName, sceneItemId);
            return (JsonObject)response["sceneItemTransform"];
        }

        /// <summary>
        /// Gets the JsonObject of transform settings for a scene item. Use this one you don't want it populated with default values.
        /// Scenes and Groups
        /// </summary>
        /// <param name="sceneName">Name of the scene the item is in</param>
        /// <param name="sceneItemId">Numeric ID of the scene item</param>
        /// <returns>Object containing scene item transform info</returns>
        public JsonObject GetSceneItemTransformRaw(string sceneName, int sceneItemId)
        {
            var request = new JsonObject
            {
                { nameof(sceneName), sceneName },
                { nameof(sceneItemId), sceneItemId }
            };

            return SendRequest(nameof(GetSceneItemTransform), request);
        }

        /// <summary>
        /// Gets the enable state of a scene item.
        /// Scenes and Groups
        /// </summary>
        /// <param name="sceneName">Name of the scene the item is in</param>
        /// <param name="sceneItemId">Numeric ID of the scene item</param>
        /// <returns>Whether the scene item is enabled. `true` for enabled, `false` for disabled</returns>
        public bool GetSceneItemEnabled(string sceneName, int sceneItemId)
        {
            var request = new JsonObject
            {
                { nameof(sceneName), sceneName },
                { nameof(sceneItemId), sceneItemId }
            };

            var response = SendRequest(nameof(GetSceneItemEnabled), request);
            return (bool)response["sceneItemEnabled"];
        }

        /// <summary>
        /// Gets the enable state of a scene item.
        /// Scenes and Groups
        /// </summary>
        /// <param name="sceneName">Name of the scene the item is in</param>
        /// <param name="sceneItemId">Numeric ID of the scene item</param>
        /// <param name="sceneItemEnabled">New enable state of the scene item</param>
        public void SetSceneItemEnabled(string sceneName, int sceneItemId, bool sceneItemEnabled)
        {
            var request = new JsonObject
            {
                { nameof(sceneName), sceneName },
                { nameof(sceneItemId), sceneItemId },
                { nameof(sceneItemEnabled), sceneItemEnabled }
            };

            SendRequest(nameof(SetSceneItemEnabled), request);
        }

        /// <summary>
        /// Gets the lock state of a scene item.
        /// Scenes and Groups
        /// </summary>
        /// <param name="sceneName">Name of the scene the item is in</param>
        /// <param name="sceneItemId">Numeric ID of the scene item</param>
        /// <returns>Whether the scene item is locked. `true` for locked, `false` for unlocked</returns>
        public bool GetSceneItemLocked(string sceneName, int sceneItemId)
        {
            var request = new JsonObject
            {
                { nameof(sceneName), sceneName },
                { nameof(sceneItemId), sceneItemId }
            };

            var response = SendRequest(nameof(GetSceneItemLocked), request);
            return (bool)response["sceneItemLocked"];
        }

        /// <summary>
        /// Sets the lock state of a scene item.
        /// Scenes and Group
        /// </summary>
        /// <param name="sceneName">Name of the scene the item is in</param>
        /// <param name="sceneItemId">Numeric ID of the scene item</param>
        /// <param name="sceneItemLocked">New lock state of the scene item</param>
        public void SetSceneItemLocked(string sceneName, int sceneItemId, bool sceneItemLocked)
        {
            var request = new JsonObject
            {
                { nameof(sceneName), sceneName },
                { nameof(sceneItemId), sceneItemId },
                { nameof(sceneItemLocked), sceneItemLocked }
            };

            SendRequest(nameof(SetSceneItemLocked), request);
        }

        /// <summary>
        /// Gets the index position of a scene item in a scene.
        /// An index of 0 is at the bottom of the source list in the UI.
        /// Scenes and Groups
        /// </summary>
        /// <param name="sceneName">Name of the scene the item is in</param>
        /// <param name="sceneItemId">Numeric ID of the scene item</param>
        /// <returns>Index position of the scene item</returns>
        public int GetSceneItemIndex(string sceneName, int sceneItemId)
        {
            var request = new JsonObject
            {
                { nameof(sceneName), sceneName },
                { nameof(sceneItemId), sceneItemId }
            };

            var response = SendRequest(nameof(GetSceneItemIndex), request);
            return (int)response["sceneItemIndex"];
        }

        /// <summary>
        /// Sets the index position of a scene item in a scene.
        /// Scenes and Groups
        /// </summary>
        /// <param name="sceneName">Name of the scene the item is in</param>
        /// <param name="sceneItemId">Numeric ID of the scene item</param>
        /// <param name="sceneItemIndex">New index position of the scene item</param>
        public void SetSceneItemIndex(string sceneName, int sceneItemId, int sceneItemIndex)
        {
            var request = new JsonObject
            {
                { nameof(sceneName), sceneName },
                { nameof(sceneItemId), sceneItemId },
                { nameof(sceneItemIndex), sceneItemIndex }
            };

            SendRequest(nameof(SetSceneItemIndex), request);
        }


        /// <summary>
        /// Removes a scene from OBS.
        /// </summary>
        /// <param name="sceneName">Name of the scene to remove</param>
        public void RemoveScene(string sceneName)
        {
            var request = new JsonObject
            {
                { nameof(sceneName), sceneName }
            };

            SendRequest(nameof(RemoveScene), request);
        }

        /// <summary>
        /// Sets the name of a scene (rename).
        /// </summary>
        /// <param name="sceneName">Name of the scene to be renamed</param>
        /// <param name="newSceneName">New name for the scene</param>
        public void SetSceneName(string sceneName, string newSceneName)
        {
            var request = new JsonObject
            {
                { nameof(sceneName), sceneName },
                { nameof(newSceneName), newSceneName }
            };

            SendRequest(nameof(SetSceneName), request);
        }
    }
}