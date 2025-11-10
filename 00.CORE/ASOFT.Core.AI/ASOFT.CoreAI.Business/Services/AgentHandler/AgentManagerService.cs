using ASOFT.CoreAI.Abstractions;
using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq.Dynamic.Core;
using static ASOFT.CoreAI.Common.AIConstants;

namespace ASOFT.CoreAI.Business;

public class AgentManagerService
{
    private readonly IChatHistoryHandler _chatHistoryHandler;
    private readonly IRedisService _redisHandler;
    private readonly SettingsManagerService _settingsManager;
    private readonly AgentPromptService _agentPromptService;
    private readonly ReadFileOrchestratorService _readFileOrchestratorService;

    public AgentManagerService(Kernel kernel,
        IChatHistoryHandler chatHistoryHandler,
        SettingsManagerService settingsManager,
        IRedisService redisHandler, 
        IRedisMemoryProvider redisMemoryProvider,
        AgentPromptService agentPromptService,
        ReadFileOrchestratorService readFileOrchestratorService)
    {
        _chatHistoryHandler = chatHistoryHandler;
        _settingsManager = settingsManager;
        _redisHandler = redisHandler;
        _agentPromptService = agentPromptService;
        _readFileOrchestratorService = readFileOrchestratorService;
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", _settingsManager.GetKeyReadOCR());
    }

    #region Api xử lý gọi Agent

    public async Task<ChatResponseModel> CallHandlerAgentAsync(AgentRequest request, CancellationToken cancellationToken = default)
    {
        if (request.PluginCodes == null || request.PluginCodes.Count == 0)
            return ChatHandlerHelper.CreateResponse(request.ChatSessionID, "Danh sách agent trống.");

        return await HandleAgentByMediatorAsync(request, cancellationToken);
    }

    #endregion Api xử lý gọi Agent

    #region Convert JObject từ API được sang đối tượng cụ thể

    public List<T> ParseItemsToModelList<T>(List<JObject>? items) where T : class
    {
        if (items == null || items.Count == 0)
            return new List<T>();

        var jsonArrayString = JsonConvert.SerializeObject(items);
        return JsonConvert.DeserializeObject<List<T>>(jsonArrayString) ?? new List<T>();
    }

    #endregion Convert JObject từ API được sang đối tượng cụ thể

    #region Xử lý gọi các Agent tương ứng với plugin code

    private async Task<ChatResponseModel> HandleAgentByMediatorAsync(AgentRequest request, CancellationToken cancellationToken = default)
    {
        if (request.PluginCodes == null || !request.PluginCodes.Any())
            return ChatHandlerHelper.CreateResponse(request.ChatSessionID, "Không có plugin code nào được cung cấp.");

        var handlers = GetAgentHandlers(request);

        foreach (var pluginCode in request.PluginCodes)
        {
            if (handlers.TryGetValue(pluginCode, out var handler))
            {
                var response = await handler();
                if (response != null && !string.IsNullOrEmpty(response.Result) && !IsNoDataMessage(response.Result))
                {
                    if ((response.ChatSessionID == Guid.Empty || response.ChatSessionID == null) && request.ChatSessionID != null && request.ChatSessionID != Guid.Empty)
                    {
                        response.ChatSessionID = request.ChatSessionID;
                    }
                    return response;
                }
            }
        }
        return ChatHandlerHelper.CreateResponse(request.ChatSessionID, "Module không hợp lệ hoặc agent không được hỗ trợ.");
    }

    private Dictionary<string, Func<Task<ChatResponseModel>>> GetAgentHandlers(AgentRequest request) => new()
    {
        [AgentKeys.OO_AGENT_OOF2110] = () => TryHandlePluginAsync<SimpleTaskInfo>(request, AgentKeys.OO_AGENT_OOF2110, "công việc"),
        [AgentKeys.OO_AGENT_OOF2160] = () => TryHandlePluginAsync<OOT2160ViewModel>(request, AgentKeys.OO_AGENT_OOF2160, "vấn đề"),
        [AgentKeys.OO_AGENT_OOF2190] = () => TryHandlePluginAsync<MilestoneViewModel>(request, AgentKeys.OO_AGENT_OOF2190, "milestone"),
        [AgentKeys.CRM_AGENT_CRMF2030] = () => TryHandlePluginAsync<CRMF2030ViewModel>(request, AgentKeys.CRM_AGENT_CRMF2030, "đầu mối"),
        [AgentKeys.CRM_AGENT_CRMF2050] = () => TryHandlePluginAsync<CRMF2050ViewModel>(request, AgentKeys.CRM_AGENT_CRMF2050, "cơ hội"),
        [AgentKeys.CRM_AGENT_CRMF2160] = () => TryHandlePluginAsync<CRMF2160ViewModel>(request, AgentKeys.CRM_AGENT_CRMF2160, "yêu cầu hỗ trợ"),
        [AgentKeys.RESEARCH_AGENT] = () => TryHandlePluginAsync<RedisearchResultItem>(request, AgentKeys.RESEARCH_AGENT, "tra cứu thông tin"),
        [AgentKeys.READFILE_AGENT] = () => TryHandlePluginAsync<RedisearchResultItem>(request, AgentKeys.READFILE_AGENT, "Đọc thông tin"),
        [AgentKeys.BEM_AGENT_BEMF2000] = () => TryHandlePluginAsync<BEMF2000ViewModel>(request, AgentKeys.BEM_AGENT_BEMF2000, "DNTT/DNTTTU/DNTU"),
        [AgentKeys.HRM_AGENT_HRMF2220] = () => TryHandlePluginAsync<BEMF2000ViewModel>(request, AgentKeys.HRM_AGENT_HRMF2220, "Chấm công")
    };

