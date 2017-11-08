using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mars.Land;
using Mars;
using Long5;
using ZLKJ.DingWei.CommonLibrary;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Newtonsoft;


namespace WinApp.hw.xyp
{
    partial class XypAdapt : ModuleBase
    {
        public override string Entry4Response(string data)
        {
            Request req = (Request)JsonConvert.DeserializeObject(data, typeof(Request));
            Response res = new Response();

            switch(req.methodName)
            {
                case MethodName.SetBaseIp:  //set ip
                    {

                        res.result = true;
                        break;
                    }
                case MethodName.GetBaseIp:  // get basestation ip
                    {
                        if(baseStsAll.ContainsKey(req.model))
                        {
                            res.result = true;
                            res.content = baseStsAll[req.model].ip;
                        }
                        else
                        {
                            res.result = false;
                        }
                        break;
                    }
                case MethodName.MsgInterval:   //msg 发送间隔
                    {
                        res.result = true;
                        break;
                    }
                case MethodName.HBInterval:   //心跳间隔
                    {
                        int f;
                        if(int.TryParse(req.model, out f))
                        {
                            this.HeartBeatFrequency = f;
                            res.result = true;
                        }
                        else
                        {
                            res.result = false;
                        }

                        break;
                    }
                case MethodName.BaseReceiverRel:   //设置分站下属读卡器
                    {
                        res.result = true;
                        break;
                    }
                case MethodName.AddDownMsg:  //添加下行短消息
                    {
                        res.result = false;
                        break;
                    }
                case MethodName.DelDownMsg:  //删除下行短消息
                    {
                        res.result = false;
                        break;
                    }
                case MethodName.SetBaseTime:  //设置分站时间
                    {
                        res.result = true;
                        break;
                    }
                case MethodName.GetBaseTime:  //读取分站时间
                    {
                        res.result = true;
                        res.content = DateTime.Now.ToString();
                        break;
                    }

                default:
                    {
                        res.result = false;
                        res.content = "not support";
                        break;
                    }

            }

            return JsonConvert.SerializeObject(res);
        }
    }
}
