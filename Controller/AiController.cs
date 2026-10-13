//using System.Net.Http.Headers;
//using System.Text;
//using System.Text.Json;
//using Microsoft.AspNetCore.Mvc;

//namespace IDS.API.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class AiController : ControllerBase
//    {
//        private readonly IHttpClientFactory _httpClientFactory;
//        private readonly IConfiguration _config;

//        public AiController(IHttpClientFactory httpClientFactory, IConfiguration config)
//        {
//            _httpClientFactory = httpClientFactory;
//            _config = config;
//        }

//        public class AskRequest { public string Message { get; set; } = string.Empty; }
//        public class AskResponse { public string Reply { get; set; } = string.Empty; }

//        [HttpPost("ask")]
//        public async Task<IActionResult> Ask([FromBody] AskRequest request)
//        {
//            if (string.IsNullOrWhiteSpace(request?.Message))
//                return BadRequest(new { error = "Message is required." });

//            const string apiKey = "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"; // ⚠️ rotate old key first, don't commit this file with the real key in it
//            // "openrouter/free" auto-selects a free model AND filters for tool-calling support
//            // (per OpenRouter's own docs), so this is the correct choice for this feature.
//            const string model = "openrouter/free";

//            var openRouter = _httpClientFactory.CreateClient();
//            openRouter.BaseAddress = new Uri("https://openrouter.ai/");
//            openRouter.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
//            openRouter.DefaultRequestHeaders.Add("HTTP-Referer", "https://your-domain.example.com");
//            openRouter.DefaultRequestHeaders.Add("X-Title", "Township UI Assistant");

//            // ── Tool definitions: only these 3 report APIs are callable, with only ──
//            // ── the parameters confirmed to exist on your real backend.            ──
//            var tools = new object[]
//            {
//                new {
//                    type = "function",
//                    function = new {
//                        name = "get_vehicle_inout_register",
//                        description = "Authorized vehicle IN/OUT entries. Use for questions about vehicles entering/exiting, today's traffic, IN-only, OUT-only, by project, vehicle type, or search by vehicle number/owner/flat.",
//                        parameters = new {
//                            type = "object",
//                            properties = new {
//                                fromDateTime = new { type = "string", description = "Format dd-MMM-yyyy, e.g. 29-Sep-2026" },
//                                toDateTime = new { type = "string", description = "Format dd-MMM-yyyy" },
//                                inOut = new { type = "string", @enum = new[] { "IN", "OUT" }, description = "Omit to get both" },
//                                nrd = new { type = "string", description = "Project/neighbourhood name" },
//                                vehicleType = new { type = "string", description = "e.g. Two Wheeler, Four Wheeler" },
//                                searchType = new { type = "string", @enum = new[] { "vehicleNumber", "ownerName", "flatNumber" } },
//                                searchValue = new { type = "string" }
//                            },
//                            required = new[] { "fromDateTime", "toDateTime" }
//                        }
//                    }
//                },
//                new {
//                    type = "function",
//                    function = new {
//                        name = "get_unauthorized_vehicle_inout_register",
//                        description = "Unauthorized vehicle IN/OUT entries — vehicles not matched to a registered vehicle. Use for 'unauthorized vehicle' questions.",
//                        parameters = new {
//                            type = "object",
//                            properties = new {
//                                fromDateTime = new { type = "string", description = "Format dd-MMM-yyyy" },
//                                toDateTime = new { type = "string", description = "Format dd-MMM-yyyy" },
//                                inOut = new { type = "string", @enum = new[] { "IN", "OUT" }, description = "Omit to get both" }
//                            },
//                            required = new[] { "fromDateTime", "toDateTime" }
//                        }
//                    }
//                },
//                                new {
//                    type = "function",
//                    function = new {
//                        name = "get_card_inout_register",
//                        description = "Card/turnstile IN-OUT access log (people entering/exiting via card readers). Use for questions about card in/out, turnstile activity, or access log.",
//                        parameters = new { type = "object", properties = new { } }
//                    }
//                },
//                new {
//                    type = "function",
//                    function = new {
//                        name = "get_resident_list",
//                        description = "List of all residents, including names, flat number, building, neighbourhood. Use for questions like 'who lives in flat X', resident names, resident count, or resident details.",
//                        parameters = new { type = "object", properties = new { } }
//                    }
//                },
//                new {
//                    type = "function",
//                    function = new {
//                        name = "get_tenant_list",
//                        description = "List of all tenants with names, flat, building, agreement dates. Use for questions about tenants.",
//                        parameters = new { type = "object", properties = new { } }
//                    }
//                },
//                new {
//                    type = "function",
//                    function = new {
//                        name = "get_landowner_list",
//                        description = "List of all landowners with names, flat, building. Use for questions about landowners.",
//                        parameters = new { type = "object", properties = new { } }
//                    }
//                },
//                new {
//                    type = "function",
//                    function = new {
//                        name = "get_guest_list",
//                        description = "List of all guests with validity dates and linked resident. Use for questions about guests.",
//                        parameters = new { type = "object", properties = new { } }
//                    }
//                },
//                new {
//                    type = "function",
//                    function = new {
//                        name = "get_visitor_list",
//                        description = "List of all visitors with visit purpose, dates, linked resident. Use for questions about visitors.",
//                        parameters = new { type = "object", properties = new { } }
//                    }
//                },
//                new {
//                    type = "function",
//                    function = new {
//                        name = "get_contractor_list",
//                        description = "List of all contractors with company, service type, validity dates. Use for questions about contractors.",
//                        parameters = new { type = "object", properties = new { } }
//                    }
//                },
//                new {
//                    type = "function",
//                    function = new {
//                        name = "get_employee_list",
//                        description = "List of all employees with designation, project, date of joining. Use for questions about employees or staff.",
//                        parameters = new { type = "object", properties = new { } }
//                    }
//                },
//                new {
//                    type = "function",
//                    function = new {
//                        name = "get_service_provider_list",
//                        description = "List of all service providers. Use for questions about service providers.",
//                        parameters = new { type = "object", properties = new { } }
//                    }
//                },
//                new {
//                    type = "function",
//                    function = new {
//                        name = "get_building_list",
//                        description = "List of all buildings in the township. Use for questions about buildings.",
//                        parameters = new { type = "object", properties = new { } }
//                    }
//                },
//                new {
//                    type = "function",
//                    function = new {
//                        name = "get_neighbourhood_list",
//                        description = "List of all neighbourhoods/projects (NRDs). Use for questions about neighbourhoods or projects.",
//                        parameters = new { type = "object", properties = new { } }
//                    }
//                }
//            };

