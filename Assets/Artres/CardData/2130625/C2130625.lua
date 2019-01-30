local C2130625={}

function C2130625.InitInfo(card)
	card:SetCardTypeByString("陷阱");
	card:SetName("白衣天使");
	card:SetTrapTypeByString("通常");
	card:SetEffectInfo("自己因战斗或者卡的效果受到伤害时可以发动。自己恢复1000LP。自己的墓地有「白衣の天使／白衣天使」存在的场合，再恢复那个张数×500LP。");
end

function C2130625.LaunchEffect(card)
	local reduceLifeEffectProcess = CS.Assets.Script.Duel.EffectProcess.ReduceLifeEffectProcess(500, CS.Assets.Script.Duel.EffectProcess.ReduceLifeType.Effect, card:GetDuelCardScript():GetOwner():GetOpponentPlayer())
    card:GetDuelCardScript():GetOwner():GetOpponentPlayer():AddEffectProcess(reduceLifeEffectProcess);
end

return C2130625