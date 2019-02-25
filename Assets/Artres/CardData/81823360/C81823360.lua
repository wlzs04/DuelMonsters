local C81823360={}

function C81823360.InitInfo(card)
	card:SetCardTypeByString("怪兽");
	card:SetName("巨噬者X");
	card:SetPropertyTypeByString("水");
	card:SetMonsterTypeByString("恐龙");

	card:SetLevel(4);
	card:SetOriginalAttackValue(2000);
	card:SetOriginalDefenseValue(0);
	card:SetEffectInfo("突然出现在太古大海原的恐龙型仿生体。虽然可以凭借其引以为傲的消音装甲，偷偷地接近猎物的背后，悄无声息地将其咬住，但进入捕食模式后身体会发光，因此常常让猎物逃走。");
end

function C81823360.LaunchEffect(card)
	local reduceLifeEffectProcess = CS.Assets.Script.Duel.EffectProcess.ReduceLifeEffectProcess(500, CS.Assets.Script.Duel.EffectProcess.ReduceLifeType.Effect, card:GetDuelCardScript():GetOwner():GetOpponentPlayer())
    card:GetDuelCardScript():GetOwner():GetOpponentPlayer():AddEffectProcess(reduceLifeEffectProcess);
end

return C81823360