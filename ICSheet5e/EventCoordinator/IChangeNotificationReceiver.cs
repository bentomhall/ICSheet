﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSheet5e.EventCoordinator
{
    interface IChangeNotificationReceiver
    {
        void OnChangeNotificationReceived(object sender, EventArgs e);
    }
}
