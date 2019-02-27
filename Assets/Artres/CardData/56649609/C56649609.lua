local C52589809={}

function C52589809.InitInfo(card)
	card:SetCardTypeByString("怪兽");
	card:SetName("幻煌龙 螺旋");
	card:SetPropertyTypeByString("水");
	card:SetMonsterTypeByString("幻龙");

	card:SetLevel(8);
	card:SetOriginalAttackValue(2900);
	card:SetOriginalDefenseValue(2900);
	card:SetEffectInfo("历经炽烈战涡的猛龙。其负伤的身躯接触古老之光达成浸涡。随即，那龙展开双翅，成为制霸天涡的煌者。那全新的煌，不知是梦是真还是幻。");
end

function C52589809.LaunchEffect(card)
	local changeLifeEffectProcess = CS.Assets.Script.Duel.EffectProcess.ChangeLifeEffectProcess(500, CS.Assets.Script.Duel.EffectProcess.ChangeLifeType.Effect, card:GetDuelCardScript():GetOwner():GetOpponentPlayer())
    card:GetDuelCardScript():GetOwner():GetOpponentPlayer():AddEffectProcess(changeLifeEffectProcess);
end

return C52589809