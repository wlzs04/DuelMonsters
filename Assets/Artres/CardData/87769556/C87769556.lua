local C87769556={}

function C87769556.InitInfo(card)
	card:SetCardTypeByString("魔法");
	card:SetName("魔术师的右手");
	card:SetMagicTypeByString("永续");
	card:SetEffectInfo("1回合1次，自己场上有魔法师族怪兽存在的场合，对方发动的魔法卡的效果无效并破坏。");
end

function C87769556.LaunchEffect(card)
	local changeLifeEffectProcess = CS.Assets.Script.Duel.EffectProcess.ChangeLifeEffectProcess(1000, CS.Assets.Script.Duel.ChangeLifeType.ReduceLifeType.Effect, card:GetDuelCardScript():GetOwner():GetOpponentPlayer())
	card:GetDuelCardScript():GetOwner():GetOpponentPlayer():AddEffectProcess(changeLifeEffectProcess);
end

return C87769556