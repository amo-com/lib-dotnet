﻿using Amo.Lib;
using Amo.Lib.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Service.Impls
{
    public class Test : Interfaces.ITest
    {
        private readonly ILog _log;
        public Test(ILog log)
        {
            this._log = log;
        }

        public void Wait()
        {
            _log.Info(new LogEntity<string>() { Data = "Demo.Service.Test.Wait()" });
        }

        public void Work()
        {
            _log.Info(new LogEntity<string>() { Data = "Demo.Service.Test.Work()" });
        }

        public async Task<int> GetIdAsync()
        {
            return await Task.Run(() => { return 55; });
        }

        public async Task<List<string>> GetNamesAsync()
        {
            return await Task.Run(() => { return new List<string>() { "11", "22" }; });
        }
    }
}
