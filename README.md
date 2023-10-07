# 游戏Mod

## [小骨：英雄杀手(Skul: The Hero Slayer)](https://store.steampowered.com/app/1147560/Skul_The_Hero_Slayer/)

本项目所有 Mod 均基于 BepInEx 框架开发。由于 **Skul: The Hero Slayer** 精简了 `Unity` 的部分库, 需要手动解压缺失的 `Unity` 才能正常启动 `BepInEx`。

### 小骨：英雄杀手 Mod 安装方式
假设 `$(SkulDirectory)` 是 Skul.exe 所在的文件夹。
对于 Windows，通常是: `C:\Program Files (x86)\Steam\steamapps\common\Skul` 或者其他类似的路径。

* 下载并解压 [Unstripped Unity files 2020.3.34](https://unity.bepinex.dev/libraries/2020.3.34.zip) 到 `$(SkulDirectory)\Skul_Data\Managed\`

* 下载并解压 [Unstripped corelibs 2020.3.34](https://unity.bepinex.dev/corlibs/2020.3.34.zip) 到 `$(SkulDirectory)\Skul_Data\Managed\`

* 下载并解压 [BepInEx 5.4.21](https://github.com/BepInEx/BepInEx/releases/tag/v5.4.21) 到 `$(SkulDirectory)`
  * Windows 系统: [x64](https://github.com/BepInEx/BepInEx/releases/download/v5.4.21/BepInEx_x64_5.4.21.0.zip)
  * Linux 系统: [unix](https://github.com/BepInEx/BepInEx/releases/download/v5.4.21/BepInEx_unix_5.4.21.0.zip)

* 下载 [你需要的Mod](https://github.com/shabbywu/GameMod/releases) 到 `$(SkulDirectory)/BepInEx/plugins`
    * 如果 `$(SkulDirectory)/BepInEx/plugins` 目录不存在, 手动创建即可

## Mod 介绍
### MultipleGain(多倍收益)
使用该 Mod 后, 可根据配置调整收益倍率(支持 Gold、DarkQuartz、Bone、HeartQuartz)

## FAQ
1. 如何开发新的 Mod

1.1. 从模板创建 Mod 项目
```bash
dotnet new bepinex5plugin -T netstandard2.0 -U 2020.3.34 -n {Your-Mod-Name}
```

1.2. 添加 lib 到 Mod 项目
```xml
# 编辑 {Your-Mod-Name}/{Your-Mod-Name}.csproj 在 Project 中追加以下内容
  <ItemGroup>
    <Reference Include="Assembly-CSharp">
      <HintPath>../lib/Assembly-CSharp.dll</HintPath>
    </Reference>
  </ItemGroup>
```


## [最后的咒语](https://store.steampowered.com/app/1105670/)
本项目所有 Mod 均基于 BepInEx 框架开发. [如何安装 BepInEx 框架](https://docs.bepinex.dev/master/articles/user_guide/installation/index.html)

## 最后的咒语 Mod 安装方式
假设 `$(TheLastSpellDirectory)` 是 `The Last Spell.exe` 所在的文件夹。
对于 Windows，通常是: `C:\Program Files (x86)\Steam\steamapps\common\The Last Spell` 或者其他类似的路径。


* 下载并解压 [BepInEx 5.4.21](https://github.com/BepInEx/BepInEx/releases/tag/v5.4.21) 到 `$(TheLastSpellDirectory)`
  * Windows 系统: [x64](https://github.com/BepInEx/BepInEx/releases/download/v5.4.21/BepInEx_x64_5.4.21.0.zip)

* 下载 [你需要的Mod](https://github.com/shabbywu/GameMod/releases) 到 `$(TheLastSpellDirectory)/BepInEx/plugins`
    * 如果 `$(TheLastSpellDirectory)/BepInEx/plugins` 目录不存在, 手动创建即可

## Mod 介绍
### UnlimitedReroll(无限重投)
使用该 Mod 后, 游戏内的刷新按钮将可以执行多次(或无限次, 取决于你的配置), 以下是该 Mod 提供的功能
- 升级后将可无限刷新加点选项(不减少刷新次数)
- 禁用商店刷新价格上涨
- 商店0刷新消耗(由于过于影响平衡性默认不启用, 可以通过配置项启用该功能)
- 增加每晚战利品刷新次数(默认配置额外增加 10 次刷新次数)

### MultipleGain(多倍收益)
使用该 Mod 后, 可根据配置提高每晚的收益(经验和污秽精华)

## FAQ
1. 如何开发新的 Mod
```bash
dotnet new bepinex5plugin -T netstandard2.0 -U 2018.4.36 -n {Your-Mod-Name}
```

## [觅长生](https://store.steampowered.com/app/1189490/)
本项目所有 Mod 均基于 BepInEx 框架开发. [如何安装 BepInEx 框架](https://docs.bepinex.dev/master/articles/user_guide/installation/index.html)

## Mod 介绍
### BattleGains(提高战斗收益)

调整战斗胜利后的收益, 例如物品掉落倍率, 金钱掉落倍率, 装备掉落倍率。
- minor: 降低默认倍率至 2, 任务物品、丹方等类型的物品只掉落1份。

### BetterShoppingExperience(更好的商店体验)

支持使用 W/S/上/下 切换交易对象

### CollectGains(提高采集收益)

调整采集的时间耗时, 1个月的时间调整成1日, 1年的时间调整成1个月

### InstantlyForgeAndRefine(瞬间炼丹、炼器）

调整炼丹、炼器的耗时为0

更新: 1.0.0:
- 修复由于新版的炼丹界面重构导致的挂载点失效的问题

### ShoterLearnTime(缩短学习时间)

调整学习、突破功法的耗时，可在配置中调整倍率
- fix: 修复稳定版本 0.9.1.130 后功能不可用的问题, 并增加一种根据悟性动态控制缩短倍率的方案

### WuDaoGains(悟道收益调整)

调整悟道的收益, 悟道点的获取倍率、降低感悟灵感的时间消耗、提高灵感提供的经验值

### ForgetWuDaoSkill(遗忘悟道技能)

可在学习悟道技能的界面中直接遗忘悟道技能。   
bugfix: 修复无法查看未达到领悟条件的技能信息(by https://github.com/Cherrysaber)

### FriendlyLianDan(更友好的炼丹体验)

提供更舒适的炼丹体验.
- 炼丹界面中展示 Hover 中的丹药的功效
- 自动计算所有炼丹丹方
- 仅展示可炼制的丹方
- 丹方按药草的价值排序
- 优化炼丹丹方展示逻辑, 延迟加载丹方列表, 减少资源开销。

更新: 1.0.0:
- 修复由于新版的炼丹界面重构导致的挂载点失效的问题
- 修改数据源, 不再直接读取 json 文件, 而是使用内置模块 JSONClass 完成数据加载.

### StrengthenDongfu(强化洞府)

提高在洞府中的修炼和灵田的效率。

### BetterTooltips(更好的 Tooltips 弹窗)

增加物品栏中的 Tooltips 弹窗的信息
- 展示药草可以炼制的丹药
- 展示药草的产地

## FAQ
1. 如何开发新的 Mod
```bash
dotnet new bepinex5plugin -T net46 -U 2018.4.36 -n {Your-Mod-Name}
```
