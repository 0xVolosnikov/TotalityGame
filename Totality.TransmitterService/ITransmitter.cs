using Totality.CommonClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Totality.Model;
using Totality.Model.Interfaces;

namespace Totality.TransmitterService
{
    [ServiceContract(CallbackContract = typeof(ICallbackService))]
    public interface ITransmitterService : ITransmitter
    {
        [OperationContract]
        bool Register(string myName);

        [OperationContract]
        bool AddOrders(List<Order> orders);

        [OperationContract]
        bool ShootDownNuke();

        [OperationContract]
        bool DipMsg(DipMsg msg);
    }

    public interface ICallbackService
    {
        [OperationContract(IsOneWay = true)]
        void InitializeNukeDialog();

        [OperationContract(IsOneWay = true)]
        void UpdateNukeDialog(List<NukeRocket> rockets);

        [OperationContract(IsOneWay = true)]
        void UpdateNews();

        [OperationContract(IsOneWay = true)]
        void SendDip(DipMsg msg);
    }
}
