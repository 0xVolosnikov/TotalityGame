using System;
using System.Collections.Generic;
using System.ServiceModel;
using Totality.Model;
using Totality.Model.Diplomatical;
using Totality.Model.Interfaces;

namespace Totality.TransmitterService
{
    [ServiceContract(CallbackContract = typeof(ICallbackService))]
    public interface ITransmitterService : ITransmitter
    {
        [OperationContract]
        bool Register(string myName);

        [OperationContract]
        bool AddOrders(List<Order> orders, string name);

        [OperationContract]
        bool ShootDownNuke(string defender, Guid rocketId);

        [OperationContract]
        bool DipMsg(DipMsg msg);

        [OperationContract]
        bool AskUpdate(string myName);

        [OperationContract]
        List<DipContract> AskContracts(string myName, string targetName);

        [OperationContract]
        Country GetCountryData(string name);

        [OperationContract]
        Dictionary<string, long> GetCurrencyStock();

        [OperationContract]
        Dictionary<string, long> GetCurrencyDemands();

        [OperationContract]
        Dictionary<string, double> GetSumIndPowers();
    }

    public interface ICallbackService
    {
        [OperationContract(IsOneWay = true)]
        void InitializeNukeDialog();

        [OperationContract(IsOneWay = true)]
        void UpdateNukeDialog(List<NukeRocket> rockets);

        [OperationContract(IsOneWay = true)]
        void SendNews(List<News> newsList);

        [OperationContract(IsOneWay = true)]
        void SendResults(List<OrderResult> results);

        [OperationContract(IsOneWay = true)]
        void UpdateClient(Country country);

        [OperationContract(IsOneWay = true)]
        void SendDip(DipMsg msg);

        [OperationContract(IsOneWay = true)]
        void SendContracts(List<DipContract> contracts);


    }
}