    private bool IsNoDataMessage(string message) => message.StartsWith("Không có");

    #endregion Xử lý gọi các Agent tương ứng với plugin code

    #region xử lý các plugin cụ thể

    private async Task<ChatResponseModel> TryHandlePluginAsync<T>(AgentRequest request, string agentCode, string dataName) where T : class
    {
        var items = ParseItemsToModelList<T>(request.Items);

        if ((items == null || !items.Any()) && agentCode != AgentKeys.READFILE_AGENT)
            return ChatHandlerHelper.CreateResponse(request.ChatSessionID, $"Không có {dataName} nào phù hợp với yêu cầu của bạn.");

        var pluginData = new Dictionary<string, object> { [agentCode] = items };

        return await HandleByFilterAsync(request, agentCode, pluginData);
    }

    private bool TryGetPluginData<T>(IDictionary<string, object> pluginData, string pluginCode, out List<T>? data)
    {
        data = null;

        if (!pluginData.TryGetValue(pluginCode, out var obj))
            return false;

        if (obj is List<T> list && list.Any())
        {
            data = list;
            _ = ExtractUrlFromData(data);
            return true;
        }
        else if (obj is List<JObject> jObjects && jObjects.Any())
        {
            data = [.. jObjects.Select(x => x.ToObject<T>())];
            _ = ExtractUrlFromData(data);
            return true;
        }
        return false;
    }

    private async Task ExtractUrlFromData<T>(List<T> data)
    {
        var prop = typeof(T).GetProperty("Url");
        if (prop == null || !prop.CanWrite)
            return;
        string baseUrl = await _settingsManager.GetExternalApi();

        if (string.IsNullOrEmpty(baseUrl))
            return;

        foreach (var item in data)
            prop.SetValue(item, baseUrl);
    }

    #endregion xử lý các plugin cụ thể

    #region Xử lý tạo câu trả lời với Agent tương ứng

    private async Task<ChatResponseModel> HandleByFilterAsync(AgentRequest request, string agentCode, IDictionary<string, object> pluginData)
    {
        var valueRecords = _settingsManager.GetNumberRecords();
        var chatHistoryModel = _chatHistoryHandler.CreateChatHistoryModel(request, agentCode, valueRecords.maxChat);

        #region Lấy dữ liệu từ Redis Vector Store và lịch sử chat

        string indexName = AgentKeyHelper.GetIndexKey(agentCode);
        // lấy dữ liệu train
        var trainingData = await _redisHandler.GetDataTrainingAsync(request, indexName, valueRecords.maxTraining);

        // Lấy lich sử chat từ cơ sở dữ liệu
        var chatHistory = await _chatHistoryHandler.GetChatHistoryAsync(chatHistoryModel);

        #endregion Lấy dữ liệu từ Redis Vector Store và lịch sử chat

        // Lưu câu hỏi vào lịch sử chat
        var chatMessages = await _chatHistoryHandler.SaveChatMessageAsync(chatHistoryModel);
        string answer = await GenerateAnswerAsync(request, pluginData, agentCode, indexName, chatHistory, trainingData, valueRecords.maxTraining);

        if (string.IsNullOrEmpty(answer))
            answer = "Chưa có thông tin nào phù hợp với câu hỏi của bạn. Bạn có thể bổ sung thêm những thông tin chi tiết hơn được không?";

        if (chatMessages != null && chatMessages.APK != Guid.Empty)
            await _chatHistoryHandler.SaveChatResponseAsync(answer, chatMessages);

        return ChatHandlerHelper.CreateResponse(chatMessages?.ChatSessionID ?? Guid.Empty, answer);
    }

