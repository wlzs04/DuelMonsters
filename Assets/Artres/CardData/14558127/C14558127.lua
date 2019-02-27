local C14558127={}

function C14558127.InitInfo(card)
	card:SetCardTypeByString("怪兽");
	card:SetName("灰流丽");
	card:SetPropertyTypeByString("炎");
	card:SetMonsterTypeByString("不死");

	card:SetLevel(3);
	card:SetOriginalAttackValue(0);
	card:SetOriginalDefenseValue(1800);
	card:SetEffectInfo("「灰流丽」的效果1回合只能使用1次。①：包含以下其中任意种效果的魔法·陷阱·怪兽的效果发动时，把这张卡从手卡丢弃才能发动。那个效果无效。这个效果在对方回合也能发动。●从卡组把卡加入手卡的效果●从卡组把怪兽特殊召唤的效果●从卡组把卡送去墓地的效果。");
end

function C14558127.LaunchEffect(card)
	local changeLifeEffectProcess = CS.Assets.Script.Duel.EffectProcess.ChangeLifeEffectProcess(500, CS.Assets.Script.Duel.EffectProcess.ChangeLifeType.Effect, card:GetDuelCardScript():GetOwner():GetOpponentPlayer())
    card:GetDuelCardScript():GetOwner():GetOpponentPlayer():AddEffectProcess(changeLifeEffectProcess);
end

return C14558127