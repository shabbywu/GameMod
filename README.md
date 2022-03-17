# 游戏Mod
## [最后的咒语](https://store.steampowered.com/app/1105670/)
本项目所有 Mod 均基于 BepInEx 框架开发. [如何安装 BepInEx 框架](https://docs.bepinex.dev/master/articles/user_guide/installation/index.html)

## Mod 介绍
### UnlimitedReroll(无限重投)
使用该 Mod 后, 升级后将可无限刷新加点选项.

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
