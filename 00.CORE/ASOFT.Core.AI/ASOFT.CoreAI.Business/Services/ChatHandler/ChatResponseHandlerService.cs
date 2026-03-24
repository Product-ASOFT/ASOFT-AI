using ASOFT.Core.Common.InjectionChecker;
using ASOFT.CoreAI.Abstractions;
using ASOFT.CoreAI.Abstractions.PromptTemplate;
using ASOFT.CoreAI.Business.Services.ChatHandler;
using ASOFT.CoreAI.Common;
using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Infrastructure;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ASOFT.CoreAI.Business
{
    public class ChatResponseHandlerService : IChatResponseHandlerService
    {
        private readonly SettingsManagerService _settingsManagerService;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger _logger;
        public ChatResponseHandlerService(SettingsManagerService settingsManagerService,
            IHttpClientFactory clientFactory,
            ILoggerFactory logger)
        {
            _settingsManagerService = settingsManagerService;
            _clientFactory = clientFactory;
            _logger = Checker.NotNull(logger, nameof(logger)).CreateLogger(GetType());
        }
        public async Task<ItemChatResponse> InvokeAsync(string promptSystem, string question)
        {
            // 1. Lấy cấu hình LLM
            var llmConfig = await _settingsManagerService.GetConfigLLMsAsync();
            if (llmConfig == null || string.IsNullOrWhiteSpace(llmConfig.BaseUrl))
            {
                return new ItemChatResponse
                {
                    Text = "Chưa có thông tin kết nối với model LLM"
                };
            }

            // 2. Tạo HttpClient
            var httpClient = _clientFactory.CreateClient("LLM");

            httpClient.Timeout = Timeout.InfiniteTimeSpan;

            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(15));

            var requestBody = new
            {
                messages = new[]
                {

                    new
                    {
                        role = AIRoleName.ROLE_SYSTEM,
                        content = promptSystem
                    },
                    new
                    {
                        role = AIRoleName.ROLE_USER,
                        content = question
                    }
                },
                max_new_tokens = llmConfig.MaxToken,
                temperature = llmConfig.Temperature
            };

            var requestJson = JsonSerializer.Serialize(requestBody);

            using var request = new HttpRequestMessage(HttpMethod.Post, llmConfig.BaseUrl)
            {
                Content = new StringContent(requestJson, Encoding.UTF8, "application/json")
            };

            var sw = Stopwatch.StartNew();

            try
            {
                // 4. Gửi request
                var response = await httpClient.SendAsync(
                    request,
                    HttpCompletionOption.ResponseHeadersRead,
                    cts.Token);

                // 5. Check HTTP status
                if (!response.IsSuccessStatusCode)
                {
                    sw.Stop();

                    var errorBody = await response.Content.ReadAsStringAsync();
                    _logger.LogError(
                        "LLM trả HTTP {StatusCode} sau {Elapsed}s. Body: {Body}",
                        response.StatusCode,
                        sw.Elapsed.TotalSeconds,
                        errorBody);

                    return new ItemChatResponse
                    {
                        Text = "LLM phản hồi không thành công"
                    };
                }
                // 6. Đọc response body
                var content = await response.Content.ReadAsStringAsync(cts.Token);
                sw.Stop();

                // 7. Check response rỗng
                if (string.IsNullOrWhiteSpace(content))
                {
                    _logger.LogError(
                        "LLM trả response rỗng sau {Elapsed}s",
                        sw.Elapsed.TotalSeconds);

                    return new ItemChatResponse
                    {
                        Text = "LLM không trả dữ liệu"
                    };
                }

                content = content.Trim();

                // 8. Check có phải JSON không
                if (!content.StartsWith("{") && !content.StartsWith("["))
                {
                    _logger.LogError(
                        "LLM trả dữ liệu không phải JSON sau {Elapsed}s. Raw: {Content}",
                        sw.Elapsed.TotalSeconds,
                        content);

                    return new ItemChatResponse
                    {
                        Text = "LLM trả dữ liệu không đúng định dạng JSON"
                    };
                }

                // 9. Parse JSON
                try
                {
                    var jsonOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var result = JsonSerializer.Deserialize<ItemChatResponse>(content, jsonOptions);

                    if (result == null)
                    {
                        _logger.LogError("Lỗi không có thông tin được parse JSON từ LLM. Raw response: {Content}", content);
                        throw new JsonException("Deserialize trả về null");
                    }

                    return result;
                }
                catch (JsonException ex)
                {
                    _logger.LogError(
                        ex,
                        "Lỗi parse JSON từ LLM. Raw response: {Content}",
                        content);

                    return new ItemChatResponse
                    {
                        Text = "Lỗi parse dữ liệu từ LLM"
                    };
                }
            }
            catch (OperationCanceledException ex) when (cts.IsCancellationRequested)
            {
                sw.Stop();
                _logger.LogError(
                    ex,
                    "Gọi LLM bị timeout sau {Elapsed}s",
                    sw.Elapsed.TotalSeconds);

                return new ItemChatResponse
                {
                    Text = "Gọi LLM bị timeout"
                };
            }
            catch (HttpRequestException ex)
            {
                sw.Stop();
                _logger.LogError(
                    ex,
                    "Lỗi kết nối LLM sau {Elapsed}s",
                    sw.Elapsed.TotalSeconds);

                return new ItemChatResponse
                {
                    Text = "Không thể kết nối tới LLM"
                };
            }
            catch (Exception ex)
            {
                sw.Stop();
                _logger.LogError(
                    ex,
                    "Lỗi không xác định khi gọi LLM sau {Elapsed}s",
                    sw.Elapsed.TotalSeconds);

                return new ItemChatResponse
                {
                    Text = "Lỗi không xác định khi gọi LLM"
                };
            }
        }

        public async Task<ItemChatResponse> InvokePromptAsync(string promptSystem, string promptContent, KernelArguments? arguments = null)
        {
            var result = new ItemChatResponse();
            try
            {
                string promptRender = HandlebarsRenderer.RenderPrompt(promptContent, arguments);
                if (string.IsNullOrEmpty(promptRender))
                {
                    result.Text = "Không tồn tại Prompt!";
                    return result;
                }
                result = await InvokeAsync(promptSystem, promptRender);
            }
            catch (Exception)
            {

                return new ItemChatResponse
                {
                    Text = "Lỗi parse dữ liệu từ LLM",
                };
            }
            return result;
        }
    }
}
