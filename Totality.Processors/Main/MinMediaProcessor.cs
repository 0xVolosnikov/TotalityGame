﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Totality.CommonClasses;
using Totality.Model;
using Totality.Model.Interfaces;

namespace Totality.Processors.Main
{
    class MinMediaProcessor : AbstractProcessor, IMinisteryProcessor
    {
        public MinMediaProcessor(IDataLayer dataLayer) : base(dataLayer)
        {
        }

        public bool ProcessOrder(Order order)
        {
            throw new NotImplementedException();
        }
    }
}
