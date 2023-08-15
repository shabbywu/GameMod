echo "build all mod in TheLastSpellMod"
dotnet build TheLastSpellMod/UnlimitedReroll -o bin/TheLastSpellMod --configuration Release
dotnet build TheLastSpellMod/MultipleGain -o bin/TheLastSpellMod --configuration Release

echo "build all mod in michangsheng"
dotnet build michangsheng/BattleGains -o bin/michangsheng --configuration Release
dotnet build michangsheng/WuDaoGains -o bin/michangsheng --configuration Release
dotnet build michangsheng/ShoterLearnTime -o bin/michangsheng --configuration Release
dotnet build michangsheng/BetterShoppingExperience -o bin/michangsheng --configuration Release
dotnet build michangsheng/InstantlyForgeAndRefine -o bin/michangsheng --configuration Release
dotnet build michangsheng/CollectGains -o bin/michangsheng --configuration Release
dotnet build michangsheng/ForgetWuDaoSkill -o bin/michangsheng --configuration Release
dotnet build michangsheng/FriendlyLianDan -o bin/michangsheng --configuration Release
dotnet build michangsheng/StrengthenDongfu -o bin/michangsheng --configuration Release
dotnet build michangsheng/BetterTooltips -o bin/michangsheng --configuration Release

echo "build all mod in Skul.Mod"
dotnet build Skul.Mod/MultipleGain -o bin/Skul.Mod --configuration Release
