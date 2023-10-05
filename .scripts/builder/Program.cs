using System;
using System.IO;
using System.Diagnostics;
using System.CommandLine;
using System.Collections.Generic;

class Program
{

    [System.Serializable]
    public class BuildFailedException : System.Exception
    {
        public BuildFailedException() { }
        public BuildFailedException(string message) : base(message) { }
        public BuildFailedException(string message, System.Exception inner) : base(message, inner) { }
        protected BuildFailedException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    class ModInfo {
        public string gameName;
        public string modName;
        
        public ModInfo (string gameName, string modName) {
            this.modName = modName;
            this.gameName = gameName;
        }

        public string uniqueModName {
            get {
                return $"{this.gameName}_{this.modName}";
            }
        }
    }

    class ModBuilder {
        ModInfo modInfo;
        string srcDirPath;
        string destDirPath;

        public ModBuilder(ModInfo modInfo, string srcDirPath, string destDirPath) {
            this.modInfo = modInfo;
            this.srcDirPath = srcDirPath;
            this.destDirPath = destDirPath;
        }

        public void Build() {
            // 获取系统的临时目录路径
            string systemTempPath = Path.GetTempPath();

            // 创建一个唯一的子目录名
            string uniqueDirName = Guid.NewGuid().ToString();

            // 使用 Path.Combine() 创建新的临时目录路径
            string tempDirPath = Path.Combine(systemTempPath, uniqueDirName);

            try
            {
                // 创建临时目录
                Directory.CreateDirectory(tempDirPath);
                try
                {
                    Directory.CreateDirectory(destDirPath);
                }
                catch (System.Exception)
                {
                    
                    throw;
                }

                // 开始构建
                Process process = Process.Start("dotnet", $"build {this.srcDirPath} -o {tempDirPath} --configuration Release");

                if (process == null) {
                    throw new BuildFailedException($"启动 {this.modInfo.uniqueModName} 构建任务失败");
                }

                process.WaitForExit();
                if (process.ExitCode != 0) {
                    throw new BuildFailedException($"构建 {this.modInfo.uniqueModName} 失败");
                }

                // 移动文件
                File.Move(Path.Combine(tempDirPath, this.modInfo.modName + ".dll"), Path.Combine(destDirPath, this.modInfo.uniqueModName + ".dll"));
                Console.WriteLine($"构建 {Path.Combine(destDirPath, this.modInfo.uniqueModName)} 成功");
            }
            finally
            {
                // 在使用完临时目录后，你可以删除它
                if (Directory.Exists(tempDirPath))
                {
                    Directory.Delete(tempDirPath, true);
                }
            }
        }
    }

    static async Task<int> Main(string[] args) {
        var gameModRootDirOption = new Option<string>(
            name: "--src-root-path",
            description: "The path of GameMod Project"
        );

        var destDirOption = new Option<string>(
            name: "--dest-dir-path",
            description: "The destination directory path for mod builder.");

        var rootCommand = new RootCommand("Sample app for System.CommandLine");
        rootCommand.AddOption(gameModRootDirOption);
        rootCommand.AddOption(destDirOption);

        rootCommand.SetHandler((srcRootPath, destDirPath) => 
            {
                if (srcRootPath == null || destDirPath == null) {
                    Console.WriteLine("srcRootPath/destDirPath 不能为空");
                    return;
                }
                run(srcRootPath, destDirPath);
            },
            gameModRootDirOption, destDirOption);

        return await rootCommand.InvokeAsync(args);
    }

    static void run(string srcRootPath, string destDirPath) {
        
        // GameMod Path -> GameMod Name
        Dictionary<string, ModInfo> gameModDefinitions = new Dictionary<string, ModInfo>();

        // 最后的咒语
        gameModDefinitions["TheLastSpellMod/UnlimitedReroll"] = new ModInfo("TheLastSpell", "UnlimitedReroll");
        gameModDefinitions["TheLastSpellMod/MultipleGain"] = new ModInfo("TheLastSpell", "MultipleGain");

        // 觅长生
        gameModDefinitions["michangsheng/BattleGains"] = new ModInfo("michangsheng", "BattleGains");
        gameModDefinitions["michangsheng/WuDaoGains"] = new ModInfo("michangsheng", "WuDaoGains");
        gameModDefinitions["michangsheng/ShoterLearnTime"] = new ModInfo("michangsheng", "ShoterLearnTime");
        gameModDefinitions["michangsheng/BetterShoppingExperience"] = new ModInfo("michangsheng", "BetterShoppingExperience");
        gameModDefinitions["michangsheng/InstantlyForgeAndRefine"] = new ModInfo("michangsheng", "InstantlyForgeAndRefine");
        gameModDefinitions["michangsheng/CollectGains"] = new ModInfo("michangsheng", "CollectGains");
        gameModDefinitions["michangsheng/ForgetWuDaoSkill"] = new ModInfo("michangsheng", "ForgetWuDaoSkill");
        gameModDefinitions["michangsheng/FriendlyLianDan"] = new ModInfo("michangsheng", "FriendlyLianDan");
        gameModDefinitions["michangsheng/StrengthenDongfu"] = new ModInfo("michangsheng", "StrengthenDongfu");
        gameModDefinitions["michangsheng/BetterTooltips"] = new ModInfo("michangsheng", "BetterTooltips");

        // 小骨
        gameModDefinitions["Skul.Mod/MultipleGain"] = new ModInfo("Skul", "MultipleGain");
        gameModDefinitions["Skul.Mod/DropRarity"] = new ModInfo("Skul", "DropRarity");

        // 启动构建
        foreach (var kvp in gameModDefinitions)
        {
            
            var builder = new ModBuilder(kvp.Value, Path.Combine(srcRootPath, kvp.Key), destDirPath);
            builder.Build();
        }
    }
}
