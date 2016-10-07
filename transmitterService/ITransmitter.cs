using CommonClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace transmitterService
{
    [ServiceContract(CallbackContract = typeof(ICallback))]
    public interface ITransmitter
    {
        [OperationContract]
        bool register(string myName);

        [OperationContract]
        bool addOrders(List<Order> orders);

        [OperationContract]
        bool shootDownNuke();

        [OperationContract]
        bool dipMsg(DipMsg msg);
    }

    public interface ICallback
    {
        [OperationContract(IsOneWay = true)]
        void InitializeNukeDialog();

        [OperationContract(IsOneWay = true)]
        void updateNukeDialog(List<NukeRocket> rockets);

        [OperationContract(IsOneWay = true)]
        void updateNews();

        [OperationContract(IsOneWay = true)]
        void sendDip(DipMsg msg);
    }
}
