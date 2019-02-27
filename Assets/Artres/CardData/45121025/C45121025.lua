local C45121025={}

function C45121025.InitInfo(card)
	card:SetCardTypeByString("怪兽");
	card:SetName("黑影鬼王");
	card:SetPropertyTypeByString("地");
	card:SetMonsterTypeByString("兽战士");

	card:SetLevel(4);
	card:SetOriginalAttackValue(1200);
	card:SetOriginalDefenseValue(1400);
	card:SetEffectInfo("被黑影附体的恶鬼。用惊人的速度突击。");
end

function C45121025.LaunchEffect(card)
	local changeLifeEffectProcess = CS.Assets.Script.Duel.EffectProcess.ChangeLifeEffectProcess(500, CS.Assets.Script.Duel.EffectProcess.ChangeLifeType.Effect, card:GetDuelCardScript():GetOwner():GetOpponentPlayer())
    card:GetDuelCardScript():GetOwner():GetOpponentPlayer():AddEffectProcess(changeLifeEffectProcess);
end

return C45121025