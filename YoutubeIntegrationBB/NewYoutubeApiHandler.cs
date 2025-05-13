using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using YTLiveChat.Services;
using YTLiveChat.Contracts;
using YTLiveChat.Contracts.Services;
using System.Text.Json;
using YTLiveChat.Contracts.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using JetBrains.Annotations;
using UnityEngine;
namespace YoutubeIntegrationBB
{
    public class NewYoutubeApiHandler : IDisposable
    {
        private readonly ILogger<NewYoutubeApiHandler> _logger;

        private readonly HttpClient _httpClient;
        internal IYTLiveChat _ytLiveChat;
        private readonly ILogger<YTLiveChat.Services.YTLiveChat> _chatLogger = NullLogger<YTLiveChat.Services.YTLiveChat>.Instance;
        private readonly ILogger<YTHttpClient> _httpClientLogger = NullLogger<YTHttpClient>.Instance;
        private bool _disposed = false;
        public NewYoutubeApiHandler()
        {
            var options = new YTLiveChatOptions
            {
                RequestFrequency = 2000,
                YoutubeBaseUrl = "https://www.youtube.com"
            };

            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri(options.YoutubeBaseUrl)
            };

            UnityLoggerFactory factory = new();

            _logger = factory.CreateLogger<NewYoutubeApiHandler>();
            var httpClientLogger = factory.CreateLogger<YTHttpClient>();
            var chatLogger = factory.CreateLogger<YTLiveChat.Services.YTLiveChat>();

            var ytHttpClient = new YTHttpClient(_httpClient,_httpClientLogger);

            _ytLiveChat = new YTLiveChat.Services.YTLiveChat(options, ytHttpClient,_chatLogger);

            _ytLiveChat.InitialPageLoaded += OnLoaded;
            _ytLiveChat.ErrorOccurred += Onerror;
            _ytLiveChat.ChatReceived += OnRecieve;

        }

        public void Begin(string liveid)
        {
            _logger.LogInformation("Starting YTLiveChat");
            _ytLiveChat.Start(handle: liveid);
            _logger.LogInformation("Started");
        }

        public void Stop(string liveid)
        {
            _logger.LogInformation("Stopping YTLiveChat");
            _ytLiveChat.Stop();
            _logger.LogInformation("Stopped");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void OnLoaded(object? sender, InitialPageLoadedEventArgs e)
        {
            _logger.LogInformation("Loaded {EventData}", e);

        }

        private void OnRecieve(object? sender, ChatReceivedEventArgs e)
        {
            var CPH = BasePlugin.Instance.CPH;
            _logger.LogInformation("new chat", e);
            if (CPH.canVote)
            {
                BasePlugin.Instance.CPH.VoteHandler(sender, e);
            }
        }

        private void Onerror(object? sender, ErrorOccurredEventArgs e)
        {
            _logger.LogError(e.GetException(), "Error occured.");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing) {
                    _ytLiveChat.Dispose();
                    _httpClient.Dispose();

                }
                _disposed = true;
            }
        }
    }
}
