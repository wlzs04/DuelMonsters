local C4178474={}

function C4178474.InitInfo(card)
	card:SetCardTypeByString("陷阱");
	card:SetName("雷破");
	card:SetMagicTypeByString("通常");
	card:SetEffectInfo("①：可以舍弃1张手牌，以场上的1张卡为对象发动。将那张卡破坏。");
end

function C4178474.CanLaunchEffect(card)
	return card:GetOwner():GetHandCards().Count>0 and card:GetDuelCardScript():GetDuelScene():GetCardNumberInArea()>1;
end

function C4178474.LaunchEffect(card)
	local discardHandCardEffectProcess = CS.Assets.Script.Duel.EffectProcess.DiscardHandCardEffectProcess(card,1,C4178474.AfterDiscardHandCard, card:GetOwner());
    card:GetOwner():AddEffectProcess(discardHandCardEffectProcess);
end

function C4178474.AfterDiscardHandCard(launchEffectCard,discardCard)
	local chooseCardEffectProcess = CS.Assets.Script.Duel.EffectProcess.ChooseCardEffectProcess(launchEffectCard,C4178474.ChooseCardJudgeAction,C4178474.ChooseCardCallback, launchEffectCard:GetOwner());
    chooseCardEffectProcess:SetTitle("请选择要破坏的卡牌！");
	launchEffectCard:GetOwner():AddEffectProcess(chooseCardEffectProcess);
end

function C4178474.ChooseCardJudgeAction(launchEffectCard,chooseCard)
	return chooseCard:IsInArea() and chooseCard ~=launchEffectCard;
end

function C4178474.ChooseCardCallback(launchEffectCard,chooseCard)
	if(not C4178474.ChooseCardJudgeAction(launchEffectCard,chooseCard))then
		return
	end
	
	local moveCardToTombEffectProcess = CS.Assets.Script.Duel.EffectProcess.MoveCardToTombEffectProcess(chooseCard,CS.Assets.Script.Duel.EffectProcess.MoveCardToTombType.Effect, chooseCard:GetOwner());
    chooseCard:GetOwner():AddEffectProcess(moveCardToTombEffectProcess);
end

return C4178474