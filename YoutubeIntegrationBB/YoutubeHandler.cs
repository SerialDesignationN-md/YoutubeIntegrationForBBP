using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace YoutubeIntegrationBB
{
    public class ChatMessage
    {
        public string Author;
        public string Message;
    }

    public class YouTubeChatListener : MonoBehaviour
    {
        private string apiKey = null; // Your YouTube API key
        private string videoId = null; // Target YouTube livestream Video ID
        private string liveChatId;
        private string nextPageToken;
        public bool isConnected = true;

        internal Action<ChatMessage> callback;

        private HttpClient httpClient = new HttpClient();

        private async void Start()
        {
            DontDestroyOnLoad(this.gameObject);

            apiKey = BasePlugin.Instance.ApiKey.Value;
            videoId = BasePlugin.Instance.VideoID.Value;

            liveChatId = await GetLiveChatIdAsync(videoId);
            if (string.IsNullOrEmpty(liveChatId) || string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(videoId)) 
            {
                Debug.LogError("Failed to get liveChatId. Check your API key and Video ID.");
                isConnected = false;
                return;
            }

            Debug.Log("YouTube Chat Listener started!");
            _ = PollChatLoop();
        }

        private async Task<string> GetLiveChatIdAsync(string videoId)
        {
            try
            {
                string url = $"https://www.googleapis.com/youtube/v3/videos?part=liveStreamingDetails&id={videoId}&key={apiKey}";
                var response = await httpClient.GetStringAsync(url);
                JObject json = JObject.Parse(response);
                Debug.Log(json);
                return (string)json["items"]?[0]?["liveStreamingDetails"]?["activeLiveChatId"];
            }
            catch (Exception ex)
            {
                isConnected = false;
                Debug.LogError($"Error getting liveChatId: {ex.Message}");
                return null;
            }
        }

        private async Task PollChatLoop()
        {
            while (true)
            {
                var messages = await GetLiveChatMessagesAsync();
                foreach (var chatMsg in messages)
                {
                    HandleChatMessage(chatMsg, callback);
                }
                await Task.Delay(7000);
            }
        }

        private async Task<List<ChatMessage>> GetLiveChatMessagesAsync()
        {
            var messages = new List<ChatMessage>();
            try
            {
                string url = $"https://www.googleapis.com/youtube/v3/liveChat/messages?liveChatId={liveChatId}&part=snippet,authorDetails&key={apiKey}";
                if (!string.IsNullOrEmpty(nextPageToken))
                    url += $"&pageToken={nextPageToken}";

                var response = await httpClient.GetStringAsync(url);
                JObject json = JObject.Parse(response);

                nextPageToken = (string)json["nextPageToken"];

                foreach (var item in json["items"])
                {
                    string messageText = (string)item["snippet"]?["displayMessage"];
                    string author = (string)item["authorDetails"]?["displayName"];

                    if (!string.IsNullOrEmpty(author) && !string.IsNullOrEmpty(messageText))
                    {
                        messages.Add(new ChatMessage
                        {
                            Author = author,
                            Message = messageText
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error polling chat messages: {ex.Message}");
            }

            return messages;
        }

        private void HandleChatMessage(ChatMessage chatMessage, Action<ChatMessage> action)
        {
            if (chatMessage == null || action == null)
                return;

            Debug.Log($"[{chatMessage.Author}] {chatMessage.Message}");
            action.Invoke(chatMessage);
        }
    }
}
