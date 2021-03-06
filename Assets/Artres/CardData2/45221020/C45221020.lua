local C45221020={}

function C45221020.InitInfo(card)
	card:SetCardTypeByString("怪兽");
	card:SetName("寻宝熊猫");
	card:SetPropertyTypeByString("地");
	card:SetMonsterTypeByString("兽");

	card:SetLevel(4);
	card:SetOriginalAttackValue(1100);
	card:SetOriginalDefenseValue(2000);
	card:SetEffectInfo("从自己墓地把最多3张魔法·陷阱卡里侧表示除外才能发动。和除外的卡数量相同等级的1只通常怪兽从卡组特殊召唤。");
end

function C45221020.LaunchEffect(card)
	local changeLifeEffectProcess = CS.Assets.Script.Duel.EffectProcess.ChangeLifeEffectProcess(500, CS.Assets.Script.Duel.EffectProcess.ChangeLifeType.Effect, card:GetDuelCardScript():GetOwner():GetOpponentPlayer())
    card:GetDuelCardScript():GetOwner():GetOpponentPlayer():AddEffectProcess(changeLifeEffectProcess);
end

return C45221020