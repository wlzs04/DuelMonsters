local C58984738={}

function C58984738.InitInfo(card)
	card:SetCardTypeByString("怪兽");
	card:SetName("真龙拳士 雾动轰·铁拳");
	card:SetPropertyTypeByString("水");
	card:SetMonsterTypeByString("幻龙");

	card:SetLevel(6);
	card:SetOriginalAttackValue(2500);
	card:SetOriginalDefenseValue(1200);
	card:SetEffectInfo("这张卡表侧表示上级召唤的场合，可以作为怪兽的代替而把自己场上的永续魔法·永续陷阱卡解放。①：1回合1次，上级召唤的这张卡存在，对方把魔法·陷阱·怪兽的效果发动时才能发动。从卡组选1张「真龙」永续陷阱卡加入手卡或在自己场上发动。");
end

function C58984738.LaunchEffect(card)
	local reduceLifeEffectProcess = CS.Assets.Script.Duel.EffectProcess.ReduceLifeEffectProcess(500, CS.Assets.Script.Duel.EffectProcess.ReduceLifeType.Effect, card:GetDuelCardScript():GetOwner():GetOpponentPlayer())
    card:GetDuelCardScript():GetOwner():GetOpponentPlayer():AddEffectProcess(reduceLifeEffectProcess);
end

return C58984738