//            var systemPrompt =
//                "You are an assistant for a residential township access-control system. " +
//                "You have tools for: vehicle in/out reports, unauthorized vehicles, card/turnstile logs, " +
//                "residents, tenants, landowners, guests, visitors, contractors, employees, buildings, and neighbourhoods. " +
//                "Whenever the question relates to any of these, call the matching tool instead of guessing any data. " +
//                "Today's date is " + DateTime.Now.ToString("dd-MMM-yyyy") + ". " +
//                "After receiving tool results, summarize them clearly and concisely, mentioning specific names/numbers " +
//                "from the data when relevant (e.g. list resident names if asked).";

//            var messages = new List<object>
//            {
//                new { role = "system", content = systemPrompt },
//                new { role = "user", content = request.Message }
//            };

//            try
//            {
//                const int maxToolRounds = 5;

//                for (int round = 0; round < maxToolRounds; round++)
//                {
//                    var responseJson = await CallOpenRouter(openRouter, model, messages, tools);
//                    using var doc = JsonDocument.Parse(responseJson);
//                    var choice = doc.RootElement.GetProperty("choices")[0].GetProperty("message");

//                    var hasToolCalls = choice.TryGetProperty("tool_calls", out var toolCalls)
//                        && toolCalls.ValueKind == JsonValueKind.Array
//                        && toolCalls.GetArrayLength() > 0;

//                    if (hasToolCalls)
//                    {
//                        // Append the assistant's tool-call message to the conversation, then execute each tool
//                        messages.Add(JsonSerializer.Deserialize<object>(choice.GetRawText())!);

//                        foreach (var toolCall in toolCalls.EnumerateArray())
//                        {
//                            var toolCallId = toolCall.GetProperty("id").GetString();
//                            var fnName = toolCall.GetProperty("function").GetProperty("name").GetString();
//                            var argsJson = toolCall.GetProperty("function").GetProperty("arguments").GetString();
//                            var args = string.IsNullOrWhiteSpace(argsJson)
//                                ? new Dictionary<string, string>()
//                                : JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(argsJson)!
//                                    .ToDictionary(kv => kv.Key, kv => kv.Value.ToString());

//                            var toolResult = await ExecuteTool(fnName!, args);

//                            messages.Add(new
//                            {
//                                role = "tool",
//                                tool_call_id = toolCallId,
//                                content = toolResult
//                            });
//                        }

//                        // Loop again: model may finalize now, or call more tools
//                        continue;
//                    }

//                    // No tool call this round — check for actual text content
//                    var hasContent = choice.TryGetProperty("content", out var contentEl)
//                        && contentEl.ValueKind == JsonValueKind.String
//                        && !string.IsNullOrWhiteSpace(contentEl.GetString());

//                    if (hasContent)
//                    {
//                        return Ok(new AskResponse { Reply = contentEl.GetString()! });
//                    }

//                    // Model returned neither tool calls nor usable content — stop here rather than loop forever
//                    break;
//                }

