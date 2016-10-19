using Totality.CommonClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Totality.Model;

namespace Totality.transmitterService
{
    [ServiceContract(CallbackContract = typeof(ICallback))]
    public interface ITransmitter
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

    public interface ICallback
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
