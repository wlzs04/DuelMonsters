local C2370081={}

function C2370081.InitInfo(card)
	card:SetCardTypeByString("魔法");
	card:SetName("钢之甲壳");
	card:SetMagicTypeByString("装备");
	card:SetEffectInfo("只有水属性怪兽才能装备。装备怪兽的攻击力上升400点，守备力下降200点。");
end

function C2370081.LaunchEffect(card)
	local reduceLifeEffectProcess = CS.Assets.Script.Duel.EffectProcess.ReduceLifeEffectProcess(500, CS.Assets.Script.Duel.EffectProcess.ReduceLifeType.Effect, card:GetDuelCardScript():GetOwner():GetOpponentPlayer())
    card:GetDuelCardScript():GetOwner():GetOpponentPlayer():AddEffectProcess(reduceLifeEffectProcess);
end

return C2370081