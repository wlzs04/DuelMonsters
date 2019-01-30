local C83764718={}

function C83764718.InitInfo(card)
	card:SetCardTypeByString("魔法");
	card:SetName("死者苏生");
	card:SetMagicTypeByString("通常");
	card:SetEffectInfo("①：可以以自己或者对手墓地的1只怪兽为对象发动。将那只怪兽特殊召唤到自己的场上。");
end

function C83764718.LaunchEffect(card)
	local reduceLifeEffectProcess = CS.Assets.Script.Duel.EffectProcess.ReduceLifeEffectProcess(500, CS.Assets.Script.Duel.EffectProcess.ReduceLifeType.Effect, card:GetDuelCardScript():GetOwner():GetOpponentPlayer())
    card:GetDuelCardScript():GetOwner():GetOpponentPlayer():AddEffectProcess(reduceLifeEffectProcess);
end

return C83764718