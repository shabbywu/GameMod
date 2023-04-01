# 联机对战 Mod

## RoadMap
- [ ] 实现开启对战功能
- [ ] 实现联机功能
    - [ ] 实现战斗房间
    - [ ] JSON 对象状态同步与复原
    - [ ] 对战核心(指令传输)
    - [ ] 战斗对话功能
- [ ] 实现对战录像与回放

## 觅长生游戏流程
- StartGame.cs 启动游戏
    - 加载主角至 KBEngineApp.app.player() (addAvatar -> GetValue<Avatar> -> initSkill......)
- 对局逻辑
    - RoundManager 控制对局流程
        - UseSkill 判断使用技能的条件, 实际调用 avatar.spell.spellSkill 使用技能
    - Spell 技能结算
        - spellSkill 进行技能结算
        - 数据源(jsonData.instance.skillJsonData)
    - GUIPackage.Skill 技能结算
        - Puting
            - RoundManager.FightTalk
            - onBuffTickByType 结算 skill 产生的 buff
            - attaker.fightTemp.UseSkill 标记技能使用记录
    - FightTempValue 接收对局操作
        - ResetRound 重置回合
        - UseSkill 使用技能
        - useAI 自动对局(依赖策略树)
    - UI 逻辑
        - UIFightSkillItem::ClickSkill 选中技能, 实际上调用了 RoundManager.SetChoiceSkill
        - UIFightWeaponItem::ClickSkill 释放武器技能, 实际上是设置了 RoundManager.ChoiceSkill 再触发 RoundManager.UseSkill



## 联机对战核心
### 对象设计
- 对局(Room)
- 事件循环(Servant)
    - RPC 连接处理
    - 对战逻辑处理
    - 延迟任务处理
        - 逃跑?



## 杂谈
对战模块参考 [游戏王](https://github.com/lllyasviel/YGOProUnity_V2)

- [主循环](https://github.com/lllyasviel/YGOProUnity_V2/blob/master/Assets/SibylSystem/Program.cs#L1016)
- [TCP 连接](https://github.com/lllyasviel/YGOProUnity_V2/blob/master/Assets/SibylSystem/MonoHelpers/TcpHelper.cs)
    - [发送](https://github.com/lllyasviel/YGOProUnity_V2/blob/master/Assets/SibylSystem/MonoHelpers/TcpHelper.cs#L215)
- [对局核心](https://github.com/lllyasviel/YGOProUnity_V2/blob/master/Assets/SibylSystem/Ocgcore/Ocgcore.cs#L981)
