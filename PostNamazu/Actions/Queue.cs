using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PostNamazu.Attributes;
using PostNamazu.Common;

namespace PostNamazu.Actions
{
    internal class Queue : NamazuModule
    {
        private static readonly List<string> QueuePending = new(); //注册qid的队列

        [Command("queue")]
        [Command("DoQueueActions")]
        public async void DoQueue(string command)
        {
            try
            {
                CheckBeforeExecution(command);
                var actions = JsonConvert.DeserializeObject<QueueAction[]>(command);
                var qid = ""; //QueueID，为空时不会受到打断指令的影响
                foreach (var action in actions)
                {
                    await Task.Run(async () =>
                    {
                        await Task.Delay(action.d);

                        if (qid != "" && !QueuePending.Contains(qid)) //注册了qid，但是对应qid已经被移除(stop)的场合
                            return; //打断
                        switch (action.c.ToLower())
                        {
                            case "qid":
                                if (qid != "")
                                    QueuePending.Remove(qid); //如果之前已经有旧的qid，则先移除旧的qid
                                qid = action.p;
                                if (qid != "")
                                    QueuePending.Add(qid); //注册新的qid
                                break;
                            default:
                                try
                                {
                                    PostNamazu.DoAction(action.c, action.p);
                                }
                                catch (Exception e)
                                {
                                    Log(e.ToString());
                                }
                                break;
                        }
                    });
                    if (qid != "" && !QueuePending.Contains(qid))
                    {
                        Log(I18n.Translate("Queue/Broken", "队列 {0} 已被要求中断", qid));
                        break;
                    }
                }
                if (qid != "")
                    QueuePending.Remove(qid); //执行完毕，移除队列
            }
            catch (Exception e)
            {
                Log(e.ToString());
            }

        }
        [Command("stop")]
        [Command("break")]
        [Command("BreakQueueActions")]
        public void BreakQueue(string command)
        {
            Log(I18n.Translate("Queue/Break", "要求打断队列：{0}", command));
            switch (command.ToLower())
            {
                case "all":
                    QueuePending.Clear();
                    break;
                default:
                    QueuePending.RemoveAll(qid => Regex.IsMatch(qid, $"^{command}$"));
                    break;
            }
        }

        public class QueueAction
        {
            [JsonProperty]
            public string c { get; set; }
            [JsonProperty]
            public string p { get; set; }
            [JsonProperty]
            public int d { get; set; }
        }
    }
}