//                return Ok(new AskResponse { Reply = "I gathered the data but the assistant didn't return a final answer. Please try rephrasing your question." });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { error = "Unexpected error calling OpenRouter.", details = ex.Message });
//            }
//        }

//        private async Task<string> CallOpenRouter(HttpClient client, string model, List<object> messages, object tools)
//        {
//            var payload = new { model, messages, tools, tool_choice = "auto" };
//            var json = JsonSerializer.Serialize(payload);
//            var content = new StringContent(json, Encoding.UTF8, "application/json");

//            var response = await client.PostAsync("api/v1/chat/completions", content);
//            var body = await response.Content.ReadAsStringAsync();

//            if (!response.IsSuccessStatusCode)
//                throw new Exception($"OpenRouter request failed: {body}");

//            return body;
//        }

//        // ── Executes a tool call against your REAL existing report APIs ──────────
//        private async Task<string> ExecuteTool(string toolName, Dictionary<string, string> args)
//        {
//            var selfClient = _httpClientFactory.CreateClient();
//            var apiBase = _config["SelfApiBaseUrl"]; // e.g. "http://localhost:5555/api/"

//            string url;
//            switch (toolName)
//            {
//                case "get_vehicle_inout_register":
//                    url = $"{apiBase}Report/vehicleInoutRegister?fromDateTime={Uri.EscapeDataString(GetArg(args, "fromDateTime"))}" +
//                          $"&toDateTime={Uri.EscapeDataString(GetArg(args, "toDateTime"))}" +
//                          (args.ContainsKey("inOut") ? $"&inOut={args["inOut"]}" : "") +
//                          (args.ContainsKey("nrd") ? $"&nrd={Uri.EscapeDataString(args["nrd"])}" : "") +
//                          (args.ContainsKey("vehicleType") ? $"&vehicleType={Uri.EscapeDataString(args["vehicleType"])}" : "") +
//                          (args.ContainsKey("searchType") ? $"&searchType={args["searchType"]}" : "") +
//                          (args.ContainsKey("searchValue") ? $"&searchValue={Uri.EscapeDataString(args["searchValue"])}" : "") +
//                          "&pageNumber=1&pageSize=50";
//                    break;

//                case "get_unauthorized_vehicle_inout_register":
//                    url = $"{apiBase}Report/UnAuthorisedVehicleInoutRegister?fromDateTime={Uri.EscapeDataString(GetArg(args, "fromDateTime"))}" +
//                          $"&toDateTime={Uri.EscapeDataString(GetArg(args, "toDateTime"))}" +
//                          (args.ContainsKey("inOut") ? $"&inOut={args["inOut"]}" : "") +
//                          "&pageNumber=1&pageSize=50";
//                    break;

//                case "get_card_inout_register":
//                    // No confirmed filter params on this endpoint yet — fetches full log as-is.
//                    url = $"{apiBase}Report/InoutRegister";
//                    break;

//                case "get_resident_list":
//                    url = $"{apiBase}Resident/";
//                    break;

//                case "get_tenant_list":
//                    url = $"{apiBase}Tenent";
//                    break;

//                case "get_landowner_list":
//                    url = $"{apiBase}Landowner";
//                    break;

//                case "get_guest_list":
//                    url = $"{apiBase}Guest";
//                    break;

//                case "get_visitor_list":
//                    url = $"{apiBase}Visitor";
//                    break;

//                case "get_contractor_list":
//                    url = $"{apiBase}Contractor";
//                    break;

//                case "get_employee_list":
//                    url = $"{apiBase}Employee";
//                    break;

//                case "get_service_provider_list":
//                    url = $"{apiBase}Service_Provider/GetAllServiceProviders";
//                    break;

//                case "get_building_list":
//                    url = $"{apiBase}Building";
//                    break;

//                case "get_neighbourhood_list":
//                    url = $"{apiBase}NRD";
//                    break;

//                default:
//                    return "{\"error\":\"Unknown tool requested.\"}";
//            }

//            try
//            {
//                var res = await selfClient.GetAsync(url);
//                var body = await res.Content.ReadAsStringAsync();
//                if (!res.IsSuccessStatusCode)
//                    return $"{{\"error\":\"Report API returned {(int)res.StatusCode}\",\"details\":{JsonSerializer.Serialize(body)}}}";

//                // Cap payload size sent back to the LLM to avoid huge token usage
//                return body.Length > 8000 ? body.Substring(0, 8000) + "...[truncated]" : body;
//            }
//            catch (Exception ex)
//            {
//                return $"{{\"error\":\"Failed to call report API\",\"details\":{JsonSerializer.Serialize(ex.Message)}}}";
//            }
//        }

//        private static string GetArg(Dictionary<string, string> args, string key) =>
//            args.TryGetValue(key, out var v) ? v : DateTime.Now.ToString("dd-MMM-yyyy");
//    }
//}