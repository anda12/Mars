using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mars.Land;
using Mars;
using Long5;
using ZLKJ.DingWei.CommonLibrary;
using ZLKJ.DingWei.CommonLibrary.Device;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WinApp.hw
{
    partial class Device : ModuleBase
    {

        private Dictionary<string, DevAdapt> DevAdaptAll;

        private int send_pub_event(MethodName methodname, string id)
        {

            EventModel evt = new EventModel();
            evt.methodname = methodname;
            evt.id = id;
            string json = JsonConvert.SerializeObject(evt);

            this.FaninClientSendData("HwData", json);

            return 0;
        }

        private bool UpdateBaseStation(BaseStationModel bsm)
        {
            BaseStationModel bm = null;
            string baseid = null;

            if (bsm == null)
            {
                return false;
            }


            if (bsm.id != null)
            {
                bm = DeviceDA.GetBaseStationByID(bsm.id);
                if (bm == null)
                {
                    Logging.logger.Error("get by basetion by id failed" + bsm.id);
                    return false;
                }
                baseid = bm.basecode;
            }
            else if (bsm.basecode != null)
            {
                baseid = bsm.basecode;
                bm = DeviceDA.GetBaseStationByCode(baseid);
                if (bm == null)
                {
                    Logging.logger.Error("get by basetion by basecode failed" + baseid);
                    return false;
                }
            }

            if (string.Compare(bsm.state, bm.state) != 0)
            {
                devstsl[baseid].state = bsm.state;

                if (string.Compare(bsm.state, DevGlobal.breakdown) == 0)
                {
                    //how to deal the receiver ??
                }
                else if (string.Compare(bsm.state, DevGlobal.nodevice) == 0)  //nodev
                {

                }
                else if (string.Compare(bsm.state, DevGlobal.normal) == 0)
                {
                    //??
                }
            }

            bool rlt = DeviceDA.EditBaseStation(bsm);

            return rlt;
        }

        private int fromRecvcodeGetPort(string recvcode, out string baseid)
        {
            if (recvcode == null)
            {
                baseid = null;
                return -1;
            }

            string[] rec = recvcode.Split('_');
            if (rec.Length <= 1)
            {
                baseid = null;
                return -1;
            }

            int index;
            try
            {
                index = int.Parse(rec[1]);
                if (index >= 8)
                {
                    Logging.logger.Error("idex wrong");
                    baseid = null;
                    return -1;
                }
            }
            catch (Exception err)
            {
                Logging.logger.Error("wrong with " + recvcode);
                Logging.logger.Error(err.Message);
                baseid = null;
                return -1;
            }

            baseid = rec[0];
            return index;
        }
        private bool UpdateReciver(ReceiverModel rm)
        {
            if (rm == null)
            {
                return false;
            }
            string recvcode = rm.receivercode;
            ReceiverModel rmd = DeviceDA.GetReceiverByCode(recvcode);
            if (rmd == null)
            {
                rmd = DeviceDA.GetReceiverByID(rm.id);
                if (rmd == null)
                {
                    return false;
                }
            }

            if (string.Compare(rm.id, rmd.id) != 0)
            {
                Logging.logger.Error("id different " + rm.id + " " + rmd.id);
                return false;
            }
            bool rlt = false;
            if (string.Compare(rm.state, rmd.state) != 0)
            {
                //update state
                string baseid;
                int i = fromRecvcodeGetPort(rm.receivercode, out baseid);
                if (i == -1)
                {
                    Logging.logger.Error("receiver code wrong");
                }
                else
                {
                    if (devstsl.ContainsKey(baseid))
                    {
                        DevStsModel dsm = devstsl[baseid];
                        dsm.recvsts[i].status = rm.state;
                        rlt = DeviceDA.EditReceiver(rm);
                    }
                    else
                    {
                        Logging.logger.Error("devstsl wrong with baseid " + rm.receivercode + "   " + baseid);
                    }
                }
            }
            else
            {
                rlt = DeviceDA.EditReceiver(rm);
            }

            return rlt;
        }

        public static int RemoteRegistModuleWarn(ModuleBase current, string name, DevAdapt wm)
        {
            if (current == null || wm == null || name == null)
            {
                Logging.logger.Error(MsgConstStr.ParmNull);
                return -1;
            }

            if (wm.ModName == null || wm.CmdSerName == null)
            {
                Logging.logger.Error(MsgConstStr.ParmNull);
                return -1;
            }


            StrMessage msg = new StrMessage();
            msg.Header = MsgCmd.Regist;
            msg.Content = JsonConvert.SerializeObject(wm);
            string sd = JsonConvert.SerializeObject(msg);
            string od = string.Empty;
            if (current.RequestGetData(name, sd, out od) == 0)
            {
                StrMessage rlt = (StrMessage)JsonConvert.DeserializeObject(od, typeof(StrMessage));
                if (rlt.Header != MsgCmd.OK)
                {
                    Logging.logger.Error(msg.Content);
                    return -2;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                Logging.logger.Error(MsgConstStr.RegistProcessFail);
                return -1;
            }
        }

        private bool CheckCmdServerExist(string ser)
        {
            return true;
        }

        private int RegDevAdaptModLocal(DevAdapt da)
        {
            if (DevAdaptAll == null)
            {
                DevAdaptAll = new Dictionary<string, DevAdapt>();
            }

            if(DevAdaptAll.ContainsKey(da.ModName))
            {
                Logging.logger.Warn("This module already exist");
                return 0;
            }

            if(da.CmdSerName == "")
            {
                Logging.logger.Warn("The cmd server name is wrong");
                return -1;
            }

            if(!CheckCmdServerExist(da.CmdSerName))
            {
                Logging.logger.Error("The cmd server name do n't exist");
                return -1;
            }

            DevAdaptAll.Add(da.ModName, da);
            return 0;
        }

        public override string Entry4Response(string data)
        {


            JObject obj = (JObject)JsonConvert.DeserializeObject(data);

            if (obj.Property("methodName") != null)
            {
                Response resp = new Response();
                string json = string.Empty;
                try
                {
                    BaseStationModel baseStationModel = null;
                    List<BaseStationModel> baseStationModelList = null;
                    ReceiverModel receiverModel = null;
                    List<ReceiverModel> receiverModelList = null;
                    BaseStationReceiverModel baseStationReceiverModel = null;
                    List<BaseStationReceiverModel> baseStationReceiverModelList = null;
                    CardModel cardModel = null;
                    List<CardModel> cardModelList = null;


                    Request req = (Request)JsonConvert.DeserializeObject(data, typeof(Request));
                    switch (req.methodName)
                    {
                        #region 基站
                        case MethodName.AddBaseStation:
                            baseStationModel = (BaseStationModel)JsonConvert.DeserializeObject(req.model, typeof(BaseStationModel));
                            resp.result = DeviceDA.AddBaseStation(baseStationModel);
                            resp.content = string.Empty;
                            resp.recordcount = 0;
                            json = JsonConvert.SerializeObject(resp);
                            ////serverSocket.SendFrame(json);
                            send_pub_event(MethodName.AddBaseStation, baseStationModel.basecode);
                            break;
                        case MethodName.EditBaseStation:
                            baseStationModel = (BaseStationModel)JsonConvert.DeserializeObject(req.model, typeof(BaseStationModel));
                            resp.result = UpdateBaseStation(baseStationModel);
                            resp.content = string.Empty;
                            resp.recordcount = 0;
                            json = JsonConvert.SerializeObject(resp);
                            //serverSocket.SendFrame(json);
                            send_pub_event(MethodName.EditBaseStation, baseStationModel.basecode);
                            break;
                        case MethodName.DeleteBaseStation:
                            baseStationModel = (BaseStationModel)JsonConvert.DeserializeObject(req.model, typeof(BaseStationModel));
                            resp.result = DeviceDA.DeleteBaseStation(baseStationModel.id);
                            resp.content = string.Empty;
                            resp.recordcount = 0;
                            json = JsonConvert.SerializeObject(resp);
                            //serverSocket.SendFrame(json);
                            send_pub_event(MethodName.DeleteBaseStation, baseStationModel.basecode);
                            break;
                        case MethodName.GetAllBaseStation:
                            baseStationModelList = DeviceDA.GetAllBaseStation();
                            if (baseStationModelList.Count == 0)
                            {
                                resp.content = string.Empty;
                                resp.result = false;
                            }
                            else
                            {
                                resp.content = JsonConvert.SerializeObject(baseStationModelList);
                                resp.result = true;
                            }
                            resp.recordcount = 0;
                            json = JsonConvert.SerializeObject(resp);
                            //serverSocket.SendFrame(json);
                            break;
                        case MethodName.GetAllBaseStationCount:
                            resp.result = true;
                            resp.content = string.Empty;
                            resp.recordcount = DeviceDA.GetAllBaseStationCount();
                            json = JsonConvert.SerializeObject(resp);
                            //serverSocket.SendFrame(json);
                            break;
                        case MethodName.GetPagingBaseStation:
                            baseStationModelList = DeviceDA.GetPagingBaseStation(req.start, req.size);
                            if (baseStationModelList.Count == 0)
                            {
                                resp.result = false;
                                resp.content = string.Empty;
                                resp.recordcount = 0;
                            }
                            else
                            {
                                resp.content = JsonConvert.SerializeObject(baseStationModelList);
                                resp.result = true;
                                resp.recordcount = DeviceDA.GetAllBaseStationCount();
                            }
                            json = JsonConvert.SerializeObject(resp);
                            //serverSocket.SendFrame(json);
                            break;
                        case MethodName.GetBaseStationByCode:
                            baseStationModel = (BaseStationModel)JsonConvert.DeserializeObject(req.model, typeof(BaseStationModel));
                            baseStationModel = DeviceDA.GetBaseStationByCode(baseStationModel.basecode);
                            if (baseStationModel == null)
                            {
                                resp.result = false;
                                resp.content = string.Empty;
                            }
                            else
                            {
                                resp.content = JsonConvert.SerializeObject(baseStationModel);
                                resp.result = true;
                            }
                            resp.recordcount = 0;
                            json = JsonConvert.SerializeObject(resp);
                            //serverSocket.SendFrame(json);
                            break;
                        case MethodName.GetBaseStationByID:
                            baseStationModel = (BaseStationModel)JsonConvert.DeserializeObject(req.model, typeof(BaseStationModel));
                            baseStationModel = DeviceDA.GetBaseStationByID(baseStationModel.id);
                            if (baseStationModel == null)
                            {
                                resp.result = false;
                                resp.content = string.Empty;
                            }
                            else
                            {
                                resp.content = JsonConvert.SerializeObject(baseStationModel);
                                resp.result = true;
                            }
                            resp.recordcount = 0;
                            json = JsonConvert.SerializeObject(resp);
                            //serverSocket.SendFrame(json);
                            break;
                        case MethodName.GetBaseStationWithoutReceiver:
                            baseStationModelList = DeviceDA.GetBaseStationWithoutReceiver();
                            if (baseStationModelList.Count == 0)
                            {
                                resp.result = false;
                                resp.content = string.Empty;
                            }
                            else
                            {
                                resp.content = JsonConvert.SerializeObject(baseStationModelList);
                                resp.result = true;
                            }
                            resp.recordcount = 0;
                            json = JsonConvert.SerializeObject(resp);
                            //serverSocket.SendFrame(json);
                            break;
                        case MethodName.SetBaseStationStateByCode:
                            baseStationModel = (BaseStationModel)JsonConvert.DeserializeObject(req.model, typeof(BaseStationModel));
                            resp.result = UpdateBaseStation(baseStationModel);
                            resp.content = string.Empty;
                            resp.recordcount = 0;
                            json = JsonConvert.SerializeObject(resp);
                            //serverSocket.SendFrame(json);
                            send_pub_event(MethodName.SetBaseStationStateByCode, baseStationModel.basecode);
                            break;

                        #endregion

                        #region 接收器
                        case MethodName.AddReceiver:
                            receiverModel = (ReceiverModel)JsonConvert.DeserializeObject(req.model, typeof(ReceiverModel));
                            resp.result = DeviceDA.AddReceiver(receiverModel);
                            resp.content = string.Empty;
                            resp.recordcount = 0;
                            json = JsonConvert.SerializeObject(resp);
                            //serverSocket.SendFrame(json);
                            send_pub_event(MethodName.AddReceiver, receiverModel.receivercode);
                            break;
                        case MethodName.EditReceiver:
                            receiverModel = (ReceiverModel)JsonConvert.DeserializeObject(req.model, typeof(ReceiverModel));
                            resp.result = UpdateReciver(receiverModel);
                            resp.content = string.Empty;
                            resp.recordcount = 0;
                            json = JsonConvert.SerializeObject(resp);
                            //serverSocket.SendFrame(json);
                            send_pub_event(MethodName.EditReceiver, receiverModel.receivercode);
                            break;
                        case MethodName.DeleteReceiver:
                            receiverModel = (ReceiverModel)JsonConvert.DeserializeObject(req.model, typeof(ReceiverModel));
                            resp.result = DeviceDA.DeleteReceiver(receiverModel.id);
                            resp.content = string.Empty;
                            resp.recordcount = 0;
                            json = JsonConvert.SerializeObject(resp);
                            //serverSocket.SendFrame(json);
                            send_pub_event(MethodName.DeleteReceiver, receiverModel.receivercode);
                            break;
                        case MethodName.GetAllReceiverCount:
                            resp.result = true;
                            resp.content = string.Empty;
                            resp.recordcount = DeviceDA.GetAllReceiverCount();
                            json = JsonConvert.SerializeObject(resp);
                            //serverSocket.SendFrame(json);
                            break;
                        case MethodName.GetPagingReceiver:
                            receiverModelList = DeviceDA.GetPagingReceiver(req.start, req.size);
                            if (receiverModelList.Count == 0)
                            {
                                resp.result = false;
                                resp.content = string.Empty;
                                resp.recordcount = 0;
                            }
                            else
                            {
                                resp.result = true;
                                resp.content = JsonConvert.SerializeObject(receiverModelList);
                                resp.recordcount = DeviceDA.GetAllReceiverCount();
                            }
                            json = JsonConvert.SerializeObject(resp);
                            //serverSocket.SendFrame(json);
                            break;
                        case MethodName.GetAllReceiver:
                            receiverModelList = DeviceDA.GetAllReceiver();
                            if (receiverModelList.Count == 0)
                            {
                                resp.result = false;
                                resp.content = string.Empty;
                                resp.recordcount = 0;
                            }
                            else
                            {
                                resp.result = true;
                                resp.content = JsonConvert.SerializeObject(receiverModelList);
                                resp.recordcount = 0;
                            }
                            json = JsonConvert.SerializeObject(resp);
                            //serverSocket.SendFrame(json);
                            break;
                        case MethodName.GetReceiverByCode:
                            receiverModel = (ReceiverModel)JsonConvert.DeserializeObject(req.model, typeof(ReceiverModel));
                            receiverModel = DeviceDA.GetReceiverByCode(receiverModel.receivercode);
                            if (receiverModel == null)
                            {
                                resp.result = false;
                                resp.content = string.Empty;
                            }
                            else
                            {
                                resp.result = true;
                                resp.content = JsonConvert.SerializeObject(receiverModel);
                            }
                            resp.recordcount = 0;
                            json = JsonConvert.SerializeObject(resp);
                            //serverSocket.SendFrame(json);
                            break;
                        case MethodName.GetReceiverByID:
                            receiverModel = (ReceiverModel)JsonConvert.DeserializeObject(req.model, typeof(ReceiverModel));
                            receiverModel = DeviceDA.GetReceiverByID(receiverModel.id);
                            if (receiverModel == null)
                            {
                                resp.result = false;
                                resp.content = string.Empty;
                            }
                            else
                            {
                                resp.result = true;
                                resp.content = JsonConvert.SerializeObject(receiverModel);
                            }
                            resp.recordcount = 0;
                            json = JsonConvert.SerializeObject(resp);
                            //serverSocket.SendFrame(json);
                            break;
                        case MethodName.GetReceiverWithoutBaseStation:
                            receiverModelList = DeviceDA.GetReceiverWithoutBaseStation();
                            if (receiverModelList.Count == 0)
                            {
                                resp.result = false;
                                resp.content = string.Empty;
                                resp.recordcount = 0;
                            }
                            else
                            {
                                resp.result = true;
                                resp.content = JsonConvert.SerializeObject(receiverModelList);
                                resp.recordcount = 0;
                            }
                            json = JsonConvert.SerializeObject(resp);
                            //serverSocket.SendFrame(json);
                            break;
                        //case MethodName.GetReceiverLocation:
                        //    receiverModel = (ReceiverModel)JsonConvert.DeserializeObject(req.model, typeof(ReceiverModel));
                        //    ReceiverLocationModel receiverLocationModel = DeviceDA.GetReceiverLocation(receiverModel.receivercode);
                        //    if (receiverLocationModel == null)
                        //    {
                        //        resp.result = false;
                        //        resp.content = string.Empty;
                        //        resp.recordcount = 0;
                        //    }
                        //    else
                        //    {
                        //        resp.result = true;
                        //        resp.content = JsonConvert.SerializeObject(receiverLocationModel);
                        //        resp.recordcount = 0;
                        //    }
                        //    json = JsonConvert.SerializeObject(resp);
                        //    //serverSocket.SendFrame(json);
                        //    break;
                        case MethodName.SetReceiverStateByCode:
                            receiverModel = (ReceiverModel)JsonConvert.DeserializeObject(req.model, typeof(ReceiverModel));
                            resp.result = UpdateReciver(receiverModel);
                            resp.content = string.Empty;
                            resp.recordcount = 0;
                            json = JsonConvert.SerializeObject(resp);
                            //serverSocket.SendFrame(json);
                            send_pub_event(MethodName.SetReceiverStateByCode, receiverModel.receivercode);
                            break;
                        #endregion

                        #region 基站和接收器关系
                        case MethodName.AddBaseStationReceiver:
                            baseStationReceiverModel = (BaseStationReceiverModel)JsonConvert.DeserializeObject(req.model, typeof(BaseStationReceiverModel));
                            resp.result = DeviceDA.AddBaseStationReceiver(baseStationReceiverModel);
                            resp.content = string.Empty;
                            resp.recordcount = 0;
                            json = JsonConvert.SerializeObject(resp);
                            //serverSocket.SendFrame(json);
                            break;
                        case MethodName.EditBaseStationReceiverById:
                            baseStationReceiverModel = (BaseStationReceiverModel)JsonConvert.DeserializeObject(req.model, typeof(BaseStationReceiverModel));
                            resp.result = DeviceDA.EditBaseStationReceiverById(baseStationReceiverModel);
                            resp.content = string.Empty;
                            resp.recordcount = 0;
                            json = JsonConvert.SerializeObject(resp);
                            //serverSocket.SendFrame(json);
                            break;
                        case MethodName.EditBaseStationReceiverRecvcode:
                            baseStationReceiverModel = (BaseStationReceiverModel)JsonConvert.DeserializeObject(req.model, typeof(BaseStationReceiverModel));
                            resp.result = DeviceDA.EditBaseStationReceiverByRecvcode(baseStationReceiverModel);
                            resp.content = string.Empty;
                            resp.recordcount = 0;
                            json = JsonConvert.SerializeObject(resp);
                            //serverSocket.SendFrame(json);
                            break;
                        case MethodName.EditBaseStationReceiverBasecode:
                            baseStationReceiverModel = (BaseStationReceiverModel)JsonConvert.DeserializeObject(req.model, typeof(BaseStationReceiverModel));
                            resp.result = DeviceDA.EditBaseStationReceiverByBasecode(baseStationReceiverModel);
                            resp.content = string.Empty;
                            resp.recordcount = 0;
                            json = JsonConvert.SerializeObject(resp);
                            //serverSocket.SendFrame(json);
                            break;
                        case MethodName.DeleteBaseStationReceiverByBothCode:
                            baseStationReceiverModel = (BaseStationReceiverModel)JsonConvert.DeserializeObject(req.model, typeof(BaseStationReceiverModel));
                            resp.result = DeviceDA.DeleteBaseStationReceiverByBothCode(baseStationReceiverModel.basecode, baseStationReceiverModel.receivercode);
                            resp.content = string.Empty;
                            resp.recordcount = 0;
                            json = JsonConvert.SerializeObject(resp);
                            //serverSocket.SendFrame(json);
                            break;
                        case MethodName.DeleteBaseStationReceiverByBaseCode:
                            baseStationReceiverModel = (BaseStationReceiverModel)JsonConvert.DeserializeObject(req.model, typeof(BaseStationReceiverModel));
                            resp.result = DeviceDA.DeleteBaseStationReceiverByBaseCode(baseStationReceiverModel.basecode);
                            resp.content = string.Empty;
                            resp.recordcount = 0;
                            json = JsonConvert.SerializeObject(resp);
                            //serverSocket.SendFrame(json);
                            break;
                        case MethodName.DeleteBaseStationReceiverByReceiverCode:
                            baseStationReceiverModel = (BaseStationReceiverModel)JsonConvert.DeserializeObject(req.model, typeof(BaseStationReceiverModel));
                            resp.result = DeviceDA.DeleteBaseStationReceiverByReceiverCode(baseStationReceiverModel.receivercode);
                            resp.content = string.Empty;
                            resp.recordcount = 0;
                            json = JsonConvert.SerializeObject(resp);
                            //serverSocket.SendFrame(json);
                            break;
                        case MethodName.GetAllBaseStationReceiver:
                            baseStationReceiverModelList = DeviceDA.GetAllBaseStationReceiver();
                            if (baseStationReceiverModelList.Count == 0)
                            {
                                resp.result = false;
                                resp.content = string.Empty;
                            }
                            else
                            {
                                resp.result = true;
                                resp.content = JsonConvert.SerializeObject(baseStationReceiverModelList);
                            }
                            resp.recordcount = 0;
                            json = JsonConvert.SerializeObject(resp);
                            //serverSocket.SendFrame(json);
                            break;
                        case MethodName.GetBaseStationReceiverByBaseCode:
                            baseStationReceiverModel = (BaseStationReceiverModel)JsonConvert.DeserializeObject(req.model, typeof(BaseStationReceiverModel));
                            baseStationReceiverModelList = DeviceDA.GetBaseStationReceiverByBaseCode(baseStationReceiverModel.basecode);
                            if (baseStationReceiverModelList.Count == 0)
                            {
                                resp.result = false;
                                resp.content = string.Empty;
                            }
                            else
                            {
                                resp.result = true;
                                resp.content = JsonConvert.SerializeObject(baseStationReceiverModelList);
                            }
                            resp.recordcount = 0;
                            json = JsonConvert.SerializeObject(resp);
                            //serverSocket.SendFrame(json);
                            break;
                        case MethodName.GetBaseStationReceiverByReceiverCode:
                            baseStationReceiverModel = (BaseStationReceiverModel)JsonConvert.DeserializeObject(req.model, typeof(BaseStationReceiverModel));
                            baseStationReceiverModel = DeviceDA.GetBaseStationReceiverByReceiverCode(baseStationReceiverModel.receivercode);
                            if (baseStationReceiverModel == null)
                            {
                                resp.result = false;
                                resp.content = string.Empty;
                            }
                            else
                            {
                                resp.result = true;
                                resp.content = JsonConvert.SerializeObject(baseStationReceiverModel);
                            }
                            resp.recordcount = 0;
                            json = JsonConvert.SerializeObject(resp);
                            //serverSocket.SendFrame(json);
                            break;
                        #endregion

                        #region 卡
                        case MethodName.AddCard:
                            cardModel = (CardModel)JsonConvert.DeserializeObject(req.model, typeof(CardModel));
                            resp.result = DeviceDA.AddCard(cardModel);
                            resp.content = string.Empty;
                            resp.recordcount = 0;
                            json = JsonConvert.SerializeObject(resp);
                            //serverSocket.SendFrame(json);
                            send_pub_event(MethodName.AddCard, cardModel.cardcode);
                            break;
                        case MethodName.EditCard:
                            cardModel = (CardModel)JsonConvert.DeserializeObject(req.model, typeof(CardModel));
                            resp.result = DeviceDA.EditCard(cardModel);
                            resp.content = string.Empty;
                            resp.recordcount = 0;
                            json = JsonConvert.SerializeObject(resp);
                            //serverSocket.SendFrame(json);
                            send_pub_event(MethodName.EditCard, cardModel.cardcode);
                            break;
                        case MethodName.DeleteCard:
                            cardModel = (CardModel)JsonConvert.DeserializeObject(req.model, typeof(CardModel));
                            resp.result = DeviceDA.DeleteCard(cardModel.id);
                            resp.content = string.Empty;
                            resp.recordcount = 0;
                            json = JsonConvert.SerializeObject(resp);
                            //serverSocket.SendFrame(json);
                            send_pub_event(MethodName.DeleteCard, cardModel.cardcode);
                            break;
                        case MethodName.GetAllCard:
                            cardModelList = DeviceDA.GetAllCard();
                            if (cardModelList.Count == 0)
                            {
                                resp.result = false;
                                resp.content = string.Empty;
                            }
                            else
                            {
                                resp.result = true;
                                resp.content = JsonConvert.SerializeObject(cardModelList);
                            }
                            resp.recordcount = 0;
                            json = JsonConvert.SerializeObject(resp);
                            //serverSocket.SendFrame(json);
                            break;
                        case MethodName.GetAllCardCount:
                            resp.result = true;
                            resp.content = string.Empty;
                            resp.recordcount = DeviceDA.GetAllCardCount();
                            json = JsonConvert.SerializeObject(resp);
                            //serverSocket.SendFrame(json);
                            break;
                        case MethodName.GetPagingCard:
                            cardModelList = DeviceDA.GetPagingCard(req.start, req.size);
                            if (cardModelList.Count == 0)
                            {
                                resp.result = false;
                                resp.content = string.Empty;
                                resp.recordcount = 0;
                            }
                            else
                            {
                                resp.result = true;
                                resp.content = JsonConvert.SerializeObject(cardModelList);
                                resp.recordcount = DeviceDA.GetAllCardCount();
                            }
                            json = JsonConvert.SerializeObject(resp);
                            //serverSocket.SendFrame(json);
                            break;
                        case MethodName.GetCardByID:
                            cardModel = (CardModel)JsonConvert.DeserializeObject(req.model, typeof(CardModel));
                            cardModel = DeviceDA.GetCardByID(cardModel.id);
                            if (cardModel == null)
                            {
                                resp.result = false;
                                resp.content = string.Empty;
                            }
                            else
                            {
                                resp.result = true;
                                resp.content = JsonConvert.SerializeObject(cardModel);
                            }
                            resp.recordcount = 0;
                            json = JsonConvert.SerializeObject(resp);
                            //serverSocket.SendFrame(json);
                            break;
                        case MethodName.GetCardByCode:
                            cardModel = (CardModel)JsonConvert.DeserializeObject(req.model, typeof(CardModel));
                            cardModel = DeviceDA.GetCardByCode(cardModel.cardcode);
                            if (cardModel == null)
                            {
                                resp.result = false;
                                resp.content = string.Empty;
                            }
                            else
                            {
                                resp.result = true;
                                resp.content = JsonConvert.SerializeObject(cardModel);
                            }
                            resp.recordcount = 0;
                            json = JsonConvert.SerializeObject(resp);
                            //serverSocket.SendFrame(json);
                            break;
                        case MethodName.GetLowPowerCardInfo:
                            List<CardPowerModel> cpml = DeviceDA.GetAllCardPower();

                            if (cpml.Count == 0)
                            {
                                resp.result = false;
                                resp.content = string.Empty;
                            }
                            else
                            {
                                resp.result = true;
                                resp.content = JsonConvert.SerializeObject(cpml);
                            }
                            resp.recordcount = 0;
                            json = JsonConvert.SerializeObject(resp);
                            //serverSocket.SendFrame(json);
                            break;
                        case MethodName.GetLowPowerCardById:
                            CardPowerModel cpm = (CardPowerModel)JsonConvert.DeserializeObject(req.model, typeof(CardPowerModel));
                            cpm = DeviceDA.GetCardPowerByCardCode(cpm.cardcode);
                            if (cardModel == null)
                            {
                                resp.result = false;
                                resp.content = string.Empty;
                            }
                            else
                            {
                                resp.result = true;
                                resp.content = JsonConvert.SerializeObject(cpm);
                            }
                            resp.recordcount = 0;
                            json = JsonConvert.SerializeObject(resp);
                            //serverSocket.SendFrame(json);
                            break;
                        #endregion


                        #region 退出线程
                        //case MethodName.Bye:
                        //    resp.result = true;
                        //    resp.content = string.Empty;
                        //    resp.recordcount = 0;
                        //    json = JsonConvert.SerializeObject(resp);
                        //    //serverSocket.SendFrame(json);
                        //    running = false;
                        //    break;
                        #endregion
                    }
                }
                catch (Exception err)
                {
                    resp.result = false;
                    resp.content = err.Message;
                    resp.recordcount = 0;
                    json = JsonConvert.SerializeObject(resp);
                    //serverSocket.SendFrame(json);
                    Logging.logger.Error(err.Message);
                    throw (err);
                }
                return json;
            }
            else if (obj.Property("Header") != null)
            {
                string tag = obj["Header"].ToString();
                if (tag == MsgCmd.Regist.ToString())
                {
                    DevAdapt da = (DevAdapt)JsonConvert.DeserializeObject(obj["Content"].ToString(), typeof(DevAdapt));

                    int rlt = RegDevAdaptModLocal(da);
                    if (rlt != 0)
                    {
                        Logging.logger.Error("Regist device adapt loacal failed ");
                        StrMessage rdata = new StrMessage();
                        rdata.Header = MsgCmd.Fail;
                        rdata.Content = MsgConstStr.WarnInfoExist;
                        return JsonConvert.SerializeObject(rdata);
                    }
                    else
                    {
                        StrMessage rdata = new StrMessage();
                        rdata.Header = MsgCmd.OK;
                        return JsonConvert.SerializeObject(rdata);
                    }
                }
                else if (tag == MsgCmd.HWCMD.ToString())
                {

                }
            }
            else
            {
                Logging.logger.Error(MsgConstStr.CMDNotFount);
            }

            return "";

        }
    }
}
