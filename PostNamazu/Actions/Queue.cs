using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using PostNamazu.Attributes;

namespace PostNamazu.Actions
{
    internal class Queue : NamazuModule
    {
        [Command("queue")] [Command("DoQueueActions")]
        public async void DoQueue(string command)
        {
            if (!isReady) {
                PluginUI.Log("执行错误：接收到指令，但是没有对应的游戏进程");
                throw new Exception("没有对应的游戏进程");
            }

            if (command == "")
                throw new Exception("指令为空");
            try
            {
                var actions = JsonConvert.DeserializeObject<QueueAction[]>(command);
                foreach (var action in actions)
                {
                    await Task.Run(async () =>
                    {
                        await Task.Delay(action.d);
                        PostNamazu.DoAction(action.c, action.p);
                    });
                }
            }
            catch (Exception e)
            {
                Log(e.Message);
            }

        }

        public class QueueAction
        {
            [JsonProperty]
            public string? c { get; set; }
            [JsonProperty]
            public string? p { get; set; }
            [JsonProperty]
            public int d { get; set; }
        }
    }
}
