local C46130346={}

function C46130346.InitInfo(card)
	card:SetCardTypeByString("魔法");
	card:SetName("火球");
	card:SetMagicTypeByString("通常");
	card:SetEffectInfo("给予对手LP500点伤害。");
end

function C46130346.LaunchEffect(card)
	local changeLifeEffectProcess = CS.Assets.Script.Duel.EffectProcess.ChangeLifeEffectProcess(-500, CS.Assets.Script.Duel.EffectProcess.ChangeLifeType.Effect, card:GetDuelCardScript():GetOwner():GetOpponentPlayer())
    card:GetDuelCardScript():GetOwner():GetOpponentPlayer():AddEffectProcess(changeLifeEffectProcess);
end

return C46130346