local C20155904={}

function C20155904.InitInfo(card)
	card:SetCardTypeByString("怪兽");
	card:SetName("十二兽 鸡拳");
	card:SetPropertyTypeByString("地");
	card:SetMonsterTypeByString("兽战士");

	card:SetLevel(4)
	card:SetAttackValue(800)
	card:SetDefenseValue(1200)
	card:SetEffectInfo("①：这张卡被战斗·效果破坏的场合，以「十二兽 鸡拳」以外的自己墓地1张「十二兽」卡为对象才能发动。那张卡回到卡组。②：持有这张卡作为素材中的原本种族是兽战士族的XYZ怪兽得到以下效果。");
end

function C20155904.LaunchEffect(card)
	local reduceLifeEffectProcess = CS.Assets.Script.Duel.EffectProcess.ReduceLifeEffectProcess(500, CS.Assets.Script.Duel.EffectProcess.ReduceLifeType.Effect, card:GetDuelCardScript():GetOwner():GetOpponentPlayer())
    card:GetDuelCardScript():GetOwner():GetOpponentPlayer():AddEffectProcess(reduceLifeEffectProcess);
end

return C20155904