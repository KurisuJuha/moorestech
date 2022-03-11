﻿using System;
using System.IO;
using System.Threading;
using Core.Item;
using Core.Update;
using Microsoft.Extensions.DependencyInjection;
using PlayerInventory;
using Server.Event;
using Server.PacketHandle;
using World;
using World.Event;

namespace Server
{
    public static class StartServer
    {
        private const int argsCount = 1;
        
        public static void Main(string[] args)
        {
            try
            {
#if DEBUG
                var configPath = DebugFolderPath;
#else
                var configPath = ReleasesFolderPath;
                if (args.Length == 0)
                {
                    Console.WriteLine("コマンドライン引数にコンフィグのパスが指定されていませんでした。デフォルトコンフィグパスを使用します。");
                } 
                else
                {
                    configPath = args[0];
                }
#endif
            
            
                var (packet, serviceProvider) = new PacketResponseCreatorDiContainerGenerators().Create(configPath);

                new Thread(() =>
                {
                    new PacketHandler().StartServer(packet);
                }).Start();
                new Thread(() =>
                {
                    while (true)
                    {
                        GameUpdate.Update();
                    }
                }).Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadKey();
            }
        }

        
        private static string DebugFolderPath
        {
            get
            {
                DirectoryInfo di = new DirectoryInfo(Environment.CurrentDirectory);
                DirectoryInfo diParent = di.Parent.Parent.Parent.Parent;
                return Path.Combine(diParent.FullName, "Server.Starter", "Config");
            }
        }
        private static string ReleasesFolderPath
        {
            get
            {
                DirectoryInfo di = new DirectoryInfo(Environment.CurrentDirectory);
                return Path.Combine(di.FullName, "Config");
            }
        }
    }
}