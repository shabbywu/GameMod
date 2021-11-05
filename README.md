# TheLastSpellMod

本项目所有 Mod 均基于 BepInEx 框架开发. [如何安装 BepInEx 框架](https://docs.bepinex.dev/master/articles/user_guide/installation/index.html)

# Mod 介绍
## UnlimitedReroll(无限重投)
使用该 Mod 后, 升级后将可无限刷新加点选项.

## MultipleGain(多倍收益)
使用该 Mod 后, 可根据配置提高每晚的收益(经验和污秽精华)


# FAQ
1. 如何开发新的 Mod
```bash
dotnet new bepinex5plugin -T netstandard2.0 -U 2018.4.36 -n MultipleExperience
```