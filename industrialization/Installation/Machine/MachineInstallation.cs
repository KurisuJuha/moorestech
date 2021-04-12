﻿using System;
using industrialization.Config;
using industrialization.Inventory;
using industrialization.Item;

namespace industrialization.Installation.Machine
{
    //TODO きちんと入ってきたアイテムを処理する機構を作る
    //TODO レシピを取得する
    public class MachineInstallation : InstallationBase,IMachine
    {
        private MachineInventory _machineInventory;

        public MachineInstallation(int installationId, Guid guid, IInstallationInventory connect) : base(installationId,guid)
        {
            GUID = guid;
            InstallationID = installationId;
            _machineInventory = new MachineInventory(installationId,connect);
        }

        public MachineState GetState()
        {
            throw new NotImplementedException();
        }

        public void SupplyPower(double power)
        {
            throw new NotImplementedException();
        }
    }
}