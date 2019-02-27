local C45159319={}

function C45159319.InitInfo(card)
	card:SetCardTypeByString("怪兽");
	card:SetName("石像迎击炮");
	card:SetPropertyTypeByString("地");
	card:SetMonsterTypeByString("岩石");

	card:SetLevel(4);
	card:SetOriginalAttackValue(1100);
	card:SetOriginalDefenseValue(2000);
	card:SetEffectInfo("这张卡1个回合1次可以变成里侧守备表示。");
end

function C45159319.LaunchEffect(card)
	local changeLifeEffectProcess = CS.Assets.Script.Duel.EffectProcess.ChangeLifeEffectProcess(500, CS.Assets.Script.Duel.EffectProcess.ChangeLifeType.Effect, card:GetDuelCardScript():GetOwner():GetOpponentPlayer())
    card:GetDuelCardScript():GetOwner():GetOpponentPlayer():AddEffectProcess(changeLifeEffectProcess);
end

return C45159319