    private async Task<string> GenerateAnswerAsync(
        AgentRequest request,
        IDictionary<string, object> pluginData,
        string agentCode,
        string indexName,
        IEnumerable<ChatHistoryResponseModel> chatHistory,
        IEnumerable<RedisearchResultItem> trainingData,
        int limit)
    {
        bool isCheckData = true;

        var agentMap = new List<(string Key, object? Data)>
            {
                (AgentKeys.OO_AGENT_OOF2110, TryGetPluginData(pluginData, AgentKeys.OO_AGENT_OOF2110, out List<SimpleTaskInfo>? simpleTasks) ? simpleTasks : null),
                (AgentKeys.OO_AGENT_OOF2160, TryGetPluginData(pluginData, AgentKeys.OO_AGENT_OOF2160, out List<OOT2160ViewModel>? issues) ? issues : null),
                (AgentKeys.OO_AGENT_OOF2190, TryGetPluginData(pluginData, AgentKeys.OO_AGENT_OOF2190, out List<MilestoneViewModel>? milestones) ? milestones : null),
                (AgentKeys.CRM_AGENT_CRMF2030, TryGetPluginData(pluginData, AgentKeys.CRM_AGENT_CRMF2030, out List<CRMF2030ViewModel>? keyContacts) ? keyContacts : null),
                (AgentKeys.CRM_AGENT_CRMF2050, TryGetPluginData(pluginData, AgentKeys.CRM_AGENT_CRMF2050, out List<CRMF2050ViewModel>? opportunities) ? opportunities : null),
                (AgentKeys.CRM_AGENT_CRMF2160, TryGetPluginData(pluginData, AgentKeys.CRM_AGENT_CRMF2160, out List<CRMF2160ViewModel>? supportRequests) ? supportRequests : null)
            };

        foreach (var (key, data) in agentMap)
        {
            if (data == null) continue;

            string promptTemplate = await _agentPromptService.GetPromptTemplate(key);
            if (string.IsNullOrWhiteSpace(promptTemplate))
                return $"Hiện tại bạn chưa tạo Prompt cho agent `{key}`. Vui lòng thiết lập Prompt để tiếp tục.";

            switch (key)
            {
                case AgentKeys.OO_AGENT_OOF2110:
                    return await _agentPromptService.SendPromptWithAgentAsync(request, isCheckData, (List<SimpleTaskInfo>)data, chatHistory, promptTemplate, trainingData);

                case AgentKeys.OO_AGENT_OOF2160:
                    return await _agentPromptService.SendPromptWithAgentAsync(request, isCheckData, (List<OOT2160ViewModel>)data, chatHistory, promptTemplate, trainingData);

                case AgentKeys.OO_AGENT_OOF2190:
                    return await _agentPromptService.SendPromptWithAgentAsync(request, isCheckData, (List<MilestoneViewModel>)data, chatHistory, promptTemplate, trainingData);

                case AgentKeys.CRM_AGENT_CRMF2030:
                    return await _agentPromptService.SendPromptWithAgentAsync(request, isCheckData, (List<CRMF2030ViewModel>)data, chatHistory, promptTemplate, trainingData);

                case AgentKeys.CRM_AGENT_CRMF2050:
                    return await _agentPromptService.SendPromptWithAgentAsync(request, isCheckData, (List<CRMF2050ViewModel>)data, chatHistory, promptTemplate, trainingData);

                case AgentKeys.CRM_AGENT_CRMF2160:
                    return await _agentPromptService.SendPromptWithAgentAsync(request, isCheckData, (List<CRMF2160ViewModel>)data, chatHistory, promptTemplate, trainingData);
            }
        }
        // Agent tìm kiếm thông tin
        if (TryGetPluginData(pluginData, AgentKeys.RESEARCH_AGENT, out List<RedisearchResultItem>? redisearchResultItems))
        {
            string promptTemplate = await _agentPromptService.GetPromptTemplate(AgentKeys.RESEARCH_AGENT);
            if (string.IsNullOrWhiteSpace(promptTemplate))
                return "Hiện tại bạn chưa tạo Prompt tìm kiếm thông tin trong hệ thống. Vui lòng thiết lập Prompt để tiếp tục.";

            var answerOCR = new List<ResultReadFileModel>();
            return await _agentPromptService.SendPromptWithDocsAsync(request, promptTemplate, answerOCR, chatHistory, trainingData, redisearchResultItems);
        }

        // Agent đọc file OCR
        if (pluginData.ContainsKey(AgentKeys.READFILE_AGENT))
        {
            string promptTemplate = await _agentPromptService.GetPromptTemplate(AgentKeys.READFILE_AGENT);
            if (string.IsNullOrWhiteSpace(promptTemplate))
                return "Hiện tại bạn chưa tạo Prompt để đọc file. Vui lòng thiết lập Prompt để tiếp tục.";

            if(request.FilePaths == null || request.FilePaths.Count == 0)
                return "Vui lòng cung cấp đường dẫn file để đọc nội dung.";

            var answerOCRs = await _readFileOrchestratorService.ReadFileFromChatBot(request.FilePaths, request.ChatSessionID!.Value);
            trainingData = await _redisHandler.GetDataByReadFileAsync(request, indexName, limit);

            return await _agentPromptService.SendPromptWithDocsAsync(request, promptTemplate, answerOCRs, chatHistory, trainingData, new List<RedisearchResultItem>());
        }

        return "Agent chưa cung cấp thông tin phù hợp. Bạn có thể đặt lại câu hỏi cụ thể hơn không?";
    }
    #endregion Xử lý tạo câu trả lời với Agent tương ứng
}